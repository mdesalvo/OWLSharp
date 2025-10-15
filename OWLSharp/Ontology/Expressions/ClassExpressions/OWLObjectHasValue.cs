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
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLObjectHasValue is a class expression that restricts a class to individuals having a specific named individual as the value for a given object property.
    /// For example, ObjectHasValue(bornIn, Italy) defines the class of all individuals whose bornIn property has the exact value Italy (a specific named individual),
    /// allowing you to define classes based on the presence of particular object property values.
    /// </summary>
    [XmlRoot("ObjectHasValue")]
    public sealed class OWLObjectHasValue : OWLClassExpression
    {
        #region Properties
        /// <summary>
        /// The object property expression on which this class expression is defined
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=1)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=1)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }

        /// <summary>
        /// The individual required to be targeted by the object property expression
        /// </summary>
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName= "AnonymousIndividual", Order=2)]
        public OWLIndividualExpression IndividualExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectHasValue() { }

        /// <summary>
        /// Builds an OWLObjectHasValue expression with the given object property expression and required individual
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLObjectHasValue(OWLObjectPropertyExpression objectPropertyExpression, OWLIndividualExpression individualExpression)
        {
            ObjectPropertyExpression = objectPropertyExpression ?? throw new OWLException($"Cannot create OWLObjectHasValue because given '{nameof(objectPropertyExpression)}' parameter is null");
            IndividualExpression = individualExpression ?? throw new OWLException($"Cannot create OWLObjectHasValue because given '{nameof(individualExpression)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLObjectHasValue expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(ObjectPropertyExpression.ToSWRLString());
            sb.Append(" value ");
            sb.Append(IndividualExpression.ToSWRLString());
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLObjectHasValue expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            RDFResource idvExpressionIRI = IndividualExpression.GetIRI();
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_PROPERTY, objPropExpressionIRI));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.HAS_VALUE, idvExpressionIRI));
            return graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI))
                        .UnionWith(IndividualExpression.ToRDFGraph(idvExpressionIRI));
        }
        #endregion
    }
}