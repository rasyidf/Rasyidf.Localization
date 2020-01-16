using Rasyidf.Localization;

using System.Windows;

namespace Localization.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LocalizationService.Current.Initialize();  

        }

        void Application_Startup(object sender, StartupEventArgs e)
        {
        }
    }
}