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
public class OWLFactEquivalentDataPropertiesEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailEquivalentDataPropertiesAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/isOld"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/wasBornNYearsAgo"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")))
            ],
            DataPropertyAxioms = [
                new OWLEquivalentDataProperties([
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/wasBornNYearsAgo"))]),
                new OWLEquivalentDataProperties([
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/wasBornNYearsAgo")),
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/isOld"))])
            ],
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                    new OWLLiteral(new RDFTypedLiteral("26", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
            ]
        };
        List<OWLInference> inferences = OWLFactEquivalentDataPropertiesEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsFalse(inferences.Any(i => i.Axiom is OWLEquivalentDataProperties));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf4
                                          && string.Equals(inf4.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/wasBornNYearsAgo")
                                          && string.Equals(inf4.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                                          && string.Equals(inf4.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf5
                                          && string.Equals(inf5.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isOld")
                                          && string.Equals(inf5.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                                          && string.Equals(inf5.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer")));
    }
    #endregion
}
