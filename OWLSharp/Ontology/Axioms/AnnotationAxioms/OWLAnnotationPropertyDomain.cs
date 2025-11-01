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
    /// <summary>
    /// OWLAnnotationPropertyDomain axiom suggests the intended type of subjects for an annotation property,
    /// indicating which entities or element types the annotation property is typically applied to.
    /// For example, AnnotationPropertyDomain(dc:creator owl:Ontology) suggests that the dc:creator annotation property
    /// is intended for use on ontology elements, though this axiom has no formal semantic constraints
    /// and serves purely as documentation to guide ontology developers in proper annotation usage.
    /// </summary>
    [XmlRoot("AnnotationPropertyDomain")]
    public sealed class OWLAnnotationPropertyDomain : OWLAnnotationAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the property used for this annotation axiom (e.g: http://purl.org/dc/elements/1.1/creator)
        /// </summary>
        [XmlElement(Order=2)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        /// <summary>
        /// Represents the IRI of the class expression being domain of the annotation property (e.g: http://www.w3.org/2002/07/owl#Ontology)
        /// </summary>
        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string IRI { get; set; }

        /// <summary>
        /// Represents the xsd:qualifiedName of the class expression being domain of the annotation property (e.g: owl:Ontology)
        /// </summary>
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=4)]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationPropertyDomain() { }

        /// <summary>
        /// Builds an OWLAnnotationPropertyDomain with the given annotation property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty) : this()
            => AnnotationProperty = annotationProperty ?? throw new OWLException($"Cannot create OWLAnnotationPropertyDomain because given '{nameof(annotationProperty)}' parameter is null");

        /// <summary>
        /// Builds an OWLAnnotationPropertyDomain with the given annotation property and class/datarange expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty, RDFResource iri) : this(annotationProperty)
            => IRI = iri?.ToString() ?? throw new OWLException($"Cannot create OWLAnnotationPropertyDomain because given '{nameof(iri)}' parameter is null");

        /// <summary>
        /// Builds an OWLAnnotationPropertyDomain with the given annotation property and class/datarange expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty, XmlQualifiedName abbreviatedIRI) : this(annotationProperty)
            => AbbreviatedIRI = abbreviatedIRI ?? throw new OWLException($"Cannot create OWLAnnotationPropertyDomain because given '{nameof(abbreviatedIRI)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLAnnotationPropertyDomain to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = AnnotationProperty.ToRDFGraph();

            //Axiom Triple
            string domainIRI = IRI;
            if (string.IsNullOrEmpty(domainIRI))
                domainIRI = string.Concat(AbbreviatedIRI.Namespace, AbbreviatedIRI.Name);
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