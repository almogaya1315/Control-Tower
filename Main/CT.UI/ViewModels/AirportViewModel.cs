using CT.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CT.UI.ViewModels
{
    public class AirportViewModel
    {
        public ICommand AddFlightCommand { get; set; }

        public AirportViewModel()
        {
            AddFlightCommand = new AddFlightCommand(AddFlight);
        }

        public void AddFlight()
        {

        }
    }
}
