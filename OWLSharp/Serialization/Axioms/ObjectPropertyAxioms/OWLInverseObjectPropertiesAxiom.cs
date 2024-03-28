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
    public class OWLInverseObjectPropertiesAxiom : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=1)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=1)]
        public OWLObjectPropertyExpression LeftObjectPropertyExpression { get; set; }

        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression RightObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLInverseObjectPropertiesAxiom() : base() { }
        public OWLInverseObjectPropertiesAxiom(OWLObjectPropertyExpression leftObjectPropertyExpression, OWLObjectPropertyExpression rightObjectPropertyExpression) : this()
        {
            LeftObjectPropertyExpression = leftObjectPropertyExpression ?? throw new OWLException("Cannot create OWLInverseObjectPropertiesAxiom because given \"leftObjectPropertyExpression\" parameter is null");
            RightObjectPropertyExpression = rightObjectPropertyExpression ?? throw new OWLException("Cannot create OWLInverseObjectPropertiesAxiom because given \"rightObjectPropertyExpression\" parameter is null");
        }
        #endregion
    }
}