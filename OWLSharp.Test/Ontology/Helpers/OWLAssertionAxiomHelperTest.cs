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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Helpers
{
    [TestClass]
    public class OWLAssertionAxiomHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldGetAssertionAxioms()
		{
			OWLOntology ontology = new OWLOntology()
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
					new OWLDataPropertyAssertion(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLNamedIndividual(new RDFResource("ex:Carl")) ]),
					new OWLNegativeDataPropertyAssertion(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLNegativeObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
					new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
					new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLNamedIndividual(new RDFResource("ex:Carl")) ])
                ]
            };

            List<OWLClassAssertion> classAssertion = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            Assert.IsTrue(classAssertion.Count == 1);

			List<OWLDataPropertyAssertion> dataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            Assert.IsTrue(dataPropertyAssertion.Count == 1);

			List<OWLDifferentIndividuals> differentIndividuals = ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>();
            Assert.IsTrue(differentIndividuals.Count == 1);

			List<OWLNegativeDataPropertyAssertion> negativeDataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>();
            Assert.IsTrue(negativeDataPropertyAssertion.Count == 1);

			List<OWLNegativeObjectPropertyAssertion> negativeObjectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>();
            Assert.IsTrue(negativeObjectPropertyAssertion.Count == 1);

			List<OWLObjectPropertyAssertion> objectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            Assert.IsTrue(objectPropertyAssertion.Count == 1);

			List<OWLSameIndividual> sameIndividual = ontology.GetAssertionAxiomsOfType<OWLSameIndividual>();
            Assert.IsTrue(sameIndividual.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 0);
        }
		#endregion
	}
}