using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Rasyidf.Localization
{
    /// <summary>
    ///
    /// </summary>
    public class LocalizationDictionary
    {
        /// <summary>
        ///
        /// </summary>
        public static LocalizationDictionary Default() => new LocalizationDictionary()
        {
            CultureName = CultureInfo.InstalledUICulture.NativeName,
            EnglishName = CultureInfo.InstalledUICulture.EnglishName,
            CultureId = CultureInfo.InstalledUICulture.Name,
            RTL = false,
            Author = "Anonymous"
        };

        /// <summary>
        ///
        /// </summary>
        public CultureInfo Culture => CultureInfo.GetCultureInfo(CultureId);

        #region Public Methods

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="uid"></param>
        /// <param name="vid"></param>
        /// <returns></returns>
        public TValue Translate<TValue>(string uid, string vid)
        {
            return (TValue)Translate(uid, vid, null, typeof(TValue));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="uid"></param>
        /// <param name="vid"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public TValue Translate<TValue>(string uid, string vid, TValue defaultValue)
        {
            try
            {
                return (TValue)Translate(uid, vid, defaultValue, typeof(TValue));
            }
            catch (LocalizationException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="valueId"></param>
        /// <param name="defaultValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Translate(string uid, string valueId, object defaultValue, Type type)
        {
            return OnTranslate(uid, valueId, defaultValue, type);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static LocalizationDictionary GetResources(CultureInfo cultureInfo)
        {
            if (cultureInfo is null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            if (!LocalizationService.RegisteredPacks.ContainsKey(cultureInfo)) return Default();

            LocalizationDictionary dictionary = LocalizationService.RegisteredPacks[cultureInfo];
            return dictionary;
        }

        #endregion Public Methods

        /// <summary>
        ///
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CultureId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string EnglishName { get; set; }

        public bool RTL { get; set; }
        public string Author { get; set; }

        internal Dictionary<string, Dictionary<string, string>> Data =
            new Dictionary<string, Dictionary<string, string>>();

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

                Debug.WriteLine("Vid must not be null or empty");

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

                return TypeDescriptor.GetConverter(type)
                                     .ConvertFromString(textValue);
            }
            catch (Exception ex)
            {
                #region Trace

                Debug.WriteLine($"Failed to translate text {textValue} in dictionary {EnglishName}:\n{ex.Message}");

                #endregion Trace

                throw new LocalizationException(LocalizerError.Unknown);
            }
        }
    }
}