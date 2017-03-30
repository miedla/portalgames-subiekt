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

                //Console.WriteLine("imie kontrahenta: " + imie);
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

                if (imie.Length > 20)
                {
                    okh.OsobaImie = imie.Substring(0, 20);
                }
                else
                {
                    okh.NazwaPelna = imie;
                }

                okh.Symbol = symbol;
                okh.Miejscowosc = miejscowosc;
                //okh.OsobaImie = imie;
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
                        "SELECT * from baza8706_11.`order` o WHERE o.paid = 1 GROUP BY o.customer_id HAVING COUNT(*) = 1";

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
        /// <param name="symbolsProducts">Lista symboli produktów wystawiona do sprzedaży</param>
        /// <param name="idZakupu">id zakupu z bazy sklepu internetowego</param>
        /// <returns>Zwraca obiekt Zamowienie</returns>
        public static void DodajZamowienie(InsERT.Subiekt sgt, int idKontrahenta, List<string> symbolsProducts, string idZakupu)
        {
            var zamowienie = new Zamowienie();
            var zkDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentZK);
            zkDokument.KontrahentId = idKontrahenta;
            
            foreach (var idProduct in symbolsProducts)
            {
                if (sgt.Towary.Istnieje(idProduct))
                {
                    zkDokument.Pozycje.Dodaj(idProduct);
                }
            }


            zkDokument.Zapisz();
            zamowienie.NumerZamowienia = zkDokument.NumerPelny;
            zamowienie.IdZamowienia = idZakupu;
            WstawNumerZamowieniaDoBazy(zamowienie);
        }

        /// <summary>
        /// Sprawdza czy zamówienie już istnieje
        /// </summary>
        /// <param name="sgt"></param>
        /// <param name="numerZamowienia"> Numer zamówienia w subiekcie</param>
        /// <returns>Zwraca prawde lu fałsz</returns>
        public static bool SprawdzCzyZamowienieJuzIstnieje(InsERT.Subiekt sgt, string numerZamowienia)
        {
            var oDok = sgt.SuDokumentyManager.Istnieje(numerZamowienia);
            return oDok;
        }

        /// <summary>
        /// Metoda tworzy fakture zaliczkową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="nazwaDokumentu">Nazwa (Numer) dokumunetu zamowienia</param>
        /// <param name="typPrzedpalty">Jaki ma być zastosowany typ przedpłaty.
        /// Do wyboru są 3 typy: gotowka, przelew, karta</param>
        /// <param name="kwota">kwota jaka została zapłacona w zaliczce</param>
        public static void WystawFaktureZaliczkowa(InsERT.Subiekt sgt, string nazwaDokumentu, string typPrzedpalty, double kwota)
        {
            var fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzal);
            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);

                fsDokument.NaPodstawie(oDok.Identyfikator);
                switch (typPrzedpalty)
                {
                    case "gotowka":
                        fsDokument.PlatnoscGotowkaKwota = kwota;
                        break;

                    case "przelew":
                        fsDokument.PlatnoscGotowkaKwota = 0;
                        fsDokument.PlatnoscPrzelewKwota = kwota;
                        break;
                    case "karta":
                        fsDokument.PlatnoscGotowkaKwota = 0;
                        fsDokument.PlatnoscKredytKwota = kwota;
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

        /// <summary>
        /// Metoda wystawia dokumeny wydania towaru zewnętrznego
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="idKontrahenta">Id kontrahenta z bazy danych</param>
        /// <param name="idProducts">Lista id produktów</param>
        public static void WydanieZewnetrzne(InsERT.Subiekt sgt, int idKontrahenta, List<int> idProducts)
        {
            var wzDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentWZv);
            wzDokument.KontrahentId = idKontrahenta;
            foreach (var idProduct in idProducts)
            {
                wzDokument.Pozycje.Dodaj(idProduct);
            }
            wzDokument.Zapisz();
        }

        /// <summary>
        /// Metoda zwraca aktualnego kontrahenta który dokonał operacji na tabeli oreder
        /// </summary>
        /// <returns>Zwraca obiekt Kontrahent</returns>
        public static Kontrahent PobierzKontrahenta()
        {
            var kontrahent = new Kontrahent();
//            kontrahent.Zamowienie = new Zamowienie();
            var portalGamesConnString = new MySqlConnectionStringBuilder
            {
                Server = ConfigConnection.PortalGamesServer,
                UserID = ConfigConnection.PortalGamesUser,
                Password = ConfigConnection.PortalGamesPassword,
                Database = ConfigConnection.PortalGamesBaza
            };

            var sqlCommand = "select * from baza8706_devportalgames.`order` order by 1 desc LIMIT 1;";
            using (var conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kontrahent.IdZakupu = reader["id"].ToString();
                        kontrahent.Id = reader["customer_id"].ToString();
                        kontrahent.Ulica = reader["address_street"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_street"].ToString();
                        kontrahent.Miasto = reader["address_city"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_city"].ToString();
                        kontrahent.Zip = reader["address_zip"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_zip"].ToString();
                        kontrahent.PanstwoKod = reader["address_state_id"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_state_id"].ToString();
                        kontrahent.Nazwa = reader["customer_name"].ToString() == string.Empty
                            ? "brak"
                            : reader["customer_name"].ToString();
                        kontrahent.Imie = reader["maddress_name"].ToString() == string.Empty
                            ? "brak"
                            : reader["maddress_name"].ToString();
                        kontrahent.Nazwisko = reader["maddress_lastname"].ToString() == string.Empty
                            ? "brak"
                            : reader["maddress_lastname"].ToString();
                        kontrahent.Fima = reader["company_name"].ToString() == string.Empty
                            ? "brak"
                            : reader["company_name"].ToString();
                        kontrahent.MiastoKod = reader["address_zip"].ToString() == string.Empty
                            ? "brak"
                            : reader["address_zip"].ToString();
                        kontrahent.NumerZamowienia = reader["order_name"].ToString() == string.Empty
                            ? "brak"
                            : reader["order_name"].ToString();
                        kontrahent.Zamowienie.IdZamowienia = reader["id"].ToString() == string.Empty
                            ? "brak"
                            : reader["id"].ToString();
                        kontrahent.Zamowienie.WplaconaKwota = reader["paid_price"].ToString() == string.Empty
                            ? "brak"
                            : reader["paid_price"].ToString();
                        kontrahent.Zamowienie.KwotaDoZaplaty = reader["sum"].ToString() == string.Empty
                            ? "brak"
                            : reader["sum"].ToString();
                    }
                }
            }
            return kontrahent;
        }

        /// <summary>
        /// Metoda odpytuje baze sklepu i zwraca z ostatniego rekordu tabeli zamowien jego id
        /// </summary>
        /// <returns>Zwraca id zamowienia</returns>
        public static void WczytajZamowienie(string id)
        {
            var zamowienie = new Zamowienie();
            var portalGamesConnString = new MySqlConnectionStringBuilder
            {
                Server = ConfigConnection.PortalGamesServer,
                UserID = ConfigConnection.PortalGamesUser,
                Password = ConfigConnection.PortalGamesPassword,
                Database = ConfigConnection.PortalGamesBaza
            };

            var sqlCommand = "select * from baza8706_devportalgames.`order`where id = " + id + ";";
            using (var conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        zamowienie.IdZamowienia = reader["id"].ToString() == string.Empty
                            ? "brak"
                            : reader["id"].ToString();
                        zamowienie.WplaconaKwota = reader["paid_price"].ToString() == string.Empty
                            ? "brak"
                            : reader["paid_price"].ToString();
                        zamowienie.KwotaDoZaplaty = reader["sum"].ToString() == string.Empty
                            ? "brak"
                            : reader["sum"].ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Metoda integrująca sklep internetowy z Subiektem. Najpierw pobiera ostatnie zamówienie z bazy sklepu internetowego 
        /// nastepnie dodaje kontrahenta do bazy subiekta jeśli jeszcze go nie ma. W kolejnym etapie 
        /// sprawdza czy istnieje już w subiekcie zamówienie (jeśli nie to do dodaje). Następnie sprawda 
        /// czy została wpłacona jakaś kwota za produkt, jeśli tak to dodaje wystawia fakture zaliczkową
        /// a jeśli kwota jest pełna - to znaczy jeśli produkt został w pełni opłacony to wystawia fakturę zaliczkowo końcową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        public static void ZakupProces(InsERT.Subiekt sgt)
        {
            var kontrahent = PobierzKontrahenta();
            
            DodajKontrahenta(sgt, kontrahent.Nazwa, kontrahent.Id, kontrahent.Miasto, kontrahent.Ulica, 1,
                kontrahent.MiastoKod, kontrahent.Imie, kontrahent.Nazwisko);
            var kontrahentSubiekt = sgt.Kontrahenci.Wczytaj(kontrahent.Id);
            int idkontrahentaSubiekt = kontrahentSubiekt.Identyfikator();
            if (!SprawdzCzyZamowienieJuzIstnieje(sgt, kontrahent.NumerZamowienia))
            {
                var listaProduktow = PobierzListeProduktowZZamowienia(kontrahent.IdZakupu);
                DodajZamowienie(sgt, idkontrahentaSubiekt, listaProduktow, kontrahent.IdZakupu);
            }
            if (Convert.ToDouble(kontrahent.Zamowienie.WplaconaKwota) > 0)
            {
                WystawFaktureZaliczkowa(sgt, kontrahent.NumerZamowienia, "przelew",Convert.ToDouble(kontrahent.Zamowienie.WplaconaKwota));
            }
            if (Convert.ToDouble(kontrahent.Zamowienie.WplaconaKwota) >=
                Convert.ToDouble(kontrahent.Zamowienie.KwotaDoZaplaty))
            {
                WystawFaktureZaliczkowaKoncowa(sgt, kontrahent.NumerZamowienia);
            }
        }

        /// <summary>
        /// Metoda zwraca liste zakupionych produktów w postaci listy ich id 
        /// </summary>
        /// <param name="idZamowienia"> Id zamowienia </param>
        /// <returns>Zwraca liste intów </returns>
        public static List<string> PobierzListeProduktowZZamowienia(string idZamowienia)
        {
            var listaProduktow = new List<string>();
            var portalGamesConnString = new MySqlConnectionStringBuilder
            {
                Server = ConfigConnection.PortalGamesServer,
                UserID = ConfigConnection.PortalGamesUser,
                Password = ConfigConnection.PortalGamesPassword,
                Database = ConfigConnection.PortalGamesBaza
            };

            var sqlCommand = "select * from order_item where order_id = " + idZamowienia + ";";
            using (var conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        listaProduktow.Add(reader["product_code"].ToString());
                    }
                }
            }
            return listaProduktow;
        }

        /// <summary>
        /// Metoda wstawia numer zamowienia z bazy subiekta do bazy sklepu
        /// </summary>
        /// <param name="zamowienie">Obiekt Zamowienie</param>
        public static void WstawNumerZamowieniaDoBazy(Zamowienie zamowienie)
        {
            var portalGamesConnString = new MySqlConnectionStringBuilder
            {
                Server = ConfigConnection.PortalGamesServer,
                UserID = ConfigConnection.PortalGamesUser,
                Password = ConfigConnection.PortalGamesPassword,
                Database = ConfigConnection.PortalGamesBaza
            };

            var sqlCommand = "UPDATE baza8706_devportalgames.`order` SET order_name = '" + zamowienie.NumerZamowienia +
                             "' WHERE id = " + zamowienie.IdZamowienia + ";";
            using (var conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                    }
                }
            }
        }
    }
}