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
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDataOneOf is a datatype expression that defines an enumerated datatype containing exactly the specified set of literal values.
    /// For example, DataOneOf("red" "green" "blue") represents a datatype restricted to only those three string literals,
    /// allowing you to constrain datatype properties to a finite, explicit list of allowed values.
    /// </summary>
    [XmlRoot("DataOneOf")]
    public sealed class OWLDataOneOf : OWLDataRangeExpression
    {
        #region Properties
        /// <summary>
        /// The set of literal values defined by this enumeration
        /// </summary>
        [XmlElement(ElementName="Literal")]
        public List<OWLLiteral> Literals { get; set; }
        #endregion

        #region Ctors
        internal OWLDataOneOf() { }

        /// <summary>
        /// Builds an OWLDataOneOf on the given set of literals (must be at least 1)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataOneOf(List<OWLLiteral> literals)
        {
            #region Guards
            if (literals == null)
                throw new OWLException($"Cannot create OWLDataOneOf because given '{nameof(literals)}' parameter is null");
            if (literals.Count == 0)
                throw new OWLException($"Cannot create OWLDataOneOf because given '{nameof(literals)}' parameter must contain at least 1 element");
            if (literals.Any(lit => lit == null))
                throw new OWLException($"Cannot create OWLDataOneOf because given '{nameof(literals)}' parameter contains a null element");
            #endregion

            Literals = literals;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this OWLDataOneOf expression
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("({");
            sb.Append(string.Join(",", Literals.Select(lit => lit.ToSWRLString())));
            sb.Append("})");

            return sb.ToString();
        }

        /// <summary>
        /// Exports this OWLDataOneOf expression to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFCollection dataOneOfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Literal);
            foreach (OWLLiteral dataOneOfLiteral in Literals)
                dataOneOfCollection.AddItem(dataOneOfLiteral.GetLiteral());
            graph.AddCollection(dataOneOfCollection);
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ONE_OF, dataOneOfCollection.ReificationSubject));

            return graph;
        }
        #endregion
    }
}