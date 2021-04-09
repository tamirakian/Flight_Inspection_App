using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Flight_Inspection_App.Models
{
    class featureModel : INotifyPropertyChanged
    {
        [DllImport("anomalyProject.dll")]
        public static extern IntPtr CreateTimeSeries(string str);

        [DllImport("anomalyProject.dll")]
        public static extern IntPtr Create(IntPtr str);

        [DllImport("anomalyProject.dll")]
        public static extern void learnNormalExtern(string str);

        public event PropertyChangedEventHandler PropertyChanged;
        private string trainCsvFile;
        private string testCsvFile;
        private string dllFile;
        private TcpClient client;

        public featureModel()
        {
            this.trainCsvFile = "reg_flight.csv";
            this.testCsvFile = "reg_flight.csv";
        }

        public void startCommunicatingWithServer()
        {
            //this.client = new TcpClient("127.0.0.1", 5800);
            //sendLine("hello from client");
            IntPtr detector = Create(CreateTimeSeries(trainCsvFile));
        }

        public void sendLine(string line)
        {
            //if we havnt connected yet we dont send nothing
            if (!client.Connected)
            {
                return;
            }

            line = line + "\n\r";
            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] data = System.Text.Encoding.ASCII.GetBytes(line);

            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);
        }
    }
}
