/*
  Copyright 2014-2025 Marco De Salvo
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

namespace OWLSharp.Test.Reasoner;

[TestClass]
public class OWLSubObjectPropertyOfEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailSubObjectPropertyOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTopFriend"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend")),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend"))),
                new OWLEquivalentObjectProperties([
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend")),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTopFriend"))])
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
            ]
        };
        List<OWLInference> inferences = OWLSubObjectPropertyOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubObjectPropertyOf inf
                                          && string.Equals(inf.SubObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasBestFriend")
                                          && string.Equals(inf.SuperObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                                          && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasFriend")
                                          && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                                          && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2
                                          && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                                          && string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                                          && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubObjectPropertyOf inf3
                                          && string.Equals(inf3.SubObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTopFriend")
                                          && string.Equals(inf3.SuperObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasFriend")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubObjectPropertyOf inf4
                                          && string.Equals(inf4.SubObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTopFriend")
                                          && string.Equals(inf4.SuperObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
    }

    [TestMethod]
    public void ShouldEntailSubObjectPropertyOfWithInverseObjectAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend")),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend"))),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
            ]
        };
        List<OWLInference> inferences = OWLSubObjectPropertyOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubObjectPropertyOf inf
                                          && string.Equals(inf.SubObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasBestFriend")
                                          && string.Equals(inf.SuperObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                                          && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasFriend")
                                          && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")
                                          && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2
                                          && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                                          && string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")
                                          && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")));
    }
    #endregion
}