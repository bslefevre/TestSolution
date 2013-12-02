using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Alure.Base.BL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    [TestClass]
    public class RandomBusinessObjectTest
    {
        [TestMethod]
        public void HeeftSuperUserLijstGenerator()
        {
            var yes = Autorizer.IsObjectGeautoriseerd(51269);
            var yes2 = Autorizer.IsObjectGeautoriseerd("UO_GeneratorLijst", "superuser");
            Assert.IsTrue(yes);
            Assert.IsTrue(yes2);
        }

        public void Controls()
        {
            
        }
    }
}
