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

        [XmlAttribute("AbbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        public OWLAnnotationProperty() { }
        public OWLAnnotationProperty(RDFResource annotationPropertyUri)
            => IRI = annotationPropertyUri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationProperty because given \"annotationPropertyUri\" parameter is null");
        public OWLAnnotationProperty(string xsdQName)
        {
            try
            {
                RDFTypedLiteral xsdQNameLiteral = new RDFTypedLiteral(xsdQName, RDFModelEnums.RDFDatatypes.XSD_QNAME);
                AbbreviatedIRI = new XmlQualifiedName(xsdQNameLiteral.Value);
            }
            catch { throw new OWLException("Cannot create OWLAnnotationProperty because given \"xsdQName\" parameter is not a valid xsd:QName"); }
        }
        #endregion
    }
}