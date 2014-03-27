using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using Alure;
using Alure.Base.BL;
using Microsoft.Office.Interop.Excel;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;
using Section = Microsoft.Office.Interop.Word.Section;
using Table = Microsoft.Office.Interop.Word.Table;

namespace TestBusinessObject
{
    public static class AlureBuildNotes
    {
        public static int? ParentId { get; set; }

        public static string ExcelFileName { get; set; }

        public static IOrderedEnumerable<AlureWorkItem> GetAlureWorkItemCollection()
        {
            var tfsUri = new Uri("http://vmtfs2010:8080/tfs/alure");
            var collection = new TfsTeamProjectCollection(tfsUri);
            var store = collection.GetService<WorkItemStore>();
            var project = store.Projects["Alure"];
            var queryFolder = project.QueryHierarchy["Team Queries"] as QueryFolder;
            var query = queryFolder["Bugs opgelost laatste 2 weken"];
            var queryDef = store.GetQueryDefinition(query.Id);
            var queryString = queryDef.QueryText.Replace("@project", "'Alure'");

            var wiQuery = new Query(store, queryString);
            var workItemLinkInfoCollection = wiQuery.RunLinkQuery().ToCollection();

            //var parentWorkItemLinkInfoCollection = workItemLinkInfoCollection.Where(x => x.SourceId == 0);

            Func<WorkItemLinkInfo, bool> wilib = x => x.SourceId == ParentId;

            var childrenWorkItemLinkInfoCollection = workItemLinkInfoCollection.Where(ParentId.HasValue ? wilib : x => x.SourceId > 0);

            //var workItemLinkInfo = childrenWorkItemLinkInfoCollection.FirstOrDefault(x => x.TargetId == 9132);

            var workItemCollection = new Collection<AlureWorkItem>();
            childrenWorkItemLinkInfoCollection.ForEach(x => workItemCollection.Add(new AlureWorkItem(store.GetWorkItem(x.TargetId))));
            return workItemCollection.OrderBy(x => x.Id);


            //var workItem = store.GetWorkItem(workItemLinkInfo.TargetId);

            //var fieldCollection = workItem.Fields;

            //var buildNotes = workItem.Fields["Buildnotes"];

            //if (buildNotes.Value.ToNullCheckString().IsNullOrEmpty())
            //    Console.WriteLine("Vul wat in bij nummer {0}");

            //var sprint = workItem.Fields["Iteration Path"];
            //name + id
            //var fieldCasted = fieldCollection.Cast<Field>().ToCollection(); //.FirstOrDefault(x => x.Name == "Sprint");

            //var dic = new InnolanDictionary<string, object>();

            //fieldCasted.ForEach(x => dic[x.Name] = x.Value);


            //var results = store.Query(queryDef.QueryText.Replace("@project", "'Alure'"));
            //.Cast<WorkItem>()
            //.Where(item => item.Type.Name == "Bug");

            //results = results.OrderByDescending(item => item.Id);
        }

        public class AlureCell : Dictionary<int, Collection<object>>
        {
            
        }

        public class AlureWorkItem
        {
            private readonly WorkItem _workItem;

            public AlureWorkItem(WorkItem workItem)
            {
                _workItem = workItem;
            }

            [ExcelSetting(1)]
            public int Id { get { return _workItem.Id; } }

            [ExcelSetting(2)]
            public string Titel { get { return _workItem.Fields["Title"].Value.ToNullCheckString(); } }

            [ExcelSetting(3)]
            public string TopDeskNummer { get { return _workItem.Fields["TOPdesk nummer"].Value.ToNullCheckString(); } }

            [ExcelSetting(4)]
            public string Klant { get { return _workItem.Fields["Klant"].Value.ToNullCheckString(); } }

            [ExcelSetting(5)]
            public string Buildnotes { get { return HtmlToRtf(_workItem.Fields["Buildnotes"].Value.ToNullCheckString()); } }

            [ExcelSetting(6)]
            public string Sprint { get { return _workItem.Fields["Iteration Path"].Value.ToNullCheckString(); } }

            private static RichTextBox _rtbTemp;

            public static string HtmlToRtf(string html)
            {
                if (html.IsNullOrEmpty()) return String.Empty;

                if (_rtbTemp == null) _rtbTemp = new RichTextBox();
                var wb = new WebBrowser();
                wb.Navigate("about:blank");

                wb.Document.Write(html);
                wb.Document.ExecCommand("SelectAll", false, null);
                wb.Document.ExecCommand("Copy", false, null);

                _rtbTemp.SelectAll();
                _rtbTemp.Paste();

                return _rtbTemp.Text;
            }

            public static Collection<object> Collector(IEnumerable<AlureWorkItem> workItemCollection, string propertyName)
            {
                var objects = new Collection<object>();
                workItemCollection.ForEach(x => objects.Add(x.GetPropertyValue(propertyName)));
                return objects;
            }

            public static AlureCell ConstrueerAlureCell(IOrderedEnumerable<AlureWorkItem> alureWorkItems)
            {
                var alureCell = new AlureCell();
                var propertyInfoCollection =
                    typeof(AlureWorkItem).GetProperties().Where(x => x.CanRead).ToCollection();

                foreach (var propertyInfo in propertyInfoCollection)
                {
                    var columnIndex = propertyInfo.GetCustomAttribute<ExcelSettingAttribute>().ColumnIndex;
                    var name = propertyInfo.Name;
                    alureCell.Add(columnIndex, Collector(alureWorkItems, name));
                }
                return alureCell;
            }
        }

