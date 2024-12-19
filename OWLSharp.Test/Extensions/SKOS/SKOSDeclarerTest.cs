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
using OWLSharp.Extensions.SKOS;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.SKOS
{
    [TestClass]
    public class SKOSDeclarerTest
    {
        #region Tests
        [TestMethod]
        public void ShouldDeclareConceptScheme()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareSKOSConceptScheme(new RDFResource("ex:ConceptScheme"), [new RDFResource("ex:ConceptA")]);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 5);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 2);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareSKOSConceptScheme(null, [ new RDFResource("ex:ConceptScheme") ]));
        }

        [TestMethod]
        public void ShouldDeclareConcept()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareSKOSConcept(new RDFResource("ex:Concept"), [
                new RDFPlainLiteral("This is a concept"), 
                new RDFPlainLiteral("This is a concept", "en-US")]);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count == 2);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareSKOSConcept(null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareSKOSConcept(new RDFResource("ex:Concept"), [
                new RDFPlainLiteral("This is a concept"), new RDFPlainLiteral("This is the same concept")]));
        }

        [TestMethod]
        public void ShouldDeclareCollection()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareSKOSCollection(new RDFResource("ex:Collection"), 
                [ new RDFResource("ex:ConceptA"), new RDFResource("ex:ConceptB") ], 
                [ new RDFPlainLiteral("This is a collection"), new RDFPlainLiteral("This is a collection", "en-US") ]);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 7);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count == 2);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareSKOSCollection(null, [ new RDFResource("ex:ConceptA") ]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareSKOSCollection(new RDFResource("ex:Collection"), null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareSKOSCollection(new RDFResource("ex:Collection"), []));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareSKOSCollection(new RDFResource("ex:Collection"), 
                [ new RDFResource("ex:ConceptA"), new RDFResource("ex:ConceptB") ],
                [ new RDFPlainLiteral("This is a collection", "en-US"), new RDFPlainLiteral("This is the same collection", "en-US") ]));
        }
        #endregion
    }
}