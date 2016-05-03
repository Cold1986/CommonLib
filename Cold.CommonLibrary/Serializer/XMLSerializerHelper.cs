using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommonLibrary
{
    public class XMLSerializerHelper
    {
        public static string XmlSerialize<T>(T item, bool removeNamespaces)
        {
            try
            {
                XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
                xmlns.Add(string.Empty, string.Empty);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
                StringBuilder stringBuilder = new StringBuilder();
                using (StringWriter stringWriter = new StringWriter(stringBuilder))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        if (removeNamespaces)
                            xmlSerializer.Serialize(xmlWriter, item, xmlns);
                        else
                            xmlSerializer.Serialize(xmlWriter, item);
                        return stringBuilder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
