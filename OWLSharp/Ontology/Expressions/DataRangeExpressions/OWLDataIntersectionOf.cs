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
using System.Xml;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    [XmlRoot("DataIntersectionOf")]
    public class OWLDataIntersectionOf : OWLDataRangeExpression
    {
        #region Properties
        //Register here all derived types of OWLDataRangeExpression
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
        public OWLDataIntersectionOf(List<OWLDataRangeExpression> datarangeExpressions)
        {
            #region Guards
            if (datarangeExpressions == null)
                throw new OWLException("Cannot create OWLDataIntersectionOf because given \"datarangeExpressions\" parameter is null");
            if (datarangeExpressions.Count < 2)
                throw new OWLException("Cannot create OWLDataIntersectionOf because given \"datarangeExpressions\" parameter must contain at least 2 elements");
            if (datarangeExpressions.Any(dre => dre == null))
                throw new OWLException("Cannot create OWLDataIntersectionOf because given \"datarangeExpressions\" parameter contains a null element");
            #endregion

            DataRangeExpressions = datarangeExpressions;
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(string.Join(" and ", DataRangeExpressions.Select(drExpr => drExpr.ToSWRLString())));
            sb.Append(')');

            return sb.ToString();
        }

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