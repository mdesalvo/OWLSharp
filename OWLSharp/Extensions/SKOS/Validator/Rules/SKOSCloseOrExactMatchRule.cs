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

using System.Collections.Generic;
using RDFSharp.Model;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOS validator rule checking for consistency of skos:[close|exact]Match relations
    /// </summary>
    internal class SKOSCloseOrExactMatchRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> concepts = ontology.GetConceptsEnumerator();
            while (concepts.MoveNext())
            {
                //skos:related
                foreach (RDFResource closeMatchConcept in ontology.GetCloseMatchConcepts(concepts.Current))
                { 
                    if (!ontology.CheckCloseOrExactMatchCompatibility(concepts.Current, closeMatchConcept))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSCloseOrExactMatchRule),
                            $"Violation of 'skos:closeMatch' behavior of concept '{concepts.Current}' against concept '{closeMatchConcept}'.",
                            "These concepts have a hierarchical or mapping clash: this violates the integrity of 'skos:closeMatch' relations."));
                }
                //skos:relatedMatch
                foreach (RDFResource exactMatchConcept in ontology.GetExactMatchConcepts(concepts.Current))
                { 
                    if (!ontology.CheckCloseOrExactMatchCompatibility(concepts.Current, exactMatchConcept))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSCloseOrExactMatchRule),
                            $"Violation of 'skos:exactMatch' behavior of concept '{concepts.Current}' against concept '{exactMatchConcept}'.",
                            "These concepts have a hierarchical or mapping clash: this violates the integrity of 'skos:exactMatch' relations."));
                }
            }

            return validatorRuleReport;
        }
    }
}