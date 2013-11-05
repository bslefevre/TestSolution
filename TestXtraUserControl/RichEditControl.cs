using System.Windows;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestXtraUserControl
{
    [TestClass]
    public class RichEditControlTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        public void OpenForm_InsertTestHTML()
        {
            var xtraUserControl = new XtraUserControl
                {
                    Dock = DockStyle.Fill
                };
            var richEditControl = new RichEditControl();

            richEditControl.HtmlText = "<html><head><title></title><style type=\"text/css\">h1	{text-align:center;font-family:Arial, Helvetica, Sans-Serif;}p	{text-indent:20px;}</style></head><body bgcolor = \"#ffffcc\" text = \"Pink\"><h1>Hello, World!</h1><p>You can modify the text in the box to the left any way you like, andthen click the \"Show Page\" button below the box to display theresult here. Go ahead and do this as often and as long as you like.</p><p>You can also use this page to test your Javascript functions and localstyle declarations. Everything you do will be handled entirely by your ownbrowser; nothing you type into the text box will be sent back to theserver.</p><p>When you are satisfied with your page, you can select all text in thetextarea and copy it to a new text file, with an extension ofeither <b>.htm</b> or <b>.html</b>, depending on your Operating System.This file can then be moved to your Web server.</p></body></html>";
            //#000000
            xtraUserControl.Controls.Add(richEditControl);
            richEditControl.Width = (int) SystemParameters.PrimaryScreenWidth;
            richEditControl.Height = (int)SystemParameters.PrimaryScreenHeight;

            var form = new XtraForm() { Text = "InsertTestHTML" };
            form.Controls.Add(xtraUserControl);
            form.ShowDialog();
        }
    }
}