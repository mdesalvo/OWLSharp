/*
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
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator
{
    [TestClass]
    public class OWLTermsDisjointnessAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeDisjointnessTermsClassCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person"))),
					new OWLDeclaration(new OWLDatatype(new RDFResource("http://xmlns.com/foaf/0.1/Person")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDisjointnessAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDisjointnessAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected clash on terms disjointness for class with IRI: 'http://xmlns.com/foaf/0.1/Person'")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDisjointnessAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public void ShouldAnalyzeDisjointnessTermsDatatypeCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.INTEGER)),
					new OWLDeclaration(new OWLDataProperty(RDFVocabulary.XSD.INTEGER))
                ]
            };
            List<OWLIssue> issues = OWLTermsDisjointnessAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDisjointnessAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected clash on terms disjointness for datatype with IRI: 'http://www.w3.org/2001/XMLSchema#integer'")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDisjointnessAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public void ShouldAnalyzeDisjointnessTermsDataPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/age"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/age")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDisjointnessAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDisjointnessAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected clash on terms disjointness for data property with IRI: 'http://xmlns.com/foaf/0.1/age'")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDisjointnessAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public void ShouldAnalyzeDisjointnessTermsObjectPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
					new OWLDeclaration(new OWLAnnotationProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDisjointnessAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDisjointnessAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected clash on terms disjointness for object property with IRI: 'http://xmlns.com/foaf/0.1/knows'")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDisjointnessAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public void ShouldAnalyzeDisjointnessTermsAnnotationPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLAnnotationProperty(new RDFResource("http://xmlns.com/foaf/0.1/author"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/author")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDisjointnessAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDisjointnessAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected clash on terms disjointness for annotation property with IRI: 'http://xmlns.com/foaf/0.1/author'")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDisjointnessAnalysisRule.rulesugg)));
        }
		#endregion
    }
}