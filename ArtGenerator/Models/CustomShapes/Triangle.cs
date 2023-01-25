using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    public class Triangle : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        private Func<Geometry, double, double, Geometry> _cutGeometry { get; set; }
        public Triangle() { }
        public Triangle(Geometry geometry)
        {
            _definingGeometry = geometry;
        }
        public Triangle(Func<Geometry, double, double, Geometry> CutShape)
        {
            _cutGeometry = CutShape;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                Point p1 = new(0, 0);
                Point p2 = new((int)Math.Floor(Width), 0);
                Point p3 = new((int)Math.Floor(Width) / 2, (int)Math.Floor(Height));

                List<PathSegment> segments = new(3);
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));
                segments.Add(new LineSegment(p3, true));

                List<PathFigure> figures = new(1);
                PathFigure pf = new(p1, segments, true);
                figures.Add(pf);

                _definingGeometry = new PathGeometry(figures, FillRule.EvenOdd, null);
                if (_cutGeometry != null) _definingGeometry = _cutGeometry(_definingGeometry, Width, Height); 
                return _definingGeometry;
            }
        }
    }
}
