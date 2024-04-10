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

using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Expressions
{
    [XmlRoot("DataMaxCardinality")]
    public class OWLDataMaxCardinality : OWLClassExpression
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
    }
}