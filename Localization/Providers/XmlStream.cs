using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Rasyidf.Localization.Providers
{
    public class XmlStream : LanguagePackBase
    {

        public string Path { get; set; }
        
        public XmlStream(string path)
        {
            if (File.Exists(path))
                Path = path;
            else
                throw new ArgumentException($"File {path} doesn't exist");
        }

        protected override void OnLoad()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(Path);

            var docElement = xmlDocument.DocumentElement;

            if (docElement == null) return;

            if (docElement.Name != "Pack")
            {
                throw new XmlException("Invalid root element. Must be Pack");
            }


            EnglishName = docElement.Attributes["EnglishName"]?.Value; 
            CultureName = docElement.Attributes["CultureName"]?.Value; 
            CultureId   = docElement.Attributes["Culture"]?.Value;


            foreach (XmlNode node in docElement.ChildNodes)
            {
                if (node.Name != "Value") continue;

                var innerData = new Dictionary<string, string>();

                if (node.Attributes == null) continue;

                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name == "Id")
                    {
                        if (Data.ContainsKey(attribute.Value)) continue;

                        Data[attribute.Value] = innerData;
                    }
                    else
                    {
                        innerData[attribute.Name] = attribute.Value;
                    }
                }
            }
        }

        protected override void OnUnload()
        {
            Data.Clear();
        }


    }
}