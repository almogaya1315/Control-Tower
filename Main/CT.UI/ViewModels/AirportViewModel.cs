using CT.Common.Commands;
using CT.Common.DTO_Models;
using CT.UI.Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CT.UI.ViewModels
{
    public class AirportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        SimServiceProxy simProxy;

        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddFlightCommand { get; set; }

        FlightDTO currentFlight;
        public FlightDTO CurrentFlight
        {
            get
            {
                return currentFlight;
            }
            set
            {
                currentFlight = value;
                RaisePropertyChanged("CurrentFlight");
            }
        }

        public AirportViewModel(SimServiceProxy proxy)
        {
            simProxy = proxy;

            AddFlightCommand = new AddFlightCommand(AddFlight);
        }

        public void AddFlight()
        {
            //simProxy.CreateFlightObject();
        }
    }
}
