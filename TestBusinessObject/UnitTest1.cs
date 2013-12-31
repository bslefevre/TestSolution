using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApplication;

namespace TestBusinessObject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var xtraUserControl = new XtraUserControl2();
            xtraUserControl.Show();
        }
    }
}
