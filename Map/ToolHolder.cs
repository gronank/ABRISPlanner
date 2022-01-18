using Carmenta.Engine;
using Carmenta.Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABRISPlanner.Map
{
    internal class ToolHolder
    {
        private readonly ITool DefaultTool;
        private readonly MapControl MapControl;
        public ToolHolder(ITool tool,MapControl mapControl)
        {
            DefaultTool = tool;
            MapControl = mapControl;
            Reset();
            
        }
        public void SetTool(IResetableTool tool)
        {
            if(MapControl.Tool is IResetableTool resetable)
            {
                resetable.Reset -= Reset;
            }
            MapControl.Tool = tool;
            tool.Reset += Reset;
        }
        public void Reset() => MapControl.Tool = DefaultTool;
    }
}
