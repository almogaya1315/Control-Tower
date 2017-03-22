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
            if (targetType != typeof(ObservableCollection<FlightDTO>))
                throw new InvalidOperationException("Target type must be System.Collections.ObjectModel.ObservableCollection<FlightDTO>");

            if ((value as ObservableCollection<FlightDTO>).FirstOrDefault().FlightSerial == -1)
                return new ObservableCollection<FlightDTO>();
            else return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
