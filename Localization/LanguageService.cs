using Rasyidf.Localization.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Rasyidf.Localization
{
    public class LanguageService : INotifyPropertyChanged
    {
        
        #region Fields

        CultureInfo _cultureInfo;
        LanguageItem _pack;

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

               
                _cultureInfo = value;

                Thread.CurrentThread.CurrentUICulture = _cultureInfo;

                var newDictionary = LanguageItem.GetResources(_cultureInfo);

                LanguagePack = newDictionary;
                OnPropertyChanged(nameof(Culture));
            }
        }

        public static Dictionary<CultureInfo, LanguageItem> RegisteredPacks { get; } = new Dictionary<CultureInfo, LanguageItem>();
        public LanguageItem LanguagePack
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


        public static LanguageService Current { get; } = new LanguageService();

        /// <summary>
        /// Initialize Language Service
        /// </summary>
        /// <param name="path"></param>
        /// <param name="default"></param>
        public void Initialize(string path = "Languages", string @default="en-us")
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

                var filepath = path + @"\" + t.Name;

                StreamBase LanguagePackStream;

                switch (t.Extension)
                {
                    case ".xml":
                        LanguagePackStream = new XmlStream(filepath);
                        break;

                    case ".json":
                        LanguagePackStream = new JsonStream(filepath);
                        break;
                    default:
                        LanguagePackStream = new NullStream();
                        break;
                }

                LanguagePackStream.Load(); 
                StreamBase.RegisterPacks(LanguagePackStream); 
            }
             
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo directory, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
            return directory.EnumerateFiles().Where(f => allowedExtensions.Contains(f.Extension));
        }

        public void ChangeLanguage(LanguageItem value)
        {
            _cultureInfo = value.Culture;
            Thread.CurrentThread.CurrentUICulture = _cultureInfo;
            LanguagePack = value;
            OnPropertyChanged(nameof(Culture));
        }
         
        public static string GetString(string uid, string valueid, string @default = "")
        {
            return Current.LanguagePack.Translate(uid, valueid, @default);
        }


    }

    public static class StringExtension
    {
        public static string Translate(this string self, string @default = "", char separator =',')
        {
            var val = self.Split(separator);
            return LanguageService.GetString(val[0], val[1], @default);
        }
    }

}