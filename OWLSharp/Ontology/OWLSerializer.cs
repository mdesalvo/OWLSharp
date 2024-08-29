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
using System.Collections.Generic;
using System.Linq;
using System;

namespace OWLSharp.Ontology
{
    internal static class OWLSerializer
    {
        internal static string SerializeOntology(OWLOntology ontology)
        {
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            //Hide hard-coded .NET prefixes (e.g: xsi)
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            //Initialize Semantic Web prefixes
            xmlSerializerNamespaces.Add(RDFVocabulary.OWL.PREFIX, RDFVocabulary.OWL.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.RDF.PREFIX, RDFVocabulary.RDF.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.XSD.PREFIX, RDFVocabulary.XSD.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.XML.PREFIX, RDFVocabulary.XML.BASE_URI);
            //Initialize user-declared prefixes
            ontology.Prefixes.ForEach(pfx => 
            {
                if (!string.Equals(pfx.Name, RDFVocabulary.OWL.PREFIX, System.StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(pfx.Name, RDFVocabulary.RDFS.PREFIX, System.StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(pfx.Name, RDFVocabulary.RDF.PREFIX, System.StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(pfx.Name, RDFVocabulary.XSD.PREFIX, System.StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(pfx.Name, RDFVocabulary.XML.PREFIX, System.StringComparison.OrdinalIgnoreCase))
                    xmlSerializerNamespaces.Add(pfx.Name, pfx.IRI);
            });

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

        internal static OWLOntology DeserializeOntology(string ontology)
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
                    OWLOntology owlOntology = (OWLOntology)xmlSerializer.Deserialize(reader);

                    //Remove duplicated prefixes
                    List<OWLPrefix> prefixes = new List<OWLPrefix>();
                    owlOntology.Prefixes.ForEach(pfx =>
                    {
                        if (!prefixes.Any(pfxs => string.Equals(pfxs.Name, pfx.Name, StringComparison.OrdinalIgnoreCase)))
                            prefixes.Add(pfx);
                    });
                    owlOntology.Prefixes = prefixes;

                    return owlOntology;
                }
            }
        }
    
		internal static string SerializeObject<T>(T objectToSerialize, XmlSerializerNamespaces xmlSerializerNamespaces=null) where T : class
        {
            //Hide hard-coded .NET prefixes (e.g: xsi)
			if (xmlSerializerNamespaces == null)
            	xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (UTF8StringWriter stringWriter = new UTF8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, 
                    new XmlWriterSettings()
                    {
                        Encoding = stringWriter.Encoding,
                        Indent = false,
                        NewLineHandling = NewLineHandling.None,
                        OmitXmlDeclaration = true
                    }))
                {
                    xmlSerializer.Serialize(writer, objectToSerialize, xmlSerializerNamespaces);
                    return stringWriter.ToString();
                }
            }
        }

        internal static T DeserializeObject<T>(string objectToDeserialize) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(objectToDeserialize))
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
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
        }
	}

    internal class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}