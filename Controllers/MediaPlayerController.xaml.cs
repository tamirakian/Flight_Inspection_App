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
    /// <summary>
    /// Interaction logic for MediaPlayerController.xaml
    /// </summary>
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
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
    }
}
