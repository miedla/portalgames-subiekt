using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace subiekt_sfera_test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            InsERT.GT gt = new InsERT.GT();
            InsERT.Subiekt sgt;
            gt.Produkt = InsERT.ProduktEnum.gtaProduktSubiekt;

            gt.Serwer = ConfigConnection.ServerGt;//"(local)\\INSERTGT";
            gt.Baza = ConfigConnection.BazaGt;//"test3";
            gt.Autentykacja = InsERT.AutentykacjaEnum.gtaAutentykacjaWindows;//gtaAutentykacjaMieszana;
            gt.Operator = ConfigConnection.OperatorGt;//"Szef";
            gt.OperatorHaslo = ConfigConnection.OperatorGThaslo;//"";
            Debug.WriteLine("gt.Polaczenie: " + gt.Polaczenie.ConnectionString); 
            
            sgt = (InsERT.Subiekt) gt.Uruchom((int) InsERT.UruchomDopasujEnum.gtaUruchomDopasuj, (int) InsERT.UruchomEnum.gtaUruchom);

//            sgt.KontrahenciManager.DodajKontrahenta();

            Utils.WstawDokumentPrzyjeciaPlatnosci(sgt,1,4,"przykladowy tytul",15,"PLN");
            Utils.WstawDokumentWystawieniaPlatnosci(sgt,1,4,"przykladowy tytul",15,"PLN");
            //Utils.DodajKontrahenta(sgt, "Ukaszz", "testowy ze sfery", "9123sd", "Poczesna", "Szkolna", "39");
           // Utils.GetUsersFromPortalGames(sgt);

            //sgt.Okno.Widoczne = true;

            Console.ReadKey();
        }
    }
}
