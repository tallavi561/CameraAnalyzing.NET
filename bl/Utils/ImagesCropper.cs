using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace CameraAnalyzer.bl.Utils
{
    public static class ImagesCropper
    {
        /**
         * Crops a region from an image and saves it as a new image.
         *
         * @param x1 Left coordinate of crop area.
         * @param y1 Top coordinate of crop area.
         * @param x2 Right coordinate of crop area.
         * @param y2 Bottom coordinate of crop area.
         * @param originalFilePath Full path of the source image file.
         * @param newFilePath Full path where the cropped image will be saved.
         */
        public static void CropAndSaveImage(int x1, int y1, int x2, int y2, string originalFilePath, string newFilePath)
        {
            try
            {
                if (!File.Exists(originalFilePath))
                {
                    Logger.Instance.LogError($"Original image not found: {originalFilePath}");
                    return;
                }

                using (Image<Rgba32> image = Image.Load<Rgba32>(originalFilePath))
                {
                    int width = x2 - x1;
                    int height = y2 - y1;

                    if (width <= 0 || height <= 0)
                    {
                        Logger.Instance.LogError("Invalid crop dimensions (width/height <= 0).");
                        return;
                    }

                    var cropRectangle = new Rectangle(x1, y1, width, height);

                    image.Mutate(ctx => ctx.Crop(cropRectangle));

                    string? directory = Path.GetDirectoryName(newFilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    image.Save(newFilePath);

                    Logger.Instance.LogInfo($"Cropped image saved successfully: {newFilePath}");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError($"CropAndSaveImage failed: {ex.Message}");
            }
        }
    }
}
