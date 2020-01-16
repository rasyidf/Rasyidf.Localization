using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Rasyidf.Localization
{
    /// <summary>
    /// Base class for Language Pack Stream
    /// </summary>
    public abstract class StreamBase
    {
        /// <summary>
        /// the Language Pack Location
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Language Packages
        /// </summary>
        public Collection<LocalizationDictionary> Packs { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsLoaded { get; set; }

        /// <summary>
        ///
        /// </summary>
        public void Load()
        {
            Packs = new Collection<LocalizationDictionary>();
            OnLoad();
            IsLoaded = true;
        }

        /// <summary>
        ///
        /// </summary>
        public void Unload()
        {
            OnUnload();
            Packs = null;
            IsLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public static void RegisterPack(LocalizationDictionary item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (LocalizationService.RegisteredPacks.ContainsKey(item.Culture))
            {
                return;
            }

            LocalizationService.RegisteredPacks.Add(item.Culture, item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dictionary"></param>
        public static void RegisterPacks(StreamBase dictionary)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            for (int i = 0; i < dictionary.Packs.Count; i++)
            {
                LocalizationDictionary item = dictionary.Packs[i];
                if (LocalizationService.RegisteredPacks.ContainsKey(item.Culture))
                {
                    continue;
                }

                LocalizationService.RegisteredPacks.Add(item.Culture, item);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cultureInfo"></param>
        public static void UnregisterPack(CultureInfo cultureInfo)
        {
            if (!LocalizationService.RegisteredPacks.ContainsKey(cultureInfo))
            {
                return;
            }
            LocalizationService.RegisteredPacks.Remove(cultureInfo);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cultureInfo"></param>
        public static void UnregisterPacks(CultureInfo[] cultureInfo)
        {
            if (cultureInfo is null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            foreach (CultureInfo item in cultureInfo)
            {
                if (!LocalizationService.RegisteredPacks.ContainsKey(item))
                {
                    return;
                }
                LocalizationService.RegisteredPacks.Remove(item);
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected abstract void OnLoad();

        /// <summary>
        ///
        /// </summary>
        protected abstract void OnUnload();
    }

    /// <summary>
    /// Null Stream
    /// </summary>
    public class NullStream : StreamBase
    {
        /// <summary>
        /// Empty
        /// </summary>
        protected override void OnLoad()
        {
        }

        /// <summary>
        /// Empty
        /// </summary>
        protected override void OnUnload()
        {
        }
    }
}