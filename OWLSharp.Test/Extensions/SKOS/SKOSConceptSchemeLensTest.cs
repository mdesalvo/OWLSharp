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

using RDFSharp.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSConceptSchemeLensTest
    {
        #region Initialize
        private SKOSConceptScheme ConceptScheme { get; set; }
        private SKOSConceptSchemeLens ConceptSchemeLens { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            ConceptScheme = new SKOSConceptScheme("ex:conceptScheme")
                .Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept scheme"))
                .DeclareConcept(new RDFResource("ex:concept1"))
                .DeclareConcept(new RDFResource("ex:concept2"))
                .DeclareConcept(new RDFResource("ex:concept3"))
                .DeclareConcept(new RDFResource("ex:concept4"))
                .DeclareConcept(new RDFResource("ex:concept5"))
                .DeclareConcept(new RDFResource("ex:concept6"))
                .DeclareConcept(new RDFResource("ex:concept7"))
                .DeclareConcept(new RDFResource("ex:concept8"))
                .DeclareConcept(new RDFResource("ex:concept9"))
                .DeclareConcept(new RDFResource("ex:concept10"))
                .DeclareConcept(new RDFResource("ex:concept11"))
                .DeclareConcept(new RDFResource("ex:concept12"))
                .DeclareConcept(new RDFResource("ex:concept13"))
                .DeclareConcept(new RDFResource("ex:concept14"))
                .AnnotateConcept(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept"))
                .DeclarePreferredLabel(new RDFResource("ex:concept1"), new RDFResource("ex:label1"), new RDFPlainLiteral("concept1", "en-US"))
                .DeclareAlternativeLabel(new RDFResource("ex:concept1"), new RDFResource("ex:label2"), new RDFPlainLiteral("konzept1", "de-DE"))
                .DeclareHiddenLabel(new RDFResource("ex:concept1"), new RDFResource("ex:label3"), new RDFPlainLiteral("concetto1", "it-IT"))
                .DeclarePreferredLabel(new RDFResource("ex:concept1"), new RDFPlainLiteral("concept1"))
                .DeclareAlternativeLabel(new RDFResource("ex:concept1"), new RDFPlainLiteral("konzept1"))
                .DeclareHiddenLabel(new RDFResource("ex:concept1"), new RDFPlainLiteral("concetto1"))
                .DeclareNotation(new RDFResource("ex:concept1"), new RDFTypedLiteral("this is concept 1", RDFModelEnums.RDFDatatypes.RDFS_LITERAL))
                .DocumentConceptWithNote(new RDFResource("ex:concept1"), new RDFPlainLiteral("note"))
                .DocumentConceptWithChangeNote(new RDFResource("ex:concept1"), new RDFPlainLiteral("note"))
                .DocumentConceptWithEditorialNote(new RDFResource("ex:concept1"), new RDFPlainLiteral("note"))                
                .DocumentConceptWithHistoryNote(new RDFResource("ex:concept1"), new RDFPlainLiteral("note"))
                .DocumentConceptWithScopeNote(new RDFResource("ex:concept1"), new RDFPlainLiteral("note"))
                .DocumentConceptWithDefinition(new RDFResource("ex:concept1"), new RDFTypedLiteral("this is concept 2", RDFModelEnums.RDFDatatypes.RDFS_LITERAL))
                .DocumentConceptWithExample(new RDFResource("ex:concept1"), new RDFTypedLiteral("this is concept 2", RDFModelEnums.RDFDatatypes.RDFS_LITERAL))
                .DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"))
                .DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") })
                .AnnotateCollection(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection"))
                .DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() { new RDFResource("ex:concept2"), new RDFResource("ex:collection1") })    
                .DeclareBroaderConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))
                .DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))
                .DeclareBroaderTransitiveConcepts(new RDFResource("ex:concept4"), new RDFResource("ex:concept5"))
                .DeclareNarrowerConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept6"))
                .DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept7"))
                .DeclareNarrowerTransitiveConcepts(new RDFResource("ex:concept7"), new RDFResource("ex:concept8"))
                .DeclareBroadMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept9"))
                .DeclareCloseMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept10"))
                .DeclareExactMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept11"))
                .DeclareExactMatchConcepts(new RDFResource("ex:concept11"), new RDFResource("ex:concept12"))
                .DeclareRelatedConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept13"))
                .DeclareRelatedMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept14"))
                .DeclareTopConcept(new RDFResource("ex:concept1"));

            ConceptSchemeLens = new SKOSConceptSchemeLens(new RDFResource("ex:concept1"), ConceptScheme);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void ShouldBeTopConcept()
            => Assert.IsTrue(ConceptSchemeLens.IsTopConcept());

        [TestMethod]
        public async Task ShouldBeTopConceptAsync()
            => Assert.IsTrue(await ConceptSchemeLens.IsTopConceptAsync());

        [TestMethod]
        public void ShouldGetBroaderConcepts()
            => Assert.IsTrue(ConceptSchemeLens.BroaderConcepts().Count == 3);

        [TestMethod]
        public async Task ShouldGetBroaderConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.BroaderConceptsAsync()).Count == 3);

        [TestMethod]
        public void ShouldGetNarrowerConcepts()
            => Assert.IsTrue(ConceptSchemeLens.NarrowerConcepts().Count == 3);

        [TestMethod]
        public async Task ShouldGetNarrowerConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.NarrowerConceptsAsync()).Count == 3);

        [TestMethod]
        public void ShouldGetBroadMatchConcepts()
           => Assert.IsTrue(ConceptSchemeLens.BroadMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetBroadMatchConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.BroadMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetNarrowMatchConcepts()
            => Assert.IsTrue(ConceptSchemeLens.NarrowMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetNarrowMatchConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.NarrowMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetCloseMatchConcepts()
          => Assert.IsTrue(ConceptSchemeLens.CloseMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetCloseMatchConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.CloseMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetExactMatchConcepts()
            => Assert.IsTrue(ConceptSchemeLens.ExactMatchConcepts().Count == 2);

        [TestMethod]
        public async Task ShouldGetExactMatchConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.ExactMatchConceptsAsync()).Count == 2);

        [TestMethod]
        public void ShouldGetRelatedConcepts()
          => Assert.IsTrue(ConceptSchemeLens.RelatedConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetRelatedConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.RelatedConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetRelatedMatchConcepts()
          => Assert.IsTrue(ConceptSchemeLens.RelatedMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetRelatedMatchConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.RelatedMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetMappingRelatedConcepts()
          => Assert.IsTrue(ConceptSchemeLens.MappingRelatedConcepts().Count == 5);

        [TestMethod]
        public async Task ShouldGetMappingRelatedConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.MappingRelatedConceptsAsync()).Count == 5);

        [TestMethod]
        public void ShouldGetSemanticRelatedConcepts()
          => Assert.IsTrue(ConceptSchemeLens.SemanticRelatedConcepts().Count == 10);

        [TestMethod]
        public async Task ShouldGetSemanticRelatedConceptsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.SemanticRelatedConceptsAsync()).Count == 10);

        [TestMethod]
        public void ShouldGetNotations()
          => Assert.IsTrue(ConceptSchemeLens.Notations().Count == 1);

        [TestMethod]
        public async Task ShouldGetNotationsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.NotationsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetLabelRelations()
          => Assert.IsTrue(ConceptSchemeLens.LabelRelations().Count == 3
                            && ConceptSchemeLens.LabelRelations().Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL) && lr.Item2.Equals(new RDFResource("ex:label1")))
                             && ConceptSchemeLens.LabelRelations().Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL) && lr.Item2.Equals(new RDFResource("ex:label2")))
                              && ConceptSchemeLens.LabelRelations().Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL) && lr.Item2.Equals(new RDFResource("ex:label3"))));

        [TestMethod]
        public async Task ShouldGetLabelRelationsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.LabelRelationsAsync()).Count == 3
                              && (await ConceptSchemeLens.LabelRelationsAsync()).Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL) && lr.Item2.Equals(new RDFResource("ex:label1")))
                               && (await ConceptSchemeLens.LabelRelationsAsync()).Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL) && lr.Item2.Equals(new RDFResource("ex:label2")))
                                && (await ConceptSchemeLens.LabelRelationsAsync()).Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL) && lr.Item2.Equals(new RDFResource("ex:label3"))));

        [TestMethod]
        public void ShouldGetLabelAnnotations()
          => Assert.IsTrue(ConceptSchemeLens.LabelAnnotations().Count == 3
                            && ConceptSchemeLens.LabelAnnotations().Any(la => la.Item1.Equals(RDFVocabulary.SKOS.PREF_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concept1")))
                             && ConceptSchemeLens.LabelAnnotations().Any(la => la.Item1.Equals(RDFVocabulary.SKOS.ALT_LABEL) && la.Item2.Equals(new RDFPlainLiteral("konzept1")))
                              && ConceptSchemeLens.LabelAnnotations().Any(la => la.Item1.Equals(RDFVocabulary.SKOS.HIDDEN_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concetto1"))));

        [TestMethod]
        public async Task ShouldGetLabelAnnotationsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.LabelAnnotationsAsync()).Count == 3
                              && (await ConceptSchemeLens.LabelAnnotationsAsync()).Any(la => la.Item1.Equals(RDFVocabulary.SKOS.PREF_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concept1")))
                               && (await ConceptSchemeLens.LabelAnnotationsAsync()).Any(la => la.Item1.Equals(RDFVocabulary.SKOS.ALT_LABEL) && la.Item2.Equals(new RDFPlainLiteral("konzept1")))
                                && (await ConceptSchemeLens.LabelAnnotationsAsync()).Any(la => la.Item1.Equals(RDFVocabulary.SKOS.HIDDEN_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concetto1"))));

        [TestMethod]
        public void ShouldGetDocumentationAnnotations()
          => Assert.IsTrue(ConceptSchemeLens.DocumentationAnnotations().Count == 7);

        [TestMethod]
        public async Task ShouldGetDocumentationAnnotationsAsync()
            => Assert.IsTrue((await ConceptSchemeLens.DocumentationAnnotationsAsync()).Count == 7);
        #endregion
    }
}