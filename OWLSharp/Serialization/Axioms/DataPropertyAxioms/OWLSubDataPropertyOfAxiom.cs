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
    public class OWLSubDataPropertyOfAxiom : OWLDataPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=2)]
        public OWLDataPropertyExpression SubDataPropertyExpression { get; set; }

        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=3)]
        public OWLDataPropertyExpression SuperDataPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSubDataPropertyOfAxiom() : base() { }
        public OWLSubDataPropertyOfAxiom(OWLDataPropertyExpression subDataPropertyExpression, OWLDataPropertyExpression superDataPropertyExpression) : this()
        {
            SubDataPropertyExpression = subDataPropertyExpression ?? throw new OWLException("Cannot create OWLSubDataPropertyOfAxiom because given \"subDataPropertyExpression\" parameter is null");
            SuperDataPropertyExpression = superDataPropertyExpression ?? throw new OWLException("Cannot create OWLSubDataPropertyOfAxiom because given \"superDataPropertyExpression\" parameter is null");
        }
        #endregion
    }
}