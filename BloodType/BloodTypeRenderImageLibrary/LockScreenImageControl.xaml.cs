using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.Windows.Navigation;

namespace BloodTypeRenderImageLibrary
{
    public partial class LockScreenImageControl : UserControl
    {

        public LockScreenImageControl(string bloodtype)
        {
            InitializeComponent();
            //TB1.Text = "Created at por CZ " + DateTime.Now.ToShortTimeString();
            TB1.Text = bloodtype;
        }

        public event EventHandler<SaveJpegCompleteEventArgs> SaveJpegComplete;

        public void BeginSaveJpeg()
        {

            BitmapImage backgroundImage = new System.Windows.Media.Imaging.BitmapImage();

            IAsyncResult result = Microsoft.Xna.Framework.GamerServices.Guide.BeginShowMessageBox("Question", "Do you wish choose a picture from the album?",
                new string[] { "yes", "no" }, 0, Microsoft.Xna.Framework.GamerServices.MessageBoxIcon.Alert, null, null);

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
                            backgroundImage.SetSource(e.ChosenPhoto);
                            BackgroundImage.Source = backgroundImage;
                            ReplaceImage();
                        }
                    };
                    photoChooserTask.Show();
                }
                else
                {
                    SaveDefault(backgroundImage);
                }
            }

            return;
        }

        private void SaveDefault(BitmapImage backgroundImage)
        {
            backgroundImage = new BitmapImage(new Uri("lock.jpg", UriKind.Relative));
            backgroundImage.CreateOptions = BitmapCreateOptions.None;

            backgroundImage.ImageOpened += (s1, args) =>
            {
                BackgroundImage.Source = backgroundImage;
                ReplaceImage();
            };
        }

        private void ReplaceImage()
        {
            try
            {
                // Explicitly size the control - for use in a background agent
                this.UpdateLayout();
                this.Measure(new Size(480, 800));
                this.UpdateLayout();
                this.Arrange(new Rect(0, 0, 480, 800));

                var wb = new WriteableBitmap(480, 800);
                wb.Render(LayoutRoot, null);
                wb.Invalidate();
                // Create a filename for JPEG file in isolated storage.
                String fileName = "Lock_" + Guid.NewGuid().ToString() + ".jpg";
                var myStore = IsolatedStorageFile.GetUserStoreForApplication();

                using (IsolatedStorageFileStream myFileStream = myStore.CreateFile(fileName))
                {
                    // Encode WriteableBitmap object to a JPEG stream.
                    wb.SaveJpeg(myFileStream, wb.PixelWidth, wb.PixelHeight, 0, 75);
                    myFileStream.Close();
                }

                // Delete images from earlier execution
                var filesTodelete = from f in myStore.GetFileNames("Lock_*").AsQueryable()
                                    where f != fileName
                                    select f;
                foreach (var file in filesTodelete)
                {
                    myStore.DeleteFile(file);
                }

                // Fire completion event
                if (SaveJpegComplete != null)
                {
                    SaveJpegComplete(this, new SaveJpegCompleteEventArgs(true, fileName));
                    MessageBox.Show("Lock screen changed!");
                }

            }
            catch (Exception ex)
            {
                // Log it
                System.Diagnostics.Debug.WriteLine("Exception in SaveJpeg: " + ex.ToString());

                if (SaveJpegComplete != null)
                {
                    var args1 = new SaveJpegCompleteEventArgs(false, "");
                    args1.Exception = ex;
                    SaveJpegComplete(this, args1);
                }
            }
        }

    }

    public class SaveJpegCompleteEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public string ImageFileName { get; set; }

        public SaveJpegCompleteEventArgs(bool success, string fileName)
        {
            Success = success;
            ImageFileName = fileName;
        }
    }
}
