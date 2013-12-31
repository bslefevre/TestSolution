using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Alure.Base.BL;
using Alure.Base.BL.Caching;
using Alure.Base.BL.Queries;
using Alure.Base.BL.Test;
using Alure.CRM.BL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    [TestClass]
    public class RandomBusinessObjectTest : TestBase
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        public void GetFieldsCategorieRelatie_KenmerkAdded_AreEqual()
        {
            DataLayerFactory.SetDataModel(new Collection<Instelling>());
            DataLayerFactory.SetQueryHandler<InstellingenByGebruikerQuery, IEnumerable<Instelling>>(query => new Collection<Instelling>());

            DataLayerFactory.SetHaalOpDelegate<Kenmerk>(
                (bo, context, key) =>
                new Collection<Kenmerk>
                    {
                        new Kenmerk {IsRelatieKenmerk = true, Omschrijving = "IsRelatieKenmerk"},
                        new Kenmerk {IsConnectieKenmerk = true, Omschrijving = "IsConnectieKenmerk"},
                        new Kenmerk {IsExtraRelatieVeld = true, Omschrijving = "IsExtraRelatieVeld"}
                    });
            var relatieGroep = new RelatieGroep();
            var provider = new LijstGeneratorRelatiesEnConnectiesFieldProvider(relatieGroep);
            var generatorLijstKoloms = provider.GetFields(provider.GetCategorien().SingleOrDefault(x => x.Label == "Relatie")).ToCollection();
            var result = generatorLijstKoloms.Where(x => x.Property == null).ToCollection().Count;
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void GetFieldsCategorieConnectie_KenmerkAdded_AreEqual()
        {
            DataLayerFactory.SetDataModel(new Collection<Instelling>());
            DataLayerFactory.SetQueryHandler<InstellingenByGebruikerQuery, IEnumerable<Instelling>>(query => new Collection<Instelling>());

            DataLayerFactory.SetHaalOpDelegate<Kenmerk>(
                (bo, context, key) =>
                new Collection<Kenmerk>
                    {
                        new Kenmerk {IsRelatieKenmerk = true, Omschrijving = "IsRelatieKenmerk"},
                        new Kenmerk {IsConnectieKenmerk = true, Omschrijving = "IsConnectieKenmerk"},
                        new Kenmerk {IsExtraRelatieVeld = true, Omschrijving = "IsExtraRelatieVeld"}
                    });
            var relatieGroep = new RelatieGroep();
            var provider = new LijstGeneratorRelatiesEnConnectiesFieldProvider(relatieGroep);
            var generatorLijstKoloms = provider.GetFields(provider.GetCategorien().SingleOrDefault(x => x.Label == "Connectie")).ToCollection();
            var result = generatorLijstKoloms.Where(x => x.Property == null).ToCollection().Count;
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void GetFieldsSubRelatieConnectie_KenmerkAdded_AreEqual()
        {
            DataLayerFactory.SetDataModel(new Collection<Instelling>());
            DataLayerFactory.SetQueryHandler<InstellingenByGebruikerQuery, IEnumerable<Instelling>>(query => new Collection<Instelling>());

            DataLayerFactory.SetHaalOpDelegate<Kenmerk>(
                (bo, context, key) =>
                new Collection<Kenmerk>
                    {
                        new Kenmerk {IsRelatieKenmerk = true, Omschrijving = "IsRelatieKenmerk"},
                        new Kenmerk {IsConnectieKenmerk = true, Omschrijving = "IsConnectieKenmerk"},
                        new Kenmerk {IsExtraRelatieVeld = true, Omschrijving = "IsExtraRelatieVeld"}
                    });
            var relatieGroep = new RelatieGroep();
            var provider = new LijstGeneratorRelatiesEnConnectiesFieldProvider(relatieGroep);
            var generatorLijstKoloms = provider.GetFields(provider.GetCategorien().SingleOrDefault(x => x.Label == "SubRelatie")).ToCollection();
            var result = generatorLijstKoloms.Where(x => x.Property == null).ToCollection().Count;
            Assert.AreEqual(2, result);
        }
    }
}
