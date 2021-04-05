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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using Flight_Inspection_App.WindowObjects;
using System.Diagnostics;

namespace Flight_Inspection_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FlightSimulatorViewModel fsView;
        private string fgLocation;

        public MainWindow()
        {
            InitializeComponent();
            fsView = new FlightSimulatorViewModel(new FlightSimulator(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)));
            DataContext = fsView;
        }

        private void ImportBTn_Click(object sender, RoutedEventArgs e)
        {
            UploadFileWindow uploadWin = new UploadFileWindow();
            uploadWin.ShowDialog();
            fsView.VM_UploadReg(uploadWin.FileName);
        }

        private void StartBTn_Click(object sender, RoutedEventArgs e)
        {
            if (fgLocation == null)
            {
                UploadFlightGearLocationWindow uploadFGWin = new UploadFlightGearLocationWindow();
                uploadFGWin.ShowDialog();
                fgLocation = uploadFGWin.FileName;
            }
            else
            {
                Process.Start(fgLocation);
                UserFeaturesWindow featureWin = new UserFeaturesWindow();
                featureWin.Show();
                this.Close();
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
