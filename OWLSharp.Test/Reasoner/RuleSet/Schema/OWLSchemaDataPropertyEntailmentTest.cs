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
public class OWLSchemaDataPropertyEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailSchemaDataPropertyCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasAge")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaDataPropertyEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubDataPropertyOf spof
                                          && string.Equals(spof.SubDataProperty.GetIRI().ToString(), "ex:hasAge")
                                          && string.Equals(spof.SuperDataProperty.GetIRI().ToString(), "ex:hasAge")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentDataProperties eqdp
                                          && eqdp.DataProperties.TrueForAll(dp => string.Equals(dp.GetIRI().ToString(), "ex:hasAge"))));
    }

    [TestMethod]
    public void ShouldNotEntailSchemaDataPropertyBecauseNoDeclaredDataProperties()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasFriend")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaDataPropertyEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
