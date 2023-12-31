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

using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOS-XL validator rule checking for consistency of skosxl:literalForm relations
    /// </summary>
    internal class SKOSXLLiteralFormRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            IEnumerator<RDFResource> labels = ontology.GetLabelsEnumerator();
            while (labels.MoveNext())
            {
                //skosxl:literalForm
                if (ontology.Data.ABoxGraph[labels.Current, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, null, null].TriplesCount != 1)
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(SKOSXLLiteralFormRule),
                        $"Violation of 'skosxl:literalForm' behavior on skosxl:Label '{labels.Current}'",
                        "SKOS-XL labels must have exactly one literal form"));
            }

            return validatorRuleReport;
        }
    }
}