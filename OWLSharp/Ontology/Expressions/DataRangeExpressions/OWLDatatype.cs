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
    /// OWLDatatype represents a set of data values (literals) with a specific structure and interpretation, identified by an IRI,
    /// typically from XML Schema (like xsd:string, xsd:integer, xsd:dateTime).
    /// Datatypes define the range of datatype properties and can be built-in (predefined) or custom-defined,
    /// providing the foundation for representing concrete data values in the ontology.
    /// </summary>
    [XmlRoot("Datatype")]
    public sealed class OWLDatatype : OWLDataRangeExpression, IOWLEntity
    {
        #region Properties
        /// <summary>
        /// The IRI of the datatype (e.g: http://www.w3.org/2001/XMLSchema#integer)
        /// </summary>
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        /// <summary>
        /// The xsd:qualifiedName representation of the datatype (e.g: xsd:integer)
        /// </summary>
        [XmlAttribute("abbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLDatatype() { }

        /// <summary>
        /// Builds a datatype with the given IRI (e.g: http://www.w3.org/2001/XMLSchema#integer)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDatatype(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new OWLException($"Cannot create OWLDatatype because given '{nameof(iri)}' parameter is null");
            if (iri.IsBlank)
                throw new OWLException($"Cannot create OWLDatatype because given '{nameof(iri)}' parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
            ExpressionIRI = iri;
        }

        /// <summary>
        /// Builds a datatype with the given xsd:qualifiedName (e.g: xsd:integer)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDatatype(XmlQualifiedName abbreviatedIri)
        {
            AbbreviatedIRI = abbreviatedIri ?? throw new OWLException($"Cannot create OWLDatatype because given '{nameof(abbreviatedIri)}' parameter is null");
            ExpressionIRI = new RDFResource(string.Concat(abbreviatedIri.Namespace, abbreviatedIri.Name));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the IRI representation of this datatype
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
        /// Exports this datatype to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();

            graph.AddTriple(new RDFTriple(GetIRI(), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE));

            return graph;
        }
        #endregion
    }
}