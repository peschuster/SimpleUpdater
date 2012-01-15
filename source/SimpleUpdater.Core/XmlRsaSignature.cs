using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SimpleUpdater.Core
{
    /// <summary>
    /// Klasse zum Signieren von XML-Dokumenten
    /// </summary>
    internal class XmlRsaSignature
    {
        /// <summary>
        /// Asymetrisches Verfahren mit Schlüsselinformationen
        /// </summary>
        private RSA rsa = null;

        /// <summary>
        /// Erzeugt eine neue Instanz der XmlSignature-Klasse
        /// </summary>
        /// <param name="xmlRsaKey">RSA key(s)</param>
        public XmlRsaSignature(string xmlRsaKey)
        {
            rsa = RSA.Create();
         
            rsa.FromXmlString(xmlRsaKey);
        }

        /// <summary>
        /// Erzeugt für das übergebene XML-Dokument eine Signatur 
        /// und fügt diese in das Dokument ein
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public XmlDocument SignDocument(XmlDocument doc)
        {
            // Erstelle eine Referenz für die Transformation und 
            // füge diese Referenz dem Signatur-Wrapper hinzu
            Reference reference = new Reference();
            reference.Uri = String.Empty;
            
            XmlDsigEnvelopedSignatureTransform envelop = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(envelop);

            // Erstelle den Signatur-Wrapper mit Schlüsselinformationen
            SignedXml signedDoc = new SignedXml(doc);
            signedDoc.SigningKey = rsa;
            signedDoc.AddReference(reference);

            // Berechne die Signatur
            signedDoc.ComputeSignature();

            // Füge die Signatur in das XML-Dokument ein und gebe es zurück
            doc.DocumentElement.AppendChild(doc.ImportNode(signedDoc.GetXml(), true));

            return doc;
        }
        
        /// <summary>
        /// Verifies the xml document (<paramref name="doc"/>).
        /// </summary>
        /// <param name="doc">The xml document.</param>
        /// <returns>True, if document signature is valid.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="doc"/> is null or no crypto algorithm is set.</exception>
        /// <exception cref="System.ArgumentException">If document does not contain a xml signature.</exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException">On invalid signature format or wrong configured crypto algorithm.</exception>
        public bool VerifyDocument(XmlDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            // extract the xml signature element.
            XmlNodeList nodeList = doc.GetElementsByTagName("Signature");

            if (nodeList.Count < 1)
            {
                throw new ArgumentException("XmlDocument does not contain a signature node.", "doc");
            }

            SignedXml signedDoc = new SignedXml(doc);
            signedDoc.LoadXml((XmlElement)nodeList[0]);

            // Überprüfe das Dokument anhand der Schlüsselinformationen
            return signedDoc.CheckSignature(rsa);
        }

        /// <summary>
        /// Verifies the <paramref name="doc"/> and returns the original XmlDocument with out signature.
        /// </summary>
        /// <param name="doc">The xml document.</param>
        /// <returns>The original xml document with put signature.</returns>
        public XmlDocument ExtractSignature(XmlDocument doc)
        {
            if (!VerifyDocument(doc))
                throw new CryptographicException("Document signature not valid!");

            XmlNodeList nodeList = doc.GetElementsByTagName("Signature");
            if (nodeList.Count > 0)
            {
                doc.DocumentElement.RemoveChild(nodeList[0]);
            }

            return doc;
        }
    }
}