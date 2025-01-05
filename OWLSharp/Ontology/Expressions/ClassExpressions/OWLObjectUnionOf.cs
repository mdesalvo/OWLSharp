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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    [XmlRoot("ObjectUnionOf")]
    public class OWLObjectUnionOf : OWLClassExpression
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
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue")]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality")]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality")]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality")]
        public List<OWLClassExpression> ClassExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectUnionOf() { }
        public OWLObjectUnionOf(List<OWLClassExpression> classExpressions)
        {
            #region Guards
            if (classExpressions == null)
                throw new OWLException("Cannot create OWLObjectUnionOf because given \"classExpressions\" parameter is null");
            if (classExpressions.Count < 2)
                throw new OWLException("Cannot create OWLObjectUnionOf because given \"classExpressions\" parameter must contain at least 2 elements");
            if (classExpressions.Any(cex => cex == null))
                throw new OWLException("Cannot create OWLObjectUnionOf because given \"classExpressions\" parameter contains a null element");
            #endregion

            ClassExpressions = classExpressions;
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(string.Join(" or ", ClassExpressions.Select(clsExpr => clsExpr.ToSWRLString())));
            sb.Append(')');

            return sb.ToString();
        }

        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFCollection objectUnionOfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (OWLClassExpression classExpression in ClassExpressions)
            {
                RDFResource clsExpressionIRI = classExpression.GetIRI();
                objectUnionOfCollection.AddItem(clsExpressionIRI);
                graph = graph.UnionWith(classExpression.ToRDFGraph(clsExpressionIRI));
            }
            graph.AddCollection(objectUnionOfCollection);
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.UNION_OF, objectUnionOfCollection.ReificationSubject));

            return graph;
        }
        #endregion
    }
}