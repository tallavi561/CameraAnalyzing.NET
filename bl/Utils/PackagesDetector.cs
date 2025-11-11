using System.Collections.Generic;
using System.Linq;
using System.Drawing;          // System.Drawing.Image + RectangleF
using Yolov8Net;               // חבילת Yolov8.Net 1.0.4
using CameraAnalyzer.bl.Models;

namespace CameraAnalyzer.bl.Utils
{
    public static class PackagesDetector
    {
        // Create() מחזיר IPredictor
        private static readonly IPredictor _model = YoloV8Predictor.Create("models/yolov5n.onnx");

        /**
         * Detect packages and return bounding boxes.
         */
        public static List<BoundingBox> Detect(string imagePath, float confidenceThreshold = 0.35f)
        {
            using var image = Image.FromFile(imagePath);     // שים לב: System.Drawing.Image

            var results = _model.Predict(image);             // IEnumerable<Prediction>

            var boxes = results
                .Where(r => r.Score >= confidenceThreshold)  // ← לא Confidence, אלא Score
                .Select(r =>
                {
                    var rect = r.Rectangle; // RectangleF
                    return new BoundingBox
                    {
                        x1 = (int)rect.Left,
                        y1 = (int)rect.Top,
                        x2 = (int)rect.Right,
                        y2 = (int)rect.Bottom
                    };
                })
                .ToList();

            Logger.LogInfo($"Detected {boxes.Count} packages (threshold={confidenceThreshold}).");
            return boxes;
        }
    }
}
