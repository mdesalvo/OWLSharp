/*
   Copyright 2012-2024 Marco De Salvo

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

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSConceptSchemeHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCheckHasConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareConcept(new RDFResource("ex:concept1"));
            conceptScheme.DeclareConcept(new RDFResource("ex:concept2"));

            Assert.IsTrue(conceptScheme.CheckHasConcept(new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.CheckHasConcept(new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldCheckHasNotConcept()
        {
            SKOSConceptScheme conceptSchemeNULL = null;
            SKOSConceptScheme conceptSchemeEMPTY = new SKOSConceptScheme("ex:conceptSchemeEmpty");
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareConcept(new RDFResource("ex:concept1"));
            conceptScheme.DeclareConcept(new RDFResource("ex:concept2"));

            Assert.IsFalse(conceptScheme.CheckHasConcept(new RDFResource("ex:concept3")));
            Assert.IsFalse(conceptScheme.CheckHasConcept(null));            
            Assert.IsFalse(conceptSchemeNULL.CheckHasConcept(new RDFResource("ex:concept1")));
            Assert.IsFalse(conceptSchemeEMPTY.CheckHasConcept(new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldCheckHasCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() { 
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });

            Assert.IsTrue(conceptScheme.CheckHasCollection(new RDFResource("ex:collection")));
        }

        [TestMethod]
        public void ShouldCheckHasNotCollection()
        {
            SKOSConceptScheme conceptSchemeNULL = null;
            SKOSConceptScheme conceptSchemeEMPTY = new SKOSConceptScheme("ex:conceptSchemeEmpty");
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });

            Assert.IsFalse(conceptScheme.CheckHasCollection(new RDFResource("ex:collection2")));
            Assert.IsFalse(conceptScheme.CheckHasCollection(null));
            Assert.IsFalse(conceptSchemeNULL.CheckHasCollection(new RDFResource("ex:collection")));
            Assert.IsFalse(conceptSchemeEMPTY.CheckHasCollection(new RDFResource("ex:collection")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembers()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });
            List<RDFResource> collectionMembers = conceptScheme.GetCollectionMembers(new RDFResource("ex:collection"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 2);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembersWithSubCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection2") });
            conceptScheme.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:concept3") });
            List<RDFResource> collectionMembers = conceptScheme.GetCollectionMembers(new RDFResource("ex:collection1"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 3);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));
            Assert.IsTrue(collectionMembers[2].Equals(new RDFResource("ex:concept3")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembersWithSubOrderedCollection()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:orderedCollection") });
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:concept3") });
            List<RDFResource> collectionMembers = conceptScheme.GetCollectionMembers(new RDFResource("ex:collection"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 3);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));
            Assert.IsTrue(collectionMembers[2].Equals(new RDFResource("ex:concept3")));
        }

        [TestMethod]
        public void ShouldGetCollectionMembersWithNestedSubCollections()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection2") });
            conceptScheme.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:orderedCollection") });
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept3"), new RDFResource("ex:concept4") });
            List<RDFResource> collectionMembers = conceptScheme.GetCollectionMembers(new RDFResource("ex:collection1"));

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
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });

            Assert.IsTrue(conceptScheme.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection")));
        }

        [TestMethod]
        public void ShouldCheckHasNotOrderedCollection()
        {
            SKOSConceptScheme conceptSchemeNULL = null;
            SKOSConceptScheme conceptSchemeEMPTY = new SKOSConceptScheme("ex:conceptSchemeEmpty");
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });

            Assert.IsFalse(conceptScheme.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection2")));
            Assert.IsFalse(conceptScheme.CheckHasOrderedCollection(null));
            Assert.IsFalse(conceptSchemeNULL.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection")));
            Assert.IsFalse(conceptSchemeEMPTY.CheckHasOrderedCollection(new RDFResource("ex:orderedCollection")));
        }

        [TestMethod]
        public void ShouldGetOrderedCollectionMembers()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareOrderedCollection(new RDFResource("ex:orderedCollection"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:concept2") });
            List<RDFResource> orderedCollectionMembers = conceptScheme.GetOrderedCollectionMembers(new RDFResource("ex:orderedCollection"));

            Assert.IsNotNull(orderedCollectionMembers);
            Assert.IsTrue(orderedCollectionMembers.Count == 2);
            Assert.IsTrue(orderedCollectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(orderedCollectionMembers[1].Equals(new RDFResource("ex:concept2")));
        }

        [TestMethod]
        public void ShouldNotInfiniteLoopInGettingRecursiveCollectionMembers()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection2") });
            conceptScheme.DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() {
                new RDFResource("ex:concept2"), new RDFResource("ex:collection1") });
            List<RDFResource> collectionMembers = conceptScheme.GetCollectionMembers(new RDFResource("ex:collection1"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 2);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
            Assert.IsTrue(collectionMembers[1].Equals(new RDFResource("ex:concept2")));

            List<RDFResource> collectionMembers2 = conceptScheme.GetCollectionMembers(new RDFResource("ex:collection2"));

            Assert.IsNotNull(collectionMembers2);
            Assert.IsTrue(collectionMembers2.Count == 2);
            Assert.IsTrue(collectionMembers2[0].Equals(new RDFResource("ex:concept2")));
            Assert.IsTrue(collectionMembers2[1].Equals(new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldNotInfiniteLoopInGettingSelfRecursiveCollectionMembers()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() {
                new RDFResource("ex:concept1"), new RDFResource("ex:collection1") });
            List<RDFResource> collectionMembers = conceptScheme.GetCollectionMembers(new RDFResource("ex:collection1"));

            Assert.IsNotNull(collectionMembers);
            Assert.IsTrue(collectionMembers.Count == 1);
            Assert.IsTrue(collectionMembers[0].Equals(new RDFResource("ex:concept1")));
        }

        //ANALYZER

        [TestMethod]
        public void ShouldCheckHasTopConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareTopConcept(new RDFResource("ex:concept1"));

            Assert.IsTrue(conceptScheme.CheckHasTopConcept(new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldCheckHasNotTopConcept()
        {
            SKOSConceptScheme conceptSchemeNULL = null;
            SKOSConceptScheme conceptSchemeEMPTY = new SKOSConceptScheme("ex:conceptSchemeEmpty");
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareTopConcept(new RDFResource("ex:concept1"));

            Assert.IsFalse(conceptScheme.CheckHasTopConcept(new RDFResource("ex:concept2")));
            Assert.IsFalse(conceptScheme.CheckHasTopConcept(null));
            Assert.IsFalse(conceptSchemeNULL.CheckHasTopConcept(new RDFResource("ex:concept1")));
            Assert.IsFalse(conceptSchemeEMPTY.CheckHasTopConcept(new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldGetBroaderConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetBroaderConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetBroaderConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(conceptScheme.GetBroaderConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasBroaderConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasBroaderConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasBroaderConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept4"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotBroaderConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(conceptScheme.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsFalse(conceptScheme.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetNarrowerConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetNarrowerConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetNarrowerConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(conceptScheme.GetNarrowerConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNarrowerConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasNarrowerConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasNarrowerConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept4"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotNarrowerConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowerConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(conceptScheme.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsFalse(conceptScheme.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetRelatedConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));

            Assert.IsTrue(conceptScheme.GetRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetRelatedConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasRelatedConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));

            Assert.IsTrue(conceptScheme.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasRelatedConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1")));
            Assert.IsTrue(conceptScheme.CheckHasRelatedConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasRelatedConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept2"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotRelatedConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareRelatedConcepts(new RDFResource("ex:concept2"), new RDFResource("ex:concept3"));

            Assert.IsFalse(conceptScheme.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsFalse(conceptScheme.CheckHasRelatedConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1")));
        }

        [TestMethod]
        public void ShouldGetBroadMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetBroadMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetBroadMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(conceptScheme.GetBroadMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
        }

        [TestMethod]
        public void ShouldCheckHasBroadMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasBroadMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldCheckHasNotBroadMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareBroadMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(conceptScheme.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetNarrowMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetNarrowMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetNarrowMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(conceptScheme.GetNarrowMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
        }

        [TestMethod]
        public void ShouldCheckHasNarrowMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasNarrowMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldCheckHasNotNarrowMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(conceptScheme.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetRelatedMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetRelatedMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetRelatedMatchConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(conceptScheme.GetRelatedMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(conceptScheme.GetRelatedMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(conceptScheme.GetRelatedMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
            Assert.IsTrue(conceptScheme.GetRelatedMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasRelatedMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasRelatedMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasRelatedMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasRelatedMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
            Assert.IsTrue(conceptScheme.CheckHasRelatedMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept3"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotRelatedMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareRelatedMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(conceptScheme.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetCloseMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetCloseMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetCloseMatchConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(conceptScheme.GetCloseMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(conceptScheme.GetCloseMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(conceptScheme.GetCloseMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
            Assert.IsTrue(conceptScheme.GetCloseMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasCloseMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasCloseMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasCloseMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasCloseMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
            Assert.IsTrue(conceptScheme.CheckHasCloseMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept3"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotCloseMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(conceptScheme.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetExactMatchConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept2")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3"))));
            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept3")).Any(c => c.Equals(new RDFResource("ex:concept4"))));
            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
            Assert.IsTrue(conceptScheme.GetExactMatchConcepts(new RDFResource("ex:concept4")).Any(c => c.Equals(new RDFResource("ex:concept1")))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasExactMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept2"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept1"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept3"), new RDFResource("ex:concept4")));
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept3"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //Inference
            Assert.IsTrue(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept4"), new RDFResource("ex:concept1"))); //Inference
        }

        [TestMethod]
        public void ShouldCheckHasNotExactMatchConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareCloseMatchConcepts(new RDFResource("ex:concept3"), new RDFResource("ex:concept4"));

            Assert.IsFalse(conceptScheme.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4")));
        }

        [TestMethod]
        public void ShouldGetMappingRelatedConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareMappingRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"));

            Assert.IsTrue(conceptScheme.GetMappingRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetMappingRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
            Assert.IsTrue(conceptScheme.GetMappingRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
        }

        [TestMethod]
        public void ShouldGetSemanticRelatedConcepts()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareSemanticRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"));
            conceptScheme.DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"));
            conceptScheme.DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"));

            Assert.IsTrue(conceptScheme.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept2"))));
            Assert.IsTrue(conceptScheme.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept3")))); //Inference
            Assert.IsTrue(conceptScheme.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept4")))); //Inference
            Assert.IsTrue(conceptScheme.GetSemanticRelatedConcepts(new RDFResource("ex:concept1")).Any(c => c.Equals(new RDFResource("ex:concept5")))); //Inference
        }

        [TestMethod]
        public void ShouldGetNotations()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareNotation(new RDFResource("ex:concept"), new RDFPlainLiteral("notation"));

            Assert.IsTrue(conceptScheme.GetConceptNotations(new RDFResource("ex:concept")).Any(c => c.Equals(new RDFPlainLiteral("notation"))));
        }
        #endregion
    }
}