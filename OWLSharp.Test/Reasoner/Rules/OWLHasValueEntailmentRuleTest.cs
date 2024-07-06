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
    public class OWLHasValueEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailObjectHasValueCase()
        {


            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
					new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions"))),
                ],
				ClassAxioms = [
					new OWLSubClassOf(
						new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
						new OWLObjectHasValue(
							new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj")),
							new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))))
				],
				AssertionAxioms = [
					new OWLClassAssertion(
						new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
						new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions")))
				]
            };
            List<OWLAxiom> inferences = OWLHasValueEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.IsInference));
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0] is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://frede.gat/stuff#propObj")
							&& string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemDefinedByClassRestrictions")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny"));
        }
        #endregion
    }
}