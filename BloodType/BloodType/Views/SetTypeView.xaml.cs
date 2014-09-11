using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows.Navigation;
using Windows.ApplicationModel.DataTransfer;

namespace BloodType.Views
{
    public partial class SetTypeView : PhoneApplicationPage
    {
        //share
        private DataTransferManager dataTransferManager;
        private ApplicationBarIconButton shareButton;

        /// <summary>
        /// TO DO MENU BUTTONS
        /// TO DO LOCALIZATED STRINGS
        /// TO DO TIPS FROM BLOOD TYPES
        /// </summary>
        public SetTypeView()
        {
            InitializeComponent();
            BuildApplicationBar();
            Loaded += (s, e) =>
                {
                    PlayBubbles.Begin();
                };
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.2;
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.IsMenuEnabled = true;


            shareButton = new ApplicationBarIconButton();
            shareButton.IconUri = new Uri("/Assets/share.png", UriKind.Relative);
            shareButton.Text = "share";
            ApplicationBar.Buttons.Add(shareButton);
            shareButton.IsEnabled = false;


            shareButton.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(myTypeText.Text))
                {
                    System.Windows.MessageBox.Show("You must choose one type of blood, please!");
                }
                else
                {
                    DataTransferManager.ShowShareUI();
                }
            };


            ApplicationBarMenuItem tile = new ApplicationBarMenuItem();
            tile.Text = "set on live tiles";
            ApplicationBar.MenuItems.Add(tile);

            tile.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(myTypeText.Text))
                {
                    System.Windows.MessageBox.Show("You must choose one type of blood, please!");
                }
                else
                {
                    App.BloodViewModel.SaveSettings(myTypeText.Text);
                    System.Windows.MessageBox.Show("Well done!");
                }
            };


            ApplicationBarMenuItem lockScreen = new ApplicationBarMenuItem();
            lockScreen.Text = "set as lock screen";
            ApplicationBar.MenuItems.Add(lockScreen);

            lockScreen.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(myTypeText.Text))
                {
                    System.Windows.MessageBox.Show("You must choose one type of blood, please!");
                }
                else
                {
                    NavigationService.Navigate(new Uri(string.Format("/Views/ImagePreviewView.xaml?type={0}", myTypeText.Text), UriKind.Relative));
                }
            };
        }

        private async void sliderMy_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OldValue != 0.0)
            {
                myTypeText.Text = App.BloodViewModel.GetBloodByNumber(Convert.ToInt32(e.NewValue), true);

                await System.Threading.Tasks.Task.Delay(300);
                resultText.Text = myTypeText.Text;
                shareButton.IsEnabled = true;
                PlayType.Begin();
            }
        }


        private void SetTypeView_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "Blood Type Founded!!!";
            request.Data.Properties.Description = "BloodType Share";
            request.Data.SetText("Just in case, I've just set up my blood type using BLOODTYPE app from WindowsPhone 8! It one is: " + myTypeText.Text);
        }

        //share register
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += SetTypeView_DataRequested;
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            dataTransferManager.DataRequested -= SetTypeView_DataRequested;
        }
    }
}