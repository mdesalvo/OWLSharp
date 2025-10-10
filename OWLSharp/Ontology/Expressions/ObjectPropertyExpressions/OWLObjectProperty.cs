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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLObjectProperty is an expression suitable for modeling properties connecting individuals of the A-BOX
    /// </summary>
    [XmlRoot("ObjectProperty")]
    public sealed class OWLObjectProperty : OWLObjectPropertyExpression, IOWLEntity
    {
        #region Properties
        /// <summary>
        /// The IRI of the object property (e.g: http://xmlns.com/foaf/0.1/knows)
        /// </summary>
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        /// <summary>
        /// The xsd:qualifiedName representation of the object property (e.g: foaf:knows)
        /// </summary>
        [XmlAttribute("abbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectProperty() { }

        /// <summary>
        /// Builds an OWLObjectProperty with the given IRI (e.g: http://xmlns.com/foaf/0.1/knows)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLObjectProperty(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new OWLException($"Cannot create OWLObjectProperty because given '{nameof(iri)}' parameter is null");
            if (iri.IsBlank)
                throw new OWLException($"Cannot create OWLObjectProperty because given '{nameof(iri)}' parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
            ExpressionIRI = iri;
        }
        
        /// <summary>
        /// Builds an OWLObjectProperty with the given xsd:qualifiedName (e.g: foaf:knows)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLObjectProperty(XmlQualifiedName abbreviatedIri)
        {
            AbbreviatedIRI = abbreviatedIri ?? throw new OWLException($"Cannot create OWLObjectProperty because given '{nameof(abbreviatedIri)}' parameter is null");
            ExpressionIRI = new RDFResource(string.Concat(abbreviatedIri.Namespace, abbreviatedIri.Name));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the IRI representation of this object property
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
        /// Exports this object property to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();

            graph.AddTriple(new RDFTriple(GetIRI(), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));

            return graph;
        }
        #endregion
    }

    /// <summary>
    /// OWLObjectPropertyChain is an expression suitable for modeling chains of object property expressions relating individuals of the A-BOX.
    /// It is found in SubObjectPropertyOf axioms as child expression: SubObjectPropertyOf(ObjectPropertyChain(Father,Brother), Uncle)
    /// </summary>
    [XmlRoot("ObjectPropertyChain")]
    public class OWLObjectPropertyChain
    {
        #region Properties
        /// <summary>
        /// The chain of object property expressions
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty")]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf")]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectPropertyChain() { }

        /// <summary>
        /// Builds an OWLObjectPropertyChain with the given chain of object property expressions (must be at least 2)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLObjectPropertyChain(List<OWLObjectPropertyExpression> objectPropertyExpressions)
        {
            #region Guards
            if (objectPropertyExpressions == null)
                throw new OWLException($"Cannot create OWLObjectPropertyChain because given '{nameof(objectPropertyExpressions)}' parameter is null");
            if (objectPropertyExpressions.Count < 2)
                throw new OWLException($"Cannot create OWLObjectPropertyChain because given '{nameof(objectPropertyExpressions)}' parameter must contain at least 2 elements");
            if (objectPropertyExpressions.Any(ope => ope == null))
                throw new OWLException($"Cannot create OWLObjectPropertyChain because given '{nameof(objectPropertyExpressions)}' parameter contains a null element");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions;
        }
        #endregion
    }
}