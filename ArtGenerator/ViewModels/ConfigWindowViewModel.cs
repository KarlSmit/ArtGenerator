using ArtGenerator.Models;
using ArtGenerator.Models.CustomShapes;
using ArtGenerator.Services;
using ArtGenerator.Views;
using ArtGenerator.Views.Config;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;

namespace ArtGenerator.ViewModels
{
    public class ConfigWindowViewModel : BindableBase, IDialogAware
    {
        private IConfigService _configService { get; set; }
        public Variables Variables { get; set; }

        //  ICommand EnableShapeCommand executed when clicked on a button
        private readonly DelegateCommand<string> _enableShapeCommand;
        public ICommand EnableShapeCommand => _enableShapeCommand;

        // Whith LineBackground you can change the color of the line button
        private Brush _lineBackground;
        public Brush LineBackground
        {
            get { return _lineBackground; }
            set { SetProperty(ref _lineBackground, value); }
        }

        // Whith CurvedLineBackground you can change the color of the curvedline button
        private Brush _curvedLineBackground;
        public Brush CurvedLineBackground
        {
            get { return _curvedLineBackground; }
            set { SetProperty(ref _curvedLineBackground, value); }
        }

        // Whith CirkelBackground you can change the color of the cirkel button
        private Brush _circleBackground;
        public Brush CircleBackground
        {
            get { return _circleBackground; }
            set { SetProperty(ref _circleBackground, value); }
        }

        // Whith SquareBackground you can change the color of the square button
        private Brush _squareBackground;
        public Brush SquareBackground
        {
            get { return _squareBackground; }
            set { SetProperty(ref _squareBackground, value); }
        }

        // Whith PointBackground you can change the color of the point button
        private Brush _pointBackground;
        public Brush PointBackground
        {
            get { return _pointBackground; }
            set { SetProperty(ref _pointBackground, value); }
        }

        // Whith TriangleBackground you can change the color of the triangle button
        private Brush _triangleBackground;
        public Brush TriangleBackground
        {
            get { return _triangleBackground; }
            set { SetProperty(ref _triangleBackground, value); }
        }

        // Whith StarBackground you can change the color of the star button
        private Brush _starBackground;
        public Brush StarBackground
        {
            get { return _starBackground; }
            set { SetProperty(ref _starBackground, value); }
        }

        // Whith LineBackground you can change the color of the line button
        private Brush _scissorsBackground;
        public Brush ScissorsBackground
        {
            get { return _scissorsBackground; }
            set { SetProperty(ref _scissorsBackground, value); }
        }

        // Whith CustomShapeBackground you can change the color of the line button
        private Brush _customShapeBackground;
        public Brush CustomShapeBackground
        {
            get { return _customShapeBackground; }
            set { SetProperty(ref _customShapeBackground, value); }
        }

        // Whith CustomShapeBackground you can change the color of the line button
        private Brush _locationDetectionBackground;
        public Brush LocationDetectionBackground
        {
            get { return _locationDetectionBackground; }
            set { SetProperty(ref _locationDetectionBackground, value); }
        }

        private string _popupText;
        public string PopupText
        {
            get { return _popupText; }
            set { SetProperty(ref _popupText, value); }
        }

        // BeginView is de start pagina van de config
        public object BeginView { get; set; }

        public ICommand OpenShapesControl => _shapesControl;
        private readonly DelegateCommand _shapesControl;
        public ICommand OpenIterationsControl => _iterationsControl;
        private readonly DelegateCommand _iterationsControl;
        public ICommand OpenCustomShapesControl => _customShapesControl;
        private readonly DelegateCommand _customShapesControl;
        public ICommand OpenMiscalleneousControl => _openMiscellaneousControl;
        private readonly DelegateCommand _openMiscellaneousControl;
        public ICommand SaveConfigCommand => _saveConfig;
        private readonly DelegateCommand _saveConfig;
        public ICommand OnShapeChange => _onShapeChange;
        private readonly DelegateCommand _onShapeChange;
        public ICommand CancelConfigCommand => _cancelConfigChanges;
        private readonly DelegateCommand _cancelConfigChanges;
        public ICommand ResetConfigCommand => _resetToDefaultConfig;
        private readonly DelegateCommand _resetToDefaultConfig;
        public ICommand OpenFolderWithPaintings => _openFolderWithPaintings;
        private readonly DelegateCommand _openFolderWithPaintings;

