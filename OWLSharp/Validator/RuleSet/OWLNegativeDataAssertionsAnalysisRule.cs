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
    internal static class OWLNegativeDataAssertionsAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.NegativeDataAssertionsAnalysis.ToString();
        internal static readonly string rulesugg = "There should not be data assertions conflicting with negative data assertions!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            List<OWLNegativeDataPropertyAssertion> ndpAsns = ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>();

            //NegativeDataPropertyAssertion(OP,IDV,LIT) ^ DataPropertyAssertion(OP,IDV,LIT) -> ERROR
            foreach (OWLNegativeDataPropertyAssertion ndpAsn in ndpAsns)
            {
                RDFResource ndpAsnIndividualIRI = ndpAsn.IndividualExpression.GetIRI();
                RDFLiteral ndpAsnLiteral = ndpAsn.Literal.GetLiteral();
                OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, ndpAsn.DataProperty)
                                       .Where(dpAsn => dpAsn.IndividualExpression.GetIRI().Equals(ndpAsnIndividualIRI)
                                                         && dpAsn.Literal.GetLiteral().Equals(ndpAsnLiteral))
                                       .ToList()
                                       .ForEach(dpAsn =>
                                       {
                                           issues.Add(new OWLIssue(
                                               OWLEnums.OWLIssueSeverity.Error, 
                                               rulename, 
                                               $"Violated NegativeDataPropertyAssertion axiom with signature: '{ndpAsn.GetXML()}'", 
                                               rulesugg));
                                       });
            }

            return issues;
        }
    }
}