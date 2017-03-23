using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subiekt_sfera_test
{
    public static class Utils
    {
        public static string serverGT = "(local)\\INSERTGT";
        public static string bazaGT = "test3";
        public static string operatorGT = "Szef";
        public static string operatorGThaslo = "";

        public static string portalGamesServer = "8706.m.tld.pl";
        public static string portalGamesBaza = "baza8706_11";
        public static string portalGamesUser = "admin8706_11";
        public static string portalGamesPassword = "0NgFs9%Mg6";

        public static Dictionary<int, string> panstwa = new Dictionary<int, string>
        {
            { 1, "PL" },
            { 2, "CZ" }

        };

        public static void DodajKontrahenta(InsERT.Subiekt sgt, string nazwa, string symbol, string miejscowosc, string ulica, int panstwo, string kodPocztowy, string imie = null, string nazwisko = null)
        {
            bool czyIstnieje = true;

            czyIstnieje = sgt.Kontrahenci.Istnieje(symbol);

            if (!czyIstnieje)
            {
                InsERT.Kontrahent okh = sgt.Kontrahenci.Dodaj();
                okh.Typ = InsERT.KontrahentTypEnum.gtaKontrahentTypDostOdb;
                okh.Osoba = true;
                
                Console.WriteLine("kodPocztowy kontrahenta: " + kodPocztowy);
                if(nazwa.Length > 50)
                {
                    okh.Nazwa = nazwa.Substring(0, 50);
                    okh.NazwaPelna = nazwa;
                }
                else
                {
                    okh.Nazwa = nazwa;
                }

                if(ulica.Length > 50)
                {
                    okh.Ulica = ulica.Substring(0, 50);
                }
                else
                {
                    okh.Ulica = ulica;
                }

                if(kodPocztowy.Length > 8)
                {
                    okh.KodPocztowy = kodPocztowy.Substring(0, 8);
                }
                else
                {
                    okh.KodPocztowy = kodPocztowy;
                }
                
                okh.Symbol = symbol;
                okh.Miejscowosc = miejscowosc;
                okh.OsobaImie = imie;
                okh.OsobaNazwisko = nazwisko;
                okh.Panstwo = panstwo;

                //Console.WriteLine("Dodano kontrahenta: " + okh.Nazwa);
                okh.Zapisz();
                okh.Zamknij();
            }
            else
            {
                //InsERT.Kontrahent okh = sgt.Kontrahenci.Wczytaj(symbol);
                //okh.Osoba = true;

                //okh.Zapisz();
                //okh.Zamknij();
                //Console.WriteLine("Kontrahent o symbolu: " + symbol + " juz istnieje");
            }
        }

        public static void GetUsersFromPortalGames(InsERT.Subiekt sgt)
        {
            MySqlConnectionStringBuilder portalGamesConnString = new MySqlConnectionStringBuilder();
            portalGamesConnString.Server = portalGamesServer;
            portalGamesConnString.UserID = portalGamesUser;
            portalGamesConnString.Password = portalGamesPassword;
            portalGamesConnString.Database = portalGamesBaza;

            using (MySqlConnection conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    //string sqlCommand = "SELECT c.user_id, c.address_street, c.address_city, c.address_zip, s.name as country, c.daddress_name, c.daddress_lastname, c.phone, c.company_name, c.loyality_points, c.discount, u.email " +
                    //"FROM customer c " +
                    //"JOIN user u ON(c.user_id = u.id) JOIN state s ON (s.id = c.address_state_id);";
                    string sqlCommand = "SELECT * from baza8706_11.`order` o WHERE o.paid = 1 GROUP BY o.customer_id HAVING COUNT(*) = 1";

                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader["customer_id"].ToString();
                        string ulica = reader["address_street"].ToString() == string.Empty ? "brak" : reader["address_street"].ToString();
                        string miasto = reader["address_city"].ToString() == string.Empty ? "brak" : reader["address_city"].ToString();
                        string zip = reader["address_zip"].ToString() == string.Empty ? "brak" : reader["address_zip"].ToString();
                        string panstwo_kod = reader["address_state_id"].ToString() == string.Empty ? "brak" : reader["address_state_id"].ToString();
                        string nazwa = reader["customer_name"].ToString() == string.Empty ? "brak" : reader["customer_name"].ToString();
                        string imie = reader["maddress_name"].ToString() == string.Empty ? "brak" : reader["maddress_name"].ToString();
                        string nazwisko = reader["maddress_lastname"].ToString() == string.Empty ? "brak" : reader["maddress_lastname"].ToString();
                        string fima = reader["company_name"].ToString() == string.Empty ? "brak" : reader["company_name"].ToString();
                        string miasto_kod = reader["address_zip"].ToString() == string.Empty ? "brak" : reader["address_zip"].ToString();

                        int panstwo_id = 1;
                        if (!string.IsNullOrEmpty(panstwo_kod))
                        {
                            panstwo_id = panstwa.Where(p => p.Value.Equals(panstwo_kod)).Select(p => p.Key).FirstOrDefault();
                            
                        }

                        DodajKontrahenta(sgt, nazwa, id, miasto, ulica, panstwo_id, miasto_kod, imie, nazwisko);
                    }
                    Console.WriteLine("Wszyscy kontrahenci zostali dodani");
                }
            }
        }
    }
}
