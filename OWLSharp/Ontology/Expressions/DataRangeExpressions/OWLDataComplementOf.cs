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

using System.Xml;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Expressions
{
    [XmlRoot("DataComplementOf")]
    public class OWLDataComplementOf : OWLDataRangeExpression
    {
        #region Properties
        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype")]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf")]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf")]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf")]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf")]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction")]
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataComplementOf() { }
        public OWLDataComplementOf(OWLDataRangeExpression datarangeExpression)
            => DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataComplementOf because given \"datarangeExpression\" parameter is null");
        #endregion

		#region Methods
		internal override RDFGraph ToRDFGraph()
		{
			RDFGraph graph = new RDFGraph();

			graph.AddTriple(new RDFTriple(ExpressionIRI, RDFVocabulary.OWL.COMPLEMENT_OF, DataRangeExpression.ExpressionIRI));

			return graph;
		}
		#endregion
    }
}