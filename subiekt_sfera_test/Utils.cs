using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using InsERT;

namespace subiekt_sfera_test
{
    public static class Utils
    {
        public static void DodajKontrahenta(InsERT.Subiekt sgt, string nazwa, string nazwaPelna, string symbol,
            string miejscowosc, string ulica, string nrLokalu, string imie = null, string nazwisko = null,
            string panstwo = null, string kodPocztowy = null)
        {
            try
            {
                InsERT.Kontrahent okh = sgt.Kontrahenci.Dodaj();
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void GetUsersFromPortalGames(InsERT.Subiekt sgt)
        {
            var portalGamesConnString = new MySqlConnectionStringBuilder
            {
                Server = ConfigConnection.PortalGamesServer,
                UserID = ConfigConnection.PortalGamesUser,
                Password = ConfigConnection.PortalGamesPassword,
                Database = ConfigConnection.PortalGamesBaza
            };

            using (MySqlConnection conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    string sqlCommand =
                        "SELECT c.user_id, c.address_street, c.address_city, c.address_zip, s.name as country, c.daddress_name, c.daddress_lastname, c.phone, c.company_name, c.loyality_points, c.discount, u.email " +
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
        /// <summary>
        /// Funkcja wystawia dokument przyjęcia płatności w Subiekcie - KP
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="idKasy">Id kasy z której została wykonana operacja wystawienia dokumentu</param>
        /// <param name="idKontrahenta">Id kontrahenta z bazy danych</param>
        /// <param name="tytul">Tytul jaki bedzie widniał na dokumencie</param>
        /// <param name="cena">Cena jaka została wystawiona na dokumencie</param>
        /// <param name="kurs">Nazwa kursu waluty jaki ma przyjąć dokumen (np. PLN, USD, EUR)</param>
        public static void WstawDokumentPrzyjeciaPlatnosci(InsERT.Subiekt sgt, long idKasy, int idKontrahenta, string tytul, decimal cena, string kurs)
        {
            var finDokument = sgt.FinManager.DodajDokumentKasowy(DokFinTypEnum.gtaDokFinTypKP, (int)idKasy);
            finDokument.Data = DateTime.Now;
            finDokument.ObiektPowiazanyWstaw(DokFinObiektTypEnum.gtaDokFinObiektKontrahent, idKontrahenta);
            finDokument.WartoscPoczatkowaWaluta = cena;
            finDokument.Waluta = kurs;
            finDokument.Tytulem = tytul;
            finDokument.Zapisz();
        }
        /// <summary>
        /// Funkcja wystawia dokument wystawienia płatności w Subiekcie - KW
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="idKasy">Id kasy z której została wykonana operacja wystawienia dokumentu</param>
        /// <param name="idKontrahenta">Id kontrahenta z bazy danych</param>
        /// <param name="tytul">Tytul jaki bedzie widniał na dokumencie</param>
        /// <param name="cena">Cena jaka została wystawiona na dokumencie</param>
        /// <param name="kurs">Nazwa kursu waluty jaki ma przyjąć dokumen (np. PLN, USD, EUR)</param>
        public static void WstawDokumentWystawieniaPlatnosci(InsERT.Subiekt sgt, long idKasy, int idKontrahenta, string tytul, decimal cena, string kurs)
        {
            var finDokument = sgt.FinManager.DodajDokumentKasowy(DokFinTypEnum.gtaDokFinTypKW, (int)idKasy);
            finDokument.Data = DateTime.Now;
            finDokument.ObiektPowiazanyWstaw(DokFinObiektTypEnum.gtaDokFinObiektKontrahent, idKontrahenta);
            finDokument.WartoscPoczatkowaWaluta = cena;
            finDokument.Waluta = kurs;
            finDokument.Tytulem = tytul;
            finDokument.Zapisz();
        }
    }
}