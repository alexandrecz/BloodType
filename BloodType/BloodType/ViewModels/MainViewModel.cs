using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BloodType.Resources;

namespace BloodType.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _sampleProperty = "Sample Runtime Property Value";



        public MainViewModel()
        {
            this.MainItems = new ObservableCollection<ItemViewModel>();
            this.SettingsItems = new ObservableCollection<ItemViewModel>();
        }

        public ObservableCollection<ItemViewModel> MainItems { get; private set; }
        public ObservableCollection<ItemViewModel> SettingsItems { get; private set; }


        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public void LoadData()
        {
            this.MainItems.Add(new ItemViewModel() { Id = 1, LineOne =AppResources.FindType, LineTwo = "just have fun trying find out your blood type" });
            this.MainItems.Add(new ItemViewModel() { Id = 2, LineOne = AppResources.SetType, LineTwo = "if you know your right blood type, set up it here" });
            this.SettingsItems.Add(new ItemViewModel() { Id = 1, LineOne = AppResources.LockScreen, LineTwo = "put your blood type in the lockscreen" });
            //this.SettingsItems.Add(new ItemViewModel() { Id = 2, LineOne = AppResources.LiveTile, LineTwo = "manage your data settings" });
            this.IsDataLoaded = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}