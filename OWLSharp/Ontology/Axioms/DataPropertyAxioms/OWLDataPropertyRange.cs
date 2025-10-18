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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDataPropertyRange axiom constrains a datatype property by specifying the data range or datatype that its values must belong to.
    /// For example, DataPropertyRange(hasAge xsd:nonNegativeInteger) asserts that all values of the hasAge property must be non-negative integers,
    /// allowing reasoners to validate data values and detect inconsistencies when property values fall outside the specified range.
    /// </summary>
    [XmlRoot("DataPropertyRange")]
    public sealed class OWLDataPropertyRange : OWLDataPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the property used for this data axiom (e.g: http://example.org/hasAge)
        /// </summary>
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }

        /// <summary>
        /// Represents the datarange expression asserted to be range of the data property (e.g: http://www.w3.org/2001/XMLSchema#nonNegativeInteger)
        /// </summary>
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=3)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=3)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=3)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=3)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=3)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=3)]
        public OWLDataRangeExpression DataRangeExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLDataPropertyRange() { }

        /// <summary>
        /// Builds an OWLDataPropertyRange with the given data property and datarange expression as range
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataPropertyRange(OWLDataProperty dataProperty, OWLDataRangeExpression datarangeExpression) : this()
        {
            DataProperty = dataProperty ?? throw new OWLException($"Cannot create OWLDataPropertyRange because given '{nameof(dataProperty)}' parameter is null");
            DataRangeExpression = datarangeExpression ?? throw new OWLException($"Cannot create OWLDataPropertyRange because given '{nameof(datarangeExpression)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLDataPropertyRange to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = DataProperty.ToRDFGraph();

            RDFResource drExpressionIRI = DataRangeExpression.GetIRI();
            graph = graph.UnionWith(DataRangeExpression.ToRDFGraph(drExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(DataProperty.GetIRI(), RDFVocabulary.RDFS.RANGE, drExpressionIRI);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}