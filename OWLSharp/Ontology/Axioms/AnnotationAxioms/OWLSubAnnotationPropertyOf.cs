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
    /// OWLSubAnnotationPropertyOf axiom declares a hierarchical relationship between two annotation properties,
    /// stating that one annotation property is a specialization of another.
    /// For example, SubAnnotationPropertyOf(skos:prefLabel rdfs:label) indicates that skos:prefLabel is a more specific
    /// kind of rdfs:label, suggesting that whenever skos:prefLabel is used, it implies a label relationship, though
    /// this has no formal semantic impact on reasoning and primarily serves to organize annotation property vocabularies hierarchically.
    /// </summary>
    [XmlRoot("SubAnnotationPropertyOf")]
    public sealed class OWLSubAnnotationPropertyOf : OWLAnnotationAxiom
    {
        #region Properties
        /// <summary>
        /// The annotation property representing the "child" in the hierarchy (e.g: http://www.w3.org/2004/02/skos/core#prefLabel)
        /// </summary>
        [XmlElement(ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationProperty SubAnnotationProperty { get; set; }

        /// <summary>
        /// The annotation property representing the "mother" in the hierarchy (e.g: http://www.w3.org/2000/01/rdf-schema#label)
        /// </summary>
        [XmlElement(ElementName="AnnotationProperty", Order=3)]
        public OWLAnnotationProperty SuperAnnotationProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLSubAnnotationPropertyOf() { }

        /// <summary>
        /// Builds an OWLSubAnnotationPropertyOf with the given child and mother annotation properties
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLSubAnnotationPropertyOf(OWLAnnotationProperty subAnnotationProperty, OWLAnnotationProperty superAnnotationProperty) : this()
        {
            SubAnnotationProperty = subAnnotationProperty ?? throw new OWLException($"Cannot create OWLSubAnnotationPropertyOf because given '{nameof(subAnnotationProperty)}' parameter is null");
            SuperAnnotationProperty = superAnnotationProperty ?? throw new OWLException($"Cannot create OWLSubAnnotationPropertyOf because given '{nameof(superAnnotationProperty)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLSubAnnotationPropertyOf to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = SubAnnotationProperty.ToRDFGraph()
                              .UnionWith(SuperAnnotationProperty.ToRDFGraph());

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(SubAnnotationProperty.GetIRI(), RDFVocabulary.RDFS.SUB_PROPERTY_OF, SuperAnnotationProperty.GetIRI());
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}