using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Collections.Generic;
using InsERT;
using System.Runtime.InteropServices;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Security;

namespace subiekt_sfera_test
{
    /// <summary>
    /// KLasa zawiera wszystkie funkcje, z którego korzysta web serwis
    /// </summary>
    public static class Utils
    {
        public enum RodzajSklepu
        {
            Sklep,
            Shop,
            Geshaft,
            Hurt
        };
        public static MySqlConnectionStringBuilder portalGamesConnString = new MySqlConnectionStringBuilder
        {
            Server = ConfigConnection.PortalGamesServerProperty,
            UserID = ConfigConnection.PortalGamesUserProperty,
            Password = ConfigConnection.PortalGamesPasswordProperty,
            Database = ConfigConnection.PortalGamesBazaProperty
        };

        public static MySqlConnectionStringBuilder portalGamesShopConnString = new MySqlConnectionStringBuilder
        {
            Server = ConfigConnection.PortalGamesShopServerProperty,
            UserID = ConfigConnection.PortalGamesShopUserProperty,
            Password = ConfigConnection.PortalGamesShopPasswordProperty,
            Database = ConfigConnection.PortalGamesShopBazaProperty
        };

        public static SqlConnectionStringBuilder subiektConn = new SqlConnectionStringBuilder();

        public static MySqlConnectionStringBuilder PortalGamesConnSring
        {
            get { return portalGamesConnString; }
            set { portalGamesConnString = value; }
        }

        public static MySqlConnectionStringBuilder PortalGamesShopConnSring
        {
            get { return portalGamesShopConnString; }
            set { portalGamesShopConnString = value; }
        }

        public static string NumerZamowienia = "brak";

        /// <summary>
        /// Dodanie kontrahenta do Subiekta
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        /// <param name="nazwa">Nazwa kontrahenta</param>
        /// <param name="symbol">Symbol kontrahenta</param>
        /// <param name="miejscowosc">Miejscowość kontrahenta</param>
        /// <param name="ulica">Ulica kontrahenta</param>
        /// <param name="panstwo">Państwo kontrahenta</param>
        /// <param name="kodPocztowy">Kod pocztowy kontrahenta</param>
        /// <param name="imie">Imię kontrahenta</param>
        /// <param name="nazwisko">Nazwisko kontrahenta</param>
        public static void DodajKontrahenta(InsERT.Subiekt sgt, string nazwa, string symbol, string miejscowosc,
            string ulica, int panstwo, string kodPocztowy, string NIP, string email, string imie = null, string nazwisko = null,
            string companyName = null, string companyAdress = null, string companyZip = null, string companyCity = null)
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

                if (imie.Length >= 20)
                {
                    List<int> indexes = new List<int>();
                    for (int i = imie.IndexOf(" "); i > -1 && i <= imie.Length; i = imie.IndexOf(" ", i + 1))
                    {
                        if (i < 20)
                        {
                            indexes.Add(i);
                        }
                        else
                        {
                            break;
                        }
                    }

                    int maxIdx = indexes.Max();

                    int idx = imie.LastIndexOf(" ");

                    okh.OsobaImie = imie.Substring(0, maxIdx);
                }
                else
                {
                    okh.OsobaImie = imie;
                }

                okh.NazwaPelna = nazwa;
                //string nazwaPelna = imie + nazwisko;

                //if(nazwaPelna.Length <= 150)
                //{
                //    okh.NazwaPelna = nazwaPelna;
                //}
                //else
                //{
                //    okh.NazwaPelna = nazwaPelna.Substring(0, 150);
                //}

                okh.Symbol = symbol;
                okh.Miejscowosc = miejscowosc;
                //okh.OsobaImie = imie;
                okh.OsobaNazwisko = nazwisko;
                if (!string.IsNullOrEmpty(NIP))
                {
                    okh.NIP = NIP;
                }

                if (!string.IsNullOrEmpty(email))
                {
                    okh.Email = email;
                }

                if (panstwo != 0)
                {
                    okh.Panstwo = panstwo;
                }
                else
                {
                    okh.Panstwo = SlownikEnum.gtaBrak;
                }

                if (!string.IsNullOrEmpty(companyName))
                {
                    okh.Nazwa = companyName;
                    okh.Osoba = false;
                }
                if (!string.IsNullOrEmpty(companyAdress))
                {
                    okh.Ulica = companyAdress;
                }
                if (!string.IsNullOrEmpty(companyCity))
                {
                    okh.Miejscowosc = companyCity;
                }
                if (!string.IsNullOrEmpty(companyZip))
                {
                    okh.KodPocztowy = companyZip;
                }

