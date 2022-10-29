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
using System.Text.RegularExpressions;

namespace Flight_Inspection_App
{
    // Interaction logic for MediaPlayerController.xaml
    public partial class MediaPlayerController : UserControl
    {
        private MediaPlayerViewModel mpvm;
        string V_CSVFile;
        string V_curTime;

        public MediaPlayerController()
        {
            this.mpvm = new MediaPlayerViewModel(new Models.MediaPlayerModel(V_CSVFile , V_curTime));
            InitializeComponent();
        }

        // checks if the input given by the user is a decimal number.
        private void NumberDecimalValidationTextbox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e) {}
    }
}
