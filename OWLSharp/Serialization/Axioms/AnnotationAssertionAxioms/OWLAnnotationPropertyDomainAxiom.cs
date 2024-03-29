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
    public class OWLAnnotationPropertyDomainAxiom : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty", Order=1)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        [XmlElement("IRI", DataType="anyURI", Order=2)]
        public string DomainIRI { get; set; }

        [XmlElement("AbbreviatedIRI", DataType="QName", Order=3)]
        public XmlQualifiedName DomainAbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationPropertyDomainAxiom() : base() { }
        public OWLAnnotationPropertyDomainAxiom(OWLAnnotationProperty annotationProperty, RDFResource domainIri) : this()
        {
            AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomainAxiom because given \"annotationProperty\" parameter is null");
            DomainIRI = domainIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomainAxiom because given \"domainIri\" parameter is null");
        }
        public OWLAnnotationPropertyDomainAxiom(OWLAnnotationProperty annotationProperty, XmlQualifiedName domainAbbreviatedIRI) : this()
        {
            AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomainAxiom because given \"annotationProperty\" parameter is null");
            DomainAbbreviatedIRI = domainAbbreviatedIRI ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomainAxiom because given \"domainAbbreviatedIRI\" parameter is null");
        }
        #endregion
    }
}