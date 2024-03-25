using RDFSharp.Model;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace OWLSharp
{
    public class OWLOntologySerializer
    {
        public static string Serialize(OWLOntology ontology)
        {
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            xmlSerializerNamespaces.Add(RDFVocabulary.OWL.PREFIX, RDFVocabulary.OWL.BASE_URI);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OWLOntology));
            using (UTF8StringWriter stringWriter = new UTF8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, 
                    new XmlWriterSettings()
                    {
                        Encoding = stringWriter.Encoding,
                        Indent = true,
                        NewLineHandling = NewLineHandling.None
                    }))
                {
                    xmlSerializer.Serialize(writer, ontology, xmlSerializerNamespaces);
                    return stringWriter.ToString();
                }
            }
        }

        public static OWLOntology Deserialize(string ontology)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OWLOntology));
            using (StringReader stringReader = new StringReader(ontology))
            {
                using (XmlReader reader = XmlReader.Create(stringReader,
                    new XmlReaderSettings()
                    {
                        DtdProcessing = DtdProcessing.Parse,
                        IgnoreComments = true,
                        IgnoreWhitespace = true,
                        IgnoreProcessingInstructions = true
                    }))
                {
                    return (OWLOntology)xmlSerializer.Deserialize(reader);
                }
            }
        }
    }

    internal class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}