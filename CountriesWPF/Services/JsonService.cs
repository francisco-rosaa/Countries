using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CountriesWPF.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CountriesWPF.Services
{
    public class JsonService
    {
        /// <summary>
        /// Converts JSON string to list of countries
        /// Checks for empty values and puts 0 or empty string where that happens
        /// </summary>
        /// <param name="result">JSON string</param>
        /// <param name="progress">IProgress interface</param>
        /// <returns>List of countries</returns>
        public async Task<List<Country>> DeserializeCountriesAsync(string result, IProgress<SvgProgressReport> progress)
        {
            List<Country> countries = new List<Country>();

            JToken tokens = JToken.Parse(result);

            await Task.Run(() => 
            {
                for (int i = 0; i < tokens.Count(); i++)
                {
                    string code = string.Empty;
                    string name = string.Empty;
                    string capital = string.Empty;
                    string region = string.Empty;
                    string subRegion = string.Empty;
                    int population;
                    int area;
                    double gini;
                    double latitude;
                    double longitude;
                    string flagUrl = string.Empty;

                    if (!string.IsNullOrEmpty(tokens.SelectToken($"[{i}].alpha3Code").ToString()))
                    {
                        code = tokens.SelectToken($"[{i}].alpha3Code").ToString();
                    }

                    if (!string.IsNullOrEmpty(tokens.SelectToken($"[{i}].name").ToString()))
                    {
                        name = tokens.SelectToken($"[{i}].name").ToString();
                    }

                    if (!string.IsNullOrEmpty(tokens.SelectToken($"[{i}].capital").ToString()))
                    {
                        capital = tokens.SelectToken($"[{i}].capital").ToString();
                    }

                    if (!string.IsNullOrEmpty(tokens.SelectToken($"[{i}].region").ToString()))
                    {
                        region = tokens.SelectToken($"[{i}].region").ToString();
                    }

                    if (!string.IsNullOrEmpty(tokens.SelectToken($"[{i}].subregion").ToString()))
                    {
                        subRegion = tokens.SelectToken($"[{i}].subregion").ToString();
                    }

                    if (!int.TryParse(tokens.SelectToken($"[{i}].population").ToString(), out population))
                    {
                        population = 0;
                    }

                    if (!int.TryParse(tokens.SelectToken($"[{i}].area").ToString(), out area))
                    {
                        area = 0;
                    }

                    if (!double.TryParse(tokens.SelectToken($"[{i}].gini").ToString(), out gini))
                    {
                        gini = 0;
                    }

                    if (!string.IsNullOrEmpty(tokens.SelectToken($"[{i}].latlng").ToString()))
                    {
                        latitude = Convert.ToDouble(tokens.SelectToken($"[{i}].latlng[0]"));

                        longitude = Convert.ToDouble(tokens.SelectToken($"[{i}].latlng[1]"));
                    }
                    else
                    {
                        latitude = 0;
                        longitude = 0;
                    }

                    if (!string.IsNullOrEmpty(tokens.SelectToken($"[{i}].flag").ToString()))
                    {
                        flagUrl = tokens.SelectToken($"[{i}].flag").ToString();
                    }

                    Country country = new Country
                    {
                        Code = code,
                        Name = name,
                        Capital = capital,
                        Region = region,
                        SubRegion = subRegion,
                        Population = population,
                        Area = area,
                        Gini = gini,
                        Latitude = latitude,
                        Longitude = longitude,
                        FlagUrl = flagUrl
                    };

                    countries.Add(country);
                }
            });

            countries = await Task.Run(() => countries.OrderBy(x => x.Name).ToList());

            SvgService svgService = new SvgService();

            countries = await Task.Run(() => svgService.ConvertWebSvgAsync(countries, progress));

            return countries;
        }

        /// <summary>
        /// Converts JSON string to list of extra info
        /// </summary>
        /// <param name="result">JSON string</param>
        /// <returns>List of extra info</returns>
        public async Task<ExtraInfo> DeserializeExtraInfoAsync(string result)
        {
            ExtraInfo extraInfo = null;

            await Task.Run(() =>
            {
                extraInfo = JsonConvert.DeserializeObject<ExtraInfo>(result);
            });

            return extraInfo;
        }
    }
}
