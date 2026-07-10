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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>W3C OWL2 RL/RDF: prp-asyp (assertion-pair violation check). The AsymmetricObjectProperty+SymmetricObjectProperty overlap check is an OWLSharp extension</para>
    /// </summary>
    internal static class OWLAsymmetricObjectPropertyAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis);
        internal const string rulesugg1 = "There should not be object properties at the same time asymmetric and symmetric!";
        internal const string rulesugg2 = "There should not be object assertions switching source/target individuals under the same asymmetric object property!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
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
                //AsymmetricObjectProperty(ObjectInverseOf(OP)) is legal OWL2: the axiom is really about OP itself (seen from the
                //opposite direction), so the assertions to check must be OP's, not the inverse expression's
                OWLObjectProperty asymObjPropInvOfValue = (asymObjProp.ObjectPropertyExpression as OWLObjectInverseOf)?.ObjectProperty;

                #region Recalibration
                List<OWLObjectPropertyAssertion> asymObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, asymObjProp.ObjectPropertyExpression);
                foreach (OWLObjectPropertyAssertion asymObjPropAsn in asymObjPropAsns)
                {
                    //In case the asymmetric object property works under inverse logic, we must swap source/target of the object assertion
                    if (asymObjPropInvOfValue != null)
                    {
                        (asymObjPropAsn.SourceIndividualExpression, asymObjPropAsn.TargetIndividualExpression) = (asymObjPropAsn.TargetIndividualExpression, asymObjPropAsn.SourceIndividualExpression);
                        asymObjPropAsn.ObjectPropertyExpression = asymObjPropInvOfValue;
                    }
                }
                //Swapping source/target above can make two previously-distinct assertions collapse into duplicates: dedupe before the pairing check below
                asymObjPropAsns = OWLAxiomHelper.RemoveDuplicates(asymObjPropAsns);
                #endregion

                //Asymmetry is violated only by a genuine pair of opposite-direction assertions (IDV1->IDV2 and IDV2->IDV1) under
                //the SAME property; a reflexive self-assertion (IDV1->IDV1) would match both outer/inner as itself, but that
                //case is out of scope here (covered separately by ReflexiveObjectPropertyAnalysis/IrreflexiveObjectPropertyAnalysis)
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