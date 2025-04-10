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
    [XmlRoot("SubObjectPropertyOf")]
    public sealed class OWLSubObjectPropertyOf : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression SubObjectPropertyExpression { get; set; }

        [XmlElement("ObjectPropertyChain", Order=3)]
        public OWLObjectPropertyChain SubObjectPropertyChain { get; set; }

        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=4)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=4)]
        public OWLObjectPropertyExpression SuperObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSubObjectPropertyOf()
        { }
        public OWLSubObjectPropertyOf(OWLObjectProperty subObjectProperty, OWLObjectProperty superObjectProperty) : this()
        {
            SubObjectPropertyExpression = subObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectProperty\" parameter is null");
            SuperObjectPropertyExpression = superObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectProperty\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectProperty subObjectProperty, OWLObjectInverseOf superObjectInverseOf) : this()
        {
            SubObjectPropertyExpression = subObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectProperty\" parameter is null");
            SuperObjectPropertyExpression = superObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectInverseOf\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectInverseOf subObjectInverseOf, OWLObjectProperty superObjectProperty) : this()
        {
            SubObjectPropertyExpression = subObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectInverseOf\" parameter is null");
            SuperObjectPropertyExpression = superObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectProperty\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectInverseOf subObjectInverseOf, OWLObjectInverseOf superObjectInverseOf) : this()
        {
            SubObjectPropertyExpression = subObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectInverseOf\" parameter is null");
            SuperObjectPropertyExpression = superObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectInverseOf\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectPropertyChain subObjectPropertyChain, OWLObjectProperty superObjectProperty) : this()
        {
            SubObjectPropertyChain = subObjectPropertyChain ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectPropertyChain\" parameter is null");
            SuperObjectPropertyExpression = superObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectProperty\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectPropertyChain subObjectPropertyChain, OWLObjectInverseOf superObjectInverseOf) : this()
        {
            SubObjectPropertyChain = subObjectPropertyChain ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectPropertyChain\" parameter is null");
            SuperObjectPropertyExpression = superObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectInverseOf\" parameter is null");
        }
        #endregion

        #region Methods
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