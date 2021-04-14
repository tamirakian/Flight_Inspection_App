using System;
using System.Windows;

namespace Flight_Inspection_App.WindowObjects
{
    /// <summary>
    /// Interaction logic for UploadFlightGearLocationWindow.xaml
    /// </summary>
    public partial class UploadDLL : Window
    {
        private Microsoft.Win32.OpenFileDialog dlg;
        private string filename;

        public UploadDLL()
        {
            InitializeComponent();
            // Create OpenFileDialog
            dlg = new Microsoft.Win32.OpenFileDialog();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".dll";
            dlg.Filter = "dll files (*.dll) |*.dll";

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