        // Navigation pages that can be opened
        private Iteration _iterationControl = new();
        private Shapes _shapeControl = new();
        private CustomShapeWindow _customShapeControl;

        private ObservableCollection<PaintingShape> _testShapes;
        public ObservableCollection<PaintingShape> TestShapes { get { return _testShapes; } set { SetProperty(ref _testShapes, value); } }

        // An event for when a request of closing the window is sent -- part of IDialogAware
        public event Action<IDialogResult> RequestClose;

        public string Title => "Configuratiescherm";
        public ConfigWindowViewModel(IConfigService configService)
        {
            _customShapeControl = new(new CustomShapeWindowViewModel(configService));
            _configService = configService;

            _configService.LoadLanguage(Application.Current);

            var item = Application.Current.Resources.MergedDictionaries.Where(l => l.Source != null && l.Source.ToString().StartsWith("Languages"));

            PopupText = item.First()["ConfigWindow.ConfigChanged"].ToString();

            Variables = configService.GetConfig();
            TestShapes = new();

            _shapesControl = new DelegateCommand(() => ForthToPage(_shapeControl));
            _iterationsControl = new DelegateCommand(() => ForthToPage(_iterationControl));
            _customShapesControl = new DelegateCommand(() => ForthToPage(_customShapeControl));
            _saveConfig = new DelegateCommand(() => SaveConfig());
            _enableShapeCommand = new DelegateCommand<string>((a) => ChangeBackgroundCommand(a));
            _onShapeChange = new DelegateCommand(() => UpdateShapes());
            _cancelConfigChanges = new DelegateCommand(() => ResetConfig());
            _resetToDefaultConfig = new DelegateCommand(() => ResetToDefaultConfig());
            _openFolderWithPaintings = new(() => GoToPaintingFolder());


            LoadConfigData();
            _configService.SaveConfig();

            ForthToPage(_shapeControl);
        }

        private static void GoToPaintingFolder()
        {
            Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArtGenerator");
        }

        private async void ChangeTextToSaved()
        {
            Variables.HasChanged = true;
            var item = Application.Current.Resources.MergedDictionaries.Where(l => l.Source != null && l.Source.ToString().StartsWith("Languages"));
            PopupText = item.First()["ConfigWindow.ConfigSaved"].ToString();

            await Task.Delay(TimeSpan.FromSeconds(3));

            Variables.HasChanged = false;

            await Task.Delay(TimeSpan.FromSeconds(1));
            PopupText = item.First()["ConfigWindow.ConfigChanged"].ToString();
        }

        private void SaveConfig()
        {
            _configService.SaveConfig();
            Task.Run(() => ChangeTextToSaved());
        }

        private void ResetToDefaultConfig()
        {
            _configService.LoadDefaultConfig();
            _configService.SaveConfig();
            ResetConfig();
        }

        public void ResetConfig()
        {
            _configService.LoadConfig();
            UpdateShapes();
            LoadConfigData();
            Variables.HasChanged = false;
        }
        
        public void UpdateShapes()
        {
            TestShapes.Clear();
            PaintingShape max = new Triangle();
            max.Margin = new Thickness(10, 10, 0, 0);
            max.Width = _configService.GetConfig().MaxShapeSize;
            max.Height = _configService.GetConfig().MaxShapeSize;
            max.Fill = Brushes.Orange;
            max.Stroke = Brushes.Black;
            max.StrokeThickness = _configService.GetConfig().MaxBorderSize;
            TestShapes.Add(max);

            PaintingShape min = new Triangle();
            int calculatedMargin = (_configService.GetConfig().MaxShapeSize / 2) - (_configService.GetConfig().MinShapeSize / 2);
            min.Margin = new Thickness(10 + calculatedMargin, 10 + calculatedMargin, 0, 0);
            min.Width = _configService.GetConfig().MinShapeSize;
            min.Height = _configService.GetConfig().MinShapeSize;
            min.Fill = Brushes.Blue;
            min.Stroke = Brushes.Black;
            min.StrokeThickness = _configService.GetConfig().MinBorderSize;
            TestShapes.Add(min);
        }

