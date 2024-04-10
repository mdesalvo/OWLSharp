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
    [XmlRoot("AnnotationAssertion")]
    public class OWLAnnotationAssertion : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        //AnnotationSubject (cannot be a self-object, since this would introduce an additional XmlElement)

        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string SubjectIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=4)]
        public XmlQualifiedName SubjectAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual", Order=5)]
        public OWLAnonymousIndividual SubjectAnonymousIndividual { get; set; }

        //AnnotationValue (cannot be a self-object, since this would introduce an additional XmlElement)

        [XmlElement("IRI", DataType="anyURI", Order=6)]
        public string ValueIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=7)]
        public XmlQualifiedName ValueAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual", Order=8)]
        public OWLAnonymousIndividual ValueAnonymousIndividual { get; set; }
        [XmlElement(ElementName="Literal", Order=9)]
        public OWLLiteral ValueLiteral { get; set; }
        #endregion

        #region Internal-Ctors
        internal OWLAnnotationAssertion() : base() { }
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty) : this()
            => AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"annotationProperty\" parameter is null");
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri) : this(annotationProperty)
            => SubjectIRI = subjectIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"subjectIri\" parameter is null");
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri) : this(annotationProperty)
            => SubjectAbbreviatedIRI = subjectAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"subjectAbbreviatedIri\" parameter is null");
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual) : this(annotationProperty)
            => SubjectAnonymousIndividual = subjectAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"subjectAnonymousIndividual\" parameter is null");
        #endregion

        #region Public-Ctors
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, RDFResource valueIri) : this(annotationProperty, subjectIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, RDFResource valueIri) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, RDFResource valueIri) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueIri\" parameter is null");

        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectIri)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAbbreviatedIri\" parameter is null");

        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectIri)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAnonymousIndividual\" parameter is null");

        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLLiteral valueLiteral) : this(annotationProperty, subjectIri)
            => ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueLiteral\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, OWLLiteral valueLiteral) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueLiteral\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, OWLLiteral valueLiteral) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueLiteral\" parameter is null");
        #endregion
    }
}