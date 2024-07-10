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
    public class OWLEquivalentClassesEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailEquivalentClassesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mankind"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Humans"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/EarthMan")))
                ],
                ClassAxioms = [ 
                    new OWLEquivalentClasses([
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mankind")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Humans"))]),
					new OWLEquivalentClasses([
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Humans")),
						new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/EarthMan"))])
				]
            };
            List<OWLAxiom> inferences = OWLEquivalentClassesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.IsInference));
            Assert.IsTrue(inferences.Count == 4);
            Assert.IsTrue(inferences[0] is OWLEquivalentClasses inf 
                            && string.Equals(inf.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mankind")
                            && string.Equals(inf.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/EarthMan"));
			Assert.IsTrue(inferences[1] is OWLEquivalentClasses inf1
                            && string.Equals(inf1.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Humans")
                            && string.Equals(inf1.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mankind"));
			Assert.IsTrue(inferences[2] is OWLEquivalentClasses inf2
                            && string.Equals(inf2.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/EarthMan")
                            && string.Equals(inf2.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Humans"));
			Assert.IsTrue(inferences[3] is OWLEquivalentClasses inf3 
                            && string.Equals(inf3.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/EarthMan")
                            && string.Equals(inf3.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mankind"));
        }
        #endregion
    }
}