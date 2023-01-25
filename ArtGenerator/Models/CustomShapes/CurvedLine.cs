using System;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    class CurvedLine : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        public CurvedLine() { }
        public CurvedLine(Geometry geometry)
        {
            _definingGeometry = geometry;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                Random r = new();

                PathFigure myPathFigure = new();
                myPathFigure.StartPoint = new Point(r.Next(0, (int)Math.Floor(Width)), r.Next(0, (int)Math.Floor(Height)));

                PointCollection Points = new(3);
                Points.Add(new Point(r.Next(0, (int)Math.Floor(Width)), r.Next(0, (int)Math.Floor(Height))));
                Points.Add(new Point(r.Next(0, (int)Math.Floor(Width)), r.Next(0, (int)Math.Floor(Height))));
                Points.Add(new Point(r.Next(0, (int)Math.Floor(Width)), r.Next(0, (int)Math.Floor(Height))));

                PolyBezierSegment myBezierSegment = new();
                myBezierSegment.Points = Points;

                PathSegmentCollection myPathSegmentCollection = new();
                myPathSegmentCollection.Add(myBezierSegment);

                myPathFigure.Segments = myPathSegmentCollection;

                PathFigureCollection myPathFigureCollection = new();
                myPathFigureCollection.Add(myPathFigure);

                PathGeometry myPathGeometry = new();
                myPathGeometry.Figures = myPathFigureCollection;

                Geometry g = myPathGeometry;

                _definingGeometry = g;

                return _definingGeometry;
            }
        }
    }
}
