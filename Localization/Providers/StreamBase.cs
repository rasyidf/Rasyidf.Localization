using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Rasyidf.Localization
{
    public abstract class StreamBase
    {
        public string Path { get; set; }
        public List<LanguageItem> Packs { get; set; }
        public bool IsLoaded { get; set; }

        public void Load()
        {
            Packs = new List<LanguageItem>();
            OnLoad();
            IsLoaded = true;
        }

        public void Unload()
        {
            OnUnload();
            Packs = null;
            IsLoaded = false;
        }
        public static void RegisterPack(LanguageItem item)
        {
            if (LanguageService.RegisteredPacks.ContainsKey(item.Culture))
            {
                return;
            }

            LanguageService.RegisteredPacks.Add(item.Culture, item);
        }
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
        public static void UnregisterPack(CultureInfo cultureInfo)
        {
            if (!LanguageService.RegisteredPacks.ContainsKey(cultureInfo))
            {
                return;
            }
            LanguageService.RegisteredPacks.Remove(cultureInfo);
        }
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

        protected abstract void OnLoad();

        protected abstract void OnUnload();


    }
    public class NullStream : StreamBase
    {
        protected override void OnLoad()
        {

        }

        protected override void OnUnload()
        {

        }
    }
}