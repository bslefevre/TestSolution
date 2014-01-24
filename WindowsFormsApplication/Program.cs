using System;
using System.Windows.Forms;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WindowsFormsApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //var form = new XtraForm { Text = "Test" };
            //form.Controls.Add(new XtraUserControl2 {Dock = DockStyle.Fill});
            //form.ShowDialog();

            //Application.Run(new LAN { Width = 800, Height = 800 });
            Application.Run(new Form1());
        }
    }

    //[TestClass]
    //public class TestClassDing : TestBase
    //{
    //    [TestMethod]
    //    public void TestAllesDan()
    //    {
    //        var form = new XtraForm { Text = "Test", Width = 800, Height = 800};
    //        var xtraUserControl2 = new XtraUserControl2 {Dock = DockStyle.Fill};
    //        form.Controls.Add(xtraUserControl2);
    //        form.ShowDialog();
    //    }
    //}
}