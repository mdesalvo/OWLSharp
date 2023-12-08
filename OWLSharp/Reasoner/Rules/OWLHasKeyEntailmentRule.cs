﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWLSharp
{
    /// <summary>
    /// OWL2 reasoner rule targeting data knowledge (A-BOX) to infer owl:sameAs relations from owl:hasKey relations
    /// </summary>
    internal static class OWLHasKeyEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            Dictionary<string, List<RDFResource>> GetKeyValuesOfClass(RDFResource currentClass, List<RDFResource> keyProperties)
            {
                Dictionary<string, List<RDFResource>> keyValueRegister = new Dictionary<string, List<RDFResource>>();

                //Iterate individuals of the current class in order to calculate their key values
                foreach (RDFResource classIndividual in ontology.Data.GetIndividualsOf(ontology.Model, currentClass))
                {
                    //Calculate the key values of the current individual
                    StringBuilder sb = new StringBuilder();
                    foreach (RDFResource keyProperty in keyProperties)
                    {
                        RDFGraph individualKeyValueGraph = ontology.Data.ABoxGraph[classIndividual, keyProperty, null, null];
                        if (individualKeyValueGraph.TriplesCount > 0)
                            sb.Append(string.Join("§§", individualKeyValueGraph.Select(t => t.Object.ToString())));
                    }

                    //Collect the key values of the current individual into the register
                    string sbValue = sb.ToString();
                    if (!string.IsNullOrEmpty(sbValue))
                    {
                        if (!keyValueRegister.ContainsKey(sbValue))
                            keyValueRegister.Add(sbValue, new List<RDFResource>());
                        keyValueRegister[sbValue].Add(classIndividual);
                    }
                }

                return keyValueRegister;
            }
            void AnalyzeKeyValueCollisions(Dictionary<string, List<RDFResource>> keyValueRegister, OWLReasonerReport report)
            {
                //Collisions are signalled by key values generated by 2+ individuals
                foreach (KeyValuePair<string, List<RDFResource>> collisionKeyValueRegister in keyValueRegister.Where(kvr => kvr.Value.Count > 1)
                                                                                                              .ToDictionary(kv => kv.Key, kv => kv.Value))
                {
                    //Analyze semantic compatibility between colliding members
                    foreach (RDFResource outerIndividual in collisionKeyValueRegister.Value)
                        foreach (RDFResource innerIndividual in collisionKeyValueRegister.Value.Where(innerIndiv => !innerIndiv.Equals(outerIndividual)))
                        {
                            //In order to accept the collision and generate a reasoner evidence, colliding members must not be related by owl:differentFrom
                            if (ontology.Data.CheckSameAsCompatibility(outerIndividual, innerIndividual))
                            {
                                //Create the inferences
                                OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                                    nameof(OWLHasKeyEntailmentRule), new RDFTriple(outerIndividual, RDFVocabulary.OWL.SAME_AS, innerIndividual).SetInference());
                                OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                                    nameof(OWLHasKeyEntailmentRule), new RDFTriple(innerIndividual, RDFVocabulary.OWL.SAME_AS, outerIndividual).SetInference());

                                //Add the inference to the report
                                if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceA.EvidenceContent))
                                    report.AddEvidence(evidenceA);
                                if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceB.EvidenceContent))
                                    report.AddEvidence(evidenceB);
                            }
                        }
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:Class
            IEnumerator<RDFResource> classesEnumerator = ontology.Model.ClassModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
            {
                //owl:hasKey
                List<RDFResource> keyProperties = ontology.Model.ClassModel.GetKeyPropertiesOf(classesEnumerator.Current);
                if (keyProperties.Any())
                {
                    //Calculate key values for members of the constrained class
                    Dictionary<string, List<RDFResource>> keyValueRegister = GetKeyValuesOfClass(classesEnumerator.Current, keyProperties);

                    //Filter key values generated by 2+ members (this is the signal of collision)
                    AnalyzeKeyValueCollisions(keyValueRegister, reasonerRuleReport);
                }   
            }                

            return reasonerRuleReport;
        }
    }
}