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

using RDFSharp.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOS validator rule checking for consistency of skos:prefLabel annotations
    /// </summary>
    internal class SKOSPreferredLabelRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> concepts = ontology.GetConceptsEnumerator();
            while (concepts.MoveNext())
            {
                //skos:prefLabel
                List<RDFPlainLiteral> skosPrefLabels = ontology.Data.OBoxGraph[concepts.Current, RDFVocabulary.SKOS.PREF_LABEL, null, null]
                                                        .Where(t => t.Object is RDFPlainLiteral)
                                                        .Select(t => t.Object as RDFPlainLiteral)
                                                        .ToList();
                HashSet<string> skosPrefLabelLanguages = new HashSet<string>();
                foreach (RDFPlainLiteral skosPrefLabel in skosPrefLabels)
                {
                    if (!skosPrefLabelLanguages.Contains(skosPrefLabel.Language))
                        skosPrefLabelLanguages.Add(skosPrefLabel.Language);
                    else
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSPreferredLabelRule),
                            $"Violation of 'skos:prefLabel' annotation for concept '{concepts.Current}'",
                            "If you annotate a 'skos:Concept' with a linguistic 'skos:prefLabel' annotation, its language tag should not be used more than once."));
                }

                //skosxl:prefLabel
                List<RDFResource> skosxlPrefLabels = ontology.Data.ABoxGraph[concepts.Current, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, null, null]
                                                      .Where(t => t.Object is RDFResource)
                                                      .Select(t => t.Object as RDFResource)
                                                      .ToList();
                HashSet<string> skosxlPrefLabelLanguages = new HashSet<string>();
                foreach (RDFResource skosxlPrefLabel in skosxlPrefLabels)
                {
                    List<RDFPlainLiteral> skosxlPrefLabelValues = ontology.Data.ABoxGraph[skosxlPrefLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, null, null]
                                                                   .Where(t => t.Object is RDFPlainLiteral)
                                                                   .Select(t => t.Object as RDFPlainLiteral)
                                                                   .ToList();
                    foreach (RDFPlainLiteral skosxlPrefLabelValue in skosxlPrefLabelValues)
                    {
                        if (!skosxlPrefLabelLanguages.Contains(skosxlPrefLabelValue.Language))
                            skosxlPrefLabelLanguages.Add(skosxlPrefLabelValue.Language);
                        else
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(SKOSPreferredLabelRule),
                                $"Violation of 'skosxl:prefLabel' relation for concept '{concepts.Current}' with label '{skosxlPrefLabel}'",
                                "If you relate a 'skos:Concept' with a linguistic 'skosxl:prefLabel' relation, its language tag should not be used more than once."));
                    }
                }
            }

            return validatorRuleReport;
        }
    }
}