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
    /// SKOS validator rule checking for consistency of skos:[related|relatedMatch] relations
    /// </summary>
    internal class SKOSRelatedRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> concepts = ontology.GetConceptsEnumerator();
            while (concepts.MoveNext())
            {
                //skos:related
                foreach (RDFResource relatedConcept in ontology.GetRelatedConcepts(concepts.Current))
                { 
                    if (!ontology.CheckRelatedCompatibility(concepts.Current, relatedConcept))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSRelatedRule),
                            $"Violation of 'skos:related' behavior of concept '{concepts.Current}' against concept '{relatedConcept}'.",
                            "These concepts have a hierarchical or mapping clash: this violates the integrity of 'skos:related' relations."));
                }
                //skos:relatedMatch
                foreach (RDFResource relatedMatchConcept in ontology.GetRelatedMatchConcepts(concepts.Current))
                { 
                    if (!ontology.CheckRelatedCompatibility(concepts.Current, relatedMatchConcept))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSRelatedRule),
                            $"Violation of 'skos:relatedMatch' behavior of concept '{concepts.Current}' against concept '{relatedMatchConcept}'.",
                            "These concepts have a hierarchical or mapping clash: this violates the integrity of 'skos:relatedMatch' relations."));
                }
            }

            return validatorRuleReport;
        }
    }
}