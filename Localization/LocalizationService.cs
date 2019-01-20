using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Rasyidf.Localization.Providers;

namespace Rasyidf.Localization
{
    public class LocalizationService : INotifyPropertyChanged
    {
        #region Fields

        CultureInfo _cultureInfo;
        BaseLanguagePack _pack;

        #endregion Fields

        #region Properties

        public CultureInfo Culture
        {
            get => _cultureInfo ?? (_cultureInfo = CultureInfo.CurrentUICulture);
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (Equals(value, _cultureInfo))
                {
                    return;
                }

                if (_cultureInfo != null)
                {
                    var currentDictionary = BaseLanguagePack.GetResources(_cultureInfo);
                    currentDictionary.Unload();
                }
                _cultureInfo = value;

                Thread.CurrentThread.CurrentUICulture = _cultureInfo;

                var newDictionary = BaseLanguagePack.GetResources(_cultureInfo);
                newDictionary.Load();

                Pack = newDictionary;
                OnPropertyChanged(nameof(Culture));
            }
        }

        public static Dictionary<CultureInfo, BaseLanguagePack> RegisteredPacks { get; } = new Dictionary<CultureInfo, BaseLanguagePack>();

        public BaseLanguagePack Pack
        {
            get => _pack;
            set
            {
                if (value == null || value == _pack) return;

                _pack = value;
                OnPropertyChanged(nameof(Pack));
            }
        }

        #endregion Properties


        public static LocalizationService Current { get; } = new LocalizationService();

        public void Register(string path, string language)
        {
            if (path != null)
            {
                ScanLanguagesInFolder(path);
            }
            Current.Culture = CultureInfo.GetCultureInfo(language); 
            OnPropertyChanged(nameof(BaseLanguage));
        }

        public BaseLanguagePack BaseLanguage { get; set; }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static void ScanLanguagesInFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.Print("Path doesn't exist");
                return;
            }

            var di = new DirectoryInfo(path);

            var files = GetFilesByExtensions(di, ".xml", ".json").ToArray();

            foreach (var t in files)
            {
                var g = Path.GetFileNameWithoutExtension(t.Name);

                var filepath = path + @"\" + t.Name;

                BaseLanguagePack baseLanguagePack;

                switch (t.Extension)
                {
                    case ".xml":
                        baseLanguagePack = new XmlStream(filepath);
                        break;

                    case ".json":
                        baseLanguagePack = new JsonStream(filepath);
                        break;
                    default:
                        baseLanguagePack = BaseLanguagePack.Null;
                        break;
                }
                BaseLanguagePack.RegisterDictionary(CultureInfo.GetCultureInfo(g), baseLanguagePack);
                // PreLoad BaseLanguage to Memory.
                Current.Culture = CultureInfo.GetCultureInfo(g);
            }
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo directory, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase); 
            return directory.EnumerateFiles().Where(f => allowedExtensions.Contains(f.Extension));
        }

        public void ChangeLanguage(BaseLanguagePack value)
        {
            _cultureInfo = value.Culture;
            Thread.CurrentThread.CurrentUICulture = _cultureInfo;
            if (!value.IsLoaded)
            {
                value.Load(); 
            } 
            Pack = value;
            OnPropertyChanged(nameof(Culture));
        }
    }

     
}