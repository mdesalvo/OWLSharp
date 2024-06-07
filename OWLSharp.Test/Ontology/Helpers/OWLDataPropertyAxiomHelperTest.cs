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
    public class OWLDataPropertyAxiomHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldGetDataPropertyAxioms()
		{
			OWLOntology ontology = new OWLOntology()
            {
                DataPropertyAxioms = [
                    new OWLSubDataPropertyOf(new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLDataProperty(RDFVocabulary.DC.TITLE)),
					new OWLFunctionalDataProperty(new OWLDataProperty(RDFVocabulary.RDFS.LABEL)),
					new OWLEquivalentDataProperties([ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]),
					new OWLDisjointDataProperties([ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.NAME), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]),
					new OWLDataPropertyRange(new OWLDataProperty(RDFVocabulary.RDFS.COMMENT), new OWLDatatype(RDFVocabulary.XSD.STRING)),
					new OWLDataPropertyDomain(new OWLDataProperty(RDFVocabulary.RDFS.COMMENT), new OWLClass(RDFVocabulary.FOAF.PERSON))
                ]
            };

            List<OWLSubDataPropertyOf> subDataPropertyOf = ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>();
            Assert.IsTrue(subDataPropertyOf.Count == 1);

            List<OWLFunctionalDataProperty> functionalDataProperty = ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>();
            Assert.IsTrue(functionalDataProperty.Count == 1);

            List<OWLEquivalentDataProperties> equivalentDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>();
            Assert.IsTrue(equivalentDataProperties.Count == 1);

            List<OWLDisjointDataProperties> disjointDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>();
            Assert.IsTrue(disjointDataProperties.Count == 1);

			List<OWLDataPropertyRange> dataPropertyRange = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>();
            Assert.IsTrue(dataPropertyRange.Count == 1);

			List<OWLDataPropertyDomain> dataPropertyDomain = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>();
            Assert.IsTrue(dataPropertyDomain.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Count == 0);
        }
		#endregion
	}
}