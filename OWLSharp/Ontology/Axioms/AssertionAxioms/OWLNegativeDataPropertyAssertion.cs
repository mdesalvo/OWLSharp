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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLNegativeDataPropertyAssertion axiom explicitly states that a specific individual does NOT have a particular
    /// literal value for a given datatype property. For example, NegativeDataPropertyAssertion(hasAge :John "25"^^xsd:integer)
    /// asserts that :John's age is not 25, allowing expression of negative knowledge and enabling reasoners to detect
    /// inconsistencies when other axioms would entail the negated relationship.
    /// </summary>
    [XmlRoot("NegativeDataPropertyAssertion")]
    public sealed class OWLNegativeDataPropertyAssertion : OWLAssertionAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the data property used by this negative assertion (e.g: http://xmlns.com/foaf/0.1/age)
        /// </summary>
        [XmlElement(ElementName="DataProperty", Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        /// <summary>
        /// Represents the individual owner of this negative assertion (e.g: http://example.org/John)
        /// </summary>
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression IndividualExpression { get; set; }

        /// <summary>
        /// Represents the literal value assumed by the negative data property of this assertion (e.g: 25)
        /// </summary>
        [XmlElement(Order=4)]
        public OWLLiteral Literal { get; set; }
        #endregion

        #region Ctors
        internal OWLNegativeDataPropertyAssertion() { }

        /// <summary>
        /// Builds an OWLNegativeDataPropertyAssertion with the given data property and literal value
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal OWLNegativeDataPropertyAssertion(OWLDataProperty dataProperty, OWLLiteral literal) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException($"Cannot create OWLNegativeDataPropertyAssertion because given '{nameof(dataProperty)}' parameter is null");
            Literal = literal ?? throw new OWLException($"Cannot create OWLNegativeDataPropertyAssertion because given '{nameof(literal)}' parameter is null");
        }

        /// <summary>
        /// Builds an OWLNegativeDataPropertyAssertion with the given data property, named individual and literal value
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLNegativeDataPropertyAssertion(OWLDataProperty dataProperty, OWLNamedIndividual namedIndividual, OWLLiteral literal) : this(dataProperty, literal)
            => IndividualExpression = namedIndividual ?? throw new OWLException($"Cannot create OWLNegativeDataPropertyAssertion because given '{nameof(namedIndividual)}' parameter is null");

        /// <summary>
        /// Builds an OWLNegativeDataPropertyAssertion with the given data property, anonymous individual and literal value
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLNegativeDataPropertyAssertion(OWLDataProperty dataProperty, OWLAnonymousIndividual anonymousIndividual, OWLLiteral literal) : this(dataProperty, literal)
            => IndividualExpression = anonymousIndividual ?? throw new OWLException($"Cannot create OWLNegativeDataPropertyAssertion because given '{nameof(anonymousIndividual)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLNegativeDataPropertyAssertion to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = DataProperty.ToRDFGraph();

            RDFResource idvExpressionIRI = IndividualExpression.GetIRI();
            RDFResource negativeDataPropertyAssertionIRI = new RDFResource();
            graph.AddTriple(new RDFTriple(negativeDataPropertyAssertionIRI, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, idvExpressionIRI));
            graph.AddTriple(new RDFTriple(negativeDataPropertyAssertionIRI, RDFVocabulary.OWL.ASSERTION_PROPERTY, DataProperty.GetIRI()));
            graph.AddTriple(new RDFTriple(negativeDataPropertyAssertionIRI, RDFVocabulary.OWL.TARGET_VALUE, Literal.GetLiteral()));
            graph = graph.UnionWith(IndividualExpression.ToRDFGraph(idvExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(negativeDataPropertyAssertionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraphInternal(negativeDataPropertyAssertionIRI));

            return graph;
        }
        #endregion
    }
}