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
    [XmlRoot("ObjectPropertyAssertion")]
    public sealed class OWLObjectPropertyAssertion : OWLAssertionAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression SourceIndividualExpression { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=4)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=4)]
        public OWLIndividualExpression TargetIndividualExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectPropertyAssertion()
        { }
        public OWLObjectPropertyAssertion(OWLObjectPropertyExpression objectPropertyExpression, OWLIndividualExpression sourceIndividualExpression, OWLIndividualExpression targetIndividualExpression) : this()
        {
            ObjectPropertyExpression = objectPropertyExpression ?? throw new OWLException("Cannot create OWLObjectPropertyAssertion because given \"objectPropertyExpression\" parameter is null");
            SourceIndividualExpression = sourceIndividualExpression ?? throw new OWLException("Cannot create OWLObjectPropertyAssertion because given \"sourceIndividualExpression\" parameter is null");
            TargetIndividualExpression = targetIndividualExpression ?? throw new OWLException("Cannot create OWLObjectPropertyAssertion because given \"targetIndividualExpression\" parameter is null");
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            RDFResource sidvExpressionIRI = SourceIndividualExpression.GetIRI();
            RDFResource tidvExpressionIRI = TargetIndividualExpression.GetIRI();
            RDFTriple axiomTriple;

            //ObjectInverseOf
            if (ObjectPropertyExpression is OWLObjectInverseOf objectInverseOf)
            {
                RDFResource objectInverseOfIRI = objectInverseOf.ObjectProperty.GetIRI();
                graph = graph.UnionWith(objectInverseOf.ObjectProperty.ToRDFGraph())
                             .UnionWith(SourceIndividualExpression.ToRDFGraph(sidvExpressionIRI))
                             .UnionWith(TargetIndividualExpression.ToRDFGraph(tidvExpressionIRI));

                //Axiom Triple
                axiomTriple = new RDFTriple(tidvExpressionIRI, objectInverseOfIRI, sidvExpressionIRI);
            }

            //ObjectProperty
            else
            {
                RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
                graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI))
                             .UnionWith(SourceIndividualExpression.ToRDFGraph(sidvExpressionIRI))
                             .UnionWith(TargetIndividualExpression.ToRDFGraph(tidvExpressionIRI));

                //Axiom Triple
                axiomTriple = new RDFTriple(sidvExpressionIRI, objPropExpressionIRI, tidvExpressionIRI);
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