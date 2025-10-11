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
    /// OWLDataSomeValuesFrom is a class expression that restricts a class to individuals having at least one value for a specified datatype property that belongs to a given data range.
    /// For example, DataSomeValuesFrom(hasAge, xsd:integer) defines the class of individuals that have at least one hasAge value that is an integer greater than or equal to 18,
    /// representing an existential quantification over data property values.
    /// </summary>
    [XmlRoot("DataSomeValuesFrom")]
    public sealed class OWLDataSomeValuesFrom : OWLClassExpression
    {
        #region Properties
        [XmlElement(Order=1)]
        public OWLDataProperty DataProperty { get; set; }

        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=2)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=2)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=2)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=2)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataSomeValuesFrom() { }
        public OWLDataSomeValuesFrom(OWLDataProperty dataProperty, OWLDataRangeExpression datarangeExpression)
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLDataSomeValuesFrom because given \"dataProperty\" parameter is null");
            DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataSomeValuesFrom because given \"datarangeExpression\" parameter is null");
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(DataProperty.ToSWRLString());
            sb.Append(" some ");
            sb.Append(DataRangeExpression.ToSWRLString());
            sb.Append(')');

            return sb.ToString();
        }

        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFResource drExpressionIRI = DataRangeExpression.GetIRI();
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.SOME_VALUES_FROM, drExpressionIRI));
            graph = graph.UnionWith(DataRangeExpression.ToRDFGraph(drExpressionIRI));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_PROPERTY, DataProperty.GetIRI()));
            return graph.UnionWith(DataProperty.ToRDFGraph());
        }
        #endregion
    }
}