using System;
using System.Collections.Generic;
using System.IO;
using Rasyidf.Localization.Parsers;

namespace Rasyidf.Localization.Providers
{
    public class JsonStream : LanguagePackBase
    { 

        public string Path { get; set; }
         

        public JsonStream(string path)
        {
            if (File.Exists(path))
                Path = path;
            else
                throw new ArgumentException($"File {path} doesn't exist");
        }

        static string SetIfContains(Dictionary<string, object> pairs, string key, string defaults = "")
        {
            return pairs.ContainsKey(key)
                   ? pairs[key].ToString()
                   : defaults;
        }

        protected override void OnLoad()
        {
            var fileJson = File.ReadAllText(Path);

            var a = fileJson.FromJson<Dictionary<string, object>>();
            if (a == null) return; 

            EnglishName = SetIfContains(a, "EnglishName");
            CultureName = SetIfContains(a, "CultureName");
            CultureId = SetIfContains(a, "Culture");

            if (!(a["Data"] is List<object> data)) return;

            foreach (Dictionary<string, object> node in data)
            {
                if (node == null) continue; 
                var innerData = new Dictionary<string, string>();
                foreach (var attribute in node)
                {
                    if (attribute.Key == "id") continue;
                    innerData[attribute.Key] = attribute.Value.ToString();
                }

                if (Data.ContainsKey(node["Id"].ToString())) continue;
                Data.Add(node["Id"].ToString(), innerData);
            }
        }

        protected override void OnUnload()
        {
            Data.Clear();
        }

      
    }
}