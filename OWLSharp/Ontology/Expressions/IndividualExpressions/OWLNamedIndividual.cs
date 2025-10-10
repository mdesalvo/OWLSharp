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
    /// OWLNamedIndividual is an entity suitable for modeling individuals of the A-BOX
    /// </summary>
    [XmlRoot("NamedIndividual")]
    public sealed class OWLNamedIndividual : OWLIndividualExpression, IOWLEntity
    {
        #region Properties
        /// <summary>
        /// The IRI of the individual (e.g: http://example.org/idvs/Bob)
        /// </summary>
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        /// <summary>
        /// The xsd:qualifiedName representation of the individual (e.g: ex:Bob)
        /// </summary>
        [XmlAttribute("abbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLNamedIndividual() { }

        /// <summary>
        /// Builds an individual with the given IRI (e.g: http://example.org/idvs/Bob)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLNamedIndividual(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new OWLException($"Cannot create OWLNamedIndividual because given '{nameof(iri)}' parameter is null");
            if (iri.IsBlank)
                throw new OWLException($"Cannot create OWLNamedIndividual because given '{nameof(iri)}' parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
            ExpressionIRI = iri;
        }

        /// <summary>
        /// Builds an individual with the given xsd:qualifiedName (e.g: ex:Bob)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLNamedIndividual(XmlQualifiedName abbreviatedIri)
        {
            AbbreviatedIRI = abbreviatedIri ?? throw new OWLException($"Cannot create OWLNamedIndividual because given '{nameof(abbreviatedIri)}' parameter is null");
            ExpressionIRI = new RDFResource(string.Concat(abbreviatedIri.Namespace, abbreviatedIri.Name));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the IRI representation of this individual
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
        /// Exports this individual to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();

            graph.AddTriple(new RDFTriple(GetIRI(), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL));

            return graph;
        }
        #endregion
    }
}