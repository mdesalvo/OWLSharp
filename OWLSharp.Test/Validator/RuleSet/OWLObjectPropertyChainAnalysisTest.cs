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
public class OWLObjectPropertyChainAnalysisTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainAsymmetricCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainFunctionalCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainInverseFunctionalCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainIrreflexiveCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldNotAnalyzeObjectPropertyChainSuperPropertyAsLastElementCase()
    {
        //SubObjectPropertyOf(ObjectPropertyChain(hasFather,hasUncle),hasUncle) -> NO ISSUE
        //OWL2 DL regularity admits the superproperty as the last element of the chain (Q o OP => OP)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldNotAnalyzeObjectPropertyChainSuperPropertyAsFirstElementCase()
    {
        //SubObjectPropertyOf(ObjectPropertyChain(OP,Q),OP) -> NO ISSUE
        //OWL2 DL regularity admits the superproperty as the first element of the chain (OP o Q => OP)
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:OP")), new OWLObjectProperty(new RDFResource("ex:Q")) ]),
                    new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldNotAnalyzeObjectPropertyChainSuperPropertyAsFirstAndLastElementOfTwoCase()
    {
        //SubObjectPropertyOf(ObjectPropertyChain(OP,OP),OP) -> NO ISSUE
        //OP o OP => OP is the legal two-element case with the superproperty at both ends
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:OP")), new OWLObjectProperty(new RDFResource("ex:OP")) ]),
                    new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainSuperPropertyAsInnerElementCase()
    {
        //SubObjectPropertyOf(ObjectPropertyChain(Q,OP,R),OP) -> ERROR
        //The superproperty in an inner position (neither first nor last) makes the chain irregular
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:Q")), new OWLObjectProperty(new RDFResource("ex:OP")), new OWLObjectProperty(new RDFResource("ex:R")) ]),
                    new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainSuperPropertyAtBothEndsOfLongerChainCase()
    {
        //SubObjectPropertyOf(ObjectPropertyChain(OP,Q,OP),OP) -> ERROR
        //The superproperty at both ends of a chain longer than two elements makes the chain irregular
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:OP")), new OWLObjectProperty(new RDFResource("ex:Q")), new OWLObjectProperty(new RDFResource("ex:OP")) ]),
                    new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysis.rulesugg2)));
    }
    #endregion
}