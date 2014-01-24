using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SolReadTextFromImage  
{
    public class Class1
    {
        public void ProcessImage(Image<Bgr, byte> image)
        {
            Stopwatch watch = Stopwatch.StartNew(); // time the detection process

            List<Image<Gray, Byte>> licensePlateImagesList = new List<Image<Gray, byte>>();
            List<Image<Gray, Byte>> filteredLicensePlateImagesList = new List<Image<Gray, byte>>();
            List<MCvBox2D> licenseBoxList = new List<MCvBox2D>();
            List<string> words = DetectLicensePlate(
               image,
               licensePlateImagesList,
               filteredLicensePlateImagesList,
               licenseBoxList);

            watch.Stop(); //stop the timer
            //processTimeLabel.Text = String.Format("License Plate Recognition time: {0} milli-seconds", watch.Elapsed.TotalMilliseconds);

            //panel1.Controls.Clear();
            Point startPoint = new Point(10, 10);
            for (int i = 0; i < words.Count; i++)
            {
                //AddLabelAndImage(
                //   ref startPoint,
                //   String.Format("License: {0}", words[i]),
                //   licensePlateImagesList[i].ConcateVertical(filteredLicensePlateImagesList[i]));
                image.Draw(licenseBoxList[i], new Bgr(Color.Red), 2);
            }

            //imageBox1.Image = image;
        }

        public List<String> DetectLicensePlate(
         Image<Bgr, byte> img,
         List<Image<Gray, Byte>> licensePlateImagesList,
         List<Image<Gray, Byte>> filteredLicensePlateImagesList,
         List<MCvBox2D> detectedLicensePlateRegionList)
        {
            List<String> licenses = new List<String>();
            using (Image<Gray, byte> gray = img.Convert<Gray, Byte>())
            //using (Image<Gray, byte> gray = GetWhitePixelMask(img))
            using (Image<Gray, Byte> canny = new Image<Gray, byte>(gray.Size))
            using (MemStorage stor = new MemStorage())
            {
                CvInvoke.cvCanny(gray, canny, 100, 50, 3);

                Contour<Point> contours = canny.FindContours(
                     Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                     Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE,
                     stor);
                FindLicensePlate(contours, gray, canny, licensePlateImagesList, filteredLicensePlateImagesList, detectedLicensePlateRegionList, licenses);
            }
            return licenses;
        }

        private void FindLicensePlate(
         Contour<System.Drawing.Point> contours, Image<Gray, Byte> gray, Image<Gray, Byte> canny,
         List<Image<Gray, Byte>> licensePlateImagesList, List<Image<Gray, Byte>> filteredLicensePlateImagesList, List<MCvBox2D> detectedLicensePlateRegionList,
         List<String> licenses)
        {
            for (; contours != null; contours = contours.HNext)
            {
                int numberOfChildren = GetNumberOfChildren(contours);
                //if it does not contains any children (charactor), it is not a license plate region
                if (numberOfChildren == 0) continue;

                if (contours.Area > 400)
                {
                    if (numberOfChildren < 3)
                    {
                        //If the contour has less than 3 children, it is not a license plate (assuming license plate has at least 3 charactor)
                        //However we should search the children of this contour to see if any of them is a license plate
                        FindLicensePlate(contours.VNext, gray, canny, licensePlateImagesList, filteredLicensePlateImagesList, detectedLicensePlateRegionList, licenses);
                        continue;
                    }

                    MCvBox2D box = contours.GetMinAreaRect();
                    if (box.angle < -45.0)
                    {
                        float tmp = box.size.Width;
                        box.size.Width = box.size.Height;
                        box.size.Height = tmp;
                        box.angle += 90.0f;
                    }
                    else if (box.angle > 45.0)
                    {
                        float tmp = box.size.Width;
                        box.size.Width = box.size.Height;
                        box.size.Height = tmp;
                        box.angle -= 90.0f;
                    }

                    double whRatio = (double)box.size.Width / box.size.Height;
                    if (!(3.0 < whRatio && whRatio < 10.0))
                    //if (!(1.0 < whRatio && whRatio < 2.0))
                    {  //if the width height ratio is not in the specific range,it is not a license plate 
                        //However we should search the children of this contour to see if any of them is a license plate
                        Contour<System.Drawing.Point> child = contours.VNext;
                        if (child != null)
                            FindLicensePlate(child, gray, canny, licensePlateImagesList, filteredLicensePlateImagesList, detectedLicensePlateRegionList, licenses);
                        continue;
                    }

                    using (Image<Gray, Byte> tmp1 = gray.Copy(box))
                    //resize the license plate such that the front is ~ 10-12. This size of front results in better accuracy from tesseract
                    using (Image<Gray, Byte> tmp2 = tmp1.Resize(240, 180, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC, true))
                    {
                        //removes some pixels from the edge
                        int edgePixelSize = 2;
                        tmp2.ROI = new System.Drawing.Rectangle(new System.Drawing.Point(edgePixelSize, edgePixelSize), tmp2.Size - new System.Drawing.Size(2 * edgePixelSize, 2 * edgePixelSize));
                        Image<Gray, Byte> plate = tmp2.Copy();

                        Image<Gray, Byte> filteredPlate = FilterPlate(plate);

                        

                        //licenses.Add(strBuilder.ToString());
                        licensePlateImagesList.Add(plate);
                        filteredLicensePlateImagesList.Add(filteredPlate);
                        detectedLicensePlateRegionList.Add(box);

                    }
                }
            }
        }

        private static Image<Gray, Byte> FilterPlate(Image<Gray, Byte> plate)
        {
            Image<Gray, Byte> thresh = plate.ThresholdBinaryInv(new Gray(120), new Gray(255));

            using (Image<Gray, Byte> plateMask = new Image<Gray, byte>(plate.Size))
            using (Image<Gray, Byte> plateCanny = plate.Canny(100, 50))
            using (MemStorage stor = new MemStorage())
            {
                plateMask.SetValue(255.0);
                for (
                   Contour<System.Drawing.Point> contours = plateCanny.FindContours(
                      Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                      Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL,
                      stor);
                   contours != null;
                   contours = contours.HNext)
                {
                    System.Drawing.Rectangle rect = contours.BoundingRectangle;
                    if (rect.Height > (plate.Height >> 1))
                    {
                        rect.X -= 1; rect.Y -= 1; rect.Width += 2; rect.Height += 2;
                        rect.Intersect(plate.ROI);

                        plateMask.Draw(rect, new Gray(0.0), -1);
                    }
                }

                thresh.SetValue(0, plateMask);
            }

            thresh._Erode(1);
            thresh._Dilate(1);

            return thresh;
        }

        private static int GetNumberOfChildren(Contour<System.Drawing.Point> contours)
        {
            Contour<System.Drawing.Point> child = contours.VNext;
            if (child == null) return 0;
            int count = 0;
            while (child != null)
            {
                count++;
                child = child.HNext;
            }
            return count;
        }
    }
}
