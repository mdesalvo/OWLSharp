/*
   Copyright 2014-2024 Marco De Salvo

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
    [XmlRoot("DataPropertyDomain")]
    public class OWLDataPropertyDomain : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        //Register here all derived types of OWLClassExpression
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
        internal OWLDataPropertyDomain() : base() { }
        public OWLDataPropertyDomain(OWLDataProperty dataProperty, OWLClassExpression classExpression) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLDataPropertyDomain because given \"dataProperty\" parameter is null");
            ClassExpression = classExpression ?? throw new OWLException("Cannot create OWLDataPropertyDomain because given \"classExpression\" parameter is null");
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource clsExpressionIRI = ClassExpression.GetIRI();
            graph = graph.UnionWith(DataProperty.ToRDFGraph())
                         .UnionWith(ClassExpression.ToRDFGraph(clsExpressionIRI));

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