using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Sockets;

namespace Flight_Inspection_App.WindowObjects
{
    /// <summary>
    /// Interaction logic for UserFeaturesWindow.xaml
    /// </summary>
    public partial class UserFeaturesWindow : Window
    {
        private FlightSimulatorViewModel fsView;

        public UserFeaturesWindow(string csvFile)
        {
            InitializeComponent();
            fsView = new FlightSimulatorViewModel(new FlightSimulator(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)));
            fsView.VM_UploadReg(csvFile);
            DataContext = fsView;
        }
    }
}
