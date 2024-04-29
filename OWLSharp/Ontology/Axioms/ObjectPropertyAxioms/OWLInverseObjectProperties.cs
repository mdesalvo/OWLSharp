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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("InverseObjectProperties")]
    public class OWLInverseObjectProperties : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression LeftObjectPropertyExpression { get; set; }

        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=3)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=3)]
        public OWLObjectPropertyExpression RightObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLInverseObjectProperties() : base() { }
        public OWLInverseObjectProperties(OWLObjectPropertyExpression leftObjectPropertyExpression, OWLObjectPropertyExpression rightObjectPropertyExpression) : this()
        {
            LeftObjectPropertyExpression = leftObjectPropertyExpression ?? throw new OWLException("Cannot create OWLInverseObjectProperties because given \"leftObjectPropertyExpression\" parameter is null");
            RightObjectPropertyExpression = rightObjectPropertyExpression ?? throw new OWLException("Cannot create OWLInverseObjectProperties because given \"rightObjectPropertyExpression\" parameter is null");
        }
        #endregion

        #region Methods
        internal override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //TODO

            return graph;
        }
        #endregion
    }
}