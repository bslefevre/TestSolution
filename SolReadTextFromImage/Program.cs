using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using tessnet2;

namespace SolReadTextFromImage
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Option();

            var @class = new Class1();
            var fileName = string.Format("{0}{1}", FolderName, FileName);
            var image = new Image<Bgr, byte>(fileName);
            @class.ProcessImage(image);


            //Tesseract3();
            // Code usage sample
            Ocr ocr = new Ocr();
            using (Bitmap bmp = new Bitmap(string.Format("{0}{1}", FolderName, FileName)))
            {
                tessnet2.Tesseract tessocr = new tessnet2.Tesseract();
                tessocr.Init("tessdata", "eng", false);
                tessocr.GetThresholdedImage(bmp, Rectangle.Empty)
                       .Save("c:\\temp\\" + Guid.NewGuid().ToString() + ".bmp");
                // Tessdata directory must be in the directory than this exe
                Console.WriteLine("Multithread version");
                ocr.DoOCRMultiThred(bmp, "eng");
                Console.WriteLine("Normal version");
                ocr.DoOCRNormal(bmp, "eng");
            }
        }

        public static string FolderName = "c:/Temp/";
        public const string FileName = "2014-01-11 163654-HYPERION.jpg";
        public static void Option()
        {
            string fileName = FolderName + FileName;

            Bitmap bmp = new Bitmap(fileName);

            Tesseract ocr = new Tesseract();
            
            //set the "tessdata" folder's parent folder
            ocr.Init("tessdata", "eng", false);

            ocr.SetVariable("tessedit_char_whitelist", "BCDFGHJKLMNPQRSTVWXYZ0123456789-");
            ocr.SetVariable("language_model_penalty_non_freq_dict_word", "1");
            ocr.SetVariable("language_model_penalty_non_dict_word ", "1");
            ocr.SetVariable("load_system_dawg", "0");

            List<Word> result = ocr.DoOCR(bmp, Rectangle.Empty);

            foreach (Word word in result)
            {
                Console.WriteLine(word.Confidence + ": " + word.Text + " (" + word.Left + ", " + word.Top + ")"
                    + " W=" + (word.Right - word.Left) + ", H=" + (word.Bottom - word.Top));
            }

            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);
            Graphics g = Graphics.FromImage(newBmp);
            g.DrawImage(bmp, 0, 0);

            foreach (Word word in result)
            {
                g.DrawRectangle(new Pen(new SolidBrush(Color.Red)),
                    new Rectangle(word.Left, word.Top, (word.Right - word.Left), (word.Bottom - word.Top)));
            }

            newBmp.Save(fileName + ".new.bmp");

            int lineCount = Tesseract.LineCount(result);
            Console.WriteLine("Line Count: " + lineCount);
            for (int i = 0; i < lineCount; i++)
            {
                String val = Tesseract.GetLineText(result, i);
                Console.WriteLine("Line " + i + ": " + val);
            }
        }
    }

    public class Ocr
    {
        public void DumpResult(List<tessnet2.Word> result)
        {
            foreach (tessnet2.Word word in result)
                Console.WriteLine("{0} : {1}", word.Confidence, word.Text);
        }

        public List<tessnet2.Word> DoOCRNormal(Bitmap image, string lang)
        {
            tessnet2.Tesseract ocr = new tessnet2.Tesseract();
            ocr.Init("tessdata", lang, false);
            List<tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);
            DumpResult(result);
            return result;
        }

        private ManualResetEvent m_event;

        public void DoOCRMultiThred(Bitmap image, string lang)
        {
            tessnet2.Tesseract ocr = new tessnet2.Tesseract();
            ocr.Init("tessdata", lang, false);
            // If the OcrDone delegate is not null then this'll be the multithreaded version
            ocr.OcrDone = new tessnet2.Tesseract.OcrDoneHandler(Finished);
            // For event to work, must use the multithreaded version
            ocr.ProgressEvent += new tessnet2.Tesseract.ProgressHandler(ocr_ProgressEvent);
            m_event = new ManualResetEvent(false);
            ocr.DoOCR(image, Rectangle.Empty);
            // Wait here it's finished
            m_event.WaitOne();
        }

        public void Finished(List<tessnet2.Word> result)
        {
            DumpResult(result);
            m_event.Set();
        }

        private void ocr_ProgressEvent(int percent)
        {
            Console.WriteLine("{0}% progression", percent);
        }
    }
}