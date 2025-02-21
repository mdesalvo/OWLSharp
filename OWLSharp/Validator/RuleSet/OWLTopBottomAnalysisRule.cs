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

using OWLSharp.Ontology;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Validator
{
    internal static class OWLTopBottomAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.TopBottomAnalysis.ToString();
        internal const string rulesuggT1 = "There should not be any direct or indirect SubObjectPropertyOf axioms having reserved owl:topObjectProperty property in position of subproperty: this object property should be the root object property!";
        internal const string rulesuggT2 = "There should not be any direct or indirect SubDataPropertyOf axioms having reserved owl:topDataProperty property in position of subproperty: this data property should be the root data property!";       
        internal const string rulesuggB1 = "There should not be any direct or indirect SubObjectPropertyOf axioms having reserved owl:bottomObjectProperty property in position of superproperty: this object property should be the bottom object property!";
        internal const string rulesuggB2 = "There should not be any direct or indirect SubDataPropertyOf axioms having reserved owl:bottomDataProperty property in position of superproperty: this data property should be the bottom data property!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, Dictionary<string, object> validatorCache)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            if (ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY)).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected object property axioms causing reserved owl:topObjectProperty property to not be the root object property of the ontology",
                    rulesuggT1));
            if (ontology.GetSuperDataPropertiesOf(new OWLDataProperty(RDFVocabulary.OWL.TOP_DATA_PROPERTY)).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected data property axioms causing reserved owl:topDataProperty property to not be the root data property of the ontology",
                    rulesuggT2));

            if (ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY)).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected object property axioms causing reserved owl:bottomObjectProperty property to not be the bottom object property of the ontology",
                    rulesuggB1));
            if (ontology.GetSubDataPropertiesOf(new OWLDataProperty(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY)).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected data property axioms causing reserved owl:bottomDataProperty property to not be the bottom data property of the ontology",
                    rulesuggB2));

            return issues;
        }
    }
}