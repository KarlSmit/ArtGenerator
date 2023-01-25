namespace ArtGenerator.Models
{
    /// <summary>
    /// A cell for keeping track of the location of shapes
    /// </summary>
    public class Cell
    {
        public int Id { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int Column { get; set; }

        public int Row { get; set; }
    }
}