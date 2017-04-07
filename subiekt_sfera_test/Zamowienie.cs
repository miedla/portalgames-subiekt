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
        public string IdZamowienia { get; set; }
        public string WplaconaKwota { get; set; }
        public string WplaconaKwotaSum { get; set; }
        public string KwotaDoZaplaty { get; set; }
        public string WartoscProduktu { get; set; }
        public string DostawaKwota { get; set; }
        public string DostawaRodzaj { get; set; }
        public string WplataRodzaj { get; set; }
    }
}