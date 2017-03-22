﻿using CT.Common.DTO_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.Abstracts
{
    public abstract class CTBindingData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ObservableCollection<FlightDTO> preUnloading;
        public ObservableCollection<FlightDTO> PreUnloading
        {
            get
            {
                return preUnloading;
            }
            set
            {
                preUnloading = value;
                RaisePropertyChanged("PreUnloading");
            }
        }

        ObservableCollection<FlightDTO> preDeparting;
        public ObservableCollection<FlightDTO> PreDeparting
        {
            get
            {
                return preDeparting;
            }
            set
            {
                preDeparting = value;
                RaisePropertyChanged("PreDeparting");
            }
        }

        FlightDTO flightInLanding1;
        public FlightDTO FlightInLanding1
        {
            get
            {
                return flightInLanding1;
            }
            set
            {
                flightInLanding1 = value;
                RaisePropertyChanged("FlightInLanding1");
            }
        }
    }
}