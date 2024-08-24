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
    public class SWRLSameAsAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSameAsAtomWithVariableAsRightArgument()
        {
            SWRLSameAsAtom SameAsAtom = new SWRLSameAsAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(SameAsAtom);
            Assert.IsNotNull(SameAsAtom.Predicate);
            Assert.IsTrue(SameAsAtom.Predicate.GetIRI().Equals(RDFVocabulary.OWL.SAME_AS));
            Assert.IsNotNull(SameAsAtom.LeftArgument);
            Assert.IsTrue(SameAsAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(SameAsAtom.RightArgument);
            Assert.IsTrue(SameAsAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(string.Equals("sameAs(?L,?R)", SameAsAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateSameAsAtomWithResourceAsRightArgument()
        {
            SWRLSameAsAtom SameAsAtom = new SWRLSameAsAtom(new RDFVariable("?L"), new RDFResource("ex:val"));

            Assert.IsNotNull(SameAsAtom);
            Assert.IsNotNull(SameAsAtom.Predicate);
            Assert.IsTrue(SameAsAtom.Predicate.GetIRI().Equals(RDFVocabulary.OWL.SAME_AS));
            Assert.IsNotNull(SameAsAtom.LeftArgument);
            Assert.IsTrue(SameAsAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(SameAsAtom.RightArgument);
            Assert.IsTrue(SameAsAtom.RightArgument.Equals(new RDFResource("ex:val")));
            Assert.IsTrue(string.Equals("sameAs(?L,ex:val)", SameAsAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameAsAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLSameAsAtom(null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameAsAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new SWRLSameAsAtom(new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameAsAtomBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new SWRLSameAsAtom(new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnAntecedentWithVariableAsRightArgument()
        {
            SWRLSameAsAtom sameAsAtom = new SWRLSameAsAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                ],
                AssertionAxioms = [
                     new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2"))])
                 ]
            };

            DataTable antecedentTable = sameAsAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnAntecedentWithResourceAsRightArgument()
        {
            SWRLSameAsAtom sameAsAtom = new SWRLSameAsAtom(new RDFVariable("?L"), new RDFResource("ex:indiv2"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv3"))),
                ],
                AssertionAxioms = [
                     new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("ex:indiv1")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv2")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv3"))])
                 ]
            };

            DataTable antecedentTable = sameAsAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnConsequentWithVariableAsRightArgument()
        {
            SWRLSameAsAtom SameAsAtom = new SWRLSameAsAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv1", "ex:indiv2");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = SameAsAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnConsequentWithResourceAsRightArgument()
        {
            SWRLSameAsAtom SameAsAtom = new SWRLSameAsAtom(new RDFVariable("?L"), new RDFResource("ex:val1"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = SameAsAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
        }
        #endregion
    }
}