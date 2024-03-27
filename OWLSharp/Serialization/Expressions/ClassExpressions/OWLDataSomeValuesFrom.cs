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

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLDataSomeValuesFrom : OWLClassExpression
    {
        #region Properties
        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=1)]
        public List<OWLDataPropertyExpression> DataPropertyExpressions { get; set; }

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
        public OWLDataSomeValuesFrom(List<OWLDataPropertyExpression> dataPropertyExpressions, OWLDataRangeExpression datarangeExpression)
        {
            #region Guards
            if (dataPropertyExpressions == null)
                throw new OWLException("Cannot create OWLDataSomeValuesFrom because given \"dataPropertyExpressions\" parameter is null");
            if (dataPropertyExpressions.Count < 1)
                throw new OWLException("Cannot create OWLDataSomeValuesFrom because given \"dataPropertyExpressions\" parameter must contain at least 1 element");
            if (datarangeExpression == null)
                throw new OWLException("Cannot create OWLDataSomeValuesFrom because given \"datarangeExpression\" parameter is null");
            #endregion

            DataPropertyExpressions = dataPropertyExpressions;
            DataRangeExpression = datarangeExpression;
        }
        #endregion
    }
}