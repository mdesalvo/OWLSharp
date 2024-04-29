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

        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string SubjectIRI { get; set; }

        //AnnotationValue (cannot be a self-object, since this would introduce an additional XmlElement)

        [XmlElement("IRI", DataType="anyURI", Order=4)]
        public string ValueIRI { get; set; }
        [XmlElement("Literal", Order=5)]
        public OWLLiteral ValueLiteral { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationAssertion() : base() { }
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri) : this() 
        {
            AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"annotationProperty\" parameter is null");
            SubjectIRI = subjectIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"subjectIri\" parameter is null");
        }
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, RDFResource valueIri) : this(annotationProperty, subjectIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueIri\" parameter is null");
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLLiteral valueLiteral) : this(annotationProperty, subjectIri)
            => ValueLiteral = valueLiteral ?? throw new OWLException("Cannot create OWLAnnotationAssertion because given \"valueLiteral\" parameter is null");
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //TODO

            return graph;
        }
        #endregion
    }
}