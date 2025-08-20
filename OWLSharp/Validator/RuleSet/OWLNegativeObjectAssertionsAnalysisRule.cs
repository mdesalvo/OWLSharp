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
    internal static class OWLNegativeObjectAssertionsAnalysisRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.NegativeObjectAssertionsAnalysis);
        internal const string rulesugg = "There should not be object assertions conflicting with negative object assertions!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, OWLValidatorContext validatorContext)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables


            //NegativeObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ERROR
            foreach (OWLNegativeObjectPropertyAssertion nopAsn in OWLAssertionAxiomHelper.CalibrateNegativeObjectAssertions(ontology))
            {
                RDFResource ndpAsnSourceIndividualIRI = nopAsn.SourceIndividualExpression.GetIRI();
                RDFResource ndpAsnTargetIndividualIRI = nopAsn.TargetIndividualExpression.GetIRI();
                OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(validatorContext.ObjectPropertyAssertions, nopAsn.ObjectPropertyExpression)
                                       .Where(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(ndpAsnSourceIndividualIRI)
                                                        && opAsn.TargetIndividualExpression.GetIRI().Equals(ndpAsnTargetIndividualIRI))
                                       .ToList()
                                       .ForEach(opAsn =>
                                       {
                                           issues.Add(new OWLIssue(
                                               OWLEnums.OWLIssueSeverity.Error,
                                               rulename,
                                               $"Violated NegativeObjectPropertyAssertion axiom with signature: '{nopAsn.GetXML()}'",
                                               rulesugg));
                                       });
            }

            return issues;
        }
    }
}