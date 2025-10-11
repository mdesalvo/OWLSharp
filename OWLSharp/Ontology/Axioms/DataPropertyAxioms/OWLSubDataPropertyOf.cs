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
    /// OWLSubDataPropertyOf axiom asserts that one datatype property is a specialization of another, meaning that
    /// whenever the subproperty relates an individual to a literal, the superproperty also relates them.
    /// For example, SubDataPropertyOf(hasBirthDate hasDate) states that every birth date relationship implies
    /// a more general date relationship, allowing reasoners to propagate property assertions up the property hierarchy
    /// and infer additional datatype property connections.
    /// </summary>
    [XmlRoot("SubDataPropertyOf")]
    public sealed class OWLSubDataPropertyOf : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=2)]
        public OWLDataProperty SubDataProperty { get; set; }

        [XmlElement(ElementName="DataProperty", Order=3)]
        public OWLDataProperty SuperDataProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLSubDataPropertyOf()
        { }
        public OWLSubDataPropertyOf(OWLDataProperty subDataProperty, OWLDataProperty superDataProperty) : this()
        {
            SubDataProperty = subDataProperty ?? throw new OWLException("Cannot create OWLSubDataPropertyOf because given \"subDataProperty\" parameter is null");
            SuperDataProperty = superDataProperty ?? throw new OWLException("Cannot create OWLSubDataPropertyOf because given \"superDataProperty\" parameter is null");
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph = graph.UnionWith(SubDataProperty.ToRDFGraph())
                         .UnionWith(SuperDataProperty.ToRDFGraph());

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(SubDataProperty.GetIRI(), RDFVocabulary.RDFS.SUB_PROPERTY_OF, SuperDataProperty.GetIRI());
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}