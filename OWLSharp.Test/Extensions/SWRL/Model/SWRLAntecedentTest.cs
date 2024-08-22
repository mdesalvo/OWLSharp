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
using OWLSharp.Extensions.SWRL.Model;
using OWLSharp.Extensions.SWRL.Model.Atoms;
using OWLSharp.Extensions.SWRL.Model.BuiltIns;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Data;

namespace OWLSharp.Test.Extensions.SWRL.Model
{
    [TestClass]
    public class SWRLAntecedentTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateEmptyAntecedent()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent();

            Assert.IsNotNull(antecedent);
            Assert.IsNotNull(antecedent.Atoms);
            Assert.IsTrue(antecedent.Atoms.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateAntecedentWithAtom()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent() {
                Atoms = [
                    new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), new RDFVariable("?C"))
                ]};

            Assert.IsNotNull(antecedent);
            Assert.IsNotNull(antecedent.Atoms);
            Assert.IsTrue(antecedent.Atoms.Count == 1);
            Assert.IsTrue(string.Equals("ex:class(?C)", antecedent.ToString()));
        }

        [TestMethod]
        public void ShouldCreateAntecedentWithBuiltIn()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent() {
                Atoms = [
                    new SWRLContainsBuiltIn(new RDFVariable("?C"), "ind")
                ]};

            Assert.IsNotNull(antecedent);
            Assert.IsNotNull(antecedent.Atoms);
            Assert.IsTrue(antecedent.Atoms.Count == 1);
            Assert.IsTrue(string.Equals("swrlb:contains(?C,\"ind\")", antecedent.ToString()));
        }

        [TestMethod]
        public void ShouldEvaluateAntecedent()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:class"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:class")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:class")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")))
                ]
            };

            SWRLAntecedent antecedent = new SWRLAntecedent() {
                Atoms = [
                    new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), new RDFVariable("?C")),
                    new SWRLContainsBuiltIn(new RDFVariable("?C"), "iv1")
                ]};

            Assert.IsTrue(string.Equals("ex:class(?C) ^ swrlb:contains(?C,\"iv1\")", antecedent.ToString()));

            DataTable results = antecedent.Evaluate(ontology);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Rows.Count == 1);
        }
        #endregion
    }
}