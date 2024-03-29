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
    public class OWLNegativeObjectPropertyAssertionAxiom : OWLAssertionAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=1)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=1)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=2)]
        public OWLIndividualExpression SourceIndividualExpression { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression TargetIndividualExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLNegativeObjectPropertyAssertionAxiom() : base() { }
        public OWLNegativeObjectPropertyAssertionAxiom(OWLObjectPropertyExpression objectPropertyExpression, OWLIndividualExpression sourceIndividualExpression, OWLIndividualExpression targetIndividualExpression) : this()
        {
            ObjectPropertyExpression = objectPropertyExpression ?? throw new OWLException("Cannot create OWLNegativeObjectPropertyAssertionAxiom because given \"objectPropertyExpression\" parameter is null");
            SourceIndividualExpression = sourceIndividualExpression ?? throw new OWLException("Cannot create OWLNegativeObjectPropertyAssertionAxiom because given \"sourceIndividualExpression\" parameter is null");
            TargetIndividualExpression = targetIndividualExpression ?? throw new OWLException("Cannot create OWLNegativeObjectPropertyAssertionAxiom because given \"targetIndividualExpression\" parameter is null");
        }
        #endregion
    }
}