using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV.OCR;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu;

namespace WindowsFormsApp36
{
    class NumberPlateRecognizer
    {
        private Tesseract ocr;

        public NumberPlateRecognizer(string tessdatapath, string lang)
        {
            ocr = new Tesseract(tessdatapath, lang, OcrEngineMode.TesseractLstmCombined);
        }

        private int GetNumberOfChildren(int [,] hierachy,int index) 
        {
            index = hierachy[index, 2];

            if (index < 0)
                return 0;

            int count = 1;

            while (hierachy[index,0] > 0)
            {
                count++;
                index = hierachy[index, 0];

            }
            return count;
        }

        private static UMat FilterPlate (UMat plate)
        {
            UMat thresh = new UMat();
            CvInvoke.Threshold(plate, thresh, 120, 255, ThresholdType.BinaryInv);
            Size plateSize = plate.Size;
            using (Mat plateMask = new Mat(plateSize.Height, plateSize.Width, DepthType.Cv8U, 1))
            {
                using (Mat plateCanny = new Mat())
                {
                    using (VectorOfVectorOfPoint countours = new VectorOfVectorOfPoint() )
                    {
                        plateMask.SetTo(new MCvScalar(255.0));
                        CvInvoke.Canny(plate, plateCanny, 100, 50);
                        CvInvoke.FindContours(plateCanny, countours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                        int count = countours.Size;

                        for (int i = 0; i <count;i++)
                        {
                            using (VectorOfPoint countour = countours[i])
                            {
                                Rectangle rect = CvInvoke.BoundingRectangle(countour);
                                if (rect.Height >(plateSize.Height >> 1))
                                {
                                    rect.X -= 1;
                                    rect.Y -= 1;

                                    rect.Width += 2;
                                    rect.Height += 2;
                                }


                            }
                             
                        }


                    }

                }

            }

        }
    }
}
