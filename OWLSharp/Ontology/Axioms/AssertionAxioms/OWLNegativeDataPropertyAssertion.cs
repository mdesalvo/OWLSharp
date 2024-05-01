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
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("NegativeDataPropertyAssertion")]
    public class OWLNegativeDataPropertyAssertion : OWLAssertionAxiom
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression IndividualExpression { get; set; }

        [XmlElement(Order=4)]
        public OWLLiteral Literal { get; set; }
        #endregion

        #region Ctors
        internal OWLNegativeDataPropertyAssertion() : base() { }
		internal OWLNegativeDataPropertyAssertion(OWLDataProperty dataProperty, OWLLiteral literal) : this() 
		{ 
			DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertion because given \"dataProperty\" parameter is null");
            Literal = literal ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertion because given \"literal\" parameter is null");
		}
        public OWLNegativeDataPropertyAssertion(OWLDataProperty dataProperty, OWLNamedIndividual namedIndividual, OWLLiteral literal) : this(dataProperty, literal)
        	=> IndividualExpression = namedIndividual ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertion because given \"namedIndividual\" parameter is null");
		 public OWLNegativeDataPropertyAssertion(OWLDataProperty dataProperty, OWLAnonymousIndividual anonymousIndividual, OWLLiteral literal) : this(dataProperty, literal)
        	=> IndividualExpression = anonymousIndividual ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertion because given \"anonymousIndividual\" parameter is null");
        #endregion

        #region Methods
        internal override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            RDFResource idvExpressionIRI = IndividualExpression.GetIRI();
			RDFResource negativeObjectPropertyAssertionIRI = new RDFResource();
			graph.AddTriple(new RDFTriple(negativeObjectPropertyAssertionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION));
			graph.AddTriple(new RDFTriple(negativeObjectPropertyAssertionIRI, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, idvExpressionIRI));
			graph.AddTriple(new RDFTriple(negativeObjectPropertyAssertionIRI, RDFVocabulary.OWL.ASSERTION_PROPERTY, DataProperty.GetIRI()));
			graph.AddTriple(new RDFTriple(negativeObjectPropertyAssertionIRI, RDFVocabulary.OWL.TARGET_VALUE, Literal.GetLiteral()));
			graph = graph.UnionWith(DataProperty.ToRDFGraph())
						 .UnionWith(IndividualExpression.ToRDFGraph(idvExpressionIRI));
	
            return graph;
        }
        #endregion
    }
}