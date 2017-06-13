using System;
using System.ComponentModel;
using System.Globalization;

namespace UFA.Localization
{
    public sealed class LanguageProvider : ISupportInitialize
	{
		#region Fields

		private Type _dictionaryType;
		private CultureInfo _culture;
		private object _parameter; 

		#endregion

		#region Properties

		public Type DictionaryType
		{
			get { return _dictionaryType; }
			set { _dictionaryType = value; }
		}

		public CultureInfo Culture
		{
			get { return _culture; }
			set { _culture = value; }
		}

		public object Parameter
		{
			get { return _parameter; }
			set { _parameter = value; }
		}
 
		#endregion

		#region ISupportInitialize Members

		public void BeginInit()
		{
		}

		public void EndInit()
		{
			object instance = Activator.CreateInstance(_dictionaryType, new object[] {_parameter});
			LanguageDictionary dictionary = instance as LanguageDictionary;
			LanguageDictionary.RegisterDictionary(_culture, dictionary);
		}

		#endregion
	}
}
