using System;
using System.Collections.Generic;
using System.IO;
using Rasyidf.Localization.Parsers;

namespace Rasyidf.Localization.Providers
{
    public class JsonStream : StreamBase
    {



        public JsonStream(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException($"File {path} doesn't exist");
            Path = path;
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
            if (a == null) throw new FileFormatException("Json load failed");
            var tmp = new LanguageItem()
            {
                Version = SetIfContains(a, "Version"),
                EnglishName = SetIfContains(a, "EnglishName"),
                CultureName = SetIfContains(a, "CultureName"),
                CultureId = SetIfContains(a, "Culture")
            };

            bool IsSinglePack = false;

            if (a["Type"] is string)
            {
                if (a["Type"].ToString().ToLower() == "single")
                {
                    IsSinglePack = true;
                }
            }

            if (IsSinglePack)
            {
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

                    if (tmp.Data.ContainsKey(node["Id"].ToString())) continue;

                    tmp.Data.Add(node["Id"].ToString(), innerData);
                }
                Packs.Add(tmp);
            }
        }

        protected override void OnUnload()
        {

        }


    }
}