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
    /// OWLObjectInverseOf is an expression suitable for modeling an anonymous object property representing the inverse of the given object property.
    /// It is useful for dealing with situations in which the ontology T-BOX cannot be tampered with an OWLSubObjectPropertyOf axiom specification.
    /// </summary>
    [XmlRoot("ObjectInverseOf")]
    public sealed class OWLObjectInverseOf : OWLObjectPropertyExpression
    {
        #region Properties
        /// <summary>
        /// The object property on which this OWLObjectInverseOf expression is acting as anonymous inverse (e.g: http://example.org/loves)
        /// </summary>
        [XmlElement]
        public OWLObjectProperty ObjectProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectInverseOf() { }

        /// <summary>
        /// Builds an OWLObjectInverseOf on the given object property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLObjectInverseOf(OWLObjectProperty objectProperty)
            => ObjectProperty = objectProperty ?? throw new OWLException($"Cannot create OWLObjectInverseOf because given '{nameof(objectProperty)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLObjectInverseOf expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("inverse(");
            sb.Append(ObjectProperty.ToSWRLString());
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLObjectInverseOf expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.INVERSE_OF, ObjectProperty.GetIRI()));
            return graph.UnionWith(ObjectProperty.ToRDFGraph());
        }
        #endregion
    }
}