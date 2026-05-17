using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PhotoUtilityWin32
{
    public static class StringExtensions
    {
        public static string ToTitle(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
    public class OnnxClassifier
    {
        private readonly InferenceSession _session;
        private readonly string[] _labels;

        public OnnxClassifier(string modelPath, string labelPath)
        {
            if (File.Exists(modelPath))
            {
                try
                {
                    _session = new InferenceSession(modelPath);
                }
                catch
                {
                    _session = null;
                }
            }

            _labels = File.Exists(labelPath) ? File.ReadAllLines(labelPath) : Array.Empty<string>();
        }

        public string Classify(PhotoItem item)
        {
            if (_session == null || item == null)
            {
                return "Photo Event";
            }

            try
            {
                using var bitmap = ExifScanner.LoadThumbnail(item.FilePath, 640);
                using var resized = Resize(bitmap, 640, 640);
                var tensor = CreateTensor(resized);
                var inputName = _session.InputMetadata.Keys.First();
                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, tensor) };
                using var results = _session.Run(inputs);
                var output = results.First().AsEnumerable<float>().ToArray();
                Console.WriteLine($"Output array length: {output.Length}");
                Console.WriteLine($"Highest score: {output.Max()} at index {Array.IndexOf(output, output.Max())}");
                var index = ArgMax(output);
                return GetLabel(index).ToTitle();
            }
            catch
            {
                return "Photo Event";
            }
        }

        private static DenseTensor<float> CreateTensor(Bitmap image)
        {
            // YOLO typically expects 1 image, 3 channels (RGB), Height, Width
            var tensor = new DenseTensor<float>(new[] { 1, 3, image.Height, image.Width });

            // Lock bits for high-speed memory access
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var data = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            // Format24bppRgb is BGR in memory
                            // We map them to the RGB slots in the tensor
                            int offset = (y * data.Stride) + (x * 3);
                            tensor[0, 0, y, x] = ptr[offset + 2] / 255f; // R
                            tensor[0, 1, y, x] = ptr[offset + 1] / 255f; // G
                            tensor[0, 2, y, x] = ptr[offset] / 255f;     // B
                        }
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
            return tensor;
        }

        private static Bitmap Resize(Bitmap source, int width, int height)
        {
            var output = new Bitmap(width, height, source.PixelFormat);
            using var graphics = Graphics.FromImage(output);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.DrawImage(source, 0, 0, width, height);
            return output;
        }

        private static int ArgMax(float[] values)
        {
            var bestIndex = 0;
            var bestValue = float.MinValue;
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] > bestValue)
                {
                    bestValue = values[i];
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private string GetLabel(int index)
        {
            if (_labels.Length > index)
            {
                return _labels[index];
            }

            return $"Class {index}";
        }
    }
}
