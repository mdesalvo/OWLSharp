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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    internal static class OWLEquivalentObjectPropertiesAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.EquivalentObjectPropertiesAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be object properties belonging at the same time to EquivalentObjectProperties and SubObjectPropertyOf/DisjointObjectProperties axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //EquivalentObjectProperties(OP1,OP2) ^ SubObjectPropertyOf(OP1,OP2) -> ERROR
			//EquivalentObjectProperties(OP1,OP2) ^ SubObjectPropertyOf(OP2,OP1) -> ERROR
			//EquivalentObjectProperties(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> ERROR
            foreach (OWLEquivalentObjectProperties equivObjectProps in ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>())
				if (equivObjectProps.ObjectPropertyExpressions.Any(outerOPEX => 
					  equivObjectProps.ObjectPropertyExpressions.Any(innerOPEX => !outerOPEX.GetIRI().Equals(innerOPEX.GetIRI())
																					&& (ontology.CheckIsSubObjectPropertyOf(outerOPEX, innerOPEX)
																						|| ontology.CheckIsSubObjectPropertyOf(innerOPEX, outerOPEX)
																						|| ontology.CheckAreDisjointObjectProperties(outerOPEX, innerOPEX)))))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated EquivalentObjectProperties axiom with signature: '{equivObjectProps.GetXML()}'", 
						rulesugg));

            return issues;
        }
    }
}