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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Data;

namespace OWLSharp.Test.Extensions.SWRL.Model.Atoms
{
    [TestClass]
    public class SWRLClassAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateClassAtom()
        {
            SWRLClassAtom classAtom = new SWRLClassAtom(
                new OWLClass(new RDFResource("ex:class")), 
                new RDFVariable("?C"));

            Assert.IsNotNull(classAtom);
            Assert.IsNotNull(classAtom.Predicate);
            Assert.IsTrue(classAtom.Predicate.GetIRI().Equals(new RDFResource("ex:class")));
            Assert.IsNotNull(classAtom.LeftArgument);
            Assert.IsTrue(classAtom.LeftArgument.Equals(new RDFVariable("?C")));
            Assert.IsNull(classAtom.RightArgument);
            Assert.IsTrue(string.Equals("ex:class(?C)", classAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new SWRLClassAtom(null, new RDFVariable("?C")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), null));

        [TestMethod]
        public void ShouldEvaluateClassAtomOnAntecedent()
        {
            SWRLClassAtom classAtom = new SWRLClassAtom(
                new OWLClass(new RDFResource("ex:class")), 
                new RDFVariable("?C"));

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
            
            DataTable antecedentTable = classAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateClassAtomOnConsequent()
        {
            SWRLClassAtom classAtom = new SWRLClassAtom(
                new OWLClass(new RDFResource("ex:class")), 
                new RDFVariable("?C"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv1");
            antecedentTable.Rows.Add("ex:indiv2");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = classAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 2);
        }
        #endregion
    }
}