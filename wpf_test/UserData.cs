using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpf_test
{
    public class UserData ():INotifyPropertyChanged
    {
        public List<string> FIO { get; set; }

                private string _selectedPosition_fio;
                public string SelectedPosition_fio
                {
                    get => _selectedPosition_fio;
                    set
                   {
                        if (_selectedPosition_fio != value)
                        {
                            _selectedPosition_fio = value;
                            //OnPropertyChanged(nameof(SelectedPosition_fio));
                        }
                    }
                }

        public List<string> Position { get; set; }

                    private string _selectedPosition_pos;
                    public string SelectedPosition_pos
                    {
                        get =>_selectedPosition_pos;
                        set
                        {
                            if (_selectedPosition_pos != value)
                            {
                                _selectedPosition_pos = value;
                                OnPropertyChanged(nameof(SelectedPosition_pos));
                            }
                        }
                    }

        public string? Second_name { get; set; } = "";

        private bool _sign;
        public bool Sign
        {
            get => _sign;

            set
            {
                if (_sign != value)
                {
                    _sign = value;
                    OnPropertyChanged(nameof(Sign));
                    OnChangeSign(nameof(Sign));
                } 
            }
        } 


        public bool SignEnabled { get; set; } = false;

        private string _signPath;
        public string? SignPath
        {
            get => _signPath;
            set
            {
                if (_signPath != value)
                {
                    _signPath = value;
                    OnPropertyChanged(nameof(SignPath));
                }
            }
        }


        private string _date;
        public string? Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler ChangedSign;


        protected virtual void OnChangeSign(string propertyName)
        {
            ChangedSign?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
