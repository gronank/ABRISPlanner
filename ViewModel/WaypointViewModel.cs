using ABRISPlanner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacticalSimWpf.ViewModel;

namespace ABRISPlanner.ViewModel
{
    public class WaypointViewModel:Changeable
    {
        public readonly Waypoint Waypoint;
        public readonly RouteViewModel RouteViewModel;

        public WaypointViewModel(Waypoint waypoint, RouteViewModel routeViewModel)
        {
            Waypoint = waypoint;
            RouteViewModel = routeViewModel;
        }

        public string Name { get => Waypoint.Name; set { Waypoint.Name = value; Changed("Name"); } }
        public void SetLocation(double longitude, double latitude)
        {
            Waypoint.Longitude = longitude;
            Waypoint.Latitude = latitude;
            Changed("Location");
        }
        public int Index => RouteViewModel.Waypoints.IndexOf(this);

        internal void MakeSelected(bool force = true)
        {
            if (force || RouteViewModel.SelectedWaypoint != this)
            {
                RouteViewModel.MakeSelected(false);
                RouteViewModel.SelectedWaypoint = this;
            }
        }
    }
}
