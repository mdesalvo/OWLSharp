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
    public class OWLSubClassOfAxiom : OWLClassAxiom
    {
        #region Properties
        //Register here all derived types of OWLClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=1)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=1)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=1)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=1)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=1)]
        [XmlElement(typeof(OWLObjectSomeValuesFromOf), ElementName="ObjectSomeValuesFrom", Order=1)]
        [XmlElement(typeof(OWLObjectAllValuesFromOf), ElementName="ObjectAllValuesFrom", Order=1)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=1)]
        public OWLClassExpression SubClassExpression { get; set; }

        //Register here all derived types of OWLClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=2)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=2)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=2)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=2)]
        [XmlElement(typeof(OWLObjectSomeValuesFromOf), ElementName="ObjectSomeValuesFrom", Order=2)]
        [XmlElement(typeof(OWLObjectAllValuesFromOf), ElementName="ObjectAllValuesFrom", Order=2)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=2)]
        public OWLClassExpression SuperClassExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLSubClassOfAxiom() { }
        public OWLSubClassOfAxiom(OWLClassExpression subClassExpression, OWLClassExpression superClassExpression) 
        {
            SubClassExpression = subClassExpression ?? throw new OWLException("Cannot create OWLSubClassOfAxiom because given \"subClassExpression\" parameter is null");
            SuperClassExpression = superClassExpression ?? throw new OWLException("Cannot create OWLSubClassOfAxiom because given \"superClassExpression\" parameter is null");
        }
        #endregion
    }
}