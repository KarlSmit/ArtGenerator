using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    public class CustomShape : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        private List<PathFigure> path = new();
        private List<PathFigure> cuts = new();

        public CustomShape(Geometry geometry)
        {
            _definingGeometry = geometry;
        }

        public CustomShape(string stringifiedGeometry)
        {
            List<string> shapes = stringifiedGeometry.Split(";").ToList();

            foreach (string points in shapes)
            {
                if (points.Length == 0 || points == "")
                {
                    continue;
                }

                Point first = new(0,0);
                List<PathSegment> segments = new();
                foreach (string point in points.Split(","))
                {
                    List<string> split = point.Split(".").ToList();

                    if(split.Count <= 1 || point.Length == 0 || split[0] == "0" || split[1] == "0")
                    {
                        continue;
                    }

                    Point restoredPoint = new(int.Parse(split[0]), int.Parse(split[1]));

                    if (first.X == 0)
                    {
                        first = restoredPoint;
                    }

                    segments.Add(new LineSegment(restoredPoint, true));
                }
                PathFigure pathFigure = new(first, segments, true);


                if (shapes.IndexOf(points) <= 2)
                {
                    path.Add(pathFigure);
                }
                else
                {
                    cuts.Add(pathFigure);
                }
            }
        }

        protected override Geometry DefiningGeometry
        {
            get {
                if(_definingGeometry != null) return _definingGeometry;

                ScaleTransform newScale = new(Width / 500, Height / 500);
                _definingGeometry = PathGeometry.Combine(new PathGeometry(path, FillRule.EvenOdd, newScale), new PathGeometry(cuts, FillRule.EvenOdd, newScale), GeometryCombineMode.Exclude, null);
                return _definingGeometry;
            }
        }
    }
}
