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

namespace OWLSharp
{
    public class OWLAnnotationAssertionAxiom : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(ElementName="AnnotationProperty", Order=1)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        //SUBJECT

        [XmlElement("IRI", DataType="anyURI", Order=2)]
        public string SubjectIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=3)]
        public XmlQualifiedName SubjectAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual", Order=4)]
        public OWLAnonymousIndividual SubjectAnonymousIndividual { get; set; }

        //VALUE

        [XmlElement("IRI", DataType="anyURI", Order=5)]
        public string ValueIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=6)]
        public XmlQualifiedName ValueAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual", Order=7)]
        public OWLAnonymousIndividual ValueAnonymousIndividual { get; set; }
        //Register here all derived types of OWLLiteralExpression
        [XmlElement(typeof(OWLLiteral), ElementName="Literal", Order=8)]
        public OWLLiteralExpression ValueLiteralExpression { get; set; }
        #endregion

        #region Internal-Ctors
        internal OWLAnnotationAssertionAxiom() : base() { }
        internal OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty) : this()
            => AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"annotationProperty\" parameter is null");
        internal OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, RDFResource subjectIri) : this(annotationProperty)
            => SubjectIRI = subjectIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"subjectIri\" parameter is null");
        internal OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri) : this(annotationProperty)
            => SubjectAbbreviatedIRI = subjectAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"subjectAbbreviatedIri\" parameter is null");
        internal OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual) : this(annotationProperty)
            => SubjectAnonymousIndividual = subjectAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"subjectAnonymousIndividual\" parameter is null");
        #endregion

        #region Public-Ctors
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, RDFResource valueIri) : this(annotationProperty, subjectIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, RDFResource valueIri) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, RDFResource valueIri) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectIri)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, XmlQualifiedName valueAbbreviatedIri) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectIri)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLLiteralExpression valueLiteralExpression) : this(annotationProperty, subjectIri)
            => ValueLiteralExpression = valueLiteralExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueLiteralExpression\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, XmlQualifiedName subjectAbbreviatedIri, OWLLiteralExpression valueLiteralExpression) : this(annotationProperty, subjectAbbreviatedIri)
            => ValueLiteralExpression = valueLiteralExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueLiteralExpression\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, OWLAnonymousIndividual subjectAnonymousIndividual, OWLLiteralExpression valueLiteralExpression) : this(annotationProperty, subjectAnonymousIndividual)
            => ValueLiteralExpression = valueLiteralExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueLiteralExpression\" parameter is null");
        #endregion
    }
}