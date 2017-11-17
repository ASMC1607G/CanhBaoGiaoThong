using Bing.Maps;
using Bing.Maps.Directions;
using Bing.Maps.Search;
using BingMap.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BingMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        WarningTrafficServiceClient client = new WarningTrafficServiceClient();

        public static ObservableCollection<PushpinModel> PushpinCollection { get; set; }
        Location locationTap = null;
        string diem = "";
        Geolocator g;
        double lon, lat;
        DirectionsManager directionManager;
        public static string LawSearch="";
        

        public MainPage()
        {
            this.InitializeComponent();
            PushpinCollection = new ObservableCollection<PushpinModel>();
            Login.LoadWarning();
            DataContext = this;

            //PushpinCollection.Add(new PushpinModel() {TextPushpin="W", KeyPushPin="Ha Noi", Latitude = 21.027764, Longitude = 105.834160 });
            //PushpinCollection.Add(new PushpinModel() { TextPushpin = "W", KeyPushPin = "Ha Nam", Latitude = 20.583520, Longitude = 105.922990 });
            //PushpinCollection.Add(new PushpinModel() { TextPushpin="W", KeyPushPin = "Bac Ninh", Latitude = 21.121444, Longitude = 106.111050 });
            //PushpinCollection.Add(new PushpinModel() { TextPushpin = "W", KeyPushPin = "Ha Hung Yen", Latitude = 20.852571, Longitude = 106.016997 });
            //PushpinCollection.Add(new PushpinModel() { TextPushpin = "W", KeyPushPin = "Thai Binh", Latitude = 20.538694, Longitude = 106.393478 });
        }

        private async void Pushpin_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Pushpin pus = new Pushpin();
            pus = (Pushpin)sender;
            var p = pus.Tag.ToString();

            if ((DetailContent.Visibility==Visibility.Collapsed) || diem!=p)
            {
                
                diem = p;

                AddWarningContent.Visibility = Visibility.Collapsed;
                ScrollDirection.Visibility = Visibility.Collapsed;
                DirectionContent.Visibility = Visibility.Collapsed;

                DetailBackGround.Visibility = Visibility.Visible;
                DetailContent.Visibility = Visibility.Visible;

                if (p == "myLocation")
                {
                    TenDiem.Text = "Your Location Now...";

                    ContentTextBox.Visibility = Visibility.Collapsed;
                    TimeTextbox.Visibility = Visibility.Collapsed;
                    Police.Visibility = Visibility.Collapsed;
                    Hotline.Visibility = Visibility.Collapsed;
                    userPosted.Visibility = Visibility.Collapsed;

                }
                else if(p!="tapLocation"){
                    ContentTextBox.Visibility = Visibility.Visible;
                    TimeTextbox.Visibility = Visibility.Visible;
                    Police.Visibility = Visibility.Visible;
                    Hotline.Visibility = Visibility.Visible;
                    userPosted.Visibility = Visibility.Visible;

                    ServiceReference1.Warning wn = new ServiceReference1.Warning();
                    wn=await client.GetWarningByIDAsync(int.Parse(p));
                    TenDiem.Text = wn.address;
                    ContentTextBox.Text = wn.contentWarning;
                    TimeTextbox.Text = wn.time;
                    Police.Text = wn.police;
                    Hotline.Text = wn.hotline;
                    userPosted.Text = wn.email;
                }
            }
            else if(diem==p) {
                DetailBackGround.Visibility = Visibility.Collapsed;
                DirectionContent.Visibility = Visibility.Collapsed;
                AddWarningContent.Visibility = Visibility.Collapsed;
                DetailContent.Visibility = Visibility.Collapsed;
            }
        }

        private async void FindLocationBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //xoa diem vang
            var tapPushpin = myMap.Children.Where(b => (b.GetType() == typeof(Pushpin) && ((Pushpin)b).Tag.ToString() == "tapLocation")).FirstOrDefault();
            myMap.Children.Remove(tapPushpin);
            locationTap = null;
            //xoa pushpin cu
            var removePushpin = myMap.Children.Where(b => (b.GetType() == typeof(Pushpin) && ((Pushpin)b).Tag.ToString() == "findLocation")).FirstOrDefault();
            var mes=myMap.Children.Remove(removePushpin);

            var searchManager = myMap.SearchManager;
            var request = new GeocodeRequestOptions(FindLocationBox.Text);
            var response =await searchManager.GeocodeAsync(request);

            var pushpin = new Pushpin() {
                Tag = "findLocation",
                Text="F"
            };
            pushpin.Background = new SolidColorBrush(Colors.LawnGreen);
            if (response.LocationData.FirstOrDefault() != null)
            {
                FindLocationBox.Text="";
                MapLayer.SetPosition(pushpin, response.LocationData.FirstOrDefault().Location);
                myMap.Children.Add(pushpin);

                myMap.SetView(response.LocationData.FirstOrDefault().Bounds);
            }
            else {
                MessageDialog msgDialog = new MessageDialog("No Location Found","Message");
                await msgDialog.ShowAsync();
            }
            
        }

        private void myMap_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            FindLocationBox.Visibility = Visibility.Collapsed;
            AddWraningFindLocationBox.Visibility = Visibility.Collapsed;
        }

        private void FindLocation_Click(object sender, RoutedEventArgs e)
        {

            if (FindLocationBox.Visibility == Visibility.Collapsed)
            {
                DirectionPanel.Children.Clear();

                FindLocationBox.Visibility = Visibility.Visible;

                DetailContent.Visibility = Visibility.Collapsed;
                AddWraningFindLocationBox.Visibility = Visibility.Collapsed;
                AddWarningContent.Visibility = Visibility.Collapsed;
                DirectionContent.Visibility = Visibility.Collapsed;
                DetailBackGround.Visibility = Visibility.Collapsed;
                ScrollDirection.Visibility = Visibility.Collapsed;
            }
            else {
                FindLocationBox.Visibility = Visibility.Collapsed;
            }
        }

        private async void GetDirectionButton_Click(object sender, RoutedEventArgs e)
        {
            //get location hien tai
            DirectionPanel.Children.Clear();

            g = new Geolocator();
            Geoposition DeviceLocation = await g.GetGeopositionAsync();
            Location locaitonNow = new Location(DeviceLocation.Coordinate.Point.Position.Latitude, DeviceLocation.Coordinate.Point.Position.Longitude);

            if (FromLocationTextBox.Text != "" && ToLocationTextBox.Text != "") {
                Waypoint startWayPoint;
                Waypoint endWayPoint;
                
                if (FromLocationTextBox.Text == "Your Location Now..." && ToLocationTextBox.Text!= "Your Location Now...")
                {
                    startWayPoint = new Waypoint(locaitonNow);
                }
                else {
                    startWayPoint = new Waypoint(FromLocationTextBox.Text);
                }

                if (ToLocationTextBox.Text == "Your Location Now..." && FromLocationTextBox.Text != "Your Location Now...")
                {
                    endWayPoint = new Waypoint(locaitonNow);
                }
                else
                {
                    endWayPoint = new Waypoint(ToLocationTextBox.Text);
                }

                var wayPoints = new WaypointCollection();
                wayPoints.Add(startWayPoint);
                wayPoints.Add(endWayPoint);

                directionManager = myMap.DirectionsManager;
                directionManager.Waypoints = wayPoints;
                directionManager.RequestOptions.RouteMode = RouteModeOption.Driving;

                var response = await directionManager.CalculateDirectionsAsync();
                
                if (response.Routes.FirstOrDefault() != null)
                {
                    ScrollDirection.Visibility = Visibility.Visible;
                    DetailBackGround.Visibility = Visibility.Collapsed;
                    DirectionContent.Visibility = Visibility.Collapsed;

                    directionManager.ShowRoutePath(response.Routes.FirstOrDefault());
                    DirectionPanel.Children.Clear();
                    DirectionPanel.Children.Add(directionManager.RouteSummaryView);
                }
                else
                {
                    MessageDialog msgDialog = new MessageDialog("Sorry...Dont have Navigation", "Message");
                    await msgDialog.ShowAsync();
                }

            }
        }

        private void DirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectionContent.Visibility == Visibility.Collapsed) {

                //Xoa Noi Dung TextBox
                FromLocationTextBox.Text = "";
                ToLocationTextBox.Text = "";

                //xoa chi duong hien tai
                myMap.DirectionsManager.HideRoutePath(myMap.DirectionsManager.ActiveRoute);
                myMap.DirectionsManager.ClearActiveRoute();
                MapLayer directionsLayer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(myMap, 0), 0), 0) as MapLayer;
                directionsLayer.Children.Clear();

                AddWarningContent.Visibility = Visibility.Collapsed;
                AddWraningFindLocationBox.Visibility = Visibility.Collapsed;
                ScrollDirection.Visibility = Visibility.Collapsed;
                FindLocationBox.Visibility = Visibility.Collapsed;
                DetailContent.Visibility = Visibility.Collapsed;

                DetailBackGround.Visibility = Visibility.Visible;
                DirectionContent.Visibility = Visibility.Visible;

                DirectionPanel.Children.Clear();
            }else
            {
                DetailBackGround.Visibility = Visibility.Collapsed;
                DirectionContent.Visibility = Visibility.Collapsed;
            }
        }

        private void myMap_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            //tu dong hien len nhap thong tin them Warning
            DetailBackGround.Visibility = Visibility.Visible;
            AddWarningContent.Visibility = Visibility.Visible;
            DetailContent.Visibility = Visibility.Collapsed;

            //xoa pushpin cu
            var removePushpin = myMap.Children.Where(b => (b.GetType() == typeof(Pushpin) && ((Pushpin)b).Tag.ToString() == "tapLocation")).FirstOrDefault();
            var mes = myMap.Children.Remove(removePushpin);

            //lay ra locaiton
            var pos = ((RightTappedRoutedEventArgs)e).GetPosition(myMap);
            Location location;
            myMap.TryPixelToLocation(pos, out location);
            locationTap = location;

            //them pushpin moi
            Pushpin pushpin = new Pushpin();
            pushpin.Background = new SolidColorBrush(Colors.Yellow);
            pushpin.Tag = "tapLocation";
            MapLayer.SetPosition(pushpin, location);
            myMap.Children.Add(pushpin);
            pushpin.Tapped += Pushpin_Tapped;

            myMap.SetView(location);
        }

        private void FromMyLocation_Click(object sender, RoutedEventArgs e)
        {
            FromLocationTextBox.Text = "Your Location Now...";
            if (ToLocationTextBox.Text == "Your Location Now...") {
                ToLocationTextBox.Text = "";
            }
        }

        private void ToMyLocation_Click(object sender, RoutedEventArgs e)
        {
            ToLocationTextBox.Text = "Your Location Now...";
            if (FromLocationTextBox.Text == "Your Location Now...")
            {
                FromLocationTextBox.Text = "";
            }
        }

        private void FromMyLocation_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            FromMyLocation.BorderThickness = new Thickness(0);
        }
        private void ToMyLocation_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ToMyLocation.BorderThickness = new Thickness(0);
        }

        private  void AddWarningButton_Click(object sender, RoutedEventArgs e)
        {
            DirectionContent.Visibility = Visibility.Collapsed;
            DetailBackGround.Visibility = Visibility.Collapsed;
            ScrollDirection.Visibility = Visibility.Collapsed;
            FindLocationBox.Visibility = Visibility.Collapsed;

            //kiem tra xem da tapped chua
            var tapPushpin = myMap.Children.Where(b => (b.GetType() == typeof(Pushpin) && ((Pushpin)b).Tag.ToString() == "tapLocation")).FirstOrDefault();

            if (tapPushpin != null && locationTap!=null)
            {
                DetailBackGround.Visibility = Visibility.Visible;
                AddWarningContent.Visibility = Visibility.Visible;
            }
            else {
                AddWraningFindLocationBox.Visibility = Visibility.Visible;
                FindLocationBox.Visibility = Visibility.Collapsed;
            }
        }

        private async void AddWraningFindLocationBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //xoa pushpin cu
            var removePushpin = myMap.Children.Where(b => (b.GetType() == typeof(Pushpin) && ((Pushpin)b).Tag.ToString() == "findLocation")).FirstOrDefault();
            var mes = myMap.Children.Remove(removePushpin);

            var searchManager = myMap.SearchManager;
            var request = new GeocodeRequestOptions(AddWraningFindLocationBox.Text);
            var response = await searchManager.GeocodeAsync(request);
            
            if (response.LocationData.FirstOrDefault() != null)
            {
                FindLocationBox.Text = "";
                myMap.SetView(response.LocationData.FirstOrDefault().Bounds);
            }
            else
            {
                MessageDialog msgDialog = new MessageDialog("No Location Found", "Message");
                await msgDialog.ShowAsync();
            }

            MessageDialog msgDialogThongbaos = new MessageDialog("Please select a detail location.", "Message");
            await msgDialogThongbaos.ShowAsync();
        }

        private void CancleButton_Click(object sender, RoutedEventArgs e)
        {
            var removePushpin = myMap.Children.Where(b => (b.GetType() == typeof(Pushpin) && ((Pushpin)b).Tag.ToString() == "tapLocation")).FirstOrDefault();
            var mes = myMap.Children.Remove(removePushpin);
            locationTap = null; 

            DetailBackGround.Visibility = Visibility.Collapsed;
            AddWarningContent.Visibility = Visibility.Collapsed;
            AddressTextBox.Text = "";
            WarningTextBox.Text = "";
            TimeWraningTextBox.Text = "";
            PoliceTextBox.Text = "";
            HotLineTextBox.Text = "";
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (locationTap != null && AddressTextBox.Text != "" && WarningTextBox.Text != "" && HotLineTextBox.Text != "")
            {


                //xu ly goi len services
                ServiceReference1.Warning wn = new ServiceReference1.Warning();
                wn.address = AddressTextBox.Text;
                wn.contentWarning = WarningTextBox.Text;
                wn.time = TimeWraningTextBox.Text;
                wn.police = PoliceTextBox.Text;
                wn.hotline = HotLineTextBox.Text;
                wn.lat = locationTap.Latitude;
                wn.lon = locationTap.Longitude;
                wn.email= Login.userLogin.email;

                await client.AddWarningAsync(wn);
                Login.LoadWarning();
                //gan vtri null
                locationTap = null;
                //xoa diem vang
                var tapPushpin = myMap.Children.Where(b => (b.GetType() == typeof(Pushpin) && ((Pushpin)b).Tag.ToString() == "tapLocation")).FirstOrDefault();
                myMap.Children.Remove(tapPushpin);

                AddressTextBox.Text = "";
                WarningTextBox.Text = "";
                TimeWraningTextBox.Text = "";
                PoliceTextBox.Text = "";
                HotLineTextBox.Text = "";

                MessageDialog msgDialog = new MessageDialog("Add Warning Success", "Message");
                await msgDialog.ShowAsync();
                DetailBackGround.Visibility = Visibility.Collapsed;
                AddWarningContent.Visibility = Visibility.Collapsed;
            }
            else {
                MessageDialog msgDialog = new MessageDialog("Please fill textbox", "Message");
                await msgDialog.ShowAsync();
            }
        }

        private void InforUser_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfor));
        }

        private void LawButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LawPage));
        }

        private void ContentTextBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LawSearch = ContentTextBox.Text;
            this.Frame.Navigate(typeof(LawPage));
        }

        private async void MyLocation_Click(object sender, RoutedEventArgs e)
        {
            g = new Geolocator();
            Geoposition DeviceLocation = await g.GetGeopositionAsync();
            lon = DeviceLocation.Coordinate.Point.Position.Longitude;
            lat = DeviceLocation.Coordinate.Point.Position.Latitude;
            myMap.Center = new Bing.Maps.Location(lat, lon);
            myMap.ZoomLevel = 16D;

            Location loc = new Location(lat, lon);
            Pushpin pushpin = new Pushpin();
            pushpin.Tag = "myLocation";
            pushpin.Text = "Y";
            pushpin.Background = new SolidColorBrush(Colors.Blue);
            MapLayer.SetPosition(pushpin, loc);
            myMap.Children.Add(pushpin);

            pushpin.Tapped += Pushpin_Tapped;
        }
    }
    public class PushpinModel
    {
        public string TextPushpin { get; set; }
        public string KeyPushPin { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
