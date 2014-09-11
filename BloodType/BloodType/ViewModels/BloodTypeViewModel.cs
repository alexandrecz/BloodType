using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;

namespace BloodType.ViewModels
{
    public class BloodTypeViewModel : INotifyPropertyChanged
    {
        #region attributes

        private readonly object _sync = new object();
        private const string BloodTypeValue = "";

        private string resultType = string.Empty;
        private string resultTitle = string.Empty;
        private IList<BloodType> list = new List<BloodType>();
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region constructor

        public BloodTypeViewModel()
        {
            LoadTypes();
        }

        #endregion

        #region methods

        internal void ResfreshData()
        {
            this.ResultTitle = string.Empty;
            this.ResultType = string.Empty;
        }

        private void LoadTypes()
        {
            list.Add(new BloodType(1, "O"));
            list.Add(new BloodType(2, "O, A"));
            list.Add(new BloodType(3, "O, B"));
            list.Add(new BloodType(4, "A, B"));
            list.Add(new BloodType(5, "A, B, AB"));
            list.Add(new BloodType(6, "O, A, B, AB"));
        }

        public void ChargeTile()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(BloodTypeValue))
            {
                string content = IsolatedStorageSettings.ApplicationSettings[BloodTypeValue].ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
                    FlipTileData oFliptile = new FlipTileData();

                    oFliptile.BackTitle = "bloodtype";
                    oFliptile.BackContent = content;
                    oFliptile.WideBackContent = string.Format("MY BLOOD TYPE IS {0}", content);
                    tile.Update(oFliptile);
                }
            }
        }

        public string GetBloodByNumber(int number, bool rh = false)
        {
            string result = "A+";

            if (rh)
            {
                switch (number)
                {
                    case 1:
                        {
                            result = "O-";
                            break;
                        }

                    case 2:
                        {
                            result = "O+";
                            break;
                        }

                    case 3:
                        {
                            result = "A-";
                            break;
                        }
                    case 4:
                        {
                            result = "A+";
                            break;
                        }
                    case 5:
                        {
                            result = "B-";
                            break;
                        }
                    case 6:
                        {
                            result = "B+";
                            break;
                        }
                    case 7:
                        {
                            result = "AB-";
                            break;
                        }
                    case 8:
                        {
                            result = "AB+";
                            break;
                        }
                }
            }
            else
            {
                switch (number)
                {
                    case 1:
                        {
                            result = "O";
                            break;
                        }
                    case 2:
                        {
                            result = "A";
                            break;
                        }
                    case 3:
                        {
                            result = "B";
                            break;
                        }
                    case 4:
                        {
                            result = "AB";
                            break;
                        }
                }
            }
            return result;
        }

        public string ResultType
        {
            get { return resultType; }
            private set { resultType = value; NotifyPropertyChanged("ResultType"); }
        }

        public string ResultTitle
        {
            get { return resultTitle; }
            private set { resultTitle = value; NotifyPropertyChanged("ResultTitle"); }
        }
        
        private void SaveSettings()
        {
            lock (_sync)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public void SaveSettings(string content)
        {
            IsolatedStorageSettings.ApplicationSettings[BloodTypeValue] = content;
            SaveSettings();
            ChargeTile();
        }

        public void LetsRock(int first, int second)
        {

            // 1X1          O+O=    O(R1),
            // 1X2 | 2X1    O+A=  O/A(R2),
            // 1X3 | 3X1    O+B=   O/B(R3),
            // 1X4 | 4X1    O+AB=  A/B(R4) 

            // 2X2          A+A=  (R2),
            // 2X4 | 4X2    A+AB=  A/B/AB(R5)
            // 3X2 | 3X2    B+A=   O,A,B,AB(R6)
            // 3X3          B+B=  (R3),
            // 3X4 | 4X3    B+AB=  (R5),
            // 4X4          AB+AB=  (R5)

            string check = string.Concat(first.ToString(), second.ToString());
            this.ResultTitle = "You blood type maybe is one of these:";

            if (check == "11")
            {
                this.ResultTitle = "You blood type maybe is:";
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 1) as BloodType).Result;
            }
            else if (check == "12" || check == "21")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 2) as BloodType).Result;
            }
            else if (check == "13" || check == "31")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 3) as BloodType).Result;
            }
            else if (check == "14" || check == "41")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 4) as BloodType).Result;
            }
            else if (check == "22")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 2) as BloodType).Result;
            }
            else if (check == "24" || check == "42")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 5) as BloodType).Result;
            }
            else if (check == "32" || check == "23")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 6) as BloodType).Result;
            }
            else if (check == "33")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 3) as BloodType).Result;
            }
            else if (check == "34" || check == "43")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 5) as BloodType).Result;
            }
            else if (check == "44")
            {
                this.ResultType = ((list as List<BloodType>).Find(f => f.Id == 5) as BloodType).Result;
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class BloodType
    {

        public BloodType(int id, string result)
        {
            Id = id;
            Result = result;
        }

        public int Id { get; private set; }
        public string Result { get; private set; }
    }
}