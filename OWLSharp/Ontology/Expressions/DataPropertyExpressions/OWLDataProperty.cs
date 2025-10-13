/*
   Copyright 2014-2025 Marco De Salvo

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
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDataProperty represents a binary relation between an individual and a literal value,
    /// connecting instances of classes to concrete data values.
    /// For example, properties like "hasAge", "hasName", or "hasPrice" are datatype properties
    /// that associate individuals with numbers, strings, dates, or other literal values rather than other individuals.
    /// </summary>
    [XmlRoot("DataProperty")]
    public sealed class OWLDataProperty : OWLDataPropertyExpression, IOWLEntity
    {
        #region Properties
        /// <summary>
        /// The IRI of the data property (e.g: http://xmlns.com/foaf/0.1/age)
        /// </summary>
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        /// <summary>
        /// The xsd:qualifiedName representation of the data property (e.g: foaf:age)
        /// </summary>
        [XmlAttribute("abbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLDataProperty() { }

        /// <summary>
        /// Builds a data property with the given IRI (e.g: http://xmlns.com/foaf/0.1/age)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataProperty(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new OWLException($"Cannot create OWLDataProperty because given '{nameof(iri)}' parameter is null");
            if (iri.IsBlank)
                throw new OWLException($"Cannot create OWLDataProperty because given '{nameof(iri)}' parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
            ExpressionIRI = iri;
        }

        /// <summary>
        /// Builds a data property with the given xsd:qualifiedName (e.g: foaf:age)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataProperty(XmlQualifiedName abbreviatedIri)
        {
            AbbreviatedIRI = abbreviatedIri ?? throw new OWLException($"Cannot create OWLDataProperty because given '{nameof(abbreviatedIri)}' parameter is null");
            ExpressionIRI = new RDFResource(string.Concat(abbreviatedIri.Namespace, abbreviatedIri.Name));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the IRI representation of this data property
        /// </summary>
        public override RDFResource GetIRI()
        {
            if (ExpressionIRI?.IsBlank != false)
            {
                string iri = IRI;
                if (string.IsNullOrEmpty(iri))
                    iri = string.Concat(AbbreviatedIRI.Namespace, AbbreviatedIRI.Name);
                ExpressionIRI = new RDFResource(iri);
            }
            return ExpressionIRI;
        }

        /// <summary>
        /// Exports this data property to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();

            graph.AddTriple(new RDFTriple(GetIRI(), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));

            return graph;
        }
        #endregion
    }
}