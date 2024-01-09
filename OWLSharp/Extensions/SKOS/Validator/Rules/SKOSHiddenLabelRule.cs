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
    /// SKOS validator rule checking for consistency of skos:hiddenLabel annotations
    /// </summary>
    internal class SKOSHiddenLabelRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> concepts = ontology.GetConceptsEnumerator();
            while (concepts.MoveNext())
            {
                //skos:hiddenLabel
                List<RDFPlainLiteral> skosHiddenLabels = ontology.Data.OBoxGraph[concepts.Current, RDFVocabulary.SKOS.HIDDEN_LABEL, null, null]
                                                            .Where(t => t.Object is RDFPlainLiteral)
                                                            .Select(t => t.Object as RDFPlainLiteral)
                                                            .ToList();
                foreach (RDFPlainLiteral skosHiddenLabel in skosHiddenLabels)
                    if (!ontology.CheckHiddenLabelCompatibility(concepts.Current, skosHiddenLabel))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(SKOSHiddenLabelRule),
                            $"Violation of 'skos:hiddenLabel' annotation for concept '{concepts.Current}'",
                            "If you annotate a 'skos:Concept' with a 'skos:hiddenLabel' annotation, it should not be used neither with 'skos:prefLabel' or 'skos:altLabel' annotations."));

                //skosxl:hiddenLabel
                List<RDFResource> skosxlHiddenLabels = ontology.Data.ABoxGraph[concepts.Current, RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, null, null]
                                                          .Where(t => t.Object is RDFResource)
                                                          .Select(t => t.Object as RDFResource)
                                                          .ToList();
                foreach (RDFResource skosxlHiddenLabel in skosxlHiddenLabels)
                {
                    List<RDFPlainLiteral> skosxlHiddenLabelValues = ontology.Data.ABoxGraph[skosxlHiddenLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, null, null]
                                                                       .Where(t => t.Object is RDFPlainLiteral)
                                                                       .Select(t => t.Object as RDFPlainLiteral)
                                                                       .ToList();
                    foreach (RDFPlainLiteral skosxlHiddenLabelValue in skosxlHiddenLabelValues)
                        if (!ontology.CheckHiddenLabelCompatibility(concepts.Current, skosxlHiddenLabelValue))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(SKOSHiddenLabelRule),
                                $"Violation of 'skosxl:hiddenLabel' relation for concept '{concepts.Current}' with label '{skosxlHiddenLabel}'",
                                "If you relate a 'skos:Concept' with a 'skosxl:hiddenLabel' relation, it should not be used neither with 'skosxl:prefLabel' or 'skosxl:altLabel' relations."));
                }
            }

            return validatorRuleReport;
        }
    }
}