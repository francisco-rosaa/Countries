namespace CountriesWPF.Models
{
    public class Configuration
    {
        public int MaximumDataFileAgeDays { get; set; } = 7;

        public string LocalPathToNAFlag { get; set; } = "../../../../Resources/NAFlag.svg";
    }
}
