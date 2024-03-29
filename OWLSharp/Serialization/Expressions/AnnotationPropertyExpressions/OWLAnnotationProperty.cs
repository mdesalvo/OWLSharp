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
    public class OWLAnnotationProperty : OWLAnnotationPropertyExpression
    {
        #region Properties
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        [XmlAttribute("abbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationProperty() { }
        public OWLAnnotationProperty(RDFResource iri)
            => IRI = iri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationProperty because given \"iri\" parameter is null");
        public OWLAnnotationProperty(XmlQualifiedName abbreviatedIri)
            => AbbreviatedIRI = abbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationProperty because given \"anonymousIndividual\" parameter is null");
        #endregion
    }

    public class OWLAnnotationSubject
    {
        #region Properties
        [XmlElement("IRI", DataType="anyURI", Order=1)]
        public string IRI { get; set; }

        [XmlElement("AbbreviatedIRI", DataType="QName", Order=2)]
        public XmlQualifiedName AbbreviatedIRI { get; set; }

        [XmlElement("AnonymousIndividual", Order=3)]
        public OWLAnonymousIndividual AnonymousIndividual { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationSubject() { }
        public OWLAnnotationSubject(RDFResource iri)
            => IRI = iri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationSubject because given \"iri\" parameter is null");
        public OWLAnnotationSubject(XmlQualifiedName abbreviatedIri)
            => AbbreviatedIRI = abbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationSubject because given \"abbreviatedIri\" parameter is null");
        public OWLAnnotationSubject(OWLAnonymousIndividual anonymousIndividual)
            => AnonymousIndividual = anonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationSubject because given \"anonymousIndividual\" parameter is null");
        #endregion
    }

    public class OWLAnnotationValue
    {
        #region Properties
        [XmlElement("IRI", DataType="anyURI", Order=1)]
        public string IRI { get; set; }

        [XmlElement("AbbreviatedIRI", DataType="QName", Order=2)]
        public XmlQualifiedName AbbreviatedIRI { get; set; }

        [XmlElement("AnonymousIndividual", Order=3)]
        public OWLAnonymousIndividual AnonymousIndividual { get; set; }

        //Register here all derived types of OWLLiteralExpression
        [XmlElement(typeof(OWLLiteral), ElementName="Literal", Order=4)]
        public OWLLiteralExpression LiteralExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationValue() { }
        public OWLAnnotationValue(RDFResource iri)
            => IRI = iri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationValue because given \"iri\" parameter is null");
        public OWLAnnotationValue(XmlQualifiedName abbreviatedIri)
            => AbbreviatedIRI = abbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotationValue because given \"abbreviatedIri\" parameter is null");
        public OWLAnnotationValue(OWLAnonymousIndividual anonymousIndividual)
            => AnonymousIndividual = anonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotationValue because given \"anonymousIndividual\" parameter is null");
        public OWLAnnotationValue(OWLLiteralExpression literalExpression)
            => LiteralExpression = literalExpression ?? throw new OWLException("Cannot create OWLAnnotationValue because given \"literalExpression\" parameter is null");
        #endregion
    }
}