﻿/*
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

namespace OWLSharp.Ontology.Expressions
{
    [XmlRoot("DataHasValue")]
    public class OWLDataHasValue : OWLClassExpression
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=1)]
        public OWLDataProperty DataProperty { get; set; }

        [XmlElement(ElementName="Literal", Order=2)]
        public OWLLiteral Literal { get; set; }
        #endregion

        #region Ctors
        internal OWLDataHasValue() { }
        public OWLDataHasValue(OWLDataProperty dataProperty, OWLLiteral literal)
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLDataHasValue because given \"dataProperty\" parameter is null");
            Literal = literal ?? throw new OWLException("Cannot create OWLDataHasValue because given \"literal\" parameter is null");
        }
        #endregion

        #region Methods
        internal override RDFGraph GetGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_PROPERTY, DataProperty.GetIRI()));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.HAS_VALUE, Literal.GetLiteral()));

            return graph;
        }
        #endregion
    }
}