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
    [XmlRoot("EquivalentClasses")]
    public sealed class OWLEquivalentClasses : OWLClassAxiom
    {
        #region Properties
        //Register here all derived types of OWLClassExpression
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
        public List<OWLClassExpression> ClassExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLEquivalentClasses()
        { }
        public OWLEquivalentClasses(List<OWLClassExpression> classExpressions) : this()
        {
            #region Guards
            if (classExpressions == null)
                throw new OWLException("Cannot create OWLEquivalentClasses because given \"classExpressions\" parameter is null");
            if (classExpressions.Count < 2)
                throw new OWLException("Cannot create OWLEquivalentClasses because given \"classExpressions\" parameter must contain at least 2 elements");
            if (classExpressions.Any(cex => cex == null))
                throw new OWLException("Cannot create OWLEquivalentClasses because given \"classExpressions\" parameter contains a null element");
            #endregion

            ClassExpressions = classExpressions;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            List<RDFResource> clsExpressionIRIs = [];
            foreach (OWLClassExpression classExpression in ClassExpressions)
            {
                RDFResource clsExpressionIRI = classExpression.GetIRI();
                clsExpressionIRIs.Add(clsExpressionIRI);
                graph = graph.UnionWith(classExpression.ToRDFGraph(clsExpressionIRI));
            }

            //Axiom Triple(s)
            List<RDFTriple> axiomTriples = [];
            for (int i = 0; i < ClassExpressions.Count - 1; i++)
                for (int j = i + 1; j < ClassExpressions.Count; j++)
                {
                    RDFTriple axiomTriple = new RDFTriple(clsExpressionIRIs[i], RDFVocabulary.OWL.EQUIVALENT_CLASS, clsExpressionIRIs[j]);
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