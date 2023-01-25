using ArtGenerator.Models.CustomShapes;
using ArtGenerator.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Media = System.Windows.Media;

namespace ArtGenerator.Models
{
    public class Algorithm
    {
        public int CanvasShapeHeight { get; set; }
        public int CanvasShapeWidth { get; set; }
        public int AmountOfShapes { get; set; }
        public double ChanceToBeHollow { get; set; }
        public double BorderThickness { get; set; }
        public double ColorHue { get; set; }
        public double ColorSaturation { get; set; }
        public double ColorLuminance { get; set; }
        public Dictionary<Type, int> TypeOfShapeAmounts { get; set; }
        public double SkewingX { get; set; }
        public double SkewingY { get; set; }

        public int ShapeCellId { get; set; }
        public List<string> Mutations { get; set; }

        // Shape location cells
        public List<Cell> ShapeLocationCells { get; set; }
        public int PanelHeight { get; set; }
        public int PanelWidth { get; set; }

        private List<Painting> _generations = new();
        private IConfigService _configService;
        private int _iterationCount = 0;
        private Variables _config;
        private readonly Random _rnd = new();

        public Algorithm(IConfigService configService)
        {
            _configService = configService;
            _config = _configService.GetConfig();
            TypeOfShapeAmounts = new Dictionary<Type, int>();
            Mutations = new List<string>();
            ShapeLocationCells = new();
        }

        /// <summary>
        /// Implemented for different types of settings f.e. Amount of Shapes & Shape Width/Height
        /// </summary>
        /// <param name="generations"></param>
        /// <returns></returns>
        public double Execute(List<AlgorithmItem> generations, KeyValuePair<string, bool>? shouldMutate)
        {
            if (shouldMutate.HasValue && shouldMutate.Value.Value)
            {
                var propertyName = shouldMutate.Value.Key;
                var mutatedValue = MutatePaintingProperty(propertyName);

                return mutatedValue;
            }

            double totalIterationAmount = 0;
            var iterations = 0;
            foreach (AlgorithmItem generation in generations)
            {
                totalIterationAmount += generation.Value * generation.Likes;
                iterations += generation.Likes;
            }

            return totalIterationAmount / iterations;
        }

