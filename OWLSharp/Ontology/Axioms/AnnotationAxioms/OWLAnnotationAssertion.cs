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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("AnnotationAssertion")]
    public class OWLAnnotationAssertion : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(Order=2)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }
        [XmlElement(Order=3)]
        public OWLAnnotationSubject AnnotationSubject { get; set; }
        [XmlElement(Order=4)]
        public OWLAnnotationValue AnnotationValue { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationAssertion() : base() 
        {
            AnnotationSubject = new OWLAnnotationSubject();
            AnnotationValue = new OWLAnnotationValue();
        }
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty) : this()
            => AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"annotationProperty\" parameter is null");
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri) : this(annotationProperty)
            => AnnotationSubject.SubjectIRI = subjectIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"subjectIri\" parameter is null");
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri) : this(annotationProperty)
            => AnnotationSubject.SubjectAbbreviatedIRI = subjectAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"subjectAbbreviatedIri\" parameter is null");
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual) : this(annotationProperty)
            => AnnotationSubject.SubjectAnonymousIndividual = subjectAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"subjectAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, RDFResource valueIri) : this(annotationProperty, subjectIri)
            => AnnotationValue.ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, RDFResource valueIri) : this(annotationProperty, subjectAbbreviatedIri)
            => AnnotationValue.ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, RDFResource valueIri) : this(annotationProperty, subjectAnonymousIndividual)
            => AnnotationValue.ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectIri)
            => AnnotationValue.ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectAbbreviatedIri)
            => AnnotationValue.ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectAnonymousIndividual)
            => AnnotationValue.ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectIri)
            => AnnotationValue.ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectAbbreviatedIri)
            => AnnotationValue.ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectAnonymousIndividual)
            => AnnotationValue.ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLLiteral valueLiteral) : this(annotationProperty, subjectIri)
            => AnnotationValue.ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueLiteral\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, OWLLiteral valueLiteral) : this(annotationProperty, subjectAbbreviatedIri)
            => AnnotationValue.ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueLiteral\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, OWLLiteral valueLiteral) : this(annotationProperty, subjectAnonymousIndividual)
            => AnnotationValue.ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueLiteral\" parameter is null");
        #endregion
    }

    public class OWLAnnotationSubject
    {
        #region Properties
        [XmlElement("IRI", DataType="anyURI")]
        public string SubjectIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName")]
        public XmlQualifiedName SubjectAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual")]
        public OWLAnonymousIndividual SubjectAnonymousIndividual { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationSubject() { }
        #endregion
    }

    public class OWLAnnotationValue
    {
        #region Properties
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
        internal OWLAnnotationValue() { }
        #endregion
    }
}