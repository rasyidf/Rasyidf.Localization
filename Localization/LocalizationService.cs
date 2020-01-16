namespace Rasyidf.Localization
{
    /// <summary>
    /// Language Service Across Sessions
    /// </summary>
    public class LocalizationService : INotifyPropertyChanged
    {
        #region Fields

        private CultureInfo _cultureInfo;
        private LocalizationDictionary _pack;

        #endregion Fields

        #region Properties

        /// <summary>
        ///
        /// </summary>
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

                _cultureInfo = value;

                Thread.CurrentThread.CurrentUICulture = _cultureInfo;

                var newDictionary = LocalizationDictionary.GetResources(_cultureInfo);

                LanguagePack = newDictionary;
                OnPropertyChanged(nameof(Culture));
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static Dictionary<CultureInfo, LocalizationDictionary> RegisteredPacks { get; } = new Dictionary<CultureInfo, LocalizationDictionary>();

        /// <summary>
        ///
        /// </summary>
        public LocalizationDictionary LanguagePack
        {
            get => _pack;
            set
            {
                if (value == null || value == _pack) return;

                _pack = value;
                OnPropertyChanged(nameof(LanguagePack));
            }
        }

        #endregion Properties

        /// <summary>
        ///
        /// </summary>
        public static LocalizationService Current { get; } = new LocalizationService();

        /// <summary>
        /// Initialize Language Service
        /// </summary>
        /// <param name="path"></param>
        /// <param name="default"></param>
        public void Initialize(string path = "Languages", string @default = "en-us")
        {
            if (path != null)
            {
                ScanLanguagesInFolder(path);
            }
            Current.Culture = CultureInfo.GetCultureInfo(@default);
            OnPropertyChanged(nameof(LanguagePack));
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
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
                var filepath = path + @"\" + t.Name;
                StreamBase LanguagePackStream = t.Extension switch
                {
                    ".xml" => new XmlStream(filepath),
                    ".json" => new JsonStream(filepath),
                    _ => new NullStream(),
                };

                LanguagePackStream.Load();
                StreamBase.RegisterPacks(LanguagePackStream);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo directory, params string[] extensions)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
            return directory.EnumerateFiles().Where(f => allowedExtensions.Contains(f.Extension));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public void ChangeLanguage(LocalizationDictionary value)
        {
            _cultureInfo = value?.Culture ?? throw new ArgumentNullException(nameof(value));
            Thread.CurrentThread.CurrentUICulture = _cultureInfo;
            LanguagePack = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged(nameof(Culture));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="valueId"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static string GetString(string uid, string valueId, string @default = "")
        {
            return Current.LanguagePack.Translate(uid, valueId, @default);
        }
    }

    /// <summary>
    /// String Extension for Translation
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Localize string by uid and vid
        /// separated by <paramref name="separator"/> with default of comma (',')
        /// usage "10,Header".Localize();
        /// </summary>
        /// <param name="self"></param>
        /// <param name="default"></param>
        /// <param name="separator"></param>
        /// <returns>Return Localized string</returns>
        public static string Localize(this string self, string @default = "", char separator = ',')
        {
            if (string.IsNullOrEmpty(self))
            {
                return "";
            }

            var val = self.Split(separator);
            return LocalizationService.GetString(val[0], val[1], @default);
        }
    }
}