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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>W3C OWL2 RL/RDF: cls-maxc1, cls-maxc2, cls-maxqc1, cls-maxqc2, cls-maxqc3, cls-maxqc4 (cardinality-violation checks, qualified and unqualified: Error for data values beyond the bound and for object fillers that are N+1 pairwise DifferentIndividuals, Warning for object fillers beyond the bound that the Reasoner will unify since no unique name assumption holds). The SubClassOf vs DisjointClasses overlap check is an OWLSharp extension (Warning: it forces the subclass to be equivalent to owl:Nothing, which is a modeling smell rather than an ontology-level inconsistency). The non-simple object property check on object cardinality restrictions is an OWLSharp extension (OWL2 DL simple-role restriction)</para>
    /// </summary>
    internal static class OWLSubClassOfAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.SubClassOfAnalysis);
        internal const string rulesugg1 = "A class should not be asserted as SubClassOf a class it is also stated DisjointClasses with: this forces the subclass to be equivalent to owl:Nothing!";
        internal const string rulesugg2 = "There should not be individuals violating ObjectExactCardinality or ObjectMaxCardinality constraints!";
        internal const string rulesugg3 = "There should not be individuals violating DataExactCardinality or DataMaxCardinality constraints!";
        internal const string rulesugg4 = "Individuals having more object fillers than allowed by ObjectExactCardinality or ObjectMaxCardinality constraints should declare them as DifferentIndividuals if they are meant to be distinct: otherwise the Reasoner will unify them!";
        internal const string rulesugg5 = "There should not be ObjectMinCardinality, ObjectMaxCardinality or ObjectExactCardinality constraints on non-simple object properties (transitive ones or ones having transitive or chain-defined sub properties)!";
        //Exhaustive search of pairwise different fillers explores subsets of N+1 elements: beyond this bound it would grow too much
        internal const int maxCardinalityExactSearchLimit = 3;

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
                //owl:Nothing is the bottom class: it is a subclass of every class and disjoint with every class
                //(the Reasoner entails both), so for it the overlap is the normal state and not a modeling smell
                if (!(subClassOf.SubClassExpression is OWLClass subClassAsNamedClass && subClassAsNamedClass.GetIRI().Equals(RDFVocabulary.OWL.NOTHING))
                     && ontology.CheckAreDisjointClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                        rulesugg1));
                }

                //SubClassOf(CLS,ObjectMinCardinality(OP,N)) ^ OP non-simple -> ERROR
                //SubClassOf(CLS,ObjectMaxCardinality(OP,N)) ^ OP non-simple -> ERROR
                //SubClassOf(CLS,ObjectExactCardinality(OP,N)) ^ OP non-simple -> ERROR
                //The OWL2 DL simple-role restriction is structural: it holds even when the class has no individuals
                OWLObjectPropertyExpression cardinalityObjectProperty = null;
                if (subClassOf.SuperClassExpression is OWLObjectMinCardinality minCardinality)
                {
                    //Lower bound over the fillers of the property
                    cardinalityObjectProperty = minCardinality.ObjectPropertyExpression;
                }
                else if (subClassOf.SuperClassExpression is OWLObjectMaxCardinality maxCardinality)
                {
                    //Upper bound over the fillers of the property
                    cardinalityObjectProperty = maxCardinality.ObjectPropertyExpression;
                }
                else if (subClassOf.SuperClassExpression is OWLObjectExactCardinality exactCardinality)
                {
                    //Lower and upper bound at once over the fillers of the property
                    cardinalityObjectProperty = exactCardinality.ObjectPropertyExpression;
                }
                //Any other superclass leaves the property null: it carries no object cardinality to restrict
                //(the null property is never non-simple)
                if (ontology.CheckIsNonSimpleObjectProperty(cardinalityObjectProperty))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                        rulesugg5));
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
                            //Exact cardinality bounds the fillers from above just like a maximum does: its lower bound is out of scope here
                            case OWLObjectExactCardinality objExactCardinality:
                            {
                                OWLIssue objExactCardinalityIssue = AnalyzeObjectCardinality(ontology, subClassOf, objExactCardinality.ObjectPropertyExpression,
                                    objExactCardinality.ClassExpression, objExactCardinality.Cardinality, individualIRI, opAsns, clsAsns, individualsCache);
                                if (objExactCardinalityIssue != null)
                                    issues.Add(objExactCardinalityIssue);
                                break;
                            }
                            //Maximum cardinality bounds the (qualified) fillers of the property for the individuals of the subclass
                            case OWLObjectMaxCardinality objMaxCardinality:
                            {
                                OWLIssue objMaxCardinalityIssue = AnalyzeObjectCardinality(ontology, subClassOf, objMaxCardinality.ObjectPropertyExpression,
                                    objMaxCardinality.ClassExpression, objMaxCardinality.Cardinality, individualIRI, opAsns, clsAsns, individualsCache);
                                if (objMaxCardinalityIssue != null)
                                    issues.Add(objMaxCardinalityIssue);
                                break;
                            }
                            //Exact cardinality bounds the values from above just like a maximum does: its lower bound is out of scope here
                            case OWLDataExactCardinality dtExactCardinality:
                            {
                                #region Qualified
                                string qDataRangeIRI = dtExactCardinality.DataRangeExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qDataRangeIRI);
                                #endregion

                                RDFResource dtExactCardinalityIRI = dtExactCardinality.DataProperty.GetIRI();

                                //Distinct literals are distinct values (no unique name assumption applies to data), so only their number matters:
                                //the same literal asserted twice is one value and the string form keeps lexical form, language and datatype apart
                                HashSet<string> distinctLiterals = new HashSet<string>(StringComparer.Ordinal);
                                foreach (OWLDataPropertyAssertion dpAsn in dpAsns)
                                {
                                    if (dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
                                         && dpAsn.DataProperty.GetIRI().Equals(dtExactCardinalityIRI)
                                         && (!isQualified || ontology.CheckIsLiteralOf(dtExactCardinality.DataRangeExpression, dpAsn.Literal)))
                                    {
                                        distinctLiterals.Add(dpAsn.Literal.GetLiteral().ToString());
                                    }
                                }

                                //More distinct values than the upper bound allows can never be reconciled: inconsistency
                                if (distinctLiterals.Count > int.Parse(dtExactCardinality.Cardinality, CultureInfo.InvariantCulture))
                                {
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error,
                                        rulename,
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                                        rulesugg3));
                                }
                                break;
                            }
                            //Maximum cardinality bounds the (qualified) values of the data property for the individuals of the subclass
                            case OWLDataMaxCardinality dtMaxCardinality:
                            {
                                #region Qualified
                                string qDataRangeIRI = dtMaxCardinality.DataRangeExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qDataRangeIRI);
                                #endregion

                                RDFResource dtMaxCardinalityIRI = dtMaxCardinality.DataProperty.GetIRI();

                                //Distinct literals are distinct values (no unique name assumption applies to data), so only their number matters:
                                //the same literal asserted twice is one value and the string form keeps lexical form, language and datatype apart
                                HashSet<string> distinctLiterals = new HashSet<string>(StringComparer.Ordinal);
                                foreach (OWLDataPropertyAssertion dpAsn in dpAsns)
                                {
                                    if (dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
                                         && dpAsn.DataProperty.GetIRI().Equals(dtMaxCardinalityIRI)
                                         && (!isQualified || ontology.CheckIsLiteralOf(dtMaxCardinality.DataRangeExpression, dpAsn.Literal)))
                                    {
                                        distinctLiterals.Add(dpAsn.Literal.GetLiteral().ToString());
                                    }
                                }

                                //More distinct values than the upper bound allows can never be reconciled: inconsistency
                                if (distinctLiterals.Count > int.Parse(dtMaxCardinality.Cardinality, CultureInfo.InvariantCulture))
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

        /// <summary>
        /// Builds the issue (if any) for an individual having more distinct fillers of the given object property expression than the given upper bound allows
        /// </summary>
        private static OWLIssue AnalyzeObjectCardinality(OWLOntology ontology, OWLSubClassOf subClassOf, OWLObjectPropertyExpression objectPropertyExpression,
            OWLClassExpression qualifierExpression, string cardinality, RDFResource individualIRI, List<OWLObjectPropertyAssertion> opAsns,
            List<OWLClassAssertion> clsAsns, Dictionary<string, List<OWLIndividualExpression>> individualsCache)
        {
            //An owl:Thing qualifier excludes no filler, so it is handled as the unqualified restriction
            bool isQualified = qualifierExpression != null
                                && !(qualifierExpression is OWLClass qualifierClass && qualifierClass.GetIRI().Equals(RDFVocabulary.OWL.THING));

            //Under a qualified restriction only the fillers belonging to the qualifying class are bounded
            string qualifierIRI = isQualified ? qualifierExpression.GetIRI().ToString() : null;
            if (isQualified && !individualsCache.ContainsKey(qualifierIRI))
                individualsCache.Add(qualifierIRI, ontology.GetIndividualsOf(qualifierExpression, clsAsns, false));

            //ObjectInverseOf(OP) bounds the individuals pointing to the constrained one, not those it points to
            bool isInverse = objectPropertyExpression is OWLObjectInverseOf;

            //Fillers asserted for the same individual are grouped in equality classes: the same filler asserted twice
            //or fillers related by SameIndividual denote one individual only, so they must count once
            Dictionary<string, List<OWLIndividualExpression>> equalityClasses = new Dictionary<string, List<OWLIndividualExpression>>();
            foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, objectPropertyExpression))
            {
                //Under ObjectInverseOf(OP) the constrained individual is the target of the assertion and its filler the source
                OWLIndividualExpression constrainedIndividual = isInverse ? opAsn.TargetIndividualExpression : opAsn.SourceIndividualExpression;
                OWLIndividualExpression fillerIndividual = isInverse ? opAsn.SourceIndividualExpression : opAsn.TargetIndividualExpression;

                //Assertions of other individuals do not concern the one under analysis
                if (!constrainedIndividual.GetIRI().Equals(individualIRI))
                    continue;

                //A filler outside the qualifying class is not bounded by the restriction
                RDFResource fillerIRI = fillerIndividual.GetIRI();
                if (isQualified && !individualsCache[qualifierIRI].Any(qualifierMember => qualifierMember.GetIRI().Equals(fillerIRI)))
                    continue;

                //The smallest IRI among the filler and its same individuals names the equality class: it is the same for every member of the class
                string equalityClassKey = ontology.GetSameIndividuals(fillerIndividual)
                                                  .Select(sameIndividual => sameIndividual.GetIRI().ToString())
                                                  .Append(fillerIRI.ToString())
                                                  .OrderBy(iri => iri, StringComparer.Ordinal)
                                                  .First();
                if (!equalityClasses.ContainsKey(equalityClassKey))
                    equalityClasses.Add(equalityClassKey, new List<OWLIndividualExpression>());
                equalityClasses[equalityClassKey].Add(fillerIndividual);
            }

            //Not more equality classes than the upper bound allows: the restriction is satisfied
            int upperBound = int.Parse(cardinality, CultureInfo.InvariantCulture);
            if (equalityClasses.Count <= upperBound)
                return null;

            //N+1 pairwise different equality classes can never be unified down to N: inconsistency
            //(the search is exact for small bounds only, so beyond them the finding stays a Warning)
            if (upperBound <= maxCardinalityExactSearchLimit
                 && ContainsPairwiseDifferentClasses(ontology, equalityClasses.Values.ToList(), upperBound + 1))
            {
                return new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'",
                    rulesugg2);
            }

            //No unique name assumption in OWL: the Reasoner will unify the exceeding fillers unless they are declared different
            return new OWLIssue(
                OWLEnums.OWLIssueSeverity.Warning,
                rulename,
                $"Unifiable fillers under SubClassOf axiom with signature: '{subClassOf.GetXML()}' (the Reasoner will unify them unless they are declared as DifferentIndividuals)",
                rulesugg4);
        }

        /// <summary>
        /// Checks whether the given equality classes contain a selection of the given size made of classes that are pairwise declared different
        /// </summary>
        private static bool ContainsPairwiseDifferentClasses(OWLOntology ontology, List<List<OWLIndividualExpression>> equalityClasses, int selectionSize)
        {
            //Two equality classes are different when any member of one is declared different from any member of the other;
            //pairs are memoized because the backtracking below asks for the same pair along several candidate selections
            Dictionary<long, bool> differenceCache = new Dictionary<long, bool>();
            bool AreDifferentClasses(int leftIndex, int rightIndex)
            {
                long pairKey = (long)leftIndex * equalityClasses.Count + rightIndex;
                if (!differenceCache.TryGetValue(pairKey, out bool areDifferent))
                {
                    areDifferent = equalityClasses[leftIndex].Any(leftMember => equalityClasses[rightIndex].Any(rightMember => ontology.CheckAreDifferentIndividuals(leftMember, rightMember)));
                    differenceCache.Add(pairKey, areDifferent);
                }
                return areDifferent;
            }

            //Backtracking extends the selection one class at a time: only classes different from all the already selected ones can join it
            bool ExtendSelection(List<int> selectedIndexes, int firstCandidateIndex)
            {
                //A complete selection proves the existence of enough pairwise different classes
                if (selectedIndexes.Count == selectionSize)
                    return true;

                for (int candidateIndex = firstCandidateIndex; candidateIndex < equalityClasses.Count; candidateIndex++)
                {
                    //A candidate equal-or-unknown to a selected class cannot join: the pairwise difference would not hold
                    if (!selectedIndexes.All(selectedIndex => AreDifferentClasses(selectedIndex, candidateIndex)))
                        continue;

                    selectedIndexes.Add(candidateIndex);
                    if (ExtendSelection(selectedIndexes, candidateIndex + 1))
                        return true;

                    //The candidate led to no complete selection: it is dropped to try the following ones
                    selectedIndexes.RemoveAt(selectedIndexes.Count - 1);
                }
                return false;
            }

            return ExtendSelection(new List<int>(), 0);
        }
    }
}