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
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTemporalCharacteristic"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")))
                ],
                DataPropertyAxioms = [ 
                    new OWLSubDataPropertyOf(
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic")),
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasCharacteristic"))),
					new OWLSubDataPropertyOf(
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
						new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))),
                    new OWLEquivalentDataProperties([
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))])
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
            Assert.IsTrue(inferences.Count == 6);
            Assert.IsTrue(inferences[0] is OWLSubDataPropertyOf inf
                            && string.Equals(inf.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")
                            && string.Equals(inf.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic"));
            Assert.IsTrue(inferences[1] is OWLSubDataPropertyOf inf1 
                            && string.Equals(inf1.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                            && string.Equals(inf1.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic"));
            Assert.IsTrue(inferences[2] is OWLSubDataPropertyOf inf2
                            && string.Equals(inf2.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                            && string.Equals(inf2.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic"));
            Assert.IsTrue(inferences[3] is OWLDataPropertyAssertion inf3 
                            && string.Equals(inf3.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTimeCharacteristic")
							&& string.Equals(inf3.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf3.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer"));
			Assert.IsTrue(inferences[4] is OWLDataPropertyAssertion inf4 
                            && string.Equals(inf4.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic")
							&& string.Equals(inf4.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf4.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer"));
            Assert.IsTrue(inferences[5] is OWLDataPropertyAssertion inf5
                            && string.Equals(inf5.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")
                            && string.Equals(inf5.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf5.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer"));
        }
        #endregion
    }
}