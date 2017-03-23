//using MySql.Data.MySqlClient;
//using System;
//using InsERT;
//using System.Collections.Generic;
//using System.Linq;

//namespace subiekt_sfera_test
//{
//    public static class Utils
//    {
//        public static string ServerGt = "KRYSTIAN\\INSERTGT";
//        public static string BazaGt = "test3";
//        public static string OperatorGt = "Szef";
//        public static string OperatorGThaslo = ""; 

//        public static string PortalGamesServer = "8706.m.tld.pl";
//        public static string PortalGamesBaza = "baza8706_11";
//        public static string PortalGamesUser = "admin8706_11";
//        public static string PortalGamesPassword = "0NgFs9%Mg6";

//        public static Dictionary<int, string> panstwa = new Dictionary<int, string>
//        {
//            { 1, "PL" },
//            { 2, "CZ" }

//        };

//        public static void DodajKontrahenta(InsERT.Subiekt sgt, string nazwa, string symbol, string miejscowosc, string ulica, int panstwo, string kodPocztowy, string imie = null, string nazwisko = null)
//        {
//            bool czyIstnieje = true;

//            czyIstnieje = sgt.Kontrahenci.Istnieje(symbol);

//            if (!czyIstnieje)
//            {
//                InsERT.Kontrahent okh = sgt.Kontrahenci.Dodaj();
//                okh.Typ = InsERT.KontrahentTypEnum.gtaKontrahentTypDostOdb;
//                okh.Osoba = true;
                
//                Console.WriteLine("kodPocztowy kontrahenta: " + kodPocztowy);
//                if(nazwa.Length > 50)
//                {
//                    okh.Nazwa = nazwa.Substring(0, 50);
//                    okh.NazwaPelna = nazwa;
//                }
//                else
//                {
//                    okh.Nazwa = nazwa;
//                }

//                if(ulica.Length > 50)
//                {
//                    okh.Ulica = ulica.Substring(0, 50);
//                }
//                else
//                {
//                    okh.Ulica = ulica;
//                }

//                if(kodPocztowy.Length > 8)
//                {
//                    okh.KodPocztowy = kodPocztowy.Substring(0, 8);
//                }
//                else
//                {
//                    okh.KodPocztowy = kodPocztowy;
//                }
                
//                okh.Symbol = symbol;
//                okh.Miejscowosc = miejscowosc;
//                okh.OsobaImie = imie;
//                okh.OsobaNazwisko = nazwisko;
//                okh.Panstwo = panstwo;

//                //Console.WriteLine("Dodano kontrahenta: " + okh.Nazwa);
//                okh.Zapisz();
//                okh.Zamknij();
//            }
//            else
//            {
//                //InsERT.Kontrahent okh = sgt.Kontrahenci.Wczytaj(symbol);
//                //okh.Osoba = true;

//                //okh.Zapisz();
//                //okh.Zamknij();
//                //Console.WriteLine("Kontrahent o symbolu: " + symbol + " juz istnieje");
//            }
//        }

//        public static void GetUsersFromPortalGames(InsERT.Subiekt sgt) 
//        {
//            var portalGamesConnString = new MySqlConnectionStringBuilder
//            {
//                Server = PortalGamesServer,
//                UserID = PortalGamesUser,
//                Password = PortalGamesPassword,
//                Database = PortalGamesBaza
//            };

//            using (MySqlConnection conn = new MySqlConnection(portalGamesConnString.ToString()))
//            {
//                using (MySqlCommand cmd = conn.CreateCommand())
//                {
//                    //string sqlCommand = "SELECT c.user_id, c.address_street, c.address_city, c.address_zip, s.name as country, c.daddress_name, c.daddress_lastname, c.phone, c.company_name, c.loyality_points, c.discount, u.email " +
//                    //"FROM customer c " +
//                    //"JOIN user u ON(c.user_id = u.id) JOIN state s ON (s.id = c.address_state_id);";
//                    string sqlCommand = "SELECT * from baza8706_11.`order` o WHERE o.paid = 1 GROUP BY o.customer_id HAVING COUNT(*) = 1";

//                    cmd.CommandText = sqlCommand;

//                    conn.Open();
//                    MySqlDataReader reader = cmd.ExecuteReader();

//                    while (reader.Read())
//                    {
//                        string id = reader["customer_id"].ToString();
//                        string ulica = reader["address_street"].ToString() == string.Empty ? "brak" : reader["address_street"].ToString();
//                        string miasto = reader["address_city"].ToString() == string.Empty ? "brak" : reader["address_city"].ToString();
//                        string zip = reader["address_zip"].ToString() == string.Empty ? "brak" : reader["address_zip"].ToString();
//                        string panstwo_kod = reader["address_state_id"].ToString() == string.Empty ? "brak" : reader["address_state_id"].ToString();
//                        string nazwa = reader["customer_name"].ToString() == string.Empty ? "brak" : reader["customer_name"].ToString();
//                        string imie = reader["maddress_name"].ToString() == string.Empty ? "brak" : reader["maddress_name"].ToString();
//                        string nazwisko = reader["maddress_lastname"].ToString() == string.Empty ? "brak" : reader["maddress_lastname"].ToString();
//                        string fima = reader["company_name"].ToString() == string.Empty ? "brak" : reader["company_name"].ToString();
//                        string miasto_kod = reader["address_zip"].ToString() == string.Empty ? "brak" : reader["address_zip"].ToString();

//                        int panstwo_id = 1;
//                        if (!string.IsNullOrEmpty(panstwo_kod))
//                        {
//                            panstwo_id = panstwa.Where(p => p.Value.Equals(panstwo_kod)).Select(p => p.Key).FirstOrDefault();
                            
//                        }

//                        DodajKontrahenta(sgt, nazwa, id, miasto, ulica, panstwo_id, miasto_kod, imie, nazwisko);
//                    }
//                    Console.WriteLine("Wszyscy kontrahenci zostali dodani");
//                }
//            }
//        }

//        public static void WstawDokumentPrzyjeciaPlatnosci(InsERT.Subiekt sgt, long id, int idKontrahenta, string tytul, decimal cena, string kurs)
//        {
//            var finDokument = sgt.FinManager.DodajDokumentKasowy(DokFinTypEnum.gtaDokFinTypKP, (int) id);
//            finDokument.Data = DateTime.Now;
//            finDokument.ObiektPowiazanyWstaw(DokFinObiektTypEnum.gtaDokFinObiektKontrahent, idKontrahenta);
//            finDokument.WartoscPoczatkowaWaluta = cena;
//            finDokument.Waluta = kurs;
//            finDokument.Tytulem = tytul;
//            finDokument.Zapisz();
//        }
//        public static void WstawDokumentWystawieniaPlatnosci(InsERT.Subiekt sgt, long id, int idKontrahenta, string tytul, decimal cena, string kurs)
//        {
//            var finDokument = sgt.FinManager.DodajDokumentKasowy(DokFinTypEnum.gtaDokFinTypKW, (int)id);
//            finDokument.Data = DateTime.Now;
//            finDokument.ObiektPowiazanyWstaw(DokFinObiektTypEnum.gtaDokFinObiektKontrahent, idKontrahenta);
//            finDokument.WartoscPoczatkowaWaluta = cena;
//            finDokument.Waluta = kurs;
//            finDokument.Tytulem = tytul;
//            finDokument.Zapisz();
//        }
//    }
//}