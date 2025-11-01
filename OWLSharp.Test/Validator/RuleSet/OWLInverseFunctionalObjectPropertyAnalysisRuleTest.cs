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
public class OWLInverseFunctionalObjectPropertyAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeInverseFunctionalObjectPropertySimpleCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))), //clash with first object assertion (because john and stiv are different idvs)
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John")) ])
            ],
            ObjectPropertyAxioms = [
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
            ]
        };
        List<OWLIssue> issues = OWLInverseFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLInverseFunctionalObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLInverseFunctionalObjectPropertyAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeInverseFunctionalObjectPropertyInverseAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:op1"))),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))), //clash with first object assertion (because john and stiv are different idvs)
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John")) ])
            ],
            ObjectPropertyAxioms = [
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
            ]
        };
        List<OWLIssue> issues = OWLInverseFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLInverseFunctionalObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLInverseFunctionalObjectPropertyAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeInverseFunctionalObjectPropertyInverseFOPCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))), //clash with first object assertion (because john and stiv are different idvs)
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John")) ])
            ],
            ObjectPropertyAxioms = [
                new OWLInverseFunctionalObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:op1")))),
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
            ]
        };
        List<OWLIssue> issues = OWLInverseFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLInverseFunctionalObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLInverseFunctionalObjectPropertyAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeInverseFunctionalObjectPropertyTransitiveCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:op1")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1")))
            ]
        };
        List<OWLIssue> issues = OWLInverseFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLInverseFunctionalObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLInverseFunctionalObjectPropertyAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeInverseFunctionalObjectPropertySuperTransitiveCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:op3"))),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLObjectProperty(new RDFResource("ex:op2"))),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLObjectProperty(new RDFResource("ex:op3")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op3")))
            ]
        };
        List<OWLIssue> issues = OWLInverseFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLInverseFunctionalObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLInverseFunctionalObjectPropertyAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeInverseFunctionalObjectPropertySimpleCaseAndDontFindIssuesBecauseNoDifferentFrom()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:op2")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John")))
            ],
            ObjectPropertyAxioms = [
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
            ]
        };
        List<OWLIssue> issues = OWLInverseFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }
    #endregion
}