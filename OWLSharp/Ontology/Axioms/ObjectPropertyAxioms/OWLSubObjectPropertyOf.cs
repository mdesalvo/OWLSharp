/*
   Copyright 2014-2026 Marco De Salvo

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
using System.Linq;
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
        /// Gets the contribution of this OWLSubObjectPropertyOf to the OWL2/Manchester rendering of the ontology
        /// (property chains go under the SubPropertyChain: section of the named super property frame;
        /// null if the frame-owning property expression is anonymous, since Manchester frames require named entities)
        /// </summary>
        internal override OWLManchesterFrameItem ToManchesterFrameItem(OWLManchesterContext manchesterContext)
        {
            if (SubObjectPropertyChain != null)
            {
                OWLObjectProperty superObjProp = OWLManchesterContext.GetNamedObjectProperty(SuperObjectPropertyExpression);
                //Without a named super property there is no frame to host the chain
                if (superObjProp == null)
                    return null;

                //Chain composed into "inverse P" holds exactly when the reversed chain of inverses is composed into P
                //(the inverse of "p1 o p2" is "inverse p2 o inverse p1"), so the chain is flipped and put on the frame of P
                string chainText = SuperObjectPropertyExpression is OWLObjectInverseOf
                    ? string.Join(" o ", SubObjectPropertyChain.ObjectPropertyExpressions.AsEnumerable().Reverse().Select(opex => manchesterContext.RenderInverse(opex)))
                    : string.Join(" o ", SubObjectPropertyChain.ObjectPropertyExpressions.Select(opex => opex.ToManchesterString(manchesterContext)));
                return new OWLManchesterFrameItem {
                    FrameKind = OWLManchesterFrameKind.ObjectProperty,
                    EntityName = superObjProp.ToManchesterString(manchesterContext),
                    SectionKeyword = "SubPropertyChain:",
                    ItemText = $"{manchesterContext.RenderAxiomAnnotations(Annotations)}{chainText}" };
            }

            OWLObjectProperty subObjProp = OWLManchesterContext.GetNamedObjectProperty(SubObjectPropertyExpression);
            //Without a named sub property there is no frame to host the axiom
            if (subObjProp == null)
                return null;

            //"inverse P" below Q holds exactly when P is below "inverse Q": the frame of P takes the inverted super property
            string superText = SubObjectPropertyExpression is OWLObjectInverseOf
                ? manchesterContext.RenderInverse(SuperObjectPropertyExpression)
                : SuperObjectPropertyExpression.ToManchesterString(manchesterContext);
            return new OWLManchesterFrameItem {
                FrameKind = OWLManchesterFrameKind.ObjectProperty,
                EntityName = subObjProp.ToManchesterString(manchesterContext),
                SectionKeyword = "SubPropertyOf:",
                ItemText = $"{manchesterContext.RenderAxiomAnnotations(Annotations)}{superText}" };
        }

        /// <summary>
        /// Gets the OWL2/Functional-Style representation of this SubObjectPropertyOf axiom
        /// (this axiom has two syntactic forms: a plain subproperty/superproperty pair, or a property chain
        /// as the "sub" side - only one of SubObjectPropertyExpression/SubObjectPropertyChain is ever set)
        /// </summary>
        internal override string ToFunctionalString(OWLFunctionalContext functionalContext)
        {
            if (SubObjectPropertyChain != null)
            {
                string objectPropertyChainArguments = string.Join(" ", SubObjectPropertyChain.ObjectPropertyExpressions.Select(opex => opex.ToFunctionalString(functionalContext)));
                return $"SubObjectPropertyOf( {functionalContext.RenderAxiomAnnotations(Annotations)}ObjectPropertyChain( {objectPropertyChainArguments} ) {SuperObjectPropertyExpression.ToFunctionalString(functionalContext)} )";
            }

            return $"SubObjectPropertyOf( {functionalContext.RenderAxiomAnnotations(Annotations)}{SubObjectPropertyExpression.ToFunctionalString(functionalContext)} {SuperObjectPropertyExpression.ToFunctionalString(functionalContext)} )";
        }

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