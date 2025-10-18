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
    /// OWLTransitiveObjectProperty axiom asserts that an object property chains through intermediate individuals,
    /// meaning that if the property relates A to B and B to C, then it also relates A to C.
    /// For example, TransitiveObjectProperty(hasAncestor) states that if John has ancestor Mary and Mary has ancestor Peter,
    /// then John has ancestor Peter, allowing reasoners to infer indirect relationships through chains of direct assertions
    /// and compute transitive closures.
    /// </summary>
    [XmlRoot("TransitiveObjectProperty")]
    public sealed class OWLTransitiveObjectProperty : OWLObjectPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The object property expression asserted to have transitive behavior (e.g: http://example.org/hasAncestor)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLTransitiveObjectProperty() { }

        /// <summary>
        /// Builds an OWLTransitiveObjectProperty with the given object property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLTransitiveObjectProperty(OWLObjectProperty objectProperty) : this()
           => ObjectPropertyExpression = objectProperty ?? throw new OWLException($"Cannot create OWLTransitiveObjectProperty because given '{nameof(objectProperty)}' parameter is null");

        /// <summary>
        /// Builds an OWLTransitiveObjectProperty with the given anonymous inverse property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLTransitiveObjectProperty(OWLObjectInverseOf objectInverseOf) : this()
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException($"Cannot create OWLTransitiveObjectProperty because given '{nameof(objectInverseOf)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLTransitiveObjectProperty to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}