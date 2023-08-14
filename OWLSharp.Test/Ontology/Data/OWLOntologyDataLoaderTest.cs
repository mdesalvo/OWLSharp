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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyDataLoaderTest
    {
        #region Initialize
        private OWLOntology Ontology { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RDFGraph graph = new RDFGraph();

            //Declarations (Model)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:annprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            //Declarations (Data)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INDIVIDUAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv3"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv4"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldiff"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldiff"), RDFVocabulary.OWL.DISTINCT_MEMBERS, new RDFResource("bnode:distinctMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv3")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:distinctMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv4")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            //Annotations
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv2"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
            //Relations
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.OWL.SAME_AS, new RDFResource("ex:indiv2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.OWL.DIFFERENT_FROM, new RDFResource("ex:indiv3")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indiv1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:indiv2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indiv1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFResource("ex:dtprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.OWL.TARGET_VALUE, new RDFPlainLiteral("negval")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("val")));


            //Load
            Ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(Ontology);
            Assert.IsNotNull(Ontology.Model);
            Assert.IsNotNull(Ontology.Model.ClassModel);
            Assert.IsNotNull(Ontology.Model.PropertyModel);
            Assert.IsNotNull(Ontology.Data);
        }
        #endregion

        #region Test
        [TestMethod]
        public void ShouldLoadIndividualDeclarations()
        {   
            Assert.IsTrue(Ontology.Data.IndividualsCount == 4);
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv1")));
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv2")));
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv3")));
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv4")));
        }

        [TestMethod]
        public void ShouldLoadIndividualAnnotations()
        {
            Assert.IsTrue(Ontology.Data.CheckHasAnnotation(new RDFResource("ex:indiv1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            Assert.IsTrue(Ontology.Data.CheckHasAnnotation(new RDFResource("ex:indiv2"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
        }

        [TestMethod]
        public void ShouldLoadSameAsRelations()
        {
            Assert.IsTrue(Ontology.Data.CheckIsSameIndividual(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2")));
            Assert.IsTrue(Ontology.Data.CheckIsSameIndividual(new RDFResource("ex:indiv2"), new RDFResource("ex:indiv1")));
        }

        [TestMethod]
        public void ShouldLoadDifferentFromRelations()
        {
            Assert.IsTrue(Ontology.Data.CheckIsDifferentIndividual(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv3")));
            Assert.IsTrue(Ontology.Data.CheckIsDifferentIndividual(new RDFResource("ex:indiv3"), new RDFResource("ex:indiv1")));
        }

        [TestMethod]
        public void ShouldLoadNegativeObjectAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasNegativeObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2")));

        [TestMethod]
        public void ShouldLoadNegativeDatatypeAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasNegativeDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("negval")));

        [TestMethod]
        public void ShouldLoadObjectAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv3")));

        [TestMethod]
        public void ShouldLoadDatatypeAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("val")));

        [TestMethod]
        public void ShouldLoadAllDifferentRelations()
        {
            Assert.IsTrue(Ontology.Data.AllDifferentCount == 1);
            IEnumerator<RDFResource> alldiffEnum = Ontology.Data.AllDifferentEnumerator;
            while (alldiffEnum.MoveNext())
                Assert.IsTrue(alldiffEnum.Current.Equals(new RDFResource("ex:alldiff")));
            Ontology.Data.CheckIsDifferentIndividual(new RDFResource("ex:indiv3"), new RDFResource("ex:indiv4"));
        }
        #endregion
    }
}