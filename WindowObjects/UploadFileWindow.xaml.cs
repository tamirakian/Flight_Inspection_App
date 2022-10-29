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
    /// Interaction logic for UploadFileWindow.xaml
    public partial class UploadFileWindow : Window
    {
        private Microsoft.Win32.OpenFileDialog dlg;
        private string CSVfilename;
        private string anomalyfilename;
        private string settingsfilename;

        public UploadFileWindow()
        {
            InitializeComponent();
            // Create OpenFileDialog
            dlg = new Microsoft.Win32.OpenFileDialog();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (*.csv)|*.csv";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                CSVfilename = dlg.FileName;
                FilePath.Text = CSVfilename;
            }
        }

        private void BtnUpload_Click2(object sender, RoutedEventArgs e)
        {
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (*.csv)|*.csv";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                anomalyfilename = dlg.FileName;
                FilePath2.Text = anomalyfilename;
            }
        }

        private void BtnUpload_Click3(object sender, RoutedEventArgs e)
        {
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                settingsfilename = dlg.FileName;
                FilePath3.Text = settingsfilename;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string CSVFileName
        {
            get { return CSVfilename; }
        }

        public string Anomalyfilename
        {
            get { return anomalyfilename; }
        }

        public string Settingsfilename
        {
            get { return settingsfilename; }
        }
    }
}
