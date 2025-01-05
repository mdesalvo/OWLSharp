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
    [XmlRoot("DifferentIndividuals")]
    public class OWLDifferentIndividuals : OWLAssertionAxiom
    {
        #region Properties
        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual", Order=2)]
        public List<OWLIndividualExpression> IndividualExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDifferentIndividuals() : base() { }
        public OWLDifferentIndividuals(List<OWLIndividualExpression> individualExpressions) : this()
        {
            #region Guards
            if (individualExpressions == null)
                throw new OWLException("Cannot create OWLDifferentIndividuals because given \"individualExpressions\" parameter is null");
            if (individualExpressions.Count < 2)
                throw new OWLException("Cannot create OWLDifferentIndividuals because given \"individualExpressions\" parameter must contain at least 2 elements");
            if (individualExpressions.Any(iex => iex == null))
                throw new OWLException("Cannot create OWLDifferentIndividuals because given \"individualExpressions\" parameter contains a null element");
            #endregion

            IndividualExpressions = individualExpressions;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //differentFrom
            if (IndividualExpressions.Count == 2)
            {
                RDFResource leftIdvExpressionIRI = IndividualExpressions[0].GetIRI();
                RDFResource rightIdvExpressionIRI = IndividualExpressions[1].GetIRI();
                graph = graph.UnionWith(IndividualExpressions[0].ToRDFGraph(leftIdvExpressionIRI))
                             .UnionWith(IndividualExpressions[1].ToRDFGraph(rightIdvExpressionIRI));

                //Axiom Triple
                RDFTriple axiomTriple = new RDFTriple(leftIdvExpressionIRI, RDFVocabulary.OWL.DIFFERENT_FROM, rightIdvExpressionIRI);
                graph.AddTriple(axiomTriple);

                //Annotations
                foreach (OWLAnnotation annotation in Annotations)
                    graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));
            }

            //AllDifferent
            else
            {
                RDFResource allDifferentIRI = new RDFResource(); 
                RDFCollection differentIndividualsCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
                foreach (OWLIndividualExpression idvExpression in IndividualExpressions)
                {
                    RDFResource idvExpressionIRI = idvExpression.GetIRI();
                    differentIndividualsCollection.AddItem(idvExpressionIRI);
                    graph = graph.UnionWith(idvExpression.ToRDFGraph(idvExpressionIRI));
                }
                graph.AddCollection(differentIndividualsCollection);
                graph.AddTriple(new RDFTriple(allDifferentIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT));
                graph.AddTriple(new RDFTriple(allDifferentIRI, RDFVocabulary.OWL.DISTINCT_MEMBERS, differentIndividualsCollection.ReificationSubject));

                //Annotations
                foreach (OWLAnnotation annotation in Annotations)
                    graph = graph.UnionWith(annotation.ToRDFGraphInternal(allDifferentIRI));
            }

            return graph;
        }
        #endregion
    }
}