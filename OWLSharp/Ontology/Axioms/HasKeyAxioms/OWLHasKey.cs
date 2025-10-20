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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLHasKey axiom specifies that a class is uniquely identified by a set of properties (object and/or datatype properties),
    /// meaning that individuals sharing the same values for all key properties must be the same individual.
    /// For example, HasKey(Person hasSSN) states that two Person individuals with identical social security numbers
    /// are actually the same person, allowing reasoners to infer SameIndividual assertions based on matching key property values
    /// and enforce uniqueness constraints similar to database keys.
    /// </summary>
    [XmlRoot("HasKey")]
    public sealed class OWLHasKey : OWLAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the class expression on which the key property is defines (e.g: http://example.org/Person)
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
        /// Represents the set of object property expressions whose target individuals uniquely identify the class (e.g: http://example.org/HasIRI)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=3)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=3)]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }

        /// <summary>
        /// Represents the set of data properties whose target literal values uniquely identify the class (e.g: http://example.org/hasSSN)
        /// </summary>
        [XmlElement(ElementName="DataProperty", Order=4)]
        public List<OWLDataProperty> DataProperties { get; set; }
        #endregion

        #region Ctors
        internal OWLHasKey() { }

        /// <summary>
        /// Builds an OWLHasKey with the given class expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal OWLHasKey(OWLClassExpression classExpression) : this()
            => ClassExpression = classExpression ?? throw new OWLException($"Cannot create OWLHasKey because given '{nameof(classExpression)}' parameter is null");

        /// <summary>
        /// Builds an OWLHasKey with the given class expression and given set of identifying object property expressions
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLHasKey(OWLClassExpression classExpression, List<OWLObjectPropertyExpression> objectPropertyExpressions) : this(classExpression)
        {
            #region Guards
            if (objectPropertyExpressions?.Any(ope => ope == null) ?? false)
                throw new OWLException($"Cannot create OWLHasKey because given '{nameof(objectPropertyExpressions)}' parameter contains a null element");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions ?? new List<OWLObjectPropertyExpression>();
            DataProperties = new List<OWLDataProperty>();
        }

        /// <summary>
        /// Builds an OWLHasKey with the given class expression and given set of identifying data properties
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLHasKey(OWLClassExpression classExpression, List<OWLDataProperty> dataProperties) : this(classExpression)
        {
            #region Guards
            if (dataProperties?.Any(dp => dp == null) ?? false)
                throw new OWLException($"Cannot create OWLHasKey because given '{nameof(dataProperties)}' parameter contains a null element");
            #endregion

            ObjectPropertyExpressions = new List<OWLObjectPropertyExpression>();
            DataProperties = dataProperties ?? new List<OWLDataProperty>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLHasKey to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource clsExprIRI = ClassExpression.GetIRI();
            graph = graph.UnionWith(ClassExpression.ToRDFGraph(clsExprIRI));

            RDFCollection keyPropsCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (OWLObjectPropertyExpression objectPropertyExpression in ObjectPropertyExpressions)
            {
                RDFResource objPropExpressionIRI = objectPropertyExpression.GetIRI();
                keyPropsCollection.AddItem(objPropExpressionIRI);
                graph = graph.UnionWith(objectPropertyExpression.ToRDFGraph(objPropExpressionIRI));
            }
            foreach (OWLDataProperty dataProperty in DataProperties)
            {
                keyPropsCollection.AddItem(dataProperty.GetIRI());
                graph = graph.UnionWith(dataProperty.ToRDFGraph());
            }
            graph.AddCollection(keyPropsCollection);

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(clsExprIRI, RDFVocabulary.OWL.HAS_KEY, keyPropsCollection.ReificationSubject);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}