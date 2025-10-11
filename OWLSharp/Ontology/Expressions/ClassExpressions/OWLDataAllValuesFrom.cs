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
    /// OWLDataAllValuesFrom is a class expression that restricts a class to individuals whose datatype property values ALL belong to a specified data range.
    /// For example, DataAllValuesFrom(hasAge, xsd:integer) defines the class of individuals where every value of the hasAge property (if any exists)
    /// is an integer greater than or equal to 18, representing a universal quantification over data property values.
    /// </summary>
    [XmlRoot("DataAllValuesFrom")]
    public sealed class OWLDataAllValuesFrom : OWLClassExpression
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
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataAllValuesFrom() { }
        public OWLDataAllValuesFrom(OWLDataProperty dataProperty, OWLDataRangeExpression datarangeExpression)
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLDataAllValuesFrom because given \"dataProperty\" parameter is null");
            DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataAllValuesFrom because given \"datarangeExpression\" parameter is null");
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(DataProperty.ToSWRLString());
            sb.Append(" only ");
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
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ALL_VALUES_FROM, drExpressionIRI));
            graph = graph.UnionWith(DataRangeExpression.ToRDFGraph(drExpressionIRI));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_PROPERTY, DataProperty.GetIRI()));
            return graph.UnionWith(DataProperty.ToRDFGraph());
        }
        #endregion
    }
}