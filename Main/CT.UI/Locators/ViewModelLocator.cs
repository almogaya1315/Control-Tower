using CT.UI.Proxy;
using CT.UI.ViewModels;
using CT.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CT.UI.Locators
{
    public class ViewModelLocator
    {
        public ViewModelLocator() { }

        public ViewModelLocator(AirportUserControl control, SimServiceProxy proxy) : this()
        {
            airportViewModel = new AirportViewModel(control, proxy);
        }

        AirportViewModel airportViewModel;
        public AirportViewModel AirportViewModel
        {
            get
            {
                return airportViewModel;
            }
        }
    }
}
