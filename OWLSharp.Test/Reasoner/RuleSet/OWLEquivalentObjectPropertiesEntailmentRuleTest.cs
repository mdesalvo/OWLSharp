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
using OWLSharp.Reasoner;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner
{
    [TestClass]
    public class OWLEquivalentObjectPropertiesEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailEquivalentObjectPropertiesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/supports"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLEquivalentObjectProperties([
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps")),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf"))]),
					new OWLEquivalentObjectProperties([
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf")),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/supports"))])
				],
				AssertionAxioms = [
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ]
            };
            List<OWLInference> inferences = OWLEquivalentObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf 
                            && string.Equals(inf.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")
                            && string.Equals(inf.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")
							&& string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2 
                            && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
							&& string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf3
                            && string.Equals(inf3.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")
                            && string.Equals(inf3.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf4
                            && string.Equals(inf4.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf4.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf5
                            && string.Equals(inf5.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf5.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")));
        }

		[TestMethod]
        public void ShouldEntailEquivalentObjectPropertiesWithInverseObjectAssertionCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/supports"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ],
                ObjectPropertyAxioms = [
                    new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf"))]),
                    new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/supports"))])
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps"))),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ]
            };
            List<OWLInference> inferences = OWLEquivalentObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf
                            && string.Equals(inf.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")
                            && string.Equals(inf.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2
                            && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")
                            && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf3
                            && string.Equals(inf3.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")
                            && string.Equals(inf3.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf4
                            && string.Equals(inf4.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf4.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf5
                            && string.Equals(inf5.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf5.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")));
        }
        #endregion
    }
}