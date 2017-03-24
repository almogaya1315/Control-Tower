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
using System.Timers;

namespace CT.UI.Views
{
    /// <summary>
    /// The core window of the application
    /// </summary>
    public partial class MainWindow : Window
    {
        AirportUserControl airportUserControl { get; set; }

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                throw new Exception($"Main window did not initialize. {e.Message}");
            }
            finally
            {
                Content = new AirportUserControl(this);
            }
        }
    }
}
