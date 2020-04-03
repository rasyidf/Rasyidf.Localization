using Demo.WPF.Helpers;

using Rasyidf.Localization;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Demo.WPF.ViewModels
{
    class MainWindowViewModel : Observable
    {
        public MainWindowViewModel()
        {
            var a = LocalizationService.RegisteredPacks;
            var cultures = a.Keys;
            foreach (var culture in cultures)
            {
                var pack = LocalizationDictionary.GetResources(culture);
                Cultures.Add(pack);
                CultureMenus.Add(new MenuItem() { Header = $"{pack.EnglishName} ({pack.CultureName})", Tag = pack });
            }
        }

        private RelayCommand<LocalizationDictionary> _changeLanguageCommand;
        public RelayCommand<LocalizationDictionary> ChangeLanguageCommand => _changeLanguageCommand ?? (_changeLanguageCommand = new RelayCommand<LocalizationDictionary>(ChangeLanguage));

        private RelayCommand _showMessageCommand;
        public RelayCommand ShowMessageCommand => _showMessageCommand ?? (_showMessageCommand = new RelayCommand(ShowMessage));

        private void ShowMessage()
        {
            MessageBox.Show(Application.Current.MainWindow,
                            "511,Text".Localize(),
                            LocalizationService.GetString("511", "Header", "Header"));
        }

        private void ChangeLanguage(LocalizationDictionary value)
        {
            if (value != null)
            {
                LocalizationService.Current.ChangeLanguage(value);
            }
        }

        private ObservableCollection<LocalizationDictionary> _cultures = new ObservableCollection<LocalizationDictionary>();
        private LocalizationDictionary _selectedPack;
        private ObservableCollection<MenuItem> _cultureMenus = new ObservableCollection<MenuItem>();
        public ObservableCollection<LocalizationDictionary> Cultures
        {
            get => _cultures; set
            {
                Set(ref _cultures, value);
            }
        }
        public ObservableCollection<MenuItem> CultureMenus
        {
            get => _cultureMenus; set => Set(ref _cultureMenus, value);
        }
        public int LanguageCount => Cultures.Count;

        public LocalizationDictionary SelectedPack
        {
            get => _selectedPack;
            set
            {
                Set(ref _selectedPack, value);
                LocalizationService.Current.ChangeLanguage(value);
            }
        }

    }
}
