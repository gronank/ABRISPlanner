using ClipperLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABRISPlanner.SpecParsing
{
    public class ParseGeometry
    {
        const float scale = 1e4f;
        const float NmToDegrees = 1.0f / 60;
        public static IEnumerable<List<PointF>> ComputeBuffer(IEnumerable<PointF> points, double delta)
        {
            var polys = points.Select(ToClipper).ToList();
            var buffer = Clipper.OffsetPolygons(polys, delta * NmToDegrees * scale);
            return buffer.Select(line => line.Select(ToPoint).ToList());
        }

        private static PointF ToPoint(IntPoint point)
        {
            return new(point.X / scale, point.Y / scale);
        }

        private static List<IntPoint> ToClipper(PointF point)
        {
            return new() { new((int)(point.X * scale), (int)(point.Y * scale)) };
        }
    }
}
