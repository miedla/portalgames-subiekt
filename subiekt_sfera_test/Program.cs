using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using InsERT;
using MySql.Data.MySqlClient;

namespace subiekt_sfera_test
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
//                string order_idString = args[0];
                string czyZagranicznaString = args[0];
                int order_id;
                int rodzaj_sklepu;
                //                string numer_zamowienia = args[2];

                //                if (int.TryParse(order_idString, out order_id) && int.TryParse(czyZagranicznaString, out rodzaj_sklepu))
                if ( int.TryParse(czyZagranicznaString, out rodzaj_sklepu))
                {
                    InsERT.GT gt = new InsERT.GT();
                    InsERT.Subiekt sgt;
                    gt.Produkt = InsERT.ProduktEnum.gtaProduktSubiekt;

                    string plikNazwa = "subiekt-web-service-config2-test.xml";
                    string sciezkaPliku = AppDomain.CurrentDomain.BaseDirectory + plikNazwa;//HttpContext.Current.Server.MapPath("~") + uriPliku;

                    Utils.RodzajSklepu? rodzajSklepuTmp = null;
                    switch (rodzaj_sklepu)
                    {
                        case 0:
                            rodzajSklepuTmp = Utils.RodzajSklepu.Sklep;
                            break;
                        case 1:
                            rodzajSklepuTmp = Utils.RodzajSklepu.Shop;
                            break;
                        case 2:
                            rodzajSklepuTmp = Utils.RodzajSklepu.Geshaft;
                            break;
                        case 3:
                            rodzajSklepuTmp = Utils.RodzajSklepu.Hurt;
                            break;
                           
                    }

                    if (rodzajSklepuTmp == null)
                    {
                        Console.WriteLine("rodzajSklepuTmp == null");
                    }

                    Utils.RodzajSklepu rodzajSklepu = (Utils.RodzajSklepu)rodzajSklepuTmp;

                    bool isConfLoaded = LoadConfigConnection(sciezkaPliku, rodzajSklepu);
                    if (!isConfLoaded)
                    {
                        Console.WriteLine("confload error");
                        return;
                    }

                    gt.Serwer = ConfigConnection.ServerGtProperty; //"(local)\\INSERTGT";
                    Console.WriteLine("GT serwer: " + gt.Serwer);
                    gt.Baza = ConfigConnection.BazaGtProperty; //"test3";
                    Console.WriteLine("GT baza: " + gt.Baza);
                    if (ConfigConnection.UzytkownikProperty != "")
                    {
                        gt.Autentykacja = InsERT.AutentykacjaEnum.gtaAutentykacjaMieszana; //gtaAutentykacjaMieszana;
                        gt.Uzytkownik = ConfigConnection.UzytkownikProperty;
                        gt.UzytkownikHaslo = ConfigConnection.UzytkownikHasloProperty;
                        Console.WriteLine("GT uzytkownik: " + gt.Uzytkownik);
                        Console.WriteLine("GT uzytkownik haslo: " + gt.UzytkownikHaslo);
                    }
                    else
                    {
                        gt.Autentykacja = InsERT.AutentykacjaEnum.gtaAutentykacjaWindows; //gtaAutentykacjaMieszana;
                    }
                    gt.Operator = ConfigConnection.OperatorGt; //"Szef";
                    gt.OperatorHaslo = ConfigConnection.OperatorGThaslo;
                    Console.WriteLine("GT operator: " + gt.Operator);
                    Console.WriteLine("GT operator haslo: " + gt.OperatorHaslo);

                    //if(sgt == null)
                    //{
                    //    Console.WriteLine("sgt == null ");
                    //}
                    Console.WriteLine("ConfigConnection.UzytkownikHasloProperty: "+ ConfigConnection.UzytkownikHasloProperty);
                    Console.WriteLine("ConfigConnection.UzytkownikProperty: " + ConfigConnection.UzytkownikProperty);

                    sgt = GetSubiekt();
                    sgt.MagazynId = 1;
                    if (sgt != null)
                    {
                        if (true)
                        {
//                        InsERT.Kontrahent okh = sgt.Kontrahenci.Dodaj();
//                        okh.Typ = KontrahentTypEnum.gtaKontrahentTypDostOdb;
//                        okh.Osoba = true;
//                        okh.Nazwa = "Szymon Malczyk";
//                        okh.NazwaPelna = "Kromek Kromalsky - mistrz pieczywa";
//                        okh.Ulica = "Dworska 8!@#$%^&*() (DSOSW nr 12)";
//                        okh.KodPocztowy = "54-144";
//                        okh.OsobaImie = "Szymon";
//                        okh.Symbol = "Kromek";
//                        okh.Miejscowosc = "Wrocław";
//                        okh.OsobaNazwisko = "Malczyk";
//                        //okh.NIP = NIP;
//                        okh.Email = "szazon@o2.pl";
//                        okh.Panstwo = okh.Panstwo = SlownikEnum.gtaBrak;
////                        okh.Nazwa = companyName;
////                        okh.Osoba = false;
//                        okh.Zapisz();
//                        okh.Zamknij();
                            
                        } // dodawanie kontrahentów
                        if (true)
                        {
                            //Utils.AktualizacjaKontrahentow(sgt);
                            //znajdzDoubla();s
                            //Kontrahent k = Utils.PobierzKontrahenta(order_id);
                            //int resp = Utils.WystawFaktureZaliczkowaKoncowa(sgt, numer_zamowienia, rodzajSklepu, k, order_id);//Utils.ZakupProces(sgt, order_id, rodzajSklepu);
                            //Utils.ZakupProces(sgt, order_id, rodzajSklepu);
                            //Utils.WystawFaktureZaliczkowaTest(sgt);
                            //Console.WriteLine("uruchomil sie");
                            //Console.ReadKey();
                        } // Jakieś krystianowe rzeczy
                        if (true)
                        {
                            
                        }
                    }
                    else
                    {
                        Console.WriteLine("sgt == null");
                    }
                    //Utils.FakturaKoncowaMechanizm(sgt, order_id, czYZagraniczna);//Utils.ZakupProces(sgt, order_id, czYZagraniczna);
                    // Utils.DodajKontrahenta(sgt, "AMP media asdeqeq blablalaslas sdas dsaaq", "124ff4411", "Poczesna", "Szkolna", 1, "42-2620", "AMP media asdeqeq blablalaslas ", "oooooooooqq");
                    //sgt.Zakoncz();
                    //Console.WriteLine("Zakup proces finito, resp: " + resp);

                    //Utils.SendErrorEmail("lukasz.miedla@ampmedia.pl", "test");
                    //Console.WriteLine("mail was send");
                    //switch (resp)
                    //{
                    //    case -3:
                    //        return "Kontrahent == null";
                    //    case -2:
                    //        return "błędna kwota";
                    //    case -1:
                    //        return "blad nie ma towaru";
                    //    case 0:
                    //        return "OK";
                    //    case 1:
                    //        return "faktura koncowa juz istnieje";
                    //    case 2:
                    //        return "faktura_koncowa";
                    //    default:
                    //        return "default return";
                    //}
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
           
            //moj komnetarz
        }

        private static Subiekt GetSubiekt()
        {
            var gt = new GT();
            Subiekt sgt = null;
            try
            {
                gt.Produkt = ProduktEnum.gtaProduktSubiekt;

                gt.Serwer = ConfigConnection.ServerGtProperty; //"(local)\\INSERTGT";
                if (ConfigConnection.ServerGtProperty.Contains("37.187.165"))
                {
                    throw new Exception("Trying to connect to PG server");
                }
                gt.Baza = ConfigConnection.BazaGtProperty;
                if (!string.IsNullOrEmpty(ConfigConnection.UzytkownikProperty))
                {
                    gt.Autentykacja = AutentykacjaEnum.gtaAutentykacjaMieszana; //gtaAutentykacjaMieszana;
                    gt.Uzytkownik = ConfigConnection.UzytkownikProperty;
                    gt.UzytkownikHaslo = ConfigConnection.UzytkownikHasloProperty;
                }
                else
                {
                    gt.Autentykacja = AutentykacjaEnum.gtaAutentykacjaWindows; //gtaAutentykacjaMieszana;
                }
                gt.Operator = ConfigConnection.OperatorGtProperty; //"Szef";
                gt.OperatorHaslo = ConfigConnection.OperatorGtHasloProperty; //"";

                sgt =
                    (Subiekt)
                    gt.Uruchom((int)UruchomDopasujEnum.gtaUruchomDopasuj,
                        (int)UruchomEnum.gtaUruchomWTle);

                sgt.MagazynId = 1;

                return sgt;
            }
            catch (Exception e)
            {
                sgt?.Zakoncz();
                //Utils.SendErrorEmailToDeveloper("Nie udało się uruchomić poprawnie subiekta!");
                return null;
            }
        }



        public static void znajdzDoubla()
        {
            var lista_sklep = System.IO.File.ReadAllLines(@"C:\Users\ampmedia\Desktop\kontrahenci_sklep.txt");
            var lista_shop = System.IO.File.ReadAllLines(@"C:\Users\ampmedia\Desktop\kontrahenci_shop.txt");
            var lista_geschaft = System.IO.File.ReadAllLines(@"C:\Users\ampmedia\Desktop\kontrahenci_geschaft.txt");
            List<string[]> listy = new List<string[]>();
            listy.Add(lista_sklep);
            listy.Add(lista_shop);
            listy.Add(lista_geschaft);
            Console.WriteLine("sklep -> shop && geschaft");
            foreach (string s in lista_sklep)
            {
                if (lista_shop.Contains(s))
                {
                    Console.WriteLine("sklep");
                    Console.WriteLine(s + " shop");
                }
                if (lista_geschaft.Contains(s))
                {
                    Console.WriteLine("sklep");
                    Console.WriteLine(s + " geschaft");
                }
            }
            Console.WriteLine("shop-> geschaft");
            foreach (string s in lista_shop)
            {
                if (lista_geschaft.Contains(s))
                {
                    Console.WriteLine("shop");
                    Console.WriteLine(s + lista_geschaft);
                }
            }
        }
        public static bool LoadConfigConnection(string file_path, Utils.RodzajSklepu rodzaj_sklepu)//bool czyZagraniczna)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file_path);
                string subiekt_server = doc.SelectSingleNode("config/subiekt_server").InnerText;
                string subiekt_db_name = doc.SelectSingleNode("config/subiekt_db_name").InnerText;
                string subiekt_operator = doc.SelectSingleNode("config/subiekt_operator").InnerText;
                string subiekt_operator_haslo = doc.SelectSingleNode("config/subiekt_operator_haslo").InnerText;
                string subiekt_uzytkownik = doc.SelectSingleNode("config/subiekt_uzytkownik").InnerText;
                string subiekt_uzytkownik_haslo = doc.SelectSingleNode("config/subiekt_uzytkownik_haslo").InnerText;
                string portalgames_sklep_server = doc.SelectSingleNode("config/portalgames_sklep_server").InnerText;
                string portalgames_sklep_baza = doc.SelectSingleNode("config/portalgames_sklep_baza").InnerText;
                string portalgames_sklep_uzytkownik = doc.SelectSingleNode("config/portalgames_sklep_uzytkownik").InnerText;
                string portalgames_sklep_haslo = doc.SelectSingleNode("config/portalgames_sklep_haslo").InnerText;
                string portalgames_shop_server = doc.SelectSingleNode("config/portalgames_shop_server").InnerText;
                string portalgames_shop_baza = doc.SelectSingleNode("config/portalgames_shop_baza").InnerText;
                string portalgames_shop_uzytkownik = doc.SelectSingleNode("config/portalgames_shop_uzytkownik").InnerText;
                string portalgames_shop_haslo = doc.SelectSingleNode("config/portalgames_shop_haslo").InnerText;
                string portalgames_geshaft_server = doc.SelectSingleNode("config/portalgames_geshaft_server").InnerText;
                string portalgames_geshaft_baza = doc.SelectSingleNode("config/portalgames_geshaft_baza").InnerText;
                string portalgames_geshaft_uzytkownik = doc.SelectSingleNode("config/portalgames_geshaft_uzytkownik").InnerText;
                string portalgames_geshaft_haslo = doc.SelectSingleNode("config/portalgames_geshaft_haslo").InnerText;
                string portalgames_hurt_server = doc.SelectSingleNode("config/portalgames_hurt_server").InnerText;
                string portalgames_hurt_baza = doc.SelectSingleNode("config/portalgames_hurt_baza").InnerText;
                string portalgames_hurt_uzytkownik = doc.SelectSingleNode("config/portalgames_hurt_uzytkownik").InnerText;
                string portalgames_hurt_haslo = doc.SelectSingleNode("config/portalgames_hurt_haslo").InnerText;

                ConfigConnection.ServerGtProperty = subiekt_server;
                ConfigConnection.BazaGtProperty = subiekt_db_name;
                ConfigConnection.OperatorGtProperty = subiekt_operator;
                ConfigConnection.OperatorGtHasloProperty = subiekt_operator_haslo;
                ConfigConnection.UzytkownikProperty = subiekt_uzytkownik;
                ConfigConnection.UzytkownikHasloProperty = subiekt_uzytkownik_haslo;

                //Sklep
                ConfigConnection.PortalGamesServerProperty = portalgames_sklep_server;
                ConfigConnection.PortalGamesBazaProperty = portalgames_sklep_baza;
                ConfigConnection.PortalGamesUserProperty = portalgames_sklep_uzytkownik;
                ConfigConnection.PortalGamesPasswordProperty = portalgames_sklep_haslo;

                //Shop
                ConfigConnection.PortalGamesShopServerProperty = portalgames_shop_server;
                ConfigConnection.PortalGamesShopBazaProperty = portalgames_shop_baza;
                ConfigConnection.PortalGamesShopUserProperty = portalgames_shop_uzytkownik;
                ConfigConnection.PortalGamesShopPasswordProperty = portalgames_shop_haslo;

                //Geshaft
                ConfigConnection.PortalGamesGeshaftServerProperty = portalgames_geshaft_server;
                ConfigConnection.PortalGamesGeshaftBazaProperty = portalgames_geshaft_baza;
                ConfigConnection.PortalGamesGeshaftUserProperty = portalgames_geshaft_uzytkownik;
                ConfigConnection.PortalGamesGeshaftPasswordProperty = portalgames_geshaft_haslo;

                //Hurt
                ConfigConnection.PortalGamesHurtServerProperty = portalgames_hurt_server;
                ConfigConnection.PortalGamesHurtBazaProperty = portalgames_hurt_baza;
                ConfigConnection.PortalGamesHurtUserProperty = portalgames_hurt_uzytkownik;
                ConfigConnection.PortalGamesHurtPasswordProperty = portalgames_hurt_haslo;

                if (rodzaj_sklepu == Utils.RodzajSklepu.Sklep)//(!czyZagraniczna)
                {
                    Utils.PortalGamesConnSring = new MySqlConnectionStringBuilder
                    {
                        Server = portalgames_sklep_server,
                        UserID = portalgames_sklep_uzytkownik,
                        Password = portalgames_sklep_haslo,
                        Database = portalgames_sklep_baza
                    };
                }

                if (rodzaj_sklepu == Utils.RodzajSklepu.Shop)//else
                {
                    ConfigConnection.PortalGamesServerProperty = portalgames_shop_server;
                    ConfigConnection.PortalGamesBazaProperty = portalgames_shop_baza;
                    ConfigConnection.PortalGamesUserProperty = portalgames_shop_uzytkownik;
                    ConfigConnection.PortalGamesPasswordProperty = portalgames_shop_haslo;

                    Utils.PortalGamesConnSring = new MySqlConnectionStringBuilder
                    {
                        Server = portalgames_shop_server,
                        UserID = portalgames_shop_uzytkownik,
                        Password = portalgames_shop_haslo,
                        Database = portalgames_shop_baza
                    };
                }

                if (rodzaj_sklepu == Utils.RodzajSklepu.Geshaft)
                {
                    ConfigConnection.PortalGamesServerProperty = portalgames_geshaft_server;
                    ConfigConnection.PortalGamesBazaProperty = portalgames_geshaft_baza;
                    ConfigConnection.PortalGamesUserProperty = portalgames_geshaft_uzytkownik;
                    ConfigConnection.PortalGamesPasswordProperty = portalgames_geshaft_haslo;

                    Utils.PortalGamesConnSring = new MySqlConnectionStringBuilder
                    {
                        Server = portalgames_geshaft_server,
                        UserID = portalgames_geshaft_uzytkownik,
                        Password = portalgames_geshaft_haslo,
                        Database = portalgames_geshaft_baza
                    };
                }

                if (rodzaj_sklepu == Utils.RodzajSklepu.Hurt)
                {
                    ConfigConnection.PortalGamesServerProperty = portalgames_hurt_server;
                    ConfigConnection.PortalGamesBazaProperty = portalgames_hurt_baza;
                    ConfigConnection.PortalGamesUserProperty = portalgames_hurt_uzytkownik;
                    ConfigConnection.PortalGamesPasswordProperty = portalgames_hurt_haslo;

                    Utils.PortalGamesConnSring = new MySqlConnectionStringBuilder
                    {
                        Server = portalgames_hurt_server,
                        UserID = portalgames_hurt_uzytkownik,
                        Password = portalgames_hurt_haslo,
                        Database = portalgames_hurt_baza
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("xml parsing error: " + e.ToString());
                return false;
            }
            return true;
        }
    }
}