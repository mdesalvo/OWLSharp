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

using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("DatatypeRestriction")]
    public class OWLDatatypeRestriction : OWLDataRangeExpression
    {
        #region Properties
        [XmlElement("Datatype", Order=1)]
        public OWLDatatype Datatype { get; set; }

        [XmlElement("FacetRestriction", Order=2)]
        public List<OWLFacetRestriction> FacetRestrictions { get; set; }
        #endregion

        #region Ctors
        internal OWLDatatypeRestriction() { }
        public OWLDatatypeRestriction(OWLDatatype datatypeIRI, List<OWLFacetRestriction> facetRestrictions)
        {
            #region Guards
            if (facetRestrictions == null)
                throw new OWLException("Cannot create OWLDatatypeRestriction because given \"facetRestrictions\" parameter is null");
            if (facetRestrictions.Count == 0)
                throw new OWLException("Cannot create OWLDatatypeRestriction because given \"facetRestrictions\" parameter must contain at least 1 elements");
            if (facetRestrictions.Any(fr => fr == null))
                throw new OWLException("Cannot create OWLDatatypeRestriction because given \"facetRestrictions\" parameter contains a null element");
            #endregion

            Datatype = datatypeIRI ?? throw new OWLException("Cannot create OWLDatatypeRestriction because given \"datatypeIRI\" parameter is null");
            FacetRestrictions = facetRestrictions;
        }
        #endregion
    }

    [XmlRoot("FacetRestriction")]
    public class OWLFacetRestriction
    {
        #region Statics (TODO: wait for RDFSharp-3.12)
        public static readonly RDFResource LENGTH = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "length"));
        public static readonly RDFResource MIN_LENGTH = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "minLength"));
        public static readonly RDFResource MAX_LENGTH = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "maxLength"));
        public static readonly RDFResource PATTERN = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "pattern"));
        public static readonly RDFResource ENUMERATION = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "enumeration"));
        public static readonly RDFResource MAX_INCLUSIVE = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "maxInclusive"));
        public static readonly RDFResource MAX_EXCLUSIVE = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "maxExclusive"));
        public static readonly RDFResource MIN_EXCLUSIVE = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "minExclusive"));
        public static readonly RDFResource MIN_INCLUSIVE = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "minInclusive"));
        public static readonly RDFResource TOTAL_DIGITS = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "totalDigits"));
        public static readonly RDFResource FRACTION_DIGITS = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "fractionDigits"));
        public static readonly RDFResource ASSERTION = new RDFResource(string.Concat(RDFVocabulary.XSD.BASE_URI, "assertion"));
        #endregion

        #region Properties
        [XmlElement(ElementName="Literal")]
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
    }
}