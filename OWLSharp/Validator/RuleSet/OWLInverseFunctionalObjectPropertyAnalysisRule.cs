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
    internal static class OWLInverseFunctionalObjectPropertyAnalysisRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.InverseFunctionalObjectPropertyAnalysis);
        internal const string rulesugg = "There should not be inverse functional object properties linking the same target individual to more than one source individuals within ObjectPropertyAssertion axioms if these source individuals are explicitly different!";
        internal const string rulesugg2 = "There should not be inverse functional object properties also defined as transitive, or having super properties defined as transitive!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            List<OWLInverseFunctionalObjectProperty> ifopAxms = ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>();
            if (ifopAxms.Count > 0)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

                foreach (OWLInverseFunctionalObjectProperty ifop in ifopAxms)
                {
                    #region Recalibration
                    List<OWLObjectPropertyAssertion> ifopAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, ifop.ObjectPropertyExpression);
                    foreach (OWLObjectPropertyAssertion ifopAsn in ifopAsns)
                    {
                        //In case the functional object property works under inverse logic, we must swap source/target of the object assertion
                        if (ifop.ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                        {
                            (ifopAsn.SourceIndividualExpression, ifopAsn.TargetIndividualExpression) = (ifopAsn.TargetIndividualExpression, ifopAsn.SourceIndividualExpression);
                            ifopAsn.ObjectPropertyExpression = objInvOf.ObjectProperty;
                        }
                    }
                    ifopAsns = OWLAxiomHelper.RemoveDuplicates(ifopAsns);
                    #endregion

                    //InverseFunctionalObjectProperty(IFOP) ^ ObjectPropertyAssertion(IFOP,IDV1,IDV2) ^ ObjectPropertyAssertion(IFOP,IDV3,IDV2) ^ DifferentIndividuals(IDV1,IDV3) -> ERROR
                    ifopAsns.GroupBy(opex => opex.TargetIndividualExpression.GetIRI().ToString())
                            .Select(grp => new
                            {
                                IfopAsnSources = grp.Select(g => g.SourceIndividualExpression),
                                FoundDiffFromSources = grp.Select(g => g.SourceIndividualExpression)
                                                          .Any(outerSrcIdv => grp.Select(g => g.SourceIndividualExpression)
                                                                                 .Any(innerSrcIdv => !outerSrcIdv.GetIRI().Equals(innerSrcIdv.GetIRI())
                                                                                                        && ontology.CheckAreDifferentIndividuals(outerSrcIdv, innerSrcIdv)))
                            })
                            .Where(grp => grp.FoundDiffFromSources && grp.IfopAsnSources.Count() > 1)
                            .ToList()
                            .ForEach(ifopAsn =>
                            {
                                issues.Add(new OWLIssue(
                                    OWLEnums.OWLIssueSeverity.Error,
                                    rulename,
                                    $"Violated InverseFunctionalObjectProperty axiom with signature: {ifop.GetXML()}",
                                    rulesugg));
                            });

                    //InverseFunctionalObjectProperty(IFOP) ^ TransitiveObjectProperty(IFOP) -> ERROR
                    //InverseFunctionalObjectProperty(IFOP) ^ SubObjectPropertyOf(IFOP, SOP) ^ TransitiveObjectProperty(SOP) -> ERROR
                    if (ontology.CheckHasTransitiveObjectProperty(ifop.ObjectPropertyExpression)
                         || ontology.GetSuperObjectPropertiesOf(ifop.ObjectPropertyExpression).Any(ontology.CheckHasTransitiveObjectProperty))
                    {
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error,
                            rulename,
                            $"Violated InverseFunctionalObjectProperty axiom with signature: {ifop.GetXML()}",
                            rulesugg2));
                    }
                }
            }

            return issues;
        }
    }
}