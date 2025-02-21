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
    internal static class OWLEquivalentClassesAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.EquivalentClassesAnalysis.ToString();
        internal const string rulesugg = "There should not be class expressions belonging at the same time to EquivalentClasses and SubClassOf/DisjointClasses axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, Dictionary<string, object> validatorCache)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //EquivalentClasses(CLS1,CLS2) ^ SubClassOf(CLS1,CLS2) -> ERROR
            //EquivalentClasses(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> ERROR
            //EquivalentClasses(CLS1,CLS2) ^ DisjointClasses(CLS1,CLS2) -> ERROR
            foreach (OWLEquivalentClasses equivClasses in ontology.GetClassAxiomsOfType<OWLEquivalentClasses>())
                if (equivClasses.ClassExpressions.Any(outerClass => 
                      equivClasses.ClassExpressions.Any(innerClass => !outerClass.GetIRI().Equals(innerClass.GetIRI())
                                                                          && (ontology.CheckIsSubClassOf(outerClass, innerClass)
                                                                             || ontology.CheckIsSubClassOf(innerClass, outerClass)
                                                                             || ontology.CheckAreDisjointClasses(outerClass, innerClass)))))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error, 
                        rulename, 
                        $"Violated EquivalentClasses axiom with signature: '{equivClasses.GetXML()}'", 
                        rulesugg));

            return issues;
        }
    }
}