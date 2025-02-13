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
    public class OWLDisjointDataPropertiesEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailDisjointDataPropertiesCase()
        {
            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasName"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")))
                ],
                DataPropertyAxioms = [ 
                    new OWLDisjointDataProperties([
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasName"))])
                ]
            };
            List<OWLInference> inferences = OWLDisjointDataPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointDataProperties inf
                            && string.Equals(inf.DataProperties[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasName")
                            && string.Equals(inf.DataProperties[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")));
        }
        #endregion
    }
}