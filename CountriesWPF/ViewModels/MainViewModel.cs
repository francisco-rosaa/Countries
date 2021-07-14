using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CountriesWPF.Models;
using CountriesWPF.Services;

namespace CountriesWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private NetworkService networkService;
        private ApiService apiService;
        private DataService dataService;
        private Response response;
        private List<Country> countries;
        private Country selectedCountry;
        private ExtraInfo extraInfo;
        private Configuration config;

        private string _message;
        private string _status;

        public List<Country> Countries
        {
            get
            {
                return countries;
            }
            set
            {
                countries = value;
                OnPropertyChanged("Countries");
            }
        }

        public Country SelectedCountry
        {
            get
            {
                return selectedCountry;
            }
            set
            {
                selectedCountry = value;
                OnPropertyChanged("SelectedCountry");
            }
        }

        public ExtraInfo ExtraInformation
        {
            get
            {
                return extraInfo;
            }
            set
            {
                extraInfo = value;
                OnPropertyChanged("ExtraInformation");
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifes that a property has changed
        /// </summary>
        /// <param name="propertyName">Name of the prperty that changed</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Constructor that instantiates working objects and starts countries loading
        /// </summary>
        /// <param name="progress">IProgress interface</param>
        public MainViewModel(IProgress<SvgProgressReport> progress)
        {
            networkService = new NetworkService();
            apiService = new ApiService();
            dataService = new DataService();
            response = new Response();
            countries = new List<Country>();
            selectedCountry = new Country();
            extraInfo = new ExtraInfo();
            config = new Configuration();

            LoadCountriesAsync(progress);
        }

        /// <summary>
        /// Starts countries loading, from the web Api or locally depending on reponses
        /// </summary>
        /// <param name="progress">IProgress interface</param>
        private async void LoadCountriesAsync(IProgress<SvgProgressReport> progress)
        {
            bool webLoad;

            Message = "Loading Countries...";

            Response connection = networkService.CheckConnection();

            if (connection.IsSuccess)
            {
                await LoadCountriesApiAsync(progress);
                webLoad = true;

                if (response.IsSuccess == false)
                {
                    await LoadCountriesLocalAsync(progress);
                    webLoad = false;
                }
            }
            else
            {
                await LoadCountriesLocalAsync(progress);
                webLoad = false;
            }

            if (Countries.Count == 0)
            {
                Message = "No Internet or local data";
                Status = "First start";
                return;
            }

            MessagesPresentation(webLoad);
        }

        /// <summary>
        /// Loads countries list via web Api
        /// Saves countries list locally
        /// </summary>
        /// <param name="progress">IProgress interface</param>
        /// <returns>Task</returns>
        private async Task LoadCountriesApiAsync(IProgress<SvgProgressReport> progress)
        {
            response = await apiService.GetApiDataAsync("http://restcountries.eu", "/rest/v2/all", JsonType.Countries, progress);

            if (response.IsSuccess)
            {
                Countries = response.Result as List<Country>;

                SaveCountriesLocal();
            }
        }

        /// <summary>
        /// Loads extra info via web Api
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <returns>Task</returns>
        public async Task LoadExtraInfoApiAsync(string countryCode)
        {
            response = await apiService.GetApiDataAsync("http://countriesextrainfo.somee.com", $"/api/countries/{countryCode}", JsonType.ExtraInfo);

            if (response.IsSuccess)
            {
                ExtraInformation = response.Result as ExtraInfo;
            }
            else
            {
                ExtraInformation.MedianAge = 0;
                ExtraInformation.FertilityRate = 0;
                ExtraInformation.AverageSalary = 0;
            }
        }

        /// <summary>
        /// Loads countries list locally
        /// </summary>
        /// <param name="progress">IProgress interface</param>
        /// <returns>Task</returns>
        private async Task LoadCountriesLocalAsync(IProgress<SvgProgressReport> progress)
        {
            Countries = await dataService.GetDataAsync(progress);
        }

        /// <summary>
        /// Saves countries list locally if the DB file is empty 
        /// or if last file change was more than the configured MaximumDataFileAgeDays
        /// </summary>
        private void SaveCountriesLocal()
        {
            if (dataService.IsDataFileEmpty())
            {
                dataService.SaveData(Countries);
            }

            if (dataService.GetDataFileAgeDays() > config.MaximumDataFileAgeDays)
            {
                dataService.DeleteData();
                dataService.SaveData(Countries);
            }
        }

        /// <summary>
        /// Presents information about the loaded data
        /// </summary>
        /// <param name="webLoad">Boolean that represents if it's web or local data</param>
        private void MessagesPresentation(bool webLoad)
        {
            Message = "Countries Loaded";

            if (webLoad)
            {
                Status = $"Web data ({DateTime.Today.ToString("dd/MM/yyyy")})";
            }
            else
            {
                Status = "Local data";
            }
        }
    }
}
