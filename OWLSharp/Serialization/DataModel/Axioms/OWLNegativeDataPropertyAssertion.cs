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

using System.Xml.Serialization;

namespace OWLSharp
{
    [XmlRoot("NegativeDataPropertyAssertion")]
    public class OWLNegativeDataPropertyAssertion : OWLAssertionAxiom
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression SourceIndividualExpression { get; set; }

        [XmlElement(ElementName="Literal", Order=4)]
        public OWLLiteral TargetLiteral { get; set; }
        #endregion

        #region Ctors
        internal OWLNegativeDataPropertyAssertion() : base() { }
        public OWLNegativeDataPropertyAssertion(OWLDataProperty dataProperty, OWLIndividualExpression sourceIndividualExpression, OWLLiteral targetLiteral) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertion because given \"dataProperty\" parameter is null");
            SourceIndividualExpression = sourceIndividualExpression ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertion because given \"sourceIndividualExpression\" parameter is null");
            TargetLiteral = targetLiteral ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertion because given \"targetLiteral\" parameter is null");
        }
        #endregion
    }
}