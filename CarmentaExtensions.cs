using ABRISPlanner.Model;
using ABRISPlanner.ViewModel;
using Carmenta.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABRISPlanner
{
    using CE = Carmenta.Engine;
    public static class CarmentaExtensions
    {
        public static IEnumerable<T> OfType<T>(this CE.Configuration config) => config.PublicNames.Select(n => config.GetPublicObject(n)).OfType<T>();
        public static T? FirstOfType<T>(this CE.Configuration config) => config.OfType<T>().FirstOrDefault();
        public static Point ToPoint(this Waypoint wp) => new Point(wp.Longitude, wp.Latitude);

    }
}
