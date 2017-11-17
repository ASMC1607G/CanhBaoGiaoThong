using BingMap.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BingMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        WarningTrafficServiceClient client = new WarningTrafficServiceClient();
        public static User userLogin = null;

        public Login()
        {
            this.InitializeComponent();
        }

        private async void Signin_Click(object sender, RoutedEventArgs e)
        {
            var mes = await client.LoginAsync(EmailTextBox.Text, PassWordPassBox.Password);
            if (mes != null)
            {
                //login thanh cong
                userLogin = mes;
                EmailTextBox.Text = "";
                PassWordPassBox.Password = "";
                this.Frame.Navigate(typeof(MainPage));
                LoadWarning();
            }
            else {
                MessageDialog msgDialog = new MessageDialog("EMAIL or PASSWORD incorrect", "Message");
                await msgDialog.ShowAsync();
                EmailTextBox.Text = "";
                PassWordPassBox.Password = "";
            }
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }

        private void Aboutme_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        public async static void LoadWarning() {
            WarningTrafficServiceClient client = new WarningTrafficServiceClient();
            MainPage.PushpinCollection.Clear();

            var listWarning = await client.GetAllWarningAsync();
            foreach (var w in listWarning)
            {
                MainPage.PushpinCollection.Add(new PushpinModel() { TextPushpin = "W", KeyPushPin = w.idWarning.ToString(), Latitude = (double)w.lat, Longitude = (double)w.lon });
            }
        }
    }
}
