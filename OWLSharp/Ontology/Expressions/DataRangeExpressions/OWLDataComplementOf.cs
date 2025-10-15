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

using System.Text;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDataComplementOf is a datatype expression that represents the complement of a given datatype,
    /// containing all literals that are NOT in the specified datatype.
    /// For example, DataComplementOf(xsd:integer) includes all possible literal values except integers,
    /// allowing you to define data ranges by exclusion rather than inclusion.
    /// </summary>
    [XmlRoot("DataComplementOf")]
    public sealed class OWLDataComplementOf : OWLDataRangeExpression
    {
        #region Properties
        /// <summary>
        /// The datarange expression on which this complement is defined
        /// </summary>
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype")]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf")]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf")]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf")]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf")]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction")]
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataComplementOf() { }

        /// <summary>
        /// Builds an OWLDataComplementOf on the given datarange expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataComplementOf(OWLDataRangeExpression datarangeExpression)
            => DataRangeExpression = datarangeExpression ?? throw new OWLException($"Cannot create OWLDataComplementOf because given '{nameof(datarangeExpression)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLDataComplementOf expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(not");
            sb.Append(DataRangeExpression.ToSWRLString());
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLDataComplementOf expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFResource drExpressionIRI = DataRangeExpression.GetIRI();
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.COMPLEMENT_OF, drExpressionIRI));
            return graph.UnionWith(DataRangeExpression.ToRDFGraph(drExpressionIRI));
        }
        #endregion
    }
}