        public void AnalyseGenerations(Painting painting)
        {
            // Average of all the values from all generations of paintings
            var averagePaintingHeights = new List<AlgorithmItem>();
            var averagePaintingWidths = new List<AlgorithmItem>();
            var everyPaintingAmountOfShapes = new List<AlgorithmItem>();
            var averagePaintingBorder = new List<AlgorithmItem>();
            var averageHue = new List<AlgorithmItem>();
            var averageSaturation = new List<AlgorithmItem>();
            var averageLuminance = new List<AlgorithmItem>();
            var averageChanceToBeHollow = new List<AlgorithmItem>();

            var averageSkewingX = new List<AlgorithmItem>();
            var averageSkewingY = new List<AlgorithmItem>();

            var averageCellForShapes = new List<AlgorithmItem>();

            // Reinitialize for next generation of paintings so it doesn't use the last generation's values
            TypeOfShapeAmounts = new Dictionary<Type, int>();

            // Reset the mutation list for every analysis
            Mutations = new List<string>();

            var typeOfShapes = new List<Type>()
            {
                typeof(Ellipse),
                typeof(Triangle),
                typeof(CustomShapes.Rectangle),
                typeof(CurvedLine),
                typeof(ScatterDots),
                typeof(StraightLine),
                typeof(Star),
                typeof(CurvedLinePath),
                typeof(StraightLinePath),
                typeof(CustomShape)
            };

            var noCustomSizeShapes = new List<Type>()
            {
                typeof(CurvedLine), 
                typeof(CurvedLinePath), 
                typeof(StraightLine), 
                typeof(StraightLinePath)
            };

            Dictionary<Type, List<AlgorithmItem>> generatedShapeCount = new();
            foreach(Type shapeType in typeOfShapes)
            {
                generatedShapeCount.Add(shapeType, new List<AlgorithmItem>());
            }

            // Sort the list by most likes
            var sortedParentList = GetSortedParentList();

            foreach (Painting parentPainting in sortedParentList)
            {
                int totalPaintingHeight = 0;
                int totalPaintingWidth = 0;
                double totalPaintingBorder = 0;
                double totalHollowShapes = 0;
                List<double> totalHue = new();
                double totalSaturation = 0;
                double totalLuminance = 0;
                double totalSkewingX = 0;
                double totalSkewingY = 0;

                int totalShapeCellIds = 0;
                int customShapeSizeCount = 0;
                int allShapeCount = 0;

                Dictionary<Type, AlgorithmItem> paintingShapeCount = new();
                foreach (Type shapeType in typeOfShapes)
                {
                    paintingShapeCount.Add(shapeType, new AlgorithmItem());
                }

                foreach (var shape in parentPainting.Shapes)
                {
                    if (typeOfShapes.Contains(shape.GetType()))
                    {
                        paintingShapeCount[shape.GetType()].Value += 1;
                        paintingShapeCount[shape.GetType()].Likes += parentPainting.GetLikes();
                    }

                    allShapeCount++;
                    Media.Color c;
                    Color thisColor;

                    totalSkewingX += shape.GetSkewingX();
                    totalSkewingY += shape.GetSkewingY();

                    totalPaintingBorder += shape.StrokeThickness;

                    c = ((Media.SolidColorBrush)shape.Stroke).Color;
                    thisColor = Color.FromArgb(c.A, c.R, c.G, c.B);

                    totalHue.Add(thisColor.GetHue());
                    totalSaturation += thisColor.GetSaturation();
                    totalLuminance += thisColor.GetBrightness();
                    
                    // Shapes like lines have a different way of determining sizes
                    // This means we have to skip those
                    if (noCustomSizeShapes.Contains(shape.GetType()))
                    {
                        continue;
                    }

                    if (((Media.SolidColorBrush)shape.Fill).Color.A == 0) totalHollowShapes += 100;

                    totalPaintingHeight += (int)shape.Height;
                    totalPaintingWidth += (int)shape.Width;

                    totalShapeCellIds += shape.Cell.Id;

                    customShapeSizeCount++;
                }
                
                if(customShapeSizeCount != 0)
                {
                    averagePaintingHeights.Add(new AlgorithmItem()
                    {
                        Likes = parentPainting.GetLikes(),
                        Value = totalPaintingHeight / customShapeSizeCount
                    });

                    averagePaintingWidths.Add(new AlgorithmItem()
                    {
                        Likes = parentPainting.GetLikes(),
                        Value = totalPaintingWidth / customShapeSizeCount
                    });

                    averageChanceToBeHollow.Add(new AlgorithmItem()
                    {
                        Likes = parentPainting.GetLikes(),
                        Value = totalHollowShapes / customShapeSizeCount,
                    });

                    averagePaintingBorder.Add(new AlgorithmItem()
                    {
                        Likes = parentPainting.GetLikes(),
                        Value = totalPaintingBorder / customShapeSizeCount,
                    });
                }

                everyPaintingAmountOfShapes.Add(new AlgorithmItem()
                {
                    Likes = parentPainting.GetLikes(),
                    Value = allShapeCount,
                });

                averageHue.Add(new AlgorithmItem()
                {
                    Likes = parentPainting.GetLikes(),
                    Value = calculateAverageHue(totalHue),
                });

                averageSaturation.Add(new AlgorithmItem()
                {
                    Likes = parentPainting.GetLikes(),
                    Value = totalSaturation * 100 / allShapeCount,
                });

                averageLuminance.Add(new AlgorithmItem()
                {
                    Likes = parentPainting.GetLikes(),
                    Value = totalLuminance * 100 / allShapeCount,
                });

                averageSkewingX.Add(new AlgorithmItem()
                {
                    Likes = parentPainting.GetLikes(),
                    Value = totalSkewingX / allShapeCount,
                });

                averageSkewingY.Add(new AlgorithmItem()
                {
                    Likes = parentPainting.GetLikes(),
                    Value = totalSkewingY / allShapeCount,
                });

                averageCellForShapes.Add(new AlgorithmItem()
                {
                    Likes = parentPainting.GetLikes(),
                    Value = totalShapeCellIds / allShapeCount,
                });

                foreach (Type typeOfShape in typeOfShapes)
                {
                    generatedShapeCount[typeOfShape].Add(paintingShapeCount[typeOfShape]);
                }
            }
            double mutationProbability = _config.ChanceToMutate;
            
            // Generate numbers and determine if value should be mutated

            if(averagePaintingHeights.Count != 0)
            {
                CanvasShapeHeight = (int)Execute(averagePaintingHeights, new KeyValuePair<string, bool>(nameof(CanvasShapeHeight), _rnd.NextDouble() < mutationProbability));
                CanvasShapeWidth = (int)Execute(averagePaintingWidths, new KeyValuePair<string, bool>(nameof(CanvasShapeWidth), _rnd.NextDouble() < mutationProbability));
                ChanceToBeHollow = Execute(averageChanceToBeHollow, new KeyValuePair<string, bool>(nameof(ChanceToBeHollow), _rnd.NextDouble() < mutationProbability));
                BorderThickness = Execute(averagePaintingBorder, null);
            }

            AmountOfShapes = (int)Execute(everyPaintingAmountOfShapes, new KeyValuePair<string, bool>(nameof(AmountOfShapes), _rnd.NextDouble() < mutationProbability));
            ColorHue = Execute(averageHue, new KeyValuePair<string, bool>(nameof(ColorHue), _rnd.NextDouble() < mutationProbability));
            ColorSaturation = Execute(averageSaturation, new KeyValuePair<string, bool>(nameof(ColorSaturation), _rnd.NextDouble() < mutationProbability));
            ColorLuminance = Execute(averageLuminance, new KeyValuePair<string, bool>(nameof(ColorLuminance), _rnd.NextDouble() < mutationProbability));
            ShapeCellId = (int)Execute(averageCellForShapes, null);

            // Only generate values
            SkewingX = Execute(averageSkewingX, null);
            SkewingY = Execute(averageSkewingY, null);

            foreach (Type typeOfShape in typeOfShapes)
            {
                int value = Math.Max((int)Execute(generatedShapeCount[typeOfShape], null), 0);
                TypeOfShapeAmounts[typeOfShape] = value;
            }
        }

