using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFA.Localization;

namespace Localization.Json
{
    public class JsonLanguageDictionary : LanguageDictionary
    {
        private Dictionary<string, Dictionary<string, string>> _data =
            new Dictionary<string, Dictionary<string, string>>();


        private string _path;
        private string _cultureId;
        private string _cultureName;
        private string _englishName;

        public string Path
        {
            get => _path; set => _path = value;
        }

        public override string CultureName => _cultureName;
        public override string EnglishName => _englishName;
        public override string CultureId => _cultureId;


        public JsonLanguageDictionary(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException(string.Format("File {0} doesn't exist", path));
            }
            this._path = path;
        }

        protected override void OnLoad()
        {

            string fileJson = File.ReadAllText(_path);
            var a = fileJson.FromJson<Dictionary<string, object>>();

            if (a.ContainsKey("EnglishName"))
            {
                _englishName = a["EnglishName"].ToString();
            }

            if (a.ContainsKey("CultureName"))
            {
                _cultureName = a["CultureName"].ToString();
            }
            if (a.ContainsKey("Culture"))
            {
                _cultureId = a["Culture"].ToString();
            }
            var datas = a["Data"] as List<object>;
            foreach (Dictionary<string,object> node in datas)
            {
                var innerdata = new Dictionary<string, string>();
                foreach (KeyValuePair<string, object> attribute in node)
                {
                    if (!(attribute.Key == "id"))
                    {
                        innerdata[attribute.Key] = attribute.Value.ToString();
                    } 
                }
                if (!_data.ContainsKey(node["Id"].ToString()))
                {
                    _data.Add(node["Id"].ToString(),innerdata);
                }
            }
        }

        protected override void OnUnload()
        {
            _data.Clear();
        }

        protected override object OnTranslate(string uid, string vid, object defaultValue, Type type)
        {
            if (string.IsNullOrEmpty(uid))
            {
                #region Trace
                Debug.WriteLine(string.Format("Uid must not be null or empty"));
                #endregion
                return defaultValue;
            }
            if (string.IsNullOrEmpty(vid))
            {
                #region Trace
                Debug.WriteLine(string.Format("Vid must not be null or empty"));
                #endregion
                return defaultValue;
            }
            if (!_data.ContainsKey(uid))
            {
                #region Trace
                Debug.WriteLine(string.Format("Uid {0} was not found in the {1} dictionary", uid, EnglishName));
                #endregion
                return defaultValue;
            }
            Dictionary<string, string> innerData = _data[uid];

            if (!innerData.ContainsKey(vid))
            {
                #region Trace
                Debug.WriteLine(string.Format("Vid {0} was not found for Uid {1}, in the {2} dictionary", vid, uid, EnglishName));
                #endregion
                return defaultValue;
            }
            string textValue = innerData[vid];
            try
            {
                if (type == typeof(object))
                {
                    return textValue;
                }
                else
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                    object translation = typeConverter.ConvertFromString(textValue);
                    return translation;
                }
            }
            catch (Exception ex)
            {
                #region Trace
                Debug.WriteLine(string.Format("Failed to translate text {0} in dictionary {1}:\n{2}", textValue, EnglishName, ex.Message));
                #endregion
                return null;
            }
        }
    }
}
