using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoUtilityWin32
{
    public partial class MainForm : Form
    {
        private readonly OnnxClassifier _classifier;
        private List<PhotoItem> _allPhotos = new List<PhotoItem>();
        private List<PhotoGroup> _groups = new List<PhotoGroup>();
        private const int TemporalThresholdHours = 5; // Threshold in hours to separate groups based on time
        private System.Threading.CancellationTokenSource _cancellationTokenSource;

        public MainForm()
        {
            InitializeComponent();
            var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "yolo26n-cls.onnx");
            var labelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "imagenet_classes.txt");
            _classifier = new OnnxClassifier(modelPath, labelPath);
            cmbMergeTarget.DisplayMember = nameof(PhotoGroup.DisplayName);
            btnGroupAnalyze.Enabled = false;
            btnDeleteDuplicates.Enabled = false;
            btnRenameGroup.Enabled = false;
            btnMergeGroups.Enabled = false;
        }

        private async void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog { Description = "Select a folder containing photos" };
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtFolder.Text = dialog.SelectedPath;
            lblStatus.Text = "Scanning folder metadata...";
            pbOperations.Visible = true;
            pbOperations.Style = ProgressBarStyle.Continuous;
            pbOperations.Value = 0;
            treeViewGroups.Nodes.Clear();
            cmbMergeTarget.Items.Clear();
            btnGroupAnalyze.Enabled = false;
            btnDeleteDuplicates.Enabled = false;
            btnRenameGroup.Enabled = false;
            btnMergeGroups.Enabled = false;

            try
            {
                // Get all supported image files first to calculate progress
                var files = Directory.EnumerateFiles(dialog.SelectedPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff" }.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                    .ToList();
                
                var totalFiles = files.Count;
                var processedFiles = 0;

                // Scan folder with progress updates
                _allPhotos = await Task.Run(() => ExifScanner.ScanFolder(dialog.SelectedPath));
                
                // Update progress as we process
                foreach (var photo in _allPhotos)
                {
                    processedFiles++;
                    pbOperations.Value = (int)((processedFiles / (double)totalFiles) * 100);
                    Application.DoEvents();
                }

                pbOperations.Value = 100;
                lblStatus.Text = $"Metadata scan complete: {_allPhotos.Count} photos found.";
                btnGroupAnalyze.Enabled = _allPhotos.Any();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Scan failed.";
            }
            finally
            {
                pbOperations.Visible = false;
                pbOperations.Value = 0;
            }
        }

        private async void btnGroupAnalyze_Click(object sender, EventArgs e)
        {
            if (!_allPhotos.Any())
            {
                return;
            }

            // Create cancellation token source for this operation
            _cancellationTokenSource = new CancellationTokenSource();

            // Disable buttons to prevent user interaction during analysis
            btnGroupAnalyze.Enabled = false;
            btnSelectFolder.Enabled = false;
            btnAbort.Enabled = true;
            lblStatus.Text = "Grouping photos and analyzing content...";
            pbOperations.Visible = true;
            pbOperations.Style = ProgressBarStyle.Continuous;
            pbOperations.Value = 0;

            try
            {
                var totalPhotos = _allPhotos.Count;

                // Analyze photos and group them based on temporal and visual similarity
                _groups = await Task.Run(() => AnalyzeGroups(_allPhotos, _cancellationTokenSource.Token), _cancellationTokenSource.Token);

                // Check if operation was cancelled
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    lblStatus.Text = "Analysis cancelled.";
                    _groups.Clear();
                    RefreshGroupView();
                    return;
                }

                // Simulate progress updates during analysis
                for (int i = 0; i <= 100; i++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        lblStatus.Text = "Analysis cancelled.";
                        _groups.Clear();
                        RefreshGroupView();
                        return;
                    }
                    pbOperations.Value = i;
                    await Task.Delay(10);
                }

                // Refresh the TreeView and merge target dropdown to reflect the new groups
                RefreshGroupView();
                RefreshMergeTargets();

                pbOperations.Value = 100;
                lblStatus.Text = $"Analysis complete: {_groups.Count} event group(s) created.";

                // Enable buttons for further actions
                btnDeleteDuplicates.Enabled = true;
                btnRenameGroup.Enabled = true;
                btnMergeGroups.Enabled = true;
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully
                lblStatus.Text = "Analysis cancelled by user.";
                _groups.Clear();
                RefreshGroupView();
            }
            catch (Exception ex)
            {
                // Display error message if analysis fails
                MessageBox.Show(this, ex.Message, "Analysis Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Analysis failed.";
            }
            finally
            {
                // Re-enable buttons after analysis
                pbOperations.Visible = false;
                pbOperations.Value = 0;
                btnGroupAnalyze.Enabled = true;
                btnSelectFolder.Enabled = true;
                btnAbort.Enabled = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            // Request cancellation of the current operation
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                btnAbort.Enabled = false;
                lblStatus.Text = "Cancelling operation...";
            }
        }

        private void btnRenameGroup_Click(object sender, EventArgs e)
        {
            if (treeViewGroups.SelectedNode?.Tag is not PhotoGroup group)
            {
                MessageBox.Show(this, "Select an event group to rename.", "Rename Group", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var newName = Prompt.ShowDialog("Enter a new name for the selected group:", "Rename Group", group.DisplayName);
            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            group.DisplayName = newName.Trim();
            treeViewGroups.SelectedNode.Text = group.DisplayName;
            RefreshMergeTargets();
        }

        private void btnMergeGroups_Click(object sender, EventArgs e)
        {
            if (treeViewGroups.SelectedNode?.Tag is not PhotoGroup sourceGroup)
            {
                MessageBox.Show(this, "Select a source group in the tree view first.", "Merge Groups", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cmbMergeTarget.SelectedItem is not PhotoGroup targetGroup)
            {
                MessageBox.Show(this, "Choose a target group to merge into.", "Merge Groups", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ReferenceEquals(sourceGroup, targetGroup))
            {
                MessageBox.Show(this, "Please choose a different group to merge into.", "Merge Groups", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(this, $"Merge group '{sourceGroup.DisplayName}' into '{targetGroup.DisplayName}'?", "Merge Groups", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            var mergedItems = sourceGroup.Items.Concat(targetGroup.Items).OrderBy(item => item.DateTaken).ToList();
            var mergedGroup = new PhotoGroup
            {
                Items = mergedItems,
                StartTime = mergedItems.First().DateTaken,
                EndTime = mergedItems.Last().DateTaken,
                SuggestedLabel = targetGroup.SuggestedLabel ?? sourceGroup.SuggestedLabel ?? "Merged Event",
                DisplayName = targetGroup.DisplayName
            };

            _groups.Remove(sourceGroup);
            _groups.Remove(targetGroup);
            mergedGroup.Clusters = BuildClusters(mergedGroup);
            _groups.Add(mergedGroup);
            RefreshGroupView();
            RefreshMergeTargets(); // Update cmbMergeTarget after merging groups
            lblStatus.Text = "Groups merged successfully.";
        }

        private void btnDeleteDuplicates_Click(object sender, EventArgs e)
        {
            var duplicateCandidates = _groups.SelectMany(g => g.GetDuplicateCandidates()).ToList();
            if (!duplicateCandidates.Any())
            {
                MessageBox.Show(this, "No duplicate candidates were identified.", "Delete Duplicates", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(this, $"Delete {duplicateCandidates.Count} duplicate file(s)? This removes every duplicate except the highest quality image in each cluster.", "Delete Duplicates", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            var deletedCount = 0;
            foreach (var duplicate in duplicateCandidates)
            {
                try
                {
                    if (File.Exists(duplicate.FilePath))
                    {
                        File.Delete(duplicate.FilePath);
                        deletedCount++;
                    }
                }
                catch
                {
                    // Continue deleting others even if one fails.
                }
            }

            _allPhotos = _allPhotos.Where(photo => duplicateCandidates.All(dup => !string.Equals(dup.FilePath, photo.FilePath, StringComparison.OrdinalIgnoreCase))).ToList();
            _groups = AnalyzeGroups(_allPhotos);
            RefreshGroupView();
            lblStatus.Text = $"Deleted {deletedCount} duplicate file(s).";
        }

        private async void btnCommitChanges_Click(object sender, EventArgs e)
        {
            // Show confirmation dialog before committing changes
            var confirm = MessageBox.Show(this, "Move all photos to their classified folders?", "Commit Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            btnCommitChanges.Enabled = false;
            lblStatus.Text = "Moving photos to folders...";
            pbOperations.Visible = true;
            pbOperations.Style = ProgressBarStyle.Continuous;
            pbOperations.Value = 0;

            try
            {
                // Calculate total items to process
                var totalItems = _groups.Sum(g => g.Photos.Count);
                var processedItems = 0;

                // Process each group and move photos to classified folders
                foreach (var group in _groups)
                {
                    foreach (var photo in group.Photos)
                    {
                        // Classify the photo and determine target folder
                        var coreElements = _classifier.Classify(photo);
                        var targetFolder = Path.Combine(txtFolder.Text, group.DisplayName + "_" + coreElements);

                        // Create target folder if it doesn't exist
                        if (!Directory.Exists(targetFolder))
                        {
                            Directory.CreateDirectory(targetFolder);
                        }

                        // Move the photo file to the target folder
                        var targetPath = Path.Combine(targetFolder, Path.GetFileName(photo.FilePath));
                        if (File.Exists(photo.FilePath))
                        {
                            // Delete target if it exists and then move
                            if (File.Exists(targetPath))
                            {
                                File.Delete(targetPath);
                            }
                            File.Move(photo.FilePath, targetPath);
                        }

                        // Update progress bar
                        processedItems++;
                        pbOperations.Value = (int)((processedItems / (double)totalItems) * 100);
                        Application.DoEvents();
                    }
                }

                pbOperations.Value = 100;
                MessageBox.Show(this, "Changes committed successfully.", "Commit Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Changes committed.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error during commit: {ex.Message}", "Commit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Commit failed.";
            }
            finally
            {
                pbOperations.Visible = false;
                pbOperations.Value = 0;
                btnCommitChanges.Enabled = true;
            }
        }

        private List<PhotoGroup> AnalyzeGroups(List<PhotoItem> sourcePhotos, CancellationToken cancellationToken = default)
        {
            var groups = new List<PhotoGroup>();

            // Order photos by the date they were taken
            var ordered = sourcePhotos.OrderBy(photo => photo.DateTaken).ToList();
            if (!ordered.Any())
            {
                return groups;
            }

            // Initialize the first group with the earliest photo
            var threshold = TimeSpan.FromHours(TemporalThresholdHours);
            var currentGroup = new PhotoGroup
            {
                Items = new List<PhotoItem> { ordered[0] },
                StartTime = ordered[0].DateTaken,
                EndTime = ordered[0].DateTaken
            };

            // Iterate through the photos and group them based on the time threshold
            for (var index = 1; index < ordered.Count; index++)
            {
                // Check for cancellation request
                cancellationToken.ThrowIfCancellationRequested();

                var photo = ordered[index];

                // Check if the photo falls within the current group's time threshold
                if (photo.DateTaken - currentGroup.EndTime <= threshold)
                {
                    currentGroup.Items.Add(photo);
                    currentGroup.EndTime = photo.DateTaken;
                }
                else
                {
                    // Finalize the current group and start a new one
                    currentGroup.Clusters = BuildClusters(currentGroup, cancellationToken);
                    groups.Add(currentGroup);
                    currentGroup = new PhotoGroup
                    {
                        Items = new List<PhotoItem> { photo },
                        StartTime = photo.DateTaken,
                        EndTime = photo.DateTaken
                    };
                }
            }

            // Add the last group to the list
            cancellationToken.ThrowIfCancellationRequested();
            currentGroup.Clusters = BuildClusters(currentGroup, cancellationToken);
            groups.Add(currentGroup);

            // Assign labels and display names to each group
            foreach (var group in groups)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var label = _classifier.Classify(group.Items.First());
                group.SuggestedLabel = string.IsNullOrWhiteSpace(label) ? "Photo Event" : label;
                group.DisplayName = $"{group.SuggestedLabel} - {group.StartTime:MMM yyyy}";
            }

            return groups;
        }

        private List<PhotoCluster> BuildClusters(PhotoGroup group, CancellationToken cancellationToken = default)
        {
            // Compute thumbnails and perceptual hashes for all photos in the group
            foreach (var item in group.Items)
            {
                // Check for cancellation request
                cancellationToken.ThrowIfCancellationRequested();

                if (item.Thumbnail == null)
                {
                    item.Thumbnail = ExifScanner.LoadThumbnail(item.FilePath, 256);
                }

                if (item.Hash == 0)
                {
                    item.Hash = Phash.ComputeHash(item.Thumbnail);
                }
            }

            var clusters = new List<PhotoCluster>();
            var remaining = new HashSet<PhotoItem>(group.Items);

            // Group photos based on visual similarity using perceptual hashing
            while (remaining.Any())
            {
                // Check for cancellation request
                cancellationToken.ThrowIfCancellationRequested();

                var seed = remaining.First();
                remaining.Remove(seed);
                var cluster = new PhotoCluster { Items = new List<PhotoItem> { seed } };
                var toCompare = remaining.ToList();

                foreach (var candidate in toCompare)
                {
                    // Calculate the Hamming distance between hashes to determine similarity
                    var distance = Phash.HammingDistance(seed.Hash, candidate.Hash);
                    if (distance <= 5)
                    {
                        cluster.Items.Add(candidate);
                        remaining.Remove(candidate);
                        candidate.IsDuplicate = true; // Mark as duplicate
                    }
                }

                clusters.Add(cluster);
            }

            return clusters;
        }

        private void RefreshGroupView()
        {
            treeViewGroups.Nodes.Clear();

            foreach (var group in _groups)
            {
                var groupNode = new TreeNode(group.DisplayName) { Tag = group };

                bool hasDuplicates = false;

                foreach (var photo in group.Items)
                {
                    var photoNode = new TreeNode(photo.ToString()) { Tag = photo };

                    if (photo.IsDuplicate)
                    {
                        photoNode.ForeColor = Color.Red;
                        hasDuplicates = true;
                    }

                    groupNode.Nodes.Add(photoNode);
                }

                // Set group node text color to red if it contains duplicates
                if (hasDuplicates)
                {
                    groupNode.ForeColor = Color.Red;
                }

                treeViewGroups.Nodes.Add(groupNode);
            }
        }

        private void RefreshMergeTargets()
        {
            cmbMergeTarget.Items.Clear();
            foreach (var group in _groups)
            {
                cmbMergeTarget.Items.Add(group);
            }
        }

        private void treeViewGroups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PhotoItem photo)
            {
                try
                {
                    picturePreview.Image = Image.FromFile(photo.FilePath);
                }
                catch
                {
                    picturePreview.Image = null;
                }
            }
            else
            {
                picturePreview.Image = null;
            }

            foreach (TreeNode node in treeViewGroups.Nodes)
            {
                if (node.Tag is PhotoItem item && item.IsDuplicate)
                {
                    node.ForeColor = Color.Red;
                }
                else
                {
                    node.ForeColor = Color.Black;
                }
            }
        }
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string defaultValue)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400, Text = defaultValue };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
        }
    }
}
