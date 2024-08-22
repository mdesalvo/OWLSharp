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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.SWRL.Model.Atoms;
using OWLSharp.Extensions.SWRL.Model;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWLSharp.Extensions.SWRL.Reasoner;
using System;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;

namespace OWLSharp.Test.Extensions.SWRL.Reasoner
{
    [TestClass]
    public class SWRLReasonerTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReasoner()
        {
            SWRLReasoner reasoner = new SWRLReasoner()
            {
                Rules = [
                    new SWRLRule(new Uri("ex:testRule"), "description",
                        new SWRLAntecedent(), new SWRLConsequent())
                ]
            };

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.Rules);
            Assert.IsTrue(reasoner.Rules.Count == 1);
        }

        [TestMethod]
        public async Task ShouldApplyToOntologyAsync()
        {
            string infoMsg = null;
            OWLEvents.OnInfo += (string msg) => { infoMsg += msg; };

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:class"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv")))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:class")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")))
                ]
            };

            SWRLReasoner reasoner = new SWRLReasoner()
            {
                Rules = [ 
                    new SWRLRule(new Uri("ex:testRule"), "this is test rule",
                        new SWRLAntecedent()
                        {
                            Atoms = [
                                new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), new RDFVariable("?C"))
                            ]
                        },
                        new SWRLConsequent()
                        {
                            Atoms = [
                                new SWRLClassAtom(new OWLClass(RDFVocabulary.OWL.NAMED_INDIVIDUAL), new RDFVariable("?C"))
                            ]
                        }) 
                ]
            };

            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsNotNull(infoMsg);
            Assert.IsTrue(infoMsg.IndexOf("1 unique inferences") > -1);
        }
        #endregion
    }
}