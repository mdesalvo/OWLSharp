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
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("DataMaxCardinality")]
    public sealed class OWLDataMaxCardinality : OWLClassExpression
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=1)]
        public OWLDataProperty DataProperty { get; set; }

        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=2)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=2)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=2)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=2)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }

        [XmlAttribute("cardinality", DataType="nonNegativeInteger")]
        public string Cardinality { get; set; } = "0";
        #endregion

        #region Ctors
        internal OWLDataMaxCardinality() { }
        public OWLDataMaxCardinality(OWLDataProperty dataProperty, uint cardinality)
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLDataMaxCardinality because given \"dataProperty\" parameter is null");
            Cardinality = cardinality.ToString();
        }
        public OWLDataMaxCardinality(OWLDataProperty dataProperty, uint cardinality, OWLDataRangeExpression datarangeExpression) : this(dataProperty, cardinality)
            => DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataMaxCardinality because given \"datarangeExpression\" parameter is null");
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(DataProperty.ToSWRLString());
            sb.Append(" max ");
            sb.Append(Cardinality);
            if (DataRangeExpression != null)
            {
                sb.Append(' ');
                sb.Append(DataRangeExpression.ToSWRLString());
            }
            sb.Append(')');

            return sb.ToString();
        }

        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_PROPERTY, DataProperty.GetIRI()));
            graph = graph.UnionWith(DataProperty.ToRDFGraph());
            if (DataRangeExpression == null)
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.MAX_CARDINALITY, new RDFTypedLiteral(Cardinality, RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            else
            {
                RDFResource drExpressionIRI = DataRangeExpression.GetIRI();
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_DATARANGE, drExpressionIRI));
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, new RDFTypedLiteral(Cardinality, RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
                graph = graph.UnionWith(DataRangeExpression.ToRDFGraph(drExpressionIRI));
            }

            return graph;
        }
        #endregion
    }
}