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
            var a = LocalizationService.RegisteredPacks;
            var cultures = a.Keys;
            foreach (var culture in cultures)
            {
                var pack = BaseLanguagePack.GetResources(culture);
                Cultures.Add(pack);
                CultureMenus.Add(new MenuItem() { Header= $"{pack.EnglishName} ({pack.CultureName})", Tag = pack });
            }
        }

        private RelayCommand<BaseLanguagePack> _changeLanguageCommand;
        public RelayCommand<BaseLanguagePack> ChangeLanguageCommand => _changeLanguageCommand ?? (_changeLanguageCommand = new RelayCommand<BaseLanguagePack>(ChangeLanguage));
        private RelayCommand _showMessageCommand;
        public RelayCommand ShowMessageCommand => _showMessageCommand ?? (_showMessageCommand = new RelayCommand(ShowMessage));

        private void ShowMessage(object obj)
        {
            MessageBox.Show(LocalizationService.GetString("511", "Text", "Message"),LocalizationService.GetString("511", "Header","Header"));
        }

        private void ChangeLanguage(BaseLanguagePack value)
        {
            if (value != null)
            {
                LocalizationService.Current.ChangeLanguage(value);
                OnPropertyChanged(nameof(SelectedPack));
            }
        }

        private ObservableCollection<BaseLanguagePack> _cultures = new ObservableCollection<BaseLanguagePack>();
        private BaseLanguagePack _selectedPack;
        private ObservableCollection<MenuItem> _cultureMenus = new ObservableCollection<MenuItem>();

        public ObservableCollection<BaseLanguagePack> Cultures
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

        public BaseLanguagePack SelectedPack
        {
            get => _selectedPack;
            set
            {
                _selectedPack = value;
                LocalizationService.Current.ChangeLanguage(value);
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