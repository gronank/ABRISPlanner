using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABRISPlanner.Model;
using ABRISPlanner.ViewModel;
using Carmenta.Engine;
namespace ABRISPlanner.Map
{
    partial class RoutePlanDataset : ICustomDataSet
    {
        private const ulong MaxWaypointCount = 10000;
        public Rectangle Bounds => Rectangle.Infinite;

        public bool CreatesReferences => false;

        public Crs Crs => Crs.Wgs84LongLat;

        public RectangleSet DataCoverage => throw new NotImplementedException();
        public ICustomDataSet Clone() => new RoutePlanDataset(Plan, View);
        public void InitNew(CustomDataSetContext context) => Context = context;
        private readonly PlanViewModel Plan;
        private CustomDataSetContext Context;
        private readonly View View;
        public RoutePlanDataset(PlanViewModel plan, View view)
        {
            View = view;
            View.SelectionChanged += OnViewSelectedChanged;
            Plan = plan;
            Plan.Routes.CollectionChanged += OnPlanChanged;
            Plan.PropertyChanged += OnSelectedChanged;
            foreach (var route in Plan.Routes)
            {
                route.Waypoints.CollectionChanged += OnRouteChanged;
                route.PropertyChanged += OnSelectedChanged;
            }
        }

        private void OnViewSelectedChanged(object sender, EventArgs e)
        {
            var selectedIds = View.GetSelectedIds().Where(id => id.DataSetId == Context.Id).ToList();
            if (selectedIds.Count == 0)
            {
                Plan.SelectedRoute = Plan.SelectedRoute;
                return;
            }
            var objectId = selectedIds.Select(id => id.FeatureId).Max();
            if (objectId < MaxWaypointCount)
            {
                RouteFromId(objectId).MakeSelected();
            }
            else
            {
                WaypointFromId(objectId).MakeSelected();
            }
        }

        private void OnSelectedChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender is PlanViewModel plan)
            {
                if (e.PropertyName != "SelectedRoute" || plan.SelectedRoute == null) return;
                var selectedId = routeId(plan.SelectedRoute);
                View.Select(selectedId, SelectMode.Replace);
            }
            else if(sender is RouteViewModel route)
            {
                if (e.PropertyName != "SelectedWaypoint") return;
                var selectedIDs = route.SelectedWaypoint==null ? new[] { routeId(route) } : new[] { routeId(route), waypointId(route.SelectedWaypoint) };
                View.Select(selectedIDs, SelectMode.Replace);
            }
        }

        public void FlushCache()
        {
        }

        public Feature GetFeature(ulong objectId)
        {
            if(objectId < MaxWaypointCount)
            {
                var route = RouteFromId(objectId);
                return MakeRoute(route);
            }
            else
            {
                var waypoint = WaypointFromId(objectId);
                return MakeWaypoint(waypoint);
            }
        }
        private RouteViewModel RouteFromId(ulong objectId)
        {
            return Plan.Routes[(int)objectId];
        }
        private WaypointViewModel WaypointFromId(ulong objectId)
        {
            var routeId = objectId / MaxWaypointCount - 1;
            var waypointId = objectId % MaxWaypointCount;
            return Plan.Routes[(int)routeId].Waypoints[(int)waypointId];
        }
        private void OnPlanChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var route in e.NewItems?.Cast<RouteViewModel>() ?? Enumerable.Empty<RouteViewModel>())
            {
                route.Waypoints.CollectionChanged += OnRouteChanged;
                route.PropertyChanged += OnSelectedChanged;
            }
            foreach (var route in e.OldItems?.Cast<RouteViewModel>()??Enumerable.Empty<RouteViewModel>())
            {
                route.Waypoints.CollectionChanged -= OnRouteChanged;
                route.PropertyChanged -= OnSelectedChanged;
            }
            Context.FireDataReady();
        }
        private void OnRouteChanged(object o, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var waypoint in e.NewItems?.Cast<WaypointViewModel>() ?? Enumerable.Empty<WaypointViewModel>())
            {
                waypoint.PropertyChanged += OnWaypointChanged;
            }
            foreach (var waypoint in e.OldItems?.Cast<WaypointViewModel>() ?? Enumerable.Empty<WaypointViewModel>())
            {
                waypoint.PropertyChanged -= OnWaypointChanged;
            }
            Context.FireDataReady();
        }
        private void OnWaypointChanged(object o, PropertyChangedEventArgs e)
        {
            Context.FireDataReady();
        }


        public IQueryResult Query(Rectangle area, Query query, UInt64Collection ids, ViewInfo info)
        {
            if (ids.Count == 0)
                ids = null;
            var features = ids?.Select(GetFeature) ?? Plan.Routes.SelectMany(BuildRouteFeatures);
            return new SimpleQuery(features.Where(query.Match));
        }
        private IEnumerable<Feature> BuildRouteFeatures(RouteViewModel route)
        {
            yield return MakeRoute(route);
            foreach (var waypoint in route.Waypoints)
            {
                yield return MakeWaypoint(waypoint);
            }
        }
        private Feature MakeRoute(RouteViewModel route)
        {
            var locations = route.Route.Waypoints.Select(wp => wp.ToPoint());
            return new Feature(new LineGeometry(locations), Crs, new(), routeId(route));
        }
        private Feature MakeWaypoint(WaypointViewModel waypoint)
        {
            var location = waypoint.Waypoint.ToPoint();
            return new Feature(new PointGeometry(location), Crs, new(), waypointId(waypoint));
        }
        private Id routeId(RouteViewModel route)
        {
            ulong routeIndex = (ulong)route.Index;
            return new Id(Context.Id, routeIndex);
        }
        private Id waypointId(WaypointViewModel waypoint)
        {
            ulong waypointIndex = (ulong)waypoint.Index;
            ulong routeIndex = (ulong)waypoint.RouteViewModel.Index;

            return new Id(Context.Id, waypointIndex + ((routeIndex + 1) * MaxWaypointCount));
        }
    }
}
