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

            gt.Serwer = Utils.ServerGt;//"(local)\\INSERTGT";
            gt.Baza = Utils.BazaGt;//"test3";
            gt.Autentykacja = InsERT.AutentykacjaEnum.gtaAutentykacjaWindows;//gtaAutentykacjaMieszana;
            gt.Operator = Utils.OperatorGt;//"Szef";
            gt.OperatorHaslo = Utils.OperatorGThaslo;//"";
            Debug.WriteLine("gt.Polaczenie: " + gt.Polaczenie.ConnectionString); 
            
            sgt = (InsERT.Subiekt) gt.Uruchom((int) InsERT.UruchomDopasujEnum.gtaUruchomDopasuj, (int) InsERT.UruchomEnum.gtaUruchom);

//            sgt.KontrahenciManager.DodajKontrahenta();

<<<<<<< HEAD

//            Utils.DodajKontrahenta(sgt, "Ukaszz", "testowy ze sfery", "9123sd", "Poczesna", "Szkolna", "39");
            Utils.WstawDokumentPrzyjeciaPlatnosci(sgt,1,4, "test", 7,"PLN");
            

//            Utils.WstawDokumentWystawieniaPlatnosci(sgt,1,4, "test", 7,"PLN");
=======
            Utils.DodajKontrahenta(sgt, "lukasz", "9291", "Poczesna", "Szkolna", 1, "42-262", "ukasz", "miedla");
            //Utils.DodajKontrahenta(sgt, "Ukaszz", "testowy ze sfery", "9123sd", "Poczesna", "Szkolna", "39");
            Utils.GetUsersFromPortalGames(sgt);
>>>>>>> origin/master
            //sgt.Okno.Widoczne = true;

            Console.ReadKey();
        }
    }
}
