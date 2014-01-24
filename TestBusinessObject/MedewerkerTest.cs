using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Alure.Base.BL;
using Alure.Base.BL.Caching;
using Alure.Base.BL.Queries;
using Alure.Base.BL.Test;
using Alure.Base.DA.Helpers;
using Alure.CRM.BL;
using Alure.CRM.DA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestBusinessObject
{
    [TestClass]
    public class MedewerkerTest : TestBase
    {
        private Medewerker _medewerker;
        
        protected Medewerker Medewerker
        {
            get
            {
                if (_medewerker == null)
                {
                    DataLayerFactory.SetQueryHandler<InstellingenByGebruikerQuery, IEnumerable<Instelling>>(query => new Collection<Instelling>());
                    DataLayerFactory.SetHaalOpKeyDelegate((key, context) => new Instelling());

                    var relatieType = new RelatieType { TypeCode = "MED", GroepNummer = 1 };
                    DataLayerFactory.SetHaalOpKeyDelegate((key, context) => relatieType);
                    var vestiging = new Vestiging { VestigingNummer = 1, GroepNummer = 1 };
                    var vennoot = new Vennoot { VennootNummer = 1, GroepNummer = 1 };
                    var beheerder = new Beheerder { BeheerderNummer = 1, GroepNummer = 1 };
                    var assistent = new Assistent { AssistentNummer = 1, GroepNummer = 1 };
                    DataLayerFactory.SetHaalOpKeyDelegate((key, context) => vestiging);
                    DataLayerFactory.SetHaalOpKeyDelegate((key, context) => vennoot);
                    DataLayerFactory.SetHaalOpKeyDelegate((key, context) => beheerder);
                    DataLayerFactory.SetHaalOpKeyDelegate((key, context) => assistent);

                    var relatie = new Relatie
                    {
                        TypeCode = "MED",
                        GroepNummer = 1,
                        RelatieNummer = 1,
                        VestigingNummer = 1,
                        Vestiging = vestiging,
                        VennootNummer = 1,
                        Vennoot = vennoot,
                        BeheerderNummer = 1,
                        Beheerder = beheerder,
                        AssistentNummer = 1,
                        Assistent = assistent
                    };

                    DataLayerFactory.SetDataModel(new Collection<Relatie> { relatie });
                    //DataLayerFactory.SetHaalOpKeyDelegate((key, context) => relatie);
                    _medewerker = new Medewerker
                        {
                            GebruikersId = 2,
                            GroepNummer = 1,
                            RelatieNummer = 1
                        };
                }
                return _medewerker;
            }
        }

        protected Mock<IAutorizer> SetAutorizer()
        {
            var autorizerMock = new Mock<IAutorizer>(MockBehavior.Strict);
            Autorizer.InternalAutorizer = () => autorizerMock.Object;

            return autorizerMock;
        }

        protected Mock<IAutorizer> SetAutorisatie(IEnumerable<AutorisatieItem> autorisatieItemCollection)
        {
            var huidigeGebruiker = new Gebruiker(null)
                {
                    GebruikersId = 2,
                    GroepNummer = 1,
                    RelatieNummer = 1
                };

            Gebruiker.SetHuidigeGebruikerProvider(() => huidigeGebruiker);
            
            DataLayerFactory.SetQueryHandler<GeefOrganisatieNiveauAutorisatieVoorGebruikerQuery, IEnumerable<AutorisatieItem>>(query2 => autorisatieItemCollection);

            var autorisatieItem = autorisatieItemCollection.FirstOrDefault();
            if (autorisatieItem != null) DataLayerFactory.SetHaalOpKeyDelegate((key, context) => autorisatieItem);

            return SetAutorizer();
        }

        public AutorisatieItem GeefStandaardAutorisatieItem
        {
            get { return new AutorisatieItem { GebruikersNummer = 2, GroepNummer = 1, OrganisatieNiveauNummer = 100, Entiteit = "MEERDERE_ORGANISATIENIVEAUS" }; }
        }

        [TestMethod]
        public void MedewerkerOrganisatieNiveau1_HeeftAutorisatie_IsTrue()
        {
            var autorisatieItem = new AutorisatieItem
                {
                    GebruikersNummer = 2,
                    GroepNummer = 1,
                    OrganisatieNiveau = OrganisatieNiveauDefinitie.VestigingNiveau,
                    OrganisatieNiveauNummer = 100,
                    Entiteit = "MEERDERE_ORGANISATIENIVEAUS"
                };
            Medewerker.Relatie.VestigingNummer = 100;
            var autorizer = SetAutorisatie(new Collection<AutorisatieItem> { autorisatieItem });
            autorizer.Setup(x => x.IsObjectGeautoriseerd(It.IsAny<int>())).Returns(false);

            var result = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.VestigingNiveau);
            Assert.IsTrue(result);
            Medewerker.Relatie.VestigingNummer = 1;
            _medewerker = null;
        }
        
        [TestMethod]
        public void MedewerkerOrganisatieNiveau2Tot4_HeeftAutorisatie_IsTrue()
        {
            var autorisatieItem = GeefStandaardAutorisatieItem;
            autorisatieItem.OrganisatieNiveau = OrganisatieNiveauDefinitie.VennootNiveau;
            autorisatieItem.OrganisatieNiveauNummer = 101;
            var autorisatieItem2 = GeefStandaardAutorisatieItem;
            autorisatieItem2.OrganisatieNiveau = OrganisatieNiveauDefinitie.VestigingNiveau;
            autorisatieItem2.OrganisatieNiveauNummer = 101;
            var autorisatieItem3 = GeefStandaardAutorisatieItem;
            autorisatieItem3.OrganisatieNiveau = OrganisatieNiveauDefinitie.BeheerderNiveau;
            autorisatieItem3.OrganisatieNiveauNummer = 101;
            var autorisatieItem4 = GeefStandaardAutorisatieItem;
            autorisatieItem4.OrganisatieNiveau = OrganisatieNiveauDefinitie.AssistentNiveau;
            autorisatieItem4.OrganisatieNiveauNummer = 101;

            var autorizer = SetAutorisatie(new Collection<AutorisatieItem> { autorisatieItem, autorisatieItem2, autorisatieItem3, autorisatieItem4 });
            autorizer.Setup(x => x.IsObjectGeautoriseerd(It.IsAny<int>())).Returns(false);
            
            var result2 = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.VennootNiveau);
            var result3 = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.BeheerderNiveau);
            var result4 = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.AssistentNiveau);

            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            _medewerker = null;
        }
        
        [TestMethod]
        public void MedewerkerMetAutorizer_ZonderAutorisatieItem_IsTrue()
        {
            var autorizer = SetAutorisatie(new Collection<AutorisatieItem>());
            autorizer.Setup(x => x.IsObjectGeautoriseerd(It.IsAny<int>())).Returns(true);

            var result1 = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.VestigingNiveau);
            var result2 = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.VennootNiveau);
            var result3 = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.BeheerderNiveau);
            var result4 = Medewerker.HeeftAutorisatie(OrganisatieNiveauDefinitie.AssistentNiveau);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            _medewerker = null;
        }

        [TestMethod]
        public void OrganisatieNiveauAutorisatieDictionary_Values_AreEqual()
        {
            var autorizer = SetAutorisatie(new Collection<AutorisatieItem>());
            autorizer.Setup(x => x.IsObjectGeautoriseerd(It.IsAny<int>())).Returns(true);

            var organisatieNiveauAutorisatieDictionary = Medewerker.OrganisatieNiveauAutorisatieDictionary;
            var result1 = organisatieNiveauAutorisatieDictionary[OrganisatieNiveauDefinitie.VestigingNiveau];
            var result2 = organisatieNiveauAutorisatieDictionary[OrganisatieNiveauDefinitie.VennootNiveau];
            var result3 = organisatieNiveauAutorisatieDictionary[OrganisatieNiveauDefinitie.BeheerderNiveau];
            var result4 = organisatieNiveauAutorisatieDictionary[OrganisatieNiveauDefinitie.AssistentNiveau];
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            _medewerker = null;
        }

        [TestMethod]
        public void Medewerker_HeeftAutorisatie_IsTrue()
        {
            var autorisatieItem = GeefStandaardAutorisatieItem;
            autorisatieItem.OrganisatieNiveau = OrganisatieNiveauDefinitie.VestigingNiveau;
            var autorisatieItem2 = GeefStandaardAutorisatieItem;
            autorisatieItem2.OrganisatieNiveau = OrganisatieNiveauDefinitie.VennootNiveau;
            Medewerker.Relatie.VestigingNummer = 1;
            Medewerker.Relatie.VennootNummer = 1;
            var autorizer = SetAutorisatie(new Collection<AutorisatieItem> { autorisatieItem, autorisatieItem2 });
            autorizer.Setup(x => x.IsObjectGeautoriseerd(It.IsAny<int>())).Returns(false);

            var result =
                Medewerker.HeeftAutorisatie(new Collection<int>
                    {
                        OrganisatieNiveauDefinitie.VestigingNiveau,
                        OrganisatieNiveauDefinitie.VennootNiveau
                    });
            Assert.IsTrue(result);
            _medewerker = null;
        }
    }

    public class GeefOrganisatieNiveauAutorisatieVoorGebruikerQuery : ICacheableQuery<IEnumerable<AutorisatieItem>>
    {
        public int GroepNummer { get; set; }
        public long GebruikersId { get; set; }
        public int OrganisatieNiveau { get; set; }
        public string Key { get { return string.Format("{0}|{1}|{2}", GroepNummer, GebruikersId, OrganisatieNiveau); } }
    }

    public class GeefOrganisatieNiveauAutorisatieVoorGebruikerQueryHandler : ICacheableQueryHandler<GeefOrganisatieNiveauAutorisatieVoorGebruikerQuery, AutorisatieItem>
    {
        public IEnumerable<AutorisatieItem> Handle(GeefOrganisatieNiveauAutorisatieVoorGebruikerQuery query)
        {
            return AlureDatabaseHelper.GetCollection("dbo.cs15_relautorisatie",
                                                     new AutorisatieItem_DA().CreateBusinessObjectFromRecord,
                                                     new Dictionary<string, object>
                                                         {
                                                             {"@a_rau_grpnr", query.GroepNummer},
                                                             {"@a_rau_gbr_id", query.GebruikersId},
                                                             {"@a_rau_niv", query.OrganisatieNiveau}
                                                         });
        }
    }

    public class Medewerker : Alure.Uren.BL.Medewerker
    {
        public Dictionary<int, bool> OrganisatieNiveauAutorisatieDictionary
        {
            get
            {
                return new Dictionary<int, bool>
                    {
                        {OrganisatieNiveauDefinitie.VestigingNiveau, Autorizer.IsObjectGeautoriseerd(51053)},
                        {OrganisatieNiveauDefinitie.VennootNiveau, Autorizer.IsObjectGeautoriseerd(51054)},
                        {OrganisatieNiveauDefinitie.BeheerderNiveau, Autorizer.IsObjectGeautoriseerd(51055)},
                        {OrganisatieNiveauDefinitie.AssistentNiveau, Autorizer.IsObjectGeautoriseerd(51056)}
                    };
            }
        }
        
        public bool HeeftAutorisatie(Collection<int> organisatieNiveauCollection)
        {
            var heeftAutorisatieCollection = new Collection<bool>();
            foreach (var organisatieNiveau in organisatieNiveauCollection)
            {
                heeftAutorisatieCollection.Add(HeeftAutorisatie(organisatieNiveau));
            }

            return heeftAutorisatieCollection.All(x => x);
        }

        public bool HeeftAutorisatie(int organisatieNiveau)
        {
            bool heeftAutorisatie;
            OrganisatieNiveauAutorisatieDictionary.TryGetValue(organisatieNiveau, out heeftAutorisatie);

            var organisatieNiveauNummer = GeefOrganisatieNummer(Relatie, organisatieNiveau);

            if (!heeftAutorisatie)
            {
                if (!organisatieNiveauNummer.HasValue) return true;
                switch (organisatieNiveau)
                {
                    case 1: 
                    case 2: 
                    case 3:
                    case 4: return organisatieNiveauNummer == GeefOrganisatieNummer(Gebruiker.HuidigeGebruiker.Relatie(), organisatieNiveau);
                    default:
                        return true;
                }
            }

            var query = new GeefOrganisatieNiveauAutorisatieVoorGebruikerQuery
                {
                    GebruikersId = Gebruiker.HuidigeGebruiker.GebruikersId.Value,
                    GroepNummer = Gebruiker.HuidigeGebruiker.Relatie().GroepNummer.Value,
                    OrganisatieNiveau = organisatieNiveau
                };
            var autorisatieItemCollection = QueryHandler.Handle(query).ToList();

            if (autorisatieItemCollection.All(x => !x.Gekoppeld)) return true;
            
            if (autorisatieItemCollection.Any(x => x.OrganisatieNiveau == organisatieNiveau && x.OrganisatieNiveauNummer == organisatieNiveauNummer && x.Gekoppeld))
                return true;
            return false;
        }

        public int? GeefOrganisatieNummer(Relatie relatie, int organisatieNiveau)
        {
            if (relatie == null) return null;
            switch (organisatieNiveau)
            {
                case OrganisatieNiveauDefinitie.VestigingNiveau:
                    return relatie.VestigingNummer;
                case OrganisatieNiveauDefinitie.VennootNiveau:
                    return relatie.VennootNummer;
                case OrganisatieNiveauDefinitie.BeheerderNiveau:
                    return relatie.BeheerderNummer;
                case OrganisatieNiveauDefinitie.AssistentNiveau:
                    return relatie.AssistentNummer;
                default:
                    return null;
            }
        }
    }

    [TestClass]
    public class TestGeefOrganisatieNiveauAutorisatieVoorGebruikerQuery : TestBase
    {
        [TestMethod]
        public void HaalOpAutorisatieItemCollection()
        {
            var query = new GeefOrganisatieNiveauAutorisatieVoorGebruikerQuery { GebruikersId = 2, GroepNummer = 1, OrganisatieNiveau = 1 };

            var autorisatieItem = new AutorisatieItem { GebruikersNummer = 2, GroepNummer = 1, OrganisatieNiveauNummer = 100, Entiteit = "MEERDERE_ORGANISATIENIVEAUS" };
            DataLayerFactory
                .SetQueryHandler<GeefOrganisatieNiveauAutorisatieVoorGebruikerQuery, IEnumerable<AutorisatieItem>>(
                    query2 => new Collection<AutorisatieItem> { autorisatieItem });
            DataLayerFactory.SetHaalOpKeyDelegate((key, context) => autorisatieItem);

            var autorisatieItemCollection = QueryHandler.Handle(query).ToList();
            var result = autorisatieItemCollection.Count();

            Assert.AreEqual(1, result);
            var first = autorisatieItemCollection.First();
            Assert.IsTrue(first.Gekoppeld);
        }
    }
}