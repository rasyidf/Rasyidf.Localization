using Localization.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UFA.Localization.XML;

namespace UFA.Localization
{
    public static class LanguageManager
    {
        public static void SetLanguage(string LanguageCurrent)
        {
            LanguageContext.Instance.Culture = CultureInfo.GetCultureInfo(LanguageCurrent);
        }

        public static void ScanLanguagesInFolder(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFilesByExtensions(".xml", ".json").ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                var g = Path.GetFileNameWithoutExtension(files[i].Name);
                var filepath = path + @"\" + files[i].Name;
                LanguageDictionary LD = LanguageDictionary.Null;
                switch (files[i].Extension)
                {
                    case ".xml":
                        LD = new XmlLanguageDictionary(filepath);
                        break;
                    case ".json":
                        LD = new JsonLanguageDictionary(filepath);
                        break;
                }
                LanguageDictionary.RegisterDictionary(CultureInfo.GetCultureInfo(g), LD);
                // PreLoad Language to Memory.
                LanguageContext.Instance.Culture = CultureInfo.GetCultureInfo(g);
            }
        }

    }
    public static class FileHelper
    {

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            return dir.EnumerateFiles()
                          .Where(f => allowedExtensions.Contains(f.Extension));
        }

    }
}
