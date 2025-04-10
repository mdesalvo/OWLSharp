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
    [XmlRoot("SubAnnotationPropertyOf")]
    public sealed class OWLSubAnnotationPropertyOf : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationProperty SubAnnotationProperty { get; set; }

        [XmlElement(ElementName="AnnotationProperty", Order=3)]
        public OWLAnnotationProperty SuperAnnotationProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLSubAnnotationPropertyOf()
        { }
        public OWLSubAnnotationPropertyOf(OWLAnnotationProperty subAnnotationProperty, OWLAnnotationProperty superAnnotationProperty) : this()
        {
            SubAnnotationProperty = subAnnotationProperty ?? throw new OWLException("Cannot create OWLSubAnnotationPropertyOf because given \"subAnnotationProperty\" parameter is null");
            SuperAnnotationProperty = superAnnotationProperty ?? throw new OWLException("Cannot create OWLSubAnnotationPropertyOf because given \"superAnnotationProperty\" parameter is null");
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph = graph.UnionWith(SubAnnotationProperty.ToRDFGraph())
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