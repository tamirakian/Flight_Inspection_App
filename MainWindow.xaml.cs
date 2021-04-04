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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Control currentUser;
        private MediaPlayerController mpc;
        private DataModel dm;
        public MainWindow()
        {
            InitializeComponent();
            dm = new DataModel();
            mpc = new MediaPlayerController();
            currentUser = mpc;
            myStack.Children.Add(currentUser);
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            myStack.Children.Clear();
            if(currentUser == mpc)
            {
                myStack.Children.Add(currentUser); 
            }
        }
    }
}
