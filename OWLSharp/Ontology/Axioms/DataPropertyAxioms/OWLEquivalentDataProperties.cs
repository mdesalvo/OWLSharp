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
    [XmlRoot("EquivalentDataProperties")]
    public class OWLEquivalentDataProperties : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=2)]
        public List<OWLDataProperty> DataProperties { get; set; }
        #endregion

        #region Ctors
        internal OWLEquivalentDataProperties()
        { }
        public OWLEquivalentDataProperties(List<OWLDataProperty> dataProperties) : this()
        {
            #region Guards
            if (dataProperties == null)
                throw new OWLException("Cannot create OWLEquivalentDataProperties because given \"dataProperties\" parameter is null");
            if (dataProperties.Count < 2)
                throw new OWLException("Cannot create OWLEquivalentDataProperties because given \"dataProperties\" parameter must contain at least 2 elements");
            if (dataProperties.Any(dp => dp == null))
                throw new OWLException("Cannot create OWLEquivalentObjectProperties because given \"dataProperties\" parameter contains a null element");
            #endregion

            DataProperties = dataProperties;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            List<RDFResource> dtPropIRIs = new List<RDFResource>();
            for (int i = 0; i < DataProperties.Count; i++)
            {
                dtPropIRIs.Add(DataProperties[i].GetIRI());
                graph = graph.UnionWith(DataProperties[i].ToRDFGraph());
            }

            //Axiom Triple(s)
            List<RDFTriple> axiomTriples = new List<RDFTriple>();
            for (int i = 0; i < DataProperties.Count - 1; i++)
                for (int j = i + 1; j < DataProperties.Count; j++)
                {
                    RDFTriple axiomTriple = new RDFTriple(dtPropIRIs[i], RDFVocabulary.OWL.EQUIVALENT_PROPERTY, dtPropIRIs[j]);
                    axiomTriples.Add(axiomTriple);
                    graph.AddTriple(axiomTriple);
                }

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                foreach (RDFTriple axiomTriple in axiomTriples)
                    graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}