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

using RDFSharp.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSOntologyLensTest
    {
        #region Initialize
        private OWLOntology Ontology { get; set; }
        private SKOSOntologyLens OntologyLens { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Ontology = new OWLOntology("ex:ont")
                .Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test SKOS ontology"))
                .DeclareConceptScheme(new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept5"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept6"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept7"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept8"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept9"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept10"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept11"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept12"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept13"), new RDFResource("ex:conceptScheme"))
                .DeclareConcept(new RDFResource("ex:concept14"), new RDFResource("ex:conceptScheme"))
                .AnnotateConcept(new RDFResource("ex:concept1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test concept"))
                .DeclareConceptPreferredLabel(new RDFResource("ex:concept1"), new RDFResource("ex:label1"), new RDFPlainLiteral("concept1", "en-US"))
                .DeclareConceptAlternativeLabel(new RDFResource("ex:concept1"), new RDFResource("ex:label2"), new RDFPlainLiteral("konzept1", "de-DE"))
                .DeclareConceptHiddenLabel(new RDFResource("ex:concept1"), new RDFResource("ex:label3"), new RDFPlainLiteral("concetto1", "it-IT"))
                .AnnotateConceptWithPreferredLabel(new RDFResource("ex:concept1"), new RDFPlainLiteral("concept1"))
                .AnnotateConceptWithAlternativeLabel(new RDFResource("ex:concept1"), new RDFPlainLiteral("konzept1"))
                .AnnotateConceptWithHiddenLabel(new RDFResource("ex:concept1"), new RDFPlainLiteral("concetto1"))
                .DeclareNotation(new RDFResource("ex:concept1"), new RDFTypedLiteral("this is concept 1", RDFModelEnums.RDFDatatypes.RDFS_LITERAL))
                .DocumentConcept(new RDFResource("ex:concept1"), SKOSEnums.SKOSDocumentationTypes.Note, new RDFPlainLiteral("note"))
                .DocumentConcept(new RDFResource("ex:concept1"), SKOSEnums.SKOSDocumentationTypes.ChangeNote, new RDFPlainLiteral("note"))
                .DocumentConcept(new RDFResource("ex:concept1"), SKOSEnums.SKOSDocumentationTypes.EditorialNote, new RDFPlainLiteral("note"))                
                .DocumentConcept(new RDFResource("ex:concept1"), SKOSEnums.SKOSDocumentationTypes.HistoryNote, new RDFPlainLiteral("note"))
                .DocumentConcept(new RDFResource("ex:concept1"), SKOSEnums.SKOSDocumentationTypes.ScopeNote, new RDFPlainLiteral("note"))
                .DocumentConcept(new RDFResource("ex:concept1"), SKOSEnums.SKOSDocumentationTypes.Definition, new RDFTypedLiteral("this is concept 2", RDFModelEnums.RDFDatatypes.RDFS_LITERAL))
                .DocumentConcept(new RDFResource("ex:concept1"), SKOSEnums.SKOSDocumentationTypes.Example, new RDFTypedLiteral("this is concept 2", RDFModelEnums.RDFDatatypes.RDFS_LITERAL))
                .DeclareNarrowMatchConcepts(new RDFResource("ex:concept1"), new RDFResource("ex:concept2"))
                .DeclareCollection(new RDFResource("ex:collection1"), new List<RDFResource>() { new RDFResource("ex:concept1"), new RDFResource("ex:collection2") }, new RDFResource("ex:conceptScheme"))
                .AnnotateCollection(new RDFResource("ex:collection1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test collection"))
                .DeclareCollection(new RDFResource("ex:collection2"), new List<RDFResource>() { new RDFResource("ex:concept2"), new RDFResource("ex:collection1") }, new RDFResource("ex:conceptScheme"))    
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
                .DeclareTopConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptScheme"));
            Ontology.InitializeSKOS();

            OntologyLens = new SKOSOntologyLens(new RDFResource("ex:concept1"), Ontology);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void ShouldBeTopConcept()
            => Assert.IsTrue(OntologyLens.IsTopConceptOf(new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public async Task ShouldBeTopConceptAsync()
            => Assert.IsTrue(await OntologyLens.IsTopConceptOfAsync(new RDFResource("ex:conceptScheme")));

        [TestMethod]
        public void ShouldGetBroaderConcepts()
            => Assert.IsTrue(OntologyLens.BroaderConcepts().Count == 3);

        [TestMethod]
        public async Task ShouldGetBroaderConceptsAsync()
            => Assert.IsTrue((await OntologyLens.BroaderConceptsAsync()).Count == 3);

        [TestMethod]
        public void ShouldGetNarrowerConcepts()
            => Assert.IsTrue(OntologyLens.NarrowerConcepts().Count == 3);

        [TestMethod]
        public async Task ShouldGetNarrowerConceptsAsync()
            => Assert.IsTrue((await OntologyLens.NarrowerConceptsAsync()).Count == 3);

        [TestMethod]
        public void ShouldGetBroadMatchConcepts()
           => Assert.IsTrue(OntologyLens.BroadMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetBroadMatchConceptsAsync()
            => Assert.IsTrue((await OntologyLens.BroadMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetNarrowMatchConcepts()
            => Assert.IsTrue(OntologyLens.NarrowMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetNarrowMatchConceptsAsync()
            => Assert.IsTrue((await OntologyLens.NarrowMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetCloseMatchConcepts()
          => Assert.IsTrue(OntologyLens.CloseMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetCloseMatchConceptsAsync()
            => Assert.IsTrue((await OntologyLens.CloseMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetExactMatchConcepts()
            => Assert.IsTrue(OntologyLens.ExactMatchConcepts().Count == 2);

        [TestMethod]
        public async Task ShouldGetExactMatchConceptsAsync()
            => Assert.IsTrue((await OntologyLens.ExactMatchConceptsAsync()).Count == 2);

        [TestMethod]
        public void ShouldGetRelatedConcepts()
          => Assert.IsTrue(OntologyLens.RelatedConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetRelatedConceptsAsync()
            => Assert.IsTrue((await OntologyLens.RelatedConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetRelatedMatchConcepts()
          => Assert.IsTrue(OntologyLens.RelatedMatchConcepts().Count == 1);

        [TestMethod]
        public async Task ShouldGetRelatedMatchConceptsAsync()
            => Assert.IsTrue((await OntologyLens.RelatedMatchConceptsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetMappingRelatedConcepts()
          => Assert.IsTrue(OntologyLens.MappingRelatedConcepts().Count == 5);

        [TestMethod]
        public async Task ShouldGetMappingRelatedConceptsAsync()
            => Assert.IsTrue((await OntologyLens.MappingRelatedConceptsAsync()).Count == 5);

        [TestMethod]
        public void ShouldGetSemanticRelatedConcepts()
          => Assert.IsTrue(OntologyLens.SemanticRelatedConcepts().Count == 10);

        [TestMethod]
        public async Task ShouldGetSemanticRelatedConceptsAsync()
            => Assert.IsTrue((await OntologyLens.SemanticRelatedConceptsAsync()).Count == 10);

        [TestMethod]
        public void ShouldGetNotations()
          => Assert.IsTrue(OntologyLens.Notations().Count == 1);

        [TestMethod]
        public async Task ShouldGetNotationsAsync()
            => Assert.IsTrue((await OntologyLens.NotationsAsync()).Count == 1);

        [TestMethod]
        public void ShouldGetLabelRelations()
          => Assert.IsTrue(OntologyLens.LabelRelations().Count == 3
                            && OntologyLens.LabelRelations().Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL) && lr.Item2.Equals(new RDFResource("ex:label1")))
                             && OntologyLens.LabelRelations().Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL) && lr.Item2.Equals(new RDFResource("ex:label2")))
                              && OntologyLens.LabelRelations().Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL) && lr.Item2.Equals(new RDFResource("ex:label3"))));

        [TestMethod]
        public async Task ShouldGetLabelRelationsAsync()
            => Assert.IsTrue((await OntologyLens.LabelRelationsAsync()).Count == 3
                              && (await OntologyLens.LabelRelationsAsync()).Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL) && lr.Item2.Equals(new RDFResource("ex:label1")))
                               && (await OntologyLens.LabelRelationsAsync()).Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL) && lr.Item2.Equals(new RDFResource("ex:label2")))
                                && (await OntologyLens.LabelRelationsAsync()).Any(lr => lr.Item1.Equals(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL) && lr.Item2.Equals(new RDFResource("ex:label3"))));

        [TestMethod]
        public void ShouldGetLabelAnnotations()
          => Assert.IsTrue(OntologyLens.LabelAnnotations().Count == 3
                            && OntologyLens.LabelAnnotations().Any(la => la.Item1.Equals(RDFVocabulary.SKOS.PREF_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concept1")))
                             && OntologyLens.LabelAnnotations().Any(la => la.Item1.Equals(RDFVocabulary.SKOS.ALT_LABEL) && la.Item2.Equals(new RDFPlainLiteral("konzept1")))
                              && OntologyLens.LabelAnnotations().Any(la => la.Item1.Equals(RDFVocabulary.SKOS.HIDDEN_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concetto1"))));

        [TestMethod]
        public async Task ShouldGetLabelAnnotationsAsync()
            => Assert.IsTrue((await OntologyLens.LabelAnnotationsAsync()).Count == 3
                              && (await OntologyLens.LabelAnnotationsAsync()).Any(la => la.Item1.Equals(RDFVocabulary.SKOS.PREF_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concept1")))
                               && (await OntologyLens.LabelAnnotationsAsync()).Any(la => la.Item1.Equals(RDFVocabulary.SKOS.ALT_LABEL) && la.Item2.Equals(new RDFPlainLiteral("konzept1")))
                                && (await OntologyLens.LabelAnnotationsAsync()).Any(la => la.Item1.Equals(RDFVocabulary.SKOS.HIDDEN_LABEL) && la.Item2.Equals(new RDFPlainLiteral("concetto1"))));

        [TestMethod]
        public void ShouldGetDocumentationAnnotations()
          => Assert.IsTrue(OntologyLens.DocumentationAnnotations().Count == 7);

        [TestMethod]
        public async Task ShouldGetDocumentationAnnotationsAsync()
            => Assert.IsTrue((await OntologyLens.DocumentationAnnotationsAsync()).Count == 7);
        #endregion
    }
}