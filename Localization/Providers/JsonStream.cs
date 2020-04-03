using System.Collections.Generic;
using System;
using System.IO;
using System.Globalization;
using System.Diagnostics;

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
         
        private static string SetIfContains(Dictionary<string, object> pairs, string key, string defaults = "")
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

            var author = SetIfContains(a, "Author", "Anonymous");

            bool IsSinglePack = false;

            if (a["Type"] is string)
            {
                if (a["Type"].ToString().ToUpperInvariant() == "SINGLE")
                {
                    IsSinglePack = true;
                }
            }

            if (IsSinglePack)
            {
                var tmp = new LocalizationDictionary()
                {
                    Version = SetIfContains(a, "Version"),
                    Author = author,
                    EnglishName = SetIfContains(a, "EnglishName"),
                    CultureName = SetIfContains(a, "CultureName"),
                    CultureId = SetIfContains(a, "Culture"),
                    RTL = SetIfContains(a, "RTL","FALSE").ToUpperInvariant() == "TRUE"
                };

                if (!(a["Data"] is List<object> data)) return;

                foreach (Dictionary<string, object> node in data)
                {
                    if (node == null) continue;
                    var innerData = new Dictionary<string, string>();
                    foreach (var attribute in node)
                    {
                        if (attribute.Key.ToUpperInvariant() == "ID") continue;
                        innerData[attribute.Key] = attribute.Value.ToString();
                    }

                    if (tmp.Data.ContainsKey(node["Id"].ToString())) continue;

                    tmp.Data.Add(node["Id"].ToString(), innerData);
                }

                Packs.Add(tmp);
            }
            else
            {
                var version = SetIfContains(a, "Version");
                Dictionary<string, LocalizationDictionary> temps = new Dictionary<string, LocalizationDictionary>();

                if (!(a["Languages"] is List<object> langs)) return;

                foreach (Dictionary<string, object> node in langs)
                {
                    if (node == null) continue;
                    var tmp = new LocalizationDictionary()
                    {
                        Version = version,
                        EnglishName = SetIfContains(node, "EnglishName"),
                        CultureName = SetIfContains(node, "CultureName"),
                        Author = author,
                        CultureId = SetIfContains(node, "Culture"),
                        RTL = SetIfContains(a, "RTL", "FALSE").ToUpperInvariant() == "TRUE"
                    };
                    temps.Add(tmp.CultureId, tmp);
                }

                if (!(a["Data"] is List<object> data)) return;

                foreach (var node in data)
                {
                    if (node == null) continue; 
                    if(!(data[0] is Dictionary<string, object> nodec)) return;
                    if(!(nodec["data"] is List<object> nodechild)) return;
                    foreach (Dictionary<string, object> attributes in nodechild)
                    {
                        var innerData = new Dictionary<string, Dictionary<string, string>>();
                        var currId = 0;
                        foreach (var attribute in attributes) {
                            if (attribute.Key.ToUpperInvariant() == "ID")
                            {
                                currId = Convert.ToInt32(attribute.Value, CultureInfo.InvariantCulture);
                                
                                continue;
                            }
                            if (!(attribute.Value is Dictionary<string, object> child)) continue;
                            
                            foreach (var item in child)
                            {
                                if (!innerData.ContainsKey(item.Key))
                                {
                                    innerData[item.Key] = new Dictionary<string, string>();
                                }

                                innerData[item.Key][attribute.Key] = item.Value.ToString() ;
                                
                            }
                        }
                        foreach (var item in innerData)
                        {
                            var vals = item.Value; 
                            if (!temps[item.Key].Data.ContainsKey($"{currId}"))
                            {
                                temps[item.Key].Data[$"{currId}"] = new Dictionary<string, string>();
                            }
                            foreach (var ia in vals)
                            {
                                temps[item.Key].Data[$"{currId}"].Add(ia.Key,ia.Value); 
                            }
                        } 
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