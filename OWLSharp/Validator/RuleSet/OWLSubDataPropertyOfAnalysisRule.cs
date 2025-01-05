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

namespace OWLSharp.Validator
{
    internal static class OWLSubDataPropertyOfAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.SubDataPropertyOfAnalysis.ToString();
        internal static readonly string rulesugg = "There should not be data properties belonging at the same time to SubDataPropertyOf and EquivalentDataProperties/DisjointDataProperties axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //SubDataPropertyOf(DP1,DP2) ^ SubDataPropertyOf(DP2,DP1) -> ERROR
            //SubDataPropertyOf(DP1,DP2) ^ EquivalentDataProperties(DP1,DP2) -> ERROR
            //SubDataPropertyOf(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> ERROR
            foreach (OWLSubDataPropertyOf subDataPropertyOf in ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>())
                if (ontology.CheckIsSubDataPropertyOf(subDataPropertyOf.SuperDataProperty, subDataPropertyOf.SubDataProperty)
                     || ontology.CheckAreEquivalentDataProperties(subDataPropertyOf.SubDataProperty, subDataPropertyOf.SuperDataProperty)
                     || ontology.CheckAreDisjointDataProperties(subDataPropertyOf.SubDataProperty, subDataPropertyOf.SuperDataProperty))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error, 
                        rulename, 
                        $"Violated SubDataPropertyOf axiom with signature: '{subDataPropertyOf.GetXML()}'", 
                        rulesugg));

            return issues;
        }
    }
}