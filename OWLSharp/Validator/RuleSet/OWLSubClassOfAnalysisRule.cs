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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Helpers;
using System.Collections.Generic;

namespace OWLSharp.Validator.RuleSet
{
	internal static class OWLSubClassOfAnalysisRule
	{
		internal static readonly string rulename = OWLEnums.OWLValidatorRules.SubClassOfAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be class expressions belonging at the same time to SubClassOf and EquivalentClasses/DisjointClasses axioms!";

		internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
		{
			List<OWLIssue> issues = new List<OWLIssue>();

			//SubClassOf(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> ERROR
			//SubClassOf(CLS1,CLS2) ^ EquivalentClasses(CLS1,CLS2) -> ERROR
			//SubClassOf(CLS1,CLS2) ^ DisjointClasses(CLS1,CLS2) -> ERROR
			foreach (OWLSubClassOf subClassOf in ontology.GetClassAxiomsOfType<OWLSubClassOf>())
				if (ontology.CheckIsSubClassOf(subClassOf.SuperClassExpression, subClassOf.SubClassExpression)
					|| ontology.CheckAreEquivalentClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression)
					|| ontology.CheckAreDisjointClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression))
				issues.Add(new OWLIssue(OWLEnums.OWLIssueSeverity.Error, rulename, $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", rulesugg));

			return issues;
		}
	}
}