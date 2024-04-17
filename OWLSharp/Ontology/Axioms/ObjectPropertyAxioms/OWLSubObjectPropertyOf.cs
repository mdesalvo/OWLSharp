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

using OWLSharp.Ontology.Expressions;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("SubObjectPropertyOf")]
    public class OWLSubObjectPropertyOf : OWLObjectPropertyAxiom
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
        internal OWLSubObjectPropertyOf() : base() { }
        public OWLSubObjectPropertyOf(OWLObjectProperty subObjectProperty, OWLObjectProperty superObjectProperty) : this()
        {
            SubObjectPropertyExpression = subObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectProperty\" parameter is null");
            SuperObjectPropertyExpression = superObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectProperty\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectProperty subObjectProperty, OWLObjectInverseOf superObjectInverseOf) : this()
        {
            SubObjectPropertyExpression = subObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectProperty\" parameter is null");
            SuperObjectPropertyExpression = superObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectInverseOf\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectInverseOf subObjectInverseOf, OWLObjectProperty superObjectProperty) : this()
        {
            SubObjectPropertyExpression = subObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectInverseOf\" parameter is null");
            SuperObjectPropertyExpression = superObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectProperty\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectInverseOf subObjectInverseOf, OWLObjectInverseOf superObjectInverseOf) : this()
        {
            SubObjectPropertyExpression = subObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectInverseOf\" parameter is null");
            SuperObjectPropertyExpression = superObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectInverseOf\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectPropertyChain subObjectPropertyChain, OWLObjectProperty superObjectProperty) : this()
        {
            SubObjectPropertyChain = subObjectPropertyChain ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectPropertyChain\" parameter is null");
            SuperObjectPropertyExpression = superObjectProperty ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectProperty\" parameter is null");
        }
        public OWLSubObjectPropertyOf(OWLObjectPropertyChain subObjectPropertyChain, OWLObjectInverseOf superObjectInverseOf) : this()
        {
            SubObjectPropertyChain = subObjectPropertyChain ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"subObjectPropertyChain\" parameter is null");
            SuperObjectPropertyExpression = superObjectInverseOf ?? throw new OWLException("Cannot create OWLSubObjectPropertyOf because given \"superObjectInverseOf\" parameter is null");
        }
        #endregion
    }
}