using System.Collections.ObjectModel;
using Alure.Base.BL;
using Alure.CRM.BL;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;

namespace WindowsFormsApplication
{
    public partial class XtraUserControl2 : XtraUserControl
    {
        protected Collection<TestSmallBusinessObject> BusinessObjectCollection { get; set; }

        protected Collection<Land> LandCollection { get; set; }

        public XtraUserControl2()
        {
            var testSmallBusinessObject = new TestSmallBusinessObject {Name = "TestName", Number = 1};
            testSmallBusinessObject.BusinessObjectChangeLogger.AddChange("Name", "TName");
            testSmallBusinessObject.BusinessObjectChangeLogger.AddChange("Number", 2);

            LandCollection = new Collection<Land> {new Land(null) {Omschrijving = "Test"}};

            BusinessObjectCollection = new Collection<TestSmallBusinessObject>
                {
                    testSmallBusinessObject
                };
            InitializeComponent();

            bindingSource1.DataSource = LandCollection;

            gridView1.Columns.Add(new GridColumn { Name = "Omschrijving", FieldName = "Omschrijving", Visible = true });

            var defaultSaveCancelButtonController = new DefaultSaveCancelButtonController(LandCollection, this)
                {
                    RefreshAction = RefreshAction,
                    OpslaanButtonImage = Alure.Base.PL.Properties.Resources.opslaan16,
                    AnnulerenButtonImage = Alure.Base.PL.Properties.Resources.annuleren16
                };
            //defaultSaveCancelButtonController.RefreshAction += RefreshAction;
            //defaultSaveCancelButtonController.OpslaanButtonImage = Alure.Base.PL.Properties.Resources.opslaan16;
            //defaultSaveCancelButtonController.AnnulerenButtonImage = Alure.Base.PL.Properties.Resources.annuleren16;
            var land = LandCollection[0];
            land.Omschrijving = "Test2";

            Leave += (sender, args) => defaultSaveCancelButtonController.Dispose();
        }

        private void RefreshAction()
        {
            gridView1.RefreshData();
        }
    }

    public class TestSmallBusinessObject : BusinessObject
    {
        public TestSmallBusinessObject() : base(null)
        {
            OnPropertyChanged += TestSmallBusinessObjectPropertyChanged;
        }

        private void TestSmallBusinessObjectPropertyChanged(object sender, string field, object newValue)
        {
            var bu = sender as BusinessObject;
        }

        public string Name { get; set; }
        public int Number { get; set; }
    }
}
