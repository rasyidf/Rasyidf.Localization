using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFA.Localization;

namespace Localization.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel MV = new MainWindowViewModel();
        public int LanguagesCount
        {
            get { return MV.Cultures.Count; }
        }
        public MainWindow()
        {
            var a = LanguageDictionary.RegisteredDictionaries;
            var cultures = a.Keys;
            foreach (var culture in cultures)
            {
                MV.Cultures.Add(LanguageDictionary.GetDictionary(culture));
                
            }
            InitializeComponent();
            InitializeMenus();
            this.DataContext = MV;
        }

        private void InitializeMenus()
        {
            foreach (var item in MV.Cultures)
            {
                var a = new MenuItem { Header = item.EnglishName, Tag = item  };
                a.Click += A_Click;
                mnuLanguage.Items.Add(a); 
            }
        }

        private void A_Click(object sender, RoutedEventArgs e)
        {
            lLanguages.SelectedItem = (sender as MenuItem).Tag as LanguageDictionary;
             LanguageContext.Instance.Culture = ((LanguageDictionary)lLanguages.SelectedItem).Culture;
           
        }

        private void lLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lLanguages.SelectedItem != null)
            { 
            LanguageContext.Instance.Culture =  ((LanguageDictionary) lLanguages.SelectedItem).Culture;
            }
        }
    }
}
