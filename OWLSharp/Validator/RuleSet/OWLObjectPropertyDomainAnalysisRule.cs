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

namespace OWLSharp.Validator
{
    internal static class OWLObjectPropertyDomainAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.ObjectPropertyDomainAnalysis.ToString();
        internal const string rulesugg = "There should not be individuals explicitly incompatible with domain class of object properties within ObjectPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, Dictionary<string, object> validatorCache)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();

            //ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyDomain(OP,C) ^ ClassAssertion(ObjectComplementOf(C),IDV1) -> ERROR
            foreach (OWLObjectPropertyDomain opDomain in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>())
            {
                bool isObjectInverseOf = opDomain.ObjectPropertyExpression is OWLObjectInverseOf;
                foreach (OWLObjectPropertyAssertion opDomainAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX((List<OWLObjectPropertyAssertion>)validatorCache["OPASN"], opDomain.ObjectPropertyExpression))
                    if (ontology.CheckIsNegativeIndividualOf(opDomain.ClassExpression,
                            isObjectInverseOf ? opDomainAsn.TargetIndividualExpression : opDomainAsn.SourceIndividualExpression, clsAsns))
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error,
                            rulename,
                            $"Violated ObjectPropertyDomain axiom with signature: {opDomain.GetXML()}",
                            rulesugg));
            }

            return issues;
        }
    }
}