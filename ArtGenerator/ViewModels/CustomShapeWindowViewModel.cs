using ArtGenerator.Models;
using ArtGenerator.Models.CustomShapes;
using ArtGenerator.Services;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArtGenerator.ViewModels
{
    public class CustomShapeWindowViewModel : BindableBase
    {
        private string _activeTool;
        public string ActiveTool
        {
            get { return _activeTool; }
            set { SetProperty(ref _activeTool, value); }
        }

        private int _imageLeft;
        public int ImageLeft
        {
            get { return _imageLeft; }
            set { SetProperty(ref _imageLeft, value); }
        }

        private int _imageTop;
        public int ImageTop
        {
            get { return _imageTop; }
            set { SetProperty(ref _imageTop, value); }
        }

        private double _imageScale;
        public double ImageScale
        {
            get { return _imageScale; }
            set { SetProperty(ref _imageScale, value); }
        }

        private Brush _backgroundImage;
        public Brush BackgroundImage
        {
            get { return _backgroundImage; }
            set { SetProperty(ref _backgroundImage, value); }
        }

        private Brush _penBackground;
        public Brush PenBackground
        {
            get { return _penBackground; }
            set { SetProperty(ref _penBackground, value); }
        }

        private Brush _eraserBackground;
        public Brush EraserBackground
        {
            get { return _eraserBackground; }
            set { SetProperty(ref _eraserBackground, value); }
        }

        private Brush _holdBackground;
        public Brush HoldBackground
        {
            get { return _holdBackground; }
            set { SetProperty(ref _holdBackground, value); }
        }

        private Brush _zoomBackground;
        public Brush ZoomBackground
        {
            get { return _zoomBackground; }
            set { SetProperty(ref _zoomBackground, value); }
        }

        private Brush _includeFirstBackground;
        public Brush IncludeFirstBackground
        {
            get { return _includeFirstBackground; }
            set { SetProperty(ref _includeFirstBackground, value); }
        }

        private Brush _includeSecondBackground;
        public Brush IncludeSecondBackground
        {
            get { return _includeSecondBackground; }
            set { SetProperty(ref _includeSecondBackground, value); }
        }

        private Brush _includeThirdBackground;
        public Brush IncludeThirdBackground
        {
            get { return _includeThirdBackground; }
            set { SetProperty(ref _includeThirdBackground, value); }
        }
        private Brush _excludeFirstBackground;
        public Brush ExcludeFirstBackground
        {
            get { return _excludeFirstBackground; }
            set { SetProperty(ref _excludeFirstBackground, value); }
        }

        private Brush _excludeSecondBackground;
        public Brush ExcludeSecondBackground
        {
            get { return _excludeSecondBackground; }
            set { SetProperty(ref _excludeSecondBackground, value); }
        }

        private Brush _excludeThirdBackground;
        public Brush ExcludeThirdBackground
        {
            get { return _excludeThirdBackground; }
            set { SetProperty(ref _excludeThirdBackground, value); }
        }

        private Brush _trueMaskBackground;
        public Brush TrueMaskBackground
        {
            get { return _trueMaskBackground; }
            set { SetProperty(ref _trueMaskBackground, value); }
        }

        private ObservableCollection<PaintingShape> _linesOnCanvas;
        public ObservableCollection<PaintingShape> LinesOnCanvas
        {
            get { return _linesOnCanvas; }
            set { SetProperty(ref _linesOnCanvas, value); }
        }

        private ObservableCollection<PaintingShape> _toolCanvas;
        public ObservableCollection<PaintingShape> ToolCanvas
        {
            get { return _toolCanvas; }
            set { SetProperty(ref _toolCanvas, value); }
        }

        private bool _isInfoPageVisible;
        public bool IsInfoPageVisible
        {
            get { return _isInfoPageVisible; }
            set { SetProperty(ref _isInfoPageVisible, value); }
        }

        public ICommand UsingToolOnCanas => _usingToolOnCanas;
        private readonly DelegateCommand<EventArgs> _usingToolOnCanas;

        public ICommand HoverOverCanvas => _hoverOverCanvas;
        private readonly DelegateCommand<EventArgs> _hoverOverCanvas;

        public ICommand MouseWheelUsedOnCanvas => _mouseWheelUsedOnCanvas;
        private readonly DelegateCommand<EventArgs> _mouseWheelUsedOnCanvas;

        public ICommand OnPressingKey => _onPressingKey;
        private readonly DelegateCommand<EventArgs> _onPressingKey;

        public ICommand ActiveToolCommand => _activeToolCommand;
        private readonly DelegateCommand<String> _activeToolCommand;

        public ICommand MoveBackgroundImageCommand => _moveBackgroundImageCommand;
        private readonly DelegateCommand<String> _moveBackgroundImageCommand;

        public ICommand SelectDrawingBlock => _selectDrawingBlock;
        private readonly DelegateCommand<String> _selectDrawingBlock;

        public ICommand ChangeBackgroundImage => _changeBackgroundImage;
        private readonly DelegateCommand _changeBackgroundImage;

        public ICommand SaveGeometryCommand => _saveGeometryCommand;
        private readonly DelegateCommand _saveGeometryCommand;

        public ICommand SwitchShapeView => _switchShapeView;
        private readonly DelegateCommand _switchShapeView;

        public ICommand ReloadOldSavedShape => _reloadOldSavedShape;
        private readonly DelegateCommand _reloadOldSavedShape;

        public ICommand ClearCanvasOfShapes => _clearCanvasOfShapes;
        private readonly DelegateCommand _clearCanvasOfShapes;

        public ICommand UndoPreviousEventCommand => _undoPreviousEventCommand;
        private readonly DelegateCommand _undoPreviousEventCommand;

        public ICommand SwitchVisibilityInfoPage => _switchVisibilityInfoPage;
        private readonly DelegateCommand _switchVisibilityInfoPage;

        private int _chosenBrush = 0;
        private bool _seeTrueMask = false;
        private Point _lastPosition = new();
        private Point _LastHoldPosition = new(0, 0);
        private readonly List<ShapePointers> _shapes = new();
        private Brush _unselected = Brushes.LightGray;
        private Brush _selected = Brushes.GreenYellow;
        private List<HistoryItem> _history = new();
        private IConfigService _configService { get; set; }

        public CustomShapeWindowViewModel(IConfigService configService)
        {
            _configService = configService;

            ImageTop = 0;
            ImageLeft = 0;
            ImageScale = 1;
            ToolCanvas = new();
            ActiveTool = "Pen";
            LinesOnCanvas = new();
            BackgroundImage = Brushes.AliceBlue;
            IsInfoPageVisible = false;

            IncludeFirstBackground = _selected;
            IncludeSecondBackground = _unselected;
            IncludeThirdBackground = _unselected;
            ExcludeFirstBackground = _unselected;
            ExcludeSecondBackground = _unselected;
            ExcludeThirdBackground = _unselected;
            TrueMaskBackground = _unselected;
            EraserBackground = _unselected;
            HoldBackground = _unselected;
            ZoomBackground = _unselected;
            PenBackground = _selected;

            for (int i = 0; i < 6; i++) _shapes.Add(new());

            _clearCanvasOfShapes = new(() => ClearCanvas());
            _saveGeometryCommand = new(() => SaveGeometry());
            _switchShapeView = new(() => SwitchBetweenMasks());
            _undoPreviousEventCommand = new(() => UndoLastEvent());
            _reloadOldSavedShape = new(() => ReturnOldSavedShape());
            _changeBackgroundImage = new(() => FindBackgroundImage());
            _switchVisibilityInfoPage = new(() => IsInfoPageVisible = IsInfoPageVisible == false);

            _onPressingKey = new((a) => PressingKeyEvent(a));
            _usingToolOnCanas = new((a) => ClickedOnCanvas(a));
            _activeToolCommand = new((a) => ChangeActiveTool(a));
            _hoverOverCanvas = new((a) => MouseHoverOverCanvas(a));
            _selectDrawingBlock = new((a) => ChangeDrawingMask(a));
            _moveBackgroundImageCommand = new((a) => MoveImageOnCanvas(a));
            _mouseWheelUsedOnCanvas = new((a) => MouseWheelScrollOverCanvas(a));

            LoadInFromConfig(configService.GetConfig().CustomShape);
        }

        /// <summary>
        /// Event arguments for
        /// </summary>
        /// <param name="eventArgs"></param>
        private void PressingKeyEvent(EventArgs eventArgs)
        {
            KeyboardEventArgs KeyEventArgs = (KeyboardEventArgs)eventArgs;
            var keyboard = KeyEventArgs.KeyboardDevice;

            // Just Z instead of Ctrl + Z is simpler
            if (keyboard.IsKeyDown(Key.Z))
            {
                UndoLastEvent();
            }
            else if (keyboard.IsKeyDown(Key.LeftCtrl) && keyboard.IsKeyDown(Key.W))
            {
                MoveImageOnCanvas("zoomin");
            }
            else if (keyboard.IsKeyDown(Key.LeftCtrl) && keyboard.IsKeyDown(Key.S))
            {
                MoveImageOnCanvas("zoomout");
            }
            else if (keyboard.IsKeyDown(Key.D4) || (keyboard.IsKeyDown(Key.LeftCtrl) && keyboard.IsKeyDown(Key.D1)) || keyboard.IsKeyDown(Key.NumPad4))
            {
                ChangeDrawingMask("excludeFirst");
            }
            else if (keyboard.IsKeyDown(Key.D5) || (keyboard.IsKeyDown(Key.LeftCtrl) && keyboard.IsKeyDown(Key.D2)) || keyboard.IsKeyDown(Key.NumPad5))
            {
                ChangeDrawingMask("excludeSecond");
            }
            else if (keyboard.IsKeyDown(Key.D6) || (keyboard.IsKeyDown(Key.LeftCtrl) && keyboard.IsKeyDown(Key.D3)) || keyboard.IsKeyDown(Key.NumPad6))
            {
                ChangeDrawingMask("excludeThird");
            }
            else if (keyboard.IsKeyDown(Key.W) || keyboard.IsKeyDown(Key.Up))
            {
                MoveImageOnCanvas("up");
            }
            else if (keyboard.IsKeyDown(Key.S) || keyboard.IsKeyDown(Key.Down))
            {
                MoveImageOnCanvas("down");
            }
            else if (keyboard.IsKeyDown(Key.A) || keyboard.IsKeyDown(Key.Left))
            {
                MoveImageOnCanvas("left");
            }
            else if (keyboard.IsKeyDown(Key.D) || keyboard.IsKeyDown(Key.Right))
            {
                MoveImageOnCanvas("right");
            }
            else if (keyboard.IsKeyDown(Key.D1) || keyboard.IsKeyDown(Key.NumPad1))
            {
                ChangeDrawingMask("includeFirst");
            }
            else if (keyboard.IsKeyDown(Key.D2) || keyboard.IsKeyDown(Key.NumPad2))
            {
                ChangeDrawingMask("includeSecond");
            }
            else if (keyboard.IsKeyDown(Key.D3) || keyboard.IsKeyDown(Key.NumPad3))
            {
                ChangeDrawingMask("includeThird");
            }
        }

        /// <summary>
        /// Calculates whether the used scrolled up or down
        /// </summary>
        /// <param name="eventArgs">Event data provided by the EventArgs argument</param>
        private void MouseWheelScrollOverCanvas(EventArgs eventArgs)
        {
            MouseWheelEventArgs scrollEvent = (MouseWheelEventArgs)eventArgs;
            if (scrollEvent.Delta > 0)
            {
                MoveImageOnCanvas("zoomin");
            }
            else
            {
                MoveImageOnCanvas("zoomout");
            }
        }

        /// <summary>
        /// Checks if the user is hovering over the canvas
        /// </summary>
        /// <param name="eventArgs">Event data provided by the EventArgs argument</param>
        private void MouseHoverOverCanvas(EventArgs eventArgs)
        {
            if (ToolCanvas.Count > 0) ToolCanvas.Clear();

            MouseEventArgs mouseEvent = (MouseEventArgs)eventArgs;
            Point position = mouseEvent.GetPosition(mouseEvent.Device.Target);
            position = new((int)position.X, (int)position.Y);

            if (ActiveTool == "Eraser")
            {
                PaintingShape shape = new Ellipse()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(position.X, position.Y, 0, 0)
                };

                ToolCanvas.Add(shape);
            }

            if (mouseEvent.LeftButton == MouseButtonState.Released)
            {
                if (ActiveTool == "Pen" && _shapes[_chosenBrush].Points.Count != 0)
                {
                    var thisList = _shapes[_chosenBrush].Points;
                    var lastItem = thisList[thisList.Count - 1];

                    if (Point.Subtract(lastItem, position).Length > 2)
                    {
                        if (position != _lastPosition)
                        {
                            if (lastItem != _lastPosition)
                            {
                                thisList.RemoveAt(thisList.Count - 1);
                            }
                            thisList.Add(position);
                            AllPointsinGeometry();
                        }
                    }
                }
            }
            else if (ActiveTool == "Hold")
            {
                ImageLeft = (int)Math.Floor(position.X - _LastHoldPosition.X);
                ImageTop = (int)Math.Floor(position.Y - _LastHoldPosition.Y);
            }
            else if (ActiveTool != "Zoom")
            {
                ClickedOnCanvas(eventArgs);
            }

        }

        /// <summary>
        /// Will choose what action to take based on the tool used
        /// </summary>
        /// <param name="eventArgs">Event data provided by the EventArgs argument</param>
        private void ClickedOnCanvas(EventArgs eventArgs)
        {
            _LastHoldPosition = new(0, 0);
            switch (ActiveTool)
            {
                case "Pen":
                    UsedPenTool(eventArgs);
                    break;
                case "Eraser":
                    UsedEraserTool(eventArgs);
                    break;
                case "Hold":
                    UsedHoldTool(eventArgs);
                    break;
                case "Zoom":
                    UsedZoomTool(eventArgs);
                    break;
            }
        }

        /// <summary>
        /// Adds a new point based on where the user clicked
        /// </summary>
        /// <param name="eventArgs">Event data provided by the EventArgs argument</param>
        private void UsedPenTool(EventArgs eventArgs)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)eventArgs;
            Point position = mouseEvent.GetPosition(mouseEvent.Device.Target);
            Point clickedPosition = new Point((int)position.X, (int)position.Y);
            _shapes[_chosenBrush].Points.Add(clickedPosition);
            _lastPosition = clickedPosition;

            HistoryItem customEvent = new()
            {
                Location = clickedPosition,
                Type = "Pen",
                ChosenMask = _chosenBrush,
                ArrayPosition = _shapes[_chosenBrush].Points.IndexOf(clickedPosition)
            };
            _history.Insert(0, customEvent);

            AllPointsinGeometry();
        }

        /// <summary>
        /// Removes points that were close to where the user clicked
        /// </summary>
        /// <param name="eventArgs">Event data provided by the EventArgs argument</param>
        private void UsedEraserTool(EventArgs eventArgs)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)eventArgs;
            Point position = mouseEvent.GetPosition(mouseEvent.Device.Target);

            List<Point> plannedForRemoval = new();

            foreach (Point point in _shapes[_chosenBrush].Points)
            {
                if (Point.Subtract(point, position).Length < 15)
                {
                    plannedForRemoval.Add(point);
                }
            }

            foreach (Point point in plannedForRemoval)
            {
                HistoryItem customEvent = new()
                {
                    Location = point,
                    Type = "Eraser",
                    ChosenMask = _chosenBrush,
                    ArrayPosition = _shapes[_chosenBrush].Points.IndexOf(point)
                };
                _history.Insert(0, customEvent);

                _shapes[_chosenBrush].Points.Remove(point);
            }

            AllPointsinGeometry();
        }

        /// <summary>
        /// Sets the first position that was clicked so it can be moved
        /// </summary>
        /// <param name="eventArgs">Event data provided by the EventArgs argument</param>
        private void UsedHoldTool(EventArgs eventArgs)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)eventArgs;
            Point position = mouseEvent.GetPosition(mouseEvent.Device.Target);

            _LastHoldPosition = new(position.X, position.Y);
        }

        /// <summary>
        /// Uses the zoom tool
        /// </summary>
        /// <param name="eventArgs">Event data provided by the EventArgs argument</param>
        private void UsedZoomTool(EventArgs eventArgs)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)eventArgs;

            ImageScale += 0.1;
        }

        /// <summary>
        /// Removes the last Event that occured
        /// </summary>
        private void UndoLastEvent()
        {
            if (_history.Count != 0)
            {
                var item = _history[0];
                var thisList = _shapes[item.ChosenMask].Points;

                if (item.Type == "Pen")
                {
                    thisList.Remove(item.Location);
                }
                else if (item.Type == "Eraser")
                {
                    try
                    {
                        thisList.Insert(item.ArrayPosition, item.Location);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        thisList.Add(item.Location);
                    }
                }

                _history.RemoveAt(0);

                thisList = _shapes[_chosenBrush].Points;

                if (thisList.Last() != _lastPosition)
                {
                    thisList.Remove(thisList.Last());
                }

                AllPointsinGeometry();
            }
        }

        /// <summary>
        /// Clears all the data on the canvas
        /// </summary>
        private void ClearCanvas()
        {
            for (int i = 0; i < 6; i++) _shapes[i].Points.Clear();
            LinesOnCanvas.Clear();
            AllPointsinGeometry();
        }

        /// <summary>
        /// Returns the old saved shape that was in the config.
        /// </summary>
        private void ReturnOldSavedShape()
        {
            _configService.LoadConfig();
            for (int i = 0; i < 6; i++) _shapes[i].Points.Clear();
            LinesOnCanvas.Clear();
            LoadInFromConfig(_configService.GetConfig().CustomShape);
            AllPointsinGeometry();
        }

        /// <summary>
        /// Changes the active tool that was clicked by the user
        /// </summary>
        /// <param name="clickedTool">The tool the user clicked</param>
        private void ChangeActiveTool(string clickedTool)
        {
            PenBackground = _unselected;
            EraserBackground = _unselected;
            HoldBackground = _unselected;
            ZoomBackground = _unselected;

            ActiveTool = clickedTool;

            switch (clickedTool)
            {
                case "Pen":
                    PenBackground = _selected;
                    break;
                case "Eraser":
                    EraserBackground = _selected;
                    var thisList = _shapes[_chosenBrush].Points;
                    if (thisList.Count != 0 && thisList[thisList.Count - 1] != _lastPosition)
                    {
                        thisList.RemoveAt(thisList.Count - 1);
                        AllPointsinGeometry();
                    }
                    break;
                case "Hold":
                    HoldBackground = _selected;
                    break;
                case "Zoom":
                    ZoomBackground = _selected;
                    break;
            }
        }

        /// <summary>
        /// Changes the mask that was selected
        /// </summary>
        /// <param name="itemSelected">The mask that was clicked</param>
        private void ChangeDrawingMask(string itemSelected)
        {
            IncludeFirstBackground = _unselected;
            IncludeSecondBackground = _unselected;
            IncludeThirdBackground = _unselected;
            ExcludeFirstBackground = _unselected;
            ExcludeSecondBackground = _unselected;
            ExcludeThirdBackground = _unselected;

            var thisList = _shapes[_chosenBrush].Points;

            if (thisList.Count != 0 && thisList.Last() != _lastPosition)
            {
                thisList.Remove(thisList.Last());
            }

            switch (itemSelected)
            {
                case "includeFirst":
                    _chosenBrush = 0;
                    IncludeFirstBackground = _selected;
                    break;
                case "includeSecond":
                    _chosenBrush = 1;
                    IncludeSecondBackground = _selected;
                    break;
                case "includeThird":
                    _chosenBrush = 2;
                    IncludeThirdBackground = _selected;
                    break;
                case "excludeFirst":
                    _chosenBrush = 3;
                    ExcludeFirstBackground = _selected;
                    break;
                case "excludeSecond":
                    _chosenBrush = 4;
                    ExcludeSecondBackground = _selected;
                    break;
                case "excludeThird":
                    _chosenBrush = 5;
                    ExcludeThirdBackground = _selected;
                    break;
            }

            thisList = _shapes[_chosenBrush].Points;

            if (thisList.Count != 0)
            {
                _lastPosition = thisList.Last();
            }

            AllPointsinGeometry();
        }

        /// <summary>
        /// Moves the image based on input given
        /// </summary>
        /// <param name="direction">The action to take</param>
        private void MoveImageOnCanvas(string direction)
        {
            switch (direction)
            {
                case "up":
                    ImageTop -= 2;
                    break;
                case "left":
                    ImageLeft -= 2;
                    break;
                case "right":
                    ImageLeft += 2;
                    break;
                case "down":
                    ImageTop += 2;
                    break;
                case "zoomin":
                    ImageScale += 0.04;
                    break;
                case "zoomout":
                    if (ImageScale > 0.5)
                    {
                        ImageScale -= 0.04;
                    }
                    break;
            }
        }

        /// <summary>
        /// Switches the masks so users can see how the image will turn out
        /// </summary>
        public void SwitchBetweenMasks()
        {
            _seeTrueMask = _seeTrueMask == false;
            TrueMaskBackground = _seeTrueMask ? _selected : _unselected;
            AllPointsinGeometry();
        }

        /// <summary>
        /// Saves The Geometry into the class
        /// </summary>
        private void SaveGeometry()
        {
            string savedString = "";
            int waitAmount = 0;
            foreach (ShapePointers points in _shapes)
            {
                if (points.Points.Count != 0)
                {
                    foreach (Point point in points.Points)
                    {
                        savedString += point.X + "." + point.Y + ",";
                    }
                    if (waitAmount != 0)
                    {
                        while (waitAmount > 0)
                        {
                            savedString += ";";
                            waitAmount--;
                        }
                    }
                }
                else if (savedString == "")
                {
                    waitAmount++;
                    continue;
                }
                savedString += ";";
            }

            if (savedString == "" || waitAmount > 5)
            {
                MessageBox.Show("Je hebt niets geselecteerd!");
                return;
            }
            _configService.GetConfig().CustomShape = savedString;
            _configService.SaveConfig();
        }

        /// <summary>
        /// Opens a prompt so the user can look for an image and will add it if one was added
        /// </summary>
        private void FindBackgroundImage()
        {
            OpenFileDialog dlg = new();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "All Types";
            dlg.Filter = "All Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|" +
                "BMP|*.bmp|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                ImageBrush imageBrush = new();
                imageBrush.ImageSource = new BitmapImage(new Uri(dlg.FileName, UriKind.Relative));
                BackgroundImage = imageBrush;
            }
        }

        /// <summary>
        /// Changes the selected item to a different color.
        /// </summary>
        private void ChangeSelectedBrush()
        {
            if (_shapes[_chosenBrush].Points.Count != 0 && !_seeTrueMask)
            {
                Point first = _shapes[_chosenBrush].Points[0];
                List<PathSegment> segments = new();

                foreach (Point point in _shapes[_chosenBrush].Points)
                {
                    segments.Add(new LineSegment(point, true));
                }

                List<PathFigure> pathFigure = new() { new(first, segments, true) };
                PaintingShape shape = new CustomShape(new PathGeometry(pathFigure, FillRule.EvenOdd, null))
                {
                    StrokeThickness = 2,
                    Stroke = Brushes.GreenYellow,
                    Width = 500,
                    Height = 500,
                    Margin = new Thickness(1, 1, 1, 1)
                };
                if (LinesOnCanvas.Count > 1)
                {
                    LinesOnCanvas.RemoveAt(1);
                }
                LinesOnCanvas.Add(shape);
            }
            else if (LinesOnCanvas.Count > 1)
            {
                LinesOnCanvas.RemoveAt(1);
            }
        }

        /// <summary>
        /// Reads the configs data and makes a shape from everything within it
        /// </summary>
        /// <param name="savedShape">The configs save data</param>
        private void LoadInFromConfig(string savedShape)
        {
            List<string> shapes = savedShape.Split(";").ToList();

            foreach (string points in shapes)
            {
                if (points.Length == 0 || points == "")
                {
                    continue;
                }

                foreach (string point in points.Split(","))
                {
                    List<string> split = point.Split(".").ToList();

                    if (split.Count <= 1 || point.Length == 0 || split[0] == "0" || split[1] == "0")
                    {
                        continue;
                    }

                    Point restoredPoint = new(int.Parse(split[0]), int.Parse(split[1]));

                    _shapes[shapes.IndexOf(points)].Points.Add(restoredPoint);
                }
            }

            _lastPosition = _shapes[0].Points[_shapes[0].Points.Count - 1];
            AllPointsinGeometry();
        }

        /// <summary>
        /// Makes a custom shape based on the saved shapes
        /// </summary>
        private void AllPointsinGeometry()
        {
            LinesOnCanvas.Clear();

            List<PathFigure> path = new();
            List<PathFigure> cuts = new();

            foreach (ShapePointers points in _shapes)
            {
                if (points.Points.Count != 0)
                {
                    Point first = points.Points[0];
                    List<PathSegment> segments = new();

                    foreach (Point point in points.Points)
                    {
                        segments.Add(new LineSegment(point, true));
                    }

                    PathFigure pathFigure = new(first, segments, true);

                    if (_shapes.IndexOf(points) <= 2)
                    {
                        path.Add(pathFigure);
                    }
                    else
                    {
                        cuts.Add(pathFigure);
                    }
                }
            }

            Geometry shape;
            if (_seeTrueMask)
            {
                CombinedGeometry cutForm = new();
                cutForm.Geometry1 = new PathGeometry(path, FillRule.EvenOdd, null);
                cutForm.Geometry2 = new PathGeometry(cuts, FillRule.EvenOdd, null);
                cutForm.GeometryCombineMode = GeometryCombineMode.Exclude;
                shape = cutForm;
            }
            else
            {
                path.AddRange(cuts);
                shape = new PathGeometry(path, FillRule.EvenOdd, null);
            }

            PaintingShape paintingShape = new CustomShape(shape)
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                Width = 500,
                Height = 500,
                Margin = new Thickness(1, 1, 1, 1)
            };

            LinesOnCanvas.Add(paintingShape);
            ChangeSelectedBrush();
        }
    }

    public class ShapePointers
    {
        public List<Point> Points = new();
    }

    public class HistoryItem
    {
        public string Type { get; set; }
        public int ChosenMask { get; set; }
        public int ArrayPosition { get; set; }
        public Point Location { get; set; }
    }
}
