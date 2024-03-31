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
    /// <summary>
    /// Represents a metadata for documenting any kind of axiom (including the working ontology itself). It is not considered for reasoning purposes.
    /// </summary>
    [XmlRoot("Annotation")]
    public class OWLAnnotation
    {
        #region Properties
        [XmlElement(Order=1)]
        public OWLAnnotation Annotation { get; set; }

        //Register here all derived types of OWLAnnotationPropertyExpression
        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationPropertyExpression AnnotationPropertyExpression { get; set; }

        //AnnotationValue (cannot be a self-object, since this would introduce an additional XmlElement)

        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string ValueIRI { get; set; }
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=4)]
        public XmlQualifiedName ValueAbbreviatedIRI { get; set; }
        [XmlElement("AnonymousIndividual", Order=5)]
        public OWLAnonymousIndividual ValueAnonymousIndividual { get; set; }
        //Register here all derived types of OWLLiteralExpression
        [XmlElement(typeof(OWLLiteral), ElementName="Literal", Order=6)]
        public OWLLiteralExpression ValueLiteralExpression { get; set; }
        #endregion

        #region Internal-Ctors
        internal OWLAnnotation() { }
        internal OWLAnnotation(OWLAnnotationPropertyExpression annotationPropertyExpression) : this()
            => AnnotationPropertyExpression = annotationPropertyExpression ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"annotationPropertyExpression\" parameter is null");
        #endregion

        #region Public-Ctors
        public OWLAnnotation(OWLAnnotationPropertyExpression annotationPropertyExpression, RDFResource valueIri) : this(annotationPropertyExpression)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueIri\" parameter is null");
        public OWLAnnotation(OWLAnnotationPropertyExpression annotationPropertyExpression, XmlQualifiedName valueAbbreviatedIri) : this(annotationPropertyExpression)
            => ValueAbbreviatedIRI = valueAbbreviatedIri ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueAbbreviatedIri\" parameter is null");
        public OWLAnnotation(OWLAnnotationPropertyExpression annotationPropertyExpression, OWLAnonymousIndividual valueAnonymousIndividual) : this(annotationPropertyExpression)
            => ValueAnonymousIndividual = valueAnonymousIndividual ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueAnonymousIndividual\" parameter is null");
        public OWLAnnotation(OWLAnnotationPropertyExpression annotationPropertyExpression, OWLLiteralExpression valueLiteralExpression) : this(annotationPropertyExpression)
            => ValueLiteralExpression = valueLiteralExpression ?? throw new OWLException("Cannot create OWLAnnotation because given \"valueLiteralExpression\" parameter is null");
        #endregion
    }
}