using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CT.Common.DTO_Models
{
    public class FlightDTO : INotifyPropertyChanged
    {
        int flightSerial;
        bool isAlive;
        int processId;
        int checkpointId;

        public int FlightId { get; set; }
        public int FlightSerial
        {
            get
            {
                return flightSerial;
            }
            set
            {
                flightSerial = value;
                RaisePropertyChanged("FlightSerial");
            }
        }
        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;
                RaisePropertyChanged("IsAlive");
            }
        }
        public int ProcessId
        {
            get
            {
                return processId;
            }
            set
            {
                processId = value;
                RaisePropertyChanged("ProcessId");
            }
        }
        public int CheckpointId
        {
            get
            {
                return checkpointId;
            }
            set
            {
                checkpointId = value;
                RaisePropertyChanged("CheckpointId");
            }
        }
        public virtual CheckpointDTO Checkpoint { get; set; }
        public virtual ProcessDTO Process { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
