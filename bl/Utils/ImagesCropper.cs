using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace CameraAnalyzer.bl.Utils
{
    public static class ImagesCropper
    {
        public static void CropAndSaveImage(int x1, int y1, int x2, int y2, string originalFilePath, string newFilePath)
        {
            try
            {
                if (!File.Exists(originalFilePath))
                {
                    Logger.LogError($"Original image not found: {originalFilePath}");
                    return;
                }

                using (Image<Rgba32> image = Image.Load<Rgba32>(originalFilePath))
                {
                    if (x1 < 0 || y1 < 0 || x2 > image.Width || y2 > image.Height)
                    {
                        Logger.LogError($"Crop area ({x1},{y1},{x2},{y2}) is outside image bounds ({image.Width}x{image.Height}).");
                        return;
                    }

                    int width = x2 - x1;
                    int height = y2 - y1;

                    if (width <= 0 || height <= 0)
                    {
                        Logger.LogError("Invalid crop dimensions (width/height <= 0).");
                        return;
                    }

                    var cropRectangle = new Rectangle(x1, y1, width, height);
                    image.Mutate(ctx => ctx.Crop(cropRectangle));

                    string? directory = Path.GetDirectoryName(newFilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    image.Save(newFilePath);
                    Logger.LogInfo($"Cropped image saved successfully: {newFilePath}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"CropAndSaveImage failed: {ex.Message}");
            }
        }
    }
}
