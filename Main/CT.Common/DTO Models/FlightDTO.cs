using CT.Common.Utilities;
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
    /// <summary>
    /// The model class that comunicates with the UI's bindings
    /// </summary>
    public class FlightDTO : INotifyPropertyChanged
    {
        #region props
        bool isAlive;
        int flightSerial;
        int processId;
        int checkpointId;
        string checkpointControl;
        string planeImgPath;

        public int FlightId { get; set; }
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
        public string CheckpointControl
        {
            get
            {
                return checkpointControl;
            }
            set
            {
                checkpointControl = value;
                RaisePropertyChanged("CheckpointControl");
            }
        }
        public string PlaneImgPath
        {
            get
            {
                return planeImgPath;
            }
            set
            {
                planeImgPath = value;
                RaisePropertyChanged("PlaneImgPath");
            }
        }

        public virtual CheckpointDTO Checkpoint { get; set; }
        public virtual ProcessDTO Process { get; set; }
        #endregion

        #region interface data
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
