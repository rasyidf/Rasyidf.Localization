namespace Rasyidf.Localization
{
    public class XmlStream : StreamBase
    {
        public XmlStream(string path)
        {
            if (File.Exists(path))
                Path = path;
            else
                throw new ArgumentException($"File {path} doesn't exist");
        }

        protected override void OnLoad()
        {
            var xmlDocument = new XmlDocument() { XmlResolver = null };
            using (var sreader = new StringReader(Path))
            using (var reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null, Async = true }))
            {
                xmlDocument.Load(reader);
            }

            var docElement = xmlDocument.DocumentElement;

            if (docElement == null) return;

            if (docElement.Name != "Pack")
            {
                throw new XmlException("Invalid root element. Must be Pack");
            }

            var tmp = new LocalizationDictionary()
            {
                Version = docElement.Attributes["Version"]?.Value,
                Author = docElement.Attributes["Author"]?.Value,
                EnglishName = docElement.Attributes["EnglishName"]?.Value,
                CultureName = docElement.Attributes["CultureName"]?.Value,
                CultureId = docElement.Attributes["Culture"]?.Value,
            };

            foreach (XmlNode node in docElement.ChildNodes)
            {
                if (node.Name != "Value") continue;

                var innerData = new Dictionary<string, string>();

                if (node.Attributes == null) continue;

                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name == "Id")
                    {
                        if (tmp.Data.ContainsKey(attribute.Value)) continue;

                        tmp.Data[attribute.Value] = innerData;
                    }
                    else
                    {
                        innerData[attribute.Name] = attribute.Value;
                    }
                }
            }
            Packs.Add(tmp);
        }

        protected override void OnUnload()
        {
        }
    }
}