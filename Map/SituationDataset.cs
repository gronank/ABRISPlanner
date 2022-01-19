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
            var symbols = Situation.Situation.Symbols.Select(s => MakeFeature(s));
            featureId = 0;
            return new SimpleQuery(symbols);
        }

        private Feature MakeFeature(SymbolSituationObject s)
        {
            AttributeSet attributes = new()
            {
                [NameAtom] = s.Name,
                [DesigantionAtom] = s.Name,
                [SymbolTypeAtom] = s.SymbolType,
                [SidcAtom] = GetSidc(s.SymbolType)
            };
            return new(new PointGeometry(s.Location.ToPoint()), Crs, attributes,new Id(Context.Id, featureId++));
        }

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
