﻿/*
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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("Annotation")]
    public class OWLAnnotation
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
    }
}