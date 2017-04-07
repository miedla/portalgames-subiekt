using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Collections.Generic;
using InsERT;
using System.Runtime.InteropServices;

namespace subiekt_sfera_test
{
    public static class Utils
    {
        public static Dictionary<int, string> panstwa = new Dictionary<int, string>
        {
            {1, "PL"},
            {2, "CZ"}
        };

        public static MySqlConnectionStringBuilder portalGamesConnString = new MySqlConnectionStringBuilder
        {
            Server = ConfigConnection.PortalGamesServer,
            UserID = ConfigConnection.PortalGamesUser,
            Password = ConfigConnection.PortalGamesPassword,
            Database = ConfigConnection.PortalGamesBaza
        };

        public static string NumerZamowienia = "brak";

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

        //public static void GetUserFromPortalGames(int order_id, InsERT.Subiekt sgt)
        //{
        //    Kontrahent k = PobierzKontrahenta(order_id);

        //    string nazwa = k.Nazwa;
        //    string symbol = k.Id;
        //    string miejscowosc = k.Miasto;
        //    string ulica = k.Ulica;
        //    string kod_pocztowy = k.MiastoKod;
        //    string imie = k.Imie;
        //    string nazwisko = k.Nazwisko;
        //    int panstwo_id = 1;
        //    if (!string.IsNullOrEmpty(k.PanstwoKod))
        //    {
        //        panstwo_id =
        //            panstwa.Where(p => p.Value.Equals(k.PanstwoKod)).Select(p => p.Key).FirstOrDefault();
        //    }

        //    DodajKontrahenta(sgt, nazwa, symbol, miejscowosc, ulica, panstwo_id, kod_pocztowy, imie, nazwisko);
        //}

        public static void GetUsersFromPortalGames(InsERT.Subiekt sgt)
        {
            //var portalGamesConnString = new MySqlConnectionStringBuilder
            //{
            //    Server = ConfigConnection.PortalGamesServer,
            //    UserID = ConfigConnection.PortalGamesUser,
            //    Password = ConfigConnection.PortalGamesPassword,
            //    Database = ConfigConnection.PortalGamesBaza
            //};

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
                        string panstwo_kod = reader["address_state_id"].ToString();// == string.Empty
                            //? "brak"
                            //: reader["address_state_id"].ToString();
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
                //Console.WriteLine(e);
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
        public static void DodajZamowienie(InsERT.Subiekt sgt, int idKontrahenta, List<Produkt> symbolsProducts,
            string idZakupu)
        {
            var zamowienie = new Zamowienie();
            var zkDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentZK);
            
            zkDokument.KontrahentId = idKontrahenta;
            var i = 1;
            foreach (var idProduct in symbolsProducts)
            {
                if (sgt.Towary.Istnieje(idProduct.Symbol))
                {
                    zkDokument.Pozycje.Dodaj(idProduct.Symbol);
                    zkDokument.Pozycje.Element(i).IloscJm = idProduct.Ilosc;
                }
                i++;
            }


            zkDokument.Zapisz();
            zamowienie.NumerZamowienia = zkDokument.NumerPelny;
            NumerZamowienia = zkDokument.NumerPelny;
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
        public static void WystawFaktureZaliczkowa(InsERT.Subiekt sgt, string nazwaDokumentu, string typPrzedpalty,
            decimal kwota)
        {
            //var fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzal);
            SuDokument fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzal);
            SuDokument fzaliczkowa = sgt.SuDokumentyManager.DodajFSzal();
            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);

                fzaliczkowa.NaPodstawie(oDok.Identyfikator);

                fsDokument.NaPodstawie(oDok.Identyfikator);
                ///
                //fsDokument.AutoPrzeliczanie = false;
                //object dokumentStatusEnum = oDok.StatusDokumentu;
                //fsDokument.StatusDokumentu = SubiektDokumentStatusEnum.gtaSubiektDokumentStatusOdlozony;

                Console.WriteLine("typ przedlapty: "+ typPrzedpalty);
                Console.WriteLine("kwota: " + kwota);

