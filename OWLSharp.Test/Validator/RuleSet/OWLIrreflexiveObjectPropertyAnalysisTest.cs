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
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator;

[TestClass]
public class OWLIrreflexiveObjectPropertyAnalysisTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeIrreflexiveReflexiveObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")))
            ],
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
                new OWLReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg1)));
    }

    [TestMethod]
    public void ShouldAnalyzeSimpleIrreflexiveObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeIrreflexiveObjectPropertiesWithInverseObjectAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeIrreflexiveObjectPropertiesWithInverseObjectPropertyCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeIrreflexiveObjectPropertiesWithInverseObjectPropertyAndInverseObjectAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeIrreflexiveObjectPropertyOnTransitivePropertyCase()
    {
        //IrreflexiveObjectProperty(OP) ^ TransitiveObjectProperty(OP) -> ERROR
        //A transitive property is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldAnalyzeIrreflexiveObjectPropertyOnPropertyWithTransitiveSubPropertyCase()
    {
        //IrreflexiveObjectProperty(OP) ^ SubObjectPropertyOf(SUBOP,OP) ^ TransitiveObjectProperty(SUBOP) -> ERROR
        //A property having a transitive subproperty is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:SUBOP"))),
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:SUBOP")), new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldAnalyzeIrreflexiveObjectPropertyOnPropertyDefinedByChainCase()
    {
        //IrreflexiveObjectProperty(OP) ^ SubObjectPropertyOf(ObjectPropertyChain(OP1,OP2),OP) -> ERROR
        //A property defined as the superproperty of a chain is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:OP1")), new OWLObjectProperty(new RDFResource("ex:OP2")) ]),
                    new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldNotAnalyzeIrreflexiveObjectPropertyOnPropertyUnderTransitiveCase()
    {
        //IrreflexiveObjectProperty(OP) ^ SubObjectPropertyOf(OP,SUPEROP) ^ TransitiveObjectProperty(SUPEROP) -> NO ISSUE
        //A subproperty of a transitive property stays simple (e.g. hasMother under hasAncestor)
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:SUPEROP"))),
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:OP")), new OWLObjectProperty(new RDFResource("ex:SUPEROP")))
            ]
        };
        List<OWLIssue> issues = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }
    #endregion
}