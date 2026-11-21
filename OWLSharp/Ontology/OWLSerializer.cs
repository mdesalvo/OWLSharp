/*
   Copyright 2014-2026 Marco De Salvo

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
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLSerializer is responsible for XML serialization and deserialization of any kind of ontology elements
    /// </summary>
    internal static class OWLSerializer
    {
        private static readonly XmlWriterSettings DefaultXmlWriterSettings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            NewLineHandling = NewLineHandling.None
        };
        private static readonly XmlWriterSettings CompactXmlWriterSettings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = false,
            NewLineHandling = NewLineHandling.None,
            OmitXmlDeclaration = true
        };
        private static readonly XmlReaderSettings DefaultXmlReaderSettings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            IgnoreComments = true,
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = true
        };

        //Names of the attributes whose value is an IRI that the OWL2/XML standard allows to be relative
        private static readonly HashSet<string> IRIAttributeNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "IRI", "ontologyIRI", "versionIRI", "facet", "datatypeIRI"
        };

        //An IRI is absolute when it starts with a scheme (RFC 3986): anything else is a reference relative to the document base
        private static readonly System.Text.RegularExpressions.Regex AbsoluteIRIRegex = new System.Text.RegularExpressions.Regex(@"^[A-Za-z][A-Za-z0-9+.\-]*:", System.Text.RegularExpressions.RegexOptions.Compiled);

        //Names of the elements whose model reads AbbreviatedIRI children as XML qualified names
        private static readonly HashSet<string> QualifiedNameParentNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "Annotation", "AnnotationPropertyDomain", "AnnotationPropertyRange"
        };

        /// <summary>
        /// Serializes the given ontology into an OWL2/XML string representation
        /// </summary>
        internal static string SerializeOntology(OWLOntology ontology)
        {
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            //Hide hard-coded .NET prefixes (e.g: xsi)
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            //Initialize commonly expected prefixes
            xmlSerializerNamespaces.Add(RDFVocabulary.OWL.PREFIX, RDFVocabulary.OWL.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.RDF.PREFIX, RDFVocabulary.RDF.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.XSD.PREFIX, RDFVocabulary.XSD.BASE_URI);
            xmlSerializerNamespaces.Add(RDFVocabulary.XML.PREFIX, RDFVocabulary.XML.BASE_URI);
            //Initialize user-declared prefixes
            ontology.Prefixes.ForEach(pfx =>
            {
                if (!string.Equals(pfx.Name, RDFVocabulary.OWL.PREFIX, StringComparison.OrdinalIgnoreCase)
                     && !string.Equals(pfx.Name, RDFVocabulary.RDFS.PREFIX, StringComparison.OrdinalIgnoreCase)
                     && !string.Equals(pfx.Name, RDFVocabulary.RDF.PREFIX, StringComparison.OrdinalIgnoreCase)
                     && !string.Equals(pfx.Name, RDFVocabulary.XSD.PREFIX, StringComparison.OrdinalIgnoreCase)
                     && !string.Equals(pfx.Name, RDFVocabulary.XML.PREFIX, StringComparison.OrdinalIgnoreCase))
                    xmlSerializerNamespaces.Add(pfx.Name, pfx.IRI);
            });

            #region Exclude Imports
            OWLOntology exportOntology = new OWLOntology(ontology);

            //Axioms
            exportOntology.DeclarationAxioms.RemoveAll(ax => ax.IsImport);
            exportOntology.ClassAxioms.RemoveAll(ax => ax.IsImport);
            exportOntology.ObjectPropertyAxioms.RemoveAll(ax => ax.IsImport);
            exportOntology.DataPropertyAxioms.RemoveAll(ax => ax.IsImport);
            exportOntology.DatatypeDefinitionAxioms.RemoveAll(ax => ax.IsImport);
            exportOntology.KeyAxioms.RemoveAll(ax => ax.IsImport);
            exportOntology.AssertionAxioms.RemoveAll(ax => ax.IsImport);
            exportOntology.AnnotationAxioms.RemoveAll(ax => ax.IsImport);

            //Rules
            exportOntology.Rules.RemoveAll(rl => rl.IsImport);
            #endregion

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OWLOntology));
            using (UTF8StringWriter utf8StringWriter = new UTF8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(utf8StringWriter, DefaultXmlWriterSettings))
                {
                    xmlSerializer.Serialize(xmlWriter, exportOntology, xmlSerializerNamespaces);
                    return utf8StringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserializes the given OWL2/XML string into an ontology representation, accepting OWL-namespaced elements (default or prefixed) and both versionIRI and legacy ontologyVersion
        /// </summary>
        internal static OWLOntology DeserializeOntology(string ontology)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OWLOntology));
            using (StringReader stringReader = new StringReader(ontology))
            {
                using (XmlReader reader = XmlReader.Create(stringReader, DefaultXmlReaderSettings))
                {
                    //The document is loaded through the hardened reader so that DTD and resolver policies still apply
                    XDocument ontologyDocument = XDocument.Load(reader, LoadOptions.PreserveWhitespace);

                    //Standard OWL2/XML documents are mapped onto the namespace-less shape that the model classes expect
                    NormalizeOntologyDocument(ontologyDocument);

                    using (XmlReader normalizedReader = ontologyDocument.CreateReader())
                    {
                        OWLOntology owlOntology = (OWLOntology)xmlSerializer.Deserialize(normalizedReader);

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
        }

        /// <summary>
        /// Rewrites a standard OWL2/XML document into the namespace-less and absolute-IRI shape that the model classes expect
        /// </summary>
        private static void NormalizeOntologyDocument(XDocument ontologyDocument)
        {
            //An empty document has nothing to normalize (the serializer will report it)
            if (ontologyDocument.Root == null)
                return;

            //The snapshot allows the later passes to rewrite element content without disturbing the enumeration
            List<XElement> ontologyElements = ontologyDocument.Descendants().ToList();

            //Standard documents put every element in the OWL namespace: the model only knows local names
            RemoveOWLNamespaceFromElements(ontologyElements);

            //Files written by previous OWLSharp versions carry the version IRI as ontologyVersion
            XAttribute legacyVersionAttribute = ontologyDocument.Root.Attribute("ontologyVersion");
            if (legacyVersionAttribute != null)
            {
                //The standard attribute wins when both are present: the legacy one is simply dropped
                if (ontologyDocument.Root.Attribute("versionIRI") == null)
                    ontologyDocument.Root.SetAttributeValue("versionIRI", legacyVersionAttribute.Value);
                legacyVersionAttribute.Remove();
            }

            //Relative IRIs (e.g: those written by Protege) are only meaningful against the base of the document
            ResolveRelativeIRIs(ontologyDocument.Root, ontologyElements);

            //Abbreviated IRIs are only meaningful against the prefixes declared by the document
            ExpandAbbreviatedIRIs(ontologyDocument.Root, ontologyElements);
        }

        /// <summary>
        /// Turns every OWL-namespaced element into a namespace-less element and drops the default OWL namespace declaration
        /// </summary>
        private static void RemoveOWLNamespaceFromElements(List<XElement> ontologyElements)
        {
            //The OWL2/XML standard puts every element in this namespace (as default or prefixed)
            XNamespace owlNamespace = XNamespace.Get(RDFVocabulary.OWL.BASE_URI);

            foreach (XElement ontologyElement in ontologyElements)
            {
                //Only OWL elements are renamed: elements of other vocabularies must keep their identity
                if (ontologyElement.Name.Namespace == owlNamespace)
                    ontologyElement.Name = XNamespace.None + ontologyElement.Name.LocalName;

                //The default declaration of the OWL namespace would contradict the renamed elements
                ontologyElement.Attributes()
                               .Where(attribute => attribute.IsNamespaceDeclaration
                                                    && attribute.Name.Namespace == XNamespace.None
                                                    && string.Equals(attribute.Value, RDFVocabulary.OWL.BASE_URI, StringComparison.Ordinal))
                               .Remove();
            }
        }

        /// <summary>
        /// Makes absolute every relative IRI of the document, resolving it against xml:base or the ontology IRI
        /// </summary>
        private static void ResolveRelativeIRIs(XElement ontologyRoot, List<XElement> ontologyElements)
        {
            //The explicit xml:base of the document wins over the ontology IRI
            string baseText = (string)ontologyRoot.Attribute(XNamespace.Xml + "base") ?? (string)ontologyRoot.Attribute("ontologyIRI");

            //Without an absolute base there is nothing to resolve against: values stay as they are
            if (!Uri.TryCreate(baseText, UriKind.Absolute, out Uri baseUri))
                return;

            foreach (XElement ontologyElement in ontologyElements)
            {
                //IRI-valued attributes are those of the OWL2/XML standard (including the SWRL ones)
                foreach (XAttribute attribute in ontologyElement.Attributes().ToList())
                {
                    if (attribute.Name.Namespace == XNamespace.None && IRIAttributeNames.Contains(attribute.Name.LocalName))
                        attribute.Value = ResolveRelativeIRI(attribute.Value, baseUri);
                }

                //IRI-valued element texts are those of IRI and Import elements (which never have child elements)
                if (ontologyElement.Name.Namespace == XNamespace.None
                     && !ontologyElement.HasElements
                     && (ontologyElement.Name.LocalName == "IRI" || ontologyElement.Name.LocalName == "Import"))
                    ontologyElement.Value = ResolveRelativeIRI(ontologyElement.Value, baseUri);
            }
        }

        /// <summary>
        /// Resolves the given IRI against the given base when it is relative (RFC 3986), otherwise returns it as it is
        /// </summary>
        private static string ResolveRelativeIRI(string iri, Uri baseUri)
        {
            //Surrounding whitespace is not part of an IRI written as element text
            string trimmedIRI = iri?.Trim();

            //An empty value has no reference to resolve
            if (string.IsNullOrEmpty(trimmedIRI))
                return iri;

            //An absolute IRI (RFC 3986: it starts with a scheme) is already complete and must not be touched;
            //the scheme test avoids the platform-dependent Uri parsing, which on Unix takes "/x" for an absolute file path
            if (AbsoluteIRIRegex.IsMatch(trimmedIRI))
                return iri;

            //A relative IRI is completed with the base (when it cannot be combined it stays as it is)
            return Uri.TryCreate(baseUri, trimmedIRI, out Uri resolvedUri) ? resolvedUri.ToString() : iri;
        }

        /// <summary>
        /// Replaces every AbbreviatedIRI element having a declared prefix with an IRI element holding the expanded IRI
        /// </summary>
        private static void ExpandAbbreviatedIRIs(XElement ontologyRoot, List<XElement> ontologyElements)
        {
            //The prefixes declared by the document take precedence over the standard ones
            Dictionary<string, string> prefixIRIs = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (XElement prefixElement in ontologyRoot.Elements("Prefix"))
            {
                //A prefix declaration without name or IRI cannot expand anything
                string prefixName = (string)prefixElement.Attribute("name");
                string prefixIRI = (string)prefixElement.Attribute("IRI");
                if (prefixName != null && prefixIRI != null && !prefixIRIs.ContainsKey(prefixName))
                    prefixIRIs.Add(prefixName, prefixIRI);
            }

            //The standard prefixes are always available, as it happens for the ontologies built by OWLSharp
            foreach (KeyValuePair<string, string> standardPrefix in new[]
            {
                new KeyValuePair<string, string>(RDFVocabulary.OWL.PREFIX, RDFVocabulary.OWL.BASE_URI),
                new KeyValuePair<string, string>(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI),
                new KeyValuePair<string, string>(RDFVocabulary.RDF.PREFIX, RDFVocabulary.RDF.BASE_URI),
                new KeyValuePair<string, string>(RDFVocabulary.XSD.PREFIX, RDFVocabulary.XSD.BASE_URI),
                new KeyValuePair<string, string>(RDFVocabulary.XML.PREFIX, RDFVocabulary.XML.BASE_URI)
            })
            {
                //A standard prefix redeclared by the document keeps the document meaning
                if (!prefixIRIs.ContainsKey(standardPrefix.Key))
                    prefixIRIs.Add(standardPrefix.Key, standardPrefix.Value);
            }

            foreach (XElement ontologyElement in ontologyElements)
            {
                //Only the AbbreviatedIRI elements (which never have child elements) are expanded
                if (ontologyElement.Name != XNamespace.None + "AbbreviatedIRI" || ontologyElement.HasElements)
                    continue;

                //An abbreviated IRI is made of a (possibly empty) prefix and a local part separated by the first colon
                string abbreviatedIRI = ontologyElement.Value.Trim();
                int colonIndex = abbreviatedIRI.IndexOf(':');

                //Without colon or with an undeclared prefix the element is left for the model to interpret
                if (colonIndex < 0 || !prefixIRIs.TryGetValue(abbreviatedIRI.Substring(0, colonIndex), out string expandedPrefixIRI))
                    continue;

                //The model already reads a qualified name there when the prefix is an XML namespace declaration: this keeps the existing behavior
                string abbreviatedPrefix = abbreviatedIRI.Substring(0, colonIndex);
                if (abbreviatedPrefix.Length > 0
                     && QualifiedNameParentNames.Contains(ontologyElement.Parent?.Name.LocalName ?? string.Empty)
                     && ontologyElement.GetNamespaceOfPrefix(abbreviatedPrefix) != null)
                    continue;

                //The element becomes a plain IRI element holding the namespace of the prefix followed by the local part
                ontologyElement.Name = XNamespace.None + "IRI";
                ontologyElement.Value = string.Concat(expandedPrefixIRI, abbreviatedIRI.Substring(colonIndex + 1));
            }
        }

        /// <summary>
        /// Serializes the given ontology element into an OWL2/XML string representation
        /// </summary>
        internal static string SerializeObject<T>(T objectToSerialize, XmlSerializerNamespaces xmlSerializerNamespaces=null) where T : class
        {
            //Hide hard-coded .NET prefixes (e.g: xsi)
            (xmlSerializerNamespaces ?? (xmlSerializerNamespaces = new XmlSerializerNamespaces())).Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (UTF8StringWriter utf8StringWriter = new UTF8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(utf8StringWriter, CompactXmlWriterSettings))
                {
                    xmlSerializer.Serialize(xmlWriter, objectToSerialize, xmlSerializerNamespaces);
                    return utf8StringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserializes the given OWL2/XML string into an ontology element representation
        /// </summary>
        internal static T DeserializeObject<T>(string objectToDeserialize) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(objectToDeserialize))
            {
                using (XmlReader reader = XmlReader.Create(stringReader, DefaultXmlReaderSettings))
                {
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
        }
    }

    /// <summary>
    /// UTF8StringWriter is a StringWriter requiring usage of Encoding.UTF8
    /// </summary>
    internal sealed class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}