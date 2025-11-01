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
    /// OWLDataPropertyAssertion axiom states that a specific individual is related to a particular literal value
    /// through a datatype property. For example, DataPropertyAssertion(hasAge :John "30"^^xsd:integer) asserts that
    /// the individual :John has an age of 30, providing A-BOX knowledge about concrete data values associated with
    /// individuals that can be used for reasoning and querying.
    /// </summary>
    [XmlRoot("DataPropertyAssertion")]
    public sealed class OWLDataPropertyAssertion : OWLAssertionAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the data property used by this assertion (e.g: http://xmlns.com/foaf/0.1/age)
        /// </summary>
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        /// <summary>
        /// Represents the individual owner of this assertion (e.g: http://example.org/John)
        /// </summary>
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression IndividualExpression { get; set; }

        /// <summary>
        /// Represents the literal value assumed by the data property of this assertion (e.g: 30)
        /// </summary>
        [XmlElement(Order=4)]
        public OWLLiteral Literal { get; set; }
        #endregion

        #region Ctors
        internal OWLDataPropertyAssertion() { }

        /// <summary>
        /// Builds an OWLDataPropertyAssertion with the given data property and literal value
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal OWLDataPropertyAssertion(OWLDataProperty dataProperty, OWLLiteral literal) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException($"Cannot create OWLDataPropertyAssertion because given '{nameof(dataProperty)}' parameter is null");
            Literal = literal ?? throw new OWLException($"Cannot create OWLDataPropertyAssertion because given '{nameof(literal)}' parameter is null");
        }

        /// <summary>
        /// Builds an OWLDataPropertyAssertion with the given data property, named individual and literal value
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataPropertyAssertion(OWLDataProperty dataProperty, OWLNamedIndividual namedIndividual, OWLLiteral literal) : this(dataProperty, literal)
            => IndividualExpression = namedIndividual ?? throw new OWLException($"Cannot create OWLDataPropertyAssertion because given '{nameof(namedIndividual)}' parameter is null");

        /// <summary>
        /// Builds an OWLDataPropertyAssertion with the given data property, anonymous individual and literal value
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataPropertyAssertion(OWLDataProperty dataProperty, OWLAnonymousIndividual anonymousIndividual, OWLLiteral literal) : this(dataProperty, literal)
            => IndividualExpression = anonymousIndividual ?? throw new OWLException($"Cannot create OWLDataPropertyAssertion because given '{nameof(anonymousIndividual)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLDataPropertyAssertion to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = DataProperty.ToRDFGraph();

            RDFResource idvExpressionIRI = IndividualExpression.GetIRI();
            graph = graph.UnionWith(IndividualExpression.ToRDFGraph(idvExpressionIRI));

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