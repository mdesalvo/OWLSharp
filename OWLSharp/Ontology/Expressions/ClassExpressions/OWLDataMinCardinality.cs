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
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDataMinCardinality is a class expression that restricts a class to individuals having at least a specified number of values for a given datatype property.
    /// For example, DataMinCardinality(1 hasEmail) defines the class of individuals that have one or more email addresses,
    /// setting a lower bound on the number of data property assertions without limiting the maximum.
    /// </summary>
    [XmlRoot("DataMinCardinality")]
    public sealed class OWLDataMinCardinality : OWLClassExpression
    {
        #region Properties
        /// <summary>
        /// The data property on which this class expression is defined
        /// </summary>
        [XmlElement(ElementName="DataProperty", Order=1)]
        public OWLDataProperty DataProperty { get; set; }

        /// <summary>
        /// The qualifying datarange expression required on the literal values assumed by the data property
        /// </summary>
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=2)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=2)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=2)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=2)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }

        /// <summary>
        /// The minimum cardinality required on the literal values assumed by the data property
        /// </summary>
        [XmlAttribute("cardinality", DataType="nonNegativeInteger")]
        public string Cardinality { get; set; } = "0";
        #endregion

        #region Ctors
        internal OWLDataMinCardinality() { }

        /// <summary>
        /// Builds an OWLDataMinCardinality with the given data property and cardinality
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataMinCardinality(OWLDataProperty dataProperty, uint cardinality)
        {
            DataProperty = dataProperty ?? throw new OWLException($"Cannot create OWLDataMinCardinality because given '{nameof(dataProperty)}' parameter is null");
            Cardinality = cardinality.ToString();
        }

        /// <summary>
        /// Builds an OWLDataMinCardinality with the given data property, cardinality and qualifying datarange expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataMinCardinality(OWLDataProperty dataProperty, uint cardinality, OWLDataRangeExpression datarangeExpression) : this(dataProperty, cardinality)
            => DataRangeExpression = datarangeExpression ?? throw new OWLException($"Cannot create OWLDataMinCardinality because given '{nameof(datarangeExpression)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLDataMinCardinality expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(DataProperty.ToSWRLString());
            sb.Append(" min ");
            sb.Append(Cardinality);
            if (DataRangeExpression != null)
            {
                sb.Append(' ');
                sb.Append(DataRangeExpression.ToSWRLString());
            }
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLDataMinCardinality expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_PROPERTY, DataProperty.GetIRI()));
            graph = graph.UnionWith(DataProperty.ToRDFGraph());
            if (DataRangeExpression == null)
            {
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.MIN_CARDINALITY, new RDFTypedLiteral(Cardinality, RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            }
            else
            {
                RDFResource drExpressionIRI = DataRangeExpression.GetIRI();
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_DATARANGE, drExpressionIRI));
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, new RDFTypedLiteral(Cardinality, RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
                graph = graph.UnionWith(DataRangeExpression.ToRDFGraph(drExpressionIRI));
            }

            return graph;
        }
        #endregion
    }
}