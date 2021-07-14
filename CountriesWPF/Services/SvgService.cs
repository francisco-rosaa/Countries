using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using CountriesWPF.Models;
using SharpVectors.Converters;

namespace CountriesWPF.Services
{
    public class SvgService
    {
        Configuration config = new Configuration();

        /// <summary>
        /// Converts web SVG images to DrawingImages keeping theyr geometric properties
        /// Adds individual image to each Country object
        /// </summary>
        /// <param name="countries">List of countries</param>
        /// <param name="progress">Progress report of the conversion with SvgProgressReport object</param>
        /// <returns>List of countries with images added</returns>
        public async Task<List<Country>> ConvertWebSvgAsync(List<Country> countries, IProgress<SvgProgressReport> progress)
        {
            List<Country> countriesTemp = countries;

            SvgProgressReport svgProgressReport = new SvgProgressReport();
            int totalCountries = countriesTemp.Count;
            string countryBeingDone = string.Empty;
            int countriesConverted = 0;

            await Task.Run(() =>
            {
                foreach (Country country in countriesTemp)
                {
                    countryBeingDone = country.Name;

                    if (!string.IsNullOrEmpty(country.FlagUrl))
                    {
                        DrawingImage flagImage = new SvgImageExtension(country.FlagUrl).ProvideValue(null) as DrawingImage;

                        flagImage.Freeze();
                        country.FlagImage = flagImage;
                    }
                    else
                    {
                        DrawingImage flagImage = new SvgImageExtension(config.LocalPathToNAFlag).ProvideValue(null) as DrawingImage;

                        flagImage.Freeze();
                        country.FlagImage = flagImage;
                    }

                    countriesConverted += 1; 

                    svgProgressReport.CountryBeingDone = countryBeingDone;
                    svgProgressReport.CountriesDone = countriesConverted;
                    svgProgressReport.PercentageDone = Convert.ToInt32((countriesConverted * 100) / totalCountries);
                    progress.Report(svgProgressReport);
                }
            });

            return countriesTemp;
        }

        /// <summary>
        /// Converts local SVG image to DrawingImage keeping its geometric properties
        /// Adds N/A image to each Country object
        /// </summary>
        /// <param name="countries">List of countries</param>
        /// <param name="progress">Progress report with SvgProgressReport object</param>
        /// <returns>List of countries with images added</returns>
        public async Task<List<Country>> ConvertLocalSvgAsync(List<Country> countries, IProgress<SvgProgressReport> progress)
        {
            List<Country> countriesTemp = countries;

            SvgProgressReport svgProgressReport = new SvgProgressReport();
            int totalCountries = countriesTemp.Count;
            string countryBeingDone = string.Empty;
            int countriesConverted = 0;
            
            DrawingImage flagImage = new SvgImageExtension(config.LocalPathToNAFlag).ProvideValue(null) as DrawingImage;
            flagImage.Freeze();

            await Task.Run(() =>
            {
                foreach (Country country in countriesTemp)
                {
                    countryBeingDone = country.Name;

                    country.FlagImage = flagImage;

                    countriesConverted += 1;

                    svgProgressReport.CountryBeingDone = countryBeingDone;
                    svgProgressReport.CountriesDone = countriesConverted;
                    svgProgressReport.PercentageDone = Convert.ToInt32((countriesConverted * 100) / totalCountries);
                    progress.Report(svgProgressReport);
                }
            });

            return countriesTemp;
        }
    }
}
