using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CountriesWPF.Models;

namespace CountriesWPF.Services
{
    public class ApiService
    {
        /// <summary>
        /// Gets JSON data from the inputted Api
        /// </summary>
        /// <param name="baseUrl">Api base url</param>
        /// <param name="controller">Api controller addicional url</param>
        /// <param name="jsonType">Json type retuned by the Api</param>
        /// <param name="progress">IProgress interface</param>
        /// <returns></returns>
        public async Task<Response> GetApiDataAsync(string baseUrl, string controller, JsonType jsonType, IProgress<SvgProgressReport> progress = null)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                if (jsonType == JsonType.Countries)
                {
                    JsonService jsonService = new JsonService();

                    List<Country> countries = await jsonService.DeserializeCountriesAsync(result, progress);

                    return new Response
                    {
                        IsSuccess = true,
                        Result = countries
                    };
                }

                if (jsonType == JsonType.ExtraInfo)
                {
                    JsonService jsonService = new JsonService();

                    ExtraInfo extraInfo = await jsonService.DeserializeExtraInfoAsync(result);

                    return new Response
                    {
                        IsSuccess = true,
                        Result = extraInfo
                    };
                }

                return new Response
                {
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
