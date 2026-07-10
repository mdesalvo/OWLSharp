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

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>W3C OWL2 RL/RDF: cls-maxc1, cls-maxc2, cls-maxqc1, cls-maxqc2, cls-maxqc3, cls-maxqc4 (cardinality-violation checks, qualified and unqualified). The SubClassOf vs DisjointClasses overlap check is an OWLSharp extension (Warning: it forces the subclass to be equivalent to owl:Nothing, which is a modeling smell rather than an ontology-level inconsistency)</para>
    /// </summary>
    internal static class OWLSubClassOfAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.SubClassOfAnalysis);
        internal const string rulesugg1 = "A class should not be asserted as SubClassOf a class it is also stated DisjointClasses with: this forces the subclass to be equivalent to owl:Nothing!";
        internal const string rulesugg2 = "There should not be individuals violating ObjectExactCardinality or ObjectMaxCardinality constraints!";
        internal const string rulesugg3 = "There should not be individuals violating DataExactCardinality or DataMaxCardinality constraints!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            Dictionary<string, List<OWLIndividualExpression>> individualsCache = new Dictionary<string, List<OWLIndividualExpression>>();

            foreach (OWLSubClassOf subClassOf in ontology.GetClassAxiomsOfType<OWLSubClassOf>())
            {
                //SubClassOf(CLS1,CLS2) ^ DisjointClasses(CLS1,CLS2) -> WARNING
                //NOTE: SubClassOf(CLS1,CLS2) combined with the mutual SubClassOf(CLS2,CLS1) or with EquivalentClasses(CLS1,CLS2) is NOT
                //flagged: both are just redundant (not contradictory) restatements of CLS1=CLS2, and mutual SubClassOf in particular is a
                //common, deliberate idiom for expressing class equivalence without an explicit EquivalentClasses axiom
                if (ontology.CheckAreDisjointClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                        rulesugg1));
                }

                //SubClassOf(CLS,[Object|Data][Exact|Max]Cardinality(P,N)
                if (subClassOf.SuperClassExpression is OWLObjectExactCardinality
                     || subClassOf.SuperClassExpression is OWLObjectMaxCardinality
                     || subClassOf.SuperClassExpression is OWLDataExactCardinality
                     || subClassOf.SuperClassExpression is OWLDataMaxCardinality)
                {
                    //Materialize individuals of the subclass (cached because the same class can recur as subclass across
                    //several SubClassOf axioms, and computing its individuals is expensive)
                    string subClassIRI = subClassOf.SubClassExpression.GetIRI().ToString();
                    if (!individualsCache.ContainsKey(subClassIRI))
                        individualsCache.Add(subClassIRI, ontology.GetIndividualsOf(subClassOf.SubClassExpression, clsAsns, false));

                    //Filter assertions of the current individual, depending on the nature of the superclass
                    foreach (OWLIndividualExpression individual in individualsCache[subClassIRI])
                    {
                        RDFResource individualIRI = individual.GetIRI();
                        switch (subClassOf.SuperClassExpression)
                        {
                            case OWLObjectExactCardinality objExactCardinality:
                            {
                                #region Qualified
                                //A qualified cardinality restriction (onClass present) only counts assertions whose target
                                //individual belongs to that class; an unqualified one counts all assertions on the property
                                string qClassIRI = objExactCardinality.ClassExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qClassIRI);
                                if (isQualified)
                                {
                                    //Materialize individuals of the qualified class
                                    if (!individualsCache.ContainsKey(qClassIRI))
                                        individualsCache.Add(qClassIRI, ontology.GetIndividualsOf(objExactCardinality.ClassExpression, clsAsns, false));
                                }
                                #endregion

                                RDFResource objExactCardinalityIRI = objExactCardinality.ObjectPropertyExpression.GetIRI();
                                int assertionsCount = 0;
                                foreach (OWLObjectPropertyAssertion opAsn in opAsns)
                                {
                                    //Only relevant when qualified: check whether this assertion's target is one of the
                                    //individuals belonging to the restriction's onClass
                                    bool opAsnIdvFoundAsQualifiedTarget = false;
                                    if (isQualified)
                                    {
                                        RDFResource opAsnTargetIndividualIRI = opAsn.TargetIndividualExpression.GetIRI();
                                        foreach (OWLIndividualExpression idv in individualsCache[qClassIRI])
                                            if (idv.GetIRI().Equals(opAsnTargetIndividualIRI))
                                            {
                                                opAsnIdvFoundAsQualifiedTarget = true;
                                                break;
                                            }
                                    }

                                    if (opAsn.SourceIndividualExpression.GetIRI().Equals(individualIRI)
                                         && opAsn.ObjectPropertyExpression.GetIRI().Equals(objExactCardinalityIRI)
                                         && (!isQualified || opAsnIdvFoundAsQualifiedTarget))
                                    {
                                        assertionsCount++;
                                    }
                                }

                                if (assertionsCount > int.Parse(objExactCardinality.Cardinality))
                                {
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error,
                                        rulename,
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                                        rulesugg2));
                                }
                                break;
                            }
                            case OWLObjectMaxCardinality objMaxCardinality:
                            {
                                #region Qualified
                                string qClassIRI = objMaxCardinality.ClassExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qClassIRI);
                                if (isQualified)
                                {
                                    //Materialize individuals of the qualified class
                                    if (!individualsCache.ContainsKey(qClassIRI))
                                        individualsCache.Add(qClassIRI, ontology.GetIndividualsOf(objMaxCardinality.ClassExpression, clsAsns, false));
                                }
                                #endregion

                                RDFResource objMaxCardinalityIRI = objMaxCardinality.ObjectPropertyExpression.GetIRI();
                                int assertionsCount = 0;
                                foreach (OWLObjectPropertyAssertion opAsn in opAsns)
                                {
                                    bool opAsnIdvFoundAsQualifiedTarget = false;
                                    if (isQualified)
                                    {
                                        RDFResource opAsnTargetIndividualIRI = opAsn.TargetIndividualExpression.GetIRI();
                                        foreach (OWLIndividualExpression idv in individualsCache[qClassIRI])
                                            if (idv.GetIRI().Equals(opAsnTargetIndividualIRI))
                                            {
                                                opAsnIdvFoundAsQualifiedTarget = true;
                                                break;
                                            }
                                    }

                                    if (opAsn.SourceIndividualExpression.GetIRI().Equals(individualIRI)
                                         && opAsn.ObjectPropertyExpression.GetIRI().Equals(objMaxCardinalityIRI)
                                         && (!isQualified || opAsnIdvFoundAsQualifiedTarget))
                                    {
                                        assertionsCount++;
                                    }
                                }

                                if (assertionsCount > int.Parse(objMaxCardinality.Cardinality))
                                {
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error,
                                        rulename,
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                                        rulesugg2));
                                }
                                break;
                            }
                            case OWLDataExactCardinality dtExactCardinality:
                            {
                                #region Qualified
                                string qDataRangeIRI = dtExactCardinality.DataRangeExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qDataRangeIRI);
                                #endregion

                                RDFResource dtExactCardinalityIRI = dtExactCardinality.DataProperty.GetIRI();
                                int assertionsCount = 0;
                                foreach (OWLDataPropertyAssertion dpAsn in dpAsns)
                                {
                                    if (dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
                                         && dpAsn.DataProperty.GetIRI().Equals(dtExactCardinalityIRI)
                                         && (!isQualified || ontology.CheckIsLiteralOf(dtExactCardinality.DataRangeExpression, dpAsn.Literal)))
                                    {
                                        assertionsCount++;
                                    }
                                }

                                if (assertionsCount > int.Parse(dtExactCardinality.Cardinality))
                                {
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error,
                                        rulename,
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                                        rulesugg3));
                                }
                                break;
                            }
                            case OWLDataMaxCardinality dtMaxCardinality:
                            {
                                #region Qualified
                                string qDataRangeIRI = dtMaxCardinality.DataRangeExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qDataRangeIRI);
                                #endregion

                                RDFResource dtMaxCardinalityIRI = dtMaxCardinality.DataProperty.GetIRI();
                                int assertionsCount = 0;
                                foreach (OWLDataPropertyAssertion dpAsn in dpAsns)
                                {
                                    if (dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
                                         && dpAsn.DataProperty.GetIRI().Equals(dtMaxCardinalityIRI)
                                         && (!isQualified || ontology.CheckIsLiteralOf(dtMaxCardinality.DataRangeExpression, dpAsn.Literal)))
                                    {
                                        assertionsCount++;
                                    }
                                }

                                if (assertionsCount > int.Parse(dtMaxCardinality.Cardinality))
                                {
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error,
                                        rulename,
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                                        rulesugg3));
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return issues;
        }
    }
}