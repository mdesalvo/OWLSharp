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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDataUnionOf is a datatype expression that represents the union of two or more datatypes,
    /// containing all literals that belong to AT LEAST ONE of the specified datatypes.
    /// For example, DataUnionOf(xsd:integer, xsd:string) includes all integers and all strings,
    /// allowing you to create more permissive data ranges by combining multiple datatypes.
    /// </summary>
    [XmlRoot("DataUnionOf")]
    public sealed class OWLDataUnionOf : OWLDataRangeExpression
    {
        #region Properties
        /// <summary>
        /// The set of datarange expressions on which this union is defined
        /// </summary>
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype")]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf")]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf")]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf")]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf")]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction")]
        public List<OWLDataRangeExpression> DataRangeExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDataUnionOf() { }

        /// <summary>
        /// Builds an OWLDataUnionOf on the given set of datarange expressions (must be at least 2)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataUnionOf(List<OWLDataRangeExpression> datarangeExpressions)
        {
            #region Guards
            if (datarangeExpressions == null)
                throw new OWLException($"Cannot create OWLDataUnionOf because given '{nameof(datarangeExpressions)}' parameter is null");
            if (datarangeExpressions.Count < 2)
                throw new OWLException($"Cannot create OWLDataUnionOf because given '{nameof(datarangeExpressions)}' parameter must contain at least 2 elements");
            if (datarangeExpressions.Any(dre => dre == null))
                throw new OWLException($"Cannot create OWLDataUnionOf because given '{nameof(datarangeExpressions)}' parameter contains a null element");
            #endregion

            DataRangeExpressions = datarangeExpressions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLDataUnionOf expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(string.Join(" or ", DataRangeExpressions.Select(drExpr => drExpr.ToSWRLString())));
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLDataUnionOf expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFCollection dataUnionOfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (OWLDataRangeExpression dataRangeExpression in DataRangeExpressions)
            {
                RDFResource drExpressionIRI = dataRangeExpression.GetIRI();
                dataUnionOfCollection.AddItem(drExpressionIRI);
                graph = graph.UnionWith(dataRangeExpression.ToRDFGraph(drExpressionIRI));
            }
            graph.AddCollection(dataUnionOfCollection);
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.UNION_OF, dataUnionOfCollection.ReificationSubject));

            return graph;
        }
        #endregion
    }
}