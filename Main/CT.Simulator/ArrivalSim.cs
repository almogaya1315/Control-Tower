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
    /// <summary>
    /// Class responsible for the initial flight creation timer & the random flight serial 
    /// </summary>
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

        /// <summary>
        /// Creates a random 3 digits serial per flight creation
        /// </summary>
        /// <returns>the random serial</returns>
        public int CreateFlightSerial()
        {
            randomSerial = GlobalValues.GlobalRandom.Next(100, 1000);
            return randomSerial;
        }
    }
}
