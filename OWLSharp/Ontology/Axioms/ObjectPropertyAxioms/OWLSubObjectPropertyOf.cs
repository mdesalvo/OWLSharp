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
    /// OWLSubObjectPropertyOf axiom asserts that one object property is a specialization of another,
    /// meaning that whenever the subproperty relates two individuals, the superproperty also relates them.
    /// For example, SubObjectPropertyOf(hasMother hasParent) states that every mother relationship implies
    /// a more general parent relationship, allowing reasoners to propagate property assertions up the property
    /// hierarchy and infer additional object property connections between individuals.
    /// </summary>
    [XmlRoot("SubObjectPropertyOf")]
    public sealed class OWLSubObjectPropertyOf : OWLObjectPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The object property expression representing the "child" in the hierarchy (e.g: http://example.org/hasMother)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression SubObjectPropertyExpression { get; set; }

        /// <summary>
        /// The chain of object property expressions representing the "child" in the hierarchy (e.g: http://example.org/hasSister, http://example.org/hasMother)
        /// </summary>
        [XmlElement("ObjectPropertyChain", Order=3)]
        public OWLObjectPropertyChain SubObjectPropertyChain { get; set; }

        /// <summary>
        /// The object property expression representing the "mother" in the hierarchy (e.g: http://example.org/hasParent)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=4)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=4)]
        public OWLObjectPropertyExpression SuperObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSubObjectPropertyOf() { }

        /// <summary>
        /// Builds an OWLSubObjectPropertyOf with the given child and mother object property expressions
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLSubObjectPropertyOf(OWLObjectPropertyExpression subObjectPropertyExpression, OWLObjectPropertyExpression superObjectPropertyExpression) : this()
        {
            SubObjectPropertyExpression = subObjectPropertyExpression ?? throw new OWLException($"Cannot create OWLSubObjectPropertyOf because given '{nameof(subObjectPropertyExpression)}' parameter is null");
            SuperObjectPropertyExpression = superObjectPropertyExpression ?? throw new OWLException($"Cannot create OWLSubObjectPropertyOf because given '{nameof(superObjectPropertyExpression)}' parameter is null");
        }

        /// <summary>
        /// Builds an OWLSubObjectPropertyOf with the given chain of object properties as child and mother object property expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLSubObjectPropertyOf(OWLObjectPropertyChain subObjectPropertyChain, OWLObjectPropertyExpression superObjectPropertyExpression) : this()
        {
            SubObjectPropertyChain = subObjectPropertyChain ?? throw new OWLException($"Cannot create OWLSubObjectPropertyOf because given '{nameof(subObjectPropertyChain)}' parameter is null");
            SuperObjectPropertyExpression = superObjectPropertyExpression ?? throw new OWLException($"Cannot create OWLSubObjectPropertyOf because given '{nameof(superObjectPropertyExpression)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLSubObjectPropertyOf to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFTriple axiomTriple;
            RDFGraph graph = new RDFGraph();
            RDFResource superObjPropExpressionIRI = SuperObjectPropertyExpression.GetIRI();
            graph = graph.UnionWith(SuperObjectPropertyExpression.ToRDFGraph(superObjPropExpressionIRI));

            //ObjectProperty/ObjectInverseOf
            if (SubObjectPropertyChain == null)
            {
                RDFResource subObjPropExpressionIRI = SubObjectPropertyExpression.GetIRI();
                graph = graph.UnionWith(SubObjectPropertyExpression.ToRDFGraph(subObjPropExpressionIRI));

                //Axiom Triple
                axiomTriple = new RDFTriple(subObjPropExpressionIRI, RDFVocabulary.RDFS.SUB_PROPERTY_OF, superObjPropExpressionIRI);
            }

            //ObjectPropertyChain
            else
            {
                RDFCollection chainObjectPropertyExpressions = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource, true);
                foreach (OWLObjectPropertyExpression chainObjectPropertyExpression in SubObjectPropertyChain.ObjectPropertyExpressions)
                {
                    RDFResource chainObjectPropertyExpressionIRI = chainObjectPropertyExpression.GetIRI();
                    chainObjectPropertyExpressions.AddItem(chainObjectPropertyExpressionIRI);
                    graph = graph.UnionWith(chainObjectPropertyExpression.ToRDFGraph(chainObjectPropertyExpressionIRI));
                }
                graph.AddCollection(chainObjectPropertyExpressions);

                //Axiom Triple
                axiomTriple = new RDFTriple(superObjPropExpressionIRI, RDFVocabulary.OWL.PROPERTY_CHAIN_AXIOM, chainObjectPropertyExpressions.ReificationSubject);
            }
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}