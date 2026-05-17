using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PhotoUtilityWin32
{
    public static class Phash
    {
        /// <summary>
        /// Computes a perceptual hash (pHash) of an image using the DCT algorithm.
        /// This creates a 64-bit hash that can be used to identify visually similar images.
        /// </summary>
        public static ulong ComputeHash(Bitmap source)
        {
            // Resize image to 32x32 for consistent analysis
            using var resized = Resize(source, 32, 32);
            var pixels = new double[32, 32];

            // Extract grayscale values using luminance formula (standard RGB to grayscale conversion)
            for (var y = 0; y < 32; y++)
            {
                for (var x = 0; x < 32; x++)
                {
                    var color = resized.GetPixel(x, y);
                    pixels[x, y] = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
                }
            }

            // Apply 2D Discrete Cosine Transform to get frequency components
            var dct = Dct2D(pixels);
            var values = new double[64];
            var index = 0;

            // Extract top-left 8x8 region of DCT coefficients (lower frequencies contain most image info)
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    values[index++] = dct[x, y];
                }
            }

            // Compute average of the 64 DCT values
            var average = values.Average();
            ulong hash = 0;
            // Create 64-bit hash where each bit represents if a DCT value is above average
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] > average)
                {
                    hash |= 1UL << i;
                }
            }

            return hash;
        }

        /// <summary>
        /// Calculates the Hamming distance between two perceptual hashes.
        /// Lower distances indicate more visually similar images (threshold typically 5-10).
        /// </summary>
        public static int HammingDistance(ulong left, ulong right)
        {
            // XOR operation reveals differing bits between the two hashes
            var xor = left ^ right;
            // Count number of 1s in the binary representation
            return PopCount(xor);
        }

        /// <summary>
        /// Counts the number of 1-bits in a 64-bit integer (population count/Hamming weight).
        /// Uses Brian Kernighan's algorithm for efficient counting.
        /// </summary>
        private static int PopCount(ulong value)
        {
            var count = 0;
            // Each iteration removes one 1-bit from the rightmost position
            while (value != 0)
            {
                value &= value - 1;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Resizes a bitmap to specified dimensions using high-quality interpolation.
        /// </summary>
        private static Bitmap Resize(Bitmap source, int width, int height)
        {
            var result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using var graphics = Graphics.FromImage(result);
            // Configure graphics for highest quality downsampling
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.DrawImage(source, 0, 0, width, height);
            return result;
        }

        /// <summary>
        /// Applies 2D Discrete Cosine Transform to convert spatial domain to frequency domain.
        /// DCT concentrates image information in low-frequency components.
        /// </summary>
        private static double[,] Dct2D(double[,] data)
        {
            const int size = 32;
            var result = new double[size, size];
            // Compute DCT coefficient for each frequency component (u, v)
            for (var u = 0; u < size; u++)
            {
                for (var v = 0; v < size; v++)
                {
                    var sum = 0.0;
                    // Sum over all spatial domain values weighted by cosine basis functions
                    for (var x = 0; x < size; x++)
                    {
                        for (var y = 0; y < size; y++)
                        {
                            sum += data[x, y] *
                                   Math.Cos(((2 * x + 1) * u * Math.PI) / (2 * size)) *
                                   Math.Cos(((2 * y + 1) * v * Math.PI) / (2 * size));
                        }
                    }

                    // Apply normalization constants (cu and cv are smaller for u=0 or v=0)
                    var cu = u == 0 ? 1.0 / Math.Sqrt(2) : 1.0;
                    var cv = v == 0 ? 1.0 / Math.Sqrt(2) : 1.0;
                    result[u, v] = 0.25 * cu * cv * sum;
                }
            }

            return result;
        }
    }
}
