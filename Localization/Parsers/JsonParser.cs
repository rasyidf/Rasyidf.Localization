using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Rasyidf.Localization.Parsers
{
    public static class JsonParser
    {
        static Stack<List<string>> splitArrayPool = new Stack<List<string>>();
        static StringBuilder stringBuilder = new StringBuilder();
        static readonly Dictionary<Type, Dictionary<string, FieldInfo>> FieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public static T FromJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json)) return (T)ParseValue(typeof(T), ""); 
            stringBuilder.Clear();
            for (var i = 0; i < json.Length; i++)
            {
                var c = json[i];
                if (c == '\"')
                {
                    i = AppendUntilStringEnd(true, i, json);
                    continue;
                }
                if (char.IsWhiteSpace(c))
                    continue;

                stringBuilder.Append(c);
            }


            return (T)ParseValue(typeof(T), stringBuilder.ToString());
        }

        static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
        {
            stringBuilder.Append(json[startIdx]);
            for (var i = startIdx + 1; i < json.Length; i++)
            {
                switch (json[i])
                {
                    case '\\':
                    {
                        if (appendEscapeCharacter)
                            stringBuilder.Append(json[i]);
                        stringBuilder.Append(json[i + 1]);
                        i++; 
                        break;
                    }
                    case '\"':
                        stringBuilder.Append(json[i]);
                        return i;
                    default:
                        stringBuilder.Append(json[i]);
                        break;
                }
            }
            return json.Length - 1;
        }

        //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
        static List<string> Split(string json)
        {
            var splitArray = splitArrayPool.Count > 0 ? splitArrayPool.Pop() : new List<string>();
            splitArray.Clear();

            var parseDepth = 0;

            stringBuilder.Clear();

            for (var i = 1; i < json.Length - 1; i++)
            {
                switch (json[i])
                {
                    case '[':
                    case '{': 
                        parseDepth++;
                        break;

                    case ']':
                    case '}':
                        parseDepth--;
                        break;

                    case '\"':
                        i = AppendUntilStringEnd(true, i, json);
                        continue;
                    case ',':
                    case ':':
                        if (parseDepth == 0)
                        {
                            splitArray.Add(stringBuilder.ToString());
                            stringBuilder.Length = 0;
                            continue;
                        }
                        break; 
                }

                stringBuilder.Append(json[i]);

            }
            splitArray.Add(stringBuilder.ToString());
            return splitArray;
        }

        internal static object ParseValue(Type type, string json)
        {
            if (json == "null")
            {
                return null;
            }

            if (type == typeof(string))
            {
                if (json.Length <= 2)
                    return string.Empty;
                var str = json.Substring(1, json.Length - 2);
                return str.Replace("\\\\", "\"\"").Replace("\\", string.Empty).Replace("\"\"", "\\");
            }
            if (type == typeof(int))
            {
                int.TryParse(json, out var result);
                return result;
            }
            if (type == typeof(float))
            {
                float.TryParse(json, out var result);
                return result;
            }
            if (type == typeof(double))
            {
                double.TryParse(json, out var result);
                return result;
            }
            if (type == typeof(bool))
            {
                return json.ToLower() == "true";
            }


            if (type.IsArray)
            {
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;
                var arrayType = type.GetElementType();

                var elems = Split(json);
                var newArray = Array.CreateInstance(arrayType, elems.Count);

                for (var i = 0; i < elems.Count; i++)
                    newArray.SetValue(ParseValue(arrayType, elems[i]), i);
                splitArrayPool.Push(elems);
                return newArray;
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listType = type.GetGenericArguments()[0];
                    if (json[0] != '[' || json[json.Length - 1] != ']')
                        return null;

                    var elems = Split(json);
                    var list = (IList)type.GetConstructor(new[] { typeof(int) }).Invoke(new object[] { elems.Count });
                    foreach (var t in elems)
                        list.Add(ParseValue(listType, t));

                    splitArrayPool.Push(elems);
                    return list;
                }

                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType, valueType;
                    {
                        var args = type.GetGenericArguments();
                        keyType = args[0];
                        valueType = args[1];
                    }

                    //Refuse to parse dictionary keys that aren't of type string
                    if (keyType != typeof(string))
                        return null;
                    //Must be a valid dictionary element
                    if (json[0] != '{' || json[json.Length - 1] != '}')
                        return null;
                    //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                    var elems = Split(json);
                    if (elems.Count % 2 != 0)
                        return null;

                    var dictionary = (IDictionary)type.GetConstructor(new[] { typeof(int) }).Invoke(new object[] { elems.Count / 2 });
                    for (var i = 0; i < elems.Count; i += 2)
                    {
                        if (elems[i].Length <= 2)
                            continue;
                        var keyValue = elems[i].Substring(1, elems[i].Length - 2);
                        var val = ParseValue(valueType, elems[i + 1]);
                        dictionary.Add(keyValue, val);
                    }
                    return dictionary;
                }
            }

            if (type == typeof(object))
            {
                return ParseAnonymousValue(json);
            }

            if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                return ParseObject(type, json);
            }

            return null;
        }

        static object ParseAnonymousValue(string json)
        {

            var jsonLength = json.Length;

            if (jsonLength == 0)
                return null;

            switch (json)
            {
                case "true":
                    return true;
                case "false":
                    return false;
            }

            if (json[0] == '{' && json[jsonLength - 1] == '}')
            {
                var elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;
                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (var i = 0; i < elems.Count; i += 2)
                    dict.Add(elems[i].Substring(1, elems[i].Length - 2), ParseAnonymousValue(elems[i + 1]));
                return dict;

            }
            if (json[0] == '[' && json[jsonLength - 1] == ']')
            {
                var items = Split(json);
                var finalList = new List<object>(items.Count);
                finalList.AddRange(items.Select(ParseAnonymousValue));

                return finalList;
            }
            if (json[0] == '\"' && json[jsonLength - 1] == '\"')
            {
                return json.Substring(1, jsonLength - 2).Replace("\\", string.Empty);
            }

            if (!char.IsDigit(json[0]) && json[0] != '-') return null;

            if (json.Contains("."))
            {
                double.TryParse(json, out var value);
                return value;
            }

            int.TryParse(json, out var valueInt);
            return valueInt;

            // handles json == "null" as well as invalid JSON
        }

        static object ParseObject(Type type, string json)
        {
            var instance = FormatterServices.GetUninitializedObject(type);

            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            var elems = Split(json);
            if (elems.Count % 2 != 0)
                return instance;

            if (!FieldInfoCache.TryGetValue(type, out var nameToField))
            {
                nameToField = type.GetFields().Where(field => field.IsPublic).ToDictionary(field => field.Name);
                FieldInfoCache.Add(type, nameToField);
            }
            if (!PropertyInfoCache.TryGetValue(type, out var nameToProperty))
            {
                nameToProperty = type.GetProperties().ToDictionary(p => p.Name);
                PropertyInfoCache.Add(type, nameToProperty);
            }

            for (var i = 0; i < elems.Count; i += 2)
            {
                if (elems[i].Length <= 2)
                    continue;

                var key = elems[i].Substring(1, elems[i].Length - 2);
                var value = elems[i + 1];

                if (nameToField.TryGetValue(key, out var fieldInfo))
                    fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value));
                else if (nameToProperty.TryGetValue(key, out var propertyInfo))
                    propertyInfo.SetValue(instance, ParseValue(propertyInfo.PropertyType, value), null);
            }

            return instance;
        }
    }
}