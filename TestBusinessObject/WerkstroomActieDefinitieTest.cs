using Alure.WS.BL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Alure.Base.BL.Test;

namespace TestBusinessObject
{
    [TestClass]
    public class WerkstroomActieDefinitieTest : TestBase
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        public void IsPrimaireActieAndDirectStarten_WerkstroomActieDefinitieInit_IsFalse()
        {
            var werkstroomActieDefinitie = new WerkstroomActieDefinitie();
            Assert.IsFalse(werkstroomActieDefinitie.IsPrimaireActie);
            Assert.IsFalse(werkstroomActieDefinitie.DirectStarten);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void IsPrimaireActieEnabled_IsAutomatischUitvoeren_IsFalse()
        {
            var werkstroomActieDefinitie = new WerkstroomActieDefinitie
                {
                    AutomatischUitvoeren = true
                };

            var result = werkstroomActieDefinitie.IsPrimaireActieEnabled;
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void IsDirectStartenEnabled_IsAutomatischUitvoeren_IsFalse()
        {
            var werkstroomActieDefinitie = new WerkstroomActieDefinitie
            {
                AutomatischUitvoeren = true
            };

            var result = werkstroomActieDefinitie.IsDirectStartenEnabled;
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void IsAutomatischeActieEnabled_IsPrimaireActie_IsFalse()
        {
            var werkstroomActieDefinitie = new WerkstroomActieDefinitie
            {
                ActieSoort = WerkstroomActieDefinitie.ActieSoortWaardes.Stempel,
                IsPrimaireActie = true
            };

            var result = werkstroomActieDefinitie.AutomatischUitvoeren;
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void IsDirectStartenEnabled_IsPrimaireActie_IsTrue()
        {
            var werkstroomActieDefinitie = new WerkstroomActieDefinitie {IsPrimaireActie = true};
            var result = werkstroomActieDefinitie.IsDirectStartenEnabled;
            Assert.IsTrue(result);
        }
    }
}
