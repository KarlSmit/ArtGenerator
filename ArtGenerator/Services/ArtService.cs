using ArtGenerator.Models;
using ArtGenerator.Models.CustomShapes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ArtGenerator.Services
{
    public class ArtService : IArtService
    {
        private readonly Random _rnd = new();
        private readonly List<Type> _noCustomSize = new() { typeof(CurvedLine), typeof(CurvedLinePath), typeof(StraightLine), typeof(StraightLinePath) };

        private int _iterationNumber = 0;
        private IConfigService _configService;

        private int _panelWidth;
        private int _panelHeight;

        private Algorithm _algorithm;
        private Variables _config;

        public ArtService(IConfigService configService)
        {
            _configService = configService;

            _algorithm = new Algorithm(_configService);
            _config = _configService.GetConfig();
        }

        /// <summary>
        /// Setting the panel width from the main window
        /// </summary>
        /// <param name="panelWidth">Widht of the panel</param>
        /// <param name="panelHeight">Height of the panel</param>
        public void SetPanelWidthAndHeight(int panelWidth, int panelHeight)
        {
            _panelWidth = panelWidth;
            _panelHeight = panelHeight;
            _algorithm.PanelWidth = panelHeight;
            _algorithm.PanelHeight = panelHeight;
        }

        /// <summary>
        /// Generate a painting either based on the algorithm or from the configuration
        /// </summary>
        /// <param name="oldPanel">The painting that was not selected</param>
        /// <param name="otherPanel">The painting that was liked</param>
        /// <returns>A new painting to be put in the panel</returns>
        public Painting GenerateArt(Painting oldPanel, Painting otherPanel)
        {
            _iterationNumber++;
            Painting painting;
            if(_algorithm.CanUseAlgorithm())
            {
                _algorithm.AnalyseGenerations(oldPanel);
                otherPanel.UpdateIterationsSurvived(_iterationNumber);

                painting = GenerateAlgorithmBasedPainting();
            }
            else
            {
                painting = GenerateRandomDefaultPainting();
            }

            return painting;
        }

        /// <summary>
        /// Create a new Algorithm so that all the previous generations are reset
        /// </summary>
        public void ResetAlgorithm()
        {
            _algorithm = new Algorithm(_configService);
        }

        /// <summary>
        /// Function that determines if you can use the algoritm
        /// </summary>
        /// <returns>The algortim if function is used</returns>
        public bool CanUseAlgorithm()
        {
            return _algorithm.CanUseAlgorithm();
        }

        public int GetIterationNumber()
        {
            return _iterationNumber;
        }

        /// <summary>
        /// Generate a painting with the default values from the configuration
        /// </summary>
        /// <returns>A randomly generated painting</returns>
        private Painting GenerateRandomDefaultPainting()
        {
            Painting painting = new();
            painting.UpdateIterationsSurvived(_iterationNumber);

            if (!_algorithm.ShapeLocationCells.Any())
            {
                CreateShapeLocationCells();
            }

            var randomShapeCount = _rnd.Next(5, _config.AddedShapesPerParent);

            int red = 0;
            int green = 0;
            int blue = 0;
            for (int shapeCounter = 0; shapeCounter < randomShapeCount; shapeCounter++)
            {
                PaintingShape shape = DetermineShape();
                int skewingX = _rnd.Next(_config.MinSkewing, _config.MaxSkewing);
                int skewingY = _rnd.Next(_config.MinSkewing, _config.MaxSkewing);
                shape.Skewing(skewingX, skewingY);

                // Shape color
                byte r = (byte)_rnd.Next(256);
                byte g = (byte)_rnd.Next(256);
                byte b = (byte)_rnd.Next(256);
                red += r;
                green += g;
                blue += b;
                Color color = Color.FromArgb(255, r, g, b);
                System.Drawing.Color c = _algorithm.CalculateComplementingColor(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
                Brush complementingColor = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));

                // Determine if shape has custom sizing enabled
                if (_noCustomSize.Contains(shape.GetType()))
                {
                    shape.Width = _panelWidth - 2;
                    shape.Height = _panelHeight - 2;
                    shape.StrokeThickness = RandomDouble(_config.MinBorderSize, _config.MaxBorderSize);
                    shape.Stroke = new SolidColorBrush(color);
                    shape.Margin = new Thickness(1, 1, 1, 1);
                }
                else
                {
                    int width = _rnd.Next(_config.MinShapeSize, _config.MaxShapeSize);
                    int height = _rnd.Next(_config.MinShapeSize, _config.MaxShapeSize);
                    shape.Width = width;
                    shape.Height = height;
                    shape.StrokeThickness = RandomDouble(_config.MinBorderSize, _config.MaxBorderSize);
                    shape.Stroke = complementingColor;
                    shape.Fill = (_rnd.NextDouble() > _config.ChanceToBeHollow) ? new SolidColorBrush(color) : new SolidColorBrush(Color.FromArgb(0,0,0,0));

                    var randomCell = _algorithm.ShapeLocationCells[_rnd.Next(0, _config.RowCount * _config.ColumnCount)];
                    shape.Cell = randomCell;
                    if (_config.LocationDetection)
                    {
                        shape.Margin = GetRandomValueBasedOnCell(randomCell);
                    }
                    else
                    {
                        shape.Margin = new Thickness(_rnd.Next(1, _panelWidth - width), _rnd.Next(1, _panelHeight - height), 1, 1);
                    }
                }
                painting.Shapes.Add(shape);
            }

            _algorithm.AddGeneration(painting);

            switch (_config.ChosenBackgroundType)
            {
                case "Custom":
                    painting.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_config.PaintingBackgroundColor); ;
                    break;

                case "Dynamic":
                    painting.Background = new SolidColorBrush(Color.FromArgb(255, (byte)(red / painting.Shapes.Count), (byte)(green / painting.Shapes.Count), (byte)(blue / painting.Shapes.Count)));
                    break;

                case "Complementing":
                    painting.Background = new SolidColorBrush(_algorithm.CalculateComplementingColor(_algorithm.GenerateMostLikedColor()));
                    break;

                case "Contrasting":
                    painting.Background = new SolidColorBrush(_algorithm.CalculateContrastingColor(_algorithm.GenerateMostLikedColor()));
                    break;
            }

            return painting;
        }

        /// <summary>
        /// Generate a painting based on the algorithm
        /// </summary>
        /// <returns>A new painting generated based on the algorithm</returns>
        private Painting GenerateAlgorithmBasedPainting()
        {
            Painting painting = new();
            painting.UpdateIterationsSurvived(_iterationNumber);
            painting.MutationCount = painting.MutationCount += _algorithm.Mutations.Count;

            int red = 0;
            int green = 0;
            int blue = 0;
            for (int shapeCounter = 0; shapeCounter < _algorithm.AmountOfShapes; shapeCounter++)
            {
                PaintingShape shape = DetermineShapeAlgorithm();
                int skewingX = _algorithm.GetRandomValueInScale(_config.MinSkewing, _config.MaxSkewing, _algorithm.SkewingX);
                int skewingY = _algorithm.GetRandomValueInScale(_config.MinSkewing, _config.MaxSkewing, _algorithm.SkewingY);
                shape.Skewing(skewingX, skewingY);

                // Shape color
                byte r = (byte)_rnd.Next(256);
                byte g = (byte)_rnd.Next(256);
                byte b = (byte)_rnd.Next(256);
                red += r;
                green += g;
                blue += b;

                // Determine if shape has custom sizing enabled
                if (_noCustomSize.Contains(shape.GetType()))
                {
                    shape.Width = _panelWidth - 2;
                    shape.Height = _panelHeight - 2;
                    shape.StrokeThickness = _algorithm.GetRandomValueInScale(_config.MinBorderSize, _config.MaxBorderSize, _algorithm.BorderThickness);
                    shape.Stroke = new SolidColorBrush(_algorithm.GenerateMostLikedColor());
                    shape.Margin = new Thickness(1, 1, 1, 1);
                }
                else
                {
                    Color bestColor = _algorithm.GenerateMostLikedColor();
                    Color fill = _rnd.NextDouble() > (_algorithm.GetRandomValueInScale(0, 100, _algorithm.ChanceToBeHollow) / 100.0) ? bestColor : Color.FromArgb(0,0,0,0);
                    System.Drawing.Color c = _algorithm.CalculateComplementingColor(System.Drawing.Color.FromArgb(bestColor.A, bestColor.R, bestColor.G, bestColor.B));
                    Brush complementingColor = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));

                    int width = _algorithm.GetRandomValueInScale(_config.MinShapeSize, _config.MaxShapeSize, _algorithm.CanvasShapeWidth);
                    int height = _algorithm.GetRandomValueInScale(_config.MinShapeSize, _config.MaxShapeSize, _algorithm.CanvasShapeHeight);
                    shape.Width = width;
                    shape.Height = height;
                    shape.StrokeThickness = _algorithm.GetRandomValueInScale(_config.MinBorderSize, _config.MaxBorderSize, _algorithm.BorderThickness);
                    shape.Stroke = complementingColor;
                    shape.Fill = new SolidColorBrush(fill);

                    var algoCell = _algorithm.ShapeLocationCells.Find(c => c.Id == _algorithm.ShapeCellId);
                    shape.Cell = algoCell;

                    if(_config.LocationDetection)
                    {
                        shape.Margin = GenerateThicknessAroundCell(algoCell);
                    }
                    else
                    {
                        shape.Margin = new Thickness(_rnd.Next(1, _panelWidth - width), _rnd.Next(1, _panelHeight - height), 1, 1);
                    }
                }
                painting.Shapes.Add(shape);
            }

            _algorithm.AddGeneration(painting);
            switch (_config.ChosenBackgroundType)
            {
                case "Custom":
                    painting.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_config.PaintingBackgroundColor); ;
                    break;

                case "Dynamic":
                    painting.Background = new SolidColorBrush(Color.FromArgb(255, (byte)(red / painting.Shapes.Count), (byte)(green / painting.Shapes.Count), (byte)(blue / painting.Shapes.Count)));
                    break;

                case "Complementing":
                    painting.Background = new SolidColorBrush(_algorithm.CalculateComplementingColor(_algorithm.GenerateMostLikedColor()));
                    break;

                case "Contrasting":
                    painting.Background = new SolidColorBrush(_algorithm.CalculateContrastingColor(_algorithm.GenerateMostLikedColor()));
                    break;
            }

            return painting;
        }

        /// <summary>
        /// Loads saved data that was created.
        /// </summary>
        public void LoadGenerationData(string serializedJsonData, out Painting leftPanel, out Painting rightPanel)
        {
            AlgorithmDNA jsonData = JsonConvert.DeserializeObject<AlgorithmDNA>(serializedJsonData);

            _iterationNumber = jsonData.Iteration;

            leftPanel = new()
            {
                Background = jsonData.LeftPanel.Background,
                MayLiveUntil = jsonData.LeftPanel.MayLiveUntil,
                MutationCount = jsonData.LeftPanel.MutationCount,
                Shapes = ConvertToPaintingShape(jsonData.LeftPanel.Shapes)
            };
            leftPanel.SetIterationsSurvived(jsonData.LeftPanel.IterationsSurvived);

            rightPanel = new()
            {
                Background = jsonData.RightPanel.Background,
                MayLiveUntil = jsonData.RightPanel.MayLiveUntil,
                MutationCount = jsonData.RightPanel.MutationCount,
                Shapes = ConvertToPaintingShape(jsonData.RightPanel.Shapes)
            };
            rightPanel.SetIterationsSurvived(jsonData.RightPanel.IterationsSurvived);

            List<Painting> generations = new();
            foreach (PaintingDNA generation in jsonData.Generations)
            {
                Painting newPainting = new()
                {
                    Background = generation.Background,
                    MayLiveUntil = generation.MayLiveUntil,
                    MutationCount = generation.MutationCount,
                    Shapes = ConvertToPaintingShape(generation.Shapes)
                };
                newPainting.SetIterationsSurvived(generation.IterationsSurvived);
                generations.Add(newPainting);

            }

            _algorithm.SetAlgorithmData(generations, jsonData.Iteration);
        }

        /// <summary>
        /// Saves the data that was created until now.
        /// </summary>
        public void SaveGenerationData(Painting leftPanel, Painting rightPanel)
        {
            List<PaintingDNA> generations = new();
            foreach (Painting parent in _algorithm.GetSortedParentList())
            {
                generations.Add(new()
                {
                    Background = parent.Background,
                    IterationsSurvived = parent.GetLikes() + 2,
                    MayLiveUntil = parent.MayLiveUntil,
                    MutationCount = parent.MutationCount,
                    Shapes = ConvertToPaintingShapeDNA(parent.Shapes.ToList())
                });
            }

            var data = new AlgorithmDNA()
            {
                DateCreated = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"),
                Iteration = _iterationNumber,
                LeftPanel = new() {
                    Background = leftPanel.Background,
                    IterationsSurvived = leftPanel.GetLikes() + 2,
                    MayLiveUntil = leftPanel.MayLiveUntil,
                    MutationCount = leftPanel.MutationCount,
                    Shapes = ConvertToPaintingShapeDNA(leftPanel.Shapes.ToList())
                },
                RightPanel = new()
                {
                    Background = rightPanel.Background,
                    IterationsSurvived = rightPanel.GetLikes() + 2,
                    MayLiveUntil = rightPanel.MayLiveUntil,
                    MutationCount = rightPanel.MutationCount,
                    Shapes = ConvertToPaintingShapeDNA(rightPanel.Shapes.ToList())
                },
                Generations = generations
            };

            string fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArtGenerator\\latest.json";
            string JsonData = JsonConvert.SerializeObject(data);

            // Moves the file so it doesn't delete old Generation Data
            if(File.Exists(fileLocation))
            {
                AlgorithmDNA temporary = JsonConvert.DeserializeObject<AlgorithmDNA>(File.ReadAllText(fileLocation));
                string newLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArtGenerator\\OldGenerations\\" + temporary.DateCreated + ".json";
                if (File.Exists(newLocation)) File.Delete(newLocation);
                File.Move(fileLocation, newLocation );
            }

            File.WriteAllLines(fileLocation, new List<string>() { JsonData });
        }

        /// <summary>
        /// Converts a stringified list of PaintingShapeDNA to normal Paintingshapes
        /// </summary>
        /// <param name="stringified">String containing List of PaintingShapesDNA</param>
        /// <returns>Shapes with all it's original values</returns>
        private ObservableCollection<PaintingShape> ConvertToPaintingShape(string stringified)
        {
            List<PaintingShapeDNA> shapesList = JsonConvert.DeserializeObject<List<PaintingShapeDNA>>(stringified);
            ObservableCollection <PaintingShape> convertedList = new();
            foreach (PaintingShapeDNA shape in shapesList)
            {
                PaintingShape newShape = (PaintingShape)Activator.CreateInstance(shape.type, shape.DefiningGeometry);
                newShape.Skewing((int)shape.AngleX, (int)shape.AngleY);
                newShape.Width = shape.Width;
                newShape.Height = shape.Height;
                newShape.Fill = shape.Fill;
                newShape.Cell = (shape.Cell != null) ? new()
                {
                    Id = shape.Cell.Id,
                    Height = shape.Cell.Height,
                    Width = shape.Cell.Width,
                    Column = shape.Cell.Column,
                    Row = shape.Cell.Row
                } : null;
                newShape.Margin = shape.Margin;
                newShape.Stroke = shape.Border;
                newShape.StrokeThickness = shape.BorderThickness;
                convertedList.Add(newShape);
            }
            return convertedList;
        }

        /// <summary>
        /// Exports all Shape data to PaintingShapeDNA so it can be saved
        /// </summary>
        /// <param name="shapesList">List of PaintingShapes</param>
        /// <returns>Stringified version of a List with PaintingShapesDNA</returns>
        private string ConvertToPaintingShapeDNA(List<PaintingShape> shapesList)
        {
            List<PaintingShapeDNA> convertedList = new();
            foreach(PaintingShape shape in shapesList)
            {
                convertedList.Add(new()
                {
                    type = shape.GetType(),
                    Border = shape.Stroke,
                    Fill = shape.Fill,
                    Width = shape.Width,
                    Height = shape.Height,
                    AngleX = shape.GetSkewingX(),
                    AngleY = shape.GetSkewingY(),
                    Margin = shape.Margin,
                    Cell = (shape.Cell != null) ? new CellDNA
                    {
                        Id = shape.Cell.Id,
                        Height = shape.Cell.Height,
                        Width = shape.Cell.Width,
                        Column = shape.Cell.Column,
                        Row = shape.Cell.Row
                    } : null,
                    BorderThickness = shape.StrokeThickness,
                    DefiningGeometry = shape.GetGeometry(),
                });
            }
            return JsonConvert.SerializeObject(convertedList);
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

        /// <summary>
        /// A method that randomly chooses a shape to add to the canvas.
        /// </summary>
        /// <returns>A PaintingShape</returns>
        private PaintingShape DetermineShape()
        {
            var weightedgarbage = new Dictionary<Type, int>()
            {
                {typeof(Ellipse), (_config.AllowCircle) ? 3: 0 },
                {typeof(Triangle), (_config.AllowTriangle) ? 3: 0},
                {typeof(Rectangle), (_config.AllowRectangle) ? 3: 0},
                {typeof(CurvedLine), (_config.AllowCurvedLine) ? 3: 0},
                {typeof(ScatterDots), (_config.AllowDot) ? 3: 0},
                {typeof(StraightLine), (_config.AllowLine) ? 3: 0},
                {typeof(Star), (_config.AllowStar) ? 2: 0},
                {typeof(CurvedLinePath), (_config.AllowCurvedLine) ? 1: 0},
                {typeof(CustomShape), (_config.AllowCustomShapes) ? 1: 0},
                {typeof(StraightLinePath),  (_config.AllowLine) ? 1: 0}
            };
            var p = weightedgarbage
                .Select(i => i.Value).Cast<Dictionary<Type, int>>()
                .SelectMany(w => w.Where(c => c.Value != 0));
            int total = weightedgarbage.Values.Sum();
            var coole = _rnd.Next(total);
            var cito = 0;
            foreach (var item in weightedgarbage)
            {
                if (cito + item.Value >= coole)
                {
                    if (item.Value != 0)
                    {
                        if (item.Key == typeof(CustomShape))
                        {
                            return new CustomShape(_configService.GetConfig().CustomShape);
                        }
                        var shapeInstance = (PaintingShape)Activator.CreateInstance(item.Key);
                        if (!_noCustomSize.Contains(shapeInstance.GetType()))
                        {
                            if (shapeInstance.GetType() != typeof(ScatterDots))
                            {
                                if (_config.AllowCutsInShapes)
                                {
                                    Func<Geometry, double, double, Geometry> cutFunc = CutThisShape;
                                    return (PaintingShape)Activator.CreateInstance(item.Key, cutFunc); ; /*geometry*/
                                }
                            }
                        }
                        return shapeInstance;
                    }
                }
                cito += item.Value;
            }
            return new Rectangle();
        }

        /// <summary>
        /// A method that randomly chooses a shape to add to the canvas.
        /// </summary>
        /// <returns>A PaintingShape</returns>
        private PaintingShape DetermineShapeAlgorithm()
        {
            var shapeAmount = _algorithm.TypeOfShapeAmounts;
            int currentIttoration = 0;
            var ConfigedShapeAmount = new Dictionary<Type, int>()
            {
                {typeof(Ellipse), (_config.AllowCircle) ? shapeAmount[typeof(Ellipse)]: 0 },
                {typeof(Triangle), (_config.AllowTriangle) ? shapeAmount[typeof(Triangle)]: 0},
                {typeof(Rectangle), (_config.AllowRectangle) ? shapeAmount[typeof(Rectangle)]: 0},
                {typeof(CurvedLine), (_config.AllowCurvedLine) ? shapeAmount[typeof(CurvedLine)]: 0},
                {typeof(ScatterDots), (_config.AllowDot) ? shapeAmount[typeof(ScatterDots)]: 0},
                {typeof(StraightLine), (_config.AllowLine) ? shapeAmount[typeof(StraightLine)]: 0},
                {typeof(Star), (_config.AllowStar) ? shapeAmount[typeof(Star)]: 0},
                {typeof(CurvedLinePath), (_config.AllowCurvedLine) ? shapeAmount[typeof(CurvedLinePath)]: 0},
                {typeof(CustomShape), (_config.AllowCustomShapes) ? shapeAmount[typeof(CustomShape)]: 0},
                {typeof(StraightLinePath),  (_config.AllowLine) ? shapeAmount[typeof(StraightLinePath)]: 0}
            };
            int configTotal = ConfigedShapeAmount.Values.Sum();
            int random = _rnd.Next(configTotal);

            foreach (Type key in ConfigedShapeAmount.Keys)
            {
                if (currentIttoration + ConfigedShapeAmount[key] >= random)
                {
                    if (key == typeof(CustomShape))
                    {
                        return new CustomShape(_configService.GetConfig().CustomShape);
                    }

                    var shapeInstance = (PaintingShape)Activator.CreateInstance(key);
                    if (!_noCustomSize.Contains(shapeInstance.GetType()))
                    {
                        if (shapeInstance.GetType() != typeof(ScatterDots))
                        {
                            if (_config.AllowCutsInShapes)
                            {
                                Func<Geometry, double, double, Geometry> cutFunc = CutThisShape;
                                return (PaintingShape)Activator.CreateInstance(key, cutFunc); ; /*geometry*/
                            }
                        }
                    }
                    return shapeInstance;
                }
                currentIttoration += ConfigedShapeAmount[key];
            }
            return DetermineShape();
        }

        public Geometry CutThisShape(Geometry shape, double width, double height)
        {
            int shapeWidth = 20;
            int shapeHeight = 20;


            Random r = new();
            if (r.NextDouble() <= 0.5)
            {
                Geometry cut;
                if (r.NextDouble() < 0.5)
                {
                    cut = new EllipseGeometry(new(r.Next(0, (int)width), r.Next(0, (int)height)), shapeWidth, shapeHeight);
                }
                else
                {
                    cut = new RectangleGeometry(new(r.Next(-(int)width, (int)width), r.Next(-(int)height, (int)height), shapeWidth * 1.5, shapeHeight * 1.5), 0, 0);
                }
                return PathGeometry.Combine(shape, cut, GeometryCombineMode.Exclude, null);
            }
            return PathGeometry.Combine(shape, shape, GeometryCombineMode.Union, null);
        }

        private void CreateShapeLocationCells()
        {
            int rowCount = _config.RowCount;
            int columnCount = _config.ColumnCount;
            int singleCellHeight = _panelHeight / rowCount;
            int singleCellWidth = _panelWidth / columnCount;

            int width = 0;
            int height = singleCellHeight;
            int cellCounter = 0;
            for(int columnCounter = 0; columnCounter < columnCount; columnCounter++)
            {
                for(int rowCounter = 0; rowCounter < rowCount; rowCounter++)
                {
                    var cell = new Cell()
                    {
                        Id = cellCounter,
                        Height = height,
                        Width = width,
                        Column = columnCounter,
                        Row = rowCounter,
                    };

                    _algorithm.ShapeLocationCells.Add(cell);
                    height += singleCellHeight;
                    cellCounter++;
                }

                height = singleCellHeight;
                width += singleCellWidth;
            }
        }

        private Thickness GenerateThicknessAroundCell(Cell cell)
        {
            int radius = (int)Math.Round((double)_algorithm.ShapeLocationCells.Count() / 10, 0);

            if(radius >= _config.ColumnCount || radius >= _config.RowCount)
            {
                radius = radius / 2;
            }

            int maxConfigRow = _config.RowCount - 1;
            int maxConfigColumn = _config.ColumnCount - 1;

            int minCol = 0;
            int maxCol = 0;

            int minRow = 0;
            int maxRow = 0;
            
            if(cell.Row - radius < 0)
            {
                minRow = cell.Row;
                maxRow = cell.Row + radius;
            } else if (cell.Row + radius > maxConfigRow)
            {
                minRow = cell.Row - radius;
                maxRow = cell.Row;
            } else
            {
                minRow = cell.Row - radius;
                maxRow = cell.Row + radius;
            }

            if(cell.Column - radius < 0)
            {
                minCol = cell.Column;
                maxCol = cell.Column + radius;
            } else if (cell.Column + radius > maxConfigColumn)
            {
                minCol = cell.Column - radius;
                maxCol = cell.Column;
            } else
            {
                minCol = cell.Column - radius;
                maxCol = cell.Column + radius;
            }

            var randomRow = _rnd.Next(minRow, maxRow);
            var randomCol = _rnd.Next(minCol, maxCol);

            var selectedCell = _algorithm.ShapeLocationCells.Where(c => c.Row == randomRow && c.Column == randomCol).FirstOrDefault();
            if (selectedCell == null)
            {

            }
            return GetRandomValueBasedOnCell(selectedCell);
        }

        private Thickness GetRandomValueBasedOnCell(Cell cell)
        {
            Cell nextCell = null;
            nextCell = _algorithm.ShapeLocationCells.Where(c => c.Column == cell.Column + 1 && c.Row == cell.Row).FirstOrDefault();
            if(cell.Column == _config.ColumnCount - 1)
            {
                nextCell = cell;
                cell = _algorithm.ShapeLocationCells.Where(c => c.Column == cell.Column - 1 && c.Row == cell.Row).FirstOrDefault();
            }

            int randomWidth = _rnd.Next(cell.Width, nextCell.Width);
            int randomHeight = _rnd.Next(cell.Height, nextCell.Height);

            return new Thickness(randomWidth, randomHeight, 1, 1);
        }
    }

    public class CellDNA
    {
        public int Id;
        public int Height;
        public int Width;
        public int Column;
        public int Row;
    }
    public class PaintingShapeDNA
    {
        public Type type;
        public CellDNA Cell;
        public Brush Fill;
        public Brush Border;
        public double Width;
        public double Height;
        public double AngleX;
        public double AngleY;
        public double BorderThickness;
        public Geometry DefiningGeometry;
        public Thickness Margin;
    }

    public class PaintingDNA
    {
        public Brush Background;
        public int MayLiveUntil;
        public int MutationCount;
        public int IterationsSurvived;
        public string Shapes;
    }

    public class AlgorithmDNA
    {
        public int Iteration;
        public string DateCreated;
        public PaintingDNA LeftPanel;
        public PaintingDNA RightPanel;
        public List<PaintingDNA> Generations;
    }
}
