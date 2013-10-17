using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestXtraUserControl
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var userControl = new XtraUserControl { Width = 300, Height = 300, Dock = DockStyle.Fill };

            var memoEdit = new MemoEdit { Text = "werkstroomTaakDefinitie.WerkInstructies" };
            memoEdit.Properties.ReadOnly = true;
            var annulerenButton = new SimpleButton { Text = "OK", DialogResult = DialogResult.OK };

            var layoutControl = new LayoutControl { Dock = DockStyle.Fill };
            var lci = layoutControl.AddItem(string.Empty, memoEdit);
            lci.TextVisible = false;
            layoutControl.AddItem(string.Empty, annulerenButton);
            layoutControl.Controls.Add(memoEdit);
            layoutControl.Controls.Add(annulerenButton);

            userControl.Controls.Add(layoutControl);
            var xtraForm = new XtraForm { Text = "_controller.HuidigObject.UserDisplay" };
            xtraForm.Controls.Add(userControl);
            var dialogResult = xtraForm.ShowDialog();
            if (dialogResult == DialogResult.OK) xtraForm.Close();
        }
    }
}
