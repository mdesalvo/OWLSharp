/*
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
    [XmlRoot("ObjectPropertyRange")]
    public class OWLObjectPropertyRange : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }

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
        public OWLClassExpression ClassExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectPropertyRange()
        { }
        internal OWLObjectPropertyRange(OWLClassExpression classExpression) : this()
            => ClassExpression = classExpression ?? throw new OWLException("Cannot create OWLObjectPropertyRange because given \"classExpression\" parameter is null");
        public OWLObjectPropertyRange(OWLObjectProperty objectProperty, OWLClassExpression classExpression) : this(classExpression)
            => ObjectPropertyExpression = objectProperty ?? throw new OWLException("Cannot create OWLObjectPropertyRange because given \"objectProperty\" parameter is null");
        public OWLObjectPropertyRange(OWLObjectInverseOf objectInverseOf, OWLClassExpression classExpression) : this(classExpression)
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException("Cannot create OWLObjectPropertyRange because given \"objectInverseOf\" parameter is null");
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            RDFResource clsExpressionIRI = ClassExpression.GetIRI();
            graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI))
                         .UnionWith(ClassExpression.ToRDFGraph(clsExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDFS.RANGE, clsExpressionIRI);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}