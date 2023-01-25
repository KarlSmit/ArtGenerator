using Prism.Mvvm;
using System.Collections.Generic;

namespace ArtGenerator.Models
{
    public class Variables : BindableBase
    {
        private double _chanceToBeHollow;
        private double _chanceToMutate;
        private bool _allowRectangle;
        private bool _allowTriangle;
        private bool _allowCircle;
        private bool _allowLine;
        private bool _allowCurvedLine;
        private bool _allowStar;
        private bool _allowDot;
        private bool _allowCutsInShapes;
        private bool _allowCustomShapes;
        private bool _saveAllPictures;
        private bool _locationDetection;
        private bool _hasChanged;
        private int _maxShapeSize;
        private int _minShapeSize;
        private int _maxSkewing;
        private int _minSkewing;
        private int _maxBorderSize;
        private int _minBorderSize;
        private int _startCalculationsAtAmountOfParents;
        private int _addedShapesPerParent;
        private int _panelWidth;
        private int _panelHeight;
        private string _titleColor;
        private string _customShape;
        private string _backgroundColor;
        private string _paintingBackgroundColor;
        private string _chosenCulture;
        private string _chosenBackgroundType;
        private List<string> _availableLanguages;
        private List<string> _paintingBackgroundTypes;
        public double ChanceToBeHollow { get { return _chanceToBeHollow; } set { SetProperty(ref _chanceToBeHollow, value); HasChanged = true; HasChanged = true; } }
        public double ChanceToMutate { get { return _chanceToMutate; } set { SetProperty(ref _chanceToMutate, value); HasChanged = true; } }
        public bool AllowRectangle { get { return _allowRectangle; } set { SetProperty(ref _allowRectangle, value); HasChanged = true; } }
        public bool AllowTriangle { get { return _allowTriangle; } set { SetProperty(ref _allowTriangle, value); HasChanged = true; } }
        public bool AllowCircle { get { return _allowCircle; } set { SetProperty(ref _allowCircle, value); HasChanged = true; } }
        public bool AllowLine { get { return _allowLine; } set { SetProperty(ref _allowLine, value); HasChanged = true; } }
        public bool AllowCurvedLine { get { return _allowCurvedLine; } set { SetProperty(ref _allowCurvedLine, value); HasChanged = true; } }
        public bool AllowStar { get { return _allowStar; } set { SetProperty(ref _allowStar, value); HasChanged = true; } }
        public bool AllowDot { get { return _allowDot; } set { SetProperty(ref _allowDot, value); HasChanged = true; } }
        public bool AllowCutsInShapes { get { return _allowCutsInShapes; } set { SetProperty(ref _allowCutsInShapes, value); HasChanged = true; } }
        public bool AllowCustomShapes { get { return _allowCustomShapes; } set { SetProperty(ref _allowCustomShapes, value); HasChanged = true; } }
        public bool SaveAllPictures { get { return _saveAllPictures; } set { SetProperty(ref _saveAllPictures, value); HasChanged = true; } }
        public bool LocationDetection { get { return _locationDetection; } set { SetProperty(ref _locationDetection, value); HasChanged = true; } }
        public bool HasChanged { get { return _hasChanged; } set { SetProperty(ref _hasChanged, value); } }
        public int MaxShapeSize { get { return _maxShapeSize; } set { SetProperty(ref _maxShapeSize, value); HasChanged = true; } }
        public int MinShapeSize { get { return _minShapeSize; } set { SetProperty(ref _minShapeSize, value); HasChanged = true; } }
        public int MaxSkewing { get { return _maxSkewing; } set { SetProperty(ref _maxSkewing, value); HasChanged = true; } }
        public int MinSkewing { get { return _minSkewing; } set { SetProperty(ref _minSkewing, value); HasChanged = true; } }
        public int MaxBorderSize { get { return _maxBorderSize; } set { SetProperty(ref _maxBorderSize, value); HasChanged = true; } }
        public int MinBorderSize { get { return _minBorderSize; } set { SetProperty(ref _minBorderSize, value); HasChanged = true; } }
        public int StartCalculationsAtAmountOfParents { get { return _startCalculationsAtAmountOfParents; } set { SetProperty(ref _startCalculationsAtAmountOfParents, value); HasChanged = true; } }
        public int AddedShapesPerParent { get { return _addedShapesPerParent; } set { SetProperty(ref _addedShapesPerParent, value); HasChanged = true; } }
        public int PanelWidth { get { return _panelWidth; } set { SetProperty(ref _panelWidth, value); HasChanged = true; } }
        public int PanelHeight { get { return _panelHeight; } set { SetProperty(ref _panelHeight, value); HasChanged = true; } }
        public string BackgroundColor { get { return _backgroundColor; } set { SetProperty(ref _backgroundColor, value); HasChanged = true; } }
        public string PaintingBackgroundColor { get { return _paintingBackgroundColor; } set { SetProperty(ref _paintingBackgroundColor, value); HasChanged = true; } }
        public string CustomShape { get { return _customShape; } set { SetProperty(ref _customShape, value); HasChanged = true; } }
        public string TitleColor { get { return _titleColor; } set { SetProperty(ref _titleColor, value); HasChanged = true; } }
        public string ChosenCulture { get { return _chosenCulture; } set { SetProperty(ref _chosenCulture, value); HasChanged = true; } }
        public string ChosenBackgroundType { get { return _chosenBackgroundType; } set { SetProperty(ref _chosenBackgroundType, value); HasChanged = true; } }
        public List<string> AvailableLanguages { get { return _availableLanguages; } set { SetProperty(ref _availableLanguages, value); } }
        public List<string> PaintingBackgroundTypes { get { return _paintingBackgroundTypes; } set { SetProperty(ref _paintingBackgroundTypes, value); } }

        public int RowCount = 10;
        public int ColumnCount = 10;
    }
}
