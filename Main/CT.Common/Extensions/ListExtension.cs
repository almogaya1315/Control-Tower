using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.Extensions
{
    /// <summary>
    /// Extension for the IEnumerable<T> object to enbale Observable bindings object
    /// </summary>
    public static class ListExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            var oc = new ObservableCollection<T>();
            foreach (var t in collection)
                oc.Add(t);
            return oc;
        } 
    }
}
