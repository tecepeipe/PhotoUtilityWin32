using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoUtilityWin32
{
    public static class ExifScanner
    {
        // Supported image file extensions (case-insensitive)
        private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp",
            ".tif",
            ".tiff"
        };

        /// <summary>
        /// Scans a folder recursively for supported image files and extracts metadata.
        /// Creates PhotoItem objects with EXIF date, dimensions, and file size information.
        /// </summary>
        public static List<PhotoItem> ScanFolder(string folderPath)
        {
            // Recursively find all files with supported image extensions
            var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => SupportedExtensions.Contains(Path.GetExtension(file)))
                .ToList();

            var photos = new List<PhotoItem>(files.Count);
            // Process each image file, skipping files that cannot be read
            foreach (var file in files)
            {
                try
                {
                    using var image = Image.FromFile(file);
                    // Try to read EXIF date, fall back to file's last write time
                    var dateTaken = ReadDateTaken(image) ?? File.GetLastWriteTime(file);
                    photos.Add(new PhotoItem
                    {
                        FilePath = file,
                        DateTaken = dateTaken,
                        Width = image.Width,
                        Height = image.Height,
                        FileSize = new FileInfo(file).Length
                    });
                }
                catch
                {
                    // Skip unreadable files and continue processing others
                }
            }

            return photos;
        }

        /// <summary>
        /// Reads the date taken from EXIF metadata if available.
        /// Checks DateTimeOriginal (0x9003) and DateTime (0x0132) properties.
        /// Returns null if no valid date is found.
        /// </summary>
        private static DateTime? ReadDateTaken(Image image)
        {
            // EXIF property IDs: 0x9003 = DateTimeOriginal, 0x0132 = DateTime
            var dateProperties = new[] { 0x9003, 0x0132 };
            foreach (var propertyId in dateProperties)
            {
                // Skip if image doesn't have this EXIF property
                if (!image.PropertyIdList.Contains(propertyId))
                {
                    continue;
                }

                try
                {
                    var prop = image.GetPropertyItem(propertyId);
                    // Convert EXIF byte array to string and parse as DateTime
                    var raw = System.Text.Encoding.ASCII.GetString(prop.Value).Trim('\0');
                    if (DateTime.TryParseExact(raw, "yyyy:MM:dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var parsed))
                    {
                        return parsed;
                    }
                }
                catch
                {
                    // Ignore invalid EXIF values and try next property
                }
            }

            return null;
        }

        /// <summary>
        /// Loads a thumbnail of the specified size from an image file using WPF decoder.
        /// Scales the image proportionally to fit within the size limit.
        /// </summary>
        public static Bitmap LoadThumbnail(string imagePath, int size)
        {
            var uri = new Uri(imagePath, UriKind.Absolute);
            // Decode image using WPF's BitmapDecoder for efficient streaming decoding
            var decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            var frame = decoder.Frames.First();

            // Calculate scale factor to fit image within the size while preserving aspect ratio
            var scale = Math.Min(1.0, (double)size / Math.Max(frame.PixelWidth, frame.PixelHeight));
            // Apply scaling transformation to the decoded frame
            var transformed = new TransformedBitmap(frame, new ScaleTransform(scale, scale));

            // Encode transformed image as PNG to memory stream
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(transformed));

            using var memoryStream = new MemoryStream();
            encoder.Save(memoryStream);
            memoryStream.Position = 0;

            // Create System.Drawing.Bitmap from memory stream
            return new Bitmap(memoryStream);
        }
    }
}
