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
    public class OWLDisjointClassesAxiom : OWLClassAxiom
    {
        #region Properties
        //Register here all derived types of OWLClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class")]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf")]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf")]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf")]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf")]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom")]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom")]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue")]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf")]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality")]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality")]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality")]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom")]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom")]
        public List<OWLClassExpression> ClassExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointClassesAxiom() { }
        public OWLDisjointClassesAxiom(List<OWLClassExpression> classExpressions) 
        {
            #region Guards
            if (classExpressions == null)
                throw new OWLException("Cannot create OWLDisjointClassesAxiom because given \"classExpressions\" parameter is null");
            if (classExpressions.Count < 2)
                throw new OWLException("Cannot create OWLDisjointClassesAxiom because given \"classExpressions\" parameter must contain at least 2 elements");
            #endregion

            ClassExpressions = classExpressions;
        }
        #endregion
    }
}