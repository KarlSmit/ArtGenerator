using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    public class StraightLinePath : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        private Func<Geometry, double, double, Geometry> _cutGeometry { get; set; }
        public StraightLinePath() { }
        public StraightLinePath(Geometry geometry)
        {
            _definingGeometry = geometry;
        }
        public StraightLinePath(Func<Geometry, double, double, Geometry> CutShape)
        {
            _cutGeometry = CutShape;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                Random r = new();
                List<PathSegment> segments = new(10);
                Point First;
                for (int i=0;i<10;i++)
                {
                    Point p1 = new(r.Next(0, (int)Width), r.Next(0, (int)Height));
                    if(i == 0) { First = p1; }
                    segments.Add(new LineSegment(p1, true));
                }

                List<PathFigure> figures = new(1);
                PathFigure pf = new(First, segments, true);
                figures.Add(pf);

                _definingGeometry = new PathGeometry(figures, FillRule.EvenOdd, null);
                if (_cutGeometry != null)  _definingGeometry = _cutGeometry(_definingGeometry, Width, Height); 

                return _definingGeometry;
            }
        }
    }
}
