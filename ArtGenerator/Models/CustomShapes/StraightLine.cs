using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    class StraightLine : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        public StraightLine() { }
        public StraightLine(Geometry geometry)
        {
            _definingGeometry = geometry;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                Random r = new();
                Point p1 = new(r.Next(0, (int)Math.Floor(Width)), r.Next(0, (int)Math.Floor(Height)));
                Point p2 = new(r.Next(0, (int)Math.Floor(Width)), r.Next(0, (int)Math.Floor(Height)));

                List<PathSegment> segments = new(2);
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));

                List<PathFigure> figures = new(1);
                PathFigure pf = new(p1, segments, true);
                figures.Add(pf);

                Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);

                _definingGeometry = g;
                return _definingGeometry;
            }
        }
    }
}
