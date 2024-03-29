﻿/*
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
    public class OWLSubAnnotationPropertyOfAxiom : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty", Order=1)]
        public OWLAnnotationPropertyExpression SubAnnotationPropertyExpression { get; set; }

        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationPropertyExpression SuperAnnotationPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSubAnnotationPropertyOfAxiom() : base() { }
        public OWLSubAnnotationPropertyOfAxiom(OWLAnnotationProperty subAnnotationProperty, OWLAnnotationProperty superAnnotationProperty) : this()
        {
            SubAnnotationPropertyExpression = subAnnotationProperty ?? throw new OWLException("Cannot create OWLSubAnnotationPropertyOfAxiom because given \"subAnnotationProperty\" parameter is null");
            SuperAnnotationPropertyExpression = superAnnotationProperty ?? throw new OWLException("Cannot create OWLSubAnnotationPropertyOfAxiom because given \"superAnnotationProperty\" parameter is null");
        }
        #endregion
    }
}