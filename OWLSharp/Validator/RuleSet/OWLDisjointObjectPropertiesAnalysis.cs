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
    internal static class OWLDisjointObjectPropertiesAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.DisjointObjectPropertiesAnalysis);
        internal const string rulesugg = "There should not be disjoint object properties linking the same source and target individual pairs within ObjectPropertyAssertion axioms!";
        internal const string rulesugg2 = "There should not be object properties belonging at the same time to DisjointObjectProperties and SubObjectPropertyOf/EquivalentObjectProperties axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            List<OWLDisjointObjectProperties> disjObjPropsAxms = ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>();
            if (disjObjPropsAxms.Count > 0)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> disjObPropAsns = new List<OWLObjectPropertyAssertion>();

                //DisjointObjectProperties(OP1,OP2) ^ ObjectPropertyAssertion(OP1,IDV1,IDV2) ^ ObjectPropertyAssertion(OP2,IDV1,IDV2) -> ERROR
                foreach (OWLDisjointObjectProperties disjObProps in disjObjPropsAxms)
                {
                    disjObPropAsns.Clear();
                    foreach (OWLObjectPropertyExpression disjObPropExpr in disjObProps.ObjectPropertyExpressions)
                        disjObPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, disjObPropExpr));

                    disjObPropAsns.GroupBy(opAsn => new {
                                    SrcIdv = opAsn.SourceIndividualExpression.GetIRI().ToString(),
                                    TgtIdv = opAsn.TargetIndividualExpression.GetIRI().ToString() })
                                  .Where(g => g.Count() > 1)
                                  .ToList()
                                  .ForEach(opAsn =>
                                  {
                                      issues.Add(new OWLIssue(
                                          OWLEnums.OWLIssueSeverity.Error,
                                          rulename,
                                          $"Violated DisjointObjectProperties axiom with signature: '{disjObProps.GetXML()}'",
                                          rulesugg));
                                  });

                    //DisjointObjectProperties(OP1,OP2) ^ SubDataPropertyOf(OP1,OP2) -> ERROR
                    //DisjointObjectProperties(OP1,OP2) ^ SubDataPropertyOf(OP2,OP1) -> ERROR
                    //DisjointObjectProperties(OP1,OP2) ^ EquivalentDataProperties(OP1,OP2) -> ERROR
                    if (disjObProps.ObjectPropertyExpressions.Any(outerOP =>
                          disjObProps.ObjectPropertyExpressions.Any(innerOP => !outerOP.GetIRI().Equals(innerOP.GetIRI())
                                                                                   && (ontology.CheckIsSubObjectPropertyOf(outerOP, innerOP)
                                                                                        || ontology.CheckIsSubObjectPropertyOf(innerOP, outerOP)
                                                                                        || ontology.CheckAreEquivalentObjectProperties(outerOP, innerOP)))))
                    {
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error,
                            rulename,
                            $"Violated DisjointObjectProperties axiom with signature: '{disjObProps.GetXML()}'",
                            rulesugg2));
                    }
                }
            }

            return issues;
        }
    }
}