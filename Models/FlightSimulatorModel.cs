using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Flight_Inspection_App
{
    interface FlightSimulatorModel : INotifyPropertyChanged
    {
        // connection to the simulator
        void connect(string ip, int port);
        void disconnect();
        void start();

        // helper functions.
        string FlightLen();
        int CurSampleLen();
        void UploadReg(string name);
        void RewindTime();
        void UpdateTime();

        // The flight simulator properties
        string regFlightFile { set; get; }
        string settingsFile { set; get; }
        Dictionary<string, Boolean> Flags { set; get; }
        string CurTime { set; get; }
        Boolean Stop { set; get; }
    }
}
