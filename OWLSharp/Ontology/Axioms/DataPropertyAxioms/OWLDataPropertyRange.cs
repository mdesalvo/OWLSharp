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
    [XmlRoot("DataPropertyRange")]
    public sealed class OWLDataPropertyRange : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=3)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=3)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=3)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=3)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=3)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=3)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataPropertyRange()
        { }
        public OWLDataPropertyRange(OWLDataProperty dataProperty, OWLDataRangeExpression datarangeExpression) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLDataPropertyRange because given \"dataProperty\" parameter is null");
            DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataPropertyRange because given \"datarangeExpression\" parameter is null");
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource drExpressionIRI = DataRangeExpression.GetIRI();
            graph = graph.UnionWith(DataProperty.ToRDFGraph())
                         .UnionWith(DataRangeExpression.ToRDFGraph(drExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(DataProperty.GetIRI(), RDFVocabulary.RDFS.RANGE, drExpressionIRI);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}