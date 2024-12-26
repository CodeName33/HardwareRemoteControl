using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Reg;
using Emgu.CV.Structure;

namespace HadwareRemoteControl
{
    public class TransformationData
    {
        public bool UseTransform = false;
        public PointF[] destPoints = [new(0, 0), new(0, 0), new(0, 0), new(0, 0)];

        public void InitPoints(Bitmap source)
        {
            if (destPoints[1].X == 0 && destPoints[1].Y == 0 
                &&
                destPoints[2].X == 0 && destPoints[2].Y == 0
                &&
                destPoints[3].X == 0 && destPoints[3].Y == 0
                )
            {
                destPoints[1].X = source.Width - 1;
                destPoints[2].X = source.Width - 1;
                destPoints[2].Y = source.Height - 1;
                destPoints[3].Y = source.Height - 1;
            }
        }

        public Bitmap DoTransformation(Bitmap sourceImage)
        {
            if (UseTransform)
            {
                return ImageTransform.PerspectiveImageDistortion(sourceImage, destPoints);
                //return sourceImage;//
            }
            //ImageTransform.PerspectiveImageDistortion(sourceImage, destPoints);
            return sourceImage;
        }
    }

    public static class ImageTransform
    {
        public static Bitmap PerspectiveImageDistortion(Bitmap sourceImage, PointF[] destPoints)
        {
            if (destPoints.Length != 4)
                throw new ArgumentException("Destination points array must contain exactly 4 points.");


            using Image<Bgr, Byte> image = sourceImage.ToImage<Bgr, Byte>();
            using Mat srcMat = image.Mat;

            PointF[] srcPoints =
            [
                new(0, 0),
                new(sourceImage.Width - 1, 0),
                new(sourceImage.Width - 1, sourceImage.Height - 1),
                new(0, sourceImage.Height - 1)
            ];

            using Mat perspectiveMatrix = CvInvoke.GetPerspectiveTransform(srcPoints, destPoints);

            using Mat destMat = new();
            CvInvoke.WarpPerspective(srcMat, destMat, perspectiveMatrix, new Size(sourceImage.Width, sourceImage.Height), Inter.Linear, Warp.Default, BorderType.Constant, new MCvScalar(0, 0, 0));

            Bitmap resultBitmap = destMat.ToBitmap();

            return resultBitmap;
            //return null;
        }

        /*
        private static Mat BitmapToMat(Bitmap bitmap)
        {
            // Convert Bitmap to Image<Bgr, Byte>
            Image<Bgr, Byte> image = bitmap.ToImage<Bgr, Byte>();
            return image.Mat;
        }
        //*/
    }
}
