﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CT.Common.Utilities
{
    public static class GlobalValues
    {
        public static Random GlobalRandom
        {
            get
            {
                return new Random();
            }
        }

        public static Timer GlobalTimer
        {
            get
            {
                return new Timer(3000);
            }
            set { }
        }
    }
}
