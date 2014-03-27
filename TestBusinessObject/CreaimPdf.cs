using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestBusinessObject
{
    [TestClass]
    public class TestCreAimPdf
    {
        private const string TestXml = @"<Attachements ItemId=""8""><Row DocumentId=""111"" /><Row DocumentId=""112"" /><Row DocumentId=""113"" /><Row DocumentId=""114"" /></Attachments>";

        [TestMethod]
        public void Test()
        {
            var mock = new Mock<IWebServiceCall>(MockBehavior.Loose);
            mock.Setup(x => x.WebServiceCall()).Returns(TestXml);
            var creaimPdf = new CreAimPdf(mock.Object);
            var creaimAangifte = creaimPdf.HaalOpAangifteMetBijlages("1");
            Assert.AreEqual(TestXml, creaimAangifte.Xml);
        }
    }

    public class CreAimPdf
    {
        private readonly IWebServiceCall _webserviceCall;

        public CreAimPdf(IWebServiceCall webServiceCall)
        {
            _webserviceCall = webServiceCall;
        }

        public CreAimAangifte HaalOpAangifteMetBijlages(string documentId)
        {
            // ophalen gegevens van gegevens van CREAIM
            
            // REQUEST
            // <Request><Attachements ItemId="8" /></Request>
            var xml = _webserviceCall.WebServiceCall();

            // RESPONSE
            //<Attachements ItemId="8">
            //   <Row DocumentId="111" />
            //   <Row DocumentId="112" />
            //   <Row DocumentId="113" />
            //   <Row DocumentId="114" />
            //</Attachments>

            var aangifte = new CreAimAangifte(xml);
            VerwerkAangifte(aangifte);

            return aangifte;
        }

        private void VerwerkAangifte(CreAimAangifte aangifte)
        {
            // REQUEST
            // https://<intermediair>.acceptatie-ds.nl/DigitalServices/Services/PdfDocument.ashx?DocumentId=<document id>
            
            var aangiftePdf = _webserviceCall.WebServiceCallVoorPdf(aangifte.AangiftePdf);

            // RESPONSE
            // PDF gegevens stream (byte[]).

            var bijlagePdfCollection = new Collection<string>();
            foreach (var bijlagePdf in aangifte.BijlagePdfCollection)
            {
                bijlagePdfCollection.Add(_webserviceCall.WebServiceCallVoorPdf(bijlagePdf));
            }

            // 1e in aangifte profiel
            AangiftePdfArchiveren(aangiftePdf);

            // rest in bijlage profiel
            foreach (var pdf in bijlagePdfCollection)
            {
                BijlageProfielArchiveren(pdf);
            }

            // in documentset
        }

        public virtual void BijlageProfielArchiveren(object pdf)
        {
        }

        public virtual void AangiftePdfArchiveren(object pdf)
        {
        }
    }

    public class CreAimAangifte
    {
        private Collection<string> _bijlagePdfCollection;

        public CreAimAangifte(string xml)
        {
            Xml = xml;
        }

        public string Xml { get; private set; }
        public string AangiftePdf { get; private set; }
        public Collection<string> BijlagePdfCollection
        {
            get { return _bijlagePdfCollection ?? (_bijlagePdfCollection = new Collection<string>()); }
            private set { _bijlagePdfCollection = value; }
        }
    }

    public class WebserviceCall : IWebServiceCall
    {
        public string WebServiceCall()
        {
            throw new NotImplementedException();
        }

        public string WebServiceCallVoorPdf(string aangifteUrl)
        {
            throw new NotImplementedException();
        }
    }

    public interface IWebServiceCall
    {
        string WebServiceCall();
        string WebServiceCallVoorPdf(string aangifteUrl);
    }
}