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
    /// <para>W3C OWL2 RL/RDF: cax-dw (A-Box shared-ClassAssertion check, Error: a genuine ontology inconsistency). The T-Box overlap check
    /// against SubClassOf/EquivalentClasses is an OWLSharp extension (Warning, not Error: the clash only forces the involved classes to
    /// be equivalent to owl:Nothing, which is a modeling smell rather than an ontology-level inconsistency by itself)</para>
    /// </summary>
    internal static class OWLDisjointClassesAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.DisjointClassesAnalysis);
        internal const string rulesugg = "Class expressions should not belong at the same time to DisjointClasses and SubClassOf/EquivalentClasses axioms: this forces them to be equivalent to owl:Nothing!";
        internal const string rulesugg2 = "There should not be class expressions belonging to a DisjointClasses axiom and having a class assertion on the same individual!";
        internal const string rulesugg3 = "There should not be classes that are sub classes of two classes stated disjoint: this forces them to be equivalent to owl:Nothing!";

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
                //subClassesCache memoizes GetSubClassesOf(classExpr) across DisjointClasses axioms sharing the same class expression
                Dictionary<long, List<OWLClassExpression>> subClassesCache = new Dictionary<long, List<OWLClassExpression>>();

                foreach (OWLDisjointClasses disjClasses in ontology.GetClassAxiomsOfType<OWLDisjointClasses>())
                {
                    List<OWLClassExpression> classExpressions = disjClasses.ClassExpressions.ToList();

                    //DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS1,CLS2) -> WARNING
                    //DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> WARNING
                    //DisjointClasses(CLS1,CLS2) ^ EquivalentClasses(CLS1,CLS2) -> WARNING
                    //Warning, not Error: this combination only forces the involved classes to be equivalent to owl:Nothing (an
                    //unsatisfiable-class modeling smell), not an ontology-level inconsistency -- that only follows if some individual
                    //is actually (or entailed to be) a member of one of them, which is exactly what the A-BOX Analysis below checks.
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
                                    OWLEnums.OWLIssueSeverity.Warning,
                                    rulename,
                                    $"Violated DisjointClasses axiom (T-BOX) with signature: '{disjClasses.GetXML()}'",
                                    rulesugg));
                                break;
                            }
                        }
                    }
                    #endregion

                    //SubClassOf*(CLS,CLS1) ^ SubClassOf*(CLS,CLS2) ^ DisjointClasses(CLS1,CLS2) -> WARNING
                    //Warning, not Error: such a class is equivalent to owl:Nothing (a modeling smell) and only its individuals would make the ontology inconsistent
                    #region Unsatisfiable Classes
                    //A class below several disjoint pairs of the same axiom is reported once: the issue is per (axiom,class)
                    HashSet<long> unsatisfiableClassIDs = new HashSet<long>();
                    for (int i = 0; i < classExpressions.Count; i++)
                    {
                        OWLClassExpression outerClass = classExpressions[i];
                        RDFResource outerClassIRI = outerClass.GetIRI();
                        if (!subClassesCache.ContainsKey(outerClassIRI.PatternMemberID))
                            subClassesCache.Add(outerClassIRI.PatternMemberID, ontology.GetSubClassesOf(outerClass));

                        for (int j = i + 1; j < classExpressions.Count; j++)
                        {
                            OWLClassExpression innerClass = classExpressions[j];
                            RDFResource innerClassIRI = innerClass.GetIRI();
                            if (!subClassesCache.ContainsKey(innerClassIRI.PatternMemberID))
                                subClassesCache.Add(innerClassIRI.PatternMemberID, ontology.GetSubClassesOf(innerClass));

                            //A class below both members of the pair (and being none of them) can have no individuals;
                            //owl:Nothing is below every class once the schema is reasoned and being empty is its very definition
                            foreach (OWLClassExpression commonSubClass in subClassesCache[outerClassIRI.PatternMemberID]
                                                                           .Where(outerSubClass => !outerSubClass.GetIRI().Equals(outerClassIRI)
                                                                                                    && !outerSubClass.GetIRI().Equals(innerClassIRI)
                                                                                                    && !outerSubClass.GetIRI().Equals(RDFVocabulary.OWL.NOTHING)
                                                                                                    && subClassesCache[innerClassIRI.PatternMemberID].Any(innerSubClass => innerSubClass.GetIRI().Equals(outerSubClass.GetIRI()))))
                            {
                                if (unsatisfiableClassIDs.Add(commonSubClass.GetIRI().PatternMemberID))
                                {
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Warning,
                                        rulename,
                                        $"Violated DisjointClasses axiom (T-BOX) with signature: '{disjClasses.GetXML()}' (unsatisfiable class: '{commonSubClass.GetIRI()}')",
                                        rulesugg3));
                                }
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