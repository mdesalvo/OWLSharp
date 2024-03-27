/*
   Copyright 2014-2024 Marco De Salvo

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using RDFSharp.Model;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System;

namespace OWLSharp
{
    public static class OWLSerializer
    {
        public static string Serialize(OWLOntology ontology)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot serialize OWLOntology because given \"ontology\" parameter is null");
            #endregion

            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            //Hide hard-coded .NET prefixes (e.g: xsi)
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            //Initialize standard Semantic Web prefixes
            xmlSerializerNamespaces.Add(RDFVocabulary.OWL.PREFIX, RDFVocabulary.OWL.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.RDF.PREFIX, RDFVocabulary.RDF.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.XSD.PREFIX, RDFVocabulary.XSD.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.XML.PREFIX, RDFVocabulary.XML.BASE_URI);
            //Initialize user-declared prefixes
            ontology.Prefixes.ForEach(pfx => xmlSerializerNamespaces.Add(pfx.Name, pfx.IRI));
            //Reorder axioms for pretty-printing
            ontology.Axioms.Sort((ax1,ax2) => ax1.SerializationPriority.CompareTo(ax2.SerializationPriority));

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
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot deserialize OWLOntology because given \"ontology\" parameter is null");
            #endregion

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