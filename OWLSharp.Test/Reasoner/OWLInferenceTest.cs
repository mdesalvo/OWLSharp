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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner
{
    [TestClass]
    public class OWLInferenceTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateInference()
        {
            OWLInference inference = new OWLInference(
                " rulename ",
                new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLNamedIndividual(new RDFResource("ex:Mark"))));

            Assert.IsNotNull(inference);
            Assert.IsTrue(string.Equals(inference.RuleName, "rulename"));
            Assert.IsTrue(string.Equals(inference.ToString(), inference.Axiom.GetXML()));
        }

        [TestMethod]
        public void ShouldThrowExceptiononCreatingInferenceWithNullRuleName()
            => Assert.ThrowsException<OWLException>(() => new OWLInference(null, new OWLAxiom()));

        [TestMethod]
        public void ShouldThrowExceptiononCreatingInferenceWithNullAxiom()
            => Assert.ThrowsException<OWLException>(() => new OWLInference("testRule", null));

        [TestMethod]
        public void ShouldCompareInferences()
        {
            OWLInference inferenceA = new OWLInference(
                "rulenameB",
                new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLNamedIndividual(new RDFResource("ex:Mark"))));
            OWLInference inferenceB = new OWLInference(
                 "rulenameB",
                 new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLNamedIndividual(new RDFResource("ex:Mark"))));

            Assert.IsTrue(inferenceA.Equals(inferenceB));
            Assert.IsTrue(inferenceB.Equals(inferenceA));
        }

        [TestMethod]
        public void ShouldEliminateDuplicateInferences()
        {
            List<OWLInference> inferences = [
                new OWLInference(
                    "rulenameB",
                    new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLNamedIndividual(new RDFResource("ex:Mark")))),
                new OWLInference(
                     "rulenameB",
                     new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLNamedIndividual(new RDFResource("ex:Mark")))),
                new OWLInference(
                     "rulenameC",
                     new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLNamedIndividual(new RDFResource("ex:Stiv")))),
            ];

            Assert.IsTrue(inferences.Distinct().Count() == 2);
            Assert.IsTrue(inferences[0].GetHashCode() == inferences[1].GetHashCode());
        }
        #endregion
    }
}