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
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLHasKeyHelperTest
    {
        #region Tests
		[TestMethod]
        public void ShouldDeclareHasKey()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareHasKey(new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]));

            Assert.IsTrue(ontology.KeyAxioms.Count == 1);
            Assert.IsTrue(ontology.CheckHasKey(new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)])));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareHasKey(null));

            ontology.DeclareHasKey(new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)])); //will be discarded, since duplicates are not allowed
            Assert.IsTrue(ontology.KeyAxioms.Count == 1);
        }
        #endregion
    }
}