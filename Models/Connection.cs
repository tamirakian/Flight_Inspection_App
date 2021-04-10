using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Flight_Inspection_App.Models
{
    class Connection
    {
        // the flight simulator socket
        Socket fg;
        volatile Boolean stop;

        public Connection(Socket fg)
        {
            this.fg = fg;
            stop = false;
        }

        // connect to the flight gear socket.
        public void connect(string ip, int port)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            IPAddress ipAddr = ipHost.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
            fg = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                fg.Connect(localEndPoint);
            }
            // improve
            catch (Exception e)
            {
                return;
            }
        }

        // disconnecting from the flight gear socket.
        public void disconnect()
        {
            // indicating that the socket is closed.
            stop = true;
            fg.Disconnect(false);
        }
    }
}
