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
    [XmlRoot("Annotation")]
    public sealed class OWLAnnotation
    {
        #region Properties
        [XmlElement]
        public OWLAnnotation Annotation { get; set; }

        [XmlElement]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        //AnnotationValue (cannot be a self-object, since this would introduce an additional XmlElement)

        [XmlElement("IRI", DataType="anyURI")]
        public string ValueIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName")]
        public XmlQualifiedName ValueAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual")]
        public OWLAnonymousIndividual ValueAnonymousIndividual { get; set; }
        [XmlElement("Literal")]
        public OWLLiteral ValueLiteral { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotation() { }
        internal OWLAnnotation(OWLAnnotationProperty annotationProperty) : this()
            => AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"annotationProperty\" parameter is null");
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, RDFResource valueIri) : this(annotationProperty)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueIri\" parameter is null");
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotation(OWLAnnotationProperty annotationProperty, OWLLiteral valueLiteral) : this(annotationProperty)
            => ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueLiteral\" parameter is null");
        #endregion

        #region Methods
        public void Annotate(OWLAnnotation annotation)
            => Annotation = annotation ?? throw new OWLException("Cannot annotate annotation because given \"annotation\" parameter is null");

        internal RDFGraph ToRDFGraph(RDFTriple axiomTriple)
        {
            RDFGraph graph = new RDFGraph();

            //Axiom Reification
            RDFResource axiomIRI = new RDFResource();
            graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM));
            graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_SOURCE, (RDFResource)axiomTriple.Subject));
            graph.AddTriple(new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_PROPERTY, (RDFResource)axiomTriple.Predicate));
            graph.AddTriple(axiomTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL
                ? new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_TARGET, (RDFLiteral)axiomTriple.Object)
                : new RDFTriple(axiomIRI, RDFVocabulary.OWL.ANNOTATED_TARGET, (RDFResource)axiomTriple.Object));

            //Axiom Annotation
            return graph.UnionWith(ToRDFGraphInternal(axiomIRI));
        }
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