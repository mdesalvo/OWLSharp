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
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator;

[TestClass]
public class OWLDisjointUnionAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeDisjointUnionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLNamedIndividual(new RDFResource("ex:John")))
            ],
            ClassAxioms = [
                new OWLDisjointUnion(
                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                    [ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
            ]
        };
        Dictionary<string, object> validatorCache = new Dictionary<string, object>()
        {
            { "OPASN",  OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)}
        };
        List<OWLIssue> issues = OWLDisjointUnionAnalysisRule.ExecuteRule(ontology, validatorCache);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointUnionAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointUnionAnalysisRule.rulesugg)));
    }
    #endregion
}