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
    internal static class OWLAsymmetricObjectPropertyAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis.ToString();
        internal const string rulesugg1 = "There should not be object properties at the same time asymmetric and symmetric!";
        internal const string rulesugg2 = "There should not be object assertions switching source/target individuals under the same asymmetric object property!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, OWLValidatorContext validatorContext)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLAsymmetricObjectProperty> asymObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>();
            List<OWLSymmetricObjectProperty> symObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>();

            //AsymmetricObjectProperty(OP) ^ SymmetricObjectProperty(OP) -> ERROR
            foreach (OWLAsymmetricObjectProperty asymObjProp in asymObjProps)
            {
                RDFResource asymObjPropIRI = asymObjProp.ObjectPropertyExpression.GetIRI();
                symObjProps.Where(symObjProp => symObjProp.ObjectPropertyExpression.GetIRI().Equals(asymObjPropIRI))
                           .ToList()
                           .ForEach(symObjProp =>
                           {
                               issues.Add(new OWLIssue(
                                   OWLEnums.OWLIssueSeverity.Error,
                                   rulename,
                                   $"Violated AsymmetricObjectProperty axiom with signature: '{asymObjProp.GetXML()}'",
                                   rulesugg1));
                           });
            }

            //AsymmetricObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV1) -> ERROR
            foreach (OWLAsymmetricObjectProperty asymObjProp in asymObjProps)
            {
                OWLObjectProperty asymObjPropInvOfValue = (asymObjProp.ObjectPropertyExpression as OWLObjectInverseOf)?.ObjectProperty;

                #region Recalibration
                List<OWLObjectPropertyAssertion> asymObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(validatorContext.ObjectPropertyAssertions, asymObjProp.ObjectPropertyExpression);
                foreach (OWLObjectPropertyAssertion asymObjPropAsn in asymObjPropAsns)
                {
                    //In case the asymmetric object property works under inverse logic, we must swap source/target of the object assertion
                    if (asymObjPropInvOfValue != null)
                    {
                        (asymObjPropAsn.SourceIndividualExpression, asymObjPropAsn.TargetIndividualExpression) = (asymObjPropAsn.TargetIndividualExpression, asymObjPropAsn.SourceIndividualExpression);
                        asymObjPropAsn.ObjectPropertyExpression = asymObjPropInvOfValue;
                    }
                }
                asymObjPropAsns = OWLAxiomHelper.RemoveDuplicates(asymObjPropAsns);
                #endregion

                if (asymObjPropAsns.Any(outerAsn =>
                        asymObjPropAsns.Any(innerAsn => innerAsn.SourceIndividualExpression.GetIRI().Equals(outerAsn.TargetIndividualExpression.GetIRI())
                                                         && innerAsn.TargetIndividualExpression.GetIRI().Equals(outerAsn.SourceIndividualExpression.GetIRI()))))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated AsymmetricObjectProperty axiom with signature: '{asymObjProp.GetXML()}'",
                        rulesugg2));
                }
            }

            return issues;
        }
    }
}