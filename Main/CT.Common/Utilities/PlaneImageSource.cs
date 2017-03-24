using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CT.Common.Utilities
{
    public static class PlaneImageSource
    {
        public static BitmapImage PlaneLeft
        {
            get
            {
                return new BitmapImage(new Uri(@"E:\TFS_Code\ControlTower\Main\CT.UI\Images\planeleft.png"));
            }
        }

        public static BitmapImage PlaneDown
        {
            get
            {
                return new BitmapImage(new Uri(@"E:\TFS_Code\ControlTower\Main\CT.UI\Images\planedown.png"));
            }
        }

        public static BitmapImage NoPlane
        {
            get
            {
                return new BitmapImage();
            }
        }
    }
}
