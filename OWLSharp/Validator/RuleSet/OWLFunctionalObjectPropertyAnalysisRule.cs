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
    internal static class OWLFunctionalObjectPropertyAnalysisRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.FunctionalObjectPropertyAnalysis);
        internal const string rulesugg = "There should not be functional object properties linking the same source individual to more than one target individuals within ObjectPropertyAssertion axioms if these target individuals are explicitly different!";
        internal const string rulesugg2 = "There should not be functional object properties also defined as transitive, or having super properties defined as transitive!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, OWLValidatorContext validatorContext)
        {
            List<OWLIssue> issues = [];

            foreach (OWLFunctionalObjectProperty fop in ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>())
            {
                #region Recalibration
                List<OWLObjectPropertyAssertion> fopAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(validatorContext.ObjectPropertyAssertions, fop.ObjectPropertyExpression);
                foreach (OWLObjectPropertyAssertion fopAsn in fopAsns)
                {
                    //In case the functional object property works under inverse logic, we must swap source/target of the object assertion
                    if (fop.ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {
                        (fopAsn.SourceIndividualExpression, fopAsn.TargetIndividualExpression) = (fopAsn.TargetIndividualExpression, fopAsn.SourceIndividualExpression);
                        fopAsn.ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }
                }
                fopAsns = OWLAxiomHelper.RemoveDuplicates(fopAsns);
                #endregion

                //FunctionalObjectProperty(FOP) ^ ObjectPropertyAssertion(FOP,IDV1,IDV2) ^ ObjectPropertyAssertion(FOP,IDV1,IDV3) ^ DifferentIndividuals(IDV2,IDV3) -> ERROR
                fopAsns.GroupBy(opex => opex.SourceIndividualExpression.GetIRI().ToString())
                       .Select(grp => new
                       {
                            FopAsnTargets = grp.Select(g => g.TargetIndividualExpression),
                            FoundDiffFromTargets = grp.Select(g => g.TargetIndividualExpression)
                                                      .Any(outerTgtIdv => grp.Select(g => g.TargetIndividualExpression)
                                                                             .Any(innerTgtIdv => !outerTgtIdv.GetIRI().Equals(innerTgtIdv.GetIRI())
                                                                                                   && ontology.CheckAreDifferentIndividuals(outerTgtIdv, innerTgtIdv)))
                       })
                       .Where(grp => grp.FoundDiffFromTargets && grp.FopAsnTargets.Count() > 1)
                       .ToList()
                       .ForEach(fopAsn =>
                       {
                           issues.Add(new OWLIssue(
                               OWLEnums.OWLIssueSeverity.Error,
                               rulename,
                               $"Violated FunctionalObjectProperty axiom with signature: {fop.GetXML()}",
                               rulesugg));
                       });

                //FunctionalObjectProperty(FOP) ^ TransitiveObjectProperty(FOP) -> ERROR
                //FunctionalObjectProperty(FOP) ^ SubObjectPropertyOf(FOP, SOP) ^ TransitiveObjectProperty(SOP) -> ERROR
                if (ontology.CheckHasTransitiveObjectProperty(fop.ObjectPropertyExpression)
                     || ontology.GetSuperObjectPropertiesOf(fop.ObjectPropertyExpression).Any(ontology.CheckHasTransitiveObjectProperty))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated FunctionalObjectProperty axiom with signature: {fop.GetXML()}",
                        rulesugg2));
            }

            return issues;
        }
    }
}