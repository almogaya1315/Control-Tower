using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CT.Common.Abstracts
{
    public abstract class ControlInitializer
    {
        ICollection<TextBlock> InitializeTxtblckCheckpoints(ICollection<TextBlock> txtblckCheckpoints)
        {
            return txtblckCheckpoints = new List<TextBlock>()
            {
                txtblckFlightArr1, txtblckFlightArr2, txtblckFlightArr3,
                txtblckFlightRunway, txtblckFlightTerminal1,
                txtblckFlightTerminal2, txtblckFlightDepart
            };
        }
        ICollection<ListView> InitializeLstvwsCheckpoints(ICollection<ListView> lstvwsCheckpoints)
        {
            return lstvwsCheckpoints = new List<ListView>()
            {
                lstvwParkUnload, lstvwParkDepart
            };
        }
        ICollection<Image> InitializeImgPlanes(ICollection<Image> imgPlanes)
        {
            return imgPlanes = new List<Image>()
            {
                imgPlaneArr1, imgPlaneArr2, imgPlaneArr3,
                imgPlaneRunway, imgPlaneTerminal1,
                imgPlaneTerminal2, imgPlanDepart
            };
        }
    }
}
