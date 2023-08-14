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
    /// OWL2 reasoner rule targeting data knowledge (A-BOX) to reason over reflexive object assertions
    /// </summary>
    internal static class OWLReflexivePropertyEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void SwitchReflexiveObjectAssertions(RDFResource currentProperty, OWLReasonerReport report)
            {
                foreach (RDFTriple propertyObjectAssertion in ontology.Data.ABoxGraph[null, currentProperty, null, null]
                                                                           .Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
                {                    
                    //Create the inferences
                    OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                        nameof(OWLReflexivePropertyEntailmentRule), new RDFTriple((RDFResource)propertyObjectAssertion.Subject, currentProperty, (RDFResource)propertyObjectAssertion.Subject));

                    //Add the inferences to the report
                    if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                        report.AddEvidence(evidence);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:ReflexiveProperty
            IEnumerator<RDFResource> reflexivePropertiesEnumerator = ontology.Model.PropertyModel.ReflexivePropertiesEnumerator;
            while (reflexivePropertiesEnumerator.MoveNext())
                SwitchReflexiveObjectAssertions(reflexivePropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}