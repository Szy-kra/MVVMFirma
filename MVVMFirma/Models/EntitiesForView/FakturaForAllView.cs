using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMFirma.Models.EntitiesForView
{
    //ta klasa jest ....
    public class FakturaForAllView
    {
        public string Numer { get; set; }
        public DateTime? DataWystawienia { get; set; }
        public string  NazwaFirmy { get; set; }//nazwa i nip jest zamiast klucza obcego idKontrahenta, zeby bylo czytelne dla uzytkownika
        public string NipFirmy { get; set; }
        public DateTime? TerminPlatnosci { get; set; }
        public string NazwaSposobuPlatnosci { get; set; } //to pole jest zamiast klucza obcego idSposobuPlatnosci
    }
}
