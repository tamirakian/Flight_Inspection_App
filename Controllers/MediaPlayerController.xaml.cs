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
    }
}
