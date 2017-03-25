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
    /// <summary>
    /// The class that edits the UI's List Views items source to match the app's needs in the XAML bindings
    /// </summary>
    public class CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(System.Collections.IEnumerable))
                throw new InvalidOperationException("Target type must be System.Collections.IEnumerable");

            string defaultSerial = default(string);
            foreach (string serial in (value as ObservableCollection<string>))
            {
                if (serial == "-1")
                {
                    defaultSerial = serial;
                    break;
                }
                else return value;
            }

            (value as ObservableCollection<string>).Remove(defaultSerial);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
