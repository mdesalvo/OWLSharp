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
    [XmlRoot("AnnotationPropertyDomain")]
    public class OWLAnnotationPropertyDomain : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string IRI { get; set; }

        [XmlElement("AbbreviatedIRI", DataType="QName", Order=4)]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationPropertyDomain() : base() { }
        public OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty, RDFResource iri) : this()
        {
            AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomain because given \"annotationProperty\" parameter is null");
            IRI = iri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomain because given \"iri\" parameter is null");
        }
        public OWLAnnotationPropertyDomain(OWLAnnotationProperty annotationProperty, XmlQualifiedName abbreviatedIRI) : this()
        {
            AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomain because given \"annotationProperty\" parameter is null");
            AbbreviatedIRI = abbreviatedIRI ?? throw new OWLException("Cannot create OWLAnnotationPropertyDomain because given \"abbreviatedIRI\" parameter is null");
        }
        #endregion
    }
}