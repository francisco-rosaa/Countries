using System.Windows;

namespace CountriesWPF.Services
{
    public class DialogService
    {
        /// <summary>
        /// Shows received message in MessageBox
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="message">Content of the message</param>
        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }
    }
}
