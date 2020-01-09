using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Rasyidf.Localization
{
    /// <summary>
    /// Language Converter class for binding.
    /// </summary>
    public class LanguageConverter : IValueConverter, IMultiValueConverter
    {
        #region Fields

        string _uid;
        readonly string _vid;
        readonly object _defaultValue;
        bool _isStaticUid;

        #endregion Fields

        #region Initialization

        public LanguageConverter(string uid, string vid, object defaultValue)
        {
            _uid = uid;
            _vid = vid;
            _defaultValue = defaultValue;
            _isStaticUid = true;
        }

        #endregion Initialization

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dictionary = ResolveDictionary();
            var translation = dictionary.Translate(_uid, _vid, _defaultValue, targetType);
            return translation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion IValueConverter Members

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var parametersCount = _isStaticUid ? values.Length - 1 : values.Length - 2;
                if (string.IsNullOrEmpty(_uid))
                {
                    if (values[1] == null)
                    {
                        throw new ArgumentNullException(nameof(values));
                    }
                    _isStaticUid = false;
                    _uid = values[1].ToString();
                    --parametersCount;
                }
                LanguageItem dictionary = ResolveDictionary();
                var translatedObject = dictionary.Translate(_uid, _vid, _defaultValue, targetType);

                if (translatedObject == null || parametersCount == 0) return translatedObject;

                var parameters = new object[parametersCount];
                Array.Copy(values, values.Length - parametersCount, parameters, 0, parameters.Length);

                try
                {
                    translatedObject = string.Format(translatedObject.ToString(), parameters);
                }
                catch (Exception)
                {
                    #region Trace

                    Debug.WriteLine($"LanguageConverter failed to format text {translatedObject.ToString()}");

                    #endregion Trace
                }
                return translatedObject;
            }
            catch (Exception ex)
            {
                #region Trace

                Debug.WriteLine($"LanguageConverter failed to convert text: {ex.Message}");

                #endregion Trace
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }

        #endregion IMultiValueConverter Members

        #region Privates

        static LanguageItem ResolveDictionary()
        {
            var dictionary = LanguageItem.GetResources(LanguageService.Current.Culture);

            if (dictionary is null)
            {
                throw new InvalidOperationException(
                    $"Pack for language {LanguageService.Current.Culture} was not found");
            }

            return dictionary;
        }

        #endregion Privates
    }
}