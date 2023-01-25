using System;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    class CurvedLinePath : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        private Func<Geometry, double, double, Geometry> _cutGeometry { get; set; }
        public CurvedLinePath() { }
        public CurvedLinePath(Geometry geometry)
        {
            _definingGeometry = geometry;
        }
        public CurvedLinePath(Func<Geometry, double, double, Geometry> CutShape)
        {
            _cutGeometry = CutShape;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                Random r = new();

                PathFigure myPathFigure = new();
                myPathFigure.StartPoint = new Point(0, 0);

                PointCollection Points = new(10);

                for (int i = 0; i < 10; i++)
                {
                    Point p1 = new(r.Next(0, (int)Width), r.Next(0, (int)Height));
                    Points.Add(p1);
                }

                PolyBezierSegment myBezierSegment = new();
                myBezierSegment.Points = Points;

                PathSegmentCollection myPathSegmentCollection = new();
                myPathSegmentCollection.Add(myBezierSegment);

                myPathFigure.Segments = myPathSegmentCollection;

                PathFigureCollection myPathFigureCollection = new();
                myPathFigureCollection.Add(myPathFigure);

                _definingGeometry = new PathGeometry(myPathFigureCollection, FillRule.EvenOdd, null);

                _definingGeometry = PathGeometry.Combine(_definingGeometry, _definingGeometry, GeometryCombineMode.Union, null);
                if (_cutGeometry != null)  _definingGeometry = _cutGeometry(_definingGeometry, Width, Height);
                return _definingGeometry;
            }
        }
    }
}
