using ArtGenerator.Models;
using ArtGenerator.Services;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArtGenerator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        // Settings, maybe settings/configuration class?
        public int PanelWidth { get; set; }
        public int PanelHeight { get; set; }
        public int CanvasPanelHeight { get; set; }
        public int CanvasPanelHeightForZoom { get; set; }
        public int FrameHeight { get; set; }
        public int FrameWidth { get; set; }
        public Color PickedColor { get; set; }

        private string _chosenLanguage;
        public string ChosenLanguage
        {
            get { return _chosenLanguage; }
            set { SetProperty(ref _chosenLanguage, value); }
        }

        private string _iterationNumber;
        public string IterationNumber
        {
            get { return _iterationNumber; }
            set { SetProperty(ref _iterationNumber, value); }
        }

        private Brush _backgroundColor;
        public Brush BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetProperty(ref _backgroundColor, value); }
        }

        private Painting _leftPanel;
        public Painting LeftPanel
        {
            get { return _leftPanel; }
            set { SetProperty(ref _leftPanel, value); }
        }
        private Painting _rightPanel;
        public Painting RightPanel
        {
            get { return _rightPanel; }
            set { SetProperty(ref _rightPanel, value); }
        }

        private int _sideMenuOpened;
        public int SideMenuOpened
        {
            get { return _sideMenuOpened; }
            set { SetProperty(ref _sideMenuOpened, value); }
        }

        private bool _dropDownMenuOpened;
        public bool DropDownMenuOpened
        {
            get { return _dropDownMenuOpened; }
            set { SetProperty(ref _dropDownMenuOpened, value); }
        }

        private string _hamburgerImages;
        public string HamburgerImages
        {
            get { return _hamburgerImages; }
            set { SetProperty(ref _hamburgerImages, value); }
        }

        private bool _isHelpImageVisible;
        public bool IsHelpImageVisible
        {
            get { return _isHelpImageVisible; }
            set { SetProperty(ref _isHelpImageVisible, value); }
        }

        private BitmapImage _imageInDetail;
        public BitmapImage ImageInDetail
        {
            get { return _imageInDetail; }
            set { SetProperty(ref _imageInDetail, value); }
        }

        private bool _canvasDetailClosed;
        public bool CanvasDetailClosed
        {
            get { return _canvasDetailClosed; }
            set { SetProperty(ref _canvasDetailClosed, value); }
        }

        private string _iterationsSurvived;
        public string IterationsSurvived
        {
            get { return _iterationsSurvived; }
            set { SetProperty(ref _iterationsSurvived, value); }
        }

        private string _mutationCount;
        public string MutationCount
        {
            get { return _mutationCount; }
            set { SetProperty(ref _mutationCount, value); }
        }

        private int _frameState;
        public int FrameState
        {
            get { return _frameState; }
            set { SetProperty(ref _frameState, value); }
        }

        private string _windowStateIconPath;
        public string WindowStateIconPath
        {
            get { return _windowStateIconPath; }
            set { SetProperty(ref _windowStateIconPath, value); }
        }

        private string _minimizeImages;
        public string MinimizeImages
        {
            get { return _minimizeImages; }
            set { SetProperty(ref _minimizeImages, value); }
        }

        private string _closeImages;
        public string CloseImages
        {
            get { return _closeImages; }
            set { SetProperty(ref _closeImages, value); }
        }

        private string _titleColor;
        public string TitleColor
        {
            get { return _titleColor; }
            set { SetProperty(ref _titleColor, value); }
        }

        private bool _removeImageOnClose { get; set; }
        private bool _saveImageEveryIteration { get; set; }

        private readonly DelegateCommand<string> _updatePanelCommand;
        private readonly DelegateCommand<Canvas> _openCanvasDetailWindowCommand;
        private readonly DelegateCommand _selectedColorChanged;
        private readonly DelegateCommand _openConfigWindowCommand;
        private readonly DelegateCommand _aboutUsCommand;
        private readonly DelegateCommand _loadCanvasShapes;
        private readonly DelegateCommand _changeStateSideMenu;
        private readonly DelegateCommand _showHelpImages;
        private readonly DelegateCommand _closeDetailWindow;
        private readonly DelegateCommand _saveImageCommand;
        private readonly DelegateCommand _switchFrame;
        private readonly DelegateCommand _resetAlgorithmCommand;
        private readonly DelegateCommand _saveAlgorithmDataCommand;
        private readonly DelegateCommand _loadAlgorithmDataCommand;
        private readonly DelegateCommand _switchDropDownMenu;
        private readonly DelegateCommand _checkWindowStateIcon;

        public ICommand UpdatePanelCommand => _updatePanelCommand;
        public ICommand OpenConfigWindowCommand => _openConfigWindowCommand;
        public ICommand OpenAboutUsCommand => _aboutUsCommand;
        public ICommand OpenCanvasDetailWindowCommand => _openCanvasDetailWindowCommand;
        public ICommand BackGroundColorCommand => _selectedColorChanged;
        public ICommand LoadCanvasShapes => _loadCanvasShapes;
        public ICommand ChangeStateSideMenu => _changeStateSideMenu;
        public ICommand ShowHelpImages => _showHelpImages;
        public ICommand CloseDetailWindow => _closeDetailWindow;
        public ICommand SaveImageCommand => _saveImageCommand;
        public ICommand SwitchFrame => _switchFrame;
        public ICommand ResetAlgorithmCommand => _resetAlgorithmCommand;
        public ICommand SaveAlgorithmDataCommand => _saveAlgorithmDataCommand;
        public ICommand LoadAlgorithmDataCommand => _loadAlgorithmDataCommand;
        public ICommand SwitchDropDownMenu => _switchDropDownMenu;
        public ICommand CheckWindowState => _checkWindowStateIcon;

        private IConfigService _configService;
        private IDialogService _dialogService;
        private IArtService _artService;

        public MainWindowViewModel(IConfigService configService, IArtService artService, IDialogService dialogService)
        {
            _configService = configService;
            _dialogService = dialogService;
            _artService = artService;

            SideMenuOpened = 0;
            IsHelpImageVisible = false;
            CanvasDetailClosed = true;
            DropDownMenuOpened = false;
            _removeImageOnClose = true;
            _saveImageEveryIteration = _configService.GetConfig().SaveAllPictures;
            FrameState = 1;

            // Settings
            PanelWidth = _configService.GetConfig().PanelWidth;
            PanelHeight = _configService.GetConfig().PanelHeight;
            // increase canvas heigt by 50 to accomodate the button
            CanvasPanelHeight = PanelHeight + 60;
            CanvasPanelHeightForZoom = (PanelHeight + 64) * -1;

            _artService.SetPanelWidthAndHeight(PanelWidth, PanelHeight);
            FrameHeight = (PanelHeight * 2 + 64) * -1;

            // Initialize default colors
            PickedColor = (Color)ColorConverter.ConvertFromString(_configService.GetConfig().BackgroundColor);
            _configService.LoadLanguage(Application.Current);

            _updatePanelCommand = new DelegateCommand<string>((a) => GenerateArt(a));
            _selectedColorChanged = new DelegateCommand(() => BackGroundColorSelected());
            _openConfigWindowCommand = new DelegateCommand(() => OpenConfigWindow());
            _aboutUsCommand = new DelegateCommand(() => OpenAboutUsWindow());
            _openCanvasDetailWindowCommand = new DelegateCommand<Canvas>((a) => OpenCanvasDetailWindow(a));
            _loadCanvasShapes = new DelegateCommand(() => GenerateStartingPanels());
            _changeStateSideMenu = new DelegateCommand(() => { SideMenuOpened = (SideMenuOpened == 0) ? 1 : 0; DropDownMenuOpened = false; });
            _showHelpImages = new DelegateCommand(() => IsHelpImageVisible = IsHelpImageVisible == false);
            _closeDetailWindow = new DelegateCommand(() => OpenCanvasDetailWindow(new Canvas()));
            _saveImageCommand = new DelegateCommand(() => _removeImageOnClose = false);
            _switchFrame = new DelegateCommand(() => FrameState = (FrameState == 0) ? 1 : 0);
            _resetAlgorithmCommand = new DelegateCommand(() => ResetAlgorithm());
            _loadAlgorithmDataCommand = new DelegateCommand(() => LoadInCustomGenerationData());
            _saveAlgorithmDataCommand = new DelegateCommand(() => SaveGenerationData());
            _switchDropDownMenu = new DelegateCommand(() => DropDownMenuOpened = DropDownMenuOpened == false);
            _checkWindowStateIcon = new DelegateCommand(() => CheckWindowStateIcon());
        }

        private void ResetAlgorithm()
        {
            _artService.ResetAlgorithm();
            GenerateStartingPanels();
        }

        private void GenerateStartingPanels()
        {
            RightPanel = new();
            LeftPanel = new();
            GenerateArt(null);
            BackGroundColorSelected();
        }

        private void LoadInCustomGenerationData()
        {
            OpenFileDialog dlg = new();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "*.json";
            dlg.Filter = "JSON|*.json";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArtGenerator\\";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                _artService.LoadGenerationData(File.ReadAllText(dlg.FileName), out Painting leftPanel, out Painting rightPanel);
                LeftPanel = leftPanel;
                RightPanel = rightPanel;
            }
        }

        public void SaveGenerationData()
        {
            _artService.SaveGenerationData(LeftPanel, RightPanel);
        }

        /// <summary>
        /// This function will save Canvasses sent to it, it will later keep all images if _saveImageEveryIteration is Enabled
        /// </summary>
        /// <param name="canvasObject">The Canvas in queston</param>
        /// <returns>A BitmapImage with the Image so it can be safely removed later on</returns>
        private BitmapImage SaveCanvas(Canvas canvasObject)
        {
            string canvasName = canvasObject.Name + "-" + DateTime.Now.ToString("yyyy-MM-dd--THH-mm-ss");
            RenderTargetBitmap renderTargetBitmap = new(Convert.ToInt32(canvasObject.Width), Convert.ToInt32(canvasObject.Height), 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(canvasObject);
            PngBitmapEncoder pngImage = new();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            string fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArtGenerator\\GeneratedArt\\" + canvasName + ".png";
            using (Stream fileStream = File.Create(fileLocation))
            {
                pngImage.Save(fileStream);
            }

            BitmapImage bi = new();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.UriSource = new Uri(fileLocation);
            bi.EndInit();
            return bi;
        }

        private void OpenCanvasDetailWindow(Canvas canvasObject)
        {
            if(CanvasDetailClosed)
            {
                string iterations = (canvasObject.Name == "leftPanel") ? LeftPanel.GetIterationsSurvived() : RightPanel.GetIterationsSurvived();
                string mutations = (canvasObject.Name == "leftPanel") ? LeftPanel.MutationCount.ToString() : RightPanel.MutationCount.ToString();

                ImageInDetail = SaveCanvas(canvasObject);
                CanvasDetailClosed = false;
                ResourceDictionary dictionary = Application.Current.Resources.MergedDictionaries.Where(l => l.Source != null && l.Source.ToString().StartsWith("Languages")).First();
                IterationNumber = dictionary["DetailWindow.Iteration"].ToString().Replace("%number%", _artService.GetIterationNumber().ToString());
                IterationsSurvived = dictionary["DetailWindow.PaintingLikes"].ToString().Replace("%number%", iterations);
                MutationCount = dictionary["DetailWindow.PaintingMutation"].ToString().Replace("%number%", mutations);
            }
            else
            {
                if(_removeImageOnClose)
                {
                    File.Delete(ImageInDetail.UriSource.OriginalString);
                }
                _removeImageOnClose = true;
                CanvasDetailClosed = true;
            }
        }

        private void OpenConfigWindow()
        {
            DialogServiceExtensions.OpenConfigWindow(_dialogService, null);
        }

        private void OpenAboutUsWindow()
        {
            DialogServiceExtensions.OpenAboutUsWindow(_dialogService, null);
        }

        /// <summary>
        /// Gets the preferred panel and sends it to the artservice for creating Art for the specified panel
        /// </summary>
        /// <param name="preferredPanelName">Name of the panel that was picked</param>
        private void GenerateArt(string preferredPanelName)
        {
            if (_artService.CanUseAlgorithm())
            {
                if (preferredPanelName == "RightPanel")
                {
                    LeftPanel = _artService.GenerateArt(LeftPanel, RightPanel);
                }
                else
                {
                    RightPanel = _artService.GenerateArt(RightPanel, LeftPanel);
                }
            }
            // We are using "Background" here to see if there is already one liked. If background != null then we know there is an existing painting.
            else
            {
                if (preferredPanelName == "RightPanel")
                {
                    RightPanel.UpdateIterationsSurvived(_artService.GetIterationNumber());
                }
                else if (preferredPanelName == "LeftPanel")
                {
                    LeftPanel.UpdateIterationsSurvived(_artService.GetIterationNumber());
                }
                LeftPanel = _artService.GenerateArt(LeftPanel, RightPanel);
                RightPanel = _artService.GenerateArt(RightPanel, LeftPanel);
            }
        }

        // <summary>
        // Change background color to selected and calculate titlecolor
        // </summary>
        private void BackGroundColorSelected()
        {
            // Set Background color to picked color
            BackgroundColor = new SolidColorBrush(PickedColor);

            // Create Picked color delta
            int bgDelta = Convert.ToInt32((PickedColor.R * 0.299) + (PickedColor.G * 0.587) + (PickedColor.B * 0.114));
            // Compare Delta to threshold and set title color
            _configService.GetConfig().BackgroundColor = PickedColor.ToString();
            _configService.GetConfig().TitleColor = (255 - bgDelta < 105) ? "#ff000000" : "#ffffffff";
            _configService.SaveConfig();
            TitleColor = _configService.GetConfig().TitleColor;
            HamburgerImages = (255 - bgDelta < 105) ? "/Images/Hamburger.png" : "/Images/Hamburger-Wit.png";
            MinimizeImages = (255 - bgDelta < 105) ? "/Images/Navigation/minimize.png" : "/Images/Navigation/minimize-white.png";
            CloseImages = (255 - bgDelta < 105) ? "/Images/Navigation/close.png" : "/Images/Navigation/close-white.png";
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                WindowStateIconPath = (255 - bgDelta < 105) ? "/Images/Navigation/restore.png" : "/Images/Navigation/restore-white.png";
            }
            else
            {
                WindowStateIconPath = (255 - bgDelta < 105) ? "/Images/Navigation/maximize.png" : "/Images/Navigation/maximize-white.png";
            }

        }

        private void CheckWindowStateIcon()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                if(_configService.GetConfig().TitleColor == "#ff000000")
                {
                    WindowStateIconPath = "/Images/Navigation/restore.png";
                }
                else
                {
                    WindowStateIconPath = "/Images/Navigation/restore-white.png";
                }
            }
            else
            {
                if (_configService.GetConfig().TitleColor == "#ff000000")
                {
                    WindowStateIconPath = "/Images/Navigation/maximize.png";
                }
                else
                {
                    WindowStateIconPath = "/Images/Navigation/maximize-white.png";
                }
            }
        }
    }
}