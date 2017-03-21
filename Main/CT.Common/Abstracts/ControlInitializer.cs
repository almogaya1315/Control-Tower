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
        #region initializers
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
        #endregion

        #region setters
        public Dictionary<string, string> SetTxtblckHash(ICollection<TextBlock> txtblckCheckpoints)
        {
            Dictionary<string, string> txtblckNameFlightNumberHash = new Dictionary<string, string>();
            foreach (TextBlock txtblck in txtblckCheckpoints)
                txtblckNameFlightNumberHash[txtblck.Name] = txtblck.Text;
            return txtblckNameFlightNumberHash;
        }
        public Dictionary<string, string[]> SetLstvwHash(ICollection<ListView> lstvwsCheckpoints)
        {
            Dictionary<string, string[]> lstvwNameFlightsListHash = new Dictionary<string, string[]>();
            foreach (ListView lstvw in lstvwsCheckpoints)
            {
                lstvwNameFlightsListHash[lstvw.Name] = new string[100];
                if (lstvw.Items.Count > 0)
                {
                    foreach (string lvi in lstvw.Items)
                    {
                        List<string> list = lstvwNameFlightsListHash[lstvw.Name].ToList();
                        list.RemoveAll(i => i == null);
                        list.Add(lvi);
                        lstvwNameFlightsListHash[lstvw.Name] = list.ToArray();
                    }
                }
            }
            return lstvwNameFlightsListHash;
        }
        #endregion
    }
}
