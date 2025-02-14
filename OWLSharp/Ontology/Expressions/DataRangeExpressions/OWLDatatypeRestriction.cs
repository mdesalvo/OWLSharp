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
    [XmlRoot("DatatypeRestriction")]
    public class OWLDatatypeRestriction : OWLDataRangeExpression
    {
        #region Properties
        [XmlElement(Order=1)]
        public OWLDatatype Datatype { get; set; }

        [XmlElement("FacetRestriction", Order=2)]
        public List<OWLFacetRestriction> FacetRestrictions { get; set; }
        #endregion

        #region Ctors
        internal OWLDatatypeRestriction() { }
        public OWLDatatypeRestriction(OWLDatatype datatypeIRI, List<OWLFacetRestriction> facetRestrictions)
        {
            #region Guards
            if (facetRestrictions?.Any(fr => fr == null) ?? false)
                throw new OWLException("Cannot create OWLDatatypeRestriction because given \"facetRestrictions\" parameter contains a null element");
            #endregion

            Datatype = datatypeIRI ?? throw new OWLException("Cannot create OWLDatatypeRestriction because given \"datatypeIRI\" parameter is null");
            FacetRestrictions = facetRestrictions;
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append(Datatype.ToSWRLString());
            sb.Append('[');
            sb.Append(string.Join(", ", FacetRestrictions.Select(fct => fct.ToSWRLString())));
            sb.Append(']');
            sb.Append(')');

            return sb.ToString();
        }

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

    [XmlRoot("FacetRestriction")]
    public class OWLFacetRestriction
    {
        #region Properties
        [XmlElement]
        public OWLLiteral Literal { get; set; }

        [XmlAttribute("facet", DataType="anyURI")]
        public string FacetIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLFacetRestriction() { }
        public OWLFacetRestriction(OWLLiteral literal, RDFResource facetIRI)
        {
            #region Guards
            if (facetIRI == null)
                throw new OWLException("Cannot create OWLFacetRestriction because given \"facetIRI\" parameter is null");
            if (facetIRI.IsBlank)
                throw new OWLException("Cannot create OWLFacetRestriction because given \"facetIRI\" parameter is a blank resource");
            #endregion

            Literal = literal ?? throw new OWLException("Cannot create OWLFacetRestriction because given \"literal\" parameter is null");
            FacetIRI = facetIRI.ToString();
        }
        #endregion

        #region Methods
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