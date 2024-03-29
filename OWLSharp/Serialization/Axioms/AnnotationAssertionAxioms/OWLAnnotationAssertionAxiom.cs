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
        //Register here all derived types of OWLAnnotationPropertyExpression
        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty", Order=1)]
        public OWLAnnotationPropertyExpression AnnotationPropertyExpression { get; set; }

        //AnnotationSubject (cannot be a self-object, since this would introduce an additional XmlElement)

        [XmlElement("IRI", DataType="anyURI", Order=2)]
        public string SubjectIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=3)]
        public XmlQualifiedName SubjectAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual", Order=4)]
        public OWLAnonymousIndividual SubjectAnonymousIndividual { get; set; }

        //AnnotationValue (cannot be a self-object, since this would introduce an additional XmlElement)

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
        internal OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression) : this()
            => AnnotationPropertyExpression = annotationPropertyExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"annotationPropertyExpression\" parameter is null");
        internal OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, RDFResource subjectIri) : this(annotationPropertyExpression)
            => SubjectIRI = subjectIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"subjectIri\" parameter is null");
        internal OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, XmlQualifiedName subjectAbbreviatedIri) : this(annotationPropertyExpression)
            => SubjectAbbreviatedIRI = subjectAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"subjectAbbreviatedIri\" parameter is null");
        internal OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, OWLAnonymousIndividual subjectAnonymousIndividual) : this(annotationPropertyExpression)
            => SubjectAnonymousIndividual = subjectAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"subjectAnonymousIndividual\" parameter is null");
        #endregion

        #region Public-Ctors
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, RDFResource subjectIri, RDFResource valueIri) : this(annotationPropertyExpression, subjectIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, XmlQualifiedName subjectAbbreviatedIri, RDFResource valueIri) : this(annotationPropertyExpression, subjectAbbreviatedIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, OWLAnonymousIndividual subjectAnonymousIndividual, RDFResource valueIri) : this(annotationPropertyExpression, subjectAnonymousIndividual)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, RDFResource subjectIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationPropertyExpression, subjectIri)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, XmlQualifiedName subjectAbbreviatedIri, XmlQualifiedName valueAbbreviatedIri) : this(annotationPropertyExpression, subjectAbbreviatedIri)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, OWLAnonymousIndividual subjectAnonymousIndividual, XmlQualifiedName valueAbbreviatedIri) : this(annotationPropertyExpression, subjectAnonymousIndividual)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, RDFResource subjectIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationPropertyExpression, subjectIri)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, XmlQualifiedName subjectAbbreviatedIri, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationPropertyExpression, subjectAbbreviatedIri)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, OWLAnonymousIndividual subjectAnonymousIndividual, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationPropertyExpression, subjectAnonymousIndividual)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, RDFResource subjectIri, OWLLiteralExpression valueLiteralExpression) : this(annotationPropertyExpression, subjectIri)
            => ValueLiteralExpression = valueLiteralExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueLiteralExpression\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, XmlQualifiedName subjectAbbreviatedIri, OWLLiteralExpression valueLiteralExpression) : this(annotationPropertyExpression, subjectAbbreviatedIri)
            => ValueLiteralExpression = valueLiteralExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueLiteralExpression\" parameter is null");
        public OWLAnnotationAssertionAxiom(OWLAnnotationPropertyExpression annotationPropertyExpression, OWLAnonymousIndividual subjectAnonymousIndividual, OWLLiteralExpression valueLiteralExpression) : this(annotationPropertyExpression, subjectAnonymousIndividual)
            => ValueLiteralExpression = valueLiteralExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"valueLiteralExpression\" parameter is null");
        #endregion
    }
}