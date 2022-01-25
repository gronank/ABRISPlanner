using ABRISPlanner.Model;
using ABRISPlanner.ViewModel;
using Carmenta.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABRISPlanner.Map
{
    class SituationDataset : ICustomDataSet
    {
        private static ulong featureId = 0;
        private static Atom NameAtom = new("Name");
        private static Atom DesigantionAtom = new("uniqueDesignation");
        private static Atom SymbolTypeAtom = new("SymbolType");
        private static Atom SidcAtom = new("sidc");
        private static Atom DisplayPriorityAtom = new("displayPriority");
        private static Atom ColorAtom = new("color");
        private static Atom FillStyleAtom = new("fillStyle");
        public Rectangle Bounds => Rectangle.Infinite;

        public bool CreatesReferences => false;

        public Crs Crs => Crs.Wgs84LongLat;

        public RectangleSet DataCoverage => throw new NotImplementedException();
        private readonly SituationViewModel Situation;
        private readonly View View;
        private CustomDataSetContext Context;
        public ICustomDataSet Clone() => new SituationDataset(Situation, View);
        public SituationDataset(SituationViewModel situation,View view)
        {
            Situation = situation;
            Situation.PropertyChanged += OnSituationChanged;
            View = view;
        }

        private void OnSituationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Situation")
                return;
            Context.FireDataReady();
        }

        public void FlushCache() { }

        public Feature GetFeature(ulong objectId) => null;

        public void InitNew(CustomDataSetContext context)
        {
            Context = context;
        }

        public IQueryResult Query(Rectangle area, Query query, UInt64Collection ids, ViewInfo info)
        {
            IEnumerable<Feature> features = Enumerable.Empty<Feature>();
            if (query.ReadPoints)
            {
                features = features.Concat(Situation.Situation.Symbols.Select(MakeSymbol));
            }
            if (query.ReadPolygons)
            {
                features = features.Concat(Situation.Situation.Zones.Select(MakeZone));
            }

            featureId = 0;
            return new SimpleQuery(features);
        }

        private Feature MakeZone(ZoneSituationObject zone)
        {
            AttributeSet attributes = new()
            {
                [ColorAtom] = zone.Color.ToColorString(),
                [FillStyleAtom] = zone.Style
            };
            return new Feature(new PolygonGeometry(zone.Buffer.Select(p => p.ToPoint())), Crs, attributes, new Id(Context.Id, featureId++));
        }

        private Feature MakeSymbol(SymbolSituationObject s)
        {
            AttributeSet attributes = new()
            {
                [NameAtom] = GetName(s),
                [DesigantionAtom] = s.Name,
                [SymbolTypeAtom] = s.SymbolType,
                [SidcAtom] = GetSidc(s.SymbolType),
                [DisplayPriorityAtom] = GetPriority(s)
            };
            return new(new PointGeometry(s.Location.ToPoint()), Crs, attributes,new Id(Context.Id, featureId++));
        }

        private AttributeValue GetPriority(SymbolSituationObject s) =>
            s.SymbolType switch
            {
                "landmark" => 9.0,
                "farp" => 10.0,
                "airDefenceHostile" => 8.0,
                _ => 0.0
            };

        private AttributeValue GetName(SymbolSituationObject s) =>
            s.SymbolType switch
            {
                "landmark" => s.Name,
                "farp" => s.Name,
                "airDefenceHostile" => s.Name,
                "obstacle" => s.Name,
                _ => ""
            };
        

        private string GetSidc(string symbolType) =>
            symbolType switch
            {
                "homerPoint"        => "10002500001803000000",
                "landmark"          => "10031000001212000000",
                "airDefenceHostile" => "10061000001301020000",
                "obstacle"          => "10002500001318000000",
                _                   => ""
            };
    }
}
