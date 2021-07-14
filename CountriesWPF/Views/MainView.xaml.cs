using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Toolkit.Wpf.UI.Controls;
using Windows.Devices.Geolocation;
using CountriesWPF.Models;
using CountriesWPF.ViewModels;

namespace CountriesWPF.Views
{
    public partial class MainView : Window
    {
        MainViewModel mainViewModel;
        MapControl mapControl;

        /// <summary>
        /// Contructor that instantiates working objects:
        /// - Progress for IProgress interface reporting
        /// - MainViewModel to get the data and establish the data context
        /// - MapControl to load and use Bing Maps
        /// </summary>
        public MainView()
        {
            Progress<SvgProgressReport> progress = new Progress<SvgProgressReport>();
            progress.ProgressChanged += ReportProgress;

            mainViewModel = new MainViewModel(progress);
            mapControl = new MapControl();

            InitializeComponent();
            DataContext = mainViewModel;

            mapControl.MapServiceToken = App.Token;
            gridMap.Children.Add(mapControl);
            mapControl.Loaded += MapControl_Loaded;
        }

        /// <summary>
        /// Reports progress sent via event handler
        /// </summary>
        private void ReportProgress(object sender, SvgProgressReport e)
        {
            progressBar.Visibility = Visibility.Visible;

            progressBar.Value = e.PercentageDone;
            mainViewModel.Status = $"{e.CountriesDone} | {e.CountryBeingDone}";

            if (e.PercentageDone == 100)
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Starts map at given coordinates
        /// </summary>
        private async void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            BasicGeoposition geoPosition = new BasicGeoposition() { Latitude = 39.5, Longitude = -8 };

            var center = new Geopoint(geoPosition);

            await (sender as MapControl).TrySetViewAsync(center, 6.2);
        }

        /// <summary>
        /// Presents info and map location of a country When it's selected in the combobox
        /// </summary>
        private async void comboBoxCountries_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxCountries.SelectedValue != null)
            {
                string countryCode = comboBoxCountries.SelectedValue.ToString();

                mainViewModel.SelectedCountry = await GetSelectedCountry(countryCode);

                PresentCountryInfoAsync(mainViewModel.SelectedCountry);
                await PresentExtraInfoAsync(mainViewModel.SelectedCountry.Code);
                await MapGoToGeoLocationAsync(mainViewModel.SelectedCountry.Latitude, mainViewModel.SelectedCountry.Longitude);
            }
        }

        /// <summary>
        /// Generates country object with code string
        /// </summary>
        /// <param name="countryCode">Country code string</param>
        /// <returns>Country object</returns>
        private async Task<Country> GetSelectedCountry(string countryCode)
        {
            Country selectedCountry = new Country();

            await Task.Run(() =>
            {
                foreach (Country country in mainViewModel.Countries)
                {
                    if (countryCode == country.Code)
                    {
                        selectedCountry = country;
                    }
                }
            });

            return selectedCountry;
        }

        /// <summary>
        /// Presents information about a country
        /// </summary>
        /// <param name="selectedCountry">Country object</param>
        private void PresentCountryInfoAsync(Country selectedCountry)
        {
            textBlockDetailsId.Text = "Capital" + Environment.NewLine;
            textBlockDetailsId.Text += "Region" + Environment.NewLine;
            textBlockDetailsId.Text += "SubRegion" + Environment.NewLine;
            textBlockDetailsId.Text += "Population" + Environment.NewLine;
            textBlockDetailsId.Text += "Area (km2)" + Environment.NewLine;
            textBlockDetailsId.Text += "Gini" + Environment.NewLine;

            textBlockDetails.Text = IsStringEmpty(selectedCountry.Capital) + Environment.NewLine;
            textBlockDetails.Text += IsStringEmpty(selectedCountry.Region) + Environment.NewLine;
            textBlockDetails.Text += IsStringEmpty(selectedCountry.SubRegion) + Environment.NewLine;
            textBlockDetails.Text += IsStringEmpty(selectedCountry.Population.ToString()) + Environment.NewLine;
            textBlockDetails.Text += IsStringEmpty(selectedCountry.Area.ToString()) + Environment.NewLine;
            textBlockDetails.Text += IsStringEmpty(selectedCountry.Gini.ToString()) + Environment.NewLine;
        }

        /// <summary>
        /// Gets and presents extra information about a country
        /// </summary>
        /// <param name="countryCode">Country code string</param>
        /// <returns>Task</returns>
        private async Task PresentExtraInfoAsync(string countryCode)
        {
            await mainViewModel.LoadExtraInfoApiAsync(countryCode);

            textBlockExtraMedianAgeTop.Text = "Median Age";
            textBlockExtraMedianAgeMid.Text = IsStringEmpty(mainViewModel.ExtraInformation.MedianAge.ToString());
            textBlockExtraMedianAgeBot.Text = "Years";

            textBlockExtraFertilityRateTop.Text = "Fertility Rate";
            textBlockExtraFertilityRateMid.Text = IsStringEmpty(mainViewModel.ExtraInformation.FertilityRate.ToString());
            textBlockExtraFertilityRateBot.Text = "Births";

            textBlockExtraAverageSalaryTop.Text = "Average Salary";
            textBlockExtraAverageSalaryMid.Text = IsStringEmpty(mainViewModel.ExtraInformation.AverageSalary.ToString());
            textBlockExtraAverageSalaryBot.Text = "Dollars";
        }

        /// <summary>
        /// Detects if string is 0 or empty and substites those for N/A
        /// </summary>
        /// <param name="testString">String to test</param>
        /// <returns>Same input string or N/A string</returns>
        private string IsStringEmpty(string testString)
        {
            if (string.IsNullOrEmpty(testString) || testString == "0")
            {
                return "N/A";
            }

            return testString;
        }

        /// <summary>
        /// Positions map at given coordinates
        /// </summary>
        /// <param name="latitude">latitude number</param>
        /// <param name="longitude">Longitude Number</param>
        /// <returns>Task</returns>
        private async Task MapGoToGeoLocationAsync(double latitude, double longitude)
        {
            BasicGeoposition geoPosition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };

            var center = new Geopoint(geoPosition);

            await mapControl.TrySetViewAsync(center);
        }
    }
}
