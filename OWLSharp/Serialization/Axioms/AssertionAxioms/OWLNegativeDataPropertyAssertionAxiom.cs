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
    public class OWLNegativeDataPropertyAssertionAxiom : OWLAssertionAxiom
    {
        #region Properties
        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=1)]
        public OWLDataPropertyExpression DataPropertyExpression { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=2)]
        public OWLIndividualExpression SourceIndividualExpression { get; set; }

        //Register here all derived types of OWLLiteralExpression
        [XmlElement(typeof(OWLLiteral), ElementName="Literal", Order=3)]
        public OWLLiteralExpression TargetLiteralExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLNegativeDataPropertyAssertionAxiom() : base() { }
        public OWLNegativeDataPropertyAssertionAxiom(OWLDataPropertyExpression dataPropertyExpression, OWLIndividualExpression sourceIndividualExpression, OWLLiteralExpression targetLiteralExpression) : this()
        {
            DataPropertyExpression = dataPropertyExpression ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertionAxiom because given \"dataPropertyExpression\" parameter is null");
            SourceIndividualExpression = sourceIndividualExpression ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertionAxiom because given \"sourceIndividualExpression\" parameter is null");
            TargetLiteralExpression = targetLiteralExpression ?? throw new OWLException("Cannot create OWLNegativeDataPropertyAssertionAxiom because given \"targetLiteralExpression\" parameter is null");
        }
        #endregion
    }
}