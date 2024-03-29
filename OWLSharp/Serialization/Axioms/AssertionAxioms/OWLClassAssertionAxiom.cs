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
    public class OWLClassAssertionAxiom : OWLAssertionAxiom
    {
        #region Properties
        //Register here all derived types of OWLClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=2)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=1)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=1)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=1)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=1)]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom", Order=1)]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom", Order=1)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=1)]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf", Order=1)]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality", Order=1)]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality", Order=1)]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality", Order=1)]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom", Order=1)]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom", Order=1)]
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue", Order=1)]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality", Order=1)]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality", Order=1)]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality", Order=1)]
        public OWLClassExpression ClassExpression { get; set; }

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=2)]
        public OWLIndividualExpression IndividualExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLClassAssertionAxiom() : base() { }
        public OWLClassAssertionAxiom(OWLClassExpression classExpression, OWLIndividualExpression individualExpression) : this()
        {
            ClassExpression = classExpression ?? throw new OWLException("Cannot create OWLClassAssertionAxiom because given \"classExpression\" parameter is null");
            IndividualExpression = individualExpression ?? throw new OWLException("Cannot create OWLClassAssertionAxiom because given \"individualExpression\" parameter is null");
        }
        #endregion
    }
}