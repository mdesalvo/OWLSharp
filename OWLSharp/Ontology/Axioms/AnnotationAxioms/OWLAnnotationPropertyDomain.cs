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
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("AnnotationPropertyDomain")]
    public class OWLAnnotationPropertyDomain : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(Order=2)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string IRI { get; set; }

        [XmlElement("AbbreviatedIRI", DataType="QName", Order=4)]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationPropertyDomain()
        { }
        internal OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty) : this() 
            => AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomain because given \"annotationProperty\" parameter is null");
        public OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty, RDFResource iri) : this(annotationProperty)
            => IRI = iri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomain because given \"iri\" parameter is null");
        public OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty, XmlQualifiedName abbreviatedIRI) : this(annotationProperty)
            => AbbreviatedIRI = abbreviatedIRI ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomain because given \"abbreviatedIRI\" parameter is null");
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            string domainIRI = IRI;
            if (string.IsNullOrEmpty(domainIRI))
                domainIRI = string.Concat(AbbreviatedIRI.Namespace, AbbreviatedIRI.Name);
            graph = graph.UnionWith(AnnotationProperty.ToRDFGraph());

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(AnnotationProperty.GetIRI(), RDFVocabulary.RDFS.DOMAIN, new RDFResource(domainIRI));
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}