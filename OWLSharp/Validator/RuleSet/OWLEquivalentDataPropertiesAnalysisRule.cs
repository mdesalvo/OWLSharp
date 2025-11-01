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
    internal static class OWLEquivalentDataPropertiesAnalysisRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.EquivalentDataPropertiesAnalysis);
        internal const string rulesugg = "There should not be data properties belonging at the same time to EquivalentDataProperties and SubDataPropertyOf/DisjointDataProperties axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //EquivalentDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP1,DP2) -> ERROR
            //EquivalentDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP2,DP1) -> ERROR
            //EquivalentDataProperties(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> ERROR
            foreach (OWLEquivalentDataProperties equivDataProps in ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>())
                if (equivDataProps.DataProperties.Any(outerDP =>
                      equivDataProps.DataProperties.Any(innerDP => !outerDP.GetIRI().Equals(innerDP.GetIRI())
                                                                    && (ontology.CheckIsSubDataPropertyOf(outerDP, innerDP)
                                                                        || ontology.CheckIsSubDataPropertyOf(innerDP, outerDP)
                                                                        || ontology.CheckAreDisjointDataProperties(outerDP, innerDP)))))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated EquivalentDataProperties axiom with signature: '{equivDataProps.GetXML()}'",
                        rulesugg));
                }

            return issues;
        }
    }
}