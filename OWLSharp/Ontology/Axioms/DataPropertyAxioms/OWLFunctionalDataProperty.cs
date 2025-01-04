﻿/*
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
    [XmlRoot("FunctionalDataProperty")]
    public class OWLFunctionalDataProperty : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(Order=2)]
        public OWLDataProperty DataProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLFunctionalDataProperty() : base() { }
        public OWLFunctionalDataProperty(OWLDataProperty dataProperty) : this()
            => DataProperty = dataProperty ?? throw new OWLException("Cannot create OWLFunctionalDataProperty because given \"dataProperty\" parameter is null");
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph = graph.UnionWith(DataProperty.ToRDFGraph());

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(DataProperty.GetIRI(), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY);
            graph.AddTriple(axiomTriple);            

			//Annotations
			foreach (OWLAnnotation annotation in Annotations)
				graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}