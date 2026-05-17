using System;
using System.Drawing;
using System.IO;

namespace PhotoUtilityWin32
{
    /// <summary>
    /// Represents a photo item with metadata and classification information.
    /// Stores file information, EXIF date, dimensions, and perceptual hash for duplicate detection.
    /// </summary>
    public class PhotoItem
    {
        public string FilePath { get; set; }
        public DateTime DateTaken { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long FileSize { get; set; }
        public Bitmap Thumbnail { get; set; }
        // Perceptual hash used for visual similarity comparison
        public ulong Hash { get; set; }
        // Flag indicating whether this photo is a duplicate of another
        public bool IsDuplicate { get; set; }

        public override string ToString()
        {
            // Display only the filename without path for UI representation
            return Path.GetFileName(FilePath);
        }
    }
}