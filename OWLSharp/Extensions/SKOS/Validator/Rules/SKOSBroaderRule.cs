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
    /// SKOS validator rule checking for consistency of skos:[broader|broaderTransitive|broadMatch] relations
    /// </summary>
    internal class SKOSBroaderRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> concepts = ontology.GetConceptsEnumerator();
            while (concepts.MoveNext())
            {
                //skos:broader, skos:broaderTransitive
                foreach (RDFResource broaderConcept in ontology.GetBroaderConcepts(concepts.Current))
                { 
                    if (!ontology.CheckBroaderCompatibility(concepts.Current, broaderConcept))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSBroaderRule),
                            $"Violation of 'skos:[broader|broaderTransitive|broadMatch]' behavior of concept '{concepts.Current}' against concept '{broaderConcept}'.",
                            "These concepts have a hierarchical or associative or mapping clash: this violates the integrity of 'skos:[broader|broaderTransitive|broadMatch]' relations."));
                }
            }

            return validatorRuleReport;
        }
    }
}