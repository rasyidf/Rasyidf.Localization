using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        public Collection<LanguageItem> Packs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLoaded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            Packs = new Collection<LanguageItem>();
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
        public static void RegisterPack(LanguageItem item)
        {
            if (LanguageService.RegisteredPacks.ContainsKey(item.Culture))
            {
                return;
            }

            LanguageService.RegisteredPacks.Add(item.Culture, item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public static void RegisterPacks(StreamBase dictionary)
        {
            foreach (LanguageItem item in dictionary.Packs)
            {
                if (LanguageService.RegisteredPacks.ContainsKey(item.Culture))
                {
                    continue;
                }

                LanguageService.RegisteredPacks.Add(item.Culture, item);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cultureInfo"></param>
        public static void UnregisterPack(CultureInfo cultureInfo)
        {
            if (!LanguageService.RegisteredPacks.ContainsKey(cultureInfo))
            {
                return;
            }
            LanguageService.RegisteredPacks.Remove(cultureInfo);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cultureInfo"></param>
        public static void UnregisterPacks(CultureInfo[] cultureInfo)
        {
            foreach (CultureInfo item in cultureInfo)
            {
                if (!LanguageService.RegisteredPacks.ContainsKey(item))
                {
                    return;
                }
                LanguageService.RegisteredPacks.Remove(item);
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