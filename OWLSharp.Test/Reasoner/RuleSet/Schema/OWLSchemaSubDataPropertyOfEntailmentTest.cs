/*
  Copyright 2014-2026 Marco De Salvo
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
public class OWLSchemaSubDataPropertyOfEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailSubDataPropertyOfTransitivityCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasCharacteristic"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTemporalCharacteristic"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")))
            ],
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic")),
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasCharacteristic"))),
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))),
                new OWLEquivalentDataProperties([
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")),
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))])
            ],
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                    new OWLLiteral(new RDFTypedLiteral("26", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
            ]
        };
        List<OWLInference> inferences = OWLSchemaSubDataPropertyOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubDataPropertyOf inf
                                          && string.Equals(inf.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")
                                          && string.Equals(inf.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubDataPropertyOf inf1
                                          && string.Equals(inf1.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                                          && string.Equals(inf1.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubDataPropertyOf inf2
                                          && string.Equals(inf2.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                                          && string.Equals(inf2.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")));
        Assert.IsFalse(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion));
    }
    #endregion
}
