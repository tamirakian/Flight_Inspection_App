using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Flight_Inspection_App.WindowObjects
{
    /// <summary>
    /// Interaction logic for UserFeaturesWindow.xaml
    /// </summary>
    public partial class UserFeaturesWindow : Window
    {
        private FlightSimulatorViewModel fsView;
        private Dictionary<string, Boolean> flags;
        private string playSpeed;

        bool LoadInputNumber(string str)
        {
            float f;
            if (float.TryParse(str, out f))
            {
                return true;
            }
            return false;
        }

        public UserFeaturesWindow(string csvFile)
        {
            InitializeComponent();
            fsView = new FlightSimulatorViewModel(new FlightSimulator());
            fsView.VM_UploadReg(csvFile);
            DataContext = fsView;
            flags = fsView.VM_Flags;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (userSpeedInput.Text != "")
            {
                fsView.VM_Speed = float.Parse(userSpeedInput.Text, CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            UploadFileWindow uploadWin = new UploadFileWindow();
            uploadWin.ShowDialog();
            fsView.VM_UploadReg(uploadWin.FileName);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            bool start = false;
            if (fsView.VM_Flags["Start"])
            {
                start = true;
            }
            foreach (string butn in flags.Keys.ToList())
            {
                flags[butn] = false;
            }
            flags["Play"] = true;
            if (start)
            {
                flags["Start"] = true;
            }
            fsView.VM_Flags = flags;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys.ToList())
            {
                flags[butn] = false;
            }
            flags["Stop"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys.ToList())
            {
                flags[butn] = false;
            }
            flags["Begin"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys.ToList())
            {
                flags[butn] = false;
            }
            flags["Pause"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys.ToList())
            {
                flags[butn] = false;
            }
            flags["End"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys.ToList())
            {
                flags[butn] = false;
            }
            flags["Forward"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnRewind_Click(object sender, RoutedEventArgs e)
        {
            foreach (string butn in flags.Keys.ToList())
            {
                flags[butn] = false;
            }
            flags["Rewind"] = true;
            fsView.VM_Flags = flags;
        }

        private void btnSpeed_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
