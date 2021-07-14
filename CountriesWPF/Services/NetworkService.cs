﻿using System.Net;
using CountriesWPF.Models;

namespace CountriesWPF.Services
{
    public class NetworkService
    {
        /// <summary>
        /// Checks if there is internet connnection
        /// </summary>
        /// <returns>Response with boolean IsSuccess</returns>
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return new Response
                    {
                        IsSuccess = true
                    };
                }
            }
            catch
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "No Internet connection."
                };
            }
        }
    }
}
