namespace MVVMFirma.Models.Shared
{
    public class DisplayBom
    {
        public string Material { get; private set; }
        public decimal Ilosc { get; private set; }
        public string Jednostka { get; private set; }
        public decimal Udzial { get; private set; }

        public DisplayBom(string material, decimal ilosc, string jednostka, decimal udzial)
        {
            this.Material = material;
            this.Ilosc = ilosc;
            this.Jednostka = jednostka;
            this.Udzial = udzial;
        }
    }
}