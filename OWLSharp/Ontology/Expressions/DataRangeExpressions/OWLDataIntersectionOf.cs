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
    /// OWLDataIntersectionOf is a datatype expression that represents the intersection of two or more datatypes,
    /// containing only literals that simultaneously belong to ALL specified datatypes.
    /// For example, DataIntersectionOf(xsd:integer, DataOneOf("1" "2" "3")) would represent integers that are also
    /// in the enumerated set {1, 2, 3}, allowing you to create more restrictive data ranges through combination.
    /// </summary>
    [XmlRoot("DataIntersectionOf")]
    public sealed class OWLDataIntersectionOf : OWLDataRangeExpression
    {
        #region Properties
        /// <summary>
        /// The set of datarange expressions on which this intersection is defined
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
        internal OWLDataIntersectionOf() { }

        /// <summary>
        /// Builds an OWLDataIntersectionOf on the given set of datarange expressions (must be at least 2)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataIntersectionOf(List<OWLDataRangeExpression> datarangeExpressions)
        {
            #region Guards
            if (datarangeExpressions == null)
                throw new OWLException($"Cannot create OWLDataIntersectionOf because given '{nameof(datarangeExpressions)}' parameter is null");
            if (datarangeExpressions.Count < 2)
                throw new OWLException($"Cannot create OWLDataIntersectionOf because given '{nameof(datarangeExpressions)}' parameter must contain at least 2 elements");
            if (datarangeExpressions.Any(dre => dre == null))
                throw new OWLException($"Cannot create OWLDataIntersectionOf because given '{nameof(datarangeExpressions)}' parameter contains a null element");
            #endregion

            DataRangeExpressions = datarangeExpressions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLDataIntersectionOf expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(string.Join(" and ", DataRangeExpressions.Select(drExpr => drExpr.ToSWRLString())));
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLDataIntersectionOf expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFCollection dataIntersectionOfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (OWLDataRangeExpression dataRangeExpression in DataRangeExpressions)
            {
                RDFResource drExpressionIRI = dataRangeExpression.GetIRI();
                dataIntersectionOfCollection.AddItem(drExpressionIRI);
                graph = graph.UnionWith(dataRangeExpression.ToRDFGraph(drExpressionIRI));
            }
            graph.AddCollection(dataIntersectionOfCollection);
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.INTERSECTION_OF, dataIntersectionOfCollection.ReificationSubject));

            return graph;
        }
        #endregion
    }
}