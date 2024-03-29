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
    public class OWLSubObjectPropertyOfAxiom : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression SubObjectPropertyExpression { get; set; }

        [XmlElement("ObjectPropertyChain", Order=3)]
        public OWLObjectPropertyChain SubObjectPropertyChain { get; set; }

        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=4)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=4)]
        public OWLObjectPropertyExpression SuperObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSubObjectPropertyOfAxiom() : base() { }
        public OWLSubObjectPropertyOfAxiom(OWLObjectPropertyExpression subObjectPropertyExpression, OWLObjectPropertyExpression superObjectPropertyExpression) : this()
        {
            SubObjectPropertyExpression = subObjectPropertyExpression ?? throw new OWLException("Cannot create OWLSubObjectPropertyOfAxiom because given \"subObjectPropertyExpression\" parameter is null");
            SuperObjectPropertyExpression = superObjectPropertyExpression ?? throw new OWLException("Cannot create OWLSubObjectPropertyOfAxiom because given \"superObjectPropertyExpression\" parameter is null");
        }
        public OWLSubObjectPropertyOfAxiom(OWLObjectPropertyChain subObjectPropertyChain, OWLObjectPropertyExpression superObjectPropertyExpression) : this()
        {
            SubObjectPropertyChain = subObjectPropertyChain ?? throw new OWLException("Cannot create OWLSubObjectPropertyOfAxiom because given \"subObjectPropertyChain\" parameter is null");
            SuperObjectPropertyExpression = superObjectPropertyExpression ?? throw new OWLException("Cannot create OWLSubObjectPropertyOfAxiom because given \"superObjectPropertyExpression\" parameter is null");
        }
        #endregion
    }
}