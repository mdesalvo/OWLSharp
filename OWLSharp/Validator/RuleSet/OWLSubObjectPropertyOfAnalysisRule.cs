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
    internal static class OWLSubObjectPropertyOfAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.SubObjectPropertyOfAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be object properties belonging at the same time to SubObjectPropertyOf and EquivalentObjectProperties/DisjointObjectProperties axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //SubObjectPropertyOf(OP1,OP2) ^ SubObjectPropertyOf(OP2,OP1) -> ERROR
			//SubObjectPropertyOf(OP1,OP2) ^ EquivalentObjectProperties(OP1,OP2) -> ERROR
			//SubObjectPropertyOf(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> ERROR
            foreach (OWLSubObjectPropertyOf subObjectPropertyOf in ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>())
				if (ontology.CheckIsSubObjectPropertyOf(subObjectPropertyOf.SuperObjectPropertyExpression, subObjectPropertyOf.SubObjectPropertyExpression)
					 || ontology.CheckAreEquivalentObjectProperties(subObjectPropertyOf.SubObjectPropertyExpression, subObjectPropertyOf.SuperObjectPropertyExpression)
					 || ontology.CheckAreDisjointObjectProperties(subObjectPropertyOf.SubObjectPropertyExpression, subObjectPropertyOf.SuperObjectPropertyExpression))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated SubObjectPropertyOf axiom with signature: '{subObjectPropertyOf.GetXML()}'", 
						rulesugg));

            return issues;
        }
    }
}