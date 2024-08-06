﻿/*
  Copyright 2014-2024 Marco De Salvo
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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Validator;
using OWLSharp.Validator.Rules;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator.Rules
{
    [TestClass]
    public class OWLFunctionalObjectPropertyAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeFunctionalObjectPropertySimpleCase()
        {
            OWLOntology ontology = new OWLOntology()
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
					new OWLFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op1"))),
					new OWLFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:op2")))
				],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
                ]
            };
            List<OWLIssue> issues = OWLFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLFunctionalObjectPropertyAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLFunctionalObjectPropertyAnalysisRule.rulesugg)));
        }
        #endregion
    }
}