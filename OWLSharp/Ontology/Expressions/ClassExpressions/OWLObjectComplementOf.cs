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

using System.Text;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLObjectComplementOf is a class expression that represents the complement of a given class,
    /// containing all individuals that are NOT members of the specified class.
    /// For example, ObjectComplementOf(Vehicle) defines the class of all individuals in the domain that are not vehicles,
    /// allowing you to define classes through negation and express exclusionary constraints.
    /// </summary>
    [XmlRoot("ObjectComplementOf")]
    public sealed class OWLObjectComplementOf : OWLClassExpression
    {
        #region Properties
        /// <summary>
        /// The class expression on which this complement is defined
        /// </summary>
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
        public OWLClassExpression ClassExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectComplementOf() { }

        /// <summary>
        /// Builds an OWLObjectComplementOf on the given class expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLObjectComplementOf(OWLClassExpression classExpression)
            => ClassExpression = classExpression ?? throw new OWLException($"Cannot create OWLObjectComplementOf because given '{nameof(classExpression)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLObjectComplementOf expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(not");
            sb.Append(ClassExpression.ToSWRLString());
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLObjectComplementOf expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFResource clsExpressionIRI = ClassExpression.GetIRI();
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.COMPLEMENT_OF, clsExpressionIRI));
            return graph.UnionWith(ClassExpression.ToRDFGraph(clsExpressionIRI));
        }
        #endregion
    }
}