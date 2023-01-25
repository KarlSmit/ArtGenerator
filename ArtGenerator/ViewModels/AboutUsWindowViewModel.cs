using ArtGenerator.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ArtGenerator.ViewModels
{
    class AboutUsWindowViewModel : BindableBase, IDialogAware
    {
        public string Title => "";

        public event Action<IDialogResult> RequestClose;

        //  ICommand EnableShapeCommand executed when clicked on a button
        private readonly DelegateCommand<string> _clickedButtonCommand;
        public ICommand ClickedButtonCommand => _clickedButtonCommand;

        public AboutUsWindowViewModel(IDialogService dialogService, IConfigService configService)
        {
            configService.LoadLanguage(Application.Current);
            _clickedButtonCommand = new DelegateCommand<string>((a) => GoToHyperlinkCommand(a));
        }

        
        private void GoToHyperlinkCommand(string url)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
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
