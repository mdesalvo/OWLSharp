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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Helpers
{
    [TestClass]
    public class OWLObjectPropertyAxiomHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldGetObjectPropertyAxioms()
		{
			OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:hasFather")), new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]), new OWLObjectProperty(new RDFResource("ex:hasUncle"))),
					new OWLTransitiveObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLSymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLReflexiveObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLObjectPropertyRange(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLClass(RDFVocabulary.FOAF.PERSON)),
					new OWLObjectPropertyDomain(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLClass(RDFVocabulary.FOAF.PERSON)),
					new OWLIrreflexiveObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLInverseObjectProperties(new OWLObjectProperty(new RDFResource("ex:hasWife")), new OWLObjectProperty(new RDFResource("ex:isWifeOf"))),
					new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLFunctionalObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLEquivalentObjectProperties([ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]),
					new OWLDisjointObjectProperties([ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER), new OWLObjectProperty(RDFVocabulary.FOAF.ACCOUNT) ]),
					new OWLAsymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ]
            };

            List<OWLSubObjectPropertyOf> subObjectPropertyOf = ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>();
            Assert.IsTrue(subObjectPropertyOf.Count == 1);

			List<OWLTransitiveObjectProperty> transitiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>();
            Assert.IsTrue(transitiveObjectProperty.Count == 1);

			List<OWLSymmetricObjectProperty> symmetricObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>();
            Assert.IsTrue(symmetricObjectProperty.Count == 1);

			List<OWLReflexiveObjectProperty> reflexiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>();
            Assert.IsTrue(reflexiveObjectProperty.Count == 1);

			List<OWLObjectPropertyRange> objectPropertyRange = ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>();
            Assert.IsTrue(objectPropertyRange.Count == 1);

			List<OWLObjectPropertyDomain> objectPropertyDomain = ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>();
            Assert.IsTrue(objectPropertyDomain.Count == 1);

			List<OWLIrreflexiveObjectProperty> irreflexiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>();
            Assert.IsTrue(irreflexiveObjectProperty.Count == 1);

			List<OWLInverseObjectProperties> inverseObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();
            Assert.IsTrue(inverseObjectProperty.Count == 1);

			List<OWLInverseFunctionalObjectProperty> inverseFunctionalObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>();
            Assert.IsTrue(inverseFunctionalObjectProperty.Count == 1);

			List<OWLFunctionalObjectProperty> functionalObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>();
            Assert.IsTrue(functionalObjectProperty.Count == 1);

			List<OWLEquivalentObjectProperties> equivalentObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>();
            Assert.IsTrue(equivalentObjectProperty.Count == 1);

			List<OWLDisjointObjectProperties> disjointObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>();
            Assert.IsTrue(disjointObjectProperty.Count == 1);

			List<OWLAsymmetricObjectProperty> asymmetricObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>();
            Assert.IsTrue(asymmetricObjectProperty.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Count == 0);
        }
		#endregion
	}
}