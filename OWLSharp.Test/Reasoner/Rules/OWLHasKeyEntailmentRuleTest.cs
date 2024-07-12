﻿/*
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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Reasoner.Rules;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Reasoner.Rules
{
    [TestClass]
    public class OWLHasKeyEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailObjectHasKeyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Rose"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Abigail"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
                ],
				KeyAxioms = [
					new OWLHasKey(
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
						[ new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")) ],
						null 
					)
				],
				AssertionAxioms = [
					new OWLClassAssertion(
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))
					),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))
					),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))
					),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Rose"))
					),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Helen"))
					),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Abigail"))
					),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Abigail"))
					)
				]
            };
            List<OWLAxiom> inferences = OWLHasKeyEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.IsInference));
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0] is OWLSameIndividual inf 
                            && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Glener")
							&& string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Henry"));
        }
        #endregion
    }
}