        /// <summary>
        /// Generate a random value for a property based on the mutations list
        /// The mutations list is generated in the Execute() method
        /// </summary>
        private double MutatePaintingProperty(string propertyName)
        {
            Mutations.Add(propertyName);

            return propertyName switch
            {
                nameof(CanvasShapeHeight) => _rnd.Next(_config.MinShapeSize, _config.MaxShapeSize),
                nameof(CanvasShapeWidth) => _rnd.Next(_config.MinShapeSize, _config.MaxShapeSize),
                nameof(AmountOfShapes) => _rnd.Next(5, _config.AddedShapesPerParent),
                nameof(ColorHue) => RandomDouble(0, 360),
                nameof(ColorSaturation) => RandomDouble(0, 100),
                nameof(ColorLuminance) => RandomDouble(0, 100),
                _ => 0,
            };
        }

        /// <summary>
        /// Divide the smaller & larger number up in scales.
        /// Determine the scale of value.
        /// Return a random number between the scale.
        /// </summary>
        /// <param name="scaleSmaller">The smaller number of the scale</param>
        /// <param name="scaleLarger">The larger number of the scale</param>
        /// <param name="value">The value to be fit in a scale</param>
        /// <returns>A random number between a scale based on value</returns>
        public int GetRandomValueInScale(int scaleSmaller, int scaleLarger, double value)
        {
            // Temp fix voor wanneer config wordt gebruikt
            if (value == 0 || value < scaleSmaller || value > scaleLarger)
            {
                return _rnd.Next(scaleSmaller, scaleLarger);
            }

            if(scaleSmaller == value && scaleLarger == value)
            {
                return (int)value;
            }

            // The number between the scale
            int betweenNumber = scaleLarger - scaleSmaller;
            // Index of the group
            int index = 0;
            // Initiate the group
            int[] groups = new int[5];
            // Loop through every number till 0
            while (betweenNumber > 0)
            {
                // Add one to a group
                groups[index] += 1;
                // Subtract one from total
                betweenNumber--;
                // Change index based on the remainder
                index = (index + 1) % 5;
            }

            // Random number for the result
            int randomNumberFromScale = 0;
            // The smaller number e.g 0
            int scaleNumberSumSmaller = 0;
            // The bigger number e.g 20
            int scaleNumberSumLarger = groups[0];
            for (int scaleCounter = 0; scaleCounter < 5; scaleCounter++)
            {
                // If number is between the scales, generate a random number between those
                if (value >= scaleNumberSumSmaller && value <= scaleNumberSumLarger)
                {
                    randomNumberFromScale = _rnd.Next(scaleNumberSumSmaller, scaleNumberSumLarger);
                    break;
                }
                // Add to smaller number e.g 0 += 20 = 20
                scaleNumberSumSmaller += groups[scaleCounter];
                // Add to larger number e.g 20 += 20 = 40
                scaleNumberSumLarger += groups[scaleCounter];
            }

            return randomNumberFromScale;
        }

