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

using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Helpers;
using OWLSharp.Reasoner;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SWRL
{
    public class SWRLReasoner
    {
        #region Properties
        public List<SWRLRule> Rules { get; internal set; }
        #endregion

        #region Ctors
        public SWRLReasoner()
            => Rules = new List<SWRLRule>();
        #endregion

        #region Methods
        public async Task<List<OWLInference>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching SWRL reasoner on ontology '{ontology.IRI}'...");

                //Initialize inference registry				
                Dictionary<string, List<OWLInference>> inferenceRegistry = new Dictionary<string, List<OWLInference>>();
                Rules.ForEach(rule => inferenceRegistry.Add(rule.IRI.ToString(), null));

                //Execute reasoner rules
                await Task.Run(() =>
                    Parallel.ForEach(Rules, async (rule) =>
                    {
                        OWLEvents.RaiseInfo($"Launching SWRL rule {rule.IRI}...");

                        inferenceRegistry[rule.IRI.ToString()] = await rule.ApplyToOntologyAsync(ontology);

                        OWLEvents.RaiseInfo($"Completed SWRL rule {rule.IRI} => {inferenceRegistry[rule.IRI.ToString()].Count} candidate inferences");
                    }));

                //Process inference registry
                await Task.Run(async () =>
                {
                    //Fetch axioms commonly targeted by rules
                    Task<HashSet<string>> clsAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> dtPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> opPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> ndtPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> nopPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> diffIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> sameIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Select(asn => asn.GetXML())));
                    await Task.WhenAll(clsAsnAxiomsTask, dtPropAsnAxiomsTask, opPropAsnAxiomsTask, ndtPropAsnAxiomsTask, nopPropAsnAxiomsTask, diffIdvsAxiomsTask, sameIdvsAxiomsTask);

                    //Deduplicate inferences by analyzing explicit knowledge
                    foreach (KeyValuePair<string, List<OWLInference>> inferenceRegistryEntry in inferenceRegistry.Where(ir => ir.Value?.Count > 0))
                        inferenceRegistryEntry.Value.RemoveAll(inf =>
                        {
                            string infXML = inf.Axiom.GetXML();
                            return clsAsnAxiomsTask.Result.Contains(infXML)
                                   || dtPropAsnAxiomsTask.Result.Contains(infXML)
                                   || opPropAsnAxiomsTask.Result.Contains(infXML)
                                   || ndtPropAsnAxiomsTask.Result.Contains(infXML)
                                   || nopPropAsnAxiomsTask.Result.Contains(infXML)
                                   || diffIdvsAxiomsTask.Result.Contains(infXML)
                                   || sameIdvsAxiomsTask.Result.Contains(infXML);
                        });

                    //Collect inferences and perform final cleanup
                    inferences.AddRange(inferenceRegistry.SelectMany(ir => ir.Value).Distinct());
                    inferenceRegistry.Clear();
                });

                OWLEvents.RaiseInfo($"Completed SWRL reasoner on ontology {ontology.IRI} => {inferences.Count} unique inferences");
            }

            return inferences;
        }
        #endregion
    }
}