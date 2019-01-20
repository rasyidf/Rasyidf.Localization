using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Rasyidf.Localization
{
    public abstract class BaseLanguagePack
    {
        #region Fields

        public static readonly BaseLanguagePack Null = new NullStream();

        #endregion Fields

        #region Properties

        public CultureInfo Culture => CultureInfo.GetCultureInfo(CultureId);


        #endregion Properties

        #region Public Methods

        public void Load()
        {
            OnLoad();
            IsLoaded = true;
        }

        public void Unload()
        {
            OnUnload();
            IsLoaded = false;
        }

        public TValue Translate<TValue>(string uid, string vid)
        {
            return (TValue)Translate(uid, vid, null, typeof(TValue));
        }

        public object Translate(string uid, string vid, object defaultValue, Type type)
        {
            return OnTranslate(uid, vid, defaultValue, type);
        }

        public static void RegisterDictionary(CultureInfo cultureInfo, BaseLanguagePack dictionary)
        {
            if (!LocalizationService.RegisteredPacks.ContainsKey(cultureInfo))
            {
                LocalizationService.RegisteredPacks.Add(cultureInfo, dictionary);
            }
        }

        public static void UnregisterDictionary(CultureInfo cultureInfo)
        {
            if (LocalizationService.RegisteredPacks.ContainsKey(cultureInfo))
            {
                LocalizationService.RegisteredPacks.Remove(cultureInfo);
            }
        }

        public static BaseLanguagePack GetResources(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            if (!LocalizationService.RegisteredPacks.ContainsKey(cultureInfo)) return Null;

            BaseLanguagePack dictionary = LocalizationService.RegisteredPacks[cultureInfo];
            return dictionary;
        }

        #endregion Public Methods

        #region Overrideables

        public string CultureId { get; set; }
        public string CultureName { get; set; }
        public string EnglishName { get; set; }

        public bool IsLoaded { get; set; }

        internal Dictionary<string, Dictionary<string, string>> Data =
            new Dictionary<string, Dictionary<string, string>>();

        protected abstract void OnLoad();

        protected abstract void OnUnload();

        private object OnTranslate(string uid, string vid, object defaultValue, Type type)
        {


            if (string.IsNullOrEmpty(uid))
            {
                #region Trace

                Debug.Print("Uid must not be null or empty");

                #endregion Trace

                return defaultValue;
            }
            if (string.IsNullOrEmpty(vid))
            {
                #region Trace

                Debug.WriteLine(string.Format("Vid must not be null or empty"));

                #endregion Trace

                return defaultValue;
            }
            if (!Data.ContainsKey(uid))
            {
                #region Trace

                Debug.WriteLine($"Uid {uid} was not found in the {EnglishName} dictionary");

                #endregion Trace

                return defaultValue;
            }
            Dictionary<string, string> innerData = Data[uid];

            if (!innerData.ContainsKey(vid))
            {
                #region Trace

                Debug.WriteLine($"Vid {vid} was not found for Uid {uid}, in the {EnglishName} dictionary");

                #endregion Trace

                return defaultValue;
            }
            string textValue = innerData[vid];
            try
            {
                if (type == typeof(object))
                    return textValue;


                TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                object translation = typeConverter.ConvertFromString(textValue);
                return translation;

            }
            catch (Exception ex)
            {
                #region Trace

                Debug.WriteLine($"Failed to translate text {textValue} in dictionary {EnglishName}:\n{ex.Message}");

                #endregion Trace

                return null;
            }
        }
        #endregion Overrideables

        #region Null Pack

        public sealed class NullStream : BaseLanguagePack
        {
            protected override void OnLoad()
            {
                CultureName = CultureInfo.InstalledUICulture.NativeName;
                EnglishName = CultureInfo.InstalledUICulture.EnglishName;
                CultureId = CultureInfo.InstalledUICulture.Name;
            }

            protected override void OnUnload()
            {
            }

        }

        #endregion Null Pack
    }
}