                //Console.WriteLine("Dodano kontrahenta: " + okh.Nazwa);
                okh.Zapisz();
                okh.Zamknij();
            }
        }

        public static void AktualizacjaKontrahentow(Subiekt sgt)
        {
            bool czyIstnieje = true;
            var kontrahenci = PobierzKontrahentow();
            var lista = new List<string>();
            int zmodyfkowaniKontrahenci = 0;
            int zdublowaniKontrahenci = 0;
            foreach (Kontrahent k in kontrahenci)
            {
                
                czyIstnieje = sgt.Kontrahenci.Istnieje(k.Id);

                if (czyIstnieje)
                {
                    InsERT.Kontrahent kontrahent = sgt.Kontrahenci.Wczytaj(k.Id);
                    if (k.Imie.Equals(kontrahent.OsobaImie) 
                        && k.Nazwisko.Equals(kontrahent.OsobaNazwisko) 
                        && k.Miasto.Equals(kontrahent.Miejscowosc)
                        && k.Ulica.Equals(kontrahent.Ulica))
                    {
                        lista.Add(k.Imie + " " + k.Nazwisko + " " + k.Miasto + " " + k.Ulica);
                        try
                        {
//                            Console.WriteLine(k.Imie + " " + k.Nazwisko + " " + k.Miasto + " " + k.Ulica);
                            kontrahent.Symbol = k.DatabaseType + k.Id;
                            kontrahent.Zapisz();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
//                        if ()
//                        {
//                            
//                        }
                        zmodyfkowaniKontrahenci++;
//                        Console.WriteLine(k.Id);
                    }
                    else
                    {
//                        Console.WriteLine("kontrahent o id : " + k.Id + " ma inny email");
                    }
                }
                else
                {
//                    Console.WriteLine("nie istnieje kontrahent: " + k.Id);
                }
            }
            Console.WriteLine("zmodyfikowano: " + zdublowaniKontrahenci);
//            System.IO.File.WriteAllLines(@"C:\Users\ampmedia\Desktop\kontrahenci.txt", lista);
//            Console.WriteLine("lista count: " + kontrahenci.Count);
//            Console.WriteLine("ilosc zmodyfikowanych: " + zmodyfkowaniKontrahenci);
           
        }

        public static List<Kontrahent> PobierzKontrahentow()
        {
            var kontrahenci = new List<Kontrahent>();
            //var sqlCommand = "select * from baza8706_devportalgames.`order` order by 1 desc LIMIT 1;";
            var sqlCommand =
                "select c.id, c.customer_database_type, u.name, u.lastname, c.address_city, c.address_street  from customer c JOIN `user` u on (c.user_id = u.id) GROUP BY 1;";

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
//                    cmd.Parameters.AddWithValue("@order_id", order_id);
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
                        var kontrahent = new Kontrahent();
                        kontrahent.Id = reader["id"].ToString();
                        kontrahent.DatabaseType = reader["customer_database_type"].ToString();
//                        kontrahent.Email = reader["email"].ToString();
                        kontrahent.Imie = reader["name"].ToString();
                        kontrahent.Nazwisko = reader["lastname"].ToString();
                        kontrahent.Miasto = reader["address_city"].ToString();
                        kontrahent.Ulica = reader["address_street"].ToString();
                        kontrahenci.Add(kontrahent);
                    }
                }
            }

            return kontrahenci;
        }
        /// <summary>
        /// Pobranie wszystkich uzytkonikow z Portalgames i dodanie ich do Subiekta
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        public static void GetUsersFromPortalGames(InsERT.Subiekt sgt)
        {
            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (MySqlConnection conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    string sqlCommand =
                    //"SELECT * from "+ConfigConnection.PortalGamesBazaProperty+".`order` o WHERE o.paid = 1 GROUP BY o.customer_id HAVING COUNT(*) = 1";
                    "SELECT o.*, s.subiekt_id as panstwo_subiekt from " + ConfigConnection.PortalGamesBazaProperty + ".`order` o"
                    + " JOIN state s ON(o.address_state_id=s.id)"
                    + " WHERE o.paid = 1 GROUP BY o.customer_id HAVING COUNT(*) = 1";

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
                            panstwo_id = int.Parse(reader["panstwo_subiekt"].ToString());
                            //panstwo_id =
                            //    panstwa.Where(p => p.Value.Equals(panstwo_kod)).Select(p => p.Key).FirstOrDefault();
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
        public static void WstawDokumentWystawieniaPlatnosci(InsERT.Subiekt sgt, long idKasy, int idKontrahenta,
            string tytul, decimal cena, string kurs)
        {
            var finDokument = sgt.FinManager.DodajDokumentKasowy(DokFinTypEnum.gtaDokFinTypKW, (int)idKasy);
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
                sgt.Zakoncz();
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
        public static string DodajZamowienie(InsERT.Subiekt sgt, int idKontrahenta, List<Produkt> symbolsProducts,
            string idZakupu, Kontrahent k, RodzajSklepu rodzaj_sklepu)//bool czy_zagraniczna
        {
            var zamowienie = new Zamowienie();
            //var zkDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentZK);
            var zkDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentZK);
//            zkDokument.DataOtrzymania = k.Zamowienie.DataWplaty.Date;
            Console.WriteLine("DodajZamowienie");
            //sgt.Okno.Widoczne = true;
            //sgt.Okno.Aktywuj();

            switch (rodzaj_sklepu)
            {
                case RodzajSklepu.Sklep:
                    zkDokument.KategoriaId = 18;
                    break;
                case RodzajSklepu.Shop:
                    zkDokument.KategoriaId = 19;
                    break;
                case RodzajSklepu.Geshaft:
                    zkDokument.KategoriaId = 20;
                    break;
            }

            

            zkDokument.KontrahentId = idKontrahenta;
            var i = 1;
            bool czyWszystkieTowaryIstnieja = true;

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (MySqlConnection conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                conn.Open();
                Dictionary<string,int> duplicateProducts = new Dictionary<string, int>();
                foreach (var idProduct in symbolsProducts)
                {
                    if (sgt.Towary.Istnieje(idProduct.Symbol))
                    {
                        Console.WriteLine("produkt symbol istnieje: " + idProduct.Symbol);
                        zkDokument.Pozycje.Dodaj(idProduct.Symbol);
                        //Console.WriteLine("zkDokument.Pozycje."+ (SuDokument)zkDokument.Pozycje.E;
                        zkDokument.Pozycje.Element(i).IloscJm = idProduct.Ilosc;

                        SuPozycja poz = zkDokument.Pozycje.Element(i);
                        //Console.WriteLine(string.Format("towar: {0} poz.CenaNabycia: {1}", poz.TowarNazwa, poz.CenaBruttoPrzedRabatem));
                        if (!duplicateProducts.ContainsKey(idProduct.Symbol))
                        {
                            duplicateProducts.Add(idProduct.Symbol, 0);
                        }
                        else
                        {
                            duplicateProducts[idProduct.Symbol]++;
                        }

                        List<Double> discount = PobierzRabatDlaProduktu(k.Zamowienie.IdZamowienia, idProduct.Symbol, conn);
                        
                        poz.RabatProcent = discount[duplicateProducts[idProduct.Symbol]];

                        Towar t = sgt.Towary.Wczytaj(idProduct.Symbol);
                        TwCeny ceny = t.Ceny;

                        foreach (TwCena c in ceny)
                        {
                            if (c.Waluta == k.Zamowienie.Currency)
                            {
                                if (c.Brutto == (decimal)0)
                                {
                                    string errorMsg = string.Format("Towar o symbolu: {0} ma nieustawioną cenę w Subiekcie (Cena dla tego towaru wynosi: {1} w walucie zależnej od waluty zamówienia).{2}Identyfikator zamówienia: {3}", idProduct.Symbol, poz.CenaBruttoPrzedRabatem, Environment.NewLine, k.Zamowienie.IdZamowienia);
                                    SendErrorEmail(errorMsg);//SendErrorEmail();

                                    return null;
                                }
                            }
                        }
                        
                        i++;

                    }
                    else
                    {
                        czyWszystkieTowaryIstnieja = false;

                        string errorMsg = string.Format("Towar o symbolu: {0} nie istnieje w Subiekcie.{1}Identyfikator zamówienia: {2}", idProduct.Symbol, Environment.NewLine, k.Zamowienie.IdZamowienia);
                        SendErrorEmail(errorMsg);//SendErrorEmail();
                    }
                }
            }

            if (k.Zamowienie.DostawaRodzaj == null)
            {
                string errorMsg = string.Format("Zamówienie nie posiada rodzaju dostawy.{0}Identyfikator zamówienia: {1}", Environment.NewLine, k.Zamowienie.IdZamowienia);
                SendErrorEmail(errorMsg);//SendErrorEmail();

                return null;
            }
            else if(decimal.Parse(Convert.ToDouble(k.Zamowienie.DostawaKwota).ToString()) > 0)
            {
                Towar dostawa = sgt.Towary.Wczytaj(k.Zamowienie.DostawaRodzaj);
                TwCeny dostCeny = dostawa.Ceny;
                foreach (TwCena c in dostCeny)
                {
                    c.Brutto = k.Zamowienie.DostawaKwota;

                }
                dostawa.Zapisz();
                dostawa.Zamknij();

                if (double.Parse(k.Zamowienie.DostawaKwota) != 0d)
                {
                    SuPozycja poz = zkDokument.Pozycje.Dodaj(k.Zamowienie.DostawaRodzaj);
                    //Console.WriteLine(string.Format("towar: {0} poz.CenaNabycia: {1}", poz.TowarNazwa, poz.CenaBruttoPoRabacie));
                    if (poz.CenaBruttoPoRabacie == (decimal)0)
                    {
                        return null;
                    }
                }
                else
                {
                    SuPozycja poz = zkDokument.Pozycje.Dodaj(k.Zamowienie.DostawaRodzaj);
                }
            }


            if (!czyWszystkieTowaryIstnieja)
            {
                Console.WriteLine("Czy wszystkie towary istnieja false ");

                string errorMsg = string.Format("Brak towarów z zamówienia w Subiekcie.{0}Identyfikator zamówienia: {1}", Environment.NewLine, k.Zamowienie.IdZamowienia);
                SendErrorEmail(errorMsg);//SendErrorEmail();

                return null;
            }

            if (zkDokument.WartoscBrutto == (decimal)0)
            {
                Console.WriteLine("wartosc zamowienia == 0");

                string errorMsg = string.Format("Wartość zamówienia nie może być równa 0.00.{0}Identyfikator zamówienia: {1}", Environment.NewLine, k.Zamowienie.IdZamowienia);
                SendErrorEmail(errorMsg);//SendErrorEmail();

                return null;
            }


            /////////////
            Console.WriteLine("k.Zamowienie.Currency: " + k.Zamowienie.Currency);
            if (k.Zamowienie.Currency == "USD")
            {
                zkDokument.PoziomCenyId = 4;
                Console.WriteLine("zkDokument.PoziomCenyId = 4(tak powinno): " + zkDokument.PoziomCenyId);
            }
            if (k.Zamowienie.Currency == "EUR")
            {
                zkDokument.PoziomCenyId = 5;
                Console.WriteLine("zkDokument.PoziomCenyId = 5(tak powinno): " + zkDokument.PoziomCenyId);
            }
            //zkDokument.Pozycje.PrzeliczWedlugPoziomuCen();

            if (rodzaj_sklepu == RodzajSklepu.Shop || rodzaj_sklepu == RodzajSklepu.Geshaft)//(czy_zagraniczna)
            {
                zkDokument.WalutaSymbol = k.Zamowienie.Currency;
                //zkDokument.KategoriaId = 19;
                try
                {
                    zkDokument.WalutaTypKursu = WalutaRodzajKursuEnum.gtaWalutaKursSredni;
                    zkDokument.WalutaTabelaBanku = 1;
                    DateTime today = k.Zamowienie.DataWplaty.Date;//DateTime.Now;
                    //zkDokument.WalutaDataKursu = today.AddDays(-1);
                    //zkDokument.WalutaSymbol = k.Zamowienie.Currency;
                    //zkDokument.WalutaSymbol = "USD";

                    //zkDokument.PobierzKursWaluty();

                    zkDokument.KursCenyTypKursu = WalutaRodzajKursuEnum.gtaWalutaKursSredni;
                    zkDokument.KursCenyTabelaBanku = 1;
                    //zkDokument.KursCenyDataKursu = today.AddDays(-1);

                    //zkDokument.KursCenyDataKursu = DateTime.Now;

                    if (today.DayOfWeek == DayOfWeek.Monday)
                    {
                        zkDokument.KursCenyDataKursu = today.AddDays(-3);
                        zkDokument.WalutaDataKursu = today.AddDays(-3);
                    }
                    else
                    {
                        zkDokument.KursCenyDataKursu = today.AddDays(-1);
                        zkDokument.WalutaDataKursu = today.AddDays(-1);
                    }

                    zkDokument.PobierzKursCeny();
                    zkDokument.PobierzKursWaluty();

                    int dayIndex = 1;
                    while (zkDokument.KursCenyDataKursu.Date >= today.Date)
                    {
                        zkDokument.KursCenyDataKursu = today.AddDays(-dayIndex);
                        zkDokument.PobierzKursCeny();

                        zkDokument.WalutaDataKursu = today.AddDays(-dayIndex);
                        zkDokument.PobierzKursWaluty();

                        dayIndex++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    sgt.Zakoncz();

                    string errorMsg = string.Format("Wystąpił błąd związany z pobraniem kursu waluty do zamówienia o identyfikatorze: {0}{1}{2}", k.Zamowienie.IdZamowienia, Environment.NewLine, e.ToString());
                    SendErrorEmail(errorMsg);//SendErrorEmail();

                    return null;
                }
            }
            ////////////

            Console.WriteLine("zkDokument.PoziomCenyId(B): " + zkDokument.PoziomCenyId);
            zkDokument.Pozycje.PrzeliczWedlugPoziomuCen();

            Console.WriteLine("zkDokument.WartoscBrutto: " + zkDokument.WartoscBrutto);
            Console.WriteLine("zkDokument.PoziomCenyId(A): "+ zkDokument.PoziomCenyId);

            Console.WriteLine("idKontrahenta: " + idKontrahenta);
            Console.WriteLine("okno widoczne!");
            zkDokument.Wyswietl(true);

            Console.ReadKey();
            if (!decimal.Parse(k.Zamowienie.KwotaDoZaplaty.ToString()).Equals(Convert.ToDecimal(zkDokument.WartoscBrutto)))
            {
                Console.WriteLine("k.Zamowienie.KwotaDoZaplaty != zkDokument.WartoscBrutto");

                string errorMsg = string.Format("Wartość zamówienia różni się od wartości zamówienia w Subiekcie.{0}Identyfikator zamówienia: {1}{2}{3}Kwota do zaplaty: {4}Wartość zamowienia (subiekt): {5}", Environment.NewLine, k.Zamowienie.IdZamowienia, Environment.NewLine, k.Zamowienie.KwotaDoZaplaty, Environment.NewLine, zkDokument.WartoscBrutto);
                SendErrorEmail(errorMsg);//SendErrorEmail();

                return null;
            }
            

//            zkDokument.Zapisz();
//            zamowienie.NumerZamowienia = zkDokument.NumerPelny;
//            NumerZamowienia = zkDokument.NumerPelny;
//            zamowienie.IdZamowienia = int.Parse(idZakupu);
            
            

            return zkDokument.NumerPelny;
        }

        /// <summary>
        /// Sprawdza czy zamówienie już istnieje
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        /// <param name="numerZamowienia">Numer zamówienia w subiekcie</param>
        /// <returns></returns>
        public static bool SprawdzCzyZamowienieJuzIstnieje(InsERT.Subiekt sgt, string numerZamowienia)
        {
            var oDok = sgt.SuDokumentyManager.Istnieje(numerZamowienia);
            return oDok;
        }

        public static void WystawFaktureZaliczkowaTest(Subiekt sgt)
        {
            var czas = DateTime.Now.Date.AddDays(-1);
            SuDokument fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzal);
            SuDokument fzaliczkowa = sgt.SuDokumentyManager.DodajZKzal();
//            fsDokument.DataWystawienia = czas;
//            fsDokument.DataOtrzymania = DateTime.Now.Date.AddDays(-2);
            //            fsDokument.DataSprzedazy = czas; - Metoda lub operacja nie jest zaimplementowana
            fsDokument.DataZakonczeniaDostawy = czas;


            Console.WriteLine("czas: " + czas);
            fzaliczkowa.Wyswietl();
        }

        /// <summary>
        /// Metoda tworzy fakture zaliczkową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="nazwaDokumentu">Nazwa (Numer) dokumunetu zamowienia</param>
        /// <param name="typPrzedpalty">Jaki ma być zastosowany typ przedpłaty.
        /// Do wyboru są 3 typy: gotowka, przelew, karta</param>
        /// <param name="kwota">kwota jaka została zapłacona w zaliczce</param>
        public static string WystawFaktureZaliczkowa(InsERT.Subiekt sgt, string nazwaDokumentu, int typPrzedpalty,
            decimal kwota, RodzajSklepu rodzaj_sklepu, Kontrahent k)//bool czy_zagraniczna, Kontrahent k)
        {
            //var fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzal);

            SuDokument fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzal);
            SuDokument fzaliczkowa = sgt.SuDokumentyManager.DodajFSzal();
//            fsDokument.DataWystawienia = DateTime.Now.Date;
            fsDokument.DataZakonczeniaDostawy = k.Zamowienie.DataWplaty.Date;

            switch (rodzaj_sklepu)
            {
                case RodzajSklepu.Sklep:
                    fsDokument.KategoriaId = 18;
                    break;
                case RodzajSklepu.Shop:
                    fsDokument.KategoriaId = 19;
                    break;
                case RodzajSklepu.Geshaft:
                    fsDokument.KategoriaId = 20;
                    break;
            }

            if (rodzaj_sklepu == RodzajSklepu.Shop || rodzaj_sklepu == RodzajSklepu.Geshaft)//(czy_zagraniczna)
            {
                //fsDokument.KategoriaId = 19;
                if (k.Zamowienie.Currency == "USD")
                {
                    fsDokument.PoziomCenyId = 4;
                }
                if (k.Zamowienie.Currency == "EUR")
                {
                    fsDokument.PoziomCenyId = 5;
                }

                try
                {
                    //fsDokument.Pozycje.PrzeliczWedlugPoziomuCen();

                    fsDokument.WalutaTypKursu = WalutaRodzajKursuEnum.gtaWalutaKursSredni;
                    fsDokument.WalutaTabelaBanku = 1;
                    DateTime today = k.Zamowienie.DataWplaty.Date;//DateTime.Now;
                    //fsDokument.WalutaDataKursu = today.AddDays(-1);//DateTime.Now;
                    fsDokument.WalutaSymbol = k.Zamowienie.Currency;
                    //zkDokument.WalutaSymbol = "USD";


                    fsDokument.KursCenyTypKursu = WalutaRodzajKursuEnum.gtaWalutaKursSredni;
                    fsDokument.KursCenyTabelaBanku = 1;
                    //fsDokument.KursCenyDataKursu = today.AddDays(-1);//DateTime.Now;

                    if (today.DayOfWeek == DayOfWeek.Monday)
                    {
                        fsDokument.KursCenyDataKursu = today.AddDays(-3);
                        fsDokument.WalutaDataKursu = today.AddDays(-3);
                    }
                    else
                    {
                        fsDokument.KursCenyDataKursu = today.AddDays(-1);
                        fsDokument.WalutaDataKursu = today.AddDays(-1);
                    }

                    fsDokument.PobierzKursCeny();
                    fsDokument.PobierzKursWaluty();

                    int dayIndex = 1;
                    while (fsDokument.KursCenyDataKursu.Date >= today.Date)
                    {
                        fsDokument.KursCenyDataKursu = today.AddDays(-dayIndex);
                        fsDokument.PobierzKursCeny();

                        fsDokument.WalutaDataKursu = today.AddDays(-dayIndex);
                        fsDokument.PobierzKursWaluty();
                        dayIndex++;
                    }

                    //fsDokument.PobierzKursCeny();
                }
                catch (Exception e)
                {
                    sgt.Zakoncz();

                    string errorMsg = string.Format("Błąd związany z pobraniem waluty dla faktury zaliczkowej.{0}Identyfikator zamówienia: {1}", Environment.NewLine, k.Zamowienie.IdZamowienia);
                    SendErrorEmail(errorMsg);//SendErrorEmail();

                    return null;
                }
            }

            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);

                fzaliczkowa.NaPodstawie(oDok.Identyfikator);

                fsDokument.NaPodstawie(oDok.Identyfikator);
                ///

                Console.WriteLine("typ przedlapty: " + typPrzedpalty);
                Console.WriteLine("kwota: " + kwota);

                switch (typPrzedpalty)
                {
                    case 2:
                        Console.WriteLine("case 2");
                        fsDokument.PlatnoscGotowkaKwota = (decimal)0;
                        fsDokument.PlatnoscKartaKwota = (decimal)0;
                        fsDokument.PlatnoscPrzelewKwota = (decimal)kwota;
                        //fsDokument.InnaDataPlatnosciBank = k.Zamowienie.DataWplaty.Date;
                        break;
                    case 7:
                        Console.WriteLine("case 7");
                        fsDokument.PlatnoscGotowkaKwota = (decimal)0;
                        fsDokument.PlatnoscKartaKwota = (decimal)0;
                        fsDokument.PlatnoscPrzelewKwota = (decimal)kwota;
                        //fsDokument.InnaDataPlatnosciBank = k.Zamowienie.DataWplaty.Date;
                        break;
                    case 3:
                        Console.WriteLine("case 3");
                        fsDokument.PlatnoscGotowkaKwota = (decimal)kwota;
                        fsDokument.PlatnoscKartaKwota = (decimal)0;
                        fsDokument.PlatnoscPrzelewKwota = (decimal)0;
                        //fsDokument.InnaDataPlatnosciKasa = k.Zamowienie.DataWplaty.Date;
                        break;
                    case 0:
                        Console.WriteLine("case 0");
                        fsDokument.PlatnoscGotowkaKwota = (decimal)0;
                        fsDokument.PlatnoscKartaKwota = (decimal)kwota;
                        fsDokument.PlatnoscPrzelewKwota = (decimal)0;

                        switch (k.Zamowienie.Currency)
                        {
                            case "USD":
                                fsDokument.PlatnoscKartaId = 5;
                                break;
                            case "EUR":
                                fsDokument.PlatnoscKartaId = 6;
                                break;
                            case "PLN":
                                fsDokument.PlatnoscKartaId = 3;
                                break;
                        }
                        //fsDokument.PlatnoscKredytTermin = k.Zamowienie.DataWplaty.Date;//test
                        break;
                    case 1:
                        Console.WriteLine("case 1");
                        fsDokument.PlatnoscGotowkaKwota = (decimal)0;
                        fsDokument.PlatnoscKartaKwota = (decimal)kwota;
                        fsDokument.PlatnoscPrzelewKwota = (decimal)0;

                        switch (k.Zamowienie.Currency)
                        {
                            case "USD":
                                fsDokument.PlatnoscKartaId = 5;
                                break;
                            case "EUR":
                                fsDokument.PlatnoscKartaId = 6;
                                break;
                            case "PLN":
                                fsDokument.PlatnoscKartaId = 3;
                                break;
                        }
                        //fsDokument.PlatnoscKredytTermin = k.Zamowienie.DataWplaty.Date;//test
                        break;
                }

                fsDokument.DataZakonczeniaDostawy = k.Zamowienie.DataWplaty.Date;

                //fsDokument.StatusDokumentu = dokumentStatusEnum;
                fsDokument.Przelicz();
                fsDokument.Zapisz();

                string numerPelny = fsDokument.NumerPelny;
                Marshal.ReleaseComObject(fsDokument);

                return numerPelny;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                sgt.Zakoncz();

                string errorMsg = string.Format("Błąd związany ze wstawianiem płatności w Subiekcie. Sprawdź czy data wpłaty nie jest wcześniejsza od daty zamówienia.{0}Identyfikator zamówienia: {1}{2}{3}", Environment.NewLine, k.Zamowienie.IdZamowienia, Environment.NewLine, e.ToString());
                SendErrorEmail(errorMsg);//SendErrorEmail();

                return null;//e.ToString();
                //Environment.Exit(1);
                //Console.WriteLine("typPrzedpalty: " + typPrzedpalty);
                //Console.WriteLine("kwota: " + kwota);
                //throw;
            }

            //return null;
        }

        /// <summary>
        /// Zapisuje fakturę zaliczkową do bazy portalgames
        /// </summary>
        /// <param name="nr_pelny">Numer pełny dokumentu</param>
        /// <param name="order_id">Identyikator zamówienia</param>
        public static void SaveFakturaZaliczkowa(string nr_pelny, int order_id)
        {
            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                var sqlCommand =
                    "INSERT INTO " + ConfigConnection.PortalGamesBazaProperty
                    + ".`order_faktury_zaliczkowe`(order_id, faktura_nr)"
                    + "VALUES(@order_id, @faktura_id)";
                Console.WriteLine("sqlCommand: " + sqlCommand.ToString());
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.Parameters.AddWithValue("@order_id", order_id);
                    cmd.Parameters.AddWithValue("@faktura_id", nr_pelny);
                    conn.Open();

                    cmd.ExecuteNonQuery();
                }

            }
        }

        /// <summary>
        /// Metoda tworzy fakture zakliczkową końcową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// <param name="nazwaDokumentu">Nazwa (Numer) dokumunetu zamowienia</param>
        public static int WystawFaktureZaliczkowaKoncowa(InsERT.Subiekt sgt, string nr_zamowienia, RodzajSklepu rodzaj_sklepu, Kontrahent k, int id_zamowienia)//bool czy_zagraniczna, Kontrahent k, int id_zamowienia)
        {
            //Console.WriteLine("k.NumerZamowienia: " + k.NumerZamowienia);
            SuDokument fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFSzalkonc);
            fsDokument.DataWystawienia = DateTime.Now.Date;//k.Zamowienie.DataWplaty.Date;
            fsDokument.DataSprzedazy = k.Zamowienie.DataWplaty.Date;

            switch (rodzaj_sklepu)
            {
                case RodzajSklepu.Sklep:
                    fsDokument.KategoriaId = 18;
                    break;
                case RodzajSklepu.Shop:
                    fsDokument.KategoriaId = 19;
                    break;
                case RodzajSklepu.Geshaft:
                    fsDokument.KategoriaId = 20;
                    break;
            }

            if (rodzaj_sklepu == RodzajSklepu.Shop || rodzaj_sklepu == RodzajSklepu.Geshaft)//(czy_zagraniczna)
            {
                //fsDokument.KategoriaId = 19;
                if (k.Zamowienie.Currency == "USD")
                {
                    fsDokument.PoziomCenyId = 4;
                }
                if (k.Zamowienie.Currency == "EUR")
                {
                    fsDokument.PoziomCenyId = 5;
                }

                try
                {
                    //fsDokument.Pozycje.PrzeliczWedlugPoziomuCen();

                    fsDokument.WalutaTypKursu = WalutaRodzajKursuEnum.gtaWalutaKursSredni;
                    fsDokument.WalutaTabelaBanku = 1;
                    DateTime today = k.Zamowienie.DataWplaty.Date;//DateTime.Now;
                    //fsDokument.WalutaDataKursu = today.AddDays(-1);//DateTime.Now;
                    fsDokument.WalutaSymbol = k.Zamowienie.Currency;
                    //zkDokument.WalutaSymbol = "USD";

                    //fsDokument.PobierzKursWaluty();

                    fsDokument.KursCenyTypKursu = WalutaRodzajKursuEnum.gtaWalutaKursSredni;
                    fsDokument.KursCenyTabelaBanku = 1;
                    //fsDokument.KursCenyDataKursu = today.AddDays(-1);//DateTime.Now;

                    if (today.DayOfWeek == DayOfWeek.Monday)
                    {
                        fsDokument.KursCenyDataKursu = today.AddDays(-3);
                        fsDokument.WalutaDataKursu = today.AddDays(-3);
                    }
                    else
                    {
                        fsDokument.KursCenyDataKursu = today.AddDays(-1);
                        fsDokument.WalutaDataKursu = today.AddDays(-1);
                    }

                    fsDokument.PobierzKursCeny();
                    fsDokument.PobierzKursWaluty();

                    int dayIndex = 1;
                    while (fsDokument.KursCenyDataKursu.Date >= today.Date)
                    {
                        fsDokument.KursCenyDataKursu = today.AddDays(-dayIndex);
                        fsDokument.PobierzKursCeny();

                        fsDokument.WalutaDataKursu = today.AddDays(-dayIndex);
                        fsDokument.PobierzKursWaluty();

                        dayIndex++;
                    }

                    //fsDokument.PobierzKursCeny();
                }
                catch (Exception e)
                {
                    sgt.Zakoncz();

                    string errorMsg = string.Format("Błąd związany z pobraniem waluty dla faktury końcowej.{0}Identyfikator zamówienia: {1}{2}{3}", Environment.NewLine, k.Zamowienie.IdZamowienia, Environment.NewLine, e.ToString());
                    SendErrorEmail(errorMsg);//SendErrorEmail();
                }
            }

            try
            {
                var oDok = sgt.SuDokumentyManager.Wczytaj(nr_zamowienia);//(k.NumerZamowienia);//nazwaDokumentu

                fsDokument.NaPodstawie(oDok.Identyfikator);
                //var fz_iden = fsDokument.Identyfikator;
                Console.WriteLine("fsDokument.Identyfikator: " + fsDokument.Identyfikator);

                Console.WriteLine("okno widoczne!");
                fsDokument.Wyswietl(true);

                fsDokument.Przelicz();
                //fsDokument.Zapisz();

                string uriPliku = string.Format(@"static\FakturaKoncowaNr{0}{1}", fsDokument.Identyfikator, ".pdf");
                
                uriPliku = uriPliku.Replace(@"\", @"/");

                return fsDokument.Identyfikator;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                sgt.Zakoncz();

                string errorMsg = string.Format("Błąd związany z zapisaniem faktury końcowej (sprawdź czy istnieje powiązane zamówienie w Subiekcie).{0}Identyfikator zamówienia: {1}{2}{3}", Environment.NewLine, k.Zamowienie.IdZamowienia, Environment.NewLine, e.ToString());
                SendErrorEmail(errorMsg);//SendErrorEmail();

                return 0;
                //sgt.Zakoncz();
                //Environment.Exit(1);
                //throw;
            }
        }

        /// <summary>
        /// Metoda zapisuje fakturę końcową do bazy Portalgames
        /// </summary>
        /// <param name="faktura_id">Identyfikator faktury</param>
        /// <param name="id_zamowienia">Identyfikator zamówienia</param>
        public static void ZapiszFaktureKoncowa(string faktura_id, int id_zamowienia, Kontrahent k = null)
        {
            var sqlCommand =
                "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET faktura_koncowa_numer = @fkn WHERE id = @order_id;";
            if (k != null)
            {
                switch (k.Zamowienie.DostawaRodzajId)
                {
                    case 8:
                        sqlCommand = "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET faktura_koncowa_numer = @fkn, postage_code = 'Kurier DHL POBRANIE', sent = 1 WHERE id = @order_id;";
                        break;
                    case 13:
                        sqlCommand = "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET faktura_koncowa_numer = @fkn, postage_code = 'ODBIÓR OSOBISTY', sent = 1 WHERE id = @order_id;";
                        break;
                    case 7:
                        sqlCommand = "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET faktura_koncowa_numer = @fkn, postage_code = 'Kurier DHL', sent = 1 WHERE id = @order_id;";
                        break;
                }
            }

            Console.WriteLine("ZapiszFaktureKoncowa() faktura_id: " + faktura_id);

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                Console.WriteLine("sqlCommand: " + sqlCommand.ToString());
                using (var cmd = conn.CreateCommand())
                {
                    Console.WriteLine("execute command");
                    cmd.CommandText = sqlCommand;
                    cmd.Parameters.AddWithValue("@fkn", faktura_id);
                    cmd.Parameters.AddWithValue("@order_id", id_zamowienia);
                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Metoda pobiera Numer Pełny automatycznego wydania zewnętrznęgo z bazy Subiekta oraz drukuje ten dokument do pliku pdf na serwerze
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        /// <param name="fak_konc_numer">Numer faktury końcowej</param>
        /// <returns></returns>
        private static string WydanieZew(InsERT.Subiekt sgt, int fak_konc_numer)
        {
            subiektConn.UserID = ConfigConnection.UzytkownikProperty;
            subiektConn.Password = ConfigConnection.UzytkownikHasloProperty;
            subiektConn.InitialCatalog = ConfigConnection.BazaGtProperty;
            subiektConn.DataSource = ConfigConnection.ServerGtProperty;

            string sqlCommand = "SELECT dok_NrPelny FROM " + subiektConn.InitialCatalog + ".dbo.dok__Dokument WHERE dok_DoDokId = @fak_konc AND dok_Typ = 11;";

            //sgt.Okno.Widoczne = true;
            //MySqlConnectionStringBuilder subiektconn = ;
            using (var conn = new SqlConnection(subiektConn.ToString()))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("fak_konc", fak_konc_numer);
                    cmd.CommandText = sqlCommand;
                    Console.WriteLine("WydanieZew, sqlCommand: " + sqlCommand.ToString());
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string nr_pelny_wz = reader["dok_NrPelny"].ToString();
                        Console.WriteLine("WydanieZew, nr_pelny_wz: " + nr_pelny_wz.ToString());

                        if (sgt.Dokumenty.Istnieje(nr_pelny_wz))
                        {
                            SuDokument wz = sgt.Dokumenty.Wczytaj(nr_pelny_wz);

                            string uriPliku = string.Format(@"static\WydanieZewnetrzeNr{0}{1}", wz.Numer, ".pdf");
                            //string sciezkaPliku = HttpContext.Current.Server.MapPath("~") + uriPliku;//AppDomain.CurrentDomain.BaseDirectory + uriPliku;
                            //Console.WriteLine("nazwaPliku: " + sciezkaPliku);

                            //wz.Wyswietl(false);
                            //wz.ZapiszSymulacja();
                            //wz.DrukujDoPliku(sciezkaPliku, TypPlikuEnum.gtaTypPlikuPDF);//DrukujDoPlikuM(@"c:\", TypPlikuEnum.gtaTypPlikuPDF);
                            //wz.DrukujDoPliku(@"c:\", TypPlikuEnum.gtaTypPlikuPDF);
                            uriPliku = uriPliku.Replace(@"\", @"/");

                            return uriPliku;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    reader.Close();

                    return null;
                }
            }
        }
        

        /// <summary>
        /// Metoda zwraca aktualnego kontrahenta, który dokonał operacji na tabeli order (złożył zamówienie)
        /// </summary>
        /// <returns>Zwraca obiekt Kontrahent</returns>
        public static Kontrahent PobierzKontrahenta(int order_id)
        {
            var kontrahent = new Kontrahent();
            Console.WriteLine("order.id: " + order_id);
            //var sqlCommand = "select * from baza8706_devportalgames.`order` order by 1 desc LIMIT 1;";
            var sqlCommand =
                //"select * from "+ConfigConnection.PortalGamesBazaProperty + ".`order` WHERE id = @order_id;";
                "select o.*, s.name as panstwo from " + ConfigConnection.PortalGamesBazaProperty + ".`order` o JOIN `state` s ON(o.address_state_id=s.id) WHERE o.id = @order_id;";

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
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
                        kontrahent.Ulica = reader["address_street"].ToString();
                        //== string.Empty
                        //? "brak"
                        //: reader["address_street"].ToString();
                        kontrahent.Miasto = reader["address_city"].ToString();
                        //== string.Empty
                        //? "brak"
                        //: reader["address_city"].ToString();
                        kontrahent.Zip = reader["address_zip"].ToString();
                        kontrahent.DatabaseType = reader["customer_database_type"].ToString();
                        //== string.Empty
                        //? "brak"
                        //: reader["address_zip"].ToString();
                        kontrahent.PanstwoKod = reader["address_state_id"].ToString();//== string.Empty
                        //? "brak"
                        //: reader["address_state_id"].ToString();
                        kontrahent.PanstwoNazwa = reader["panstwo"].ToString();
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
                        kontrahent.Zamowienie.IdZamowienia = int.Parse(reader["id"].ToString());
                        kontrahent.Zamowienie.WartoscProduktu = reader["sum"].ToString() == string.Empty
                            ? "0"
                            : reader["sum"].ToString();
                        //kontrahent.Zamowienie.IloscWplat = reader["payment_id"].ToString() == string.Empty
                        //    ? "brak"
                        //    : reader["payment_id"].ToString();//jakie ilosc wplat toz to jest id rodzaju platnosci!
                        kontrahent.Zamowienie.WplataRodzaj = int.Parse(reader["payment_id"].ToString()); //== string.Empty
                        //? "0"
                        //: reader["payment_id"].ToString();
                        kontrahent.Zamowienie.DostawaKwota = reader["postage_price"].ToString() == string.Empty
                            ? "0"
                            : reader["postage_price"].ToString();
                        kontrahent.NIP = reader["company_nip"].ToString();
                        kontrahent.Email = reader["customer_email"].ToString();
                        //kontrahent.Zamowienie.DostawaRodzaj = int.Parse(reader["postage_id"].ToString());
                        int dostawa_id = int.Parse(reader["postage_id"].ToString());
                        kontrahent.Zamowienie.DostawaRodzajId = dostawa_id;
                        kontrahent.Zamowienie.DostawaRodzaj = "DOST";//dostawy.Where(d => d.Value.Equals(dostawa_id)).Select(p => p.Key).FirstOrDefault();
                        kontrahent.Zamowienie.FakturaKoncowaNumer = reader["faktura_koncowa_numer"].ToString();
                        kontrahent.Zamowienie.PaymentError = int.Parse(reader["payment_error"].ToString());
                        kontrahent.Zamowienie.Currency = reader["currency"].ToString();

                        kontrahent.NazwaFirmy = reader["company_name"].ToString();
                        kontrahent.AdresFirma = reader["caddress_street"].ToString();
                        kontrahent.KodPocztowyFirma = reader["caddress_zip"].ToString();
                        kontrahent.MiastoFirma = reader["caddress_city"].ToString();

                        kontrahent.Zamowienie.KwotaDoZaplaty = double.Parse(kontrahent.Zamowienie.WartoscProduktu) + double.Parse(kontrahent.Zamowienie.DostawaKwota);
                    }
                }
            }

            kontrahent.Zamowienie.WplaconaKwotaSum = PobierzWplatySum(kontrahent.Zamowienie.IdZamowienia).ToString();
            //Console.WriteLine("wplacona kwota sum: "+ kontrahent.Zamowienie.WplaconaKwotaSum);
            //if (Convert.ToInt32(kontrahent.Zamowienie.IloscWplat) > 1)
            //{
            sqlCommand =
                //"select * from baza8706_devportalgames.order_pay_history where" + " order_id = " + kontrahent.IdZakupu + " order by 1 desc limit 1;";
                "select * from " + ConfigConnection.PortalGamesBazaProperty + ".order_pay_history where" + " order_id = " + kontrahent.IdZakupu + " and price_brutto > 0 order by 1 desc limit 1;";

            kontrahent.Zamowienie.WplaconaKwota = "0";
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;

                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kontrahent.Zamowienie.WplaconaKwota = reader["price_brutto"].ToString();
                        kontrahent.Zamowienie.DataWplaty = DateTime.Parse(reader["pay_date"].ToString()).Date;
                    }
                }
            }
            //}
            return kontrahent;
        }

        /// <summary>
        /// Pobieranie dotychczasowych wpłat dla zamowienia z portalgames
        /// </summary>
        /// <param name="order_id">Identyfikator zamówienia</param>
        /// <returns></returns>
        public static double PobierzWplatySum(int order_id)
        {
            Console.WriteLine("PobierzWplatySum o_id: " + order_id);

            var sqlCommand =
                //"select * from baza8706_devportalgames.order_pay_history WHERE order_id = @order_id;";
                "select * from " + ConfigConnection.PortalGamesBazaProperty + ".order_pay_history WHERE order_id = @order_id and price_brutto > 0;";

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.Parameters.AddWithValue("order_id", order_id);

                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    double wplaconaKwotaSum = 0;
                    while (reader.Read())
                    {
                        double priceBrutto = double.Parse(reader["price_brutto"].ToString());
                        wplaconaKwotaSum += priceBrutto;
                        //kontrahent.Zamowienie.WplaconaKwota = reader["price_brutto"].ToString();
                    }

                    return wplaconaKwotaSum;
                }
            }
        }

        /// <summary>
        /// Metoda pobiera wybrane towary z subiekta
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        /// <param name="productsSymbols">Lista Produktow</param>
        /// <returns></returns>
        public static List<Towar> PobierzWybraneTowaryZsubiekt(InsERT.Subiekt sgt, List<Produkt> productsSymbols)
        {
            //Console.WriteLine("PobierzWybraneTowaryZsubiekt");

            Towary towary = sgt.Towary;
            List<Towar> lista = new List<Towar>();

            foreach (Produkt product in productsSymbols)
            {
                Towar towar = towary.Wczytaj(product.Symbol);

                if (towar != null)
                {
                    lista.Add(towar);
                }
            }

            return lista;
        }

        /// <summary>
        /// Metoda zapisuje sciezke dokumentu Wydania Zewnetrznego do tabeli bazy portalgames
        /// </summary>
        /// <param name="sciezka">Lokalizajca na serwerze, gdzie ma znajduje się dokument wydania zewnętrznego</param>
        /// <param name="order_id">Identyfikator zamówienia</param>
        public static void ZapiszSciezkeDokumentuWydZewDoBazyPortalGames(string sciezka, int order_id)
        {
            string sqlCommand =
                //"UPDATE baza8706_devportalgames.`order` SET wydanie_zewnetrzne_url = @sciezka WHERE id = @order_id";
                "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET wydanie_zewnetrzne_url = @sciezka WHERE id = @order_id";

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.Parameters.AddWithValue("@order_id", order_id);
                    if (string.IsNullOrEmpty(sciezka))
                    {
                        cmd.Parameters.AddWithValue("@sciezka", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@sciezka", sciezka);
                    }

                    //Console.WriteLine("connection string: " + conn.ConnectionString);
                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Sprawdza czy suma wpłaconych zaliczek jest większa od wymaganej kwoty z zamowienia
        /// </summary>
        /// <param name="order_id">Identyfikator zamówienia</param>
        /// <returns></returns>
        public static bool CzyBlednaKwotaSave(int order_id)
        {
            Kontrahent k = PobierzKontrahenta(order_id);

            double wplaconaKwotaSum = Double.Parse(k.Zamowienie.WplaconaKwotaSum);
            if (k.Zamowienie.KwotaDoZaplaty >= wplaconaKwotaSum)
            {
                BlednaKwotaSave(0, order_id);
                return true;
            }
            else
            {
                BlednaKwotaSave(1, order_id);
                return false;
            }
        }

        /// <summary>
        /// Metoda ustawia blad payment_error do bazy portalgames
        /// </summary>
        /// <param name="e">Wartść błędu (1 oznacza błąd, 0 oznacza brak błędu)</param>
        /// <param name="order_id">Identyfikator zamówienia</param>
        public static void BlednaKwotaSave(int e, int order_id)
        {
            string sqlCommand =
                //"UPDATE baza8706_devportalgames.`order` SET payment_error = @e WHERE id = @order_id";
                "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET payment_error = @e WHERE id = @order_id";

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.Parameters.AddWithValue("@order_id", order_id);
                    cmd.Parameters.AddWithValue("@e", e);
                    //Console.WriteLine("connection string: " + conn.ConnectionString);
                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Metoda umożliwia ustawienie wartosci kolumny zaplacone w bazie portalgames
        /// </summary>
        /// <param name="v">Wartosc zapisywana do tabeli</param>
        /// <param name="order_id">Identyfikator zamówienia</param>
        public static void ZamowienieOplacone(int v, int order_id)
        {
            string sqlCommand =
                "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET zaplacone = @v WHERE id = @order_id";

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.Parameters.AddWithValue("@order_id", order_id);
                    cmd.Parameters.AddWithValue("@v", v);
                    //Console.WriteLine("connection string: " + conn.ConnectionString);
                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Metoda pobiera rabat dla produktu
        /// </summary>
        /// <param name="order_id">Identyfikator zamówienia</param>
        /// <param name="product_code">Kod produktu</param>
        /// <param name="conn">Otwarte połączenie z bazą MySql</param>
        /// <returns></returns>
        private static List<Double> PobierzRabatDlaProduktu(int order_id, string product_code, MySqlConnection conn)
        {
            string sqlCommand = "SELECT discount FROM `order_item` WHERE product_code = @productCode AND order_id = @orderId;";
            List<Double> discount = new List<Double>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("productCode", product_code);
                cmd.Parameters.AddWithValue("orderId", order_id);
                cmd.CommandText = sqlCommand;

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    discount.Add(double.Parse(reader["discount"].ToString()));
                }
                reader.Close();
            }
            return discount;
        }

        /// <summary>
        /// Publiczna metoda do wysyłania wiadomości o zaistniałych błędach
        /// </summary>
        /// <param name="errorMessage">Tekst wiadomości</param>
        public static void SendErrorEmail(string errorMessage)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            Console.WriteLine("SendErrorEmail");
            //MailMessage mail = new MailMessage("errors@portalgames.pl", emailTo);
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            NetworkCredential mailCredential = new NetworkCredential("errors@portalgames.pl", "0SWc4kbX29");
            client.Credentials = mailCredential;
            //client.EnableSsl = true;
            client.Host = "smtp.portalgames.pl";

            foreach (string e in ConfigConnection.ErrorEmailsList)
            {
                MailMessage mail = new MailMessage("errors@portalgames.pl", e);
                mail.Subject = "Błąd w zamówieniu";
                mail.Body = errorMessage;
                client.Send(mail);
            }
        }

        /// <summary>
        /// Metoda integrująca sklep internetowy z Subiektem. Najpierw pobiera ostatnie zamówienie z bazy sklepu internetowego 
        /// nastepnie dodaje kontrahenta do bazy subiekta jeśli jeszcze go nie ma. W kolejnym etapie 
        /// sprawdza czy istnieje już w subiekcie zamówienie (jeśli nie to je dodaje). Następnie sprawdza 
        /// czy została wpłacona jakaś kwota za produkt, jeśli tak to dodaje wystawia fakture zaliczkową
        /// a jeśli kwota jest pełna - to znaczy jeśli produkt został w pełni opłacony to wystawia fakturę zaliczkową końcową
        /// </summary>
        /// <param name="sgt">Obiekt klasy InsERT.Subiekt</param>
        /// 
        public static int ZakupProces(InsERT.Subiekt sgt, int order_id, RodzajSklepu rodzajSklepu)//bool czy_zagraniczne)
        {
            //Console.WriteLine("Zakup Proces order_id: "+order_id);
            var kontrahent = PobierzKontrahenta(order_id);

            if (kontrahent != null)
            {
                if (string.IsNullOrEmpty(kontrahent.Zamowienie.FakturaKoncowaNumer))
                {
                    //Console.WriteLine("kontrahent != null");
                    int panstwo_id = 1;
                    if (!string.IsNullOrEmpty(kontrahent.PanstwoKod))
                    {
                        panstwo_id =
                            kontrahent.PobierzIdPanstwa(kontrahent.PanstwoNazwa);//panstwa.Where(p => p.Value.Equals(kontrahent.PanstwoKod)).Select(p => p.Key).FirstOrDefault();
                    }

                    DodajKontrahenta(sgt, kontrahent.Nazwa, kontrahent.Id, kontrahent.Miasto, kontrahent.Ulica, panstwo_id,
                        kontrahent.MiastoKod, kontrahent.NIP, kontrahent.Email, kontrahent.Imie, kontrahent.Nazwisko,
                        kontrahent.NazwaFirmy, kontrahent.AdresFirma, kontrahent.KodPocztowyFirma, kontrahent.MiastoFirma);
                    var kontrahentSubiekt = sgt.Kontrahenci.Wczytaj(kontrahent.Id);
                    int idkontrahentaSubiekt = kontrahentSubiekt.Identyfikator();
                    List<Produkt> listaProduktow = new List<Produkt>();

                    if (kontrahent.Zamowienie.WplataRodzaj == 3)
                    {
                        kontrahent.Zamowienie.WplaconaKwota = kontrahent.Zamowienie.KwotaDoZaplaty.ToString();
                        kontrahent.Zamowienie.WplaconaKwotaSum = kontrahent.Zamowienie.KwotaDoZaplaty.ToString();
                        kontrahent.DodajPlatnoscDoPortalgames();
                    }

                    Console.WriteLine("kontrahent.Zamowienie.WplaconaKwota: " + kontrahent.Zamowienie.WplaconaKwota);
                    Console.WriteLine("kontrahent.Zamowienie.WplaconaKwotaSum: " + kontrahent.Zamowienie.WplaconaKwotaSum);
                    Console.WriteLine("kontrahent.Zamowienie.KwotaDoZaplaty: " + kontrahent.Zamowienie.KwotaDoZaplaty);
                    if (Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwota) > 0
                        && Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwota) == Convert.ToDecimal(kontrahent.Zamowienie.KwotaDoZaplaty)
                        && Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwotaSum) == Convert.ToDecimal(kontrahent.Zamowienie.KwotaDoZaplaty)
                        && rodzajSklepu != RodzajSklepu.Hurt)
                    {
                        if (!SprawdzCzyZamowienieJuzIstnieje(sgt, kontrahent.NumerZamowienia))
                        {
                            Console.WriteLine("Zamowienie nie istnieje!");
                            listaProduktow = PobierzListeProduktowZZamowienia(kontrahent.IdZakupu);
                            Console.WriteLine("lista produktow.Count: " + listaProduktow.Count());

                            string zamowienie = DodajZamowienie(sgt, idkontrahentaSubiekt, listaProduktow, kontrahent.IdZakupu, kontrahent, rodzajSklepu);//czy_zagraniczne);

                            if (!string.IsNullOrEmpty(zamowienie))
                            {
                                kontrahent.NumerZamowienia = zamowienie.ToString();
                            }
                            else
                            {
                                Console.WriteLine("blad nie ma towaru!");
                                BlednaKwotaSave(1, kontrahent.Zamowienie.IdZamowienia);
                                return -1;
                            }
                            //}
                            //else
                            //{
                            //    Console.WriteLine("zamowienie juz istnieje");
                            //}

                            //Console.WriteLine("kontrahent.NumerZamowienia: " + kontrahent.NumerZamowienia);

                            var kwota = Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwota);
                            string nr_fak_zal = null;
                            if (kontrahent.Zamowienie.WplataRodzaj != 3)
                            {
                                nr_fak_zal = WystawFaktureZaliczkowa(sgt, kontrahent.NumerZamowienia, kontrahent.Zamowienie.WplataRodzaj, kwota, rodzajSklepu, kontrahent);//czy_zagraniczne, kontrahent);
                            }
//                            else
//                            {
                                //kontrahent.Zamowienie.WplaconaKwotaSum = kontrahent.Zamowienie.KwotaDoZaplaty.ToString();
//                                KeyValuePair<int, string> kv = WystawFaktureKoncowaPrzyPobraniu(sgt, kontrahent.NumerZamowienia, kontrahent, rodzajSklepu);//.SingleOrDefault(r => !string.IsNullOrEmpty(r.Value));
//                                if (!kv.Equals(default(KeyValuePair<int, string>)))
//                                {
//                                    nr_fak_zal = kv.Value;

//                                    int id_fak_zal = kv.Key;
//                                    FakturaKoncowaMechanizm(sgt, kontrahent.Zamowienie.IdZamowienia, rodzajSklepu, id_fak_zal);//czy_zagraniczne, id_fak_zal);
//                                }
//                                else
//                                {
//                                    string errorMsg = string.Format("WystawFaktureKoncowaPrzyPobraniu, blad.{0}Numer faktury: {1}, Identyfikator faktury: {2}",
//                                        Environment.NewLine, kv.Key, kv.Value);
//                                    SendErrorEmail(errorMsg);
//                                    BlednaKwotaSave(0, kontrahent.Zamowienie.IdZamowienia);
//                                }
//                            }

                            if (nr_fak_zal != null)
                            {
                                SaveFakturaZaliczkowa(nr_fak_zal, kontrahent.Zamowienie.IdZamowienia);
                                ZamowienieOplacone(1, kontrahent.Zamowienie.IdZamowienia);
                            }
                            else
                            {
                                BlednaKwotaSave(0, kontrahent.Zamowienie.IdZamowienia);
                                return -4;
                            }

                            if (kontrahent.Zamowienie.PaymentError == 1)
                            {
                                BlednaKwotaSave(0, kontrahent.Zamowienie.IdZamowienia);
                            }
                        }
                    }
                    else if (rodzajSklepu == RodzajSklepu.Hurt)
                    {
                        listaProduktow = PobierzListeProduktowZZamowienia(kontrahent.IdZakupu);
                        string zamowienie = DodajZamowienie(sgt, idkontrahentaSubiekt, listaProduktow, kontrahent.IdZakupu, kontrahent, rodzajSklepu);//czy_zagraniczne);

                        if (!string.IsNullOrEmpty(zamowienie))
                        {
                            kontrahent.NumerZamowienia = zamowienie.ToString();

                            if (rodzajSklepu == RodzajSklepu.Hurt)
                            {
                                ZamowienieOplacone(1, kontrahent.Zamowienie.IdZamowienia);
                                return 2;
                            }
                        }
                        else
                        {
                            Console.WriteLine("blad nie ma towaru(hurt)!");
                            BlednaKwotaSave(1, kontrahent.Zamowienie.IdZamowienia);
                            return -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("bledna kwota!");
                        BlednaKwotaSave(1, kontrahent.Zamowienie.IdZamowienia);

                        string errorMsg = string.Format("Wpłacona kwota przewyższa wartość zamówienia.{0}Wartość zamównienia: {1}, Wpłacona kwota: {2}{3}Identyfikator zamówienia: {4}",
                            Environment.NewLine, kontrahent.Zamowienie.KwotaDoZaplaty, kontrahent.Zamowienie.WplaconaKwota, Environment.NewLine, kontrahent.Zamowienie.IdZamowienia);
                        SendErrorEmail(errorMsg);

                        return -2;
                    }

                    //Console.WriteLine("kwota do zaplaty: "+ kontrahent.Zamowienie.KwotaDoZaplaty);
                    //Console.WriteLine("WplaconaKwotaSum: "+kontrahent.Zamowienie.WplaconaKwotaSum);
                    //if (Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwotaSum).Equals(Convert.ToDecimal(kontrahent.Zamowienie.KwotaDoZaplaty)))
                    //    //Convert.ToDouble(kontrahent.Zamowienie.KwotaDoZaplaty))
                    //{
                    //    ZamowienieOplacone(1, kontrahent.Zamowienie.IdZamowienia);
                    //    return 2;
                    //}
                }
                else
                {
                    return 1;
                    //Console.WriteLine("Zamowienie.FakturaKoncowaNumer: "+kontrahent.Zamowienie.FakturaKoncowaNumer);
                }

                return 0;
            }
            else
            {
                //Console.WriteLine("Kontrahent == null");
                return -3;
            }
        }
        //        public static int ZakupProces(InsERT.Subiekt sgt, int order_id, RodzajSklepu rodzajSklepu)//bool czy_zagraniczne)
        //        {
        //            //Console.WriteLine("Zakup Proces order_id: "+order_id);
        //            var kontrahent = PobierzKontrahenta(order_id);
        //
        //            if (kontrahent != null)
        //            {
        //                //if (string.IsNullOrEmpty(kontrahent.Zamowienie.FakturaKoncowaNumer))
        //                //{
        //                    //Console.WriteLine("kontrahent != null");
        //                    int panstwo_id = 1;
        //                    if (!string.IsNullOrEmpty(kontrahent.PanstwoKod))
        //                    {
        //                        panstwo_id =
        //                            kontrahent.PobierzIdPanstwa(kontrahent.PanstwoNazwa);//panstwa.Where(p => p.Value.Equals(kontrahent.PanstwoKod)).Select(p => p.Key).FirstOrDefault();
        //                    }
        //
        //                    DodajKontrahenta(sgt, kontrahent.Nazwa, kontrahent.Id, kontrahent.Miasto, kontrahent.Ulica, panstwo_id,
        //                        kontrahent.MiastoKod, kontrahent.Imie, kontrahent.Nazwisko);
        //                    var kontrahentSubiekt = sgt.Kontrahenci.Wczytaj(kontrahent.Id);
        //                    int idkontrahentaSubiekt = kontrahentSubiekt.Identyfikator();
        //                    List<Produkt> listaProduktow = new List<Produkt>();
        //
        //                    //if (kontrahent.Zamowienie.WplataRodzaj == 3)
        //                    //{
        //                    //    kontrahent.Zamowienie.WplaconaKwota = kontrahent.Zamowienie.KwotaDoZaplaty.ToString();
        //                    //    kontrahent.Zamowienie.WplaconaKwotaSum = kontrahent.Zamowienie.KwotaDoZaplaty.ToString();
        //                    //    kontrahent.DodajPlatnoscDoPortalgames();
        //                    //}
        //
        //                    Console.WriteLine("kontrahent.Zamowienie.WplaconaKwota: " + kontrahent.Zamowienie.WplaconaKwota);
        //                    Console.WriteLine("kontrahent.Zamowienie.WplaconaKwotaSum: " + kontrahent.Zamowienie.WplaconaKwotaSum);
        //                    Console.WriteLine("kontrahent.Zamowienie.KwotaDoZaplaty: " + kontrahent.Zamowienie.KwotaDoZaplaty);
        //                    if (Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwota) > 0
        //                        && Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwota) <= Convert.ToDecimal(kontrahent.Zamowienie.KwotaDoZaplaty)
        //                        && Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwotaSum) <= Convert.ToDecimal(kontrahent.Zamowienie.KwotaDoZaplaty))
        //                    {
        //                        //if (!SprawdzCzyZamowienieJuzIstnieje(sgt, kontrahent.NumerZamowienia))
        //                        //{
        //                            Console.WriteLine("Zamowienie nie istnieje!");
        //                            listaProduktow = PobierzListeProduktowZZamowienia(kontrahent.IdZakupu);
        //                            Console.WriteLine("lista produktow.Count: " + listaProduktow.Count());
        //
        //                            string zamowienie = DodajZamowienie(sgt, idkontrahentaSubiekt, listaProduktow, kontrahent.IdZakupu, kontrahent, rodzajSklepu);//czy_zagraniczne);
        //
        //                            //if (!string.IsNullOrEmpty(zamowienie))
        //                            //{
        //                            //    kontrahent.NumerZamowienia = zamowienie.ToString();
        //                            //}
        //                            //else
        //                            //{
        //                            //    Console.WriteLine("blad nie ma towaru!");
        //                            //    BlednaKwotaSave(1, kontrahent.Zamowienie.IdZamowienia);
        //                            //    return -1;
        //                            //}
        //                        //}
        //                        //else
        //                        //{
        //                        //    Console.WriteLine("zamowienie juz istnieje");
        //                        //}
        //
        //                        //Console.WriteLine("kontrahent.NumerZamowienia: " + kontrahent.NumerZamowienia);
        //
        //                        var kwota = Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwota);
        //                        string nr_fak_zal = null;
        //                        if (kontrahent.Zamowienie.WplataRodzaj != 3)
        //                        {
        //                            nr_fak_zal = WystawFaktureZaliczkowa(sgt, kontrahent.NumerZamowienia, kontrahent.Zamowienie.WplataRodzaj, kwota, rodzajSklepu, kontrahent);//czy_zagraniczne, kontrahent);
        //                        }
        //                        //else
        //                        //{
        //                        //    //kontrahent.Zamowienie.WplaconaKwotaSum = kontrahent.Zamowienie.KwotaDoZaplaty.ToString();
        //                        //    KeyValuePair<int, string> kv = WystawFaktureKoncowaPrzyPobraniu(sgt, kontrahent.NumerZamowienia, kontrahent).SingleOrDefault(r => r.Value != null);
        //                        //    nr_fak_zal = kv.Value;
        //                        //    int id_fak_zal = kv.Key;
        //                        //    FakturaKoncowaMechanizm(sgt, kontrahent.Zamowienie.IdZamowienia, rodzajSklepu, id_fak_zal);//czy_zagraniczne, id_fak_zal);
        //                        //}
        //
        //                        //if (nr_fak_zal != null)
        //                        //{
        //                        //    SaveFakturaZaliczkowa(nr_fak_zal, kontrahent.Zamowienie.IdZamowienia);
        //                        //}
        //                        //else
        //                        //{
        //                        //    BlednaKwotaSave(0, kontrahent.Zamowienie.IdZamowienia);
        //                        //    return -4;
        //                        //}
        //
        //                        //if (kontrahent.Zamowienie.PaymentError == 1)
        //                        //{
        //                        //    BlednaKwotaSave(0, kontrahent.Zamowienie.IdZamowienia);
        //                        //}
        //                        //}
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("bledna kwota!");
        //                        //BlednaKwotaSave(1, kontrahent.Zamowienie.IdZamowienia);
        //
        //                        //string errorMsg = string.Format("Wpłacona kwota przewyższa wartość zamówienia.{0}Wartość zamównienia: {1}, Wpłacona kwota: {2}{3}Identyfikator zamówienia: {4}",
        //                        //    Environment.NewLine, kontrahent.Zamowienie.KwotaDoZaplaty, kontrahent.Zamowienie.WplaconaKwota, Environment.NewLine, kontrahent.Zamowienie.IdZamowienia);
        //                        //SendErrorEmail(errorMsg);
        //
        //                        return -2;
        //                    }
        //
        //                    //Console.WriteLine("kwota do zaplaty: "+ kontrahent.Zamowienie.KwotaDoZaplaty);
        //                    //Console.WriteLine("WplaconaKwotaSum: "+kontrahent.Zamowienie.WplaconaKwotaSum);
        //                    if (Convert.ToDecimal(kontrahent.Zamowienie.WplaconaKwotaSum).Equals(Convert.ToDecimal(kontrahent.Zamowienie.KwotaDoZaplaty)))
        //                    //Convert.ToDouble(kontrahent.Zamowienie.KwotaDoZaplaty))
        //                    {
        //                        ZamowienieOplacone(1, kontrahent.Zamowienie.IdZamowienia);
        //                        return 2;
        //                    }
        //                //}
        //                //else
        //                //{
        //                //    return 1;
        //                //    //Console.WriteLine("Zamowienie.FakturaKoncowaNumer: "+kontrahent.Zamowienie.FakturaKoncowaNumer);
        //                //}
        //
        //                return 0;
        //            }
        //            else
        //            {
        //                //Console.WriteLine("Kontrahent == null");
        //                return -3;
        //            }
        //        }

        /// <summary>
        /// Metoda wystawia fakturę sprzedaży przy zamówieniu, w którym sposób płatności został ustawiony na pobranie
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        /// <param name="nazwaDokumentu">Nazwa zamówienia na podstawie którego wystawiona zostanie faktura srzedaży</param>
        /// <param name="k">Kontrahent</param>
        /// <returns></returns>
        private static Dictionary<int, string> WystawFaktureKoncowaPrzyPobraniu(InsERT.Subiekt sgt, string nazwaDokumentu, Kontrahent k)
        {
            Console.WriteLine("WystawFaktureKoncowaPrzyPobraniu()");

            SuDokument fsDokument = sgt.Dokumenty.Dodaj(SubiektDokumentEnum.gtaSubiektDokumentFS);//gtaSubiektDokumentFSzalkonc);//gtaSubiektDokumentFSzalkonc);

            fsDokument.DataWystawienia = DateTime.Now.Date;//k.Zamowienie.DataWplaty.Date;
            fsDokument.KontrahentId = k.Id;
            Console.WriteLine("fsDokument.KontrahentId: " + fsDokument.KontrahentId);

            decimal kwota = decimal.Parse(k.Zamowienie.KwotaDoZaplaty.ToString());

            try
            {
                if (sgt.Dokumenty.Istnieje(nazwaDokumentu))
                {
                    Console.WriteLine("Istnieje");
                }

                if (sgt.Dokumenty.Istnieje(nazwaDokumentu))
                {
                    SuDokument oDok = sgt.SuDokumentyManager.Wczytaj(nazwaDokumentu);
                    fsDokument.NaPodstawie(oDok.Identyfikator);

                    Console.WriteLine("oDok istenieje: " + oDok.Identyfikator);
                }
                else
                {
                    Console.WriteLine("Dokument nie istnieje");
                }

                fsDokument.PlatnoscGotowkaKwota = (decimal)kwota;

                fsDokument.Zapisz();

                string uriPliku = string.Format(@"static\FakturaKoncowaNr{0}{1}", fsDokument.Identyfikator, ".pdf");
                //string sciezkaPlikuFK = HttpContext.Current.Server.MapPath("~") + uriPliku;
                //fsDokument.DrukujDoPlikuWgWzorca(1000002, sciezkaPlikuFK, TypPlikuEnum.gtaTypPlikuPDF);//fsDokument.DrukujDoPlikuWgWzorca(1000005, sciezkaPlikuFK, TypPlikuEnum.gtaTypPlikuPDF);
                uriPliku = uriPliku.Replace(@"\", @"/");

                string numerPelny = fsDokument.NumerPelny;
                int identyfikator = fsDokument.Identyfikator;
                Marshal.ReleaseComObject(fsDokument);
                Dictionary<int, string> resp = new Dictionary<int, string>();
                resp.Add(identyfikator, numerPelny);
                //var o = new { nr = numerPelny, id = identyfikator };
                return resp;//numerPelny;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                sgt.Zakoncz();
                string errorMsg = string.Format("Błąd związany ze wstawianiem płatności w Subiekcie.{0}Identyfikator zamówienia: {1}{2}{3}", Environment.NewLine, k.Zamowienie.IdZamowienia, Environment.NewLine, e.ToString());
                SendErrorEmail(errorMsg);//SendErrorEmail();
                return null;
            }
        }

        /// <summary>
        /// Metoda obsługująca mechanizm związany z wystawieniem faktury koncowej
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        /// <param name="id_zamowienia">Identyfikator zamówienia</param>
        /// <param name="czy_zagraniczna">Wartość true określa zamówienie zagraniczne, natomiast false krajowe</param>
        public static string FakturaKoncowaMechanizm(Subiekt sgt, int id_zamowienia, RodzajSklepu rodzaj_sklepu, int fak_id = 0)//bool czy_zagraniczna, int fak_id = 0)
        {
            Kontrahent k = Utils.PobierzKontrahenta(id_zamowienia);
            if (fak_id == 0)
            {
                fak_id = WystawFaktureZaliczkowaKoncowa(sgt, k.NumerZamowienia, rodzaj_sklepu, k, id_zamowienia);//czy_zagraniczna, k, id_zamowienia);
            }

            //ZapiszFaktureKoncowa(fak_id, id_zamowienia);
            if (k.Zamowienie.PaymentError == 1)
            {
                BlednaKwotaSave(0, k.Zamowienie.IdZamowienia);
            }

            List<Produkt> listaProduktow = PobierzListeProduktowZZamowienia(k.IdZakupu);
            List<Towar> towaryZzamowienia = PobierzWybraneTowaryZsubiekt(sgt, listaProduktow);

            var kontrahentSubiekt = sgt.Kontrahenci.Wczytaj(k.Id);
            int idkontrahentaSubiekt = kontrahentSubiekt.Identyfikator();

            string sciezkaPliku = WydanieZew(sgt, fak_id);//WydanieZewnetrzne(sgt, idkontrahentaSubiekt, towaryZzamowienia, k.Zamowienie, czy_zagraniczna);

            ZapiszSciezkeDokumentuWydZewDoBazyPortalGames(sciezkaPliku, k.Zamowienie.IdZamowienia);

            string uriPliku = string.Format(@"static/FakturaKoncowaNr{0}{1}", fak_id, ".pdf");
            ZapiszFaktureKoncowa(uriPliku, id_zamowienia);

            uriPliku = uriPliku.Replace(@"\", @"/");
            sciezkaPliku = sciezkaPliku.Replace(@"\", @"/");

            var resp = new
            {
                fkUri = uriPliku,
                wzUri = sciezkaPliku
            };

            return JsonConvert.SerializeObject(resp);//uriPliku;
        }

        /// <summary>
        /// Metoda zwraca liste zakupionych produktów w postaci listy ich id 
        /// </summary>
        /// <param name="idZamowienia">Id zamowienia</param>
        /// <returns></returns>
        public static List<Produkt> PobierzListeProduktowZZamowienia(string idZamowienia)
        {
            var listaProduktow = new List<Produkt>();

            var sqlCommand = "select * from order_item where order_id = " + idZamowienia + ";";
            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
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
                            Symbol = (reader["product_code"].ToString().ToUpper())
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
            Console.WriteLine("zamowienie.NumerZamowienia: " + zamowienie.NumerZamowienia);
            Console.WriteLine("zamowienie.IdZamowienia: " + zamowienie.IdZamowienia);

            //var sqlCommand = "UPDATE baza8706_devportalgames.`order` SET order_name = '" + zamowienie.NumerZamowienia +
            //                 "' WHERE id = " + zamowienie.IdZamowienia + ";";

            var sqlCommand =
                //"UPDATE baza8706_devportalgames.`order` SET order_name = @numer_zamowienia" + " WHERE id = @order_id";
                "UPDATE " + ConfigConnection.PortalGamesBazaProperty + ".`order` SET order_name = @numer_zamowienia" + " WHERE id = @order_id";

            MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
            using (var conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("order_id", zamowienie.IdZamowienia);
                    cmd.Parameters.AddWithValue("numer_zamowienia", zamowienie.NumerZamowienia);
                    cmd.CommandText = sqlCommand;

                    conn.Open();

                    cmd.ExecuteNonQuery();
                    //var reader = cmd.ExecuteReader();

                    //while (reader.Read())
                    //{
                    //}
                }
            }
        }

        /// <summary>
        /// Metoda łącząca kilka faktur w jeden plik pdf - dlugość działania zależna od ilości plików
        /// </summary>
        /// <param name="sgt">Instancja Subiekta</param>
        /// <param name="idZamowienia">Identyfikator zamówienia</param>
        /// <param name="faktura_koncowa_sciezka">Ścieżka faktury końcowej na serwerze</param>
        /// <returns></returns>
        public static string ScalFaktury(Subiekt sgt, int idZamowienia, string faktura_koncowa_sciezka)
        {
            List<string> sciezkiFaktur = new List<string>();
            var sqlCommand = "select faktura_nr from order_faktury_zaliczkowe where order_id = " + idZamowienia + ";";
            Kontrahent k = PobierzKontrahenta(idZamowienia);
            if (k.Zamowienie.WplataRodzaj != 3)
            {
                MySqlConnectionStringBuilder portalgamesconn = PortalGamesConnSring;
                using (var conn = new MySqlConnection(portalgamesconn.ToString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlCommand;

                        conn.Open();

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string faktura_nr = reader["faktura_nr"].ToString();

                            try
                            {
                                SuDokument fakturaZal = sgt.Dokumenty.Wczytaj(faktura_nr);
                                string uriPliku = string.Format(@"static\FakturaZaliczkowaNr{0}{1}", fakturaZal.Identyfikator, ".pdf");
                                //string sciezkaPliku = HttpContext.Current.Server.MapPath("~") + uriPliku;
                                //fakturaZal.DrukujDoPliku(sciezkaPliku, TypPlikuEnum.gtaTypPlikuPDF);
                                //sciezkiFaktur.Add(sciezkaPliku);
                            }
                            catch (Exception e)
                            {
                                sgt.Zakoncz();
                                return null;
                            }
                        }
                    }
                    conn.Close();
                }
            }
            sciezkiFaktur.Add(faktura_koncowa_sciezka);

            string uriPlikuZfakurami = string.Format(@"static\FakturyDoZamowieniaNr{0}{1}", idZamowienia, ".pdf");
            //string sciezkaPlikuZfakturami = HttpContext.Current.Server.MapPath("~") + uriPlikuZfakurami;

            //MergePDFs(sciezkaPlikuZfakturami, sciezkiFaktur);

            uriPlikuZfakurami = uriPlikuZfakurami.Replace(@"\", @"/");

            return uriPlikuZfakurami;//sciezkaPlikuZfakturami;
        }
    }
}