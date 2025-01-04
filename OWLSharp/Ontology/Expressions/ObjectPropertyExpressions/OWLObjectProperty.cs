/*
   Copyright 2014-2025 Marco De Salvo

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
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("ObjectProperty")]
    public class OWLObjectProperty : OWLObjectPropertyExpression, IOWLEntity
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
        {
            #region Guards
            if (iri == null)
                throw new OWLException("Cannot create OWLObjectProperty because given \"iri\" parameter is null");
            if (iri.IsBlank)
                throw new OWLException("Cannot create OWLObjectProperty because given \"iri\" parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
            ExpressionIRI = iri;
        }
        public OWLObjectProperty(XmlQualifiedName abbreviatedIri)
        {
            AbbreviatedIRI = abbreviatedIri ?? throw new OWLException("Cannot create OWLObjectProperty because given \"abbreviatedIri\" parameter is null");
            ExpressionIRI = new RDFResource(string.Concat(abbreviatedIri.Namespace, abbreviatedIri.Name));
        }
        #endregion

        #region Methods
        public override RDFResource GetIRI()
        {
            if (ExpressionIRI == null || ExpressionIRI.IsBlank)
            {
                string iri = IRI;
                if (string.IsNullOrEmpty(iri))
                    iri = string.Concat(AbbreviatedIRI.Namespace, AbbreviatedIRI.Name);
                ExpressionIRI = new RDFResource(iri);
            }
            return ExpressionIRI;
        }

        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();

            graph.AddTriple(new RDFTriple(GetIRI(), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));

            return graph;
        }
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
            if (objectPropertyExpressions.Any(ope => ope == null))
                throw new OWLException("Cannot create OWLObjectPropertyChain because given \"objectPropertyExpressions\" parameter contains a null element");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions;
        }
        #endregion
    }
}