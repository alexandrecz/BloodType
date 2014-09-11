using BloodType.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Phone.System.UserProfile;
using Windows.Storage;

namespace BloodType.Views
{
    public partial class ImagePreviewView : PhoneApplicationPage
    {
        #region attributes

        private bool HasImage = false;
        private double _initialAngle;
        private double _initialScale;
        private ApplicationBarIconButton saveButton;
        private BitmapImage backgroundImage = new BitmapImage();

        #endregion

        #region constructor

        public ImagePreviewView()
        {
            InitializeComponent();
            BuildApplicationBar();
            this.Loaded += ImagePreviewView_Loaded;
        }

        async void ImagePreviewView_Loaded(object sender, RoutedEventArgs e)
        {
            await Save();
            SetLockScreen(TB1.Text);
        }

        #endregion

        #region methods

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.2;
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.IsMenuEnabled = true;


            saveButton = new ApplicationBarIconButton();
            saveButton.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative);
            saveButton.Text = "check";
            ApplicationBar.Buttons.Add(saveButton);
            saveButton.IsEnabled = true;

            saveButton.Click += saveButton_Click;
        }

        async void saveButton_Click(object sender, EventArgs e)
        {
            await Save();
        }

        string fileName = "Lock_" + Guid.NewGuid().ToString() + ".jpg";

        private async Task Save()
        {
            this.UpdateLayout();
            this.Measure(new Size(480, 800));
            this.UpdateLayout();
            this.Arrange(new Rect(0, 0, 480, 800));
            var wb = new WriteableBitmap(480, 800);
            wb.Render(this, null);
            wb.Invalidate();
            using (var stream = new MemoryStream())
            {

                wb.SaveJpeg(stream, 480, 800, 0, 75);
                stream.Seek(0, SeekOrigin.Begin);
                fileName = "Lock_" + Guid.NewGuid().ToString() + ".jpg";

                await SaveToLocalFolderAsync(stream, fileName);

            }

            if (HasImage)
            {
                Uri uri_UI = new Uri("ms-appdata:///Local/" + fileName, UriKind.RelativeOrAbsolute);
                Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri_UI);
            }
        }


        private async Task SaveToLocalFolderAsync(Stream file, string fileName)
        {
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Delete images from earlier execution
            var filesTodelete = from f in myStore.GetFileNames("Lock_*").AsQueryable()
                                where f != fileName
                                select f;

            
            foreach (var fi in (filesTodelete))
            {

                myStore.DeleteFile(fi);
            }

            
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream outputStream = await storageFile.OpenStreamForWriteAsync())
            {
                await file.CopyToAsync(outputStream);
            }
                      

        }





        public void BeginSaveJpeg(string bloodType)
        {

            IAsyncResult result = Microsoft.Xna.Framework.GamerServices.Guide.BeginShowMessageBox("Question", "Do you wish choose a picture from the album?", new string[] { "yes", "no" }, 0, Microsoft.Xna.Framework.GamerServices.MessageBoxIcon.None, null, null);
            result.AsyncWaitHandle.WaitOne();

            int? choice = Microsoft.Xna.Framework.GamerServices.Guide.EndShowMessageBox(result);
            if (choice.HasValue)
            {
                if (choice.Value == 0)
                {
                    PhotoChooserTask photoChooserTask = new PhotoChooserTask();
                    photoChooserTask.Completed += (s, e) =>
                    {
                        if (e.TaskResult == TaskResult.OK)
                        {
                            backgroundImage = new BitmapImage();
                            backgroundImage.SetSource(e.ChosenPhoto);
                            BackgroundImage.Source = backgroundImage;

                            Task.Delay(3000);
                            HasImage = true;
                            App.BloodViewModel.SaveSettings(bloodType);
                        }
                    };
                    photoChooserTask.Show();
                }
                else
                {
                    Uri uri_UI = new Uri("ms-appdata:///Local/" + fileName, UriKind.RelativeOrAbsolute);
                    Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri_UI);
                    App.BloodViewModel.SaveSettings(bloodType);
                }
            }


        }

        public async void SetLockScreen(string bloodType)
        {
            if (!LockScreenManager.IsProvidedByCurrentApplication)
            {
                await LockScreenManager.RequestAccessAsync();
            }

            if (LockScreenManager.IsProvidedByCurrentApplication)
            {
                LockScreenChange(bloodType, true);
            }
        }

        private async void LockScreenChange(string bloodtype, bool isAppResource)
        {
            if (!LockScreenManager.IsProvidedByCurrentApplication)
            {
                await LockScreenManager.RequestAccessAsync();
            }

            if (LockScreenManager.IsProvidedByCurrentApplication)
            {
                BeginSaveJpeg(bloodtype);
            }
            else
            {
                MessageBox.Show("Background cant be updated as you clicked no!!");
            }
        }




        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _initialAngle = MyMustacheTransformation.Rotation;
            _initialScale = MyMustacheTransformation.ScaleX;
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            MyMustacheTransformation.Rotation = _initialAngle + e.TotalAngleDelta;
            MyMustacheTransformation.ScaleX = _initialScale * e.DistanceRatio;
            MyMustacheTransformation.ScaleY = _initialScale * e.DistanceRatio;
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            System.Windows.Controls.Image rect = sender as System.Windows.Controls.Image;
            System.Windows.Media.TranslateTransform transform = rect.RenderTransform as System.Windows.Media.TranslateTransform;

            MyMustacheTransformation.TranslateX += e.HorizontalChange;
            MyMustacheTransformation.TranslateY += e.VerticalChange;
        }


        //share register
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            if (NavigationContext.QueryString.ContainsKey("type"))
            {
                TB1.Text = NavigationContext.QueryString["type"];
                if (!TB1.Text.ToString().Contains('-'))
                {
                    TB1.Text = TB1.Text.ToString().Trim() + "+";
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

        }


        #endregion



    }


}