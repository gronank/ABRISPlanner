using ABRISPlanner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TacticalSimWpf.ViewModel;
using System.Windows.Input;
using TacticalSimWpf.Commands;
using System.Collections.ObjectModel;

namespace ABRISPlanner.ViewModel
{
    public class PlanViewModel : Changeable
    {
        private readonly Plan Plan;

        private RouteViewModel SelectedRoute_;
        public ICommand AddRoute => new Command(AddRouteCommand);
        public ICommand UpdateSituation => new Command(UpdateSituationCommand);

        public ObservableCollection<RouteViewModel> Routes { get; }
        public SituationViewModel Situation { get; }
        public RouteViewModel SelectedRoute { get => SelectedRoute_; set { SelectedRoute_ = value; Changed("SelectedRoute"); } }
        public MapViewModel MapControl { get; }
        public int RouteIndex { get; set; }
        public PlanViewModel(Plan plan)
        {
            Plan = plan;
            Routes = new ObservableCollection<RouteViewModel>(Plan.Routes.Select(r => new RouteViewModel(r, this)));
            MapControl = new MapViewModel(this);
        }
        private void AddRouteCommand()
        {
            var newRoute = new Route() { Name = "New route" };
            Plan.Routes.Add(newRoute);
            Routes.Add(new RouteViewModel(newRoute, this));
            //Changed("Routes");
            SelectLastRoute();
        }
        public void DeleteRoute(Route route)
        {
            Plan.Routes.Remove(route);
            var vm = Routes.First(rVm => rVm.Route == route);
            Routes.Remove(vm);
            SelectLastRoute();
        }
        private void SelectLastRoute()
        {
            RouteIndex = Plan.Routes.Count - 1;
            Changed("RouteIndex");
        }
        private void UpdateSituationCommand() => Situation.Update();
    }
}
