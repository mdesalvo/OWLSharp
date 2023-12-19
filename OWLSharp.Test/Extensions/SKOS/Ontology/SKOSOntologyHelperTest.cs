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
using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSOntologyHelperTest
    {
        #region Tests (Declarer)
        [TestMethod]
        public void ShouldCreateOntology()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();

            //Test initialization of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(new Uri("ex:ontology")));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 0);

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 0);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 0);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 0);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 0);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 0);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 0);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldDeclareConceptScheme()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConceptScheme(new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:conceptScheme"), RDFVocabulary.SKOS.CONCEPT_SCHEME));

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 0);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 0);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 0);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 0);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptSchemeBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareConceptScheme(new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptSchemeBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareConceptScheme(null));

        [TestMethod]
        public void ShouldDeclareConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:concept")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:concept"), RDFVocabulary.SKOS.CONCEPT));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 1);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 1);

            Assert.IsTrue(ontology.GetCollectionsCount() == 0);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 0);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 0);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 0);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }
        
        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareConcept(null, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareConcept(new RDFResource("ex:concept"), null));
        
        [TestMethod]
        public void ShouldDeclareCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.CheckHasCollection(new RDFResource("ex:collection")));
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:collection")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:collection"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 1);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 1);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 0);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 0);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldDeclareCollectionWithSubcollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>()
                { new RDFResource("ex:concept2"), new RDFResource("ex:concept3") }, new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:collection1")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:collection1"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:collection2")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:collection2"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept3")));

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 2);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 2);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 0);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 0);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldDeclareCollectionWithSubOrderedcollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareOrderedCollection(new RDFResource("ex:collection2"), new List<RDFResource>()
                { new RDFResource("ex:concept2"), new RDFResource("ex:concept3") }, new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.CheckHasCollection(new RDFResource("ex:collection1")));
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:collection1")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:collection1"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(ontology.CheckHasOrderedCollection(new RDFResource("ex:collection2")));
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:collection2")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:collection2"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            Assert.IsTrue(ontology.Data.ABoxGraph[new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].Any());
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept2"), null].Any());
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3"), null].Any());

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 1);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 1);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 1);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 1);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseNullCollection()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareCollection(null,
                    new List<RDFResource>() { new RDFResource("ex:concept") }, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareCollection(new RDFResource("ex:collection"), 
                    new List<RDFResource>() { new RDFResource("ex:concept") }, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseNullList()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").DeclareCollection(new RDFResource("ex:collection"), 
                    null, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseEmptyList()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").DeclareCollection(new RDFResource("ex:collection"), 
                    new List<RDFResource>(), new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").DeclareCollection(new RDFResource("ex:collection"), 
                    new List<RDFResource>() { new RDFResource("ex:concept") }, null));
        
        [TestMethod]
        public void ShouldDeclareOrderedCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:orderedCollection")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.ABoxGraph[new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].Any());
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept2"), null].Any());

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 0);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 0);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 1);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 1);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldDeclareOrderedCollectionWithSubOrderedCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection1"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:orderedCollection2") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection2"), new List<RDFResource>()
                { new RDFResource("ex:concept2"), new RDFResource("ex:concept3") }, new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:orderedCollection1")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:orderedCollection1"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.ABoxGraph[new RDFResource("ex:orderedCollection1"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].Any());
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:orderedCollection2")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:orderedCollection2"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept2"), null].Any());
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3"), null].Any());

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 0);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 0);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 2);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 2);

            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseNullOrderedCollection()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareOrderedCollection(null,
                    new List<RDFResource>() { new RDFResource("ex:concept") }, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), 
                    new List<RDFResource>() { new RDFResource("ex:concept") }, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseNullList()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), 
                    null, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseEmptyList()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), 
                    new List<RDFResource>(), new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), 
                    new List<RDFResource>() { new RDFResource("ex:concept") }, null));
        
        [TestMethod]
        public void ShouldDeclareLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasIndividual(new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckIsIndividualOf(ontology.Model, new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LABEL));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));

            //Test counters and enumerators
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            int i0 = 0;
            IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemesEnumerator.MoveNext()) i0++;
            Assert.IsTrue(i0 == 1);

            Assert.IsTrue(ontology.GetConceptsCount() == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            Assert.IsTrue(ontology.GetCollectionsCount() == 0);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 0);

            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 0);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 0);

            Assert.IsTrue(ontology.GetLabelsCount() == 1);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 1);
        }
        
        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLabelBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareLabel(null, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLabelBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareLabel(new RDFResource("ex:label"), null));
        
        //ANNOTATIONS

        [TestMethod]
        public void ShouldLiteralAnnotateConceptScheme()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConceptScheme(new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:ConceptScheme!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:ConceptScheme!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptSchemeBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:ConceptScheme!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptSchemeBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:ConceptScheme!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptSchemeBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), null, new RDFPlainLiteral("This is a skos:ConceptScheme!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptSchemeBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), new RDFResource(), new RDFPlainLiteral("This is a skos:ConceptScheme!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptSchemeBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateConceptScheme()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConceptScheme(new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptSchemeBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptSchemeBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(null, RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptSchemeBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptSchemeBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptSchemeBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));
        
        [TestMethod]
        public void ShouldLiteralAnnotateConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Concept!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Concept!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Concept!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Concept!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(new RDFResource("ex:concept"), null, new RDFPlainLiteral("This is a skos:Concept!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(new RDFResource("ex:concept"), new RDFResource(), new RDFPlainLiteral("This is a skos:Concept!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(null, RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(new RDFResource("ex:concept"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(new RDFResource("ex:concept"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));
        
        [TestMethod]
        public void ShouldLiteralAnnotateCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareCollection(new RDFResource("ex:collection"), 
                new List<RDFResource>() { new RDFResource("ex:concept1") }, new RDFResource("ex:conceptScheme"));
            ontology.AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Collection!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Collection!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Collection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseNullCollection()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Collection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(new RDFResource("ex:collection"), null, new RDFPlainLiteral("This is a skos:Collection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(new RDFResource("ex:collection"), new RDFResource(), new RDFPlainLiteral("This is a skos:Collection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareCollection(new RDFResource("ex:collection"), 
                new List<RDFResource>() { new RDFResource("ex:concept1") }, new RDFResource("ex:conceptScheme"));
            ontology.AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseNullCollection()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(null, RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(new RDFResource("ex:collection"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(new RDFResource("ex:collection"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));
        
        [TestMethod]
        public void ShouldLiteralAnnotateOrderedCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), 
                new List<RDFResource>() { new RDFResource("ex:concept1") }, new RDFResource("ex:conceptScheme"));
            ontology.AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:OrderedCollection!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:OrderedCollection!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:OrderedCollection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseNullOrderedCollection()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:OrderedCollection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), null, new RDFPlainLiteral("This is a skos:OrderedCollection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), new RDFResource(), new RDFPlainLiteral("This is a skos:OrderedCollection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateOrderedCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), 
                new List<RDFResource>() { new RDFResource("ex:concept1") }, new RDFResource("ex:conceptScheme"));
            ontology.AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseNullOrderedCollection()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(null, RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));
        
        [TestMethod]
        public void ShouldLiteralAnnotateLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skosxl:Label!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skosxl:Label!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skosxl:Label!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skosxl:Label!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(new RDFResource("ex:label"), null, new RDFPlainLiteral("This is a skosxl:Label!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(new RDFResource("ex:label"), new RDFResource(), new RDFPlainLiteral("This is a skosxl:Label!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:label"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(null, RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(new RDFResource("ex:label"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(new RDFResource("ex:label"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));
        
        [DataTestMethod]
        [DataRow(SKOSEnums.SKOSDocumentationTypes.ChangeNote)]
        [DataRow(SKOSEnums.SKOSDocumentationTypes.Definition)]
        [DataRow(SKOSEnums.SKOSDocumentationTypes.EditorialNote)]
        [DataRow(SKOSEnums.SKOSDocumentationTypes.Example)]
        [DataRow(SKOSEnums.SKOSDocumentationTypes.HistoryNote)]
        [DataRow(SKOSEnums.SKOSDocumentationTypes.Note)]
        [DataRow(SKOSEnums.SKOSDocumentationTypes.ScopeNote)]
        public void ShouldDocumentConcept(SKOSEnums.SKOSDocumentationTypes skosDocumentationType)
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DocumentConcept(new RDFResource("ex:concept"), skosDocumentationType, new RDFPlainLiteral("This is a skos documentation!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            switch (skosDocumentationType)
            {
                case SKOSEnums.SKOSDocumentationTypes.ChangeNote:
                    Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.CHANGE_NOTE, new RDFPlainLiteral("This is a skos documentation!")));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.Definition:
                    Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.DEFINITION, new RDFPlainLiteral("This is a skos documentation!")));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.EditorialNote:
                    Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.EDITORIAL_NOTE, new RDFPlainLiteral("This is a skos documentation!")));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.Example:
                    Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.EXAMPLE, new RDFPlainLiteral("This is a skos documentation!")));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.HistoryNote:
                    Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HISTORY_NOTE, new RDFPlainLiteral("This is a skos documentation!")));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.Note:
                    Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.NOTE, new RDFPlainLiteral("This is a skos documentation!")));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.ScopeNote:
                    Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SCOPE_NOTE, new RDFPlainLiteral("This is a skos documentation!")));
                    break;
            }
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DocumentConcept(new RDFResource("ex:concept"), SKOSEnums.SKOSDocumentationTypes.ChangeNote, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DocumentConcept(null, SKOSEnums.SKOSDocumentationTypes.ChangeNote, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptNoteBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DocumentConcept(new RDFResource("ex:concept"), SKOSEnums.SKOSDocumentationTypes.ChangeNote, null));

        [TestMethod]
        public void ShouldAnnotateConceptWithPreferredLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithPreferredLabelBecauseAlreadyExistingUnlanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label2")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label2'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithPreferredLabelBecauseAlreadyExistingLanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label2", "en-US")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label2@EN-US'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithPreferredLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithPreferredLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithPreferredLabelBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                .AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithPreferredLabelBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                .AnnotateConceptPreferredLabel(null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithPreferredLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                .AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldAnnotateConceptWithAlternativeLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithAlternativeLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithAlternativeLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithAlternativeLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithAlternativeLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithAlternativeLabelBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                .AnnotateConceptAlternativeLabel(null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithAlternativeLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                .AnnotateConceptAlternativeLabel(null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithAlternativeLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                .AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldAnnotateConceptWithHiddenLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithHiddenLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithHiddenLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithHiddenLabelBecauseClashWithAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnAnnotatingConceptWithHiddenLabelBecauseClashWithExtendedAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithHiddenLabelBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                .AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithHiddenLabelBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                .AnnotateConceptHiddenLabel(null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingConceptWithHiddenLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                .AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), null));
        
        // RELATIONS

        [TestMethod]
        public void ShouldDeclareTopConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareTopConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:conceptScheme"), RDFVocabulary.SKOS.HAS_TOP_CONCEPT, new RDFResource("ex:concept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.TOP_CONCEPT_OF, new RDFResource("ex:conceptScheme")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringTopConceptBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareTopConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringTopConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareTopConcept(null, new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringTopConceptBecauseNullConceptScheme()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareTopConcept(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDeclareSemanticRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareSemanticRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.SEMANTIC_RELATION, new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSemanticRelatedConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareSemanticRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSemanticRelatedConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareSemanticRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSemanticRelatedConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareSemanticRelatedConcepts(new RDFResource("ex:concept1"), null));

        [TestMethod]
        public void ShouldDeclareMappingRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareMappingRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.MAPPING_RELATION, new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMappingRelatedConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareMappingRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMappingRelatedConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareMappingRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMappingRelatedConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareMappingRelatedConcepts(new RDFResource("ex:concept1"), null));

        [TestMethod]
        public void ShouldDeclareRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology).DeclareRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology").DeclareRelatedConcepts(new RDFResource("ex:concept1"), null));

        [TestMethod]
        public void ShouldDeclareBroaderConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:motherConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:childConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Broader relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Broader relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Broader relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroaderConcepts(null, new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroaderConcepts(new RDFResource("ex:childConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldDeclareBroaderTransitiveConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER_TRANSITIVE, new RDFResource("ex:motherConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:childConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderTransitiveConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroaderTransitive relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderTransitiveConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroaderTransitive relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderTransitiveConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroaderTransitive relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderTransitiveConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderTransitiveConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroaderTransitiveConcepts(null, new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderTransitiveConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderTransitiveConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldDeclareNarrowerConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:childConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:motherConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Narrower relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Narrower relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Narrower relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowerConcepts(null, new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldDeclareNarrowerTransitiveConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:childConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER_TRANSITIVE, new RDFResource("ex:motherConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerTransitiveConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.BROADER_TRANSITIVE, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowerTransitive relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerTransitiveConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowerTransitive relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerTransitiveConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowerTransitive relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerTransitiveConceptsBecauseNullOntology()
           => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                       .DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerTransitiveConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowerTransitiveConcepts(null, new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerTransitiveConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerTransitiveConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldDeclareCloseMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:rightConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:leftConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringCloseMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("CloseMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringCloseMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("CloseMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCloseMatchConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCloseMatchConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareCloseMatchConcepts(null, new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCloseMatchConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCloseMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:leftConcept")));

        [TestMethod]
        public void ShouldDeclareExactMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:rightConcept"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:leftConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringExactMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("ExactMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringExactMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("ExactMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringExactMatchConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringExactMatchConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareExactMatchConcepts(null, new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringExactMatchConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringExactMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:leftConcept")));

        [TestMethod]
        public void ShouldDeclareBroadMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROW_MATCH, new RDFResource("ex:childConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroadMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroadMatch relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroadMatchConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroadMatch relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroadMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroadMatch relation between concept 'ex:childConcept' and concept 'ex:motherConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroadMatchConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroadMatchConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroadMatchConcepts(null, new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroadMatchConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroadMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldDeclareNarrowMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROW_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:motherConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowMatch relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowMatchConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowMatch relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:childConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:motherConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowMatch relation between concept 'ex:motherConcept' and concept 'ex:childConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowMatchConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowMatchConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowMatchConcepts(null, new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowMatchConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldDeclareRelatedMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:rightConcept"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:leftConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringRelatedMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("RelatedMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringRelatedMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:leftConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rightConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("RelatedMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedMatchConceptsBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedMatchConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareRelatedMatchConcepts(null, new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedMatchConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:leftConcept")));

        [TestMethod]
        public void ShouldDeclareConceptNotation()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept"), new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.NOTATION, new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptNotationBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                        .DeclareConceptNotation(new RDFResource("ex:concept"), new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptNotationBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareConceptNotation(null, new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptNotationBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                        .DeclareConceptNotation(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDeclareConceptPreferredLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label", "en-US"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseAlreadyExistingUnlanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label2")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label2'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseAlreadyExistingLanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label", "en-US"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label2", "en-US")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label2@EN-US'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullOntology()
         => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                     .DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullConcept()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptPreferredLabel(null, new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullLabel()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptPreferredLabel(new RDFResource("ex:concept"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullValue()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), null));

        [TestMethod]
        public void ShouldDeclareConceptAlternativeLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label", "en-US"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullOntology()
         => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                     .DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullConcept()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptAlternativeLabel(null, new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullLabel()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullValue()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), null));

        [TestMethod]
        public void ShouldDeclareConceptHiddenLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label", "en-US"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptPreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithExtendedAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            ontology.DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label'") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullOntology()
         => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                     .DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullConcept()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptHiddenLabel(null, new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullLabel()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptHiddenLabel(new RDFResource("ex:concept"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullValue()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareConceptHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), null));

        [TestMethod]
        public void ShouldDeclareRelatedLabels()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareLabel(new RDFResource("ex:label1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedLabels(new RDFResource("ex:label1"), new RDFResource("ex:label2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(ontology.URI.Equals(ontology.URI));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:label1"), RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, new RDFResource("ex:label2")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:label2"), RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, new RDFResource("ex:label1")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedLabelsBecauseNullOntology()
         => Assert.ThrowsException<OWLException>(() => (null as OWLOntology)
                     .DeclareRelatedLabels(new RDFResource("ex:leftLabel"), new RDFResource("ex:rightLabel")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedLabelsBecauseNullLeftLabel()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareRelatedLabels(null, new RDFResource("ex:rightLabel")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedLabelsBecauseNullRightLabel()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareRelatedLabels(new RDFResource("ex:leftLabel"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedLabelsBecauseSelfLabel()
         => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ontology")
                     .DeclareRelatedLabels(new RDFResource("ex:leftLabel"), new RDFResource("ex:leftLabel")));
        #endregion

        #region Tests (Analyzer)
        [TestMethod]
        public void ShouldCheckHasConceptScheme()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareConceptScheme(new RDFResource("ex:conceptScheme1"));
            ontology.DeclareConceptScheme(new RDFResource("ex:conceptScheme2"));

            Assert.IsTrue(ontology.CheckHasConceptScheme(new RDFResource("ex:conceptScheme1")));
            Assert.IsTrue(ontology.CheckHasConceptScheme(new RDFResource("ex:conceptScheme2")));
        }

        [TestMethod]
        public void ShouldCheckHasNotConceptScheme()
        {
            OWLOntology ontologyNULL = null;
            OWLOntology ontologyEMPTY = new OWLOntology("ex:ontologyEmpty");
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareConceptScheme(new RDFResource("ex:conceptScheme1"));
            ontology.DeclareConceptScheme(new RDFResource("ex:conceptScheme2"));

            Assert.IsFalse(ontology.CheckHasConceptScheme(new RDFResource("ex:conceptScheme3")));
            Assert.IsFalse(ontology.CheckHasConceptScheme(null));
            Assert.IsFalse(ontologyNULL.CheckHasConceptScheme(new RDFResource("ex:conceptScheme1")));
            Assert.IsFalse(ontologyEMPTY.CheckHasConceptScheme(new RDFResource("ex:conceptScheme1")));
        }

        [TestMethod]
        public void ShouldCheckHasConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));

            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldCheckHasNotConcept()
        {
            OWLOntology ontologyNULL = null;
            OWLOntology ontologyEMPTY = new OWLOntology("ex:ontologyEmpty");
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));

            Assert.IsFalse(ontology.CheckHasConcept(new RDFResource("ex:concept3")));
            Assert.IsFalse(ontology.CheckHasConcept(null));
            Assert.IsFalse(ontologyNULL.CheckHasConcept(new RDFResource("ex:concept1")));
            Assert.IsFalse(ontologyEMPTY.CheckHasConcept(new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldCheckHasCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));

            Assert.IsTrue(ontology.CheckHasCollection(new RDFResource("ex:collection")));
        }

        [TestMethod]
        public void ShouldCheckHasNotCollection()
        {
            OWLOntology ontologyNULL = null;
            OWLOntology ontologyEMPTY = new OWLOntology("ex:ontologyEmpty");
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));

            Assert.IsFalse(ontology.CheckHasCollection(new RDFResource("ex:collection2")));
            Assert.IsFalse(ontology.CheckHasCollection(null));
            Assert.IsFalse(ontologyNULL.CheckHasCollection(new RDFResource("ex:collection")));
            Assert.IsFalse(ontologyEMPTY.CheckHasCollection(new RDFResource("ex:collection")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembers()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));
            List<RDFResource> collectionMembers = ontology.GetCollectionMembers(new RDFResource("ex:collection"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 2);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembersWithSubCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:concept3") }, new RDFResource("ex:conceptScheme"));
            List<RDFResource> collectionMembers = ontology.GetCollectionMembers(new RDFResource("ex:collection1"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 3);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));
            Assert.IsTrue(collectionMembers[2].Equals(new RDFResource("ex:concept3")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembersWithSubOrderedCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:orderedCollection") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:concept3") }, new RDFResource("ex:conceptScheme"));
            List<RDFResource> collectionMembers = ontology.GetCollectionMembers(new RDFResource("ex:collection"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 3);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));
            Assert.IsTrue(collectionMembers[2].Equals(new RDFResource("ex:concept3")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembersWithNestedSubCollections()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:orderedCollection") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept3"), new RDFResource("ex:concept4") }, new RDFResource("ex:conceptScheme"));
            List<RDFResource> collectionMembers = ontology.GetCollectionMembers(new RDFResource("ex:collection1"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 4);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));
            Assert.IsTrue(collectionMembers[2].Equals(new RDFResource("ex:concept3")));
            Assert.IsTrue(collectionMembers[3].Equals(new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldCheckHasOrderedCollection()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));

            Assert.IsTrue(ontology.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection")));
        }

        [TestMethod]
        public void ShouldCheckHasNotOrderedCollection()
        {
            OWLOntology ontologyNULL = null;
            OWLOntology ontologyEMPTY = new OWLOntology("ex:ontologyEmpty");
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));

            Assert.IsFalse(ontology.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection2")));
            Assert.IsFalse(ontology.CheckHasOrderedCollection(null));
            Assert.IsFalse(ontologyNULL.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection")));
            Assert.IsFalse(ontologyEMPTY.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection")));
        }

        [TestMethod]
        public void ShouldGetOrderedCollectionMembers()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));
            List<RDFResource> orderedCollectionMembers = ontology.GetOrderedCollectionMembers(new RDFResource("ex:orderedCollection"));

            Assert.IsNotNull(orderedCollectionMembers);
            Assert.IsTrue(orderedCollectionMembers.Count == 2);
            Assert.IsTrue(orderedCollectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(orderedCollectionMembers[1].Equals(new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldNotInfiniteLoopInGettingRecursiveCollectionMembers()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"));
            ontology.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:collection1") }, new RDFResource("ex:conceptScheme"));
            List<RDFResource> collectionMembers = ontology.GetCollectionMembers(new RDFResource("ex:collection1"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 2);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));

            List<RDFResource> collectionMembers2 = ontology.GetCollectionMembers(new RDFResource("ex:collection2"));

            Assert.IsNotNull(collectionMembers2);
            Assert.IsTrue(collectionMembers2.Count == 2);
            Assert.IsTrue(collectionMembers2[0].Equals(new RDFResource("ex:concept2")));
            Assert.IsTrue(collectionMembers2[1].Equals(new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldNotInfiniteLoopInGettingSelfRecursiveCollectionMembers()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection1") }, new RDFResource("ex:conceptScheme"));
            List<RDFResource> collectionMembers = ontology.GetCollectionMembers(new RDFResource("ex:collection1"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 1);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldCheckHasLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareLabel(new RDFResource("ex:label1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label2"), new RDFResource("ex:conceptScheme"));

            Assert.IsTrue(ontology.CheckHasLabel(new RDFResource("ex:label1")));
            Assert.IsTrue(ontology.CheckHasLabel(new RDFResource("ex:label2")));
        }

        [TestMethod]
        public void ShouldCheckHasNotLabel()
        {
            OWLOntology ontologyNULL = null;
            OWLOntology ontologyEMPTY = new OWLOntology("ex:ontologyEmpty");
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.DeclareLabel(new RDFResource("ex:label1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label2"), new RDFResource("ex:conceptScheme"));

            Assert.IsFalse(ontology.CheckHasLabel(new RDFResource("ex:label3")));
            Assert.IsFalse(ontology.CheckHasLabel(null));
            Assert.IsFalse(ontologyNULL.CheckHasLabel(new RDFResource("ex:label1")));
            Assert.IsFalse(ontologyEMPTY.CheckHasLabel(new RDFResource("ex:label1")));
        }

        [TestMethod]
        public void ShouldCheckHasTopConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareTopConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));

            Assert.IsTrue(ontology.CheckHasTopConcept(new RDFResource("ex:conceptScheme"), new RDFResource("ex:concept")));
        }

        [TestMethod]
        public void ShouldCheckHasNotTopConcept()
        {
            OWLOntology ontologyNULL = null;
            OWLOntology ontologyEMPTY = new OWLOntology("ex:ontologyEmpty");

            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme2"));
            ontology.DeclareTopConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareTopConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme2"));

            Assert.IsFalse(ontology.CheckHasTopConcept(new RDFResource("ex:conceptScheme"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasTopConcept(null, new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasTopConcept(new RDFResource("ex:conceptScheme"), null));
            Assert.IsFalse(ontologyNULL.CheckHasTopConcept(new RDFResource("ex:conceptScheme"), new RDFResource("ex:concept1")));
            Assert.IsFalse(ontologyEMPTY.CheckHasTopConcept(new RDFResource("ex:conceptScheme"), new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldGetBroaderConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetBroaderConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetBroaderConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(ontology.GetBroaderConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasBroaderConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept4"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotBroaderConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            ontology.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsFalse(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetNarrowerConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetNarrowerConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetNarrowerConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(ontology.GetNarrowerConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNarrowerConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept4"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotNarrowerConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowerConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsFalse(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            Assert.IsTrue(ontology.GetRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetRelatedConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasRelatedConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));

            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept2"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotRelatedConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareRelatedConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));

            Assert.IsFalse(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsFalse(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldGetBroadMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetBroadMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetBroadMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(ontology.GetBroadMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
        }

        [TestMethod]
        public void ShouldCheckHasBroadMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldCheckHasNotBroadMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareBroadMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetNarrowMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetNarrowMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetNarrowMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(ontology.GetNarrowMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
        }

        [TestMethod]
        public void ShouldCheckHasNarrowMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldCheckHasNotNarrowMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetRelatedMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
            Assert.IsTrue(ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasRelatedMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept3"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotRelatedMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareRelatedMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetCloseMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetCloseMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetCloseMatchConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(ontology.GetCloseMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(ontology.GetCloseMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(ontology.GetCloseMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
            Assert.IsTrue(ontology.GetCloseMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasCloseMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept3"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotCloseMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetExactMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
            Assert.IsTrue(ontology.GetExactMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasExactMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept3"))); //Inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //Inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept1"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotExactMatchConcept()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetMappingRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareMappingRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"));

            Assert.IsTrue(ontology.GetMappingRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetMappingRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
            Assert.IsTrue(ontology.GetMappingRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
        }

        [TestMethod]
        public void ShouldGetSemanticRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareSemanticRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"));
            ontology.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"));

            Assert.IsTrue(ontology.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(ontology.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
            Assert.IsTrue(ontology.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
            Assert.IsTrue(ontology.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept5")))); //Inference
        }

        [TestMethod]
        public void ShouldGetConceptNotations()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept"), new RDFPlainLiteral("notation"));

            Assert.IsTrue(ontology.GetConceptNotations(new RDFResource("ex:concept")).Any(c => c.Equals(new RDFPlainLiteral("notation"))));
        }
        #endregion

        #region Tests (Import/Export)
        [TestMethod]
        public void ShouldExportToGraph()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept scheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConcept(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareCollection(new RDFResource("ex:collection1"),
                new List<RDFResource>() { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"));
            ontology.AnnotateCollection(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection"));
            ontology.DeclareCollection(new RDFResource("ex:collection2"),
                new List<RDFResource>() { new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));
            RDFGraph graph = ontology.ToRDFGraph();

            Assert.IsNotNull(graph);

            //Test persistence of SKOS knowledge (sampling)
            Assert.IsTrue(graph[RDFVocabulary.SKOS.CONCEPT_SCHEME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.CONCEPT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.MEMBER, RDFVocabulary.RDFS.DOMAIN, RDFVocabulary.SKOS.COLLECTION, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.MEMBER, RDFVocabulary.RDFS.RANGE, new RDFResource("bnode:ConceptCollection"), null].Any());

            //Test persistence of user sentences
            Assert.IsTrue(graph[new RDFResource("ex:conceptScheme"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:conceptScheme"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT_SCHEME, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test concept scheme")].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test concept")].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept2"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test collection")].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2"), null].Any());
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ontology");
            ontology.InitializeSKOS();
            ontology.AnnotateConceptScheme(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept scheme"));
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            ontology.AnnotateConcept(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            ontology.DeclareCollection(new RDFResource("ex:collection1"),
                new List<RDFResource>() { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"));
            ontology.AnnotateCollection(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection"));
            ontology.DeclareCollection(new RDFResource("ex:collection2"),
                new List<RDFResource>() { new RDFResource("ex:concept2") }, new RDFResource("ex:conceptScheme"));
            RDFGraph graph = await ontology.ToRDFGraphAsync();

            Assert.IsNotNull(graph);

            //Test persistence of SKOS knowledge (sampling)
            Assert.IsTrue(graph[RDFVocabulary.SKOS.CONCEPT_SCHEME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.CONCEPT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.MEMBER, RDFVocabulary.RDFS.DOMAIN, RDFVocabulary.SKOS.COLLECTION, null].Any());
            Assert.IsTrue(graph[RDFVocabulary.SKOS.MEMBER, RDFVocabulary.RDFS.RANGE, new RDFResource("bnode:ConceptCollection"), null].Any());

            //Test persistence of user sentences
            Assert.IsTrue(graph[new RDFResource("ex:conceptScheme"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:conceptScheme"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT_SCHEME, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test concept scheme")].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test concept")].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept1"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept2"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:concept2"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test collection")].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2"), null].Any());
        }

        [TestMethod]
        public void ShouldCreateFromGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT_SCHEME));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept scheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:orderedCollection")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.MEMBER_LIST, new RDFResource("bnode:ordcollRepresentative")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:ordcollRepresentative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:ordcollRepresentative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:ordcollRepresentative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableSKOSSupport=true });

            //Test persistence of SKOS knowledge
            Assert.IsNotNull(ontology);
            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 6);

            //Test persistence of user sentences
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            Assert.IsTrue(ontology.GetConceptsCount() == 2);
            Assert.IsTrue(ontology.GetCollectionsCount() == 2);
            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 1);
            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:orderedCollection")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.ABoxGraph[new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].TriplesCount > 0);
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3"), null].TriplesCount > 0);

            //Test reconstruction of nested collections
            Assert.IsTrue(ontology.GetCollectionMembers(new RDFResource("ex:collection1")).Count == 3);
            Assert.IsTrue(ontology.GetCollectionMembers(new RDFResource("ex:collection2")).Count == 2);
            Assert.IsTrue(ontology.GetOrderedCollectionMembers(new RDFResource("ex:orderedCollection")).Count == 1);
        }

        [TestMethod]
        public async Task ShouldCreateFromGraphAsync()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT_SCHEME));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept scheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:concept1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:orderedCollection")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.MEMBER_LIST, new RDFResource("bnode:ordcollRepresentative")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:ordcollRepresentative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:ordcollRepresentative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:ordcollRepresentative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            OWLOntology ontology = await OWLOntology.FromRDFGraphAsync(graph, new OWLOntologyLoaderOptions() { EnableSKOSSupport = true });

            //Test persistence of SKOS knowledge
            Assert.IsNotNull(ontology);
            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(ontology.Data.IndividualsCount == 6);

            //Test persistence of user sentences
            Assert.IsTrue(ontology.GetConceptSchemesCount() == 1);
            Assert.IsTrue(ontology.GetConceptsCount() == 2);
            Assert.IsTrue(ontology.GetCollectionsCount() == 2);
            Assert.IsTrue(ontology.GetOrderedCollectionsCount() == 1);
            Assert.IsTrue(ontology.GetLabelsCount() == 0);
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            Assert.IsTrue(ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:orderedCollection")));
            Assert.IsTrue(ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(ontology.Data.ABoxGraph[new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].TriplesCount > 0);
            Assert.IsTrue(ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3"), null].TriplesCount > 0);

            //Test reconstruction of nested collections
            Assert.IsTrue(ontology.GetCollectionMembers(new RDFResource("ex:collection1")).Count == 3);
            Assert.IsTrue(ontology.GetCollectionMembers(new RDFResource("ex:collection2")).Count == 2);
            Assert.IsTrue(ontology.GetOrderedCollectionMembers(new RDFResource("ex:orderedCollection")).Count == 1);
        }
        #endregion
    }
}