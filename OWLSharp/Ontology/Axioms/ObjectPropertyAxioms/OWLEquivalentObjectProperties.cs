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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("EquivalentObjectProperties")]
    public sealed class OWLEquivalentObjectProperties : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLEquivalentObjectProperties()
        { }
        public OWLEquivalentObjectProperties(List<OWLObjectPropertyExpression> objectPropertyExpressions) : this()
        {
            #region Guards
            if (objectPropertyExpressions == null)
                throw new OWLException("Cannot create OWLEquivalentObjectProperties because given \"objectPropertyExpressions\" parameter is null");
            if (objectPropertyExpressions.Count < 2)
                throw new OWLException("Cannot create OWLEquivalentObjectProperties because given \"objectPropertyExpressions\" parameter must contain at least 2 elements");
            if (objectPropertyExpressions.Any(ope => ope == null))
                throw new OWLException("Cannot create OWLEquivalentObjectProperties because given \"objectPropertyExpressions\" parameter contains a null element");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            List<RDFResource> objPropIRIs = new List<RDFResource>();
            foreach (OWLObjectPropertyExpression objectPropertyExpression in ObjectPropertyExpressions)
            {
                RDFResource objPropIRI = objectPropertyExpression.GetIRI();
                objPropIRIs.Add(objPropIRI);
                graph = graph.UnionWith(objectPropertyExpression.ToRDFGraph(objPropIRI));
            }

            //Axiom Triple(s)
            List<RDFTriple> axiomTriples = new List<RDFTriple>();
            for (int i = 0; i < ObjectPropertyExpressions.Count - 1; i++)
                for (int j = i + 1; j < ObjectPropertyExpressions.Count; j++)
                {
                    RDFTriple axiomTriple = new RDFTriple(objPropIRIs[i], RDFVocabulary.OWL.EQUIVALENT_PROPERTY, objPropIRIs[j]);
                    axiomTriples.Add(axiomTriple);
                    graph.AddTriple(axiomTriple);
                }

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                foreach (RDFTriple axiomTriple in axiomTriples)
                    graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}