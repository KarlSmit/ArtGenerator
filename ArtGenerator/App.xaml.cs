using ArtGenerator.Services;
using ArtGenerator.Views;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ArtGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var startWindow = Container.Resolve<MainWindow>();
            return startWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IConfigService, ConfigService>();
            containerRegistry.RegisterSingleton<IArtService, ArtService>();
            containerRegistry.RegisterDialog<ConfigWindow>();
            containerRegistry.RegisterDialog<AboutUsWindow>();
        }
    }
}
