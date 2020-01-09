using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Rasyidf.Localization
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

            var author = SetIfContains(a, "Author","Anonymous");

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
                var tmp = new LanguageItem()
                {
                    Version = SetIfContains(a, "Version"),
                    Author = author,
                    EnglishName = SetIfContains(a, "EnglishName"),
                    CultureName = SetIfContains(a, "CultureName"),
                    CultureId = SetIfContains(a, "Culture"),
                    RTL = Convert.ToBoolean(SetIfContains(a, "RTL"))
                };

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
            } else
            {
                Dictionary<string, LanguageItem> temps = new Dictionary<string, LanguageItem>();

                if (!(a["Languages"] is List<object> langs)) return;

                foreach (Dictionary<string, object> node in langs)
                {
                    if (node == null) continue;
                    var tmp = new LanguageItem()
                    {
                        EnglishName = SetIfContains(node, "EnglishName"),
                        CultureName = SetIfContains(node, "CultureName"),
                        Author = author,
                        CultureId = SetIfContains(node, "Culture"),
                        RTL = Convert.ToBoolean(SetIfContains(a, "RTL"))
                    };
                    temps.Add(tmp.CultureId, tmp);
                }

                if (!(a["Data"] is List<object> data)) return;

                foreach (Dictionary<string, object> node in data)
                {
                    if (node == null) continue;
                    var innerData = new Dictionary<string,Dictionary<string, string>>();
                    foreach (var attribute in node)
                    {
                        if (attribute.Key == "id")
                        {
                            Debug.Print(attribute.Value.ToString());
                            continue;
                        }
                        if (!(attribute.Value is List<object> child)) continue;
                        foreach (var item in child)
                        {
                            if (item is Dictionary<string,string> dict)
                            {
                                foreach (var di in dict)
                                {
                                    innerData[di.Key][attribute.Key] = di.Value.ToString();
                                }
                            } 
                        }
                    }
                    foreach (var item in innerData)
                    {
                        if (temps[item.Key].Data.ContainsKey(node["Id"].ToString())) continue;

                        temps[item.Key].Data.Add(node["Id"].ToString(), item.Value);
                    }

                }
                foreach (var item in temps.Values)
                {
                    Packs.Add(item); 
                }
                temps.Clear();
            }
        }

        protected override void OnUnload()
        {

        }


    }
}