/*
   Copyright 2014-2024 Marco De Salvo
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

using System.Collections.Generic;
using System.Threading.Tasks;
using OWLSharp.Ontology;

namespace OWLSharp.Reasoner
{
    public class OWLReasoner
    {
        #region Properties
        public List<OWLEnums.OWLReasonerRules> Rules { get; set; }
        #endregion

        #region Ctors
        public OWLReasoner()
			=> Rules = new List<OWLEnums.OWLReasonerRules>();
        #endregion

        #region Methods
        public OWLReasonerReport ApplyToOntology(OWLOntology ontology)
        {
            OWLReasonerReport reasonerReport = new OWLReasonerReport();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Reasoner is going to be applied to Ontology '{ontology.IRI}': this may require intensive processing, depending on size and complexity of domain knowledge and rules");

                //Initialize inference registry
                Dictionary<string, OWLReasonerReport> inferenceRegistry = new Dictionary<string, OWLReasonerReport>();
                foreach (OWLEnums.OWLReasonerRules reasonerRule in Rules)
                    inferenceRegistry.Add(reasonerRule.ToString(), null);

                //Execute rules
                Parallel.ForEach(Rules, 
                    reasonerRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching reasoner rule '{reasonerRule}'");

                        switch (reasonerRule)
                        {
                            case OWLEnums.OWLReasonerRules.SubClassOfEntailment:
								inferenceRegistry[OWLEnums.OWLReasonerRules.SubClassOfEntailment.ToString()] = OWLSubClassOfEntailmentRule.ExecuteRule(ontology);
								break;
                        }

                        OWLEvents.RaiseInfo($"Completed reasoner rule '{reasonerRule}': got {inferenceRegistry[reasonerRule.ToString()].Inferences.Count} inferences");
                    });

                //Process inference registry
                foreach (OWLReasonerReport inferenceRegistryReport in inferenceRegistry.Values)
                    reasonerReport.Inferences.AddRange(inferenceRegistryReport.Inferences);

                OWLEvents.RaiseInfo($"Reasoner has been applied to Ontology '{ontology.IRI}': got {reasonerReport.Inferences.Count} inferences");
            }

            return reasonerReport;
        }
        #endregion
    }
}