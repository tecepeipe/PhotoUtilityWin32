using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoUtilityWin32
{
    public class PhotoGroup
    {
        public List<PhotoItem> Items { get; set; } = new List<PhotoItem>();
        public List<PhotoCluster> Clusters { get; set; } = new List<PhotoCluster>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SuggestedLabel { get; set; }
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets all duplicate candidates from clusters, excluding the best item from each cluster.
        /// </summary>
        public IEnumerable<PhotoItem> GetDuplicateCandidates()
        {
            // Flatten all items from clusters and exclude the best (highest quality) item from each cluster
            return Clusters.SelectMany(cluster => cluster.Items.Except(new[] { cluster.BestItem }));
        }

        public List<PhotoItem> Photos => Items;

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class PhotoCluster
    {
        public List<PhotoItem> Items { get; set; } = new List<PhotoItem>();

        /// <summary>
        /// Gets the best item in the cluster based on file size and dimensions.
        /// Selects the photo with the largest file size and highest resolution.
        /// </summary>
        public PhotoItem BestItem => Items
            // Order by file size descending, then by total pixels (width * height) descending
            .OrderByDescending(item => item.FileSize)
            .ThenByDescending(item => item.Width * item.Height)
            .FirstOrDefault();
    }
}
