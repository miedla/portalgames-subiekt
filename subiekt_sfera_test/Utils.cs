using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Collections.Generic;
using InsERT;

namespace subiekt_sfera_test
{
    public static class Utils
    {
        public static Dictionary<int, string> panstwa = new Dictionary<int, string>
        {
            {1, "PL"},
            {2, "CZ"}
        };

        public static void DodajKontrahenta(InsERT.Subiekt sgt, string nazwa, string symbol, string miejscowosc,
            string ulica, int panstwo, string kodPocztowy, string imie = null, string nazwisko = null)
        {
            bool czyIstnieje = true;

            czyIstnieje = sgt.Kontrahenci.Istnieje(symbol);

            if (!czyIstnieje)
            {
                InsERT.Kontrahent okh = sgt.Kontrahenci.Dodaj();
                okh.Typ = InsERT.KontrahentTypEnum.gtaKontrahentTypDostOdb;
                okh.Osoba = true;

                Console.WriteLine("kodPocztowy kontrahenta: " + kodPocztowy);
                if (nazwa.Length > 50)
                {
                    okh.Nazwa = nazwa.Substring(0, 50);
                    okh.NazwaPelna = nazwa;
                }
                else
                {
                    okh.Nazwa = nazwa;
                }

                if (ulica.Length > 50)
                {
                    okh.Ulica = ulica.Substring(0, 50);
                }
                else
                {
                    okh.Ulica = ulica;
                }

                if (kodPocztowy.Length > 8)
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
                        string id = reader["customer_id"].ToString();
                        string ulica = reader["address_street"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_street"].ToString();
                        string miasto = reader["address_city"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_city"].ToString();
                        string zip = reader["address_zip"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_zip"].ToString();
                        string panstwo_kod = reader["address_state_id"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_state_id"].ToString();
                        string nazwa = reader["customer_name"].ToString() == string.Empty
                            ? "brak"
                            : reader["customer_name"].ToString();
                        string imie = reader["maddress_name"].ToString() == string.Empty
                            ? "brak"
                            : reader["maddress_name"].ToString();
                        string nazwisko = reader["maddress_lastname"].ToString() == string.Empty
                            ? "brak"
                            : reader["maddress_lastname"].ToString();
                        string fima = reader["company_name"].ToString() == string.Empty
                            ? "brak"
                            : reader["company_name"].ToString();
                        string miasto_kod = reader["address_zip"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_zip"].ToString();

                        int panstwo_id = 1;
                        if (!string.IsNullOrEmpty(panstwo_kod))
                        {
                            panstwo_id =
                                panstwa.Where(p => p.Value.Equals(panstwo_kod)).Select(p => p.Key).FirstOrDefault();
                        }

                        DodajKontrahenta(sgt, nazwa, id, miasto, ulica, panstwo_id, miasto_kod, imie, nazwisko);
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
        public static void WstawDokumentPrzyjeciaPlatnosci(InsERT.Subiekt sgt, long idKasy, int idKontrahenta,
            string tytul, decimal cena, string kurs)
        {
            var finDokument = sgt.FinManager.DodajDokumentKasowy(DokFinTypEnum.gtaDokFinTypKP, (int) idKasy);
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
        public static void WstawDokumentWystawieniaPlatnosci(InsERT.Subiekt sgt, long idKasy, int idKontrahenta,
            string tytul, decimal cena, string kurs)
        {
            var finDokument = sgt.FinManager.DodajDokumentKasowy(DokFinTypEnum.gtaDokFinTypKW, (int) idKasy);
            finDokument.Data = DateTime.Now;
            finDokument.ObiektPowiazanyWstaw(DokFinObiektTypEnum.gtaDokFinObiektKontrahent, idKontrahenta);
            finDokument.WartoscPoczatkowaWaluta = cena;
            finDokument.Waluta = kurs;
            finDokument.Tytulem = tytul;
            finDokument.Zapisz();
        }

        /// <summary>
        /// Metoda wystawia paragon detaliczny.
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="idProducts">Lista id produktów wystawiona do sprzedaży</param>
        public static void DodajParagon(InsERT.Subiekt sgt, List<int> idProducts)
        {
            var paDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentPA);
            foreach (var idProduct in idProducts)
            {
                paDokument.Pozycje.Dodaj(idProduct);
            }
            paDokument.Zapisz();
        }

        /// <summary>
        /// Metoda wystawia paragon imenny 
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="idKontrahenta">Id kontrahenta</param>
        /// <param name="idProducts">Lista id produktów wystawiona do sprzedaży</param>
        public static void DodajParagonImienny(InsERT.Subiekt sgt, int idKontrahenta, List<int> idProducts)
        {
            var paDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentPAi);
            paDokument.KontrahentId = idKontrahenta;
            foreach (var idProduct in idProducts)
            {
                paDokument.Pozycje.Dodaj(idProduct);
            }
            paDokument.Zapisz();
        }
        /// <summary>
        /// Metoda wystawia fakture detaliczną do paragonu imennego.
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="nazwaDokumentu">Nazwa (Numer) paragonu imiennego</param>
        public static void WystawFaktureDetaliczna(InsERT.Subiekt sgt, string nazwaDokumentu)
        {
            var fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSd);
            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);

                fsDokument.NaPodstawie(oDok.Identyfikator);
                fsDokument.Przelicz();
                fsDokument.Zapisz();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
//                throw;
            }
        }
        /// <summary>
        /// Metoda wystawia fakture sprzedaży
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="idKontrahenta">Id kontrahenta</param>
        /// <param name="idProducts">Lista id produktów wystawiona do sprzedaży</param>
        public static void DodajFaktureSprzedazy(InsERT.Subiekt sgt, int idKontrahenta, List<int> idProducts)
        {
            var fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFS);

            fsDokument.KontrahentId = idKontrahenta;
            foreach (var idProduct in idProducts)
            {
                fsDokument.Pozycje.Dodaj(idProduct);
            }
            
            fsDokument.Zapisz();
        }
        /// <summary>
        /// Metoda dodaje zamówienie 
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="idKontrahenta">Id kontrahenta</param>
        /// <param name="idProducts">Lista id produktów wystawiona do sprzedaży</param>
        public static void DodajZamowienie(InsERT.Subiekt sgt, int idKontrahenta, List<int> idProducts)
        {
            var zkDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentZK);

            zkDokument.KontrahentId = idKontrahenta;
            foreach (var idProduct in idProducts)
            {
                zkDokument.Pozycje.Dodaj(idProduct);
            }

            zkDokument.Zapisz();
        }
        /// <summary>
        /// Metoda tworzy fakture zaliczkową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="nazwaDokumentu">Nazwa (Numer) dokumunetu zamowienia</param>
        /// <param name="typPrzedpalty">Jaki ma być zastosowany typ przedpłaty.
        /// Do wyboru są 3 typy: gotowka, przelew, karta</param>
        public static void WystawFaktureZaliczkowa(InsERT.Subiekt sgt, string nazwaDokumentu, string typPrzedpalty)
        {
            var fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzal);
            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);

                fsDokument.NaPodstawie(oDok.Identyfikator);
                switch (typPrzedpalty)
                {
                    case "gotowka":
                        break;

                    case "przelew":
                        fsDokument.PlatnoscGotowkaKwota = 0;
                        fsDokument.PlatnoscPrzelewKwota = oDok.KwotaDoZaplaty();
                        break;
                    case "karta":
                        fsDokument.PlatnoscGotowkaKwota = 0;
                        fsDokument.PlatnoscKredytKwota = oDok.KwotaDoZaplaty();
                        break;
                }

                fsDokument.Przelicz();
                fsDokument.Zapisz();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                                throw;
            }
        }
        /// <summary>
        /// Metoda tworzy fakture zakliczkową końcową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="nazwaDokumentu">Nazwa (Numer) dokumunetu zamowienia</param>
        public static void WystawFaktureZaliczkowaKoncowa(InsERT.Subiekt sgt, string nazwaDokumentu)
        {
            var fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzalkonc);
            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);

                fsDokument.NaPodstawie(oDok.Identyfikator);

                fsDokument.Przelicz();
                fsDokument.Zapisz();
            }
            catch (Exception e)
            { 
                Console.WriteLine(e);
                throw;
            }
        }
    }
}