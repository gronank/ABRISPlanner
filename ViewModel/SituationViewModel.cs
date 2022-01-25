using ABRISPlanner.Model;
using System.Text.Json;
using ABRISPlanner.SpecParsing;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TacticalSimWpf.ViewModel;

namespace ABRISPlanner.ViewModel
{
    public class SituationViewModel : Changeable
    {
        private const string ServerSpecPath = @"Specs/rotorheads.serverspec.json";
        readonly public Situation Situation;
        public SituationViewModel(Situation situation)
        {
            Situation = situation;
        }
        internal void Update()
        {
            var spec = ServerSpec.Open(ServerSpecPath); 
            Situation.Symbols = spec.ReadSymbols();
            Situation.Zones = spec.ReadZones();
            Changed("Situation");
        }
    }
}
