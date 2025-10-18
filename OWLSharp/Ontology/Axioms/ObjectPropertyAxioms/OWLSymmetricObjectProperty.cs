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
    /// OWLSymmetricObjectProperty axiom asserts that an object property holds equally in both directions,
    /// meaning that if the property relates individual A to B, it also relates B to A.
    /// For example, SymmetricObjectProperty(isSiblingOf) states that if John is a sibling of Mary,
    /// then Mary is also a sibling of John, allowing reasoners to automatically infer bidirectional assertions
    /// and ensure symmetric relationships are consistently represented.
    /// </summary>
    [XmlRoot("SymmetricObjectProperty")]
    public sealed class OWLSymmetricObjectProperty : OWLObjectPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The object property expression asserted to have symmetric behavior (e.g: http://example.org/isSiblingOf)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSymmetricObjectProperty() { }

        /// <summary>
        /// Builds an OWLSymmetricObjectProperty with the given object property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLSymmetricObjectProperty(OWLObjectProperty objectProperty) : this()
            => ObjectPropertyExpression = objectProperty ?? throw new OWLException($"Cannot create OWLSymmetricObjectProperty because given '{nameof(objectProperty)}' parameter is null");

        /// <summary>
        /// Builds an OWLSymmetricObjectProperty with the given anonymous inverse property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLSymmetricObjectProperty(OWLObjectInverseOf objectInverseOf) : this()
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException($"Cannot create OWLSymmetricObjectProperty because given '{nameof(objectInverseOf)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLSymmetricObjectProperty to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}