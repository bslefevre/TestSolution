using System;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

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
            textBox2.Text = Translate(textBox1.Text, "nl", "es");
        }

        public static string Translate(string input, string from, string to)
        {
            var languagePair = string.Format("{0}|{1}", from, to);

            var url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            string result;

            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.Default;
                result = webClient.DownloadString(url);
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            var selectSingleNode = doc.DocumentNode.SelectSingleNode(string.Format("//span[@title='{0}']", input));

            return selectSingleNode != null ? selectSingleNode.InnerText : string.Empty;
        }
    }
}
