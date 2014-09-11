using Microsoft.Phone.Controls;
using System;
using System.Windows.Navigation;

namespace BloodType
{
    public partial class MainPage : PhoneApplicationPage
    {
    

        /// <summary>
        /// TO DO LOCALIZATED STRINGS
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            PLayBubbles.Begin();         
        }

        private void MainMenu_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MainMenu.SelectedItem != null)
            {
                switch ((MainMenu.SelectedItem as BloodType.ViewModels.ItemViewModel).Id)
                {
                    case 1:
                        {
                            //NavigationService.Navigate(new Uri("/Views/FindView.xaml?selectedItem=" + MainMenu.SelectedIndex, UriKind.Relative));
                            NavigationService.Navigate(new Uri("/Views/FindView.xaml", UriKind.Relative));
                            break;
                        }
                    case 2:
                        {
                            NavigationService.Navigate(new Uri("/Views/SetTypeView.xaml", UriKind.Relative));
                            break;
                        }
                }
            }

            MainMenu.SelectedItem = null;
        }

        private async void SettingsMenu_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SettingsMenu.SelectedItem != null)
            {
                switch ((SettingsMenu.SelectedItem as BloodType.ViewModels.ItemViewModel).Id)
                {
                    case 1:
                        {
                            var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
                            break;
                        }
                    case 2:
                        {
                            NavigationService.Navigate(new Uri("/Views/TileSettingsView.xaml", UriKind.Relative));
                            break;
                        }
                }
            }

            SettingsMenu.SelectedItem = null;
        }

        private void EmailMe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask emailComposeTask = new Microsoft.Phone.Tasks.EmailComposeTask();
            emailComposeTask.To = "alexandrecz@live.com";
            emailComposeTask.Subject = "BloodType - Feddback";
            emailComposeTask.Body = "Leave your comments here...";
            emailComposeTask.Show();
        }

        private void RateMe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.MarketplaceReviewTask mk = new Microsoft.Phone.Tasks.MarketplaceReviewTask();
            mk.Show();
        }                
    }
}