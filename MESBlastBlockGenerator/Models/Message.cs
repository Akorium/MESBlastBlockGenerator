using System.Xml.Serialization;
using System.Xml;

public class Message
{
    [XmlIgnore]
    public string XmlContent { get; set; }

    [XmlText]
    public XmlNode[] CDataContent
    {
        get
        {
            if (string.IsNullOrEmpty(XmlContent))
                return null;

            var doc = new XmlDocument();
            return new XmlNode[] { doc.CreateCDataSection(XmlContent) };
        }
        set
        {
            if (value == null)
            {
                XmlContent = null;
                return;
            }

            if (value.Length > 0)
                XmlContent = value[0].Value;
        }
    }
}