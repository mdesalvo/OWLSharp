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
    /// OWLFunctionalDataProperty axiom asserts that a datatype property can relate each individual
    /// to at most one literal value, enforcing a uniqueness constraint on property values.
    /// For example, FunctionalDataProperty(hasSSN) states that an individual can have at most one
    /// social security number, allowing reasoners to detect inconsistencies when multiple distinct
    /// values are asserted for the same individual and the same functional property, or to infer
    /// that different literals must be equal.
    /// </summary>
    [XmlRoot("FunctionalDataProperty")]
    public sealed class OWLFunctionalDataProperty : OWLDataPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The data property asserted to have functional behavior (e.g: http://example.org/hasSSN)
        /// </summary>
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLFunctionalDataProperty() { }

        /// <summary>
        /// Builds an OWLFunctionalDataProperty with the given data property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLFunctionalDataProperty(OWLDataProperty dataProperty) : this()
            => DataProperty = dataProperty ?? throw new OWLException($"Cannot create OWLFunctionalDataProperty because given '{nameof(dataProperty)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLFunctionalDataProperty to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = DataProperty.ToRDFGraph();

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(DataProperty.GetIRI(), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}