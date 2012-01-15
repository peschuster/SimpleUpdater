using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SimpleUpdater.Core.Import;

namespace SimpleUpdater.Core
{
    internal class UpdateInfoWrapper
    {
        private readonly XmlRsaSignature signature;
        
        public UpdateInfoWrapper(XmlRsaSignature signature)
        {
            this.signature = signature;
        }

        public UpdateInfo Data { get; set; }

        public bool Load(string content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            if (!this.signature.VerifyDocument(xmlDoc))
                return false;

            xmlDoc = this.signature.ExtractSignature(xmlDoc);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UpdateInfo));

                using (StringReader xmlString = new StringReader(xmlDoc.OuterXml))
                {
                    using (XmlReader reader = new XmlTextReader(xmlString))
                    {
                        Data = (UpdateInfo)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());

                return false;
            }

            return true;
        }

        public string Export()
        {
            StringBuilder content = new StringBuilder();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UpdateInfo));

                using (TextWriter xmlString = new StringWriter(content))
                {
                    using (XmlWriter writer = new XmlTextWriter(xmlString))
                    {
                        serializer.Serialize(writer, Data);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());

                return null;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content.ToString());

            this.signature.SignDocument(xmlDoc);

            return xmlDoc.OuterXml;
        }
    }
}
