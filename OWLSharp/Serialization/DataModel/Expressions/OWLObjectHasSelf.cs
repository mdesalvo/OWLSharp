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

namespace OWLSharp.Serialization
{
    [XmlRoot("ObjectHasSelf")]
    public class OWLObjectHasSelf : OWLClassExpression
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty")]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf")]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectHasSelf() { }
        public OWLObjectHasSelf(OWLObjectPropertyExpression objectPropertyExpression)
            => ObjectPropertyExpression = objectPropertyExpression ?? throw new OWLException("Cannot create OWLObjectHasSelf because given \"objectPropertyExpression\" parameter is null");
        #endregion
    }
}