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

namespace Flight_Inspection_App.Controllers
{
    /// <summary>
    /// Interaction logic for ImportFileController.xaml
    /// </summary>
    public partial class ImportFileController : UserControl
    {
        private Microsoft.Win32.OpenFileDialog dlg;
        private string filename;
        private FlightSimulatorModel fs;
        private UserControl control;

        public ImportFileController()
        {
            InitializeComponent();
            // Create OpenFileDialog
            dlg = new Microsoft.Win32.OpenFileDialog();
            fs = new FlightSimulator(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        }

        public ImportFileController(UserControl control) : this()
        {
            this.control = control;
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
             
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (*.csv)|*.csv";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
                FilePath.Text = filename;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(FilePath.Text != "")
            {
                fs.regFlightFile = filename;
                return;
            }
            else
            {
                MessageBox.Show("Please chose a path before submitting");
            }
        }
    }
}
