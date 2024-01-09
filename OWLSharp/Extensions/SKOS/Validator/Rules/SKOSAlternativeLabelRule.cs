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
    /// SKOS validator rule checking for consistency of skos:altLabel annotations
    /// </summary>
    internal class SKOSAlternativeLabelRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> concepts = ontology.GetConceptsEnumerator();
            while (concepts.MoveNext())
            {
                //skos:altLabel
                List<RDFPlainLiteral> skosAltLabels = ontology.Data.OBoxGraph[concepts.Current, RDFVocabulary.SKOS.ALT_LABEL, null, null]
                                                        .Where(t => t.Object is RDFPlainLiteral)
                                                        .Select(t => t.Object as RDFPlainLiteral)
                                                        .ToList();
                foreach (RDFPlainLiteral skosAltLabel in skosAltLabels)
                    if (!ontology.CheckAlternativeLabelCompatibility(concepts.Current, skosAltLabel))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSAlternativeLabelRule),
                            $"Violation of 'skos:altLabel' annotation for concept '{concepts.Current}'",
                            "If you annotate a 'skos:Concept' with a 'skos:altLabel' annotation, it should not be used neither with 'skos:prefLabel' or 'skos:hiddenLabel' annotations."));

                //skosxl:altLabel
                List<RDFResource> skosxlAltLabels = ontology.Data.ABoxGraph[concepts.Current, RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, null, null]
                                                      .Where(t => t.Object is RDFResource)
                                                      .Select(t => t.Object as RDFResource)
                                                      .ToList();
                foreach (RDFResource skosxlAltLabel in skosxlAltLabels)
                {
                    List<RDFPlainLiteral> skosxlAltLabelValues = ontology.Data.ABoxGraph[skosxlAltLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, null, null]
                                                                   .Where(t => t.Object is RDFPlainLiteral)
                                                                   .Select(t => t.Object as RDFPlainLiteral)
                                                                   .ToList();
                    foreach (RDFPlainLiteral skosxlAltLabelValue in skosxlAltLabelValues)
                        if (!ontology.CheckAlternativeLabelCompatibility(concepts.Current, skosxlAltLabelValue))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(SKOSAlternativeLabelRule),
                                $"Violation of 'skosxl:altLabel' relation for concept '{concepts.Current}' with label '{skosxlAltLabel}'",
                                "If you relate a 'skos:Concept' with a 'skosxl:altLabel' relation, it should not be used neither with 'skosxl:prefLabel' or 'skosxl:hiddenLabel' relations."));
                }
            }

            return validatorRuleReport;
        }
    }
}