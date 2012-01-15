using System;
using System.Xml.Serialization;

namespace SimpleUpdater.Core.Import
{
    [Serializable]
    public class VersionEntry
    {
        [XmlElement("Version")]
        public string VersionString { get; set; }

        [XmlIgnore]
        public Version Version
        {
            get { return new Version(VersionString); }
            set { VersionString = value.ToString(); }
        }

        [XmlElement("Date")]
        public DateTime Date { get; set; }

        [XmlElement("Required")]
        public string RequiredString { get; set; }

        [XmlIgnore]
        public Version Required
        {
            get { return new Version(RequiredString); }
            set { RequiredString = value.ToString(); }
        }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlAttribute("File")]
        public string File { get; set; }

        [XmlAttribute("Md5Hash")]
        public string Md5Hash { get; set; }
    }
}
