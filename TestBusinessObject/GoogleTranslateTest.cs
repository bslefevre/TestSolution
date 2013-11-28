using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading;
using Alure.Base.BL;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    [TestClass]
    public class GoogleTranslateTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        public void TranslateVraag_NLtoES_AreEquals()
        {
            var vertaaldeTekst = Translate(input:"Hoe gaat het?", from:"nl", to:"es");
            Assert.AreEqual("¿Cómo estás?", vertaaldeTekst);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void ReadResourceFiles()
        {
            //var assembly = Assembly.GetAssembly(typeof (Alure.WS.BL.WerkstroomTaak));
            //var manifestResourceNames = assembly.GetManifestResourceNames();


            var rrw = new ResXResourceReader(@"C:\AlureOntwikkelingDev12\Alure.WS.BL\WerkstroomTaak.resx");

            var werkstroomtaakResx = @"C:\AlureOntwikkelingDev12\Alure.WS.BL\WerkstroomTaak.EN.resx";
            
            FileStream fileStream = File.Open(werkstroomtaakResx, FileMode.OpenOrCreate);
            //if (!File.Exists(werkstroomtaakResx))
            //    fileStream = File.Create(werkstroomtaakResx);
            //else
                
            var resXResourceWriter = new ResXResourceWriter(fileStream);
            
            foreach (DictionaryEntry dictionary in rrw)
            {
                var translatedValue = Translate(dictionary.Value.ToString(), "nl", "en");
                if (translatedValue.IsNullOrEmpty())
                    translatedValue = string.Format("-NNB-{0}", dictionary.Value);
                resXResourceWriter.AddResource(dictionary.Key.ToString(), translatedValue);
                Thread.Sleep(100);
            }
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
            Thread.Sleep(100);
            var selectSingleNode = doc.DocumentNode.SelectSingleNode(string.Format("//span[@title='{0}']", input));
            return selectSingleNode != null ? selectSingleNode.InnerText : string.Empty;
        }
    }
}