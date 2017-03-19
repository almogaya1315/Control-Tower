using CT.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.UI.Locators
{
    public class ViewModelLocator
    {
        static AirportViewModel airportViewModel;

        public ViewModelLocator()
        {
            airportViewModel = new AirportViewModel();
        }

        public static AirportViewModel AirportViewModel
        {
            get
            {
                return airportViewModel;
            }
        }
    }
}
