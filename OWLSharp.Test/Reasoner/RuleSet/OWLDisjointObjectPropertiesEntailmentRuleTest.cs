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
using OWLSharp.Reasoner;
using OWLSharp.Reasoner.RuleSet;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner.RuleSet
{
    [TestClass]
    public class OWLDisjointObjectPropertiesEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailDisjointObjectPropertiesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/avoids"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
					new OWLDisjointObjectProperties([
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/avoids"))])
                ]
            };
            List<OWLInference> inferences = OWLDisjointObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointObjectProperties inf 
                            && string.Equals(inf.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/avoids")
                            && string.Equals(inf.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
        }

        [TestMethod]
        public void ShouldEntailSubObjectPropertyOfWithInverseObjectAssertionCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/avoids"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
					new OWLDisjointObjectProperties([
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
						new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/avoids")))])
                ]
            };
            List<OWLInference> inferences = OWLDisjointObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointObjectProperties inf 
                            && inf.ObjectPropertyExpressions[0] is OWLObjectInverseOf objInvOf
							 && string.Equals(objInvOf.ObjectProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/avoids")
                            && string.Equals(inf.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
        }
        #endregion
    }
}