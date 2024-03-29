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
    public class OWLDataHasValue : OWLClassExpression
    {
        #region Properties
        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=1)]
        public OWLDataPropertyExpression DataPropertyExpression { get; set; }

        //Register here all derived types of OWLLiteralExpression
        [XmlElement(typeof(OWLLiteral), ElementName="Literal", Order=2)]
        public OWLLiteralExpression LiteralExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataHasValue() { }
        public OWLDataHasValue(OWLDataPropertyExpression dataPropertyExpression, OWLLiteralExpression literalExpression)
        {
            DataPropertyExpression = dataPropertyExpression ?? throw new OWLException("Cannot create OWLDataHasValue because given \"dataPropertyExpression\" parameter is null");
            LiteralExpression = literalExpression ?? throw new OWLException("Cannot create OWLDataHasValue because given \"literalExpression\" parameter is null");
        }
        #endregion
    }
}