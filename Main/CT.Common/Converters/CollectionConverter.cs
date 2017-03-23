using CT.Common.DTO_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CT.Common.Converters
{
    public class CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(System.Collections.IEnumerable))
                throw new InvalidOperationException("Target type must be System.Collections.IEnumerable");

            FlightDTO controlObject = null;
            foreach (FlightDTO flight in (value as ObservableCollection<FlightDTO>))
            {
                if (flight.FlightSerial == -1)
                {
                    controlObject = flight;
                    break;
                }
                else return value;
            }

            (value as ObservableCollection<FlightDTO>).Remove(controlObject);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
