using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Rasyidf.Localization
{
    /// <summary>
    /// Language Converter class for binding.
    /// </summary>
    public class LocalizationConverter : IValueConverter, IMultiValueConverter
    {
        #region Fields

        private string _uid;
        private readonly string _vid;
        private readonly object _defaultValue;
        private bool _isStaticUid;

        #endregion Fields

        #region Initialization

        public LocalizationConverter(string uid, string vid, object defaultValue)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        #endregion IValueConverter Members

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (targetType is null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (culture is null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

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
                LocalizationDictionary dictionary = ResolveDictionary();
                var translatedObject = dictionary.Translate(_uid, _vid, _defaultValue, targetType);

                if (translatedObject == null || parametersCount == 0) return translatedObject;

                var parameters = new object[parametersCount];
                Array.Copy(values, values.Length - parametersCount, parameters, 0, parameters.Length);

                try
                {
                    translatedObject = string.Format(CultureInfo.InvariantCulture, translatedObject.ToString(), parameters);
                }
                catch (Exception)
                {
                    #region Trace

                    Debug.WriteLine($"LocalizationConverter failed to format text {translatedObject.ToString()}");

                    #endregion Trace

                    throw new LocalizationException(LocalizerError.FormattingFailed);
                }
                return translatedObject;
            }
            catch (LocalizationException ex)
            {
                #region Trace

                Debug.WriteLine($"LocalizationConverter failed to convert text: {ex.Message}");

                #endregion Trace
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }

        #endregion IMultiValueConverter Members

        #region Privates

        private static LocalizationDictionary ResolveDictionary()
        {
            var dictionary = LocalizationDictionary.GetResources(LocalizationService.Current.Culture);

            if (dictionary is null)
            {
                throw new InvalidOperationException(
                    $"Pack for language {LocalizationService.Current.Culture} was not found");
            }

            return dictionary;
        }

        #endregion Privates
    }
}