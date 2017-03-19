using CT.Common.Commands;
using CT.Common.DTO_Models;
using CT.Common.Utilities;
using CT.UI.Proxy;
using CT.UI.SimulatorServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CT.UI.ViewModels
{
    public class AirportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        SimServiceProxy simProxy;
        ICollection<TextBlock> txtblckCheckpoints { get; set; }
        ICollection<ListView> lstvwsCheckpoints { get; set; }
        ICollection<Image> imgPlanes { get; set; }

        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddFlightCommand { get; set; }

        BitmapImage planeImage;
        public BitmapImage PlaneImage
        {
            get
            {
                return planeImage;
            }
            set
            {
                planeImage = value;
                RaisePropertyChanged("PlaneImage");
            }
        }

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

            simProxy.OnLoadEvent += SimProxy_OnLoadEvent;
            simProxy.OnPromotionEvaluationEvent += SimProxy_OnPromotionEvaluationEvent;
            simProxy.OnDisposeEvent += SimProxy_OnDisposeEvent;

            txtblckCheckpoints = InitializeTxtblckCheckpoints();
            lstvwsCheckpoints = InitializeLstvwsCheckpoints();
            imgPlanes = InitializeImgPlanes();
        }

        public void AddFlight()
        {
            RequestFlightObject 
            simProxy.CreateFlightObject();
        }
    }
}
