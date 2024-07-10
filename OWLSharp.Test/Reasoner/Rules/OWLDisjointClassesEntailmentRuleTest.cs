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
using OWLSharp.Reasoner.Rules;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Reasoner.Rules
{
    [TestClass]
    public class OWLDisjointClassesEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailDisjointClassesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Fungine")))
                ],
                ClassAxioms = [ 
                    new OWLDisjointClasses([
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal"))]),
					new OWLDisjointClasses([
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom"))]),
                    new OWLDisjointClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom"))]),
                    new OWLEquivalentClasses([
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Fungine"))
					])
				]
            };
            List<OWLAxiom> inferences = OWLDisjointClassesEntailmentRule.ExecuteRule(ontology);
            
            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.IsInference));
            Assert.IsTrue(inferences.Count == 5);
			Assert.IsTrue(inferences[0] is OWLDisjointClasses inf 
                            && string.Equals(inf.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal")
                            && string.Equals(inf.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal"));
            Assert.IsTrue(inferences[1] is OWLDisjointClasses inf1 
                            && string.Equals(inf1.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mushroom")
                            && string.Equals(inf1.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal"));
			Assert.IsTrue(inferences[2] is OWLDisjointClasses inf2 
                            && string.Equals(inf2.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mushroom")
                            && string.Equals(inf2.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal"));
            Assert.IsTrue(inferences[3] is OWLDisjointClasses inf3
                            && string.Equals(inf3.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fungine")
                            && string.Equals(inf3.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal"));
            Assert.IsTrue(inferences[4] is OWLDisjointClasses inf4
                            && string.Equals(inf4.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fungine")
                            && string.Equals(inf4.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal"));
        }
        
        [TestMethod]
        public void ShouldEntailDisjointClassesOnDisjointUnionCase()
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
            List<OWLAxiom> inferences = OWLDisjointClassesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.IsInference));
            Assert.IsTrue(inferences.Count == 4);
            Assert.IsTrue(inferences[0] is OWLDisjointClasses inf
                            && string.Equals(inf.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                            && string.Equals(inf.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal")
                            && string.Equals(inf.ClassExpressions[2].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mushroom"));
            Assert.IsTrue(inferences[1] is OWLSubClassOf inf1
                            && string.Equals(inf1.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                            && string.Equals(inf1.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity"));
            Assert.IsTrue(inferences[2] is OWLSubClassOf inf2
                            && string.Equals(inf2.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal")
                            && string.Equals(inf2.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity"));
            Assert.IsTrue(inferences[3] is OWLSubClassOf inf3
                            && string.Equals(inf3.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mushroom")
                            && string.Equals(inf3.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity"));
        }
        #endregion
    }
}