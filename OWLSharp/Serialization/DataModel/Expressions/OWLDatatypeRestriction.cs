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
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp
{
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
            if (facetRestrictions.Count < 1)
                throw new OWLException("Cannot create OWLDatatypeRestriction because given \"facetRestrictions\" parameter must contain at least 1 elements");
            #endregion

            Datatype = datatypeIRI ?? throw new OWLException("Cannot create OWLDatatypeRestriction because given \"datatypeIRI\" parameter is null");
            FacetRestrictions = facetRestrictions;
        }
        #endregion
    }

    public class OWLFacetRestriction
    {
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
            Literal = literal ?? throw new OWLException("Cannot create OWLFacetRestriction because given \"literal\" parameter is null");
            FacetIRI = facetIRI?.ToString() ?? throw new OWLException("Cannot create OWLFacetRestriction because given \"facetIRI\" parameter is null");
        }
        #endregion
    }
}