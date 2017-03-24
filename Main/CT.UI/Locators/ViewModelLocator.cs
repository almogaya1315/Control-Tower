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
    /// <summary>
    /// The class that connects to the UI's XAML and holds the view model object
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator() { }

        /// <summary>
        /// Initializes the locator with it's view model object that recieves the UI's user control & it's proxy
        /// </summary>
        /// <param name="control">the UI's user control</param>
        /// <param name="proxy">the UI's proxy</param>
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
