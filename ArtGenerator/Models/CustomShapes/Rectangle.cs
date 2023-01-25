using System;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Models.CustomShapes
{
    class Rectangle : PaintingShape
    {
        private Geometry _definingGeometry { get; set; }
        private Func<Geometry, double, double, Geometry> _cutGeometry { get; set; }
        public Rectangle() { }
        public Rectangle(Geometry geometry)
        {
            _definingGeometry = geometry;
        }
        public Rectangle(Func<Geometry, double, double, Geometry> CutShape)
        {
            _cutGeometry = CutShape;
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry != null) return _definingGeometry;

                _definingGeometry = new RectangleGeometry(new Rect(0, 0, Width, Height), 0, 0);
                _definingGeometry = PathGeometry.Combine(_definingGeometry, _definingGeometry, GeometryCombineMode.Union, null);

                if (_cutGeometry != null) _definingGeometry = _cutGeometry(_definingGeometry, Width, Height);
                return _definingGeometry;
            }
        }
    }
}
