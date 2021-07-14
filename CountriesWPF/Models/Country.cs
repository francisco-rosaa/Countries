using System.Windows.Media;

namespace CountriesWPF.Models
{
    public class Country
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Capital { get; set; }

        public string Region { get; set; }

        public string SubRegion { get; set; }

        public int Population { get; set; }

        public int Area { get; set; }

        public double Gini { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string FlagUrl { get; set; }

        public DrawingImage FlagImage { get; set; }

        public string NameAndCode
        {
            get
            {
                return $"{Name}  ({Code})";
            }
        }
    }
}
