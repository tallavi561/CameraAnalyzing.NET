using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using CameraAnalyzer.bl.Models;
using CameraAnalyzer.bl.Utils;

namespace CameraAnalyzer.bl.APIs
{
    public class YoloAPI
    {
        private readonly InferenceSession _session;
        private readonly string[] _classes;
        private readonly string _inputName;

        public YoloAPI(string modelPath)
        {
            _session = new InferenceSession(modelPath);

            _inputName = _session.InputMetadata.Keys.First();

            _classes = File.ReadAllLines("models/coco_labels_80.txt");

            Logger.LogInfo($"YOLO → Input = {_inputName}");
        }

        public List<BoundingBox> Detect(string imagePath, float confThreshold = 0.35f)
        {
            using var img = Image.Load<Rgba32>(imagePath);
            int origW = img.Width;
            int origH = img.Height;

            using var resized = img.Clone(ctx => ctx.Resize(640, 640));
            var tensor = ImageToTensor(resized);

            var inputs = new List<NamedOnnxValue>()
            {
                NamedOnnxValue.CreateFromTensor(_inputName, tensor)
            };

            var outputs = _session.Run(inputs);

            // Get each head
            var reg1 = outputs.First(x => x.Name == "reg1").AsTensor<float>();
            var cls1 = outputs.First(x => x.Name == "cls1").AsTensor<float>();

            var reg2 = outputs.First(x => x.Name == "reg2").AsTensor<float>();
            var cls2 = outputs.First(x => x.Name == "cls2").AsTensor<float>();

            var reg3 = outputs.First(x => x.Name == "reg3").AsTensor<float>();
            var cls3 = outputs.First(x => x.Name == "cls3").AsTensor<float>();

            var list = new List<BoundingBox>();

            list.AddRange(ParseHead(reg1, cls1, origW, origH, confThreshold));
            list.AddRange(ParseHead(reg2, cls2, origW, origH, confThreshold));
            list.AddRange(ParseHead(reg3, cls3, origW, origH, confThreshold));

            return list;
        }

        // ------------------ Parse One YOLO Head ------------------
        private List<BoundingBox> ParseHead(
            Tensor<float> reg,
            Tensor<float> cls,
            int origW,
            int origH,
            float threshold)
        {
            var boxes = new List<BoundingBox>();

            int numCells = reg.Dimensions[2];   // N locations
            int numClasses = cls.Dimensions[1]; // 80

            for (int i = 0; i < numCells; i++)
            {
                float cx = reg[0, 0, i];
                float cy = reg[0, 1, i];
                float w  = reg[0, 2, i];
                float h  = reg[0, 3, i];

                // find best class
                float bestClassScore = 0;
                int bestClassIndex = -1;

                for (int c = 0; c < numClasses; c++)
                {
                    float score = cls[0, c, i];
                    if (score > bestClassScore)
                    {
                        bestClassScore = score;
                        bestClassIndex = c;
                    }
                }

                if (bestClassScore < threshold)
                    continue;

                // Convert (cx,cy,w,h) → pixel coords
                float x1 = (cx - w / 2) * origW;
                float y1 = (cy - h / 2) * origH;
                float x2 = (cx + w / 2) * origW;
                float y2 = (cy + h / 2) * origH;

                boxes.Add(new BoundingBox
                {
                    Class = _classes[bestClassIndex],
                    Confidence = bestClassScore,
                    X1 = (int)x1,
                    Y1 = (int)y1,
                    X2 = (int)x2,
                    Y2 = (int)y2
                });
            }

            return boxes;
        }

        // ------------------ Convert Image to Tensor ------------------
        private DenseTensor<float> ImageToTensor(Image<Rgba32> img)
        {
            int h = img.Height;
            int w = img.Width;

            var tensor = new DenseTensor<float>(new[] { 1, 3, h, w });

            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < h; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < w; x++)
                    {
                        tensor[0, 0, y, x] = row[x].R / 255f;
                        tensor[0, 1, y, x] = row[x].G / 255f;
                        tensor[0, 2, y, x] = row[x].B / 255f;
                    }
                }
            });

            return tensor;
        }
    }
}
