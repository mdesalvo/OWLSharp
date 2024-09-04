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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Reasoner;
using OWLSharp.Reasoner.RuleSet;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner.RuleSet
{
    [TestClass]
    public class OWLSubClassOfEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailSubClassOfCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammifero"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                ClassAxioms = [ 
                    new OWLSubClassOf(
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal"))),
					new OWLSubClassOf(
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
					new OWLEquivalentClasses([
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammifero"))
					])
				]
            };
            List<OWLInference> inferences = OWLSubClassOfEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf 
                            && string.Equals(inf.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mammifero")
                            && string.Equals(inf.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf1 
                            && string.Equals(inf1.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf1.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf2 
                            && string.Equals(inf2.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf2.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mammifero")));	
        }

        [TestMethod]
        public void ShouldEntailSubClassOfOnDisjointUnionCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/LivingEntity"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom")))
                ],
                ClassAxioms = [
                    new OWLDisjointUnion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/LivingEntity")),
                        [
                            new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")),
                            new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal")),
                            new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom"))
                        ])
                ]
            };
            List<OWLInference> inferences = OWLSubClassOfEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf
                            && string.Equals(inf.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                            && string.Equals(inf.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf1
                            && string.Equals(inf1.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal")
                            && string.Equals(inf1.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf2
                            && string.Equals(inf2.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mushroom")
                            && string.Equals(inf2.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity")));
        }
        #endregion
    }
}