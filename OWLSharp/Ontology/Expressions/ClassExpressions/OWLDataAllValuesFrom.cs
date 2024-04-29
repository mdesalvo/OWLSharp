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

namespace OWLSharp.Ontology.Expressions
{
    [XmlRoot("DataAllValuesFrom")]
    public class OWLDataAllValuesFrom : OWLClassExpression
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=1)]
        public List<OWLDataProperty> DataProperties { get; set; }

        //Register here all derived types of OWLDataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=2)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=2)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=2)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataAllValuesFrom() { }
        public OWLDataAllValuesFrom(List<OWLDataProperty> dataProperties, OWLDataRangeExpression datarangeExpression)
        {
            #region Guards
            if (dataProperties == null)
                throw new OWLException("Cannot create OWLDataAllValuesFrom because given \"dataProperties\" parameter is null");
            if (dataProperties.Count == 0)
                throw new OWLException("Cannot create OWLDataAllValuesFrom because given \"dataProperties\" parameter must contain at least 1 element");
            if (dataProperties.Any(dp => dp == null))
                throw new OWLException("Cannot create OWLDataAllValuesFrom because given \"dataProperties\" parameter contains a null element");
            #endregion

            DataProperties = dataProperties;
            DataRangeExpression = datarangeExpression ?? throw new OWLException("Cannot create OWLDataAllValuesFrom because given \"datarangeExpression\" parameter is null");
        }
        #endregion

        #region Methods
        internal override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            foreach (OWLDataProperty dataProperty in DataProperties)
            {
                graph.AddTriple(new RDFTriple(ExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
                graph.AddTriple(new RDFTriple(ExpressionIRI, RDFVocabulary.OWL.ON_PROPERTY, dataProperty.ExpressionIRI));
                graph.AddTriple(new RDFTriple(ExpressionIRI, RDFVocabulary.OWL.ALL_VALUES_FROM, DataRangeExpression.ExpressionIRI));
            }

            return graph;
        }
        #endregion
    }
}