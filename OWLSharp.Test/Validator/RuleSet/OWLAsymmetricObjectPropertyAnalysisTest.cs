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
public class OWLAsymmetricObjectPropertyAnalysisTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeAsymmetricSymmetricObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                new OWLSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg1)));
    }

    [TestMethod]
    public void ShouldAnalyzeSimpleAsymmetricObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertiesWithInverseObjectAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertiesWithInverseObjectPropertyCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertiesWithInverseObjectPropertyAndInverseObjectAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricReflexiveObjectPropertiesCase()
    {
        //AsymmetricObjectProperty(OP) ^ ReflexiveObjectProperty(OP) -> ERROR
        //A reflexive property relates every individual to itself: it cannot be asymmetric
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertyOnTransitivePropertyCase()
    {
        //AsymmetricObjectProperty(OP) ^ TransitiveObjectProperty(OP) -> ERROR
        //A transitive property is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertyOnPropertyWithTransitiveSubPropertyCase()
    {
        //AsymmetricObjectProperty(OP) ^ SubObjectPropertyOf(SUBOP,OP) ^ TransitiveObjectProperty(SUBOP) -> ERROR
        //A property having a transitive subproperty is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:SUBOP"))),
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:SUBOP")), new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertyOnPropertyDefinedByChainCase()
    {
        //AsymmetricObjectProperty(OP) ^ SubObjectPropertyOf(ObjectPropertyChain(OP1,OP2),OP) -> ERROR
        //A property defined as the superproperty of a chain is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:OP1")), new OWLObjectProperty(new RDFResource("ex:OP2")) ]),
                    new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldNotAnalyzeAsymmetricObjectPropertyOnPropertyUnderTransitiveCase()
    {
        //AsymmetricObjectProperty(OP) ^ SubObjectPropertyOf(OP,SUPEROP) ^ TransitiveObjectProperty(SUPEROP) -> NO ISSUE
        //A subproperty of a transitive property stays simple (e.g. hasMother under hasAncestor)
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:SUPEROP"))),
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:OP")), new OWLObjectProperty(new RDFResource("ex:SUPEROP")))
            ]
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }
    #endregion
}