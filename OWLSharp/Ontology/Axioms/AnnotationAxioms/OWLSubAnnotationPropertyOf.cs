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

namespace OWLSharp.Ontology
{
    [XmlRoot("SubAnnotationPropertyOf")]
    public class OWLSubAnnotationPropertyOf : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationProperty SubAnnotationPropertyExpression { get; set; }

        [XmlElement(ElementName="AnnotationProperty", Order=3)]
        public OWLAnnotationProperty SuperAnnotationPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSubAnnotationPropertyOf() : base() { }
        public OWLSubAnnotationPropertyOf(OWLAnnotationProperty subAnnotationProperty, OWLAnnotationProperty superAnnotationProperty) : this()
        {
            SubAnnotationPropertyExpression = subAnnotationProperty ?? throw new OWLException("Cannot create OWLSubAnnotationPropertyOf because given \"subAnnotationProperty\" parameter is null");
            SuperAnnotationPropertyExpression = superAnnotationProperty ?? throw new OWLException("Cannot create OWLSubAnnotationPropertyOf because given \"superAnnotationProperty\" parameter is null");
        }
        #endregion
    }
}