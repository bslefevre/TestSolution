using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_Init()
        {
            var parser = new HtmlKeyWordVervanger("<HTML>[relatie.zoeknaam]</HTML>",
                                                  new Dictionary<string, object> { { "relatie", "object" } });
            var html = parser.GetHtml(false);
            Assert.AreEqual("<HTML>[relatie.zoeknaam]</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_GetReplacedHtml_AreEqual()
        {
            var parser = new HtmlKeyWordVervanger("<HTML>[relatie.zoeknaam][relatie.nummer]</HTML>",
                                                  new Dictionary<string, object> { { "relatie", "object" } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML></HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_GetReplacedHtmlWithRelatie_AreEqual()
        {
            var relatie = new Relatie();

            var parser = new HtmlKeyWordVervanger("<HTML>[relatie.userdisplay]</HTML>",
                                                  new Dictionary<string, object> { { "relatie", relatie } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Nieuwe relatie</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void HtmlKeyWordVervanger_GetReplacedHtmlWithRelatieAndWerkstroomTaak_AreEqual()
        {
            var relatie = new Relatie();
            var werkstroomTaak = new WerkstroomTaak { Memo = "TestMemoTekst" };

            var parser = new HtmlKeyWordVervanger("<HTML>[relatie.userdisplay] - [werkstroomtaak.memo]</HTML>",
                                                  new Dictionary<string, object> { { "relatie", relatie }, { "werkstroomtaak", werkstroomTaak } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
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
                var parser = new HtmlKeyWordVervanger(text,
                                                      new Dictionary<string, object>
                                                          {
                                                              {"relatie", relatie},
                                                              {"werkstroomTaak", werkstroomTaak}
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

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_DoubleKeyWords_Equals()
        {
            var relatie = new Relatie();
            var werkstroomTaak = new WerkstroomTaak { Memo = "TestMemoTekst" };
            var parser = new HtmlKeyWordVervanger("<HTML>[relatie.userdisplay] - [werkstroomtaak.memo]</HTML>"
                                                  , new Dictionary<string, object> { { "relatie", relatie }, { "werkstroomtaak", werkstroomTaak } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Nieuwe relatie - TestMemoTekst</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_SingleAccolade_Equals()
        {
            var relatie = new Relatie();
            var werkstroomTaak = new WerkstroomTaak { Memo = "TestMemoTekst" };
            var parser = new HtmlKeyWordVervanger("<HTML>[{Geachte }relatie.userdisplay]</HTML>"
                                                  , new Dictionary<string, object> { { "relatie", relatie }, { "werkstroomTaak", werkstroomTaak } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Geachte Nieuwe relatie</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_DoubleAccolade_Equals()
        {
            var relatie = new Relatie();
            var werkstroomTaak = new WerkstroomTaak { Memo = "TestMemoTekst" };
            var parser = new HtmlKeyWordVervanger("<HTML>[{Geachte }relatie.userdisplay{,}]</HTML>"
                                                  , new Dictionary<string, object> { { "relatie", relatie }, { "werkstroomTaak", werkstroomTaak } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Geachte Nieuwe relatie,</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_DoubleKeyWordAndDoubleAccolade_Equals()
        {
            var relatie = new Relatie();
            var werkstroomTaak = new WerkstroomTaak { Memo = "TestMemoTekst" };
            var parser = new HtmlKeyWordVervanger("<HTML>[{Geachte }relatie.userdisplay{,}] groot nieuws! Lees: [{Komt die memo }werkstroomtaak.memo]</HTML>"
                                                  , new Dictionary<string, object> { { "relatie", relatie }, { "werkstroomtaak", werkstroomTaak } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Geachte Nieuwe relatie, groot nieuws! Lees: Komt die memo TestMemoTekst</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_SingleKeyWordDefaultTekst_Equals()
        {
            var relatie = new Relatie();
            var parser = new HtmlKeyWordVervanger("<HTML>[contact.userdisplay;Geachte heer]</HTML>"
                                                  , new Dictionary<string, object> { { "relatie", relatie } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Geachte heer</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_SingleKeyWordWithDoubleAccoladeAndContact_Equals()
        {
            var relatie = new Relatie();
            var parser = new HtmlKeyWordVervanger("<HTML>[{Geachte }contact.userdisplay{ van bedrijf }][relatie.userdisplay]</HTML>"
                                                  , new Dictionary<string, object> { { "contact", relatie }, { "relatie", relatie } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Geachte Nieuwe relatie van bedrijf Nieuwe relatie</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_SingleKeyWordWithAccoladeAndDefault_Equals()
        {
            var relatie = new Relatie();
            var parser = new HtmlKeyWordVervanger("<HTML>[{Geachte }contact.userdisplay;Geachte heer]</HTML>"
                                                  , new Dictionary<string, object> { { "relatie", relatie } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Geachte Geachte heer</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_DoubleKeyWordInSingleKeyWord_Equals()
        {
            const string template = "<td>[{<a href=\"mailto:[relatie.userdisplay]\">}relatie.userdisplay;-{</a>}]</td>";
            var relatie = new Relatie();

            var parser = new HtmlKeyWordVervanger(template, new Dictionary<string, object> { { "relatie", relatie } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<td><a href=\"mailto:Nieuwe relatie\">Nieuwe relatie</a></td>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_SetHtmlTwice_()
        {
            var relatie = new Relatie();

            var parser = new HtmlKeyWordVervanger("<HTML>[{Geachte }contact.userdisplay;Geachte heer]</HTML>",
                                                  new Dictionary<string, object> { { "relatie", relatie } });
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML>Geachte Geachte heer</HTML>", html);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void SetAccoladeKeyWordConverter_SetAccoladeAndEmptyKeyWord_Equals()
        {
            var parser = new HtmlKeyWordVervanger("<HTML>[{Geachte }contact.userdisplay]</HTML>",
                                                  new Dictionary<string, object>());
            parser.SetKeyWordConverter(new AccoladeKeyWordConverter());
            var html = parser.GetHtml();
            Assert.AreEqual("<HTML></HTML>", html);
        }
    }

    public class HtmlKeyWordVervanger
    {
        private KeyWordConverter _keyWordConverter;
        protected KeyWordConverter KeyWordConverter
        {
            get
            {
                if (_keyWordConverter == null)
                {
                    KeyWordConverter = new DefaultKeyWordConverter();
                }
                return _keyWordConverter;
            }
            set { _keyWordConverter = value; }
        }

        protected string Html { get; set; }

        protected Dictionary<string, object> StringReferenceToBusinessObjectDictionary { get; set; }

        public HtmlKeyWordVervanger(string html, Dictionary<string, object> stringReferenceToBusinessObjectDictionary)
        {
            Html = html;
            StringReferenceToBusinessObjectDictionary = stringReferenceToBusinessObjectDictionary;
        }

        public void SetKeyWordConverter(KeyWordConverter keyWordConverter)
        {
            KeyWordConverter = keyWordConverter;
        }

        private KeyWordConverter GetConverter()
        {
            KeyWordConverter.SetStringReferenceToBusinessObjectDictionary(StringReferenceToBusinessObjectDictionary);
            return KeyWordConverter;
        }

        public string GetHtml(bool parsed = true)
        {
            return parsed ? ParseHtml(Html) : Html;
        }

        public void SetHtml(string html)
        {
            Html = html;
            KeyWordConverter = (KeyWordConverter)Activator.CreateInstance(KeyWordConverter.GetType());
            KeyWordConverter.SetStringReferenceToBusinessObjectDictionary(StringReferenceToBusinessObjectDictionary);
        }

        private string ParseHtml(string html)
        {
            return GetMatchedKeyWord(html);
        }
        
        private string GetMatchedKeyWord(string keyWord)
        {
            var converter = GetConverter();
            return converter.Convert(keyWord);
        }
    }

    public class KeyWordConverter
    {
        public void SetStringReferenceToBusinessObjectDictionary(Dictionary<string, object> stringReferenceToBusinessObjectDictionary)
        {
            StringReferenceToBusinessObjectDictionary = stringReferenceToBusinessObjectDictionary;
        }

        protected Dictionary<string, object> StringReferenceToBusinessObjectDictionary { get; set; }

        protected virtual Char BusinessObjectSeparator
        {
            get
            {
                return System.Convert.ToChar(".");
            }
        }

        public virtual string Convert(string keyWord)
        {
            return string.Empty;
        }

        protected string GetBusinessObjectProperty(BusinessObject businessObject, string businessObjectProperty)
        {
            var propertyValue = string.Empty;
            var type = businessObject.GetType();
            var propertyInfo = type.GetProperty(businessObjectProperty,
                                                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(businessObject, new object[] { });
                propertyValue = System.Convert.ToString(value);
            }
            return propertyValue;
        }

        protected string GetMatchedKeyWordFromBusinessObject(string strippedKeyWord)
        {
            var matchedKeyWord = string.Empty;
            var splittedKeyWord = strippedKeyWord.Split(BusinessObjectSeparator);
            if (splittedKeyWord.Count() < 2) return string.Empty;
            var businessObjectString = splittedKeyWord[0];
            var businessObjectPropertyString = splittedKeyWord[1];

            if (StringReferenceToBusinessObjectDictionary.ContainsKey(businessObjectString))
            {
                var businessObjectUntyped = StringReferenceToBusinessObjectDictionary[businessObjectString];
                var businessObject = businessObjectUntyped as BusinessObject;
                if (businessObject != null)
                    matchedKeyWord = GetBusinessObjectProperty(businessObject, businessObjectPropertyString);
            }
            return matchedKeyWord;
        }
    }

    internal sealed class DefaultKeyWordConverter : KeyWordConverter
    {
        public override string Convert(string keyWord)
        {
            var matchedKeyWord = string.Empty;

            var strippedKeyWord = keyWord.Substring(1, keyWord.Length - 1).Substring(0, keyWord.Length - 2);

            if (strippedKeyWord.Contains(BusinessObjectSeparator))
                matchedKeyWord = GetMatchedKeyWordFromBusinessObject(strippedKeyWord);

            return matchedKeyWord;
        }
    }
    
    public sealed class AccoladeKeyWordConverter : KeyWordConverter
    {
        public override string Convert(string keyWord)
        {
            var tagsCollection = StringSplitter(keyWord);

            foreach (var tag in tagsCollection)
            {
                SetConvertedText(tag);
            }

            return tagsCollection.Aggregate(keyWord, (current, tag) => current.Replace(tag.Tag, tag.ConvertedText));
        }

        private void SetConvertedText(Tags tag)
        {
            if (tag.Children != null && tag.Children.Any())
                SetConvertedTextWithChildren(tag);

            if (tag.BeginChar == "[" && (tag.Children == null || !tag.Children.Any()))
            {
                SetConvertedTextWithBracketAndNoChildren(tag);
                return;
            }

            if (tag.BeginChar == "{")
            {
                SetConvertedTextWithAccolade(tag);
                return;
            }

            if (!tag.StrippedText.IsNullOrEmpty())
                SetConvertedTextWithStrippedText(tag);
        }

        private void SetConvertedTextWithStrippedText(Tags tag)
        {
            if (tag.StrippedText.Contains(";"))
            {
                var splittedStrippedText = tag.StrippedText.Split(System.Convert.ToChar(";"));
                var convertedStrippedText = GetMatchedKeyWordFromBusinessObject(splittedStrippedText[0]);
                if (convertedStrippedText.IsNullOrEmpty()) convertedStrippedText = splittedStrippedText[1];
                tag.ConvertedText = tag.ConvertedText.Replace(tag.StrippedText, convertedStrippedText);
            }
            tag.ConvertedText = tag.ConvertedText.Replace(tag.StrippedText,
                                                          GetMatchedKeyWordFromBusinessObject(tag.StrippedText));
        }

        private void SetConvertedTextWithAccolade(Tags tag)
        {
            if (tag.Ignore)
                tag.ConvertedText = string.Empty;
            else if (tag.Children != null && tag.Children.Any())
            {
                foreach (var child in tag.Children)
                    SetConvertedText(child);
            }
            else
            {
                tag.ConvertedText = tag.Tag.Substring(1, tag.Tag.Length - 1)
                                       .Substring(0, tag.Tag.Length - 2);
            }
        }

        private void SetConvertedTextWithBracketAndNoChildren(Tags tag)
        {
            if (tag.Tag.Count() < 2) return;
            var strippedKeyWord = tag.Tag.Substring(1, tag.Tag.Length - 1)
                                     .Substring(0, tag.Tag.Length - 2);
            if (strippedKeyWord.Contains(";"))
            {
                var splittedStrippedText = strippedKeyWord.Split(System.Convert.ToChar(";"));
                var convertedStrippedText = GetMatchedKeyWordFromBusinessObject(splittedStrippedText[0]);
                if (convertedStrippedText.IsNullOrEmpty()) convertedStrippedText = splittedStrippedText[1];
                tag.ConvertedText = convertedStrippedText;
            }
            else
                tag.ConvertedText = GetMatchedKeyWordFromBusinessObject(strippedKeyWord);
        }

        private void SetConvertedTextWithChildren(Tags tag)
        {
            if (!tag.StrippedText.IsNullOrEmpty() &&
                (!tag.StrippedText.Contains(";") &&
                 GetMatchedKeyWordFromBusinessObject(tag.StrippedText).IsNullOrEmpty()))
            {
                foreach (var tagse in tag.Children.Where(x => x.BeginChar == "{"))
                {
                    tagse.Ignore = true;
                }
            }

            foreach (var child in tag.Children)
            {
                SetConvertedText(child);

                if (tag.ConvertedText.IsNullOrEmpty()) tag.ConvertedText = tag.Tag;
                tag.ConvertedText = tag.ConvertedText.Replace(child.Tag, child.ConvertedText);
                if (CharPossibilities.ContainsKey(tag.ConvertedText.Substring(0, 1)))
                    tag.ConvertedText = tag.ConvertedText.Substring(1, tag.ConvertedText.Length - 1)
                                           .Substring(0, tag.ConvertedText.Length - 2);
            }
        }

        public static Collection<Tags> StringSplitter(string s)
        {
            var tagsCollection = new Collection<Tags>();

            var beginChar = string.Empty;
            var endChar = string.Empty;

            var count = 0;
            var startTagFound = false;
            var savedCount = 0;
            var amountOfStarters = 0;
            var amountOfEnders = 0;
            foreach (var character in s)
            {
                if ((CharPossibilities.ContainsKey(character.ToString(CultureInfo.InvariantCulture)) && beginChar.IsNullOrEmpty())
                    || (!beginChar.IsNullOrEmpty() && character == System.Convert.ToChar(beginChar)))
                {
                    if (!startTagFound)
                    {
                        beginChar = character.ToString(CultureInfo.InvariantCulture);
                        endChar = CharPossibilities[character.ToString(CultureInfo.InvariantCulture)];
                        startTagFound = true;
                        savedCount = count;
                        tagsCollection.Add(new Tags { BeginIndex = savedCount, BeginChar = character.ToString(CultureInfo.InvariantCulture) });
                        count++;
                        continue;
                    }
                    amountOfStarters++;
                }

                if (startTagFound && character == System.Convert.ToChar(endChar))
                {
                    if (amountOfStarters > amountOfEnders)
                    {
                        amountOfEnders++;
                        count++;
                        continue;
                    }
                    startTagFound = false;
                    amountOfStarters = 0;
                    amountOfEnders = 0;
                    var tag = tagsCollection.First(x => x.BeginIndex == savedCount);
                    if (tag != null)
                    {
                        tag.EndIndex = count;
                        tag.EndChar = endChar;
                        tag.Tag = s.Substring(tag.BeginIndex, (tag.EndIndex - tag.BeginIndex) + 1);
                    }
                    endChar = string.Empty;
                    beginChar = string.Empty;
                }

                count++;
            }

            return tagsCollection;
        }

        public static Dictionary<string, string> CharPossibilities
        {
            get
            {
                return new Dictionary<string, string>
                    {
                        {"[", "]"},
                        {"{", "}"}
                    };
            }
        }

        public class Tags
        {
            public string BeginChar;
            public string EndChar;
            public int BeginIndex;
            public int EndIndex;
            public string ConvertedText;
            public bool Ignore;

            private string _tag;
            public string Tag
            {
                get { return _tag; }
                set
                {
                    FindTagWithinTag(value.Substring(1, value.Length - 1).Substring(0, value.Length - 2));
                    _tag = value;
                }
            }

            public void FindTagWithinTag(string s)
            {
                Children = StringSplitter(s);
            }

            public string StrippedText
            {
                get
                {
                    var strippedText = Tag;
                    if (Children.Any())
                    {
                        strippedText = strippedText.Substring(1, Tag.Length - 1).Substring(0, Tag.Length - 2);
                        strippedText = Children.Aggregate(strippedText, (current, tag) => current.Replace(tag.Tag, string.Empty));
                    }
                    return strippedText;
                }
            }

            public Collection<Tags> Children;
        }
    }
}