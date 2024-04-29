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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("DatatypeDefinition")]
    public class OWLDatatypeDefinition : OWLAxiom
    {
        #region Properties
        [XmlElement(Order=2)]
        public OWLDatatype Datatype { get; set; }

        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=3)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=3)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=3)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=3)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=3)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=3)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDatatypeDefinition() : base() { }
        public OWLDatatypeDefinition(OWLDatatype datatypeIRI, OWLDataRangeExpression dataRangeExpression) : this()
        {
            Datatype = datatypeIRI ?? throw new OWLException("Cannot create OWLDatatypeDefinition because given \"datatypeIRI\" parameter is null");
            DataRangeExpression = dataRangeExpression ?? throw new OWLException("Cannot create OWLDatatypeDefinition because given \"dataRangeExpression\" parameter is null");
        }
        #endregion

        #region Methods
        internal override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //TODO

            return graph;
        }
        #endregion
    }
}