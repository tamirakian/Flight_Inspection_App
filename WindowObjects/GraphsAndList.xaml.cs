using Flight_Inspection_App.ViewModels;
using System.Windows.Controls;

namespace Flight_Inspection_App.WindowObjects
{
    /// <summary>
    /// Interaction logic for GraphsAndList.xaml
    /// </summary>
    public partial class GraphsAndList : UserControl
    {
        private GraphsAndListViewModel ViewModel;
        public GraphsAndList()
        {
            InitializeComponent();
            ViewModel = new GraphsAndListViewModel();
            DataContext = ViewModel;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlightVariables.SelectedItem != null)
            {
                ListBoxItem lbItem = (FlightVariables.SelectedItem as ListBoxItem);
                StackPanel sp = lbItem.Content as StackPanel;
                ViewModel.VM_DesiredModel = (sp.Children[0] as TextBlock).Text;
            }
        }
    }
}
