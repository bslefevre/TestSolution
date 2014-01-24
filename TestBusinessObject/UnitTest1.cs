using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Alure;
using Alure.Base.BL;
using Alure.Base.BL.Caching;
using Alure.Base.BL.Integratie.Test;
using Alure.Base.BL.Queries;
using Alure.Base.BL.Test;
using Alure.Base.DA.Helpers;
using Alure.Base.PL;
using Alure.Base.PL.Controllers;
using Alure.CRM.BL.Queries;
using Alure.WS.BL;
using Alure.WS.BL.Queries;
using Alure.WS.DA;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    [TestClass]
    public class UnitTest1 : TestBase
    {
        public class CustomController : Controller<WerkstroomTaak>
        {
            public override void HaalOpData(BusinessObject model)
            {
                var werkstroomTaakCollection = new Collection<WerkstroomTaak>();

                werkstroomTaakCollection.Add(new WerkstroomTaak {TopUid = 1, Tekst = "Tekst"});
                werkstroomTaakCollection.Add(new WerkstroomTaak {TopUid = 2, Tekst = "Tekst2" });
                werkstroomTaakCollection.Add(new WerkstroomTaak {TopUid = 3, Tekst = "Tekst3" });

                SetDataModel(werkstroomTaakCollection);
            }

            public override void RegisterKolommen(string context)
            {
                AddColumn(x => x.TopUid);
                var gridView = AddColumn(x => x.Tekst);
                gridView.Fixed = FixedStyle.Left;

                var gridBand = new GridBand { Name = "TestBand", Caption = "TestBandCap" };

                foreach (var bandedGridColumn in Lijst.Columns.OfType<BandedGridColumn>())
                    bandedGridColumn.OwnerBand = gridBand;

                (Lijst as BandedGridView).Bands.Add(gridBand);
            }
        }

        [SchermNaamAttribute("WerkstroomTaak")]
        public class UO_TestLijst : UO_Lijst<WerkstroomTaak>
        {
            public override void SetController()
            {
                Controller = new CustomController();
                Controller.SetModel(defaultBindingSource); // tmp
                Controller.RegisterLijst(gridControl1); // tmp
                var bandedGridView = new BandedGridView();

                bandedGridView.ConfigureDefaultGridView();
                //bandedGridView.PopupMenuShowing += gridView1_PopupMenuShowing;
                Lijst.GridControl.MainView = bandedGridView;
                Lijst.GridControl.ViewCollection.Add(bandedGridView);

                Controller.SetView(this); // tmp
                Controller.RegisterKolommen("Test"); // tmp
                Controller.HaalOpData(null); // tmp


                var werkstroomTaak = new WerkstroomTaak();
                werkstroomTaak.EinigeTaakVanWerkstroom();

                //Root = 1
                // // Child = 1.1
                // // Child = 1.2
                // //  // Child = 1.2.1
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void BandedGridView_Test()
        {
            var instellingen = new Collection<Instelling>();
            DataLayerFactory.SetQueryHandler<InstellingenByGebruikerQuery, IEnumerable<Instelling>>(query => instellingen);
            DataLayerFactory.SetDataModel(instellingen);

            var uoLijst = new UO_TestLijst();
            var stdForm = new StdForm<WerkstroomTaak>(uoLijst);
            
            stdForm.ShowDialog();
        }
    }

    [TestClass]
    public class UnitTestDA : TestBaseClassDA
    {
        [TestMethod]
        public void WerkstroomTakenBijClientOfOpdrachtQuery_()
        {
            var handler = new WerkstroomTakenBijClientOfOpdrachtQueryHandler();
            QueryHandler.RegisterHandler(typeof(WerkstroomTakenBijClientOfOpdrachtQuery), handler);

            var query = new WerkstroomTakenBijClientOfOpdrachtQuery
                    {
                        GroepNummer = 1,
                        RelatieNummer = 161,
                        AdministratieNummer = 100,
                        ClientNummer = 222204,
                        ClientOpdrachtNummer = 2011,
                        SoortQuery = WerkstroomTaakQuery.SoortQueryWaardes.Opdracht,
                        AfgerondeWerkstromenTonen = true,
                        OpdrachtNummer = new KeyValuePair<int, int>(2011, 2013),
                        SoortIndicator = TaakQuery.SoortIndicatorWaardes.Dagen,
                        Betrokkene = true
                    };
            var werkstroomTaakCollection = QueryHandler.Handle(query).ToCollection();
            Assert.AreEqual(2, werkstroomTaakCollection.Count);
        }
    }

    public class WerkstroomTakenBijClientOfOpdrachtQueryHandler : IQueryHandler<WerkstroomTakenBijClientOfOpdrachtQuery, IEnumerable<WerkstroomTaak>>
    {
        public IEnumerable<WerkstroomTaak> Handle(WerkstroomTakenBijClientOfOpdrachtQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"@a_grpnr", query.GroepNummer},
                    {"@a_relnr", query.RelatieNummer},
                    {"@a_admnr", query.AdministratieNummer},
                    {"@a_cltnr", query.ClientNummer},
                    {"@a_prjnr", query.ClientOpdrachtNummer},
                    {"@a_soort", query.SoortQueryValue},
                    {"@a_penctonen", query.PlanningEnControlTakenTonen ? 1 : 0},
                    {"@a_afgerondtonen", query.AfgerondeWerkstromenTonen ? 1 : 0},
                    {"@a_projectnr", query.OpdrachtNummer.Key},
                    {"@a_totprojectnr", query.OpdrachtNummer.Value},
                    {"@a_factor_of_vast", TaakQuery.FromSoortIndicatorWaarde(query.SoortIndicator)},
                    {"@a_factor1", query.Percentage1},
                    {"@a_factor2", query.Percentage2},
                    {"@a_factor3", query.Percentage3},
                    {"@a_vast1", query.Dagen1},
                    {"@a_vast2", query.Dagen2},
                    {"@a_vast3", query.Dagen3},
                    {"@a_root", DBNull.Value},
                    {"@a_verantw", query.Verantwoordelijke ? 1 : 0},
                    {"@a_uitvoerend", query.Uitvoerende ? 1 : 0},
                    {"@a_activator", query.Activator ? 1 : 0},
                    {"@a_betrokkene", query.Betrokkene ? 1 : 0},
                    {"@a_flw_uid", DBNull.Value}
                };
            
            return AlureDatabaseHelper.GetCollection<WerkstroomTaak>(sql: "[dba].[s01_flowtaak]",
                                                                     businessObject: null,
                                                                     creator: new WerkstroomTaak_DA().CreateBusinessObjectFromRecord,
                                                                     parameters: parameters);
        }
    }

    public class WerkstroomTakenBijClientOfOpdrachtQuery : IQuery<IEnumerable<WerkstroomTaak>>
    {
        public int GroepNummer { get; set; }

        public int RelatieNummer { get; set; }

        public int AdministratieNummer { get; set; }

        public long? ClientNummer { get; set; }

        public int? ClientOpdrachtNummer { get; set; }

        public string SoortQueryValue
        {
            get { return WerkstroomTaakQuery.FromSoortQueryWaarde(SoortQuery); }
            set { SoortQuery = WerkstroomTaakQuery.ToSoortQueryWaarde(value); }
        }

        public WerkstroomTaakQuery.SoortQueryWaardes SoortQuery { get; set; }

        public bool PlanningEnControlTakenTonen { get; set; }

        public bool AfgerondeWerkstromenTonen { get; set; }

        public TaakQuery.SoortIndicatorWaardes? SoortIndicator { get; set; }

        public KeyValuePair<int,int> OpdrachtNummer { get; set; }

        public int Percentage1 { get; set; }
        public int Percentage2 { get; set; }
        public int Percentage3 { get; set; }

        public int Dagen1 { get; set; }
        public int Dagen2 { get; set; }
        public int Dagen3 { get; set; }

        public bool Verantwoordelijke { get; set; }

        public bool Uitvoerende { get; set; }

        public bool Activator { get; set; }

        public bool Betrokkene { get; set; }
    }
}