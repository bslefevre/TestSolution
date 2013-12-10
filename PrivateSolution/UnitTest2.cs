using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrivateSolution
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = @"Z:\Dropbox\Vihicle Data\CSV\";

            using (OdbcConnection conn = new OdbcConnection(
              "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" +
              path + ";Extensions=asc,csv,tab,txt"))
            {
                using (OdbcCommand cmd =
                  new OdbcCommand("SELECT * FROM FoFo.csv", conn))
                {
                    conn.Open();

                    using (OdbcDataReader dr =
                      cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (dr.Read())
                        {
                            Debug.WriteLine(dr[0].ToString());
                            //Debug.WriteLine(dr[1].ToString());
                            //Debug.WriteLine(dr[2].ToString());

                            //etc...
                        }
                    }
                }
                Console.ReadLine();
            }
        }


        //[Serializable]
        public class RoadTrip
        {
            [XmlAttribute]
            public long Odometer { get; set; }
            [XmlAttribute]
            public long TripAfstand { get; set; }
        }

        //"Odometer (km);Trip afstand;Datum;Hoeveelheid getanked;Eenheid;Prijs per eenheid;Totaal;Gedeeltelijke tank;km/L;Notitie;Octaan- of Cetaangetal;Lokatie;Betaalwijze;Rijcondities;Reset;Categorieën;Vlaggen;Valutacode;Wisselkoers;Breedtegraad;Lengtegraad;ID;Trip Comp Fuel Economy;Trip Comp Avg. Speed;Trip Comp Temperature;Trip Comp Drive Time"
        // http://stackoverflow.com/questions/284324/what-is-the-best-way-to-build-xml-in-c-sharp-code
        [TestMethod]
        public void TestMethod2()
        {
            var lines = File.ReadAllLines(@"Z:\Dropbox\Vihicle Data\CSV\FoFo.csv");

            var roadTripCollection = new Collection<RoadTrip>();
            roadTripCollection.Add(new RoadTrip{Odometer = 10, TripAfstand = 20});
            //var xmlSerializer = new XmlSerializer(typeof (RoadTrip));
            //TextWriter writer = new StreamWriter(@"C:\Temp\Test.xml");
            //xmlSerializer.Serialize(writer, roadTripCollection);
            

            //XmlDocument d = new XmlDocument();




            foreach (var line in lines)
            {
                var splittedLine = line.Split(';');
            }

            
        }

        [TestMethod]
        public void TestMethod3()
        {
            var reader = new XmlSerializer(typeof (RoadTrip));
            var file = new StreamReader(@"Z:\Dropbox\Vihicle Data\CSV\FoFo.csv");
            //var overview = new RoadTrip();
            reader.UnknownAttribute += reader_UnknownAttribute;
            reader.UnknownElement += ReaderOnUnknownElement;
            reader.UnknownNode += reader_UnknownNode;
            reader.UnreferencedObject += reader_UnreferencedObject;
            var iets = reader.Deserialize(file);

        }

        void reader_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            var b = e;
        }

        void reader_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            var b = e;
        }

        private void ReaderOnUnknownElement(object sender, XmlElementEventArgs e)
        {
            var b = e;
        }

        void reader_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            var b = e;
        }


    }
}
