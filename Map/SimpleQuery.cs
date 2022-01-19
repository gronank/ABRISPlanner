using System.Collections.Generic;
using System.Linq;
using Carmenta.Engine;
namespace ABRISPlanner.Map
{

    class SimpleQuery : IQueryResult
        {
            private readonly IEnumerator<Feature>  Features;
            public SimpleQuery(IEnumerable<Feature> ftrs)=>Features = ftrs.GetEnumerator();
            public IEnumerable<Visualizer> GetVisualizers(Feature feature)=>Enumerable.Empty<Visualizer>();
            public Feature Next() => Features.MoveNext() ? Features.Current : null;

        }
    
}
