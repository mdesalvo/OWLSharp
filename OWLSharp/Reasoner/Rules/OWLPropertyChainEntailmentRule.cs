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

using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL2 reasoner rule targeting data knowledge (A-BOX) to infer assertions from owl:propertyChainAxiom relations
    /// </summary>
    internal static class OWLPropertyChainEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferAssertionsFromChainAxiomProperty(RDFResource currentProperty, List<RDFResource> chainProperties, OWLReasonerReport report)
            {
                //Transform OWL2 property chain into equivalent SPARQL property path
                RDFPropertyPath propertyChainPath = new RDFPropertyPath(new RDFVariable("?PROPERTY_CHAIN_AXIOM_START"), new RDFVariable("?PROPERTY_CHAIN_AXIOM_END"));
                foreach (RDFResource chainProperty in chainProperties)
                    propertyChainPath.AddSequenceStep(new RDFPropertyPathStep(chainProperty));

                //Execute SPARQL construct query on ontology data to extract property chain inferences
                RDFConstructQueryResult queryResult =
                    new RDFConstructQuery()
                        .AddPatternGroup(new RDFPatternGroup()
                            .AddPropertyPath(propertyChainPath))
                        .AddTemplate(new RDFPattern(new RDFVariable("?PROPERTY_CHAIN_AXIOM_START"), currentProperty, new RDFVariable("?PROPERTY_CHAIN_AXIOM_END")))
                        .ApplyToGraph(ontology.Data.ABoxGraph);

                //Populate result with corresponding inference assertions
                foreach (RDFTriple queryResultTriple in queryResult.ToRDFGraph())
                {
                    //Create the inference
                    OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                        nameof(OWLPropertyChainEntailmentRule), queryResultTriple.SetInference());

                    //Add the inference to the report
                    if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                        report.AddEvidence(evidence);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
            {
                //owl:propertyChainAxiom
                List<RDFResource> chainProperties = ontology.Model.PropertyModel.GetChainAxiomPropertiesOf(objectPropertiesEnumerator.Current);
                if (chainProperties.Any())
                    InferAssertionsFromChainAxiomProperty(objectPropertiesEnumerator.Current, chainProperties, reasonerRuleReport);
            }   

            return reasonerRuleReport;
        }
    }
}