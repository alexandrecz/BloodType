using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace BloodType.Views
{
    /// <summary>
    /// TO DO: LOCALIZATED STRINGS AND LOGIC RANDOM
    /// </summary>
    public partial class FindView : PhoneApplicationPage
    {
        private DataTransferManager dataTransferManager;
        private string bodyMessage = string.Empty;
        public FindView()
        {
            InitializeComponent();
            DataContext = App.BloodViewModel;

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
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton findButton = new ApplicationBarIconButton();
            findButton.IconUri = new Uri("/Assets/feature.search.png", UriKind.Relative);
            findButton.Text = "find";
            ApplicationBar.Buttons.Add(findButton);

            ApplicationBarIconButton shareButton = new ApplicationBarIconButton();
            shareButton.IconUri = new Uri("/Assets/share.png", UriKind.Relative);
            shareButton.Text = "share";
            ApplicationBar.Buttons.Add(shareButton);
            shareButton.IsEnabled = false;

            ApplicationBarIconButton refreshButton = new ApplicationBarIconButton();
            refreshButton.IconUri = new Uri("/Assets/refresh.png", UriKind.Relative);
            refreshButton.Text = "refresh";
            ApplicationBar.Buttons.Add(refreshButton);
            refreshButton.IsEnabled = false;

            findButton.Click += (s, e) =>
            {
                if (CheckParentsType())
                {
                    (DataContext as BloodType.ViewModels.BloodTypeViewModel).LetsRock(Convert.ToInt32(sliderMother.Value), Convert.ToInt32(sliderFather.Value));

                    PlayType.Begin();
                    shareButton.IsEnabled = true;
                    refreshButton.IsEnabled = true;
                }
            };

            shareButton.Click += (s, e) =>
            {
                if (CheckParentsType())
                {
                    bodyMessage = "I have just founded my blood type using BLOODTYPE App for Windows Phone 8!";
                    if (resultText.Text.Split(',').Length > 1)
                    {
                        bodyMessage += "There is a great chance of blood type is one of these: " + resultText.Text;
                    }
                    else
                    {
                        bodyMessage += "Maybe my blood type is: " + resultText.Text;
                    }

                    DataTransferManager.ShowShareUI();
                }
            };

            refreshButton.Click += (s1, e1) =>
            {
                (DataContext as BloodType.ViewModels.BloodTypeViewModel).ResfreshData();
                sliderMother.Value = 0;
                sliderFather.Value = 0;
                motherText.Text = string.Empty;
                fatherText.Text = string.Empty;

                PlayTypeRev.Begin();
                shareButton.IsEnabled = false;
                refreshButton.IsEnabled = false;
            };
        }

        private bool CheckParentsType()
        {
            if (string.IsNullOrEmpty(motherText.Text))
            {
                System.Windows.MessageBox.Show("You must choose the Mother's blood type, please!");
            }
            else if (string.IsNullOrEmpty(fatherText.Text))
            {
                System.Windows.MessageBox.Show("You must choose the Father's blood type, please!");
            }
            return (!string.IsNullOrEmpty(motherText.Text)) && (!string.IsNullOrEmpty(fatherText.Text));
        }

        private void sliderFather_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OldValue != 0.0)
            {
                fatherText.Text = App.BloodViewModel.GetBloodByNumber(Convert.ToInt32(e.NewValue));
            }
        }

        private void sliderMother_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OldValue != 0.0)
            {
                motherText.Text = App.BloodViewModel.GetBloodByNumber(Convert.ToInt32(e.NewValue));
            }
        }


        private void SetTypeView_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "Blood Type Founded!!!";
            request.Data.Properties.Description = "BloodType share";
            request.Data.SetText(bodyMessage);

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