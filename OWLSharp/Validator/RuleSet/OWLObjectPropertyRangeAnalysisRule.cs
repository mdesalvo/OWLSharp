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
    internal static class OWLObjectPropertyRangeAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.ObjectPropertyRangeAnalysis.ToString();
        internal const string rulesugg = "There should not be individuals explicitly incompatible with range class of object properties within ObjectPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, OWLValidatorContext validatorContext)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();

            //ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyRange(OP,C) ^ ClassAssertion(ObjectComplementOf(C),IDV2) -> ERROR
            foreach (OWLObjectPropertyRange opRange in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>())
            {
                bool isObjectInverseOf = opRange.ObjectPropertyExpression is OWLObjectInverseOf;
                foreach (OWLObjectPropertyAssertion opRangeAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(validatorContext.ObjectPropertyAssertions, opRange.ObjectPropertyExpression))
                    if (ontology.CheckIsNegativeIndividualOf(opRange.ClassExpression, isObjectInverseOf ? opRangeAsn.SourceIndividualExpression : opRangeAsn.TargetIndividualExpression, clsAsns))
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error, 
                            rulename, 
                            $"Violated ObjectPropertyRange axiom with signature: {opRange.GetXML()}", 
                            rulesugg));
            }

            return issues;
        }
    }
}