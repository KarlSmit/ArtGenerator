using ArtGenerator.Models;
using ArtGenerator.Views;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtGenerator
{
    public static class DialogServiceExtensions
    {
        public static void OpenConfigWindow(this IDialogService dialogService, Action<IDialogResult> action)
        {
            dialogService.ShowDialog(nameof(ConfigWindow), new DialogParameters(), action);
        }
        public static void OpenAboutUsWindow(this IDialogService dialogService, Action<IDialogResult> action)
        {
            dialogService.ShowDialog(nameof(AboutUsWindow), new DialogParameters(), action);
        }
    }
}
