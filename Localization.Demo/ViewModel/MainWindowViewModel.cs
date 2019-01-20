using Rasyidf.Localization;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
                Cultures.Add(BaseLanguagePack.GetResources(culture));
            }
        }

        private RelayCommand<BaseLanguagePack> _changeLanguageCommand;
        public RelayCommand<BaseLanguagePack> ChangeLanguageCommand => _changeLanguageCommand
                       ?? (_changeLanguageCommand = new RelayCommand<BaseLanguagePack>(ChangeLanguage));

        private void ChangeLanguage(BaseLanguagePack value)
        {
            LocalizationService.Current.ChangeLanguage(value);

            OnPropertyChanged(nameof(SelectedPack));
        }

        private ObservableCollection<BaseLanguagePack> _cultures = new ObservableCollection<BaseLanguagePack>();
        private BaseLanguagePack _selectedPack;

        public ObservableCollection<BaseLanguagePack> Cultures
        {
            get => _cultures; set
            {
                _cultures = value;
                OnPropertyChanged(nameof(Cultures));
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