                switch (typPrzedpalty)
                {
                    case "gotowka":
                        fsDokument.PlatnoscGotowkaKwota = kwota;
                        break;

                    case "przelew":
                        fsDokument.PlatnoscPrzelewKwota = (Decimal)kwota;//kwota;
                        fsDokument.PlatnoscGotowkaKwota = 0;//15;//0;
                        break;
                    case "karta":
                        fsDokument.PlatnoscGotowkaKwota = 0;
                        fsDokument.PlatnoscKredytKwota = kwota;
                        break;
                }

                //fsDokument.StatusDokumentu = dokumentStatusEnum;

                fsDokument.Przelicz();
                fsDokument.Zapisz();

                Marshal.ReleaseComObject(fsDokument);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //sgt.Zakoncz();
                //Environment.Exit(1);
                //Console.WriteLine("typPrzedpalty: " + typPrzedpalty);
                //Console.WriteLine("kwota: " + kwota);
                //throw;
            }
        }

        /// <summary>
        /// Metoda tworzy fakture zakliczkową końcową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="nazwaDokumentu">Nazwa (Numer) dokumunetu zamowienia</param>
        public static void WystawFaktureZaliczkowaKoncowa(InsERT.Subiekt sgt, string nazwaDokumentu)
        {
            SuDokument fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzalkonc);
            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);

                fsDokument.NaPodstawie(oDok.Identyfikator);

                fsDokument.Przelicz();
                fsDokument.Zapisz();

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
                //sgt.Zakoncz();
                //Environment.Exit(1);
                //throw;
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
        public static Kontrahent PobierzKontrahenta(int order_id)
        {
            var kontrahent = new Kontrahent();
            //            kontrahent.Zamowienie = new Zamowienie();
            //var portalGamesConnString = new MySqlConnectionStringBuilder
            //{
            //    Server = ConfigConnection.PortalGamesServer,
            //    UserID = ConfigConnection.PortalGamesUser,
            //    Password = ConfigConnection.PortalGamesPassword,
            //    Database = ConfigConnection.PortalGamesBaza
            //};

            //var sqlCommand = "select * from baza8706_devportalgames.`order` order by 1 desc LIMIT 1;";
            var sqlCommand = "select * from baza8706_devportalgames.`order`WHERE id = @order_id;";
            
            using (var conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.Parameters.AddWithValue("@order_id", order_id);
                    //Console.WriteLine("connection string: " + conn.ConnectionString);
                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        //Console.WriteLine("Kontrahent o podanym id nie istnieje!");
                        return null;
                    }

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
                        kontrahent.PanstwoKod = reader["address_state_id"].ToString();//== string.Empty
                            //? "brak"
                            //: reader["address_state_id"].ToString();
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
                        kontrahent.Zamowienie.WplaconaKwotaSum = reader["paid_price"].ToString() == string.Empty
                            ? "brak"
                            : reader["paid_price"].ToString();
                        //kontrahent.Zamowienie.KwotaDoZaplaty = reader["sum"].ToString() == string.Empty
                        //    ? "brak"
                        //    : reader["sum"].ToString();
                        kontrahent.Zamowienie.WartoscProduktu = reader["sum"].ToString() == string.Empty
                            ? "0"
                            : reader["sum"].ToString();
                        //kontrahent.Zamowienie.IloscWplat = reader["payment_id"].ToString() == string.Empty
                        //    ? "brak"
                        //    : reader["payment_id"].ToString();//jakie ilosc wplat toz to jest id rodzaju platnosci!
                        kontrahent.Zamowienie.WplataRodzaj = reader["payment_id"].ToString() == string.Empty
                            ? "0"
                            : reader["payment_id"].ToString();
                        kontrahent.Zamowienie.DostawaKwota = reader["postage_price"].ToString() == string.Empty
                            ? "0"
                            : reader["postage_price"].ToString();
                        kontrahent.Zamowienie.DostawaRodzaj = reader["postage_id"].ToString() == string.Empty
                            ? "0"
                            : reader["postage_id"].ToString();

                        kontrahent.Zamowienie.KwotaDoZaplaty = kontrahent.Zamowienie.WartoscProduktu + kontrahent.Zamowienie.DostawaKwota;
                    }
                }
            }
            if (Convert.ToInt32(kontrahent.Zamowienie.IloscWplat) > 1)
            {
                sqlCommand = "select * from baza8706_devportalgames.order_pay_history where" +
                             " order_id = " + kontrahent.IdZakupu + " order by 1 desc limit 1;";
                using (var conn = new MySqlConnection(portalGamesConnString.ToString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlCommand;

                        conn.Open();
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            kontrahent.Zamowienie.WplaconaKwota = reader["price_brutto"].ToString();
                        }
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
            //var portalGamesConnString = new MySqlConnectionStringBuilder
            //{
            //    Server = ConfigConnection.PortalGamesServer,
            //    UserID = ConfigConnection.PortalGamesUser,
            //    Password = ConfigConnection.PortalGamesPassword,
            //    Database = ConfigConnection.PortalGamesBaza
            //};

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
        /// sprawdza czy istnieje już w subiekcie zamówienie (jeśli nie to je dodaje). Następnie sprawdza 
        /// czy została wpłacona jakaś kwota za produkt, jeśli tak to dodaje wystawia fakture zaliczkową
        /// a jeśli kwota jest pełna - to znaczy jeśli produkt został w pełni opłacony to wystawia fakturę zaliczkowo końcową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        public static void ZakupProces(InsERT.Subiekt sgt, int order_id)
        {
            //Console.WriteLine("Zakup Proces order_id: "+order_id);
            var kontrahent = PobierzKontrahenta(order_id);

            ///Nalezy sprawdzac czy faktura koncowa zostala juz wystawiona, jesli tak to zakonczyc na tym.
            if (kontrahent != null)
            {
                //Console.WriteLine("kontrahent != null");
                int panstwo_id = 1;
                if (!string.IsNullOrEmpty(kontrahent.PanstwoKod))
                {
                    panstwo_id =
                        panstwa.Where(p => p.Value.Equals(kontrahent.PanstwoKod)).Select(p => p.Key).FirstOrDefault();
                }

                DodajKontrahenta(sgt, kontrahent.Nazwa, kontrahent.Id, kontrahent.Miasto, kontrahent.Ulica, panstwo_id,
                    kontrahent.MiastoKod, kontrahent.Imie, kontrahent.Nazwisko);
                var kontrahentSubiekt = sgt.Kontrahenci.Wczytaj(kontrahent.Id);
                int idkontrahentaSubiekt = kontrahentSubiekt.Identyfikator();

                //Console.WriteLine("kontrahent.Zamowienie.WplaconaKwota: "+ kontrahent.Zamowienie.WplaconaKwota);
                if (Convert.ToDouble(kontrahent.Zamowienie.WplaconaKwota) > 0)
                {
                    if (!SprawdzCzyZamowienieJuzIstnieje(sgt, kontrahent.NumerZamowienia))
                    {
                        //Console.WriteLine("Zamowienie istnieje!");
                        var listaProduktow = PobierzListeProduktowZZamowienia(kontrahent.IdZakupu);
                        //Console.WriteLine("lista produktow.Count: "+listaProduktow.Count());
                        DodajZamowienie(sgt, idkontrahentaSubiekt, listaProduktow, kontrahent.IdZakupu);
                        kontrahent.NumerZamowienia = NumerZamowienia;
                    }
                    //Console.WriteLine("zamowienie juz istnieje");
                    var kwota = Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwota);
                    WystawFaktureZaliczkowa(sgt, kontrahent.NumerZamowienia, "przelew", kwota);
                }
                ///Jesli wplacona kwota jest ROWNA kwocie do zaplaty to wystawiac fakture koncowa, 
                ///jesli mniejsza to idzie faktura zaliczeniowa, 
                ///jesli większa to nie wrzucac ani faktury zaliczeniowej ani koncowej tylko oznaczyc zamowienie, ze jest blad
                ///W platnosci gotowka i karta bedzie tylko 1 faktura zaliczeniowa
                ///W platnosci przelewem moze wystapic wiele faktur zaliczeniowych
                if (Convert.ToDouble(kontrahent.Zamowienie.WplaconaKwotaSum) >=
                    Convert.ToDouble(kontrahent.Zamowienie.KwotaDoZaplaty))
                {
                    WystawFaktureZaliczkowaKoncowa(sgt, kontrahent.NumerZamowienia);
                }
            }
            else
            {
                //Console.WriteLine("Kontrahent == null");
            }
        }

        /// <summary>
        /// Metoda zwraca liste zakupionych produktów w postaci listy ich id 
        /// </summary>
        /// <param name="idZamowienia"> Id zamowienia </param>
        /// <returns>Zwraca liste intów </returns>
        public static List<Produkt> PobierzListeProduktowZZamowienia(string idZamowienia)
        {
            var listaProduktow = new List<Produkt>();

            //var portalGamesConnString = new MySqlConnectionStringBuilder
            //{
            //    Server = ConfigConnection.PortalGamesServer,
            //    UserID = ConfigConnection.PortalGamesUser,
            //    Password = ConfigConnection.PortalGamesPassword,
            //    Database = ConfigConnection.PortalGamesBaza
            //};

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
                        var produkt = new Produkt
                        {
                            Ilosc = Convert.ToInt32(reader["amount"]),
                            Symbol = (reader["product_code"].ToString())
                        };
                        listaProduktow.Add(produkt);
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
        /// <summary>
        /// Metoda dodaje do bazy sklepu intenetowego produkt. 
        /// </summary>
        /// <param name="modify_user_id">id usera ktory dodał ten produkt? W bazie wartosci są w zakresie od 0 do 3</param>
        /// <param name="tax_rate">Nie wiem co to. W bazie czesto wystepuje jako liczba 23. Pewnie podatek</param>
        /// <param name="price">Cena produktu</param>
        /// <param name="price_old">Stara cena produktu</param>
        /// <param name="delivery_cost">Koszt dostawy</param>
        /// <param name="label_new">Wartosc 0 lub 1. Czy produkt jest nowoscią?</param>
        /// <param name="label_preorder">Wartość 0 lub 1. Czy produkt jest jako preorder</param>
        /// <param name="code">Unikalny kod produktu</param>
        /// <param name="custom_id">Nie wiem za bardzo co to. Czesto wystepuje w wielkiej liczbie (2900000) lub 0 </param>
        /// <param name="amount">Czesto wystepuje w licznie 35029. Pewnie ilosc</param>
        /// <param name="weight">Waga produktu </param>
        /// <param name="packaging">Forma pokowania - 'karton','paleta'</param>
        /// <param name="packaging_amount">Prawie zawsze wysteuje w formie 0 ale moze tez przyjac 1</param>
        /// <param name="nazwaProduktu">Nazwa produktu</param>
        /// <param name="opisProduktu">Opis danego produktu</param>
        public static void WstapProduktDoBazySklepu(string modify_user_id, string tax_rate, string price,
            string price_old, string delivery_cost, string label_new, string label_preorder, string code,
            string custom_id, string amount, string weight, string packaging, string packaging_amount, string nazwaProduktu, string opisProduktu)
        {
            var id = 0;
            var portalGamesConnString = new MySqlConnectionStringBuilder
            {
                Server = ConfigConnection.PortalGamesServer,
                UserID = ConfigConnection.PortalGamesUser,
                Password = ConfigConnection.PortalGamesPassword,
                Database = ConfigConnection.PortalGamesBaza
            };

            var sqlCommand = "INSERT INTO baza8706_devportalgames.product (" +
                             "modify_user_id," +
                             "modify_user_ip," +
                             "tax_rate, " +
                             "price, " +
                             "price_old, " +
                             "delivery_cost, " +
                             "label_new, " +
                             "label_preorder, " +
                             "code, " +
                             "custom_id, " +
                             "amount, " +
                             "weight," +
                             "packaging," +
                             "packaging_amount) values(" +
                             "" + "modify_user_id" + ", " +
                             "'192.168.1.1', " +
                             "" + tax_rate + ", " +
                             "" + price + ", " +
                             "" + price_old + ", " +
                             "" + delivery_cost + ", " +
                             "" + label_new + ", " +
                             "" + label_preorder + ", " +
                             "'" + code + "', " +
                             "" + custom_id + ", " +
                             "" + amount + ", " +
                             "" + weight + ", " +
                             "'" + packaging + "', " +
                             "" + packaging_amount + "); ";

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
            sqlCommand = "SELECT * FROM product order by 1 desc limit 1;";
            using (var conn = new MySqlConnection(portalGamesConnString.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader["id"]);
                    }
                }
            }

            sqlCommand = "insert into product_description (lang, name, name2, description, tags, product_id) values ('pl', '"+ nazwaProduktu + "', '', '" + opisProduktu + "', '', "+ id +");";
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