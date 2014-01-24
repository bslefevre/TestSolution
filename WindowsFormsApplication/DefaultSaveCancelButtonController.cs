namespace WindowsFormsApplication
{
    //public class DefaultSaveCancelButtonController : IDisposable
    //{
    //    public Action RefreshAction;
    //    public Image OpslaanButtonImage { set { OpslaanBarButtonItem.Glyph = value; } }
    //    public Image AnnulerenButtonImage { set { AnnulerenBarButtonItem.Glyph = value; } }

    //    private RibbonControl _ribbonControl;
    //    private BarButtonItem _opslaanBarButtonItem;
    //    private BarButtonItem _annulerenBarButtonItem;
    //    protected IEnumerable<BusinessObject> BusinessObjectCollection { get; set; }
    //    protected XtraUserControl XtraUserControl { get; set; }
    //    protected BarButtonItem OpslaanBarButtonItem
    //    {
    //        get
    //        {
    //            if (_opslaanBarButtonItem == null)
    //            {
    //                OpslaanBarButtonItem = new BarButtonItem {Caption = "Opslaan", Enabled = false};
    //                OpslaanBarButtonItem.ItemClick += OpslaanAnnulerenBarButtonItem_ItemClick;
    //            }
    //            return _opslaanBarButtonItem;
    //        }
    //        set { _opslaanBarButtonItem = value; }
    //    }
    //    protected BarButtonItem AnnulerenBarButtonItem
    //    {
    //        get
    //        {
    //            if (_annulerenBarButtonItem == null)
    //            {
    //                AnnulerenBarButtonItem = new BarButtonItem { Caption = "Annuleren", Enabled = false };
    //                AnnulerenBarButtonItem.ItemClick += OpslaanAnnulerenBarButtonItem_ItemClick;
    //            }
    //            return _annulerenBarButtonItem;
    //        }
    //        set { _annulerenBarButtonItem = value; }
    //    }
    //    protected RibbonControl RibbonControl
    //    {
    //        get
    //        {
    //            if (_ribbonControl == null)
    //            {
    //                RibbonControl = new RibbonControl {ShowToolbarCustomizeItem = false};
    //            }
    //            return _ribbonControl;
    //        }
    //        set { _ribbonControl = value; }
    //    }

    //    public DefaultSaveCancelButtonController(IEnumerable<BusinessObject> businessObjectCollection, XtraUserControl xtraUserControl)
    //    {
    //        BusinessObjectCollection = businessObjectCollection;
    //        XtraUserControl = xtraUserControl;

    //        BusinessObjectCollection.AddEvent(BusinessObjectOnOnPropertyChanged);

    //        AddRibbonControl();
    //        AddOpslaanButton();
    //        AddAnnulerenButton();
    //    }

    //    protected void AddRibbonControl()
    //    {
    //        XtraUserControl.Controls.Add(RibbonControl);
    //    }

    //    protected void AddOpslaanButton()
    //    {
    //        RibbonControl.Toolbar.ItemLinks.Add(OpslaanBarButtonItem);
    //    }
    //    protected void AddAnnulerenButton()
    //    {
    //        RibbonControl.Toolbar.ItemLinks.Add(AnnulerenBarButtonItem);
    //    }

    //    protected void OpslaanAnnulerenBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    //    {
    //        if (e.Item == OpslaanBarButtonItem) { SlaOpCollection(); }
    //        if (e.Item == AnnulerenBarButtonItem) { ResetChanges(); }
    //    }

    //    protected void BusinessObjectOnOnPropertyChanged(object sender, string field, object newValue)
    //    {
    //        var businessObject = sender as BusinessObject;
    //        AnnulerenBarButtonItem.Enabled = businessObject != null && businessObject.Changes.Any();
    //        OpslaanBarButtonItem.Enabled = businessObject != null && businessObject.Changes.Any();
    //    }

    //    private void ResetChanges()
    //    {
    //        BusinessObjectCollection.Undo();
    //        OpslaanBarButtonItem.Enabled = false;
    //        AnnulerenBarButtonItem.Enabled = false;
    //        RefreshAction();
    //    }
    //    private void SlaOpCollection()
    //    {
    //        BusinessObjectCollection.SlaOp();
    //    }

    //    public void Dispose()
    //    {
    //        AnnulerenBarButtonItem.ItemClick -= OpslaanAnnulerenBarButtonItem_ItemClick;
    //        OpslaanBarButtonItem.ItemClick -= OpslaanAnnulerenBarButtonItem_ItemClick;
    //        BusinessObjectCollection.RemoveEvent(BusinessObjectOnOnPropertyChanged);
    //    }
    //}

    //public static class BusinessObjectCollectionExtensions
    //{
    //    public static void SlaOp<T>(this IEnumerable<T> tCollection) where T : BusinessObject
    //    {
    //        foreach (var businessObject in tCollection)
    //        {
    //            businessObject.SlaOp();
    //        }
    //    }

    //    public static void Undo<T>(this IEnumerable<T> tCollection) where T : BusinessObject
    //    {
    //        foreach (var businessObject in tCollection)
    //        {
    //            businessObject.Undo();
    //        }
    //    }

    //    public static void RemoveEvent<T>(this IEnumerable<T> tcollection, OnPropertyChangedEvent businessObjectOnOnPropertyChanged) where T : BusinessObject
    //    {
    //        foreach (var businessObject in tcollection)
    //        {
    //            businessObject.OnPropertyChanged -= businessObjectOnOnPropertyChanged;
    //        }
    //    }

    //    public static void AddEvent<T>(this IEnumerable<T> tcollection, OnPropertyChangedEvent businessObjectOnOnPropertyChanged) where T : BusinessObject
    //    {
    //        foreach (var businessObject in tcollection)
    //        {
    //            businessObject.OnPropertyChanged += businessObjectOnOnPropertyChanged;
    //        }
    //    }
    //}
}