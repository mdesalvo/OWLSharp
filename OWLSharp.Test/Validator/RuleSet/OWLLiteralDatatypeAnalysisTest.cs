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
public class OWLLiteralDatatypeAnalysisTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeLiteralDatatypeDataPropertyAssertionCase()
    {
        //This literal cannot be built through the public OWLLiteral(RDFLiteral) ctor, because RDFTypedLiteral
        //already validates eagerly against xsd:integer's value space and would throw RDFModelException.
        //The gap this rule closes is exactly this: an OWLLiteral surviving OWL2/XML deserialization (which
        //does NOT validate, since Value/DatatypeIRI are plain XmlText/XmlAttribute-bound strings) with a
        //lexical form that is not well-formed against its declared datatype (dt-not-type)
        OWLLiteral malformedIntegerLiteral = new OWLLiteral { Value = "not-a-number", DatatypeIRI = RDFVocabulary.XSD.INTEGER.ToString() };

        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    malformedIntegerLiteral)
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
            ]
        };
        List<OWLIssue> issues = OWLLiteralDatatypeAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLLiteralDatatypeAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLLiteralDatatypeAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeLiteralDatatypeNegativeDataPropertyAssertionCase()
    {
        OWLLiteral malformedBooleanLiteral = new OWLLiteral { Value = "not-a-boolean", DatatypeIRI = RDFVocabulary.XSD.BOOLEAN.ToString() };

        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLNegativeDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    malformedBooleanLiteral)
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
            ]
        };
        List<OWLIssue> issues = OWLLiteralDatatypeAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLLiteralDatatypeAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLLiteralDatatypeAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldNotAnalyzeLiteralDatatypeWellFormedCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLNegativeDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFPlainLiteral("Mark")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
            ]
        };
        List<OWLIssue> issues = OWLLiteralDatatypeAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(0, issues);
    }

    [TestMethod]
    public void ShouldNotAnalyzeLiteralDatatypeUnregisteredDatatypeCase()
    {
        //A custom/unregistered datatype has no known value space in RDFSharp's register, so it is out of scope
        //for dt-not-type (there is nothing to validate the lexical form against)
        OWLLiteral customDatatypeLiteral = new OWLLiteral { Value = "anything", DatatypeIRI = "ex:customDatatype" };

        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    customDatatypeLiteral)
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
            ]
        };
        List<OWLIssue> issues = OWLLiteralDatatypeAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(0, issues);
    }
    #endregion
}
