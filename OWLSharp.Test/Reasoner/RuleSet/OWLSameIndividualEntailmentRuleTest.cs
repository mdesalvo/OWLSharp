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
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner
{
    [TestClass]
    public class OWLSameIndividualEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailSameIndividualTransitivityCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marek"))),
                ],
                AssertionAxioms = [
                    new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marek"))
                    ])
                ]
            };
            List<OWLInference> inferences = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        }

        [TestMethod]
        public void ShouldEntailSameIndividualSimpleObjectAssertionSourceCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                AssertionAxioms = [
                    new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))
                    ]),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
                ]
            };
            List<OWLInference> inferences = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLObjectPropertyAssertion opAsn
                                                    && string.Equals(opAsn.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                                                    && string.Equals(opAsn.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Marco")
                                                    && string.Equals(opAsn.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public void ShouldEntailSameIndividualSimpleObjectAssertionTargetCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                AssertionAxioms = [
                    new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))
                    ]),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
                ]
            };
            List<OWLInference> inferences = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLObjectPropertyAssertion opAsn
                                                    && string.Equals(opAsn.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                                                    && string.Equals(opAsn.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                                                    && string.Equals(opAsn.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Marco")));
        }

        [TestMethod]
        public void ShouldEntailSameIndividualInverseObjectAssertionTargetCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                AssertionAxioms = [
                    new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))
                    ]),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
                ]
            };
            List<OWLInference> inferences = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLObjectPropertyAssertion opAsn
                                                    && string.Equals(opAsn.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                                                    && string.Equals(opAsn.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Marco")
                                                    && string.Equals(opAsn.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public void ShouldEntailSameIndividualDataAssertionCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco")))
                ],
                AssertionAxioms = [
                    new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))
                    ]),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                ]
            };
            List<OWLInference> inferences = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLDataPropertyAssertion dpAsn
                                                    && string.Equals(dpAsn.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/age")
                                                    && string.Equals(dpAsn.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Marco")
                                                    && string.Equals(dpAsn.Literal.GetLiteral().ToString(), "32^^http://www.w3.org/2001/XMLSchema#integer")));
        }

        [TestMethod]
        public void ShouldEntailSameAnonymousIndividualDataAssertionCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
                ],
                AssertionAxioms = [
                    new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLAnonymousIndividual("Marco")
                    ]),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLAnonymousIndividual("Marco"),
                        new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                ]
            };
            List<OWLInference> inferences = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLDataPropertyAssertion dpAsn
                                                    && string.Equals(dpAsn.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/age")
                                                    && string.Equals(dpAsn.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")
                                                    && string.Equals(dpAsn.Literal.GetLiteral().ToString(), "32^^http://www.w3.org/2001/XMLSchema#integer")));
        }
        #endregion
    }
}