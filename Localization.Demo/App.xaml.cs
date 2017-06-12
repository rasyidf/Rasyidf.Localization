using Localization.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UFA.Localization;
using UFA.Localization.XML;

namespace Localization.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LanguageManager.ScanLanguagesInFolder("Languages"); // Scan In Language folder
            // Set Default Language 
            LanguageManager.SetLanguage("en-US");

            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }
    }
}
