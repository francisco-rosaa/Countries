using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CountriesWPF.Models;

namespace CountriesWPF.Services
{
    public class DataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;

        /// <summary>
        /// Creates directory and SQLite DB file if it doesn't exist
        /// </summary>
        public DataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("DataSource=" + path);

                string sqlCreate =
                    "create table if not exists Countries " +
                    "(Code text, Name text, Capital text, Region text, SubRegion text, Population int, Area int, Gini real, Latitude real, Longitude real, FlagUrl text)";

                connection.Open();
                command = new SQLiteCommand(sqlCreate, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Error", ex.Message);
            }
        }

        /// <summary>
        /// Checks if DB file is empty
        /// </summary>
        /// <returns>Boolean with true of false</returns>
        public bool IsDataFileEmpty()
        {
            string testString = string.Empty;

            try
            {
                string sqlSelect = "select Code from Countries Limit 1";

                connection.Open();

                command = new SQLiteCommand(sqlSelect, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    testString = reader["Code"].ToString();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Error", ex.Message);
            }

            if (string.IsNullOrEmpty(testString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets DB file number of days since last writing
        /// </summary>
        /// <returns>Number of days</returns>
        public int GetDataFileAgeDays()
        {
            int fileAgeDays = 0;

            string file = @"Data\Countries.sqlite";

            if (File.Exists(file))
            {
                DateTime creationDate = File.GetLastWriteTime(file);

                fileAgeDays = (DateTime.Today.Date - creationDate.Date).Days;

            }

            return fileAgeDays;
        }

        /// <summary>
        /// Saves countries list to DB file
        /// </summary>
        /// <param name="countries">List of countries</param>
        public void SaveData(List<Country> countries)
        {
            try
            {
                connection.Open();

                foreach (Country country in countries)
                {
                    string sqlInsert = string.Format
                        (
                        "insert into Countries (Code, Name, Capital, Region, SubRegion, Population, Area, Gini, Latitude, Longitude, FlagUrl) " +
                        "values ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, {6}, '{7}', '{8}', '{9}', '{10}')",
                        country.Code,
                        country.Name.Replace("'", "''"),
                        country.Capital.Replace("'", "''"),
                        country.Region.Replace("'", "''"),
                        country.SubRegion.Replace("'", "''"),
                        country.Population,
                        country.Area,
                        country.Gini.ToString(CultureInfo.InvariantCulture.NumberFormat),
                        country.Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat),
                        country.Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat),
                        country.FlagUrl
                        );

                    command = new SQLiteCommand(sqlInsert, connection);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Error", ex.Message);
            }
        }

        /// <summary>
        /// Loads countries from DB file
        /// </summary>
        /// <param name="progress">IProgress interface</param>
        /// <returns>List of countries</returns>
        public async Task<List<Country>> GetDataAsync(IProgress<SvgProgressReport> progress)
        {
            List<Country> countries = new List<Country>();

            try
            {
                string sqlSelect =
                    "select Code, Name, Capital, Region, SubRegion, Population, Area, Gini, Latitude, Longitude, FlagUrl " +
                    "from Countries";

                connection.Open();

                command = new SQLiteCommand(sqlSelect, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    countries.Add(new Country
                    {
                        Code = reader["Code"].ToString(),
                        Name = reader["Name"].ToString(),
                        Capital = reader["Capital"].ToString(),
                        Region = reader["Region"].ToString(),
                        SubRegion = reader["SubRegion"].ToString(),
                        Population = Convert.ToInt32(reader["Population"]),
                        Area = Convert.ToInt32(reader["Area"]),
                        Gini = Convert.ToDouble(reader["Gini"]),
                        Latitude = Convert.ToDouble(reader["Latitude"]),
                        Longitude = Convert.ToDouble(reader["Longitude"]),
                        FlagUrl = reader["FlagUrl"].ToString()
                    });
                }

                connection.Close();

                SvgService svgService = new SvgService();

                countries = await Task.Run(() => svgService.ConvertLocalSvgAsync(countries, progress));

                return countries;
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Error", ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Deletes content from DB file
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sqlDelete =
                    "delete from Countries";

                connection.Open();
                command = new SQLiteCommand(sqlDelete, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Error", ex.Message);
            }
        }
    }
}
