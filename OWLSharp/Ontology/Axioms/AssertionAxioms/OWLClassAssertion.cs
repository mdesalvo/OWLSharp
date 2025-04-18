﻿/*
   Copyright 2014-2025 Marco De Salvo

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

using RDFSharp.Model;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("ClassAssertion")]
    public sealed class OWLClassAssertion : OWLAssertionAxiom
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

        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=3)]
        public OWLIndividualExpression IndividualExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLClassAssertion()
        { }
        internal OWLClassAssertion(OWLClassExpression classExpression) : this()
            => ClassExpression = classExpression ?? throw new OWLException("Cannot create OWLClassAssertion because given \"classExpression\" parameter is null");
        public OWLClassAssertion(OWLClassExpression classExpression, OWLNamedIndividual namedIndividual) : this(classExpression)
            => IndividualExpression = namedIndividual ?? throw new OWLException("Cannot create OWLClassAssertion because given \"namedIndividual\" parameter is null");
        public OWLClassAssertion(OWLClassExpression classExpression, OWLAnonymousIndividual anonymousIndividual) : this(classExpression)
            => IndividualExpression = anonymousIndividual ?? throw new OWLException("Cannot create OWLClassAssertion because given \"anonymousIndividual\" parameter is null");
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            RDFResource clsExpressionIRI = ClassExpression.GetIRI();
            RDFResource idvExpressionIRI = IndividualExpression.GetIRI();
            graph = graph.UnionWith(ClassExpression.ToRDFGraph(clsExpressionIRI))
                         .UnionWith(IndividualExpression.ToRDFGraph(idvExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(idvExpressionIRI, RDFVocabulary.RDF.TYPE, clsExpressionIRI);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}