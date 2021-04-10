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
                ViewModel.PointsTopRightGraph = (lbTodoList.SelectedItem as TodoItem).Title;
        }
    }
}
