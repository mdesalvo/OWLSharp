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
    public class SWRLDataPropertyAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataPropertyAtomWithVariableAsRightArgument()
        {
            SWRLDataPropertyAtom dataPropertyAtom = new SWRLDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFVariable("?R"));

            Assert.IsNotNull(dataPropertyAtom);
            Assert.IsNotNull(dataPropertyAtom.Predicate);
            Assert.IsTrue(dataPropertyAtom.Predicate.GetIRI().Equals(new RDFResource("ex:dtprop")));
            Assert.IsNotNull(dataPropertyAtom.LeftArgument);
            Assert.IsTrue(dataPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(dataPropertyAtom.RightArgument);
            Assert.IsTrue(dataPropertyAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(string.Equals("ex:dtprop(?L,?R)", dataPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateDataPropertyAtomWithLiteralAsRightArgument()
        {
            SWRLDataPropertyAtom dataPropertyAtom = new SWRLDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFPlainLiteral("hello", "en-US"));

            Assert.IsNotNull(dataPropertyAtom);
            Assert.IsNotNull(dataPropertyAtom.Predicate);
            Assert.IsTrue(dataPropertyAtom.Predicate.GetIRI().Equals(new RDFResource("ex:dtprop")));
            Assert.IsNotNull(dataPropertyAtom.LeftArgument);
            Assert.IsTrue(dataPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(dataPropertyAtom.RightArgument);
            Assert.IsTrue(dataPropertyAtom.RightArgument.Equals(new RDFPlainLiteral("hello", "en-US")));
            Assert.IsTrue(string.Equals("ex:dtprop(?L,\"hello\"@EN-US)", dataPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new SWRLDataPropertyAtom(null, new RDFVariable("?L"), new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullRightArgumentLiteral()
            => Assert.ThrowsException<OWLException>(() => new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), new RDFVariable("?L"), null as RDFLiteral));

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnAntecedentWithVariableAsRightArgument()
        {
            SWRLDataPropertyAtom dataPropertyAtom = new SWRLDataPropertyAtom(
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
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dtprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")),
                        new OWLLiteral(new RDFPlainLiteral("value")))
                ]
            };
            
            DataTable antecedentTable = dataPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnAntecedentWithLiteralAsRightArgument()
        {
            SWRLDataPropertyAtom dataPropertyAtom = new SWRLDataPropertyAtom(
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
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dtprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")),
                        new OWLLiteral(new RDFPlainLiteral("value")))
                ]
            };

            DataTable antecedentTable = dataPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnConsequentWithVariableAsRightArgument()
        {
            SWRLDataPropertyAtom dataPropertyAtom = new SWRLDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv", "value");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = dataPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnConsequentWithLiteralAsRightArgument()
        {
            SWRLDataPropertyAtom dataPropertyAtom = new SWRLDataPropertyAtom(
                new OWLDataProperty(new RDFResource("ex:dtprop")), 
                new RDFVariable("?L"), 
                new RDFPlainLiteral("hello", "en-US"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = dataPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}