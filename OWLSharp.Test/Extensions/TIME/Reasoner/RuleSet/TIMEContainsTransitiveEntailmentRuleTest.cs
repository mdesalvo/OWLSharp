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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.TIME;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEContainsTransitiveEntailmentRuleTest : TIMETestOntology
    {
        #region Tests
        [TestMethod]
        public async Task ShouldExecuteContainsTransitiveEntailment()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclareIntervalFeature(new RDFResource("ex:Feature1"), new TIMEInterval(new RDFResource("ex:Interval1")));
            ontology.DeclareIntervalFeature(new RDFResource("ex:Feature2"), new TIMEInterval(new RDFResource("ex:Interval2")));
            ontology.DeclareIntervalFeature(new RDFResource("ex:Feature3"), new TIMEInterval(new RDFResource("ex:Interval3")));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                new OWLNamedIndividual(new RDFResource("ex:Interval1")),
                new OWLNamedIndividual(new RDFResource("ex:Interval2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                new OWLNamedIndividual(new RDFResource("ex:Interval2")),
                new OWLNamedIndividual(new RDFResource("ex:Interval3"))));
            List<OWLInference> inferences = await TIMEContainsTransitiveEntailmentRule.ExecuteRuleAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }

        [TestMethod]
        public async Task ShouldExecuteContainsTransitiveEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclareIntervalFeature(new RDFResource("ex:Feature1"), new TIMEInterval(new RDFResource("ex:Interval1")));
            ontology.DeclareIntervalFeature(new RDFResource("ex:Feature2"), new TIMEInterval(new RDFResource("ex:Interval2")));
            ontology.DeclareIntervalFeature(new RDFResource("ex:Feature3"), new TIMEInterval(new RDFResource("ex:Interval3")));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                new OWLNamedIndividual(new RDFResource("ex:Interval1")),
                new OWLNamedIndividual(new RDFResource("ex:Interval2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                new OWLNamedIndividual(new RDFResource("ex:Interval2")),
                new OWLNamedIndividual(new RDFResource("ex:Interval3"))));

            TIMEReasoner reasoner = new TIMEReasoner().AddRule(TIMEEnums.TIMEReasonerRules.ContainsTransitiveEntailment);
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}