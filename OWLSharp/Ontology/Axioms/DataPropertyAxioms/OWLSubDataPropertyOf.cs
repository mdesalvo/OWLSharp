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

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("SubDataPropertyOf")]
    public class OWLSubDataPropertyOf : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=2)]
        public OWLDataProperty SubDataProperty { get; set; }

        [XmlElement(ElementName="DataProperty", Order=3)]
        public OWLDataProperty SuperDataProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLSubDataPropertyOf() : base() { }
        public OWLSubDataPropertyOf(OWLDataProperty subDataProperty, OWLDataProperty superDataProperty) : this()
        {
            SubDataProperty = subDataProperty ?? throw new OWLException("Cannot create OWLSubDataPropertyOf because given \"subDataProperty\" parameter is null");
            SuperDataProperty = superDataProperty ?? throw new OWLException("Cannot create OWLSubDataPropertyOf because given \"superDataProperty\" parameter is null");
        }
        #endregion
    }
}