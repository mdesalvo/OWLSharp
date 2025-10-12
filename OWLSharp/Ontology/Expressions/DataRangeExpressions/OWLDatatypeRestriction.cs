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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDatatypeRestriction is a datatype expression that constrains a base datatype by applying one or more facet restrictions
    /// (such as minInclusive, maxExclusive, pattern, or length).
    /// For example, DatatypeRestriction(xsd:integer minInclusive "0" maxInclusive "100") represents integers between 0 and 100 inclusive,
    /// allowing you to define precise value ranges and format constraints on data values.
    /// </summary>
    [XmlRoot("DatatypeRestriction")]
    public sealed class OWLDatatypeRestriction : OWLDataRangeExpression
    {
        #region Properties
        /// <summary>
        /// The datatype on which this restriction is applied
        /// </summary>
        [XmlElement(Order=1)]
        public OWLDatatype Datatype { get; set; }

        /// <summary>
        /// The set of constraining facets
        /// </summary>
        [XmlElement("FacetRestriction", Order=2)]
        public List<OWLFacetRestriction> FacetRestrictions { get; set; }
        #endregion

        #region Ctors
        internal OWLDatatypeRestriction() { }

        /// <summary>
        /// Builds an OWLDatatypeRestriction constraining the given datatype with the given set of facets
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDatatypeRestriction(OWLDatatype datatypeIRI, List<OWLFacetRestriction> facetRestrictions)
        {
            #region Guards
            if (facetRestrictions?.Any(fr => fr == null) ?? false)
                throw new OWLException($"Cannot create OWLDatatypeRestriction because given '{nameof(facetRestrictions)}' parameter contains a null element");
            #endregion

            Datatype = datatypeIRI ?? throw new OWLException($"Cannot create OWLDatatypeRestriction because given '{nameof(datatypeIRI)}' parameter is null");
            FacetRestrictions = facetRestrictions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this datatype restriction
        /// </summary>
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(Datatype.ToSWRLString());
            sb.Append('[');
            sb.Append(string.Join(", ", FacetRestrictions.Select(fct => fct.ToSWRLString())));
            sb.Append("])");

            return sb.ToString();
        }

        /// <summary>
        /// Exports this datatype restriction to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE));
            graph = graph.UnionWith(Datatype.ToRDFGraph());
            if (FacetRestrictions?.Count > 0)
            {
                RDFCollection facetsRestrictions = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
                foreach (OWLFacetRestriction facetRestriction in FacetRestrictions)
                {
                    RDFResource facetRepresentative = new RDFResource();
                    facetsRestrictions.AddItem(facetRepresentative);
                    graph.AddTriple(new RDFTriple(facetRepresentative, new RDFResource(facetRestriction.FacetIRI), facetRestriction.Literal.GetLiteral()));
                }
                graph.AddCollection(facetsRestrictions);
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ON_DATATYPE, Datatype.GetIRI()));
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.WITH_RESTRICTIONS, facetsRestrictions.ReificationSubject));
            }
            else
                graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.EQUIVALENT_CLASS, Datatype.GetIRI()));

            return graph;
        }
        #endregion
    }

    /// <summary>
    /// OWLFacetRestriction is a constraint applied within a DatatypeRestriction that limits the value space of a datatype
    /// using specific facets and their corresponding restriction values.
    /// For example, a facet like xsd:minInclusive paired with the value "18" restricts the datatype to values greater than or equal to 18,
    /// allowing fine-grained control over allowable literal values through standard XML Schema facets.
    /// </summary>
    [XmlRoot("FacetRestriction")]
    public class OWLFacetRestriction
    {
        #region Properties
        /// <summary>
        /// The literal value of this facet restriction (e.g: 8^^http://www.w3.org/2001/XMLSchema#integer)
        /// </summary>
        [XmlElement]
        public OWLLiteral Literal { get; set; }

        /// <summary>
        /// The IRI of this facet restriction (e.g: http://www.w3.org/2001/XMLSchema#minLength)
        /// </summary>
        [XmlAttribute("facet", DataType="anyURI")]
        public string FacetIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLFacetRestriction() { }

        /// <summary>
        /// Builds a facet restiction with the given literal value and IRI
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLFacetRestriction(OWLLiteral literal, RDFResource facetIRI)
        {
            #region Guards
            if (facetIRI == null)
                throw new OWLException($"Cannot create OWLFacetRestriction because given '{nameof(facetIRI)}' parameter is null");
            if (facetIRI.IsBlank)
                throw new OWLException($"Cannot create OWLFacetRestriction because given '{nameof(facetIRI)}' parameter is a blank resource");
            #endregion

            Literal = literal ?? throw new OWLException($"Cannot create OWLFacetRestriction because given '{nameof(literal)}' parameter is null");
            FacetIRI = facetIRI.ToString();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this datatype restriction
        /// </summary>
        public string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            if (string.Equals(FacetIRI, RDFVocabulary.XSD.LENGTH.ToString()))
                sb.Append($"length {Literal.ToSWRLString()}");
            else if (string.Equals(FacetIRI, RDFVocabulary.XSD.MIN_LENGTH.ToString()))
                sb.Append($"minLength {Literal.ToSWRLString()}");
            else if (string.Equals(FacetIRI, RDFVocabulary.XSD.MAX_LENGTH.ToString()))
                sb.Append($"maxLength {Literal.ToSWRLString()}");
            else if (string.Equals(FacetIRI, RDFVocabulary.XSD.PATTERN.ToString()))
                sb.Append($"pattern {Literal.ToSWRLString()}");
            else if (string.Equals(FacetIRI, RDFVocabulary.XSD.MAX_INCLUSIVE.ToString()))
                sb.Append($"<= {Literal.ToSWRLString()}");
            else if (string.Equals(FacetIRI, RDFVocabulary.XSD.MAX_EXCLUSIVE.ToString()))
                sb.Append($"< {Literal.ToSWRLString()}");
            else if (string.Equals(FacetIRI, RDFVocabulary.XSD.MIN_EXCLUSIVE.ToString()))
                sb.Append($"> {Literal.ToSWRLString()}");
            else if (string.Equals(FacetIRI, RDFVocabulary.XSD.MIN_INCLUSIVE.ToString()))
                sb.Append($">= {Literal.ToSWRLString()}");
            else
                throw new OWLException($"Cannot get SWRL representation of unsupported facet IRI: {FacetIRI}");

            return sb.ToString();
        }
        #endregion
    }
}