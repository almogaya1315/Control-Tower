using CT.Common.DTO_Models;
using CT.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CT.Common.Abstracts
{
    public abstract class ControlInitializer : CTBindingData
    {
        public ControlInitializer() : base() { }

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

        #region switchers
        public bool? SwitchOnNextCheckpointName(string nextCheckpointName, FlightDTO flight)
        {
            return null;

            bool? isFound = default(bool);
            switch (nextCheckpointName)
            {
                case "lstvwParkUnload":
                    FlightsInStandbyForUnloading.Add(flight);
                    FlightInRunway = InitializeFlightBindingObject();
                    //lstvwParkUnload.Items.Add(flight.FlightSerial.ToString());
                    //txtblckFlightRunway.Text = "---";
                    //imgPlaneRunway.Source = PlaneImageSource.NoPlane;
                    return isFound = true;
                case "txtblckFlightDepart":
                    FlightInDeparted = InitializeFlightBindingObject(flight);
                    FlightInRunway = InitializeFlightBindingObject();
                    //txtblckFlightDepart.Text = flight.FlightSerial.ToString();
                    //imgPlanDepart.Source = PlaneImageSource.PlaneLeft;
                    //txtblckFlightRunway.Text = "---";
                    //imgPlaneRunway.Source = PlaneImageSource.NoPlane;
                    return isFound = true;
                case "Stay in checkpoint!":
                    return isFound = null;
                case "Departed!":
                    SimProxy.OnDispose(flight.FlightSerial);
                    return isFound = null;
                case "No access to field!":
                    SimProxy.OnDispose(flight.FlightSerial);
                    return isFound = null;
            }
            return isFound = false;
        }
        public void SwitchOnCheckpointSerial(int checkpointSerial, string checkpointType,
            string nextCheckpointName, string lastCheckpointPosition, FlightDTO flight)
        {
            switch (checkpointSerial)
            {
                case 1:
                    imgPlaneArr1.Source = PlaneImageSource.PlaneLeft;
                    txtblckFlightArr1.Text = flight.FlightSerial.ToString();
                    break;
                case 2:
                    imgPlaneArr1.Source = PlaneImageSource.NoPlane;
                    txtblckFlightArr1.Text = "---";
                    imgPlaneArr2.Source = PlaneImageSource.PlaneLeft;
                    txtblckFlightArr2.Text = flight.FlightSerial.ToString();
                    break;
                case 3:
                    imgPlaneArr2.Source = PlaneImageSource.NoPlane;
                    txtblckFlightArr2.Text = "---";
                    imgPlaneArr3.Source = PlaneImageSource.PlaneLeft;
                    txtblckFlightArr3.Text = flight.FlightSerial.ToString();
                    break;
                case 4:
                    if (checkpointType == "RunwayLanded")
                    {
                        imgPlaneArr3.Source = PlaneImageSource.NoPlane;
                        txtblckFlightArr3.Text = "---";
                        imgPlaneRunway.Source = PlaneImageSource.PlaneLeft;
                        txtblckFlightRunway.Text = flight.FlightSerial.ToString();
                    }
                    else if (checkpointType == "RunwayDeparting")
                    {
                        lstvwParkDepart.Items.Remove(flight.FlightSerial.ToString());
                        imgPlaneRunway.Source = PlaneImageSource.PlaneLeft;
                        txtblckFlightRunway.Text = flight.FlightSerial.ToString();
                    }
                    break;
                case 6:
                    if (nextCheckpointName == "FlightTerminal1")
                    {
                        lstvwParkUnload.Items.Remove(flight.FlightSerial.ToString());
                        txtblckFlightTerminal1.Text = flight.FlightSerial.ToString();
                        imgPlaneTerminal1.Source = PlaneImageSource.PlaneDown;
                        txtblckTerminal1Message.Text = "Unloading...";
                    }
                    if (nextCheckpointName == "FlightTerminal2")
                    {
                        lstvwParkUnload.Items.Remove(flight.FlightSerial.ToString());
                        txtblckFlightTerminal2.Text = flight.FlightSerial.ToString();
                        imgPlaneTerminal2.Source = PlaneImageSource.PlaneDown;
                        txtblckTerminal2Message.Text = "Unloading...";
                    }
                    break;
                case 7:
                    if (nextCheckpointName == "txtblckFlightTerminal1")
                        txtblckTerminal1Message.Text = "...Boarding";
                    if (nextCheckpointName == "txtblckFlightTerminal2")
                        txtblckTerminal2Message.Text = "...Boarding";
                    break;
                case 8:
                    if (lastCheckpointPosition == "txtblckFlightTerminal1")
                    {
                        txtblckFlightTerminal1.Text = "---";
                        imgPlaneTerminal1.Source = PlaneImageSource.NoPlane;
                        txtblckTerminal1Message.Text = string.Empty;
                        lstvwParkDepart.Items.Add(flight.FlightSerial.ToString());
                    }
                    if (lastCheckpointPosition == "txtblckFlightTerminal2")
                    {
                        txtblckFlightTerminal2.Text = "---";
                        imgPlaneTerminal2.Source = PlaneImageSource.NoPlane;
                        txtblckTerminal2Message.Text = string.Empty;
                        lstvwParkDepart.Items.Add(flight.FlightSerial.ToString());
                    }
                    break;
            }
        }
        #endregion
    }
}
