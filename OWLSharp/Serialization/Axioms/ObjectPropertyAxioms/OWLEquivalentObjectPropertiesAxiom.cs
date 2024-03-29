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
using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLEquivalentObjectPropertiesAxiom : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLEquivalentObjectPropertiesAxiom() : base() { }
        public OWLEquivalentObjectPropertiesAxiom(List<OWLObjectPropertyExpression> objectPropertyExpressions) : this()
        {
            #region Guards
            if (objectPropertyExpressions == null)
                throw new OWLException("Cannot create OWLEquivalentObjectPropertiesAxiom because given \"objectPropertyExpressions\" parameter is null");
            if (objectPropertyExpressions.Count < 2)
                throw new OWLException("Cannot create OWLEquivalentObjectPropertiesAxiom because given \"objectPropertyExpressions\" parameter must contain at least 2 elements");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions;
        }
        #endregion
    }
}