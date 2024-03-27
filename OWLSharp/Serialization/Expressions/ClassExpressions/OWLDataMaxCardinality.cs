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

namespace OWLSharp
{
    public class OWLDataMaxCardinality : OWLClassExpression
    {
        #region Properties
        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=1)]
        public OWLDataPropertyExpression DataPropertyExpression { get; set; }

        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=2)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=2)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=2)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }

        [XmlAttribute("cardinality", DataType="nonNegativeInteger")]
        public string Cardinality { get; set; } = "0";
        #endregion

        #region Ctors
        internal OWLDataMaxCardinality() { }
        public OWLDataMaxCardinality(OWLDataPropertyExpression dataPropertyExpression, uint cardinality)
        {
            DataPropertyExpression = dataPropertyExpression ?? throw new OWLException("Cannot create OWLDataMaxCardinality because given \"dataPropertyExpression\" parameter is null");
            Cardinality = cardinality.ToString();
        }
        public OWLDataMaxCardinality(OWLDataPropertyExpression dataPropertyExpression, OWLDataRangeExpression datarangeExpression, uint cardinality) : this(dataPropertyExpression, cardinality)
            => DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataMaxCardinality because given \"datarangeExpression\" parameter is null");
        #endregion
    }
}