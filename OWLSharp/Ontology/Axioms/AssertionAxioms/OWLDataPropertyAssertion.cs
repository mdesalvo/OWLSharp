﻿/*
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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("DataPropertyAssertion")]
    public sealed class OWLDataPropertyAssertion : OWLAssertionAxiom
    {
        #region Properties
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression IndividualExpression { get; set; }

        [XmlElement(Order=4)]
        public OWLLiteral Literal { get; set; }
        #endregion

        #region Ctors
        internal OWLDataPropertyAssertion()
        { }
        internal OWLDataPropertyAssertion(OWLDataProperty dataProperty, OWLLiteral literal) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLDataPropertyAssertion because given \"dataProperty\" parameter is null");
            Literal = literal ?? throw new OWLException("Cannot create OWLDataPropertyAssertion because given \"literal\" parameter is null");
        }
        public OWLDataPropertyAssertion(OWLDataProperty dataProperty, OWLNamedIndividual namedIndividual, OWLLiteral literal) : this(dataProperty, literal)
            => IndividualExpression = namedIndividual ?? throw new OWLException("Cannot create OWLDataPropertyAssertion because given \"namedIndividual\" parameter is null");
        public OWLDataPropertyAssertion(OWLDataProperty dataProperty, OWLAnonymousIndividual anonymousIndividual, OWLLiteral literal) : this(dataProperty, literal)
            => IndividualExpression = anonymousIndividual ?? throw new OWLException("Cannot create OWLDataPropertyAssertion because given \"anonymousIndividual\" parameter is null");
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            RDFResource idvExpressionIRI = IndividualExpression.GetIRI();
            graph = graph.UnionWith(DataProperty.ToRDFGraph())
                         .UnionWith(IndividualExpression.ToRDFGraph(idvExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(idvExpressionIRI, DataProperty.GetIRI(), Literal.GetLiteral());
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}