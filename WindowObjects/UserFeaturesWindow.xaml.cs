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
        private Dictionary<string, Boolean> flags;

        public UserFeaturesWindow(string csvFile)
        {
            InitializeComponent();
            fsView = new FlightSimulatorViewModel(new FlightSimulator(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)));
            fsView.VM_UploadReg(csvFile);
            DataContext = fsView;
            flags = fsView.VM_Flags;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            UploadFileWindow uploadWin = new UploadFileWindow();
            uploadWin.ShowDialog();
            fsView.VM_UploadReg(uploadWin.FileName);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            foreach(string butn in flags.Keys)
            {
                flags[butn] = false;
            }
            flags["Play"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys)
            {
                flags[butn] = false;
            }
            flags["Stop"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys)
            {
                flags[butn] = false;
            }
            flags["Begin"] = true;
            fsView.VM_Flags = flags;
        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys)
            {
                flags[butn] = false;
            }
            flags["Pause"] = true;
            fsView.VM_Flags = flags;
        }
        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys)
            {
                flags[butn] = false;
            }
            flags["End"] = true;
            fsView.VM_Flags = flags;
        }
        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys)
            {
                flags[butn] = false;
            }
            flags["Forward"] = true;
            fsView.VM_Flags = flags;
        }
        private void btnRewind_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys)
            {
                flags[butn] = false;
            }
            flags["Rewind"] = true;
            fsView.VM_Flags = flags;
        }
    }
}
