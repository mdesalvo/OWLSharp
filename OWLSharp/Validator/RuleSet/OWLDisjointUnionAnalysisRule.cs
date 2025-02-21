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
    internal static class OWLDisjointUnionAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.DisjointUnionAnalysis.ToString();
        internal const string rulesugg = "There should not be class expressions belonging to a DisjointUnion axiom and having a class assertion on the same individual!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, Dictionary<string, object> validatorCache)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            Dictionary<long, HashSet<long>> idvsCache = new Dictionary<long, HashSet<long>>();
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            
            //DisjointUnion(CLS,(CLS1,CLS2)) ^ ClassAssertion(CLS1,IDV) ^ ClassAssertion(CLS2,IDV) -> ERROR
            foreach (OWLDisjointUnion disjUnion in ontology.GetClassAxiomsOfType<OWLDisjointUnion>())
            {
                //Materialize individuals of each class expression member of the DisjointUnion
                foreach (OWLClassExpression disjUnionMember in disjUnion.ClassExpressions)
                    foreach (OWLIndividualExpression disjUnionMemberIdv in ontology.GetIndividualsOf(disjUnionMember, clsAsns))
                    {
                        RDFResource disjUnionMemberIdvIRI = disjUnionMemberIdv.GetIRI();
                        if (!idvsCache.ContainsKey(disjUnionMemberIdvIRI.PatternMemberID))
                            idvsCache.Add(disjUnionMemberIdvIRI.PatternMemberID, new HashSet<long>());
                        idvsCache[disjUnionMemberIdvIRI.PatternMemberID].Add(disjUnionMember.GetIRI().PatternMemberID);
                    }

                //Analyze individuals cache to detect if there are individuals shared between the class expression members
                if (idvsCache.Any(idvc => idvc.Value.Count > 1))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error, 
                        rulename, 
                        $"Violated DisjointUnion axiom with signature: '{disjUnion.GetXML()}'", 
                        rulesugg));

                //Reset register for next DisjointUnion axiom
                idvsCache.Clear();
            }

            return issues;
        }
    }
}