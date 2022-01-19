using Carmenta.Engine;
using System;
using System.IO;
using System.Reflection;
using TacticalSimWpf.ViewModel;
using System.Linq;
using System.Windows;
using ABRISPlanner.Map;

namespace ABRISPlanner.ViewModel
{
    using CE = Carmenta.Engine;
    public class MapViewModel: Changeable
    {
        private const string configName = "Map/world_map.px";
        public CE.Controls.MapControl MapControl { get; }
        private readonly ToolHolder ToolHolder;
        public MapViewModel(PlanViewModel plan)
        {
            MapControl = new CE.Controls.MapControl();
            var loaded = LoadView();
            MapControl.View = loaded.View;
            ToolHolder = new(new StandardTool(), MapControl);
            loaded.PlanDataSetProxy.CustomDataSet = new RoutePlanDataset(plan, MapControl.View);
            loaded.SituationDataSetProxy.CustomDataSet = new SituationDataset(plan.Situation, MapControl.View);
        }

        private static (View View, CE.CustomDataSetProxy PlanDataSetProxy, CE.CustomDataSetProxy SituationDataSetProxy) LoadView()
        {
            var config = new CE.Configuration(configName);
            var view = config.FirstOfType<View>();
            var routeDataset = config.OfType<CustomDataSetProxy>().First(ds=>ds.Name == "RoutePlanDatasetProxy");
            var situationDataset = config.OfType<CustomDataSetProxy>().First(ds=>ds.Name == "SituationDatasetProxy");
            return (view, routeDataset, situationDataset);
        }

        public void AddWaypointsToRoute(RouteViewModel route, int index)
        {
            ToolHolder.SetTool(new AddWaypointsTool(route));
        }
        public string Name { get; } = "Map view model";
    }
}