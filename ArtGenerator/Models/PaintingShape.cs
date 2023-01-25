using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArtGenerator.Models
{
    /// <summary>
    /// A painting shape which is based on a shape.
    /// Making it possible to add generic variables for a shape
    /// </summary>
    public class PaintingShape : Shape
    {
        public Cell Cell { get; set; }

        public void Skewing(int skewingX, int skewingY)
        {
            LayoutTransform = new SkewTransform()
            {
                // value AngleX en AngleY worden vanuit de config gelezen
                AngleX = skewingX,
                AngleY = skewingY
            };
        }

       public double GetSkewingX()
        {
            return (double)this.LayoutTransform.GetValue(SkewTransform.AngleXProperty);
        }

        public double GetSkewingY()
        {
            return (double)this.LayoutTransform.GetValue(SkewTransform.AngleYProperty);
        }

        protected override Geometry DefiningGeometry => throw new NotImplementedException();

        public Geometry GetGeometry()
        {
            return DefiningGeometry;
        }
    }
}
