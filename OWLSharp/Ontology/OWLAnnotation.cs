/*
   Copyright 2014-2024 Marco De Salvo

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
    /// OWLAnnotation represents an annotation which can be attached to any kind of ontology axiom
    /// </summary>
    [XmlRoot("Annotation")]
    public class OWLAnnotation
    {
        #region Properties
        /// <summary>
        /// Nested sub-annotation which may be attached to this annotation
        /// </summary>
        [XmlElement]
        public OWLAnnotation Annotation { get; set; }

        /// <summary>
        /// OWL2 annotation property of this annotation
        /// </summary>
        [XmlElement]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        //AnnotationValue (cannot be a self-object, since this would introduce an additional XmlElement)

        /// <summary>
        /// Value of the annotation to be used when it expresses a RDF resource
        /// </summary>
        [XmlElement("IRI", DataType="anyURI")]
        public string ValueIRI { get; set; }

        /// <summary>
        /// Value of the annotation to be used when it expresses a prefix-abbreviated RDF resource
        /// </summary>
        [XmlElement("AbbreviatedIRI", DataType="QName")]
        public XmlQualifiedName ValueAbbreviatedIRI { get; set; }

        /// <summary>
        /// Value of the annotation to be used when it expresses an anonymous OWL2 individual
        /// </summary>
        [XmlElement("AnonymousIndividual")]
        public OWLAnonymousIndividual ValueAnonymousIndividual { get; set; }

        /// <summary>
        /// Value of the annotation to be used when it expresses a RDF literal
        /// </summary>
        [XmlElement("Literal")]
        public OWLLiteral ValueLiteral { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an empty annotation (for internal serialization purposes)
        /// </summary>
        internal OWLAnnotation() { }

        /// <summary>
        /// Builds an annotation from the given annotation property (for internal serialization purposes)
        /// </summary>
        /// <exception cref="OWLException">Thrown when the given annotation property is null</exception>
        internal OWLAnnotation(OWLAnnotationProperty annotationProperty) : this()
            => AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"annotationProperty\" parameter is null");

        /// <summary>
        /// Builds an annotation from the given annotation property and the given RDF resource
        /// </summary>
        /// <exception cref="OWLException">Thrown when the given RDF resource is null</exception>
        /// <exception cref="OWLException">Thrown when the given annotation property is null</exception>
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, RDFResource valueIri) : this(annotationProperty)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueIri\" parameter is null");

        /// <summary>
        /// Builds an annotation from the given annotation property and the given prefix-abbreviated RDF resource
        /// </summary>
        /// <exception cref="OWLException">Thrown when the given prefix-abbreviated RDF resource is null</exception>
        /// <exception cref="OWLException">Thrown when the given annotation property is null</exception>
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueAbbreviatedIri\" parameter is null");

        /// <summary>
        /// Builds an annotation from the given annotation property and the given OWL2 anonymous individual
        /// </summary>
        /// <exception cref="OWLException">Thrown when the given OWL2 anonymous individual is null</exception>
        /// <exception cref="OWLException">Thrown when the given annotation property is null</exception>
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueAnonymousIndividual\" parameter is null");

        /// <summary>
        /// Builds an annotation from the given annotation property and the given RDF literal
        /// </summary>
        /// <exception cref="OWLException">Thrown when the given RDF literal is null</exception>
        /// <exception cref="OWLException">Thrown when the given annotation property is null</exception>
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, OWLLiteral valueLiteral) : this(annotationProperty)
            => ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueLiteral\" parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Attaches the given sub-annotation to this annotation
        /// </summary>
        /// <exception cref="OWLException">Thrown when the given sub-annotation is null</exception>
        public void Annotate(OWLAnnotation annotation)
            => Annotation = annotation ?? throw new OWLException("Cannot annotate annotation because given \"annotation\" parameter is null");

        /// <summary>
        /// Reifies this annotation into the corresponding set of axiom-based RDF triples (for internal purposes)
        /// </summary>
        internal RDFGraph ToRDFGraph(RDFTriple axiomTriple)
        {
            RDFGraph graph = new RDFGraph();

            //Axiom Reification
            RDFResource axiomIRI = new RDFResource();
            graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM));
            graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_SOURCE, (RDFResource)axiomTriple.Subject));
            graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_PROPERTY, (RDFResource)axiomTriple.Predicate));
            if (axiomTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)
                graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_TARGET, (RDFLiteral)axiomTriple.Object));
            else
                graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_TARGET, (RDFResource)axiomTriple.Object));

            //Axiom Annotation
            graph = graph.UnionWith(ToRDFGraphInternal(axiomIRI));

            return graph;
        }
        /// <summary>
        /// Reifies the nested sub-annotation of this annotation into the corresponding set of axiom-based RDF triples (for internal purposes)
        /// </summary>
        internal RDFGraph ToRDFGraphInternal(RDFResource axiomIRI)
        {
            RDFGraph graph = new RDFGraph();
            graph = graph.UnionWith(AnnotationProperty.ToRDFGraph());

            //Axiom Annotation
            RDFTriple annotationTriple;
            if (!string.IsNullOrEmpty(ValueIRI))
                annotationTriple = new RDFTriple(axiomIRI, AnnotationProperty.GetIRI(), new RDFResource(ValueIRI));
            else if (ValueAbbreviatedIRI != null)
                annotationTriple = new RDFTriple(axiomIRI, AnnotationProperty.GetIRI(), new RDFResource(string.Concat(ValueAbbreviatedIRI.Namespace, ValueAbbreviatedIRI.Name)));
            else if (ValueAnonymousIndividual != null)
                annotationTriple = new RDFTriple(axiomIRI, AnnotationProperty.GetIRI(), ValueAnonymousIndividual.GetIRI());
            else
                annotationTriple = new RDFTriple(axiomIRI, AnnotationProperty.GetIRI(), ValueLiteral.GetLiteral());
            graph.AddTriple(annotationTriple);

            //SubAnnotation
            if (Annotation != null)
                graph = graph.UnionWith(Annotation.ToRDFGraph(annotationTriple));

            return graph;
        }
        #endregion
    }
}