        // A button change his color to green or white when clicked on
        public void ChangeBackgroundCommand(string shape)
        {
            var selectedColor = Brushes.GreenYellow;
            var unselectedColor = Brushes.OrangeRed;

            switch (shape)
            {
                case "Line":
                    LineBackground = (LineBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowLine = LineBackground == selectedColor;
                    break;
                case "Curvedline":
                    CurvedLineBackground = (CurvedLineBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowCurvedLine = CurvedLineBackground == selectedColor;
                    break;
                case "Circle":
                    CircleBackground = (CircleBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowCircle = CircleBackground == selectedColor;
                    break;
                case "Square":
                    SquareBackground = (SquareBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowRectangle = SquareBackground == selectedColor;
                    break;
                case "Point":
                    PointBackground = (PointBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowDot = PointBackground == selectedColor;
                    break;
                case "Triangle":
                    TriangleBackground = (TriangleBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowTriangle = TriangleBackground == selectedColor;
                    break;
                case "Star":
                    StarBackground = (StarBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowStar = StarBackground == selectedColor;
                    break;
                case "Scissors":
                    ScissorsBackground = (ScissorsBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowCutsInShapes = ScissorsBackground == selectedColor;
                    break;
                case "CustomShapes":
                    CustomShapeBackground = (CustomShapeBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().AllowCustomShapes = CustomShapeBackground == selectedColor;
                    break;
                case "LocationDetection":
                    LocationDetectionBackground = (LocationDetectionBackground == selectedColor) ? unselectedColor : selectedColor;
                    _configService.GetConfig().LocationDetection = LocationDetectionBackground == selectedColor;
                    break;
            }
        }

        // load all data form the config
        private void LoadConfigData()
        {
            var unselectedColor = Brushes.OrangeRed;

            CircleBackground = unselectedColor;
            CurvedLineBackground = unselectedColor;
            LineBackground = unselectedColor;
            PointBackground = unselectedColor;
            ScissorsBackground = unselectedColor;
            SquareBackground = unselectedColor;
            StarBackground = unselectedColor;
            TriangleBackground = unselectedColor;
            CustomShapeBackground = unselectedColor;
            LocationDetectionBackground = unselectedColor;

            if (_configService.GetConfig().AllowLine)
            {
                ChangeBackgroundCommand("Line");
            }
            if (_configService.GetConfig().AllowCurvedLine)
            {
                ChangeBackgroundCommand("Curvedline");
            }
            if (_configService.GetConfig().AllowCircle)
            {
                ChangeBackgroundCommand("Circle");
            }
            if (_configService.GetConfig().AllowRectangle)
            {
                ChangeBackgroundCommand("Square");
            }
            if (_configService.GetConfig().AllowDot)
            {
                ChangeBackgroundCommand("Point");
            }
            if (_configService.GetConfig().AllowTriangle)
            {
                ChangeBackgroundCommand("Triangle");
            }
            if (_configService.GetConfig().AllowStar)
            {
                ChangeBackgroundCommand("Star");
            }
            if (_configService.GetConfig().AllowCutsInShapes)
            {
                ChangeBackgroundCommand("Scissors");
            }
            if (_configService.GetConfig().AllowCustomShapes)
            {
                ChangeBackgroundCommand("CustomShapes");
            }
            if (_configService.GetConfig().LocationDetection)
            {
                ChangeBackgroundCommand("LocationDetection");
            }

            UpdateShapes();
        }



        // ForthToPage is een functie die naar de pagina's lijdt
        private void ForthToPage(object page)
        {
            BeginView = page;

            // Raise property changed for updating the view
            RaisePropertyChanged(nameof(BeginView));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        /// <summary>
        /// Can the window be closed? Should always be true
        /// </summary>
        /// <returns></returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// To execute when the window is opened
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        /// <summary>
        /// To execute when the window is closed
        /// </summary>
        public void OnDialogClosed() { }
    }
}