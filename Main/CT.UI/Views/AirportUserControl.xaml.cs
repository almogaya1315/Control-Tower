using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CT.UI.Proxy;
using System.Timers;
using System.ComponentModel;
using CT.UI.SimulatorServiceReference;
using CT.Common.Utilities;
using System.Threading;
using CT.Common.DTO_Models;
using CT.UI.Locators;

namespace CT.UI.Views
{
    /// <summary>
    /// The main user control with service & binding data
    /// </summary>
    public partial class AirportUserControl : UserControl
    {
        public AirportUserControl(MainWindow core)
        {
            InitializeComponent();
            SimServiceProxy SimProxy = new SimServiceProxy();
            ViewModelLocator locator = new ViewModelLocator(this, SimProxy);
            DataContext = locator.AirportViewModel;
        }
    }
}
