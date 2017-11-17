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
    public sealed partial class RegisterPage : Page
    {
        WarningTrafficServiceClient client = new WarningTrafficServiceClient();
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text != "" && PassWordPassBox.Password != "" && ConfirmPassWordPassBox.Password != "" && NameBox.Text != "" && AddressBox.Text != "" && PhoneBox.Text != "")
            {
                var rs = await client.CheckEmailAsync(EmailTextBox.Text);
                if (rs)
                {
                    if (PassWordPassBox.Password == ConfirmPassWordPassBox.Password)
                    {
                        //dang ky
                       await client.RegisterAsync(new User() { email=EmailTextBox.Text,name=NameBox.Text,address=AddressBox.Text,password=PassWordPassBox.Password,phone=PhoneBox.Text});

                        MessageDialog msgDialog = new MessageDialog("Registration success", "Message");
                        await msgDialog.ShowAsync();

                        this.Frame.Navigate(typeof(Login));
                    }
                    else {
                        MessageDialog msgDialog = new MessageDialog("Confirm Password incorrect", "Message");
                        await msgDialog.ShowAsync();
                    }
                }
                else {
                    MessageDialog msgDialog = new MessageDialog("Email is ready", "Message");
                    await msgDialog.ShowAsync();
                }
            }
            else {
                MessageDialog msgDialog = new MessageDialog("Please fill all Textbox", "Message");
                await msgDialog.ShowAsync();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Login));
        }
    }
}
