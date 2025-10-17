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

using System.Linq;
using RDFSharp.Model;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLClassAssertion axiom states that a specific individual is an instance of a particular class,
    /// asserting membership in the class's extension.
    /// For example, ClassAssertion(Person :John) declares that the individual :John is a member of the Person class,
    /// providing fundamental A-BOX knowledge about individual types that reasoners use to derive further inferences
    /// and check consistency.
    /// </summary>
    [XmlRoot("ClassAssertion")]
    public sealed class OWLClassAssertion : OWLAssertionAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the class expression which the individual is instance of (e.g: http://xmlns.com/foaf/0.1/Person)
        /// </summary>
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=2)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=2)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=2)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=2)]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom", Order=2)]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom", Order=2)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=2)]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf", Order=2)]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality", Order=2)]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality", Order=2)]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality", Order=2)]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom", Order=2)]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom", Order=2)]
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue", Order=2)]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality", Order=2)]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality", Order=2)]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality", Order=2)]
        public OWLClassExpression ClassExpression { get; set; }

        /// <summary>
        /// Represents the individual instance of the class expression (e.g: http://example.org/John)
        /// </summary>
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression IndividualExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLClassAssertion() { }

        /// <summary>
        /// Builds an OWLClassAssertion with the given class expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal OWLClassAssertion(OWLClassExpression classExpression) : this()
            => ClassExpression = classExpression ?? throw new OWLException($"Cannot create OWLClassAssertion because given '{nameof(classExpression)}' parameter is null");

        /// <summary>
        /// Builds an OWLClassAssertion with the given class expression and named individual
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLClassAssertion(OWLClassExpression classExpression, OWLNamedIndividual namedIndividual) : this(classExpression)
            => IndividualExpression = namedIndividual ?? throw new OWLException($"Cannot create OWLClassAssertion because given '{nameof(namedIndividual)}' parameter is null");

        /// <summary>
        /// Builds an OWLClassAssertion with the given class expression and anonymous individual
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLClassAssertion(OWLClassExpression classExpression, OWLAnonymousIndividual anonymousIndividual) : this(classExpression)
            => IndividualExpression = anonymousIndividual ?? throw new OWLException($"Cannot create OWLClassAssertion because given '{nameof(anonymousIndividual)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLClassAssertion to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            //Axiom Triple
            RDFResource clsExpressionIRI = ClassExpression.GetIRI();
            RDFResource idvExpressionIRI = IndividualExpression.GetIRI();
            RDFGraph graph = ClassExpression.ToRDFGraph(clsExpressionIRI)
                                .UnionWith(IndividualExpression.ToRDFGraph(idvExpressionIRI));
            RDFTriple axiomTriple = new RDFTriple(idvExpressionIRI, RDFVocabulary.RDF.TYPE, clsExpressionIRI);
            graph.AddTriple(axiomTriple);

            //Annotations
            return Annotations.Aggregate(graph,
                (current, annotation) => current.UnionWith(annotation.ToRDFGraph(axiomTriple)));
        }
        #endregion
    }
}