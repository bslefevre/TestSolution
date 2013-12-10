using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Alure.Base.BL;
using Alure.Base.BL.Test;
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

        [TestMethod]
        public void HeeftToevoegVerwijderWijzigRechten()
        {
            Autorizer.InternalAutorizer = () => new TestAutorizer(true){ ObjectAutorisatie = id => id==333 };
            // Toevoegen
            var magToevoegen = Autorizer.IsObjectGeautoriseerd(51270);
            //Verwijderen
            var magVerwijderen = Autorizer.IsObjectGeautoriseerd(51271);
            // Wijzigen
            var magWijzigen = Autorizer.IsObjectGeautoriseerd(51272);

            Assert.IsTrue(magToevoegen);
            Assert.IsTrue(magVerwijderen);
            Assert.IsTrue(magWijzigen);
        }
    }
}
