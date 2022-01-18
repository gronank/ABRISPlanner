using ABRISPlanner.Model;
using ABRISPlanner.ViewModel;
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

namespace ABRISPlanner.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Plan p = new() { Routes = new(){ new() { Name = "Route 1", Waypoints = new() { new() { Name = "wp1" }, new() { Name = "wp2" } } }, new() { Name = "Route 2" }, } };
            DataContext = new PlanViewModel(p);
        }
    }
}
