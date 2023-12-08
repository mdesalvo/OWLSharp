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
    /// OWL-DL reasoner rule targeting data knowledge (A-BOX) to infer assertions from owl:hasValue restrictions
    /// </summary>
    internal static class OWLHasValueEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferAssertionsFromHasValueRestriction(RDFResource currentHVRestriction, OWLReasonerReport report)
            {
                //Calculate subclasses of the current hasValue restriction
                List<RDFResource> subClassesOfHVRestriction = ontology.Model.ClassModel.GetSubClassesOf(currentHVRestriction);
                if (subClassesOfHVRestriction.Any())
                {
                    //Get restricted property and required value
                    RDFResource currentHVRestrictionOnProperty = OWLOntologyClassModelLoader.GetRestrictionProperty(ontology.Model.ClassModel.TBoxGraph, currentHVRestriction);
                    RDFPatternMember currentHVRestrictionValue = OWLOntologyClassModelLoader.GetRestrictionHasValue(ontology.Model.ClassModel.TBoxGraph, currentHVRestriction);

                    //Iterate calculated subclasses of the current hasValue restriction
                    foreach (RDFResource subClassOfHVRestriction in subClassesOfHVRestriction)
                    {
                        //Calculate individuals of the current subclass
                        List<RDFResource> subClassOfHVRestrictionMembers = ontology.Data.GetIndividualsOf(ontology.Model, subClassOfHVRestriction);
                        foreach (RDFResource subClassOfHVRestrictionMember in subClassOfHVRestrictionMembers)
                        {
                            //Create the inference
                            OWLReasonerEvidence evidence = currentHVRestrictionValue is RDFResource
                                ? new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, nameof(OWLHasValueEntailmentRule), new RDFTriple(subClassOfHVRestrictionMember, currentHVRestrictionOnProperty, (RDFResource)currentHVRestrictionValue).SetInference())
                                : new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, nameof(OWLHasValueEntailmentRule), new RDFTriple(subClassOfHVRestrictionMember, currentHVRestrictionOnProperty, (RDFLiteral)currentHVRestrictionValue).SetInference());

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
                //owl:hasValue
                if (ontology.Model.ClassModel.CheckHasValueRestrictionClass(restrictionsEnumerator.Current))
                    InferAssertionsFromHasValueRestriction(restrictionsEnumerator.Current, reasonerRuleReport);
            }                

            return reasonerRuleReport;
        }
    }
}