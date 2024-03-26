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
    public class OWLDataMinCardinality : OWLClassExpression
    {
        #region Properties
        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=1)]
        public OWLDataPropertyExpression DataPropertyExpression { get; set; }

        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }

        [XmlAttribute("cardinality", DataType="nonNegativeInteger")]
        public string Cardinality { get; set; } = "0";
        #endregion

        #region Ctors
        internal OWLDataMinCardinality() { }
        public OWLDataMinCardinality(OWLDataPropertyExpression dataPropertyExpression, uint cardinality)
        {
            DataPropertyExpression = dataPropertyExpression ?? throw new OWLException("Cannot create OWLDataMinCardinality because given \"dataPropertyExpression\" parameter is null");
            Cardinality = cardinality.ToString();
        }
        public OWLDataMinCardinality(OWLDataPropertyExpression dataPropertyExpression, OWLDataRangeExpression datarangeExpression, uint cardinality) : this(dataPropertyExpression, cardinality)
            => DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataMinCardinality because given \"datarangeExpression\" parameter is null");
        #endregion
    }
}