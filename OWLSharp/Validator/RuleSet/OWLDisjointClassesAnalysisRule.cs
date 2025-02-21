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
using System.Linq;

namespace OWLSharp.Validator
{
    internal static class OWLDisjointClassesAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.DisjointClassesAnalysis.ToString();
        internal const string rulesugg = "There should not be class expressions belonging at the same time to DisjointClasses and SubClassOf/EquivalentClasses axioms!";
        internal const string rulesugg2 = "There should not be class expressions belonging to a DisjointClasses axiom and having a class assertion on the same individual!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, OWLValidatorContext validatorContext)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            Dictionary<long, HashSet<long>> idvsCache = new Dictionary<long, HashSet<long>>();
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            
            foreach (OWLDisjointClasses disjClasses in ontology.GetClassAxiomsOfType<OWLDisjointClasses>())
            {
                //DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS1,CLS2) -> ERROR
                //DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> ERROR
                //DisjointClasses(CLS1,CLS2) ^ EquivalentClasses(CLS1,CLS2) -> ERROR
                if (disjClasses.ClassExpressions.Any(outerClass => 
                      disjClasses.ClassExpressions.Any(innerClass => !outerClass.GetIRI().Equals(innerClass.GetIRI())
                                                                          && (ontology.CheckIsSubClassOf(outerClass, innerClass)
                                                                             || ontology.CheckIsSubClassOf(innerClass, outerClass)
                                                                             || ontology.CheckAreEquivalentClasses(outerClass, innerClass)))))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error, 
                        rulename, 
                        $"Violated DisjointClasses axiom with signature: '{disjClasses.GetXML()}'", 
                        rulesugg));

                //DisjointClasses(CLS1,CLS2) ^ ClassAssertion(CLS1,IDV) ^ ClassAssertion(CLS2,IDV) -> ERROR
                idvsCache.Clear();
                foreach (OWLClassExpression disjClass in disjClasses.ClassExpressions)
                    foreach (OWLIndividualExpression disjClassIdv in ontology.GetIndividualsOf(disjClass, clsAsns))
                    {
                        RDFResource disjClassIdvIRI = disjClassIdv.GetIRI();
                        if (!idvsCache.ContainsKey(disjClassIdvIRI.PatternMemberID))
                            idvsCache.Add(disjClassIdvIRI.PatternMemberID, new HashSet<long>());
                        idvsCache[disjClassIdvIRI.PatternMemberID].Add(disjClass.GetIRI().PatternMemberID);
                    }
                if (idvsCache.Any(idvc => idvc.Value.Count > 1))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error, 
                        rulename, 
                        $"Violated DisjointClasses axiom with signature: '{disjClasses.GetXML()}'", 
                        rulesugg2));                
            }

            return issues;
        }
    }
}