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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDisjointDataProperties axiom asserts that two or more datatype properties cannot share the same pair of individual and literal value,
    /// meaning they cannot relate the same individual to the same data value simultaneously. For example, DisjointDataProperties(birthDate deathDate)
    /// states that an individual cannot have the same value as both their birth date and death date, allowing reasoners to detect inconsistencies
    /// when the same literal appears as a value for multiple disjoint datatype properties on the same individual.
    /// </summary>
    [XmlRoot("DisjointDataProperties")]
    public sealed class OWLDisjointDataProperties : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=2)]
        public List<OWLDataProperty> DataProperties { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointDataProperties()
        { }
        public OWLDisjointDataProperties(List<OWLDataProperty> dataProperties) : this()
        {
            #region Guards
            if (dataProperties == null)
                throw new OWLException("Cannot create OWLDisjointDataProperties because given \"dataProperties\" parameter is null");
            if (dataProperties.Count < 2)
                throw new OWLException("Cannot create OWLDisjointDataProperties because given \"dataProperties\" parameter must contain at least 2 elements");
            if (dataProperties.Any(dp => dp == null))
                throw new OWLException("Cannot create OWLDisjointDataProperties because given \"dataProperties\" parameter contains a null element");
            #endregion

            DataProperties = dataProperties;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //propertyDisjointWith
            if (DataProperties.Count == 2)
            {
                graph = graph.UnionWith(DataProperties[0].ToRDFGraph())
                             .UnionWith(DataProperties[1].ToRDFGraph());

                //Axiom Triple
                RDFTriple axiomTriple = new RDFTriple(DataProperties[0].GetIRI(), RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, DataProperties[1].GetIRI());
                graph.AddTriple(axiomTriple);

                //Annotations
                foreach (OWLAnnotation annotation in Annotations)
                    graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));
            }

            //AllDisjointProperties
            else
            {
                RDFResource allDisjointPropertiesIRI = new RDFResource();
                RDFCollection disjointDataPropertiesCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
                foreach (OWLDataProperty dataProperty in DataProperties)
                {
                    disjointDataPropertiesCollection.AddItem(dataProperty.GetIRI());
                    graph = graph.UnionWith(dataProperty.ToRDFGraph());
                }
                graph.AddCollection(disjointDataPropertiesCollection);
                graph.AddTriple(new RDFTriple(allDisjointPropertiesIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES));
                graph.AddTriple(new RDFTriple(allDisjointPropertiesIRI, RDFVocabulary.OWL.MEMBERS, disjointDataPropertiesCollection.ReificationSubject));

                //Annotations
                foreach (OWLAnnotation annotation in Annotations)
                    graph = graph.UnionWith(annotation.ToRDFGraphInternal(allDisjointPropertiesIRI));
            }

            return graph;
        }
        #endregion
    }
}