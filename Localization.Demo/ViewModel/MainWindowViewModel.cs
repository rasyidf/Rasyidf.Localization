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
                var pack = LanguagePackBase.GetResources(culture);
                Cultures.Add(pack);
                CultureMenus.Add(new MenuItem() { Header= $"{pack.EnglishName} ({pack.CultureName})", Tag = pack });
            }
        }

        private RelayCommand<LanguagePackBase> _changeLanguageCommand;
        public RelayCommand<LanguagePackBase> ChangeLanguageCommand => _changeLanguageCommand ?? (_changeLanguageCommand = new RelayCommand<LanguagePackBase>(ChangeLanguage));
        private RelayCommand _showMessageCommand;
        public RelayCommand ShowMessageCommand => _showMessageCommand ?? (_showMessageCommand = new RelayCommand(ShowMessage));

        private void ShowMessage(object obj)
        {
            MessageBox.Show(LocalizationService.GetString("511", "Text", "Message"),LocalizationService.GetString("511", "Header","Header"));
        }

        private void ChangeLanguage(LanguagePackBase value)
        {
            if (value != null)
            {
                LocalizationService.Current.ChangeLanguage(value);
                OnPropertyChanged(nameof(SelectedPack));
            }
        }

        private ObservableCollection<LanguagePackBase> _cultures = new ObservableCollection<LanguagePackBase>();
        private LanguagePackBase _selectedPack;
        private ObservableCollection<MenuItem> _cultureMenus = new ObservableCollection<MenuItem>();

        public ObservableCollection<LanguagePackBase> Cultures
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

        public LanguagePackBase SelectedPack
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