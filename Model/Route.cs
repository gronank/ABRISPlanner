using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABRISPlanner.Model
{
    public class Route
    {
        public string Name;
        public List<Waypoint> Waypoints = new();
    }
}
