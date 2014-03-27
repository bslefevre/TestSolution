using System;
using System.Linq;
using Alure.Base.BL;
using TestBusinessObject;

namespace TFSExport
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Vul US nummer in (leeg betekend hele lijst van query):");
            var sprintUsId = Console.ReadLine();

            if (!sprintUsId.IsNullOrEmpty())
            {
                AlureBuildNotes.ParentId = sprintUsId.AsInt();
            }

            Console.WriteLine("De WorkItems worden opgehaald. Dit kan even duren.");

            var alureWorkItemEnumerable = AlureBuildNotes.GetAlureWorkItemCollection();
            var alureWorkItemCollection = alureWorkItemEnumerable.ToCollection();
            
            Console.WriteLine("{0} items gevonden".Formatteer(alureWorkItemCollection.Count));

            Console.WriteLine("Wil je zien welke items geen BuildNotes hebben? (y/n)");
            var showEmptyBuildNotesString = Console.ReadLine();

            if (showEmptyBuildNotesString == "y")
            {
                var amountOfEmptyBuildNotes = alureWorkItemCollection.Count(x => x.Buildnotes.IsNullOrEmpty());
                Console.WriteLine("{0} items hebben geen BuildNotes".Formatteer(amountOfEmptyBuildNotes));
                if (amountOfEmptyBuildNotes > 0)
                {
                    Console.WriteLine("Dat zijn de volgende:");
                    foreach (var alureWorkItem in alureWorkItemCollection.Where(x=>x.Buildnotes.IsNullOrEmpty()))
                    {
                        Console.WriteLine("ID: {0}, Titel: {1}".Formatteer(alureWorkItem.Id, alureWorkItem.Titel));
                    }

                    Console.WriteLine();
                }
            }

            Console.WriteLine("Tijd om het Excel document een naam te geven: (deze komt gewoon in C:\\ te staan, extensie is niet nodig)");
            var excelBestandsnaam = Console.ReadLine();
            AlureBuildNotes.ExcelFileName = excelBestandsnaam;
            Console.WriteLine("Het Excel bestand zal nu gemaakt gaan worden.");
            AlureBuildNotes.CreateExcelDocumentWithData(AlureBuildNotes.AlureWorkItem.ConstrueerAlureCell(alureWorkItemEnumerable));
            Console.WriteLine("Er zijn blijkbaar geen fouten, het document staat klaar op C:\\");
            Console.WriteLine("Succes!");
            Console.ReadKey();
        }
    }
}