/*
   Copyright 2012-2023 Marco De Salvo

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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSConceptSchemeTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateConceptScheme()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");

            Assert.IsNotNull(conceptScheme);
            Assert.IsNotNull(conceptScheme.Ontology);

            //Test initialization of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);

            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.ConceptsCount == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            int i2 = 0;
            foreach (RDFResource skosConcept in conceptScheme) i2++;
            Assert.IsTrue(i2 == 0);

            Assert.IsTrue(conceptScheme.CollectionsCount == 0);
            int j = 0;
            IEnumerator<RDFResource> collectionsEnumerator = conceptScheme.CollectionsEnumerator;
            while (collectionsEnumerator.MoveNext()) j++;
            Assert.IsTrue(j == 0);
            
            Assert.IsTrue(conceptScheme.OrderedCollectionsCount == 0);
            int k = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = conceptScheme.OrderedCollectionsEnumerator;
            while (orderedCollectionsEnumerator.MoveNext()) k++;
            Assert.IsTrue(k == 0);
            
            Assert.IsTrue(conceptScheme.LabelsCount == 0);
            int l = 0;
            IEnumerator<RDFResource> labelsEnumerator = conceptScheme.LabelsEnumerator;
            while (labelsEnumerator.MoveNext()) l++;
            Assert.IsTrue(l == 0);
        }

        [TestMethod]
        public void ShouldDeclareConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareConcept(new RDFResource("ex:concept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:concept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:concept"), RDFVocabulary.SKOS.CONCEPT));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));

            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.ConceptsCount == 1);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 1);

            int i2 = 0;
            foreach (RDFResource skosConcept in conceptScheme) i2++;
            Assert.IsTrue(i2 == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringConceptBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareConcept(null));

        [TestMethod]
        public void ShouldDeclareCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(conceptScheme.CheckHasCollection(new RDFResource("ex:collection")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:collection")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:collection"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            
            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.ConceptsCount == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            int i2 = 0;
            foreach (RDFResource skosConcept in conceptScheme) i2++;
            Assert.IsTrue(i2 == 0);

            Assert.IsTrue(conceptScheme.CollectionsCount == 1);
            int j1 = 0;
            IEnumerator<RDFResource> collectionsEnumerator = conceptScheme.CollectionsEnumerator;
            while (collectionsEnumerator.MoveNext()) j1++;
            Assert.IsTrue(j1 == 1);
        }

        [TestMethod]
        public void ShouldDeclareCollectionWithSubcollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") });
            conceptScheme.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>()
                { new RDFResource("ex:concept2"), new RDFResource("ex:concept3") });

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:collection1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:collection1"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:collection2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:collection2"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept3")));

            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.ConceptsCount == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            int i2 = 0;
            foreach (RDFResource skosConcept in conceptScheme) i2++;
            Assert.IsTrue(i2 == 0);

            Assert.IsTrue(conceptScheme.CollectionsCount == 2);
            int j1 = 0;
            IEnumerator<RDFResource> collectionsEnumerator = conceptScheme.CollectionsEnumerator;
            while (collectionsEnumerator.MoveNext()) j1++;
            Assert.IsTrue(j1 == 2);
        }

        [TestMethod]
        public void ShouldDeclareCollectionWithSubOrderedcollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") });
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:collection2"), new List<RDFResource>()
                { new RDFResource("ex:concept2"), new RDFResource("ex:concept3") });

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(conceptScheme.CheckHasCollection(new RDFResource("ex:collection1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:collection1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:collection1"), RDFVocabulary.SKOS.COLLECTION));
            Assert.IsTrue(conceptScheme.CheckHasOrderedCollection(new RDFResource("ex:collection2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:collection2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:collection2"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].Any());
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept2"), null].Any());
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3"), null].Any());

            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.ConceptsCount == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            int i2 = 0;
            foreach (RDFResource skosConcept in conceptScheme) i2++;
            Assert.IsTrue(i2 == 0);

            Assert.IsTrue(conceptScheme.CollectionsCount == 1);
            int j1 = 0;
            IEnumerator<RDFResource> collectionsEnumerator = conceptScheme.CollectionsEnumerator;
            while (collectionsEnumerator.MoveNext()) j1++;
            Assert.IsTrue(j1 == 1);

            Assert.IsTrue(conceptScheme.OrderedCollectionsCount == 1);
            int j2 = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = conceptScheme.OrderedCollectionsEnumerator;
            while (orderedCollectionsEnumerator.MoveNext()) j2++;
            Assert.IsTrue(j2 == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareCollection(null, new List<RDFResource>() { new RDFResource("ex:concept") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseNullList()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareCollection(new RDFResource("ex:collection"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionBecauseEmptyList()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldDeclareOrderedCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:orderedCollection")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].Any());
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept2"), null].Any());

            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.ConceptsCount == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            int i2 = 0;
            foreach (RDFResource skosConcept in conceptScheme) i2++;
            Assert.IsTrue(i2 == 0);

            Assert.IsTrue(conceptScheme.OrderedCollectionsCount == 1);
            int j1 = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = conceptScheme.OrderedCollectionsEnumerator;
            while (orderedCollectionsEnumerator.MoveNext()) j1++;
            Assert.IsTrue(j1 == 1);
        }

        [TestMethod]
        public void ShouldDeclareOrderedCollectionWithSubOrderedCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection1"), new List<RDFResource>()
                { new RDFResource("ex:concept1"), new RDFResource("ex:orderedCollection2") });
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection2"), new List<RDFResource>()
                { new RDFResource("ex:concept2"), new RDFResource("ex:concept3") });

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 3);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:orderedCollection1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:orderedCollection1"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[new RDFResource("ex:orderedCollection1"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].Any());
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:orderedCollection2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:orderedCollection2"), RDFVocabulary.SKOS.ORDERED_COLLECTION));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept2"), null].Any());
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3"), null].Any());

            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.ConceptsCount == 0);
            int i1 = 0;
            IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
            while (conceptsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 0);

            int i2 = 0;
            foreach (RDFResource skosConcept in conceptScheme) i2++;
            Assert.IsTrue(i2 == 0);

            Assert.IsTrue(conceptScheme.OrderedCollectionsCount == 2);
            int j1 = 0;
            IEnumerator<RDFResource> orderedCollectionsEnumerator = conceptScheme.OrderedCollectionsEnumerator;
            while (orderedCollectionsEnumerator.MoveNext()) j1++;
            Assert.IsTrue(j1 == 2);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareOrderedCollection(null, new List<RDFResource>() { new RDFResource("ex:concept") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseNullList()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringOrderedCollectionBecauseEmptyList()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>()));

        //ANNOTATIONS

        [TestMethod]
        public void ShouldLiteralAnnotate()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:conceptScheme!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:conceptScheme!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").Annotate(null, new RDFPlainLiteral("This is a skos:conceptScheme!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").Annotate(new RDFResource(), new RDFPlainLiteral("This is a skos:conceptScheme!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").Annotate(RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotate()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.Annotate(RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:conceptScheme"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").Annotate(null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").Annotate(new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").Annotate(RDFVocabulary.RDFS.COMMENT, null as RDFResource));

        [TestMethod]
        public void ShouldLiteralAnnotateConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:concept!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:concept!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:concept!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(new RDFResource("ex:concept"), null, new RDFPlainLiteral("This is a skos:concept!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(new RDFResource("ex:concept"), new RDFResource(), new RDFPlainLiteral("This is a skos:concept!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingConceptBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(null, RDFVocabulary.RDFS.COMMENT, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(new RDFResource("ex:concept"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(new RDFResource("ex:concept"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingConceptBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateConcept(new RDFResource("ex:concept"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));

        [TestMethod]
        public void ShouldLiteralAnnotateCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Collection!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Collection!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseNullCollection()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Collection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(new RDFResource("ex:collection"), null, new RDFPlainLiteral("This is a skos:Collection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(new RDFResource("ex:collection"), new RDFResource(), new RDFPlainLiteral("This is a skos:Collection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseNullCollection()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(null, RDFVocabulary.RDFS.COMMENT, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(new RDFResource("ex:collection"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(new RDFResource("ex:collection"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));

        [TestMethod]
        public void ShouldLiteralAnnotateOrderedCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:OrderedCollection!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:OrderedCollection!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseNullOrderedCollection()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:OrderedCollection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), null, new RDFPlainLiteral("This is a skos:OrderedCollection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), new RDFResource(), new RDFPlainLiteral("This is a skos:OrderedCollection!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOrderedCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateOrderedCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseNullOrderedCollection()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(null, RDFVocabulary.RDFS.COMMENT, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOrderedCollectionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateOrderedCollection(new RDFResource("ex:orderedCollection"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));

        [TestMethod]
        public void ShouldDocumentConceptNote()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DocumentConceptWithNote(new RDFResource("ex:concept"), new RDFPlainLiteral("This is a note!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.NOTE, new RDFPlainLiteral("This is a note!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptNoteBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithNote(null, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptNoteBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithNote(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDocumentConceptChangeNote()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DocumentConceptWithChangeNote(new RDFResource("ex:concept"), new RDFPlainLiteral("This is a note!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.CHANGE_NOTE, new RDFPlainLiteral("This is a note!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptChangeNoteBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithChangeNote(null, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptChangeNoteBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithChangeNote(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDocumentConceptEditorialNote()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DocumentConceptWithEditorialNote(new RDFResource("ex:concept"), new RDFPlainLiteral("This is a note!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.EDITORIAL_NOTE, new RDFPlainLiteral("This is a note!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionODocumentingConceptEditorialNoteBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithEditorialNote(null, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptEditorialNoteBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithEditorialNote(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDocumentConceptHistoryNote()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DocumentConceptWithHistoryNote(new RDFResource("ex:concept"), new RDFPlainLiteral("This is a note!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HISTORY_NOTE, new RDFPlainLiteral("This is a note!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptHistoryNoteBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithHistoryNote(null, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptHistoryNoteBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithHistoryNote(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDocumentConceptScopeNote()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DocumentConceptWithScopeNote(new RDFResource("ex:concept"), new RDFPlainLiteral("This is a note!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SCOPE_NOTE, new RDFPlainLiteral("This is a note!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptScopeNoteBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithScopeNote(null, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptScopeNoteBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithScopeNote(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDocumentConceptExample()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DocumentConceptWithExample(new RDFResource("ex:concept"), new RDFPlainLiteral("This is a note!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.EXAMPLE, new RDFPlainLiteral("This is a note!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptExampleBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithExample(null, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptExampleBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithExample(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDocumentConceptDefinition()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DocumentConceptWithDefinition(new RDFResource("ex:concept"), new RDFPlainLiteral("This is a note!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.DEFINITION, new RDFPlainLiteral("This is a note!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDocumentingConceptDefinitionBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithDefinition(null, new RDFPlainLiteral("This is a note!")));

        [TestMethod]
        public void ShouldThrowExceptionODocumentingConceptDefinitionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DocumentConceptWithDefinition(new RDFResource("ex:concept"), null));

        //RELATIONS

        [TestMethod]
        public void ShouldDeclareTopConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareTopConcept(new RDFResource("ex:concept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:conceptScheme"), RDFVocabulary.SKOS.HAS_TOP_CONCEPT, new RDFResource("ex:concept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.TOP_CONCEPT_OF, new RDFResource("ex:conceptScheme")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringTopConceptBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareTopConcept(null));

        [TestMethod]
        public void ShouldDeclareSemanticRelatedConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareSemanticRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.SEMANTIC_RELATION, new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSemanticRelatedConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareSemanticRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSemanticRelatedConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareSemanticRelatedConcepts(new RDFResource("ex:concept1"), null));

        [TestMethod]
        public void ShouldDeclareMappingRelatedConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareMappingRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.MAPPING_RELATION, new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMappingRelatedConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareMappingRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMappingRelatedConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareMappingRelatedConcepts(new RDFResource("ex:concept1"), null));

        [TestMethod]
        public void ShouldDeclareRelatedConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareRelatedConcepts(null, new RDFResource("ex:concept2")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareRelatedConcepts(new RDFResource("ex:concept1"), null));

        [TestMethod]
        public void ShouldDeclareBroaderConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:motherConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:childConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Broader relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Broader relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Broader relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroaderConcepts(null, new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroaderConcepts(new RDFResource("ex:childConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroaderConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldDeclareBroaderTransitiveConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER_TRANSITIVE, new RDFResource("ex:motherConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:childConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderTransitiveConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroaderTransitive relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderTransitiveConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroaderTransitive relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroaderTransitiveConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroaderTransitive relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderTransitiveConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroaderTransitiveConcepts(null, new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderTransitiveConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroaderTransitiveConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroaderTransitiveConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldDeclareNarrowerConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:childConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:motherConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Narrower relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Narrower relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Narrower relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowerConcepts(null, new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowerConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldDeclareNarrowerTransitiveConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:childConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROADER_TRANSITIVE, new RDFResource("ex:motherConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerTransitiveConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.BROADER_TRANSITIVE, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowerTransitive relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerTransitiveConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowerTransitive relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowerTransitiveConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowerTransitive relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerTransitiveConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowerTransitiveConcepts(null, new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerTransitiveConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowerTransitiveConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowerTransitiveConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldDeclareCloseMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:rightConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:leftConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringCloseMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.BROADER, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("CloseMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringCloseMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("CloseMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCloseMatchConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareCloseMatchConcepts(null, new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCloseMatchConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCloseMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareCloseMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:leftConcept")));

        [TestMethod]
        public void ShouldDeclareExactMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:rightConcept"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:leftConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringExactMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("ExactMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringExactMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("ExactMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringExactMatchConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareExactMatchConcepts(null, new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringExactMatchConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringExactMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:leftConcept")));

        [TestMethod]
        public void ShouldDeclareBroadMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROW_MATCH, new RDFResource("ex:childConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroadMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroadMatch relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroadMatchConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroadMatch relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringBroadMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:motherConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:motherConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("BroadMatch relation between concept 'ex:childConcept' and concept 'ex:motherConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroadMatchConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroadMatchConcepts(null, new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroadMatchConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringBroadMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareBroadMatchConcepts(new RDFResource("ex:childConcept"), new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldDeclareNarrowMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.NARROW_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:childConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:motherConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.BROAD_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowMatch relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowMatchConceptsBecauseAssociativeRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowMatch relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNarrowMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:childConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:motherConcept"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:childConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NarrowMatch relation between concept 'ex:motherConcept' and concept 'ex:childConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowMatchConceptsBecauseNullChildConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowMatchConcepts(null, new RDFResource("ex:childConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowMatchConceptsBecauseNullMotherConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNarrowMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNarrowMatchConcepts(new RDFResource("ex:motherConcept"), new RDFResource("ex:motherConcept")));

        [TestMethod]
        public void ShouldDeclareRelatedMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:rightConcept"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:leftConcept")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringRelatedMatchConceptsBecauseHierarchicallyRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("RelatedMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringRelatedMatchConceptsBecauseMappingRelatedConcepts()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:rightConcept")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:leftConcept"), RDFVocabulary.SKOS.EXACT_MATCH, new RDFResource("ex:rightConcept")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("RelatedMatch relation between concept 'ex:leftConcept' and concept 'ex:rightConcept' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedMatchConceptsBecauseNullLeftConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareRelatedMatchConcepts(null, new RDFResource("ex:rightConcept")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedMatchConceptsBecauseNullRightConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedMatchConceptsBecauseSelfConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareRelatedMatchConcepts(new RDFResource("ex:leftConcept"), new RDFResource("ex:leftConcept")));

        [TestMethod]
        public void ShouldDeclarePreferredLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseAlreadyExistingUnlanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label2")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label2' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseAlreadyExistingLanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label2", "en-US")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label2@EN-US' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclarePreferredLabel(null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclarePreferredLabel(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDeclareAlternativeLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareAlternativeLabel(null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareAlternativeLabel(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDeclareHiddenLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithExtendedAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareHiddenLabel(null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareHiddenLabel(new RDFResource("ex:concept"), null));

        [TestMethod]
        public void ShouldDeclareNotation()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNotation(new RDFResource("ex:concept"), new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.NOTATION, new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNotationBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNotation(null, new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNotationBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareNotation(new RDFResource("ex:concept"), null));

        //EXPORT

        [TestMethod]
        public void ShouldExportToGraph()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept scheme"));
            conceptScheme.DeclareConcept(new RDFResource("ex:concept1"));
            conceptScheme.AnnotateConcept(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept"));
            conceptScheme.DeclareConcept(new RDFResource("ex:concept2"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareCollection(new RDFResource("ex:collection1"), 
                new List<RDFResource>() { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") });
            conceptScheme.AnnotateCollection(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection"));
            conceptScheme.DeclareCollection(new RDFResource("ex:collection2"), 
                new List<RDFResource>() { new RDFResource("ex:concept2") });
            RDFGraph graph = conceptScheme.ToRDFGraph();

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
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept scheme"));
            conceptScheme.DeclareConcept(new RDFResource("ex:concept1"));
            conceptScheme.AnnotateConcept(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept"));
            conceptScheme.DeclareConcept(new RDFResource("ex:concept2"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() { new RDFResource("ex:concept1") });
            conceptScheme.AnnotateCollection(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection"));
            RDFGraph graph = await conceptScheme.ToRDFGraphAsync();

            //Test persistence of SKOS knowledge
            Assert.IsNotNull(graph);
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
            Assert.IsTrue(graph[new RDFResource("ex:collection"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1"), null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test collection")].Any());
        }

        //IMPORT

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
            SKOSConceptScheme conceptScheme = SKOSConceptScheme.FromRDFGraph(graph);

            //Test persistence of SKOS knowledge
            Assert.IsNotNull(conceptScheme);
            Assert.IsNotNull(conceptScheme.Ontology);
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 6);            

            //Test persistence of user sentences
            Assert.IsTrue(conceptScheme.Equals(new RDFResource("ex:conceptScheme")));            
            Assert.IsTrue(conceptScheme.ConceptsCount == 2);
            Assert.IsTrue(conceptScheme.CollectionsCount == 2);
            Assert.IsTrue(conceptScheme.OrderedCollectionsCount == 1);
            Assert.IsTrue(conceptScheme.LabelsCount == 0);
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection1"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:collection2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection2"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:orderedCollection")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[new RDFResource("ex:orderedCollection"), RDFVocabulary.SKOS.MEMBER_LIST, null, null].TriplesCount > 0);
            Assert.IsTrue(conceptScheme.Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:concept3"), null].TriplesCount > 0);

            //Test reconstruction of nested collections
            Assert.IsTrue(conceptScheme.GetCollectionMembers(new RDFResource("ex:collection1")).Count == 3);
            Assert.IsTrue(conceptScheme.GetCollectionMembers(new RDFResource("ex:collection2")).Count == 2);
            Assert.IsTrue(conceptScheme.GetOrderedCollectionMembers(new RDFResource("ex:orderedCollection")).Count == 1);
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
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection")));
            SKOSConceptScheme conceptScheme = await SKOSConceptScheme.FromRDFGraphAsync(graph);

            //Test persistence of SKOS knowledge
            Assert.IsNotNull(conceptScheme);
            Assert.IsNotNull(conceptScheme.Ontology);
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 4);

            //Test persistence of user sentences
            Assert.IsTrue(conceptScheme.Equals(new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.ConceptsCount == 2);
            Assert.IsTrue(conceptScheme.CollectionsCount == 1);
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:collection"), RDFVocabulary.SKOS.MEMBER, new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:collection"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection")));
        }
        #endregion
    }
}