using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subiekt_sfera_test
{
    /// <summary>
    /// Statyczna klasa zawierająca pola dostępowe do bazy Subiekta, Portalgames sklep oraz Portalgames shop
    /// </summary>
    public class ConfigConnection
    {
        public static string ServerGt = "(local)\\INSERTGT";
        public static string BazaGt = "test";//"test";//test4 dla localhost
        public static string OperatorGt = "Szef";
        public static string OperatorGThaslo = "";
        public static string Uzytkownik = "sa";//"sa"; 
        public static string UzytkownikHaslo = "en2HgB2WcqvLpkwY";//"";

        public static string PortalGamesServer = "8706.m.tld.pl";
        public static string PortalGamesBaza = "baza8706_devportalgames";//"baza8706_11";
        public static string PortalGamesUser = "admin8706_devportalgames";//"admin8706_11";
        public static string PortalGamesPassword = "qazwsxM13";//"0NgFs9%Mg6";

        public static string PortalGamesShopServer = "8706.m.tld.pl";
        public static string PortalGamesShopBaza = "baza8706_baza_dev_shop_17";
        public static string PortalGamesShopUser = "admin8706_baza_dev_shop_17";
        public static string PortalGamesShopPassword = "5KeL91G9q8";

        public static string PortalGamesGeshaftServer = "8706.m.tld.pl";
        public static string PortalGamesGeshaftBaza = "baza8706_baza_dev_shop_17";
        public static string PortalGamesGeshaftUser = "admin8706_baza_dev_shop_17";
        public static string PortalGamesGeshaftPassword = "5KeL91G9q8";

        public static string PortalGamesHurtServer = "8706.m.tld.pl";
        public static string PortalGamesHurtBaza = "baza8706_13";
        public static string PortalGamesHurtUser = "admin8706_13";
        public static string PortalGamesHurtPassword = "8RlFE5kn09";

        //public static string ErrorEmail1 = "maciej.dadela@ampmedia.pl";
        //public static string ErrorEmail2 = "ksiegowosc@portalgames.pl";
        public static List<string> ErrorEmailsList = new List<string> {
            "karol.tytko@ampmedia.pl"
        };

        public static string ServerGtProperty
        {
            get { return ServerGt; }
            set { ServerGt = value; }
        }
        public static string BazaGtProperty
        {
            get { return BazaGt; }
            set { BazaGt = value; }
        }
        public static string OperatorGtProperty
        {
            get { return OperatorGt; }
            set { OperatorGt = value; }
        }
        public static string OperatorGtHasloProperty
        {
            get { return OperatorGThaslo; }
            set { OperatorGThaslo = value; }
        }
        public static string UzytkownikProperty
        {
            get { return Uzytkownik; }
            set { Uzytkownik = value; }
        }
        public static string UzytkownikHasloProperty
        {
            get { return UzytkownikHaslo; }
            set { UzytkownikHaslo = value; }
        }
        public static string PortalGamesServerProperty
        {
            get { return PortalGamesServer; }
            set { PortalGamesServer = value; }
        }
        public static string PortalGamesBazaProperty
        {
            get { return PortalGamesBaza; }
            set { PortalGamesBaza = value; }
        }
        public static string PortalGamesUserProperty
        {
            get { return PortalGamesUser; }
            set { PortalGamesUser = value; }
        }
        public static string PortalGamesPasswordProperty
        {
            get { return PortalGamesPassword; }
            set { PortalGamesPassword = value; }
        }

        public static string PortalGamesShopServerProperty
        {
            get { return PortalGamesShopServer; }
            set { PortalGamesShopServer = value; }
        }
        public static string PortalGamesShopBazaProperty
        {
            get { return PortalGamesShopBaza; }
            set { PortalGamesShopBaza = value; }
        }
        public static string PortalGamesShopUserProperty
        {
            get { return PortalGamesShopUser; }
            set { PortalGamesShopUser = value; }
        }
        public static string PortalGamesShopPasswordProperty
        {
            get { return PortalGamesShopPassword; }
            set { PortalGamesShopPassword = value; }
        }

        public static string PortalGamesGeshaftServerProperty
        {
            get { return PortalGamesGeshaftServer; }
            set { PortalGamesGeshaftServer = value; }
        }
        public static string PortalGamesGeshaftBazaProperty
        {
            get { return PortalGamesGeshaftBaza; }
            set { PortalGamesGeshaftBaza = value; }
        }
        public static string PortalGamesGeshaftUserProperty
        {
            get { return PortalGamesGeshaftUser; }
            set { PortalGamesGeshaftUser = value; }
        }
        public static string PortalGamesGeshaftPasswordProperty
        {
            get { return PortalGamesGeshaftPassword; }
            set { PortalGamesGeshaftPassword = value; }
        }

        public static string PortalGamesHurtServerProperty
        {
            get { return PortalGamesHurtServer; }
            set { PortalGamesHurtServer = value; }
        }
        public static string PortalGamesHurtBazaProperty
        {
            get { return PortalGamesHurtBaza; }
            set { PortalGamesHurtBaza = value; }
        }
        public static string PortalGamesHurtUserProperty
        {
            get { return PortalGamesHurtUser; }
            set { PortalGamesHurtUser = value; }
        }
        public static string PortalGamesHurtPasswordProperty
        {
            get { return PortalGamesHurtPassword; }
            set { PortalGamesHurtPassword = value; }
        }
    }
}
