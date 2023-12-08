/*
   Copyright 2012-2024 Marco De Salvo
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

using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL2 validator rule checking for consistency of owl:disjointUnionOf relations
    /// </summary>
    internal static class OWLDisjointUnionRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();
            Dictionary<long, HashSet<long>> individualsCache = new Dictionary<long, HashSet<long>>();

            //owl:disjointUnionOf
            IEnumerator<RDFResource> classesEnumerator = ontology.Model.ClassModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
            {
                if (ontology.Model.ClassModel.CheckHasDisjointUnionClass(classesEnumerator.Current))
                {
                    //Extract member classes of the disjointUnion definition
                    RDFResource disjointUnionRepresentative = ontology.Model.ClassModel.TBoxGraph[classesEnumerator.Current, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null].FirstOrDefault()?.Object as RDFResource;
                    List<RDFResource> disjointUnionMembers = new List<RDFResource>();
                    RDFCollection disjointUnionMembersCollection = RDFModelUtilities.DeserializeCollectionFromGraph(ontology.Model.ClassModel.TBoxGraph, disjointUnionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                    foreach (RDFPatternMember disjointUnionMember in disjointUnionMembersCollection)
                        disjointUnionMembers.Add((RDFResource)disjointUnionMember);

                    //Materialize individuals of each member class
                    foreach (RDFResource disjointUnionMember in disjointUnionMembers)
                    {
                        List<RDFResource> disointUnionMemberIndividuals = ontology.Data.GetIndividualsOf(ontology.Model, disjointUnionMember);
                        foreach (RDFResource disointUnionMemberIndividual in disointUnionMemberIndividuals)
                        {
                            if (!individualsCache.ContainsKey(disointUnionMemberIndividual.PatternMemberID))
                                individualsCache.Add(disointUnionMemberIndividual.PatternMemberID, new HashSet<long>());
                            individualsCache[disointUnionMemberIndividual.PatternMemberID].Add(disjointUnionMember.PatternMemberID);
                        }
                    }

                    //Analyze materialized cache to detect if there are clashing individuals
                    if (individualsCache.Any(idvc => idvc.Value.Count > 1))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDisjointUnionRule),
                            $"Violation of 'owl:disjointUnionOf' behavior on class '{classesEnumerator.Current}'",
                            "Revise your class model: you have disjointUnion class with an individual belonging at the same time to more than one of its classes!"));
                }
            }

            return validatorRuleReport;
        }
    }
}