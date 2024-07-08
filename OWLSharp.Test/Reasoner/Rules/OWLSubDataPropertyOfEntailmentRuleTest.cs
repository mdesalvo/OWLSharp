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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Reasoner.Rules;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Reasoner.Rules
{
    [TestClass]
    public class OWLSubDataPropertyOfEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailSubDataPropertyOfCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasCharacteristic"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")))
                ],
                DataPropertyAxioms = [ 
                    new OWLSubDataPropertyOf(
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic")),
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasCharacteristic"))),
					new OWLSubDataPropertyOf(
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic")))
				],
				AssertionAxioms = [
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
						new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
						new OWLLiteral(new RDFTypedLiteral("26", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                ]
            };
            List<OWLAxiom> inferences = OWLSubDataPropertyOfEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.IsInference));
            Assert.IsTrue(inferences.Count == 3);
            Assert.IsTrue(inferences[0] is OWLSubDataPropertyOf inf 
                            && string.Equals(inf.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                            && string.Equals(inf.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic"));
			Assert.IsTrue(inferences[1] is OWLDataPropertyAssertion inf1 
                            && string.Equals(inf1.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTimeCharacteristic")
							&& string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf1.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer"));
			Assert.IsTrue(inferences[2] is OWLDataPropertyAssertion inf2 
                            && string.Equals(inf2.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic")
							&& string.Equals(inf2.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf2.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer"));
        }
        #endregion
    }
}