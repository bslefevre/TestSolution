using System.Collections.ObjectModel;
using System.Linq;
using Alure.Base.BL;
using Alure.Base.BL.Caching;
using Alure.Base.BL.Test;
using Alure.CRM.BL;
using Alure.CRM.BL.Queries;
using Alure.WS.BL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    public class WerkstroomUitvoerendeBepaler
    {
        private readonly Taak _taak;

        public WerkstroomUitvoerendeBepaler(Taak taak)
        {
            _taak = taak;
        }

        public string BepaalUitvoerendeTekst()
        {
            var uitvoerendeTekst = string.Empty;
            if (_taak != null)
            {
                var werkstroomTaakDefinitie = _taak.DefinitieUid.HasValue ? WerkstroomTaakDefinitie.HaalOp(_taak.DefinitieUid.Value) : null;
                if (IsWerkstroomTaakDefinitieIngesteldOpVerzameling(werkstroomTaakDefinitie))
                {
                    var relatieGroep = Gebruiker.HuidigeGebruiker.GeefActieveRelatieGroep();
                    var verzameling = Verzameling.HaalOp(relatieGroep.GroepNummer.Value, werkstroomTaakDefinitie.UitvoerendeVerzameling.Value);

                    var verzamelingRelatieCollection = verzameling.GeefRelaties(true);

                    uitvoerendeTekst = verzameling != null ? verzameling.Omschrijving : string.Empty;
                }
                else
                {
                    var taakUitvoerendeCollection = TaakUitvoerende.HaalOp(_taak);
                    if (taakUitvoerendeCollection.Any())
                        uitvoerendeTekst = taakUitvoerendeCollection.Count > 1 ? "Meerdere" : taakUitvoerendeCollection.First().RelatieUserDisplay;
                    else
                        uitvoerendeTekst = "Geen uitvoerende";
                }
            }

            return uitvoerendeTekst;
        }

        private static bool IsWerkstroomTaakDefinitieIngesteldOpVerzameling(WerkstroomTaakDefinitie werkstroomTaakDefinitie)
        {
            return werkstroomTaakDefinitie != null && werkstroomTaakDefinitie.UitvoerendeVerzameling.HasValue &&
                   (!werkstroomTaakDefinitie.UitvoerendeRelatieNummer.HasValue
                    && !werkstroomTaakDefinitie.UitvoerendeOrganisatieNiveau.HasValue
                    && !werkstroomTaakDefinitie.UitvoerendeKwaliteitsNiveau.HasValue
                    && !werkstroomTaakDefinitie.UitvoerendeMedewerker
                    && !werkstroomTaakDefinitie.BijActiverenExtraUitvoerendenSelecteren
                    && !werkstroomTaakDefinitie.UitvoerendeComplianceOfficer
                    && !werkstroomTaakDefinitie.UitvoerendenVanBovenliggend
                    && !werkstroomTaakDefinitie.UitvoerendenVanVoorgaand
                    && !werkstroomTaakDefinitie.UitvoerendePrimaireBetrokkene);
        }
    }

    [TestClass]
    public class TestClass : TestBase
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        public void WerkstroomUitvoerendeBepaler_Init_Equals()
        {
            var uitvoerendeTekst = new WerkstroomUitvoerendeBepaler(null).BepaalUitvoerendeTekst();
            Assert.AreEqual(string.Empty, uitvoerendeTekst);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void WerkstroomUitvoerendeBepaler_IsVerzameling_Equals()
        {
            DataLayerFactory.SetDataModel(new Collection<RelatieGroep> { new RelatieGroep { GroepNummer = 1 } });
            DataLayerFactory.SetQueryHandler<ZijnErActieveMedewerkersInDezeVerzamelingQuery, bool>(query => true);
            DataLayerFactory.SetHaalOpKeyDelegate(
                (key, context) => new Verzameling { VerzamelingNummer = 3, GroepNummer = 1, Omschrijving = "VerzamelingOmschrijving" });
            DataLayerFactory.SetHaalOpKeyDelegate(
                (key, context) => new WerkstroomTaakDefinitie
                {
                    Uid = 2,
                    UitvoerendeVerzameling = 3,
                    UitvoerendeMedewerker = false,
                    BijActiverenExtraUitvoerendenSelecteren = false
                });
            var taak = new Taak { Uid = 1, DefinitieUid = 2 };
            var uitvoerendeTekst = new WerkstroomUitvoerendeBepaler(taak).BepaalUitvoerendeTekst();

            Assert.AreEqual("VerzamelingOmschrijving", uitvoerendeTekst);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void WerkstroomUitvoerendeBepaler_NotAnyTaakUitvoerende_Equals()
        {
            DataLayerFactory.SetQueryHandler<TaakUitvoerendenBijTaakQuery, Collection<TaakUitvoerende>>(query => new Collection<TaakUitvoerende>());
            var taak = new Taak { Uid = 1 };
            var uitvoerendeTekst = new WerkstroomUitvoerendeBepaler(taak).BepaalUitvoerendeTekst();
            Assert.AreEqual("Geen uitvoerende", uitvoerendeTekst);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void WerkstroomUitvoerendeBepaler_MoreThanOneTaakUitvoerende_Equals()
        {
            DataLayerFactory.SetQueryHandler<TaakUitvoerendenBijTaakQuery, Collection<TaakUitvoerende>>(
                query => new Collection<TaakUitvoerende> { new TaakUitvoerende(), new TaakUitvoerende() });
            var taak = new Taak { Uid = 1 };
            var uitvoerendeTekst = new WerkstroomUitvoerendeBepaler(taak).BepaalUitvoerendeTekst();
            Assert.AreEqual("Meerdere", uitvoerendeTekst);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void WerkstroomUitvoerendeBepaler_OneTaakUitvoerende_Equals()
        {
            DataLayerFactory.SetHaalOpKeyDelegate((key, context) => new Relatie { TypeCode = "CON", RelatieNummer = 1, GroepNummer = 1 });
            DataLayerFactory.SetQueryHandler<TaakUitvoerendenBijTaakQuery, Collection<TaakUitvoerende>>(
                query => new Collection<TaakUitvoerende> { new TaakUitvoerende { UitvoerendeGroepNummer = 1, UitvoerendeRelatieNummer = 1 } });
            var taak = new Taak { Uid = 1 };
            var uitvoerendeTekst = new WerkstroomUitvoerendeBepaler(taak).BepaalUitvoerendeTekst();
            Assert.AreEqual("Nieuwe relatie", uitvoerendeTekst);
        }
    }
}