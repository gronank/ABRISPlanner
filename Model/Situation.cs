using System.Collections.Generic;

namespace ABRISPlanner.Model
{
    public class Situation
    {
        public List<LineSituationObject> Lines = new();
        public List<SymbolSituationObject> Symbols = new();
        public List<ZoneSituationObject> Zones = new();
    }
}