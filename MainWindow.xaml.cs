using Flight_Inspection_App.ViewModels;
using Flight_Inspection_App.WindowObjects;
using System.Diagnostics;
using System.Windows;

namespace Flight_Inspection_App
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel();
        }

        private void ImportBTn_Click(object sender, RoutedEventArgs e)
        {
            UploadFileWindow uploadWin = new UploadFileWindow();
            uploadWin.ShowDialog();
            vm.VM_CSVFile = uploadWin.CSVFileName;
            vm.VM_AnomalyFlight = uploadWin.Anomalyfilename;
            vm.VM_Settings = uploadWin.Settingsfilename;
        }

       
        private void ImportDLLBTn_Click(object sender, RoutedEventArgs e)
        {
            UploadDLL uploadWin = new UploadDLL();
            uploadWin.ShowDialog();
            vm.VM_AlgorithmDLL = uploadWin.FileName;
        }

        private void StartBTn_Click(object sender, RoutedEventArgs e)
        {
            if (vm.VM_FGLocation == null)
            {
                UploadFlightGearLocationWindow uploadFGWin = new UploadFlightGearLocationWindow();
                uploadFGWin.ShowDialog();
                vm.VM_FGLocation = uploadFGWin.FileName;
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "CMD.exe";
                startInfo.Arguments = "/C \"" + vm.VM_FGLocation + "\" " + "--launcher";
                process.StartInfo = startInfo;
                process.Start();
                UserFeaturesWindow featureWin = new UserFeaturesWindow(vm.VM_CSVFile);
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
