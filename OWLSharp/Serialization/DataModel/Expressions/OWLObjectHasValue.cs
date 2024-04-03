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
    [XmlRoot("ObjectHasValue")]
    public class OWLObjectHasValue : OWLClassExpression
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=1)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=1)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName= "AnonymousIndividual", Order=2)]
        public OWLIndividualExpression IndividualExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectHasValue() { }
        public OWLObjectHasValue(OWLObjectPropertyExpression objectPropertyExpression, OWLIndividualExpression individualExpression)
        {
            ObjectPropertyExpression = objectPropertyExpression ?? throw new OWLException("Cannot create OWLObjectHasValue because given \"objectPropertyExpression\" parameter is null");
            IndividualExpression = individualExpression ?? throw new OWLException("Cannot create OWLObjectHasValue because given \"individualExpression\" parameter is null");
        }
        #endregion
    }
}