using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleUpdater.Core.Import
{
    public class UpdateInfo
    {
        [XmlElement("Latest")]
        public string LatestString { get; set; }

        [XmlIgnore]
        public Version Latest
        {
            get { return new Version(LatestString); }
            set { LatestString = value.ToString(); }
        }

        [XmlElement("Timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlArray("Versions")]
        [XmlArrayItem("Version")]
        public List<VersionEntry> Versions { get; set; }
        
    }
}
