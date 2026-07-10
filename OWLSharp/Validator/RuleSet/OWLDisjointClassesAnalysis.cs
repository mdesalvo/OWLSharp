/*
  Copyright 2014-2026 Marco De Salvo
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
    /// <summary>
    /// <para>W3C OWL2 RL/RDF: cax-dw (A-Box shared-ClassAssertion check). The T-Box overlap check against SubClassOf/EquivalentClasses is an OWLSharp extension</para>
    /// </summary>
    internal static class OWLDisjointClassesAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.DisjointClassesAnalysis);
        internal const string rulesugg = "There should not be class expressions belonging at the same time to DisjointClasses and SubClassOf/EquivalentClasses axioms!";
        internal const string rulesugg2 = "There should not be class expressions belonging to a DisjointClasses axiom and having a class assertion on the same individual!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            List<OWLDisjointClasses> disjClassesAxioms = ontology.GetClassAxiomsOfType<OWLDisjointClasses>();
            if (disjClassesAxioms.Count > 0)
            {
                //Temporary working variables
                List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
                //idvsCache memoizes GetIndividualsOf(classExpr) across DisjointClasses axioms sharing the same class expression
                //(expensive per-class scan of clsAsns), keyed by RDFResource.PatternMemberID for O(1) lookup instead of IRI string compare
                Dictionary<long, List<OWLIndividualExpression>> idvsCache = new Dictionary<long, List<OWLIndividualExpression>>();
                //idvsCounter tallies, within the current DisjointClasses axiom only, how many of its member classes each individual
                //belongs to: a count > 1 means the same individual was asserted into two classes stated disjoint -> A-Box contradiction
                Dictionary<long, long> idvsCounter = new Dictionary<long, long>();

                foreach (OWLDisjointClasses disjClasses in ontology.GetClassAxiomsOfType<OWLDisjointClasses>())
                {
                    List<OWLClassExpression> classExpressions = disjClasses.ClassExpressions.ToList();

                    //DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS1,CLS2) -> ERROR
                    //DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> ERROR
                    //DisjointClasses(CLS1,CLS2) ^ EquivalentClasses(CLS1,CLS2) -> ERROR
                    //violatesTBox is checked in the outer loop condition purely as a short-circuit: once one violating pair is found
                    //within this axiom, there is no need to keep scanning the remaining CLS combinations (one issue per axiom is enough)
                    #region T-BOX Analysis
                    bool violatesTBox = false;
                    for (int i = 0; i < classExpressions.Count && !violatesTBox; i++)
                    {
                        OWLClassExpression outerClass = classExpressions[i];
                        for (int j = i + 1; j < classExpressions.Count; j++)
                        {
                            OWLClassExpression innerClass = classExpressions[j];
                            if (!outerClass.GetIRI().Equals(innerClass.GetIRI())
                                 && (ontology.CheckIsSubClassOf(outerClass, innerClass)
                                      || ontology.CheckIsSubClassOf(innerClass, outerClass)
                                      || ontology.CheckAreEquivalentClasses(outerClass, innerClass)))
                            {
                                violatesTBox = true;
                                issues.Add(new OWLIssue(
                                    OWLEnums.OWLIssueSeverity.Error,
                                    rulename,
                                    $"Violated DisjointClasses axiom (T-BOX) with signature: '{disjClasses.GetXML()}'",
                                    rulesugg));
                                break;
                            }
                        }
                    }
                    #endregion

                    //DisjointClasses(CLS1,CLS2) ^ ClassAssertion(CLS1,IDV) ^ ClassAssertion(CLS2,IDV) -> ERROR
                    #region A-BOX Analysis
                    bool violatesABox = false;
                    for (int i = 0; i < classExpressions.Count && !violatesABox; i++)
                    {
                        OWLClassExpression classExpr = classExpressions[i];
                        RDFResource classExprIRI = classExpr.GetIRI();
                        if (!idvsCache.ContainsKey(classExprIRI.PatternMemberID))
                            idvsCache.Add(classExprIRI.PatternMemberID, ontology.GetIndividualsOf(classExpr, clsAsns));
                        foreach (OWLIndividualExpression idvExpr in idvsCache[classExprIRI.PatternMemberID])
                        {
                            RDFResource idvExprIRI = idvExpr.GetIRI();
                            if (!idvsCounter.ContainsKey(idvExprIRI.PatternMemberID))
                                idvsCounter.Add(idvExprIRI.PatternMemberID, 0);
                            if (++idvsCounter[idvExprIRI.PatternMemberID] > 1)
                            {
                                violatesABox = true;
                                issues.Add(new OWLIssue(
                                    OWLEnums.OWLIssueSeverity.Error,
                                    rulename,
                                    $"Violated DisjointClasses axiom (A-BOX) with signature: '{disjClasses.GetXML()}'",
                                    rulesugg2));
                                break;
                            }
                        }
                    }
                    idvsCounter.Clear();
                    #endregion
                }
            }

            return issues;
        }
    }
}