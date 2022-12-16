using ABRISPlanner.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TacticalSimWpf.Commands;
using TacticalSimWpf.ViewModel;

namespace ABRISPlanner.ViewModel
{
    public class RouteViewModel: Changeable
    {
        public readonly Route Route;
        private readonly PlanViewModel PlanViewModel;
        public ICommand AddWaypoints => new Command(AddWaypointsCommand);
        public ICommand DeleteWaypoint => new Command(DeleteWaypointCommand, () => SelectedWaypoint != null);

        

        public ICommand DeleteRoute => new Command(DeleteRouteCommand);

        private void DeleteRouteCommand() => PlanViewModel.DeleteRoute(Route);


        public RouteViewModel(Route route,PlanViewModel planViewModel)
        {
            Route = route;
            PlanViewModel = planViewModel;
            Waypoints = new(Route.Waypoints.Select(w => new WaypointViewModel(w, this)));
            Waypoints.CollectionChanged += Waypoints_CollectionChanged;
        }

        private void Waypoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((e.NewItems?.Count??0) + (e.OldItems?.Count??0) > 1) throw new ArgumentException();
            foreach(var newItem in e.NewItems?.Cast<WaypointViewModel>() ?? Enumerable.Empty<WaypointViewModel>())
            {
                if (e.NewStartingIndex == Route.Waypoints.Count)
                    Route.Waypoints.Add(newItem.Waypoint);
                else
                    Route.Waypoints.Insert(e.NewStartingIndex, newItem.Waypoint);
            }
            foreach (var oldItem in e.OldItems?.Cast<WaypointViewModel>() ?? Enumerable.Empty<WaypointViewModel>())
            {
                Route.Waypoints.Remove(oldItem.Waypoint);
            }
        }

        public string Name { get => Route.Name; set { Route.Name = value; Changed("Name"); } }
        public ObservableCollection<WaypointViewModel> Waypoints { get; }
        public WaypointViewModel SelectedWaypoint_;
        public WaypointViewModel SelectedWaypoint { get => SelectedWaypoint_; set { SelectedWaypoint_ = value; Changed("SelectedWaypoint"); } }
        private void AddWaypointsCommand()
        {
            PlanViewModel.MapControl.AddWaypointsToRoute(this, SelectedIndex);
        }

        private void DeleteWaypointCommand()
        {
            int index = SelectedIndex;
            Waypoints.Remove(SelectedWaypoint);
            index = Math.Min(index, Route.Waypoints.Count - 1);
            if(index>-1)
                SelectedWaypoint = Waypoints[index];
        }

        internal void MakeSelected(bool force = true)
        {
            if(force || PlanViewModel.SelectedRoute!=this)
            {
                PlanViewModel.SelectedRoute = this;
            }
        }

        public void AddWayPoint(WaypointViewModel w)
        {
            var index = SelectedIndex + 1;
            if (index < Route.Waypoints.Count)
            {
                Waypoints.Insert(index, w);
                Waypoints[index].MakeSelected();
            }
            else
            {
                Waypoints.Add(w);
            }
        }
        private int SelectedIndex => SelectedWaypoint==null ? Route.Waypoints.Count: Route.Waypoints.IndexOf(SelectedWaypoint.Waypoint);
        public int Index => PlanViewModel.Routes.IndexOf(this);
    }
}
