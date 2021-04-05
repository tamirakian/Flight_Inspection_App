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
    /// Interaction logic for UploadFileWindow.xaml
    /// </summary>
    public partial class UploadFileWindow : Window
    {
        private Microsoft.Win32.OpenFileDialog dlg;
        private string filename;

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
                filename = dlg.FileName;
                FilePath.Text = filename;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string FileName
        {
            get { return filename; }
        }
    }
}
