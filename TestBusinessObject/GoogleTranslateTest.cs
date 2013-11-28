using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
        public void TranslateVraag_NLtoES_AreEqual()
        {
            var vertaaldeTekst = Translate(input:"Hoe gaat het?", from:"nl", to:"es");
            Assert.AreEqual("¿Cómo estás?", vertaaldeTekst);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void CreateOneResourceFile_CountValues_AreEqual()
        {
            const string resourceFilePath = @"C:\AlureOntwikkelingDev12\Alure.WS.BL\WerkstroomTaak.resx";
            const string newResourceFilePath = @"C:\AlureOntwikkelingDev12\Alure.WS.BL\WerkstroomTaak.{0}.resx";
            CreateResourceFile(new Res{OriginalResourcePath = resourceFilePath, NewResourcePath = newResourceFilePath}, "en");

            var resx = new ResXResourceReader(File.Open(string.Format(newResourceFilePath, "EN"), FileMode.OpenOrCreate));
            var subCount = resx.Cast<DictionaryEntry>().Count();

            Assert.AreEqual(43, subCount);
        }

        private static void CreateResourceFile(Res res, string toLanguage)
        {
            if (!res.OriginalResourceFileExists) return;
            var rrw = new ResXResourceReader(res.OriginalResourcePath);
            var fileStream = File.Open(string.Format(res.NewResourcePath, toLanguage.ToUpper()), FileMode.OpenOrCreate);
            using (var resXResourceWriter = new ResXResourceWriter(fileStream))
            {
                foreach (DictionaryEntry dictionary in rrw)
                {
                    var translatedValue = Translate(dictionary.Value.ToString(), "nl", toLanguage.ToLower());
                    if (translatedValue.IsNullOrEmpty())
                        translatedValue = string.Format("-TODO-{0}", dictionary.Value);
                    resXResourceWriter.AddResource(dictionary.Key.ToString(), translatedValue);
                }
            }
        }

        private class Res
        {
            public string TypeName { get; set; }
            public string OriginalResourcePath { get; set; }
            public bool OriginalResourceFileExists { get { return File.Exists(OriginalResourcePath); } }
            public string NewResourcePath { get; set; }
            public bool NewResourceFileExists(string language) { return File.Exists(string.Format(NewResourcePath, language.ToUpper())); }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void CreateCollectionFromResourcesInAssembly_CountAmount_AreEqual()
        {
            const string baseFolder = @"C:\AlureOntwikkelingDev12\";
            var resourceCollection = GetAllResourceFilesWithinAssembly<Alure.WS.BL.WerkstroomTaak>(baseFolder);
            Assert.AreEqual(15, resourceCollection.Count);
            var resultCollection =
                resourceCollection.Where(x => x.OriginalResourceFileExists)
                                  .Select(rese => rese.NewResourceFileExists("en"))
                                  .ToList();
            Assert.IsTrue(resultCollection.TrueForAll(x => x));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void CreateAllEnglishResourceFiles_CountResult_AreEqual()
        {
            const string baseFolder = @"C:\AlureOntwikkelingDev12\";
            var resourceCollection = GetAllResourceFilesWithinAssembly<Alure.WS.BL.WerkstroomTaak>(baseFolder);
            foreach (var res in resourceCollection.Where(res => res.OriginalResourceFileExists))
            {
                CreateResourceFile(res, "en");
            }
            var resultCollection =
                resourceCollection.Select(rese => rese.OriginalResourceFileExists && rese.NewResourceFileExists("en"))
                                  .ToList();
            Assert.IsTrue(resultCollection.TrueForAll(x => x));
        }

        private Collection<Res> GetAllResourceFilesWithinAssembly<T>(string baseFolder)
        {
            var assembly = Assembly.GetAssembly(typeof (T));
            var strippedAssembly = assembly.ManifestModule.Name.Remove(assembly.ManifestModule.Name.Length - 4);
            var resourceCollection = new Collection<Res>();
            assembly.GetManifestResourceNames().ForEach(manifestResourceName =>
                {
                    var resourceName =
                        manifestResourceName.Remove(manifestResourceName.Length - 10)
                                            .Replace(string.Format("{0}.", strippedAssembly), string.Empty);
                    resourceCollection.Add(new Res
                        {
                            TypeName = resourceName,
                            OriginalResourcePath = GetFullResourcePath(baseFolder, strippedAssembly, resourceName),
                            NewResourcePath = GetFullResourcePathWithLanguage(baseFolder, strippedAssembly, resourceName)
                        });
                });
            return resourceCollection;
        }

        private static string GetFullResourcePath(string baseFolder, string assembly, string resourceName)
        {
            return string.Format(@"{0}{1}\{2}.resx", baseFolder, assembly, resourceName.Replace(".", @"\"));
        }

        private static string GetFullResourcePathWithLanguage(string baseFolder, string assembly, string resourceName)
        {
            return string.Format(@"{0}{1}\{2}{3}", baseFolder, assembly, resourceName.Replace(".", @"\"), ".{0}.resx");
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