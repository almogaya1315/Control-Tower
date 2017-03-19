using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CT.Common.Abstracts
{
    public abstract class ControlInitializer
    {
        public ICollection<TextBlock> InitializeTxtblckCheckpoints(UIElementCollection children, ICollection<TextBlock> txtblckCheckpoints)
        {
            foreach (UIElement element in children)
                if (element is TextBlock)
                    if ((element as TextBlock).Name.Contains("txtblckFlight"))
                        txtblckCheckpoints.Add(element as TextBlock);
            return txtblckCheckpoints;

            //return txtblckCheckpoints = new List<TextBlock>()
            //{
            //    txtblckFlightArr1, txtblckFlightArr2, txtblckFlightArr3,
            //    txtblckFlightRunway, txtblckFlightTerminal1,
            //    txtblckFlightTerminal2, txtblckFlightDepart
            //};
        }
        public ICollection<ListView> InitializeLstvwsCheckpoints(UIElementCollection children, ICollection<ListView> lstvwsCheckpoints)
        {
            foreach (UIElement element in children)
                if (element is ListView)
                    if ((element as ListView).Name.Contains("lstvwPark"))
                        lstvwsCheckpoints.Add(element as ListView);
            return lstvwsCheckpoints;

            //return lstvwsCheckpoints = new List<ListView>()
            //{
            //    lstvwParkUnload, lstvwParkDepart
            //};
        }
        public ICollection<Image> InitializeImgPlanes(UIElementCollection children, ICollection<Image> imgPlanes)
        {
            foreach (UIElement element in children)
                if (element is Image)
                    if ((element as Image).Name.Contains("imgPlane"))
                        imgPlanes.Add(element as Image);
            return imgPlanes;

            //return imgPlanes = new List<Image>()
            //{
            //    imgPlaneArr1, imgPlaneArr2, imgPlaneArr3,
            //    imgPlaneRunway, imgPlaneTerminal1,
            //    imgPlaneTerminal2, imgPlanDepart
            //};
        }
    }
}
