using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    public class Star : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        private Func<Geometry, double, double, Geometry> _cutGeometry { get; set; }
        public Star() { }
        public Star(Geometry geometry)
        {
            _definingGeometry = geometry;
        }
        public Star(Func<Geometry, double, double, Geometry> CutShape)
        {
            _cutGeometry = CutShape;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                Point p1 = new(0, (int)Math.Floor(Height - (Height / 4)));
                Point p2 = new((int)Math.Floor(Width / 5), 0);
                Point p3 = new((int)Math.Floor(Width), 0);
                Point p4 = new((int)Math.Floor(Width / 3), -(int)Math.Floor(Height / 3));
                Point p5 = new((int)Math.Floor(Width / 2), -(int)Math.Floor(Height));
                Point p6 = new(0, -(int)Math.Floor(Height / 2));
                Point p7 = new(-(int)Math.Floor(Width / 2), -(int)Math.Floor(Height));
                Point p8 = new(-(int)Math.Floor(Width / 3), -(int)Math.Floor(Height / 3));
                Point p9 = new(-(int)Math.Floor(Width), 0);
                Point p10 = new(-(int)Math.Floor(Width / 5), 0);

                List<PathSegment> segments = new(10);
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));
                segments.Add(new LineSegment(p3, true));
                segments.Add(new LineSegment(p4, true));
                segments.Add(new LineSegment(p5, true));
                segments.Add(new LineSegment(p6, true));
                segments.Add(new LineSegment(p7, true));
                segments.Add(new LineSegment(p8, true));
                segments.Add(new LineSegment(p9, true));
                segments.Add(new LineSegment(p10, true));

                List<PathFigure> figures = new(1);
                PathFigure pf = new(p1, segments, true);
                figures.Add(pf);

                ScaleTransform newScale = new(Width / 400, Height / 400);
                _definingGeometry = new PathGeometry(figures, FillRule.EvenOdd, newScale);
                if (_cutGeometry != null) _definingGeometry = _cutGeometry(_definingGeometry, Width, Height); 
                return _definingGeometry;
            }
        }
    }
}
