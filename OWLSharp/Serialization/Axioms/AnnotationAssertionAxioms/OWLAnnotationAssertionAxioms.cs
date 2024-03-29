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
    public class OWLAnnotationAssertionAxiom : OWLAnnotationAxiom
    {
        #region Properties
        [XmlElement(ElementName="AnnotationProperty", Order=1)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        [XmlElement(Order=2)]
        public OWLAnnotationSubject AnnotationSubject { get; set; }

        [XmlElement(Order=3)]
        public OWLAnnotationValue AnnotationValue { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationAssertionAxiom() : base() { }
        public OWLAnnotationAssertionAxiom(OWLAnnotationProperty annotationProperty, OWLAnnotationSubject annotationSubject, OWLAnnotationValue annotationValue) : this()
        {
            AnnotationProperty = annotationProperty ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"annotationProperty\" parameter is null");
            AnnotationSubject = annotationSubject ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"annotationSubject\" parameter is null");
            AnnotationValue = annotationValue ?? throw new OWLException("Cannot create OWLAnnotationAssertionAxiom because given \"annotationValue\" parameter is null");
        }
        #endregion
    }
}