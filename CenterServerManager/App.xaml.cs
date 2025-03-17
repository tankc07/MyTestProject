using System.ComponentModel;
using System.Windows;
using CenterServerManager.ViewModels;
using CenterServerManager.Views;
using Prism;
using Prism.Ioc;

namespace CenterServerManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
