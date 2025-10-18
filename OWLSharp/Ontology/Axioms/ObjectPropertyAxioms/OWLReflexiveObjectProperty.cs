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
    /// OWLReflexiveObjectProperty axiom asserts that an object property must relate every individual in its domain to itself,
    /// enforcing self-referential relationships. For example, ReflexiveObjectProperty(knows) states that every individual
    /// knows themselves, allowing reasoners to automatically infer self-loops for all relevant individuals and ensuring
    /// the property always holds reflexively within its domain.
    /// </summary>
    [XmlRoot("ReflexiveObjectProperty")]
    public sealed class OWLReflexiveObjectProperty : OWLObjectPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The object property expression asserted to have reflexive behavior (e.g: http://example.org/knows)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLReflexiveObjectProperty() { }

        /// <summary>
        /// Builds an OWLReflexiveObjectProperty with the given object property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLReflexiveObjectProperty(OWLObjectProperty objectProperty) : this()
            => ObjectPropertyExpression = objectProperty ?? throw new OWLException($"Cannot create OWLReflexiveObjectProperty because given '{nameof(objectProperty)}' parameter is null");

        /// <summary>
        /// Builds an OWLReflexiveObjectProperty with the given anonymous inverse property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLReflexiveObjectProperty(OWLObjectInverseOf objectInverseOf) : this()
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException($"Cannot create OWLReflexiveObjectProperty because given '{nameof(objectInverseOf)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLReflexiveObjectProperty to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}