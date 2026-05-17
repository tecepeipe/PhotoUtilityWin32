namespace PhotoUtilityWin32
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGroupAnalyze;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.TreeView treeViewGroups;
        private System.Windows.Forms.Button btnDeleteDuplicates;
        private System.Windows.Forms.Button btnRenameGroup;
        private System.Windows.Forms.Button btnMergeGroups;
        private System.Windows.Forms.ComboBox cmbMergeTarget;
        private System.Windows.Forms.Label lblMergeTarget;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCommitChanges;
        private System.Windows.Forms.PictureBox picturePreview;
        private System.Windows.Forms.ProgressBar pbOperations;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGroupAnalyze = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.treeViewGroups = new System.Windows.Forms.TreeView();
            this.btnDeleteDuplicates = new System.Windows.Forms.Button();
            this.btnRenameGroup = new System.Windows.Forms.Button();
            this.btnMergeGroups = new System.Windows.Forms.Button();
            this.cmbMergeTarget = new System.Windows.Forms.ComboBox();
            this.lblMergeTarget = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnCommitChanges = new System.Windows.Forms.Button();
            this.picturePreview = new System.Windows.Forms.PictureBox();
            this.pbOperations = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFolder.Location = new System.Drawing.Point(589, 10);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(94, 24);
            this.btnSelectFolder.TabIndex = 0;
            this.btnSelectFolder.Text = "Select Folder";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(84, 14);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(499, 20);
            this.txtFolder.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Folder:";
            // 
            // btnGroupAnalyze
            // 
            this.btnGroupAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGroupAnalyze.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGroupAnalyze.Location = new System.Drawing.Point(19, 50);
            this.btnGroupAnalyze.Name = "btnGroupAnalyze";
            this.btnGroupAnalyze.Size = new System.Drawing.Size(120, 24);
            this.btnGroupAnalyze.TabIndex = 3;
            this.btnGroupAnalyze.Text = "Analyze";
            this.btnGroupAnalyze.UseVisualStyleBackColor = true;
            this.btnGroupAnalyze.Click += new System.EventHandler(this.btnGroupAnalyze_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(689, 11);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(94, 24);
            this.btnAbort.TabIndex = 7;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // treeViewGroups
            // 
            this.treeViewGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewGroups.HideSelection = false;
            this.treeViewGroups.Location = new System.Drawing.Point(19, 89);
            this.treeViewGroups.Name = "treeViewGroups";
            this.treeViewGroups.Size = new System.Drawing.Size(284, 425);
            this.treeViewGroups.TabIndex = 4;
            this.treeViewGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewGroups_AfterSelect);
            // 
            // btnDeleteDuplicates
            // 
            this.btnDeleteDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteDuplicates.Location = new System.Drawing.Point(158, 49);
            this.btnDeleteDuplicates.Name = "btnDeleteDuplicates";
            this.btnDeleteDuplicates.Size = new System.Drawing.Size(129, 24);
            this.btnDeleteDuplicates.TabIndex = 5;
            this.btnDeleteDuplicates.Text = "Delete All Duplicates";
            this.btnDeleteDuplicates.UseVisualStyleBackColor = true;
            this.btnDeleteDuplicates.Click += new System.EventHandler(this.btnDeleteDuplicates_Click);
            // 
            // btnRenameGroup
            // 
            this.btnRenameGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRenameGroup.Location = new System.Drawing.Point(312, 49);
            this.btnRenameGroup.Name = "btnRenameGroup";
            this.btnRenameGroup.Size = new System.Drawing.Size(103, 24);
            this.btnRenameGroup.TabIndex = 6;
            this.btnRenameGroup.Text = "Rename Group";
            this.btnRenameGroup.UseVisualStyleBackColor = true;
            this.btnRenameGroup.Click += new System.EventHandler(this.btnRenameGroup_Click);
            // 
            // btnMergeGroups
            // 
            this.btnMergeGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMergeGroups.Location = new System.Drawing.Point(673, 50);
            this.btnMergeGroups.Name = "btnMergeGroups";
            this.btnMergeGroups.Size = new System.Drawing.Size(109, 24);
            this.btnMergeGroups.TabIndex = 7;
            this.btnMergeGroups.Text = "Merge this Group";
            this.btnMergeGroups.UseVisualStyleBackColor = true;
            this.btnMergeGroups.Click += new System.EventHandler(this.btnMergeGroups_Click);
            // 
            // cmbMergeTarget
            // 
            this.cmbMergeTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMergeTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMergeTarget.FormattingEnabled = true;
            this.cmbMergeTarget.Location = new System.Drawing.Point(484, 52);
            this.cmbMergeTarget.Name = "cmbMergeTarget";
            this.cmbMergeTarget.Size = new System.Drawing.Size(183, 21);
            this.cmbMergeTarget.TabIndex = 8;
            // 
            // lblMergeTarget
            // 
            this.lblMergeTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMergeTarget.AutoSize = true;
            this.lblMergeTarget.Location = new System.Drawing.Point(421, 55);
            this.lblMergeTarget.Name = "lblMergeTarget";
            this.lblMergeTarget.Size = new System.Drawing.Size(60, 13);
            this.lblMergeTarget.TabIndex = 9;
            this.lblMergeTarget.Text = "Merge into:";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(16, 526);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(287, 20);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.Text = "Ready.";
            // 
            // btnCommitChanges
            // 
            this.btnCommitChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommitChanges.Location = new System.Drawing.Point(644, 526);
            this.btnCommitChanges.Name = "btnCommitChanges";
            this.btnCommitChanges.Size = new System.Drawing.Size(117, 24);
            this.btnCommitChanges.TabIndex = 11;
            this.btnCommitChanges.Text = "Move to Folders";
            this.btnCommitChanges.UseVisualStyleBackColor = true;
            this.btnCommitChanges.Click += new System.EventHandler(this.btnCommitChanges_Click);
            // 
            // picturePreview
            // 
            this.picturePreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picturePreview.Location = new System.Drawing.Point(324, 89);
            this.picturePreview.Name = "picturePreview";
            this.picturePreview.Size = new System.Drawing.Size(437, 425);
            this.picturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picturePreview.TabIndex = 12;
            this.picturePreview.TabStop = false;
            // 
            // pbOperations
            // 
            this.pbOperations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbOperations.Location = new System.Drawing.Point(324, 526);
            this.pbOperations.Name = "pbOperations";
            this.pbOperations.Size = new System.Drawing.Size(298, 17);
            this.pbOperations.TabIndex = 13;
            this.pbOperations.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 555);
            this.Controls.Add(this.picturePreview);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblMergeTarget);
            this.Controls.Add(this.cmbMergeTarget);
            this.Controls.Add(this.btnMergeGroups);
            this.Controls.Add(this.btnRenameGroup);
            this.Controls.Add(this.btnDeleteDuplicates);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.treeViewGroups);
            this.Controls.Add(this.btnGroupAnalyze);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.btnCommitChanges);
            this.Controls.Add(this.pbOperations);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Photo Utility Win32 - Intelligent Grouping";
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
