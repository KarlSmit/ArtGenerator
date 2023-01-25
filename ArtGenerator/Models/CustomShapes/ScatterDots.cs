using System;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    class ScatterDots : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        public ScatterDots() { }
        public ScatterDots(Geometry geometry)
        {
            _definingGeometry = geometry;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                Random r = new();

                EllipseGeometry tmp = new(new Point(r.Next(0, (int)Width), r.Next(0, (int)Height)), 2, 2);
                Geometry g = PathGeometry.Combine(tmp, tmp, GeometryCombineMode.Union, null);

                int amountOfDots = (int)(Width + Height) / 25;
                for (int i = 0; i < amountOfDots; i++)
                {
                    int size = r.Next(1, 5);
                    g = PathGeometry.Combine(g, new EllipseGeometry(new Point(r.Next(0, (int)Width), r.Next(0, (int)Height)), size, size), GeometryCombineMode.Union, null);
                }

                _definingGeometry = g;
                return _definingGeometry;
            }
        }
    }
}
