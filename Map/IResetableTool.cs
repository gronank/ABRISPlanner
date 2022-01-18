using Carmenta.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABRISPlanner.Map
{
    public interface IResetableTool: ITool
    {
        public event Action Reset;
    }
}
