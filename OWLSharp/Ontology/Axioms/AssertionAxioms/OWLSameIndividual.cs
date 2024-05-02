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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("SameIndividual")]
    public class OWLSameIndividual : OWLAssertionAxiom
    {
        #region Properties
        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=2)]
        public List<OWLIndividualExpression> IndividualExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLSameIndividual() : base() { }
        public OWLSameIndividual(List<OWLIndividualExpression> individualExpressions) : this()
        {
            #region Guards
            if (individualExpressions == null)
                throw new OWLException("Cannot create OWLSameIndividual because given \"individualExpressions\" parameter is null");
            if (individualExpressions.Count < 2)
                throw new OWLException("Cannot create OWLSameIndividual because given \"individualExpressions\" parameter must contain at least 2 elements");
            if (individualExpressions.Any(iex => iex == null))
                throw new OWLException("Cannot create OWLSameIndividual because given \"individualExpressions\" parameter contains a null element");
            #endregion

            IndividualExpressions = individualExpressions;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            List<RDFResource> idvExpressionIRIs = new List<RDFResource>();
			for (int i = 0; i < IndividualExpressions.Count; i++)
			{
				RDFResource idvExpressionIRI = IndividualExpressions[i].GetIRI(); 
				idvExpressionIRIs.Add(idvExpressionIRI);
				graph = graph.UnionWith(IndividualExpressions[i].ToRDFGraph(idvExpressionIRI));
			}

            for (int i = 0; i < IndividualExpressions.Count - 1; i++)
				for (int j = i + 1; j < IndividualExpressions.Count; j++)
					graph.AddTriple(new RDFTriple(idvExpressionIRIs[i], RDFVocabulary.OWL.SAME_AS, idvExpressionIRIs[j]));

			//Annotations
			foreach (OWLAnnotation annotation in Annotations)
				graph = graph.UnionWith(annotation.ToRDFGraph());

            return graph;
        }
        #endregion
    }
}