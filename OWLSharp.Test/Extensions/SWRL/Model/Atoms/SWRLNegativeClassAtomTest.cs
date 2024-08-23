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
    public class SWRLNegativeClassAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateNegativeClassAtom()
        {
            SWRLNegativeClassAtom negativeClassAtom = new SWRLNegativeClassAtom(
                new OWLClass(new RDFResource("ex:class")), 
                new RDFVariable("?C"));

            Assert.IsNotNull(negativeClassAtom);
            Assert.IsNotNull(negativeClassAtom.Predicate);
            Assert.IsTrue(negativeClassAtom.Predicate.GetIRI().Equals(new RDFResource("ex:class")));
            Assert.IsNotNull(negativeClassAtom.LeftArgument);
            Assert.IsTrue(negativeClassAtom.LeftArgument.Equals(new RDFVariable("?C")));
            Assert.IsNull(negativeClassAtom.RightArgument);
            Assert.IsTrue(string.Equals("not(ex:class(?C))", negativeClassAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeClassAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new SWRLNegativeClassAtom(null, new RDFVariable("?C")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeClassAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLNegativeClassAtom(new OWLClass(new RDFResource("ex:class")), null));

        [TestMethod]
        public void ShouldEvaluateNegativeClassAtomOnAntecedent()
        {
            SWRLNegativeClassAtom negativeClassAtom = new SWRLNegativeClassAtom(
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
                        new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:class"))),
                        new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:class")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")))
                ]
            };
            
            DataTable antecedentTable = negativeClassAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateNegativeClassAtomOnConsequent()
        {
            SWRLNegativeClassAtom negativeClassAtom = new SWRLNegativeClassAtom(
                new OWLClass(new RDFResource("ex:class")), 
                new RDFVariable("?C"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv1");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = negativeClassAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}