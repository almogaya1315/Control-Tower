using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CT.Common.Utilities;
using System.Timers;
using System.ComponentModel;

namespace CT.Simulator
{
    public class ArrivalSim
    {
        int randomSerial { get; set; }

        public Timer ArrivalTimer
        {
            get
            {
                return GlobalValues.GlobalTimer;
            }
            set { }
        }

        public ArrivalSim()
        {
            randomSerial = default(int);
        }

        public int CreateFlightSerial()
        {
            randomSerial = GlobalValues.GlobalRandom.Next(100, 1000);
            return randomSerial;
        }
    }
}
