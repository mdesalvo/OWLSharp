/*
   Copyright 2012-2023 Marco De Salvo
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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL2 reasoner rule targeting data knowledge (A-BOX) to infer assertions from owl:hasSelf restrictions
    /// </summary>
    internal static class OWLHasSelfEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferAssertionsFromHasSelfRestriction(RDFResource currentHSRestriction, OWLReasonerReport report)
            {
                //Calculate subclasses of the current hasSelf restriction
                List<RDFResource> subClassesOfHSRestriction = ontology.Model.ClassModel.GetSubClassesOf(currentHSRestriction);
                if (subClassesOfHSRestriction.Any())
                {
                    //Get restricted property
                    RDFResource currentHSRestrictionOnProperty = OWLOntologyClassModelLoader.GetRestrictionProperty(ontology.Model.ClassModel.TBoxGraph, currentHSRestriction);

                    //Iterate calculated subclasses of the current hasValue restriction
                    foreach (RDFResource subClassOfHSRestriction in subClassesOfHSRestriction)
                    {
                        //Calculate individuals of the current subclass
                        List<RDFResource> subClassOfHSRestrictionMembers = ontology.Data.GetIndividualsOf(ontology.Model, subClassOfHSRestriction);
                        foreach (RDFResource subClassOfHVRestrictionMember in subClassOfHSRestrictionMembers)
                        {
                            //Create the inference
                            OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, 
                                nameof(OWLHasSelfEntailmentRule), new RDFTriple(subClassOfHVRestrictionMember, currentHSRestrictionOnProperty, subClassOfHVRestrictionMember));

                            //Add the inference to the report
                            if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                                report.AddEvidence(evidence);
                        }
                    }
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:Restriction
            IEnumerator<RDFResource> restrictionsEnumerator = ontology.Model.ClassModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                //owl:hasSelf(TRUE)
                if (ontology.Model.ClassModel.CheckHasSelfRestrictionClass(restrictionsEnumerator.Current)
                      && ontology.Model.ClassModel.TBoxGraph.ContainsTriple(new RDFTriple(restrictionsEnumerator.Current, RDFVocabulary.OWL.HAS_SELF, RDFTypedLiteral.True)))
                    InferAssertionsFromHasSelfRestriction(restrictionsEnumerator.Current, reasonerRuleReport);
            }                

            return reasonerRuleReport;
        }
    }
}