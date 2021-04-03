using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace Flight_Inspection_App
{
    static class Constants
    {
        public const string HOST_IP = "localhost";
        public const int HOST_PORT = 5400;
    }

    // the model
    class FlightSimulator : FlightSimulatorModel
    {
        // the reg_flight csv file name
        private string regFlight;
        // the xml file name
        private string settings;
        
        public event PropertyChangedEventHandler PropertyChanged;

        // the flight simulator socket
        Socket fg;
        volatile Boolean stop;

        // constructor - initializing the flight gear socket, and setting the stop value to false
        public FlightSimulator(Socket fg)
        {
            this.fg = fg;
            // indicating that the socket is running.
            stop = false;
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        // getter & setter for the csv file ****need to improve
        public string regFlightFile
        {
            get { return regFlight; }
            set { regFlight = value;
                NotifyPropertyChanged("regFlightFile");
            }
        }

        // getter & setter for the xml file ****need to improve
        public string settingsFile
        {
            get { return settings; }
            set
            {
                settings = value;
                NotifyPropertyChanged("settingsFile");
            }
        }

        // connect to the flight gear socket.
        public void connect(string ip, int port)
        {
            fg.Connect(ip, port);
        }

        // disconnecting from the flight gear socket.
        public void disconnect()
        {
            // indicating that the socket is closed.
            stop = true;
            fg.Disconnect(false);
        }

        // ****need to change
        public void start()
        {
            this.connect(Constants.HOST_IP, Constants.HOST_PORT);
            StreamReader reader = new StreamReader(regFlightFile);
            StreamWriter writer = new StreamWriter(fg. );
            new Thread(delegate ()
            {  
                while (!stop)
                {
                    fg.Send()
                }
            })
        }
    }
}
