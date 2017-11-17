using BingMap.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class LawPage : Page
    {
        WarningTrafficServiceClient client = new WarningTrafficServiceClient();
        public ObservableCollection<Law> listAllLaw = new ObservableCollection<Law>();
        public static Law lawDetail = new Law();
        

        public LawPage()
        {
            this.InitializeComponent();
            if (MainPage.LawSearch != "")
            {
                LoadLaw(MainPage.LawSearch);
            }
            else {
                LoadLaw();
            }
            //LawListView.ItemsSource = listAllLaw;
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            LawListView.ItemsSource = await client.SearchLawByNameAsync(sender.Text);
        }

        private async void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchBox.ItemsSource = await client.SuggestLawAsync(sender.Text);
        }

        private void LawListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            lawDetail = (Law)e.ClickedItem;
            this.Frame.Navigate(typeof(DetailLaw));
        }
        private  async void LoadLaw() {
            listAllLaw = await client.GetAllLawAsync();
            LawListView.ItemsSource = listAllLaw;
        }

        private async void LoadLaw(string name)
        {
            listAllLaw = await client.SearchLawByNameAsync(name);
            if (listAllLaw.Count != 0)
            {
                LawListView.ItemsSource = listAllLaw;
            }
            else {
                LoadLaw();
            }
        }


        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else {
                this.Frame.Navigate(typeof(MainPage));
            }

        }
    }
}
