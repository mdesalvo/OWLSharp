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
using RDFSharp.Model;
using RDFSharp.Query;
using System.Threading.Tasks;
using System;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Axioms;
using System.Collections.Generic;
using OWLSharp.Reasoner;

namespace OWLSharp.Test.Extensions.SWRL.Model
{
    [TestClass]
    public class SWRLRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateRule()
        {
            SWRLRule rule = new SWRLRule(new Uri("ex:testRule"), "this is test rule",
                new SWRLAntecedent() { 
                    Atoms = [ 
                        new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), new RDFVariable("?C")) 
                    ]},
                new SWRLConsequent() { 
                    Atoms = [
                        new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), new RDFVariable("?C"), RDFTypedLiteral.True) 
                    ]});

            Assert.IsNotNull(rule);
            Assert.IsTrue(string.Equals("ex:testRule", rule.IRI.ToString()));
            Assert.IsTrue(string.Equals("this is test rule", rule.Description));
            Assert.IsNotNull(rule.Antecedent);
            Assert.IsNotNull(rule.Consequent);
            Assert.IsTrue(string.Equals("ex:class(?C) -> ex:dtprop(?C,\"true\"^^xsd:boolean)", rule.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingRuleBecauseNullIRI()
            => Assert.ThrowsException<OWLException>(() => new SWRLRule(null, "this is test rule",
                new SWRLAntecedent(), new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingRuleBecauseNullDescription()
            => Assert.ThrowsException<OWLException>(() => new SWRLRule(new Uri("ex:testRule"), null,
                new SWRLAntecedent(), new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingRuleBecauseNullAntecedent()
           => Assert.ThrowsException<OWLException>(() => new SWRLRule(new Uri("ex:testRule"), "this is test rule",
               null, new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingRuleBecauseNullConsequent()
           => Assert.ThrowsException<OWLException>(() => new SWRLRule(new Uri("ex:testRule"), "description",
               new SWRLAntecedent(), null));

        [TestMethod]
        public async Task ShouldApplyToOntologyAsync()
        {
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

            SWRLRule rule = new SWRLRule(new Uri("ex:testRule"), "this is test rule",
                new SWRLAntecedent() { 
                    Atoms = [
                        new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), new RDFVariable("?C"))
                    ] },
                new SWRLConsequent() { 
                    Atoms = [
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.OWL.NAMED_INDIVIDUAL), new RDFVariable("?C"))
                    ] }); 

            List<OWLInference> inferences = await rule.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}