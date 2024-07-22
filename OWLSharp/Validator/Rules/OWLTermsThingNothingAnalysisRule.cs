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

using OWLSharp.Ontology;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Validator.Rules
{
    internal static class OWLThingNothingAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.ThingNothingAnalysis.ToString();
		internal static readonly string rulesuggT1 = "There should not be any direct or indirect SubClassOf axioms having reserved 'owl:Thing' class in position of subclass: this class should be the root entity!";
        internal static readonly string rulesuggN1 = "There should not be any direct or indirect SubClassOf axioms having reserved 'owl:Nothing' class in position of superclass: this class should be the bottom entity!";
        internal static readonly string rulesuggN2 = "There should not be any direct or indirect ClassAssertion axioms having reserved 'owl:Nothing' class: this class cannot contain individuals by design!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            if (ontology.GetSuperClassesOf(new OWLClass(RDFVocabulary.OWL.THING)).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    $"Detected class axioms causing reserved 'owl:Thing' class to not be the root entity",
                    rulesuggT1));

            if (ontology.GetSubClassesOf(new OWLClass(RDFVocabulary.OWL.NOTHING)).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    $"Detected class axioms causing reserved 'owl:Nothing' class to not be the bottom entity",
                    rulesuggN1));
            if (ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.OWL.NOTHING)).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    $"Detected individuals of reserved 'owl:Nothing' class",
                    rulesuggN2));

            return issues;
        }
    }
}