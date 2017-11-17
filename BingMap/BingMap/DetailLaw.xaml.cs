using BingMap.ServiceReference1;
using System;
using System.Collections.Generic;
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
    public sealed partial class DetailLaw : Page
    {
        public static Law LawLink = new Law();
        public DetailLaw()
        {
            this.InitializeComponent();
            NameLaw.Text = LawPage.lawDetail.nameLaw;
            BaseLaw.Text = LawPage.lawDetail.contentLaw;
            CondemnLaw.Text = LawPage.lawDetail.contentLaw;
            LinkLaw.Text = LawPage.lawDetail.description;

            MainPage.LawSearch = "";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                this.Frame.Navigate(typeof(LawPage));
            }
        }

        private void LinkLaw_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (LinkLaw.Text.IndexOf("htt") != -1) {
                LawLink.description = LinkLaw.Text;
                LawLink.idLaw = LawPage.lawDetail.idLaw;
                this.Frame.Navigate(typeof(BrowsePage));
            }
        }
    }
}
