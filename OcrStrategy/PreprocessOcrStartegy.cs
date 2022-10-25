﻿using OpenCvSharp;
using OpenCvSharp.Text;

namespace Tesseract_UI_Tools.OcrStrategy
{
    public class PreprocessOcrStartegy : AOcrStrategy
    {
        public static string StrategyName = "With preprocess";
        private OCRTesseract OpenCvEngineInstance;
        public PreprocessOcrStartegy(string[] Languages) : base(Languages)
        {
            OpenCvEngineInstance = TessdataUtil.CreateOpenCvEngine(Languages);
        }


        public override void Dispose()
        {
            OpenCvEngineInstance.Dispose();
        }

        public override void GenerateTsv(string TiffPage, string TsvPage)
        {
            OCROutput OcrOut = new OCROutput(StrategyName);

            using (ResourcesTracker t = new ResourcesTracker())
            {
                Mat TiffMat = t.T(Cv2.ImRead(TiffPage));
                Mat Gray;
                switch (TiffMat.Channels())
                {
                    case 1:
                        Gray = t.T(TiffMat.Clone());
                        break;
                    case 3:
                        Gray = t.T(TiffMat.CvtColor(ColorConversionCodes.BGR2GRAY));
                        break;
                    case 4:
                        Gray = t.T(TiffMat.CvtColor(ColorConversionCodes.BGRA2GRAY));
                        break;
                    default:
                        throw new Exception($"Cannot handle number of channels specified ({TiffMat.Channels()})");
                }
                Mat redn = t.T(Gray.GaussianBlur(new OpenCvSharp.Size(3, 3), 0));
                Mat thre = t.T(redn.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 17, 27));
                Mat strcDilate = t.T(new Mat(3, 3, MatType.CV_8UC1, new int[] {
                    1,1,1,
                    1,0,1,
                    1,1,1
                }));
                Mat strcErode = t.T(Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3)));
                Mat dilated = t.T(thre.Dilate(strcDilate)); // Open = Dilate + Erude; Close = Erude + Dilate
                Mat eroded = t.T(dilated.Erode(strcErode));
                OpenCvEngineInstance.Run(eroded, out _, out OcrOut.Rects, out OcrOut.Components, out OcrOut.Confidences, ComponentLevels.Word);
            }
            OcrOut.Save(TsvPage);
        }
    }
}
