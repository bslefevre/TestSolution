using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
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

        [TestMethod]
        [TestCategory("UnitTest")]
        public void TestMethod2()
        {
            var userControl = new XtraUserControl { Width = 300, Height = 300, Dock = DockStyle.Fill };

            var gridView1 = new GridView();
            var gridControl1 = new GridControl();
            gridControl1.MainView = gridView1;
            gridControl1.ViewCollection.Add(gridView1);
            gridView1.GridControl = gridControl1;
            gridView1.Columns[0].ColumnEdit = new RepositoryItemTextEdit();
            gridView1.Columns[1].ColumnEdit = new RepositoryItemTextEdit();
            gridView1.DragObjectStart += GridView1OnDragObjectStart;
            gridView1.CustomRowCellEdit += gridView1_CustomRowCellEdit;
            gridControl1.DataSource = new Dictionary<int, string> { { 1, "Waarde1" }, { 2, "Waarde2" }, { 3, "Waarde3" } };
            gridControl1.Dock = DockStyle.Fill;

            var gridControl2 = new GridControl();
            gridControl2.DataSource = new Dictionary<int, string> { { 4, "Waarde4" }, { 5, "Waarde5" }, { 6, "Waarde6" } };
            gridControl2.Dock = DockStyle.Fill;
            gridControl1.DragEnter += gridControl1_DragEnter;
            gridControl2.DragEnter += gridControl1_DragEnter;

            gridControl1.DragDrop += GridControl1OnDragDrop;
            gridControl2.DragDrop += GridControl1OnDragDrop;
            var layoutControl = new LayoutControl { Dock = DockStyle.Left };
            var layoutControl2 = new LayoutControl { Dock = DockStyle.Right };
            var lci = layoutControl.AddItem(string.Empty, gridControl1);
            lci.TextVisible = false;
            lci = layoutControl2.AddItem(string.Empty, gridControl2);
            lci.TextVisible = false;
            layoutControl.Controls.Add(gridControl1);
            layoutControl2.Controls.Add(gridControl2);
            userControl.Controls.Add(layoutControl);
            userControl.Controls.Add(layoutControl2);
            var xtraForm = new XtraForm { Text = "_controller.HuidigObject.UserDisplay" };
            xtraForm.Controls.Add(userControl);
            var dialogResult = xtraForm.ShowDialog();
            if (dialogResult == DialogResult.OK) xtraForm.Close();
        }

        void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            var r = e;
        }

        private void GridView1OnDragObjectStart(object sender, DragObjectStartEventArgs dragObjectStartEventArgs)
        {
            var objectStartEventArgs = dragObjectStartEventArgs;
        }

        private void GridControl1OnDragDrop(object sender, DragEventArgs dragEventArgs)
        {
            var eventArgs = dragEventArgs;
        }

        void gridControl1_DragEnter(object sender, DragEventArgs e)
        {
            var dataObject = e.Data;
        }
    }
}
