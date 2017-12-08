using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsERT;

namespace subiekt_sfera_test
{
    public class Zamowienie
    {
        public List<int> Produkty { get; set; }

        public string NumerZamowienia { get; set; }
        public string IloscWplat { get; set; }
        public int IdZamowienia { get; set; }
        public string WplaconaKwota { get; set; }
        public string WplaconaKwotaSum { get; set; }
        public double KwotaDoZaplaty { get; set; }
        public string WartoscProduktu { get; set; }
        public string DostawaKwota { get; set; }
        public string DostawaRodzaj { get; set; }
        public int? DostawaRodzajId { get; set; }
        public int WplataRodzaj { get; set; }
        public string FakturaKoncowaNumer { get; set; }
        public int PaymentError { get; set; }
        public string Currency { get; set; }
        public DateTime DataWplaty { get; set; }
    }
}