        public static void CreateExcelDocumentWithData(AlureCell alureCell)
        {
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            foreach (var keyValuePair in alureCell)
            {
                var count = 1;
                foreach (var value in keyValuePair.Value)
                {
                    xlWorkSheet.Cells[count, keyValuePair.Key] = value;
                    count++;
                }
            }
            xlWorkBook.SaveAs(string.Format("C:\\{0}.xls", ExcelFileName.IsNullOrEmpty() ? "Buildnotes" : ExcelFileName), XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            ReleaseObject(xlWorkSheet);
            ReleaseObject(xlWorkBook);
            ReleaseObject(xlApp);
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }

    public class ExcelSettingAttribute : Attribute
    {
        public int ColumnIndex;

        public ExcelSettingAttribute(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }
    }

    public class Ding
    {
        public RichTextBox MyRichTextBox { get; set; }
        
        public void DingDong()
        {
            byte[] ba = Encoding.ASCII.GetBytes(MyRichTextBox.Rtf);

            MemoryStream ms = new MemoryStream(ba);

            FlowDocument f = new FlowDocument();

            TextRange tr = new TextRange(f.ContentStart, f.ContentEnd);
            tr.Load(ms, DataFormats.Text);
            ms.Close();

            byte[] bytes = ms.ToArray();
            string documentFullName = @"C:\document.txt";
            var fs = new FileStream(documentFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            var buffer = new byte[fs.Length];
            var read = fs.Read(buffer, 0, (int)fs.Length);

            var list = buffer.ToList();
            list.AddRange(bytes.ToList());

            fs.Write(list.ToArray(), 0, (int)list.Count);
            fs.Close();
        }

        public void DingDong2()
        {
            try
            {
                var winword = new Application();

                //Set animation status for word application
                //winword.ShowWindowsInTaskbar = false;

                //Set status for word application is to be visible or not.
                //winword.Visible = false;

                //Create a missing variable for missing value
                object missing = Missing.Value;

                //Create a new document
                var document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                //Add header into the document
                foreach (Section section in document.Sections)
                {
                    //Get the header range and add the header details.
                    var headerRange = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, WdFieldType.wdFieldPage);
                    headerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = WdColorIndex.wdBlue;
                    headerRange.Font.Size = 10;
                    headerRange.Text = "Header text goes here";
                }

                //Add the footers into the document
                foreach (Section wordSection in document.Sections)
                {
                    //Get the footer range and add the footer details.
                    var footerRange = wordSection.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    footerRange.Font.ColorIndex = WdColorIndex.wdDarkRed;
                    footerRange.Font.Size = 10;
                    footerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    footerRange.Text = "Footer text goes here";
                }

                //adding text to document
                object styleStandard = "Standaard";
                document.Content.SetRange(0, 0);
                document.Content.Text = "This is test document " + Environment.NewLine;
                document.Content.set_Style(ref styleStandard);
                
                //Add paragraph with Heading 1 style
                Paragraph para1 = document.Content.Paragraphs.Add(ref missing);
                object styleHeading1 = "Kop 1";
                para1.Range.set_Style(ref styleHeading1);
                para1.Range.Text = "Para 1 text";
                para1.Range.InsertParagraphAfter();

                //Add paragraph with Heading 2 style
                var para2 = document.Content.Paragraphs.Add(ref missing);
                object styleHeading2 = "Kop 2";
                para2.Range.set_Style(ref styleHeading2);
                para2.Range.Text = "Para 2 text";
                para2.Range.InsertParagraphAfter();

                //Create a 5X5 table and insert some dummy record
                Table firstTable = document.Tables.Add(para1.Range, 5, 5, ref missing, ref missing);

                firstTable.Borders.Enable = 1;
                foreach (Row row in firstTable.Rows)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        //Header row
                        if (cell.RowIndex == 1)
                        {
                            cell.Range.Text = "Column " + cell.ColumnIndex.ToString();
                            cell.Range.Font.Bold = 1;
                            //other format properties goes here
                            cell.Range.Font.Name = "verdana";
                            cell.Range.Font.Size = 10;
                            //cell.Range.Font.ColorIndex = WdColorIndex.wdGray25;                            
                            cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                            //Center alignment for the Header cells
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                        }
                        //Data row
                        else
                        {
                            cell.Range.Text = (cell.RowIndex - 2 + cell.ColumnIndex).ToString();
                        }
                    }
                }

                object filename = @"c:\document.docx";
                document.SaveAs2(filename);
                document.Close();
                // Close word.
                winword.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    [TestClass]
    public class TestAlureBuildNotes
    {
        [TestMethod]
        public void TestWhoohoo()
        {
            AlureBuildNotes.GetAlureWorkItemCollection();
        }

        [TestMethod]
        public void TestWord()
        {
            //var ding = new Ding();
            //ding.MyRichTextBox = new RichTextBox { Text = " Tweede hahaha" };
            //ding.DingDong2();
            AlureBuildNotes.CreateExcelDocumentWithData(AlureBuildNotes.AlureWorkItem.ConstrueerAlureCell(AlureBuildNotes.GetAlureWorkItemCollection()));
        }
    }
}
