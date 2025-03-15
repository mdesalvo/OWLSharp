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

using System.Threading.Tasks;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    public static class OWLOntologyHelper
    {
        #region Methods
        public static async Task<RDFSelectQueryResult> ApplyToOntologyAsync(this RDFSelectQuery selectQuery, OWLOntology ontology, OWLReasoner reasoner=null)
        {
            #region Guards
            if (selectQuery == null)
                throw new OWLException("Cannot apply SPARQL SELECT query to ontology because given \"selectQuery\" parameter is null");
            if (ontology == null)
                throw new OWLException("Cannot apply SPARQL SELECT query to ontology because given \"ontology\" parameter is null");
            #endregion

            //Apply reasoner and integrate inferred axioms
            if (reasoner != null)
                foreach (OWLInference inference in await reasoner.ApplyToOntologyAsync(ontology))
                {
                    switch (inference.Axiom)
                    {
                        case OWLAssertionAxiom asnAx:
                            ontology.DeclareAssertionAxiom(asnAx);
                            break;
                        case OWLClassAxiom clsAx:
                            ontology.DeclareClassAxiom(clsAx);
                            break;
                        case OWLDataPropertyAxiom dpAx:
                            ontology.DeclareDataPropertyAxiom(dpAx);
                            break;
                        case OWLObjectPropertyAxiom opAx:
                            ontology.DeclareObjectPropertyAxiom(opAx);
                            break;
                        case OWLAnnotationAxiom annAx:
                            ontology.DeclareAnnotationAxiom(annAx);
                            break;
                    }
                }

            //Export ontology to graph (with support for inferences and imports)
            RDFGraph graph = await ontology.ToRDFGraphAsync(true,true);

            //Apply query to graph and return results
            return await selectQuery.ApplyToGraphAsync(graph);
        }
        #endregion
    }
}