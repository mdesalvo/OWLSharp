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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLAsymmetricObjectProperty axiom asserts that an object property cannot hold in both directions
    /// between the same pair of individuals, meaning if the property relates individual A to B,
    /// it cannot also relate B to A. For example, AsymmetricObjectProperty(isChildOf) states that if
    /// John is a child of Mary, then Mary cannot be a child of John, allowing reasoners to detect
    /// inconsistencies when bidirectional assertions would violate asymmetry.
    /// </summary>
    [XmlRoot("AsymmetricObjectProperty")]
    public sealed class OWLAsymmetricObjectProperty : OWLObjectPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The object property expression asserted to have asymmetric behavior (e.g: http://example.org/isChildOf)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLAsymmetricObjectProperty() { }

        /// <summary>
        /// Builds an OWLAsymmetricObjectProperty with the given object property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAsymmetricObjectProperty(OWLObjectProperty objectProperty) : this()
            => ObjectPropertyExpression = objectProperty ?? throw new OWLException($"Cannot create OWLAsymmetricObjectProperty because given '{nameof(objectProperty)}' parameter is null");

        /// <summary>
        /// Builds an OWLAsymmetricObjectProperty with the given anonymous inverse property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAsymmetricObjectProperty(OWLObjectInverseOf objectInverseOf) : this()
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException($"Cannot create OWLAsymmetricObjectProperty because given '{nameof(objectInverseOf)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLAsymmetricObjectProperty to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}