using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    public class InnolanKolomInstelling
    {
        private string _waarde;
        private string _value0;
        private string _value1;
        private string _value2;
        private string _value3;
        private string _value4;
        private string _value5;
        private string _value6;

        public bool Visible { get { return _value4 == "1"; } }

        public InnolanKolomInstelling(string waarde)
        {
            _waarde = waarde;
            DistribueerWaardeOverValues();
        }

        private void DistribueerWaardeOverValues()
        {
            var kol = _waarde.Split(new[] { ',' });
            _value0 = kol[0];
            _value1 = kol[1];
            _value2 = kol[2];
            _value3 = kol[3];
            _value4 = kol[4];
            _value5 = kol[5];
            _value6 = kol[6];
        }
    }

    [TestClass]
    public class TestInnolanKolomInstelling
    {
        [TestMethod]
        public void Init()
        {
            const string @string = "Typering,74,-1,N,0,0,-1;Vervallen,76,-1,N,0,0,-1;Soort,58,-1,N,0,0,-1;VestigingNummer,70,-1,N,0,0,-1;VennootNummer,75,-1,N,0,0,-1;BeheerderNummer,72,-1,N,0,0,-1;AssistentNummer,115,-1,N,0,0,-1;KwaliteitsNiveau1Nummer,162,-1,N,0,0,-1;KwaliteitsNiveau2Nummer,145,-1,N,0,0,-1;KwaliteitsNiveau3Nummer,134,0,N,1,0,-1;KwaliteitsNiveau4Nummer,116,-1,N,0,0,-1;KwaliteitsNiveau5Nummer,135,-1,N,0,0,-1;KwaliteitsNiveau6Nummer,141,-1,N,0,0,-1;KwaliteitsNiveau1RoulatieDatum,234,-1,N,0,0,-1;KwaliteitsNiveau2RoulatieDatum,217,-1,N,0,0,-1;KwaliteitsNiveau3RoulatieDatum,206,-1,N,0,0,-1;KwaliteitsNiveau4RoulatieDatum,188,-1,N,0,0,-1;KwaliteitsNiveau5RoulatieDatum,207,-1,N,0,0,-1;KwaliteitsNiveau6RoulatieDatum,213,-1,N,0,0,-1;DossierNummer,105,-1,N,0,0,-1;Zoeknaam,81,2,N,1,0,-1;KorteNaam,118,-1,N,0,0,-1;LangeNaam,138,-1,N,0,0,-1;Aanhef,85,1,N,1,0,-1;BsnNummer,133,-1,N,0,0,-1;Adres,127,-1,N,0,0,-1;HuisNummer,90,-1,N,0,0,-1;Toevoeging,148,-1,N,0,0,-1;PostcodeOpgemaakt,76,-1,N,0,0,-1;Plaats,86,-1,N,0,0,-1;PostAdres,127,-1,N,0,0,-1;PostHuisNummer,141,-1,N,0,0,-1;PostToevoeging,199,-1,N,0,0,-1;PostPostcode,127,-1,N,0,0,-1;PostPlaats,112,-1,N,0,0,-1;Voornaam,80,-1,N,0,0,-1;Voorletter,85,-1,N,0,0,-1;Roepnaam,83,-1,N,0,0,-1;Voorvoegsel,96,-1,N,0,0,-1;Tussenvoegsel,108,-1,N,0,0,-1;Achtervoegsel,106,3,N,1,0,-1;Geslacht,73,-1,N,0,0,-1;GeboorteDatum,107,-1,N,0,0,-1;Memo,60,-1,N,0,0,-1;ExternRelatieNummer,105,-1,N,0,0,-1;KvkNummer,92,-1,N,0,0,-1;BtwNummer,95,-1,N,0,0,-1;IdentificatieNummer,127,-1,N,0,0,-1;IdentificatieOmschrijving,148,-1,N,0,0,-1;IdentificatieGeldigTot,137,-1,N,0,0,-1;IdentificatieMemo,117,-1,N,0,0,-1;IdentificatieDatumAfgifte,157,-1,N,0,0,-1;IdentificatiePlaatsAfgifte,156,-1,N,0,0,-1;IdentificatieDatumRegistratie,174,-1,N,0,0,-1;BsnToevoeging,151,-1,N,0,0,-1;IdentificatieDocument,136,-1,N,0,0,-1;LoonheffingNummer,127,-1,N,0,0,-1;LoonheffingNummerToevoeging,183,-1,N,0,0,-1;IclNummer,90,-1,N,0,0,-1;IclNummerToevoeging,149,-1,N,0,0,-1;BtwNummerToevoeging,154,-1,N,0,0,-1;KvKVestigingsNummer,140,-1,N,0,0,-1;Debiteur,73,-1,N,0,0,-1;Crediteur,77,-1,N,0,0,-1;RechtsvormCode,89,-1,N,0,0,-1;IdentificatieTypeCode,111,-1,N,0,0,-1;InspectieCode,76,-1,N,0,0,-1;ComplianceOfficer,75,-1,N,0,0,-1;RedenRelatieSindsCode,118,-1,N,0,0,-1;RedenRelatieTotCode,113,-1,N,0,0,-1;TypeCode,95,-1,N,0,0,-1;BrancheCode,94,-1,N,0,0,-1;TaalCode,52,-1,N,0,0,-1;LandCode,55,-1,N,0,0,-1;TitulatuurCode,78,-1,N,0,0,-1;AlgemeenTelefoonNummer,74,-1,N,0,0,-1;AlgemeenFaxNummer,50,-1,N,0,0,-1;MobielTelefoonNummer,62,-1,N,0,0,-1;AlgemeenEmailAdres,136,-1,N,0,0,-1;Website,71,-1,N,0,0,-1;RelatieSinds,92,-1,N,0,0,-1;RelatieTot,113,-1,N,0,0,-1;DatumInDienst,92,-1,N,0,0,-1;DatumUitDienst,115,-1,N,0,0,-1;LaatsteContact,107,-1,N,0,0,-1;RelatieNummer,133,-1,N,0,0,-1;DienstverbandCode,102,-1,N,0,0,-1;FunctieOmschrijving,67,-1,N,0,0,-1;Kenmerk|1,75,-1,N,0,0,-1;Kenmerk|2,75,-1,N,0,0,-1;Kenmerk|3,75,-1,N,0,0,-1;Kenmerk|4,75,-1,N,0,0,-1;Kenmerk|5,75,-1,N,0,0,-1;Kenmerk|6,75,-1,N,0,0,-1;Kenmerk|7,75,-1,N,0,0,-1;Kenmerk|12,75,-1,N,0,0,-1;Kenmerk|13,75,-1,N,0,0,-1;Kenmerk|14,75,-1,N,0,0,-1;Kenmerk|15,75,-1,N,0,0,-1;Kenmerk|20,75,-1,N,0,0,-1;Kenmerk|21,75,6,N,1,0,-1;Kenmerk|22,75,-1,N,0,0,-1;Kenmerk|23,75,-1,N,0,0,-1;Kenmerk|24,75,-1,N,0,0,-1;Kenmerk|25,75,-1,N,0,0,-1;Kenmerk|26,75,-1,N,0,0,-1;Kenmerk|27,75,-1,N,0,0,-1;Kenmerk|28,75,-1,N,0,0,-1;Kenmerk|29,75,5,N,1,0,-1;Kenmerk|30,75,-1,N,0,0,-1;Kenmerk|31,75,-1,N,0,0,-1;Kenmerk|32,75,-1,N,0,0,-1;Kenmerk|33,75,-1,N,0,0,-1;Kenmerk|80,75,-1,N,0,0,-1;Kenmerk|81,75,4,N,1,0,-1;Kenmerk|82,75,-1,N,0,0,-1;Kenmerk|83,75,-1,N,0,0,-1;Kenmerk|92,75,-1,N,0,0,-1;Kenmerk|93,75,-1,N,0,0,-1;Kenmerk|94,75,-1,N,0,0,-1;Kenmerk|95,75,-1,N,0,0,-1;Kenmerk|96,75,-1,N,0,0,-1;Kenmerk|97,75,-1,N,0,0,-1;Kenmerk|98,75,-1,N,0,0,-1;Kenmerk|99,75,-1,N,0,0,-1;Kenmerk|100,75,-1,N,0,0,-1;Kenmerk|101,75,-1,N,0,0,-1;Kenmerk|102,75,-1,N,0,0,-1;Kenmerk|103,75,-1,N,0,0,-1;Kenmerk|104,75,-1,N,0,0,-1;Kenmerk|105,75,-1,N,0,0,-1;Kenmerk|106,75,-1,N,0,0,-1;Kenmerk|107,75,-1,N,0,0,-1;Kenmerk|108,75,-1,N,0,0,-1;Kenmerk|109,75,-1,N,0,0,-1;Kenmerk|110,75,-1,N,0,0,-1;Kenmerk|90000001,75,-1,N,0,0,-1;Kenmerk|90000002,75,-1,N,0,0,-1;Kenmerk|90000003,75,-1,N,0,0,-1;CTV|-1,75,-1,N,0,0,-1;CTN|-1,75,-1,N,0,0,-1;CTV|0,75,-1,N,0,0,-1;CTN|0,75,-1,N,0,0,-1;CTV|1,75,-1,N,0,0,-1;CTN|1,75,-1,N,0,0,-1;CTV|2,75,-1,N,0,0,-1;CTN|2,75,-1,N,0,0,-1;CTV|3,75,-1,N,0,0,-1;CTN|3,75,-1,N,0,0,-1;CTV|4,75,-1,N,0,0,-1;CTN|4,75,-1,N,0,0,-1;CTV|7,75,-1,N,0,0,-1;CTN|7,75,-1,N,0,0,-1;CTV|8,75,-1,N,0,0,-1;CTN|8,75,-1,N,0,0,-1;CTV|9,75,-1,N,0,0,-1;CTN|9,75,-1,N,0,0,-1;CTV|11,75,-1,N,0,0,-1;CTN|11,75,-1,N,0,0,-1;CTV|12,75,-1,N,0,0,-1;CTN|12,75,-1,N,0,0,-1;CTV|13,75,-1,N,0,0,-1;CTN|13,75,-1,N,0,0,-1;CTV|14,75,-1,N,0,0,-1;CTN|14,75,-1,N,0,0,-1;CTV|15,75,-1,N,0,0,-1;CTN|15,75,-1,N,0,0,-1;CTV|16,75,-1,N,0,0,-1;CTN|16,75,-1,N,0,0,-1;CTV|17,75,-1,N,0,0,-1;CTN|17,75,-1,N,0,0,-1;CTV|18,75,-1,N,0,0,-1;CTN|18,75,-1,N,0,0,-1;CTV|19,75,-1,N,0,0,-1;CTN|19,75,-1,N,0,0,-1;CTV|20,75,-1,N,0,0,-1;CTN|20,75,-1,N,0,0,-1;CTV|21,75,-1,N,0,0,-1;CTN|21,75,-1,N,0,0,-1;CTV|22,75,-1,N,0,0,-1;CTN|22,75,-1,N,0,0,-1;CTV|23,75,7,N,1,0,-1;CTN|23,75,8,N,1,0,-1;CTV|24,75,-1,N,0,0,-1;CTN|24,75,-1,N,0,0,-1;CTV|25,75,-1,N,0,0,-1;CTN|25,75,-1,N,0,0,-1;CTV|26,75,-1,N,0,0,-1;CTN|26,75,-1,N,0,0,-1;CTV|27,75,-1,N,0,0,-1;CTN|27,75,-1,N,0,0,-1;ClientNummer,75,-1,N,0,0,-1";
            var innolanKolomInstellingCollection = new Collection<InnolanKolomInstelling>();
            foreach (var stringetje in @string.Split(';'))
            {
                innolanKolomInstellingCollection.Add(new InnolanKolomInstelling(stringetje));
            }

            var innolanKolomInstellings = innolanKolomInstellingCollection.Where(x => x.Visible);
            Assert.AreEqual(9, innolanKolomInstellings.Count());
        }
    }
}