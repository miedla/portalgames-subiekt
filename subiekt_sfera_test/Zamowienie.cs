using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subiekt_sfera_test
{
    public class Zamowienie
    {
        public List<int> Produkty { get; set; }

        public string NumerZamowienia { get; set; }

        public string IdZamowienia { get; set; }
        public string WplaconaKwota { get; set; }
        public string KwotaDoZaplaty { get; set; }
    }
}