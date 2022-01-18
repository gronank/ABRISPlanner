using ABRISPlanner.Model;
using ABRISPlanner.ViewModel;
using Carmenta.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABRISPlanner.Map
{
    class AddWaypointsTool : IResetableTool
    {
        private readonly ITool BaseTool;
        private readonly RouteViewModel Route;
        private View View;
        private WaypointViewModel ActiveWaypoint;
        public AddWaypointsTool(RouteViewModel route)
        {
            Route = route;
            BaseTool = new StandardTool();
        }

        public event Action Reset;

        public bool IsActive() => BaseTool.IsActive();

        public void OnConnect(object control, View view)
        {
            BaseTool.OnConnect(control, view);
            View = view;
        }

        public void OnDisconnect()
        {
            BaseTool.OnDisconnect();
        }

        public bool OnKeyDown(int key, bool shift, bool ctrl, bool alt)=>BaseTool.OnKeyDown(key, shift, ctrl, alt);


        public bool OnKeyUp(int key, bool shift, bool ctrl, bool alt) => BaseTool.OnKeyUp(key, shift, ctrl, alt);

        public bool OnMouseDoubleClick(MouseButtons button, int x, int y, bool shift, bool ctrl, bool alt)
        {
            if (ActiveWaypoint == null) return true;
            Route.Waypoints.Remove(ActiveWaypoint);
            Reset?.Invoke();
            return true;
        }

        public bool OnMouseDown(MouseButtons button, int x, int y, bool shift, bool ctrl, bool alt)
        {
            if(ActiveWaypoint==null)
                AddWaypoint(x, y);
            AddWaypoint(x, y);
            return true;
        }
        private void AddWaypoint(int x,int y)
        {
            var location = Parse(x, y);
            var wp = new Waypoint();
            wp.Name = "<unnamed>";
            wp.Longitude = location.X;
            wp.Latitude = location.Y;
            ActiveWaypoint = new WaypointViewModel(wp, Route);
            Route.AddWayPoint(ActiveWaypoint);
        }

        public bool OnMouseMove(MouseButtons button, int x, int y, bool shift, bool ctrl, bool alt)
        {
            var location = Parse(x, y);
            ActiveWaypoint?.SetLocation(location.X, location.Y);
            return ActiveWaypoint != null;
        }

        public bool OnMouseUp(MouseButtons button, int x, int y, bool shift, bool ctrl, bool alt)
        {
            return true;
        }

        public bool OnMouseWheel(int delta, int x, int y, bool shift, bool ctrl, bool alt)
        {
            throw new NotImplementedException();
        }
        private Point Parse(int x, int y) => View.Crs.UnprojectToLongLat(View.PixelToCrs(x, y));
    }
}
