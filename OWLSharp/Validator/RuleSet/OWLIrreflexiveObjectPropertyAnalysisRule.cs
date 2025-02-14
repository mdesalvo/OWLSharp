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
    internal static class OWLIrreflexiveObjectPropertyAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.IrreflexiveObjectPropertyAnalysis.ToString();
        internal const string rulesugg1 = "There should not be object properties at the same time irreflexive and reflexive!";
        internal const string rulesugg2 = "There should not be object assertions having the same source/target individual under an irreflexive object property!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            List<OWLIrreflexiveObjectProperty> irrefObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>();
            List<OWLReflexiveObjectProperty> refObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>();

            //IrreflexiveObjectProperty(OP) ^ ReflexiveObjectProperty(OP) -> ERROR
            foreach (OWLIrreflexiveObjectProperty irrefObjProp in irrefObjProps)
            {
                RDFResource irrefObjPropIRI = irrefObjProp.ObjectPropertyExpression.GetIRI();
                refObjProps.Where(refObjProp => refObjProp.ObjectPropertyExpression.GetIRI().Equals(irrefObjPropIRI))
                    .ToList()
                    .ForEach(refObjProp =>
                    {
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error, 
                            rulename, 
                            $"Violated IrreflexiveObjectProperty axiom with signature: '{irrefObjProp.GetXML()}'", 
                            rulesugg1));
                    });
            }

            //IrreflexiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV1) -> ERROR
            foreach (OWLIrreflexiveObjectProperty irrefObjProp in irrefObjProps)
            {
                List <OWLObjectPropertyAssertion> irrefObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, irrefObjProp.ObjectPropertyExpression);
                if (irrefObjPropAsns.Any(asn => asn.SourceIndividualExpression.GetIRI().Equals(asn.TargetIndividualExpression.GetIRI())))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error, 
                        rulename, 
                        $"Violated IrreflexiveObjectProperty axiom with signature: '{irrefObjProp.GetXML()}'", 
                        rulesugg2));
                }
            }

            return issues;
        }
    }
}