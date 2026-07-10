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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>OWLSharp extension: enforces OWL2 DL property-chain regularity / simple-role restrictions (chain superproperty cannot be asymmetric/functional/inverse-functional/irreflexive; chain cannot be self-referential), which is outside the RL/RDF entailment table</para>
    /// </summary>
    internal static class OWLObjectPropertyChainAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.ObjectPropertyChainAnalysis);
        internal const string rulesugg = "There should not be ObjectPropertyChain definitions related to object property expressions of type: AsymmetricObjectProperty, FunctionalObjectProperty, InverseFunctionalObjectProperty, IrreflexiveObjectProperty!";
        internal const string rulesugg2 = "There should not be ObjectPropertyChain definitions containing object property expressions defined as their super properties (this is a loop!)";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //SubObjectPropertyOf(OPCHAIN,OP) ^ ObjectPropertyChain(OPCHAIN,(OP1,OP2)) ^ AsymmetricObjectProperty(OP) -> ERROR
            //SubObjectPropertyOf(OPCHAIN,OP) ^ ObjectPropertyChain(OPCHAIN,(OP1,OP2)) ^ FunctionalObjectProperty(OP) -> ERROR
            //SubObjectPropertyOf(OPCHAIN,OP) ^ ObjectPropertyChain(OPCHAIN,(OP1,OP2)) ^ InverseFunctionalObjectProperty(OP) -> ERROR
            //SubObjectPropertyOf(OPCHAIN,OP) ^ ObjectPropertyChain(OPCHAIN,(OP1,OP2)) ^ IrreflexiveObjectProperty(OP) -> ERROR
            foreach (OWLSubObjectPropertyOf subObjectPropertyOf in ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>()
                                                                           .Where(ax => ax.SubObjectPropertyChain != null))
            {
                if (ontology.CheckHasAsymmetricObjectProperty(subObjectPropertyOf.SuperObjectPropertyExpression)
                     || ontology.CheckHasFunctionalObjectProperty(subObjectPropertyOf.SuperObjectPropertyExpression)
                     || ontology.CheckHasInverseFunctionalObjectProperty(subObjectPropertyOf.SuperObjectPropertyExpression)
                     || ontology.CheckHasIrreflexiveObjectProperty(subObjectPropertyOf.SuperObjectPropertyExpression))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated SubObjectPropertyOf expression with ObjectPropertyChain signature: '{subObjectPropertyOf.GetXML()}'",
                        rulesugg));
                }

                //SubObjectPropertyOf(OPCHAIN,OP) ^ ObjectPropertyChain(OPCHAIN,(OP,OP1,OP2)) -> ERROR
                if (subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions.Any(opex => opex.GetIRI().Equals(subObjectPropertyOf.SuperObjectPropertyExpression.GetIRI())))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated SubObjectPropertyOf expression with ObjectPropertyChain signature: '{subObjectPropertyOf.GetXML()}'",
                        rulesugg2));
                }
            }

            return issues;
        }
    }
}