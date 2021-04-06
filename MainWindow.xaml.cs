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
using Flight_Inspection_App.ViewModels;
using Flight_Inspection_App.Models;

namespace Flight_Inspection_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel(new MainWindowModel(null, null));
        }

        private void ImportBTn_Click(object sender, RoutedEventArgs e)
        {
            UploadFileWindow uploadWin = new UploadFileWindow();
            uploadWin.ShowDialog();
            vm.VM_CSVFile = uploadWin.FileName;
        }

        private void StartBTn_Click(object sender, RoutedEventArgs e)
        {
            if (vm.VM_FGLocation == null)
            {
                UploadFlightGearLocationWindow uploadFGWin = new UploadFlightGearLocationWindow();
                uploadFGWin.ShowDialog();
                vm.VM_FGLocation = uploadFGWin.FileName;
            }
            else
            {
                Process.Start(vm.VM_FGLocation);
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