        /// <summary>
        /// Calculates the average of all the hues
        /// </summary>
        /// <param name="hues">a list full of doubles</param>
        /// <returns>The average hue the list had</returns>
        private double calculateAverageHue(List<double> hues)
        {
            double X = 0.0;
            double Y = 0.0;

            int count = hues.Count;

            // Loop through all H values
            for (int hue = 0; hue < count; ++hue)
            {
                // Add the X and Y values to the sum X and Y
                X += Math.Cos(hues[hue] / 180 * Math.PI);
                Y += Math.Sin(hues[hue] / 180 * Math.PI);
            }

            // Now average the X and Y values
            X /= count;
            Y /= count;

            // Get atan2 of those
            double hueValue = Math.Atan2(Y, X) * 180 / Math.PI;

            // Make sure that the hue values aren't higher than 360 or lower than 0
            if (hueValue > 360) hueValue -= 360;
            else if (hueValue < 0) hueValue += 360;

            return hueValue;
        }

        /// <summary>
        /// Calculates the color based on all the top liked parents
        /// </summary>
        /// <returns>The best color according to the algorithm</returns>
        public Media.Color GenerateMostLikedColor()
        {
            double h = GetRandomValueInScale(0, 360, ColorHue);
            double s = (double)GetRandomValueInScale(0, 100, ColorSaturation) / 100;
            double v = (double)GetRandomValueInScale(0, 100, ColorLuminance) / 100;
            Color c = CalculateColorUsingHSL(h, s, v);
            return Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        /// Calculates the most contrasting color. (only based on hue, not lightness level)
        /// </summary>
        /// <param name="chosenColor">The color you want complimented</param>
        /// <returns>The contrasted color</returns>
        public Color CalculateComplementingColor(Color color)
        {
            return calculateHSLToColor(color, _rnd.Next(-30, 30));
        }

        /// <summary>
        /// Calculates the most contrasting color. (only based on hue, not lightness level)
        /// </summary>
        /// <param name="chosenColor">The color you want to contrast</param>
        /// <returns>The contrasted color</returns>
        public Color CalculateContrastingColor(Color color)
        {
            return calculateHSLToColor(color, 180);
        }

        /// <summary>
        /// Sends a color back using the given HSV values
        /// </summary>
        /// <param name="h">The hue of the color</param>
        /// <param name="s">The saturation of the color</param>
        /// <param name="v">The luminance of the color</param>
        /// <returns>The contrasted color</returns>
        public Color CalculateColorUsingHSL(double h, double s, double v)
        {
            return calculateNewColor(h, s, v);
        }

        /// <summary>
        /// Calculates the HSV value of a color
        /// </summary>
        /// <param name="color"></param>
        /// <param name="extraHue"></param>
        /// <returns></returns>
        private Color calculateHSLToColor(Color color, int extraHue)
        {
            double h = color.GetHue() + extraHue;
            double s = color.GetSaturation();
            double v = color.GetBrightness();
            return calculateNewColor(h, s, v);
        }

        /// <summary>
        /// Calculates the color using the given colors.
        /// </summary>
        /// <param name="h">The hue of the color</param>
        /// <param name="s">The saturation of the color</param>
        /// <param name="v">The luminance of the color</param>
        /// <returns>A new color based on the values given</returns>
        private Color calculateNewColor(double h, double s, double v)
        {
            // check if the hue value is above 360 or below 0
            if (h > 360) h -= 360;
            else if (h < 0) h += 360;

            // if the saturation is 0 (black) return black
            if (s == 0)
            { int L = (int)v; return Color.FromArgb(255, L, L, L); }

            // hue / 360d for easier calculations later on
            double min, max, hue;
            hue = h / 360d;

            // calculate the lighest color out of the RGB's
            max = v < 0.5d ? v * (1 + s) : (v + s) - (v * s);

            // calculate the darkest color out of the RGB's
            min = (v * 2d) - max;

            return Color.FromArgb(255, (int)(255 * RGBChannelFromHue(min, max, hue + 1 / 3d)),
                                       (int)(255 * RGBChannelFromHue(min, max, hue)),
                                       (int)(255 * RGBChannelFromHue(min, max, hue - 1 / 3d)));
        }

        /// <summary>
        /// Calculates the color based on the hue given
        /// </summary>
        /// <param name="m1">The lighest RGB value of the given color</param>
        /// <param name="m2">The darkest RGB value of the given color</param>
        /// <param name="h">Hue of the color</param>
        /// <returns>RGB values of with the correct hue</returns>
        static double RGBChannelFromHue(double m1, double m2, double h)
        {
            h = (h + 1d) % 1d;
            if (h < 0) h += 1;
            if (h * 6 < 1) return m1 + (m2 - m1) * 6 * h;
            else if (h * 2 < 1) return m2;
            else if (h * 3 < 2) return m1 + (m2 - m1) * 6 * (2d / 3d - h);
            else return m1;
        }
        /// <summary>
        /// Calculates the most contrasting color. (only based on hue, not lightness level)
        /// </summary>
        /// <param name="chosenColor">The color you want complimented</param>
        /// <returns>The contrasted color</returns>
        public Media.Color CalculateComplementingColor(Media.Color color)
        {
            Color c = calculateHSLToColor(Color.FromArgb(color.A, color.R, color.G, color.B), _rnd.Next(-30, 30));
            return Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        /// Calculates the most contrasting color. (only based on hue, not lightness level)
        /// </summary>
        /// <param name="chosenColor">The color you want to contrast</param>
        /// <returns>The contrasted color</returns>
        public Media.Color CalculateContrastingColor(Media.Color color)
        {
            Color c = calculateHSLToColor(Color.FromArgb(color.A, color.R, color.G, color.B), 180);
            return Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        /// Helper function for generating a random double
        /// </summary>
        /// <param name="minSize">Minimal size of double</param>
        /// <param name="maxSize">Maximal size of double</param>
        /// <returns>A random double</returns>
        private double RandomDouble(double minSize, double maxSize)
        {
            return minSize + (maxSize - minSize) * _rnd.NextDouble();
        }

        public void AddGeneration(Painting painting)
        {
            _generations.Add(painting);
            _iterationCount++;
        }

        public List<Painting> GetSortedParentList()
        {
            var sortedParentList = new List<Painting>();
            if (_generations.Any(x => x.MutationCount > 0))
            {
                // Get top 5 most liked and 1 mutation
                sortedParentList = _generations.OrderByDescending(painting => painting.GetLikes()).Take(5).ToList();
                var mutatedPainting = _generations.Where(p => p.MutationCount > 0).OrderByDescending(pa => pa.GetLikes() == 0).FirstOrDefault();
                sortedParentList.Add(mutatedPainting);
            }
            else
            {
                // Get topt 6 most liked
                sortedParentList = _generations.OrderByDescending(painting => painting.GetLikes()).Take(6).ToList();
            }
            return sortedParentList;
        }

        public void SetAlgorithmData(List<Painting> loadedGenerations, int iteration)
        {
            _generations = loadedGenerations;
            _iterationCount = iteration;
        }

        public bool CanUseAlgorithm()
        {
            if (_iterationCount >= _configService.GetConfig().StartCalculationsAtAmountOfParents)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public class AlgorithmItem
        {
            public int Likes { get; set; }

            public double Value { get; set; }
        }
    }
}
