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
    public class SWRLObjectPropertyAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectPropertyAtomWithVariableAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")), 
                new RDFVariable("?L"), 
                new RDFVariable("?R"));

            Assert.IsNotNull(objectPropertyAtom);
            Assert.IsNotNull(objectPropertyAtom.Predicate);
            Assert.IsTrue(objectPropertyAtom.Predicate.GetIRI().Equals(new RDFResource("ex:objprop")));
            Assert.IsNotNull(objectPropertyAtom.LeftArgument);
            Assert.IsTrue(objectPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(objectPropertyAtom.RightArgument);
            Assert.IsTrue(objectPropertyAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(string.Equals("ex:objprop(?L,?R)", objectPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateObjectPropertyAtomWithResourceAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")),
                new RDFVariable("?L"), 
                new RDFResource("ex:val"));

            Assert.IsNotNull(objectPropertyAtom);
            Assert.IsNotNull(objectPropertyAtom.Predicate);
            Assert.IsTrue(objectPropertyAtom.Predicate.GetIRI().Equals(new RDFResource("ex:objprop")));
            Assert.IsNotNull(objectPropertyAtom.LeftArgument);
            Assert.IsTrue(objectPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(objectPropertyAtom.RightArgument);
            Assert.IsTrue(objectPropertyAtom.RightArgument.Equals(new RDFResource("ex:val")));
            Assert.IsTrue(string.Equals("ex:objprop(?L,ex:val)", objectPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new SWRLObjectPropertyAtom(null, new RDFVariable("?L"), new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("ex:objprop")), null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("ex:objprop")), new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("ex:objprop")), new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnAntecedentWithVariableAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")), 
                new RDFVariable("?SRC_IDV"), 
                new RDFVariable("?TGT_IDV"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:objprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:objprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")))
                ]
            };
            DataTable antecedentTable = objectPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnAntecedentWithInverseObjectAssertionAndVariableAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")),
                new RDFVariable("?SRC_IDV"),
                new RDFVariable("?TGT_IDV"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:objprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objprop"))),
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")))
                ]
            };
            DataTable antecedentTable = objectPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnAntecedentWithResourceAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")),
                new RDFVariable("?SRC_IDV"), 
                new RDFResource("ex:indiv2"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:objprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:objprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")))
                ]
            };
            DataTable antecedentTable = objectPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnAntecedentWithInverseObjectAssertionAndResourceAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")),
                new RDFVariable("?SRC_IDV"),
                new RDFResource("ex:indiv1"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:objprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objprop"))),
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")))
                ]
            };
            DataTable antecedentTable = objectPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnConsequentWithVariableAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")),
                new RDFVariable("?SRC_IDV"), 
                new RDFVariable("?TGT_IDV"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?SRC_IDV");
            antecedentTable.Columns.Add("?TGT_IDV");
            antecedentTable.Rows.Add("ex:indiv1", "ex:indiv2");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = objectPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnConsequentWithResourceAsRightArgument()
        {
            SWRLObjectPropertyAtom objectPropertyAtom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(new RDFResource("ex:objprop")),
                new RDFVariable("?SRC_IDV"), 
                new RDFResource("ex:indiv2"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?SRC_IDV");
            antecedentTable.Rows.Add("ex:indiv1");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = objectPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}