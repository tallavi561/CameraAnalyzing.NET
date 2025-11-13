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
                    int imgW = image.Width;
                    int imgH = image.Height;

                    // Clamp coordinates safely
                    x1 = Math.Clamp(x1, 0, imgW - 1);
                    y1 = Math.Clamp(y1, 0, imgH - 1);
                    x2 = Math.Clamp(x2, 0, imgW);
                    y2 = Math.Clamp(y2, 0, imgH);

                    // Ensure x1 < x2, y1 < y2
                    if (x2 <= x1 || y2 <= y1)
                    {
                        Logger.LogError($"Invalid crop box after clamping ({x1},{y1},{x2},{y2}).");
                        return;
                    }

                    int width = x2 - x1;
                    int height = y2 - y1;

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
