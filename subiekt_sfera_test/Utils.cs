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

        public static async void DodajKontrahenta(InsERT.Subiekt sgt, string nazwa, string nazwaPelna, string symbol, string miejscowosc, string ulica, string nrLokalu, string imie=null, string nazwisko=null, string panstwo=null, string kodPocztowy=null)
        {
            InsERT.Kontrahent okh = await sgt.Kontrahenci.Dodaj();
            okh.Typ = InsERT.KontrahentTypEnum.gtaKontrahentTypDostOdb;
            okh.Nazwa = nazwa;
            okh.NazwaPelna = nazwaPelna;
            okh.Symbol = symbol;
            okh.Miejscowosc = miejscowosc;
            okh.Ulica = ulica;
            okh.NrLokalu = nrLokalu;
            okh.OsobaImie = imie;
            okh.OsobaNazwisko = nazwisko;
            okh.Panstwo = panstwo;
            okh.KodPocztowy = kodPocztowy;
            //okh.Email = email;
            

            okh.Zapisz();
            okh.Zamknij();
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
                    string sqlCommand = "SELECT c.user_id, c.address_street, c.address_city, c.address_zip, s.name as country, c.daddress_name, c.daddress_lastname, c.phone, c.company_name, c.loyality_points, c.discount, u.email " +
                    "FROM customer c " +
                    "JOIN user u ON(c.user_id = u.id) JOIN state s ON (s.id = c.address_state_id);";

                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader["user_id"].ToString();
                        string address_street = reader["address_street"].ToString();
                        string address_city = reader["address_city"].ToString();
                        string address_zip = reader["address_zip"].ToString();
                        string country = reader["country"].ToString();
                        string firstname = reader["daddress_name"].ToString();
                        string lastname = reader["daddress_lastname"].ToString();
                        string phone = reader["phone"].ToString();
                        string company_name = reader["company_name"].ToString();
                        string loyality_points = reader["loyality_points"].ToString();
                        string discount = reader["discount"].ToString();
                        string email = reader["email"].ToString();
                        //DodajKontrahenta(sgt);
                        //
                    }
                }
            }
        }
    }
}
