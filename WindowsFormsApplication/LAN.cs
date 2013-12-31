using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Google.API.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApplication
{
    public partial class LAN : Form
    {
        private const string BaseLocation = @"Z:\Games\Images\Portable LAN Game Collection"; // @"C:\Users\bjorn.le.fevre.Hyperion\Documents\Test Directory";

        private const string FolderImageLocation = @"http://sitemaker.umich.edu/imdocs/files/folder.jpg";

        public LAN()
        {
            InitializeComponent();

            var testListViewItemCollection = new Collection<TestListViewItem>();

            var fileInfoCollection = new Collection<FileInfo>();
            var directoryInfoCollection = new Collection<DirectoryInfo>();
            foreach (var fileLocation in Directory.GetFiles(BaseLocation))
            {
                fileInfoCollection.Add(new FileInfo(fileLocation));
            }
            foreach (var directoryLocation in Directory.GetDirectories(BaseLocation))
            {
                directoryInfoCollection.Add(new DirectoryInfo(directoryLocation));
            }

            foreach (var directoryInfo in directoryInfoCollection)
            {
                var exists = File.Exists(string.Format(@"{0}\images\{1}.jpg", BaseLocation, directoryInfo.Name));
                Image image = null;
                if (!exists)
                    image = GetImageFromUrl(directoryInfo.Name, FolderImageLocation);
                else
                {
                    var info = new FileInfo(string.Format(@"{0}\images\{1}.jpg", BaseLocation, directoryInfo.Name));
                    
                    try
                    {
                        image = Image.FromStream(info.Open(FileMode.Open));
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                var testListViewItem = new TestListViewItem(directoryInfo.Name, FolderImageLocation);
                testListViewItem.Image = image;
                if (!imageList1.Images.ContainsKey(testListViewItem.ImageKey))
                    imageList1.Images.Add(testListViewItem.ImageKey, testListViewItem.Image);
                testListViewItemCollection.Add(testListViewItem);
            }

            foreach (var fileInfo in fileInfoCollection)
            {
                var url = string.Empty;
                var exists = File.Exists(string.Format(@"{0}\images\{1}.jpg", BaseLocation, fileInfo.Name.Replace(fileInfo.Extension, string.Empty)));
                Image image = null;
                if (!exists)
                    image = GetImageByFileInfoName(fileInfo);
                else
                {
                    var info = new FileInfo(string.Format(@"{0}\images\{1}.jpg", BaseLocation, fileInfo.Name.Replace(fileInfo.Extension, string.Empty)));
                    try
                    {
                        image = Image.FromStream(info.Open(FileMode.Open));
                    }
                    catch (ArgumentException)
                    {

                    }
                }
                var testListViewItem = new TestListViewItem(fileInfo.Name, url);
                testListViewItem.Image = image;

                if (!imageList1.Images.ContainsKey(testListViewItem.ImageKey) && testListViewItem.Image != null)
                    imageList1.Images.Add(testListViewItem.ImageKey, testListViewItem.Image);
                testListViewItemCollection.Add(testListViewItem);
            }

            listView1.Items.AddRange(testListViewItemCollection.ToArray());
        }

        private Image GetImageByFileInfoName(FileInfo fileInfo)
        {
            var fileInfoName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

            var result = GoogleSearch(string.Format("{0} icon", fileInfoName), null);
            Image imageToReturn = null;

            foreach (SearchResult searchResult in result)
            {
                var imageFromUrl = GetImageFromUrl(searchResult.url, fileInfoName);
                if (imageFromUrl != null) imageToReturn = imageFromUrl;
            }

            return imageToReturn;
        }

        private Image GetImageFromUrl(string imgKey, string imgUrl)
        {
            if (string.IsNullOrEmpty(imgUrl)) return null;
            var webClient = new WebClient();
            var imageBytes = new byte[] { };
            try
            {
                imageBytes = webClient.DownloadData(imgUrl);
            }
            catch (WebException exception)
            {

            }

            return imageBytes.Count() > 0 ? ByteArrayToImage(imageBytes, imgKey) : null;
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn, string imgKey)
        {
            if (byteArrayIn == null) return null;
            Image image = null;
            try
            {
                image = Image.FromStream(new MemoryStream(byteArrayIn));
            }
            catch (ArgumentException)
            {

            }


            if (!Directory.Exists(string.Format(@"{0}\images", BaseLocation)))
                Directory.CreateDirectory(string.Format(@"{0}\images", BaseLocation));
            image.Save(string.Format(@"{0}\images\{1}.jpg", BaseLocation, imgKey));
            return image;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            var listView = ((ListView)sender);
            var selectedListViewItemCollection = listView.SelectedItems;
            foreach (TestListViewItem listViewItem in selectedListViewItemCollection)
            {
                if (listViewItem.Selected)
                {
                    var txt = listViewItem.Text;
                }
            }
        }

        public class TestListViewItem : ListViewItem
        {
            public TestListViewItem(string text, string imgKey)
            {
                Text = text;

                ImageKey = text;
            }

            public Image Image { get; set; }
        }


        public static List<SearchResult> GoogleSearch(string search_expression, Dictionary<string, object> stats_dict)
        {
            var url_template = @"http://ajax.googleapis.com/ajax/services/search/images?v=1.0&rsz=large&safe=active&q={0}&start={1}";
            Uri search_url;
            int[] offsets = { 0, 8, 16, 24, 32, 40, 48 };

            var valueCollection = new List<string>();

            foreach (var offset in offsets)
            {
                search_url = new Uri(string.Format(url_template, search_expression, offset));
                var page = new WebClient().DownloadString(search_url);
                var o = (JObject)JsonConvert.DeserializeObject(page);
                foreach (KeyValuePair<string, JToken> keyValuePair in o)
                {
                    if (keyValuePair.Key == "responseData")
                    {
                        if (!keyValuePair.Value.HasValues) continue;

                        var results = keyValuePair.Value["results"];
                        var values = results.Values<string>("url");
                        valueCollection.AddRange(values);
                    }
                    if (valueCollection.Any()) break;
                }

                if (valueCollection.Any()) break;
            }

            return valueCollection.Select(s => new SearchResult(s, null, null, SearchResult.FindingEngine.google)).ToList();
        }

        public class SearchResult
        {
            public string url;
            public string title;
            public string content;
            public FindingEngine engine;

            public enum FindingEngine { google, bing, google_and_bing };

            public SearchResult(string url, string title, string content, FindingEngine engine)
            {
                this.url = url;
                this.title = title;
                this.content = content;
                this.engine = engine;
            }
        }
    }
}
