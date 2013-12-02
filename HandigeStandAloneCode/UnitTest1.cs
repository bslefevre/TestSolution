using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandigeStandAloneCode
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        private static readonly Dictionary<string, string> SignatureAndImageFolder = new Dictionary<string, string> { { "Signatures", "_files" }, { "Handtekeningen", "_bestanden" } };

        private static string ReadSignature()
        {
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\{0}";
            string signature = string.Empty;
            DirectoryInfo diInfo = null;
            string imageFolder = string.Empty;
            foreach (var signatureAndImage in SignatureAndImageFolder)
            {
                if (!new DirectoryInfo(string.Format(appDataDir, signatureAndImage.Key)).Exists) continue;
                diInfo = new DirectoryInfo(string.Format(appDataDir, signatureAndImage.Key));
                imageFolder = signatureAndImage.Value;
            }

            if (diInfo != null)
            {
                var fiSignature = diInfo.GetFiles("*.htm");
                if (fiSignature.Length > 0)
                {
                    var sr = new StreamReader(fiSignature[0].FullName, Encoding.Default);
                    signature = sr.ReadToEnd();

                    if (!string.IsNullOrEmpty(signature))
                    {
                        string fileName = fiSignature[0].Name.Replace(fiSignature[0].Extension, string.Empty);
                        signature = signature.Replace(fileName + imageFolder + "/", appDataDir + "/" + fileName + "/" + imageFolder);
                    }
                }
            }
            return signature;
        }
    }
}
