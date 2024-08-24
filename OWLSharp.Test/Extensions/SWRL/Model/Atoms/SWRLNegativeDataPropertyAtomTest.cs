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
    public class SWRLNegativeDataPropertyAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateNegativeDataPropertyAtomWithVariableAsRightArgument()
        {
            SWRLNegativeDataPropertyAtom negativeDataPropertyAtom = new SWRLNegativeDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFVariable("?R"));

            Assert.IsNotNull(negativeDataPropertyAtom);
            Assert.IsNotNull(negativeDataPropertyAtom.Predicate);
            Assert.IsTrue(negativeDataPropertyAtom.Predicate.GetIRI().Equals(new RDFResource("ex:dtprop")));
            Assert.IsNotNull(negativeDataPropertyAtom.LeftArgument);
            Assert.IsTrue(negativeDataPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(negativeDataPropertyAtom.RightArgument);
            Assert.IsTrue(negativeDataPropertyAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(string.Equals("not(ex:dtprop(?L,?R))", negativeDataPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateNegativeDataPropertyAtomWithLiteralAsRightArgument()
        {
            SWRLNegativeDataPropertyAtom DataPropertyAtom = new SWRLNegativeDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFPlainLiteral("hello", "en-US"));

            Assert.IsNotNull(DataPropertyAtom);
            Assert.IsNotNull(DataPropertyAtom.Predicate);
            Assert.IsTrue(DataPropertyAtom.Predicate.GetIRI().Equals(new RDFResource("ex:dtprop")));
            Assert.IsNotNull(DataPropertyAtom.LeftArgument);
            Assert.IsTrue(DataPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(DataPropertyAtom.RightArgument);
            Assert.IsTrue(DataPropertyAtom.RightArgument.Equals(new RDFPlainLiteral("hello", "en-US")));
            Assert.IsTrue(string.Equals("not(ex:dtprop(?L,\"hello\"@EN-US))", DataPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeDataPropertyAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new SWRLNegativeDataPropertyAtom(null, new RDFVariable("?L"), new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeDataPropertyAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLNegativeDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeDataPropertyAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new SWRLNegativeDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeDataPropertyAtomBecauseNullRightArgumentLiteral()
            => Assert.ThrowsException<OWLException>(() => new SWRLNegativeDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), new RDFVariable("?L"), null as RDFLiteral));

        [TestMethod]
        public void ShouldEvaluateNegativeDataPropertyAtomOnAntecedentWithVariableAsRightArgument()
        {
            SWRLNegativeDataPropertyAtom negativeDataPropertyAtom = new SWRLNegativeDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFVariable("?R"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dtprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv")))
                ],
                AssertionAxioms = [
                    new OWLNegativeDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dtprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")),
                        new OWLLiteral(new RDFPlainLiteral("value")))
                ]
            };
            
            DataTable antecedentTable = negativeDataPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateNegativeDataPropertyAtomOnAntecedentWithLiteralAsRightArgument()
        {
            SWRLNegativeDataPropertyAtom negativeDataPropertyAtom = new SWRLNegativeDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")),
                new RDFVariable("?L"),
                new RDFPlainLiteral("value"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dtprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv")))
                ],
                AssertionAxioms = [
                    new OWLNegativeDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dtprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")),
                        new OWLLiteral(new RDFPlainLiteral("value")))
                ]
            };

            DataTable antecedentTable = negativeDataPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateNegativeDataPropertyAtomOnConsequentWithVariableAsRightArgument()
        {
            SWRLNegativeDataPropertyAtom negativeDataPropertyAtom = new SWRLNegativeDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv", "value");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = negativeDataPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateNegativeDataPropertyAtomOnConsequentWithLiteralAsRightArgument()
        {
            SWRLNegativeDataPropertyAtom negativeDataPropertyAtom = new SWRLNegativeDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFPlainLiteral("hello", "en-US"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = negativeDataPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}