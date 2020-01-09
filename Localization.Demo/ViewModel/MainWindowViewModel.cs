using Rasyidf.Localization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Localization.Demo
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {

        public MainWindowViewModel()
        {
            var a = LanguageService.RegisteredPacks;
            var cultures = a.Keys;
            foreach (var culture in cultures)
            {
                var pack = LanguageItem.GetResources(culture);
                Cultures.Add(pack);
                CultureMenus.Add(new MenuItem() { Header= $"{pack.EnglishName} ({pack.CultureName})", Tag = pack });
            }
        }

        private RelayCommand<LanguageItem> _changeLanguageCommand;
        public RelayCommand<LanguageItem> ChangeLanguageCommand => _changeLanguageCommand ?? (_changeLanguageCommand = new RelayCommand<LanguageItem>(ChangeLanguage));
        
        private RelayCommand _showMessageCommand;
        public RelayCommand ShowMessageCommand => _showMessageCommand ?? (_showMessageCommand = new RelayCommand(ShowMessage));

        private void ShowMessage(object obj)
        {
            MessageBox.Show(Application.Current.MainWindow,"511,Text".Localize() ,LanguageService.GetString("511", "Header","Header"));
        }

        private void ChangeLanguage(LanguageItem value)
        {
            if (value != null)
            {
                LanguageService.Current.ChangeLanguage(value);
                OnPropertyChanged(nameof(SelectedPack));
            }
        }

        private ObservableCollection<LanguageItem> _cultures = new ObservableCollection<LanguageItem>();
        private LanguageItem _selectedPack;
        private ObservableCollection<MenuItem> _cultureMenus = new ObservableCollection<MenuItem>();

        public ObservableCollection<LanguageItem> Cultures
        {
            get => _cultures; set
            {
                _cultures = value;
                OnPropertyChanged(nameof(Cultures));
            }
        }
          public ObservableCollection<MenuItem> CultureMenus
        {
            get => _cultureMenus; set
            {
                _cultureMenus = value;
                OnPropertyChanged(nameof(CultureMenus));
            }
        }
        public int LanguageCount => Cultures.Count;

        public LanguageItem SelectedPack
        {
            get => _selectedPack;
            set
            {
                _selectedPack = value;
                LanguageService.Current.ChangeLanguage(value);
                OnPropertyChanged(nameof(SelectedPack));
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}