﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Flight_Inspection_App.Models
{
    class featureModel : INotifyPropertyChanged
    {
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

        public void startCommunicatingWithServer() {
            this.client = new TcpClient("127.0.0.1", 5800);
            sendLine("hello from client");
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
