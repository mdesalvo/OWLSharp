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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("DisjointUnion")]
    public class OWLDisjointUnion : OWLClassAxiom
    {
        #region Properties
        [XmlElement("Class", Order=2)]
        public OWLClass ClassIRI { get; set; }

        //Register here all derived types of OWLClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=3)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=3)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=3)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=3)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=3)]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom", Order=3)]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom", Order=3)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=3)]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf", Order=3)]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality", Order=3)]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality", Order=3)]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality", Order=3)]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom", Order=3)]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom", Order=3)]
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue", Order=3)]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality", Order=3)]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality", Order=3)]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality", Order=3)]
        public List<OWLClassExpression> ClassExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointUnion() : base() { }
        public OWLDisjointUnion(OWLClass classIRI, List<OWLClassExpression> classExpressions) : this()
        {
            #region Guards
            if (classExpressions == null)
                throw new OWLException("Cannot create OWLDisjointUnion because given \"classExpressions\" parameter is null");
            if (classExpressions.Count < 2)
                throw new OWLException("Cannot create OWLDisjointUnion because given \"classExpressions\" parameter must contain at least 2 elements");
            if (classExpressions.Any(cex => cex == null))
                throw new OWLException("Cannot create OWLDisjointUnion because given \"classExpressions\" parameter contains a null element");
            #endregion

            ClassIRI = classIRI ?? throw new OWLException("Cannot create OWLDisjointUnion because given \"classIRI\" parameter is null");
            ClassExpressions = classExpressions;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //TODO

            return graph;
        }
        #endregion
    }
}