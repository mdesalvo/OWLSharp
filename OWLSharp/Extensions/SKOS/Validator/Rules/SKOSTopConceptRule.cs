﻿/*
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
    /// SKOS validator rule checking for consistency of skos:hasTopConcept relations
    /// </summary>
    internal class SKOSTopConceptRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> conceptSchemes = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemes.MoveNext())
            {
                //skos:hasTopConcept
                foreach (RDFTriple hasTopConceptRelation in ontology.Data.ABoxGraph[conceptSchemes.Current, RDFVocabulary.SKOS.HAS_TOP_CONCEPT, null, null])
                { 
                    if (ontology.GetBroaderConcepts((RDFResource)hasTopConceptRelation.Object).Count > 0)
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Warning,
                            nameof(SKOSTopConceptRule),
                            $"Violation of 'skos:hasTopConcept' behavior for concept '{hasTopConceptRelation.Object}' in concept scheme '{conceptSchemes.Current}'",
                            "If you specify a 'skos:Concept' as top concept of a 'skos:ConceptScheme', this concept should not have any broader concepts"));
                }
            }

            return validatorRuleReport;
        }
    }
}