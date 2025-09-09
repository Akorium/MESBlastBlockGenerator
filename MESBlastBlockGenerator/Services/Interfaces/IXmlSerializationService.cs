using System;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Services.Interfaces
{
    public interface IXmlSerializationService
    {
        string Serialize<T>(T obj, XmlSerializerNamespaces namespaces);
        T Deserialize<T>(string xmlContent) where T : class;
        XmlSerializer GetSerializer(Type type);
    }
}