using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Flight_Inspection_App.ViewModels;

namespace Flight_Inspection_App.WindowObjects
{
    /// Interaction logic for UserFeaturesWindow.xaml
    public partial class UserFeaturesWindow : Window
    {
        private FlightSimulatorViewModel fsView;
        private Dictionary<string, Boolean> flags;
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private GraphsAndList graphsAndList;

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
            FlightSimulator FSmodel = FlightSimulator.ModelInstance;
            fsView = new FlightSimulatorViewModel();
            fsView.VM_UploadReg(csvFile);
            DataContext = fsView;
            flags = fsView.VM_Flags;
            graphsAndList = new GraphsAndList();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
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
            UploadDLL uploadWin = new UploadDLL();
            uploadWin.ShowDialog();
            fsView.VM_AlgorithmDLL = uploadWin.FileName;
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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!userIsDraggingSlider)
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = fsView.VM_GetFlightLen();
                sliProgress.Value = fsView.VM_TimeInDeci;
            }
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            int timeBefore = fsView.VM_TimeInDeci;
            userIsDraggingSlider = false;
            fsView.VM_TimeInDeci = (int)TimeSpan.FromSeconds(sliProgress.Value).TotalMilliseconds / 1000;
            if(timeBefore > fsView.VM_TimeInDeci)
            {
                fsView.VM_PointsTopRightGraph.Clear();
                fsView.VM_PointsTopLeftGraph.Clear();
            }
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }
    }
}
