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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp
{
    [XmlRoot("ObjectProperty")]
    public class OWLObjectProperty : OWLObjectPropertyExpression
    {
        #region Properties
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        [XmlAttribute("abbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectProperty() { }
        public OWLObjectProperty(RDFResource iri)
            => IRI = iri?.ToString() ?? throw new OWLException("Cannot create OWLObjectProperty because given \"iri\" parameter is null");
        public OWLObjectProperty(XmlQualifiedName abbreviatedIri)
            => AbbreviatedIRI = abbreviatedIri ?? throw new OWLException("Cannot create OWLObjectProperty because given \"abbreviatedIri\" parameter is null");
        #endregion
    }

    [XmlRoot("ObjectPropertyChain")]
    public class OWLObjectPropertyChain
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty")]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf")]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectPropertyChain() { }
        public OWLObjectPropertyChain(List<OWLObjectPropertyExpression> objectPropertyExpressions)
        {
            #region Guards
            if (objectPropertyExpressions == null)
                throw new OWLException("Cannot create OWLObjectPropertyChain because given \"objectPropertyExpressions\" parameter is null");
            if (objectPropertyExpressions.Count < 2)
                throw new OWLException("Cannot create OWLObjectPropertyChain because given \"objectPropertyExpressions\" parameter must contain at least 2 elements");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions;
        }
        #endregion
    }
}