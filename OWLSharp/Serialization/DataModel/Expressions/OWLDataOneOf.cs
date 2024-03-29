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
    public class OWLDataOneOf : OWLDataRangeExpression
    {
        #region Properties
        //Register here all derived types of OWLLiteralExpression
        [XmlElement(typeof(OWLLiteral), ElementName="Literal")]
        public List<OWLLiteralExpression> LiteralExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDataOneOf() { }
        public OWLDataOneOf(List<OWLLiteralExpression> literalExpressions)
        {
            #region Guards
            if (literalExpressions == null)
                throw new OWLException("Cannot create OWLDataOneOf because given \"literalExpressions\" parameter is null");
            if (literalExpressions.Count < 1)
                throw new OWLException("Cannot create OWLDataOneOf because given \"literalExpressions\" parameter must contain at least 1 element");
            #endregion

            LiteralExpressions = literalExpressions;
        }
        #endregion
    }
}