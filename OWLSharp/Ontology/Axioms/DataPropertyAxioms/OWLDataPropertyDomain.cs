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
    /// OWLDataPropertyDomain axiom constrains a datatype property by stating that any individual having a value for that property
    /// must belong to the specified class or class expression. For example, DataPropertyDomain(hasAge Person) asserts that
    /// if an individual has an age value, then that individual must be a Person, allowing reasoners to infer class membership
    /// from property usage and detect inconsistencies when the constraint is violated.
    /// </summary>
    [XmlRoot("DataPropertyDomain")]
    public sealed class OWLDataPropertyDomain : OWLDataPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the property used for this data axiom (e.g: http://example.org/hasAge)
        /// </summary>
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        /// <summary>
        /// Represents the class expression asserted to be domain of the data property (e.g: http://example.org/Person)
        /// </summary>
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=3)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=3)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=3)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=3)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=3)]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom", Order=3)]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom", Order=3)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=3)]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf", Order=3)]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality", Order=3)]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality", Order=3)]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality", Order=3)]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom", Order=3)]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom", Order=3)]
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue", Order=3)]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality", Order=3)]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality", Order=3)]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality", Order=3)]
        public OWLClassExpression ClassExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataPropertyDomain() { }

        /// <summary>
        /// Builds an OWLDataPropertyDomain with the given data property and given class expression as domain
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataPropertyDomain(OWLDataProperty dataProperty, OWLClassExpression classExpression) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException($"Cannot create OWLDataPropertyDomain because given '{nameof(dataProperty)}' parameter is null");
            ClassExpression = classExpression ?? throw new OWLException($"Cannot create OWLDataPropertyDomain because given '{nameof(classExpression)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLDataPropertyDomain to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = DataProperty.ToRDFGraph();

            RDFResource clsExpressionIRI = ClassExpression.GetIRI();
            graph = graph.UnionWith(ClassExpression.ToRDFGraph(clsExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(DataProperty.GetIRI(), RDFVocabulary.RDFS.DOMAIN, clsExpressionIRI);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}