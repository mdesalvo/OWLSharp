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
    /// OWLInverseFunctionalObjectProperty axiom asserts that an object property's inverse can relate
    /// each individual to at most one other individual, meaning that each value can have at most one
    /// subject through that property. For example, InverseFunctionalObjectProperty(hasSSN) states that
    /// at most one person can have any particular SSN value, allowing reasoners to infer that two
    /// individuals sharing the same SSN value must be the same individual (SameIndividual),
    /// effectively establishing a key-like constraint.
    /// </summary>
    [XmlRoot("InverseFunctionalObjectProperty")]
    public sealed class OWLInverseFunctionalObjectProperty : OWLObjectPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The object property expression asserted to have inverse functional behavior (e.g: http://example.org/hasSSN)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLInverseFunctionalObjectProperty() { }
 
        /// <summary>
        /// Builds an OWLInverseFunctionalObjectProperty with the given object property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLInverseFunctionalObjectProperty(OWLObjectProperty objectProperty) : this()
            => ObjectPropertyExpression = objectProperty ?? throw new OWLException($"Cannot create OWLInverseFunctionalObjectProperty because given '{nameof(objectProperty)}' parameter is null");

        /// <summary>
        /// Builds an OWLInverseFunctionalObjectProperty with the given anonymous inverse property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLInverseFunctionalObjectProperty(OWLObjectInverseOf objectInverseOf) : this()
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException($"Cannot create OWLInverseFunctionalObjectProperty because given '{nameof(objectInverseOf)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLInverseFunctionalObjectProperty to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}