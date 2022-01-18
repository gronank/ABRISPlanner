using System.Collections.Generic;

namespace ABRISPlanner.Model
{
    public class Situation
    {
        public List<LineSituationObject> Lines;
        public List<SymbolSituationObject> Symbols;
        public List<ZoneSituationObject> Zones;
    }
}