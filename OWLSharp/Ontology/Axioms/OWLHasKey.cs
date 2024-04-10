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
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("HasKey")]
    public class OWLHasKey : OWLAxiom
    {
        #region Properties
        //Register here all derived types of OWLClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=2)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=2)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=2)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=2)]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom", Order=2)]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom", Order=2)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=2)]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf", Order=2)]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality", Order=2)]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality", Order=2)]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality", Order=2)]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom", Order=2)]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom", Order=2)]
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue", Order=2)]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality", Order=2)]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality", Order=2)]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality", Order=2)]
        public OWLClassExpression ClassExpression { get; set; }

        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=3)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=3)]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }

        [XmlElement(ElementName="DataProperty", Order=4)]
        public List<OWLDataProperty> DataProperties { get; set; }
        #endregion

        #region Ctors
        internal OWLHasKey() : base() 
            => SerializationPriority = 6;
        public OWLHasKey(OWLClassExpression classExpression, List<OWLObjectPropertyExpression> objectPropertyExpressions, List<OWLDataProperty> dataProperties) : this()
        {
            #region Guards
            if (objectPropertyExpressions?.Any(ope => ope == null) ?? false)
                throw new OWLException("Cannot create OWLHasKey because given \"objectPropertyExpressions\" parameter contains a null element");
            if (dataProperties?.Any(dp => dp == null) ?? false)
                throw new OWLException("Cannot create OWLHasKey because given \"dataProperties\" parameter contains a null element");
            #endregion

            ClassExpression = classExpression ?? throw new OWLException("Cannot create OWLHasKey because given \"classExpression\" parameter is null");
            ObjectPropertyExpressions = objectPropertyExpressions ?? new List<OWLObjectPropertyExpression>();
            DataProperties = dataProperties ?? new List<OWLDataProperty>();
        }
        #endregion
    }
}