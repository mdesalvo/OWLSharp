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
    internal static class OWLSubClassOfAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.SubClassOfAnalysis.ToString();
        internal const string rulesugg1 = "There should not be class expressions belonging at the same time to SubClassOf and EquivalentClasses/DisjointClasses axioms!";
        internal const string rulesugg2 = "There should not be individuals violating ObjectExactCardinality or ObjectMaxCardinality constraints!";
        internal const string rulesugg3 = "There should not be individuals violating DataExactCardinality or DataMaxCardinality constraints!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            Dictionary<string, List<OWLIndividualExpression>> individualsCache = new Dictionary<string, List<OWLIndividualExpression>>();

            foreach (OWLSubClassOf subClassOf in ontology.GetClassAxiomsOfType<OWLSubClassOf>())
            {
                //SubClassOf(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> ERROR
                //SubClassOf(CLS1,CLS2) ^ EquivalentClasses(CLS1,CLS2) -> ERROR
                //SubClassOf(CLS1,CLS2) ^ DisjointClasses(CLS1,CLS2) -> ERROR
                if (ontology.CheckIsSubClassOf(subClassOf.SuperClassExpression, subClassOf.SubClassExpression)
                     || ontology.CheckAreEquivalentClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression)
                     || ontology.CheckAreDisjointClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error, 
                        rulename, 
                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
                        rulesugg1));

                //SubClassOf(CLS,[Object|Data][Exact|Max]Cardinality(P,N)
                if (subClassOf.SuperClassExpression is OWLObjectExactCardinality
                     || subClassOf.SuperClassExpression is OWLObjectMaxCardinality
                     || subClassOf.SuperClassExpression is OWLDataExactCardinality
                     || subClassOf.SuperClassExpression is OWLDataMaxCardinality)
                {
                    //Materialize individuals of the subclass
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
                                string qClassIRI = objExactCardinality.ClassExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qClassIRI);
                                if (isQualified)
                                {
                                    //Materialize individuals of the qualified class                                
                                    if (!individualsCache.ContainsKey(qClassIRI))
                                        individualsCache.Add(qClassIRI, ontology.GetIndividualsOf(objExactCardinality.ClassExpression, clsAsns, false));
                                }
                                #endregion

                                List<OWLIndividualExpression> qClassIdvs = isQualified ? individualsCache[qClassIRI] : null;
                                RDFResource objExactCardinalityIRI = objExactCardinality.ObjectPropertyExpression.GetIRI();
                                int asnsCount = opAsns.Count(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(individualIRI)
                                                                      && opAsn.ObjectPropertyExpression.GetIRI().Equals(objExactCardinalityIRI)
                                                                      && (!isQualified || qClassIdvs.Any(idv => idv.GetIRI().Equals(opAsn.TargetIndividualExpression.GetIRI()))));
                                if (asnsCount > int.Parse(objExactCardinality.Cardinality))
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error, 
                                        rulename, 
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
                                        rulesugg2));
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

                                List<OWLIndividualExpression> qClassIdvs = isQualified ? individualsCache[qClassIRI] : null;
                                RDFResource objMaxCardinalityIRI = objMaxCardinality.ObjectPropertyExpression.GetIRI();
                                int asnsCount = opAsns.Count(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(individualIRI)
                                                                      && opAsn.ObjectPropertyExpression.GetIRI().Equals(objMaxCardinalityIRI)
                                                                      && (!isQualified || qClassIdvs.Any(idv => idv.GetIRI().Equals(opAsn.TargetIndividualExpression.GetIRI()))));
                                if (asnsCount > int.Parse(objMaxCardinality.Cardinality))
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error, 
                                        rulename, 
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
                                        rulesugg2));
                                break;
                            }
                            case OWLDataExactCardinality dtExactCardinality:
                            {
                                #region Qualified
                                string qDataRangeIRI = dtExactCardinality.DataRangeExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qDataRangeIRI);
                                #endregion

                                RDFResource dtExactCardinalityIRI = dtExactCardinality.DataProperty.GetIRI();
                                int asnsCount = dpAsns.Count(dpAsn => dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
                                                                      && dpAsn.DataProperty.GetIRI().Equals(dtExactCardinalityIRI)
                                                                      && (!isQualified || ontology.CheckIsLiteralOf(dtExactCardinality.DataRangeExpression, dpAsn.Literal)));
                                if (asnsCount > int.Parse(dtExactCardinality.Cardinality))
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error, 
                                        rulename, 
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
                                        rulesugg3));
                                break;
                            }
                            case OWLDataMaxCardinality dtMaxCardinality:
                            {
                                #region Qualified
                                string qDataRangeIRI = dtMaxCardinality.DataRangeExpression?.GetIRI().ToString();
                                bool isQualified = !string.IsNullOrEmpty(qDataRangeIRI);
                                #endregion

                                RDFResource dtMaxCardinalityIRI = dtMaxCardinality.DataProperty.GetIRI();
                                int asnsCount = dpAsns.Count(dpAsn => dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
                                                                      && dpAsn.DataProperty.GetIRI().Equals(dtMaxCardinalityIRI)
                                                                      && (!isQualified || ontology.CheckIsLiteralOf(dtMaxCardinality.DataRangeExpression, dpAsn.Literal)));
                                if (asnsCount > int.Parse(dtMaxCardinality.Cardinality))
                                    issues.Add(new OWLIssue(
                                        OWLEnums.OWLIssueSeverity.Error, 
                                        rulename, 
                                        $"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
                                        rulesugg3));
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