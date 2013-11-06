using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Alure.Base.BL;
using Alure.Base.BL.Test;
using Alure.CRM.BL;
using Alure.WS.BL;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    [TestClass]
    public class HtmlKeyWordVervangerTest : TestBase
    {
        protected const string TestHtmlSingleKeyWord = "<HTML>[relatie.zoeknaam]</HTML>";

        protected const string TestHtmlDoubleKeyWord = "<HTML>[relatie.zoeknaam][relatie.nummer]</HTML>";

        protected const string TestHtmlSingleKeyWordRelatieUserDisplay = "<HTML>[relatie.userdisplay]</HTML>";

        protected const string TestHtmlDoubleKeyWordRelatieAndWerkstroomTaak = "<HTML>[relatie.userdisplay] - [werkstroomtaak.memo]</HTML>";

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_Init()
        {
            var parser = new HtmlKeyWordVervanger(TestHtmlSingleKeyWord,
                                                  new Dictionary<string, object> { { "relatie", "object" } });
            var html = parser.GetHtml(false);
            Assert.AreEqual(TestHtmlSingleKeyWord, html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_InsertHtmlWithSingleKeyWord_ContainsKey()
        {
            var parser = new HtmlKeyWordVervanger(TestHtmlSingleKeyWord,
                                                  new Dictionary<string, object> { { "relatie", "object" } });
            var matchCollection = parser.GetKeyWordsFromHtml();
            Assert.AreEqual(1, matchCollection.Count);
            Assert.IsTrue(matchCollection.ContainsKey("[relatie.zoeknaam]"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_InsertHtmlWithDoubleKeyWord_ContainsKey()
        {
            var parser = new HtmlKeyWordVervanger(TestHtmlDoubleKeyWord,
                                                  new Dictionary<string, object> { { "relatie", "object" } });
            var matchCollection = parser.GetKeyWordsFromHtml();
            Assert.AreEqual(2, matchCollection.Count);
            Assert.IsTrue(matchCollection.ContainsKey("[relatie.zoeknaam]"));
            Assert.IsTrue(matchCollection.ContainsKey("[relatie.nummer]"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_GetReplacedHtml_AreEqual()
        {
            var parser = new HtmlKeyWordVervanger(TestHtmlDoubleKeyWord,
                                                  new Dictionary<string, object> { { "relatie", "object" } });
            var html = parser.GetHtml();
            Assert.AreEqual(TestHtmlDoubleKeyWord, html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_GetReplacedHtmlWithRelatie_AreEqual()
        {
            var relatie = new Relatie();

            var parser = new HtmlKeyWordVervanger(TestHtmlSingleKeyWordRelatieUserDisplay,
                                                  new Dictionary<string, object> {{"relatie", relatie}});
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Nieuwe relatie</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_GetReplacedHtmlWithRelatieAndWerkstroomTaak_AreEqual()
        {
            var relatie = new Relatie();
            var werkstroomTaak = new WerkstroomTaak { Memo = "TestMemoTekst" };

            var parser = new HtmlKeyWordVervanger(TestHtmlDoubleKeyWordRelatieAndWerkstroomTaak,
                                                  new Dictionary<string, object>
                                                      {
                                                          {"relatie", relatie},
                                                          {"werkstroomtaak", werkstroomTaak}
                                                      });
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Nieuwe relatie - TestMemoTekst</HTML>", html);
        }

        [TestMethod]
        [Ignore]
        public void UoTest()
        {
            var userControl = new XtraUserControl { Width = 600, Height = 300, Dock = DockStyle.Fill };

            var textEdit = new TextEdit();
            var memoEdit = new MemoEdit();
            memoEdit.Properties.ReadOnly = true;
            var button = new SimpleButton { Text = "Parse HTML" };

            button.Click += (sender, args) =>
            {
                var relatie = new Relatie();
                var werkstroomTaak = new WerkstroomTaak { Memo = "TestMemoTekst" };

                var text = textEdit.EditValue.ToString();
                var parser = new HtmlKeyWordVervanger(text, new Dictionary<string, object>
                        {
                            {"relatie", relatie},
                            {"werkstroomtaak", werkstroomTaak}
                        });
                var html = parser.GetHtml();
                memoEdit.EditValue = html;
            };
            
            var layoutControl = new LayoutControl { Dock = DockStyle.Fill };
            var lci = layoutControl.AddItem(string.Empty, textEdit);
            layoutControl.AddItem(string.Empty, button);
            var lci2 = layoutControl.AddItem(string.Empty, memoEdit);
            lci.TextVisible = false;
            lci2.TextVisible = false;
            
            layoutControl.Controls.Add(textEdit);
            layoutControl.Controls.Add(button);
            layoutControl.Controls.Add(memoEdit);

            userControl.Controls.Add(layoutControl);
            var xtraForm = new XtraForm { Width = 600, Height = 300, Text = "HTML Parser" };
            xtraForm.Controls.Add(userControl);
            xtraForm.ShowDialog();
        }
    }

    public class HtmlKeyWordVervanger
    {
        protected Char Separator
        {
            get
            {
                return Convert.ToChar(".");
            }
        }

        protected string Html { get; set; }

        protected Dictionary<string, object> Dictionary { get; set; }

        public HtmlKeyWordVervanger(string html, Dictionary<string, object> dictionary)
        {
            Html = html;
            Dictionary = dictionary;
        }

        public Dictionary<string, string> GetKeyWordsFromHtml()
        {
            return (from Match match in new Regex(@"\[[^<]+?]").Matches(Html) select match.Value).ToDictionary(keyWord => keyWord, GetMatchedKeyWord);
        }

        private string GetMatchedKeyWord(string keyWord)
        {
            var matchedKeyWord = string.Empty;

            var strippedKeyWord = keyWord.Replace("[", string.Empty).Replace("]", string.Empty);

            if (strippedKeyWord.Contains(Separator))
                matchedKeyWord = GetMatchedKeyWordFromBusinessObject(strippedKeyWord);
            
            return matchedKeyWord.IsNullOrEmpty() ? keyWord : matchedKeyWord;
        }

        private string GetMatchedKeyWordFromBusinessObject(string strippedKeyWord)
        {
            var matchedKeyWord = string.Empty;
            var splittedKeyWord = strippedKeyWord.Split(Separator);
            var businessObjectString = splittedKeyWord[0];
            var businessObjectPropertyString = splittedKeyWord[1];

            if (Dictionary.ContainsKey(businessObjectString))
            {
                var businessObjectUntyped = Dictionary[businessObjectString];
                var businessObject = businessObjectUntyped as BusinessObject;
                if (businessObject != null)
                    matchedKeyWord = GetBusinessObjectProperty(businessObject, businessObjectPropertyString);
            }
            return matchedKeyWord;
        }

        private static string GetBusinessObjectProperty(BusinessObject businessObject, string businessObjectProperty)
        {
            var propertyValue = string.Empty;
            var type = businessObject.GetType();
            var propertyInfo = type.GetProperty(businessObjectProperty,
                                                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(businessObject, new object[] {});
                propertyValue = Convert.ToString(value);
            }
            return propertyValue;
        }

        public string GetHtml(bool parsed = true)
        {
            if (parsed) ParseHtml();
            return Html;
        }

        private void ParseHtml()
        {
            ReplaceKeyWords(GetKeyWordsFromHtml());
        }

        private void ReplaceKeyWords(Dictionary<string, string> keyWordsFromHtml)
        {
            Html = keyWordsFromHtml.Aggregate(Html, (current, keyWordAndParsed) => current.Replace(keyWordAndParsed.Key, keyWordAndParsed.Value));
        }
    }
}