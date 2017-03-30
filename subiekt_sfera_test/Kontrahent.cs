using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subiekt_sfera_test
{
    public class Kontrahent 
    {
        public string IdZakupu { get; set; }

        public string Id { get; set; }

        public string Ulica { get; set; }

        public string Miasto { get; set; }

        public string Zip { get; set; }

        public string PanstwoKod { get; set; }

        public string Nazwa { get; set; }

        public string Imie { get; set; }

        public string Nazwisko { get; set; }

        public string Fima { get; set; }
            
        public string MiastoKod { get; set; }

        public string NumerZamowienia { get; set; }

        public Zamowienie Zamowienie { get; set; }

        public Kontrahent()
        {
            Zamowienie = new Zamowienie();
        }
    }
}
