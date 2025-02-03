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

using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (inference.Axiom is OWLAssertionAxiom asnAx)
                        ontology.DeclareAssertionAxiom(asnAx);
                    else if (inference.Axiom is OWLClassAxiom clsAx)
                        ontology.DeclareClassAxiom(clsAx);
                    else if (inference.Axiom is OWLDataPropertyAxiom dpAx)
                        ontology.DeclareDataPropertyAxiom(dpAx);
                    else if (inference.Axiom is OWLObjectPropertyAxiom opAx)
                        ontology.DeclareObjectPropertyAxiom(opAx);
                    else if (inference.Axiom is OWLAnnotationAxiom annAx)
                        ontology.DeclareAnnotationAxiom(annAx);
                }

            //Export ontology to graph (with support for inferences)
            RDFGraph graph = await ontology.ToRDFGraphAsync();

            //Apply query to graph and return results
            return await selectQuery.ApplyToGraphAsync(graph); 
        }
        #endregion
    }
}