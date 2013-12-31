using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
//using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WindowsFormsApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var splittedStringList = SplitString(textBox1.Text);
            textBox2.Text = string.Empty;

            foreach (var @string in splittedStringList.Where(x => !string.IsNullOrEmpty(x)))
            {
                var trimmedString = @string.Trim();
                trimmedString = Translate(trimmedString, "nl", "en");
                if (@string.EndsWith("\r"))
                    trimmedString += Environment.NewLine;

                textBox2.Text += trimmedString;
            }
        }

        /// <summary>
        /// To split the string in pieces by NewRule and end of a sentence.
        /// </summary>
        /// <param name="text">List of sentences to translate.</param>
        /// <returns></returns>
        private static IEnumerable<string> SplitString(string text)
        {
            var splittedStringList = new List<string>();

            var splittedTextArray = text.Split('\n');

            foreach (var s in splittedTextArray)
            {
                var nr = 0;
                while (s.IndexOf(".", nr) > -1 || s.IndexOf(".\r", nr) > -1)
                {
                    var ir = s.IndexOf(".\r", nr);
                    var ip = s.IndexOf(".", nr);
                    var i = 0;
                    if (ip < ir || ir == -1)
                        i = ip + 1;
                    else if (ip == ir && ir > -1)
                        i = ir + 2;
                    else if (ir > -1)
                        i = ir + 2;
                    splittedStringList.Add(s.Substring(nr, i - nr));
                    nr = i;
                }

                if (!string.IsNullOrEmpty(s.Substring(nr, s.Length - nr)))
                    splittedStringList.Add(s.Substring(nr, s.Length - nr));
            }

            return splittedStringList;
        }

        public static string Translate(string input, string from, string to)
        {
            //var languagePair = string.Format("{0}|{1}", from, to);

            //var url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            //string result;

            //using (var webClient = new WebClient())
            //{
            //    webClient.Encoding = Encoding.Default;
            //    result = webClient.DownloadString(url);
            //}
            //var doc = new HtmlDocument();
            //doc.LoadHtml(result);
            //var selectSingleNode = doc.DocumentNode.SelectSingleNode(string.Format("//span[@title='{0}']", input));

            //return selectSingleNode != null ? selectSingleNode.InnerText : string.Empty;
            return string.Empty;
        }
    }
}
