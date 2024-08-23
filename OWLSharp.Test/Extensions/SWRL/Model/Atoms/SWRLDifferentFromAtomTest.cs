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
    public class SWRLDifferentFromAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDifferentFromAtomWithVariableAsRightArgument()
        {
            SWRLDifferentFromAtom differentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(differentFromAtom);
            Assert.IsNotNull(differentFromAtom.Predicate);
            Assert.IsTrue(differentFromAtom.Predicate.GetIRI().Equals(RDFVocabulary.OWL.DIFFERENT_FROM));
            Assert.IsNotNull(differentFromAtom.LeftArgument);
            Assert.IsTrue(differentFromAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(differentFromAtom.RightArgument);
            Assert.IsTrue(differentFromAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(string.Equals("differentFrom(?L,?R)", differentFromAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateDifferentFromAtomWithResourceAsRightArgument()
        {
            SWRLDifferentFromAtom differentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFResource("ex:val"));

            Assert.IsNotNull(differentFromAtom);
            Assert.IsNotNull(differentFromAtom.Predicate);
            Assert.IsTrue(differentFromAtom.Predicate.GetIRI().Equals(RDFVocabulary.OWL.DIFFERENT_FROM));
            Assert.IsNotNull(differentFromAtom.LeftArgument);
            Assert.IsTrue(differentFromAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(differentFromAtom.RightArgument);
            Assert.IsTrue(differentFromAtom.RightArgument.Equals(new RDFResource("ex:val")));
            Assert.IsTrue(string.Equals("differentFrom(?L,ex:val)", differentFromAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDifferentFromAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLDifferentFromAtom(null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDifferentFromAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new SWRLDifferentFromAtom(new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDifferentFromAtomBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new SWRLDifferentFromAtom(new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnAntecedentWithVariableAsRightArgument()
        {
            SWRLDifferentFromAtom differentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                ],
                AssertionAxioms = [
                     new OWLDifferentIndividuals([
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2"))])
                 ]
            };

            DataTable antecedentTable = differentFromAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnAntecedentWithResourceAsRightArgument()
        {
            SWRLDifferentFromAtom differentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFResource("ex:indiv2"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv3"))),
                ],
                AssertionAxioms = [
                     new OWLDifferentIndividuals([
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv3"))])
                 ]
            };

            DataTable antecedentTable = differentFromAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnConsequentWithVariableAsRightArgument()
        {
            SWRLDifferentFromAtom differentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv1", "ex:indiv2");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = differentFromAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnConsequentWithResourceAsRightArgument()
        {
            SWRLDifferentFromAtom differentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFResource("ex:val1"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = differentFromAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}