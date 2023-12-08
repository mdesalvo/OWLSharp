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

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyPropertyModelLoaderTest
    {
        #region Initialize
        private OWLOntology Ontology { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RDFGraph graph = new RDFGraph();

            //Declarations (Model)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop3"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop4"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:annprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop3"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop3"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop4"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop5"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop6"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:propertyChainAxiom"), RDFVocabulary.OWL.PROPERTY_CHAIN_AXIOM, new RDFResource("bnode:pcaMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop11")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:pcaMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldisjointproperties"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldisjointproperties"), RDFVocabulary.OWL.MEMBERS, new RDFResource("bnode:alldisjointMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:alldisjointMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop5")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            //Annotations
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
            //Relations
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:objprop1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.OWL.EQUIVALENT_PROPERTY, new RDFResource("ex:objprop3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, new RDFResource("ex:objprop4")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:objprop6")));

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
        public void ShouldLoadAnnotationPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAnnotationProperty(new RDFResource("ex:annprop")));
        }

        [TestMethod]
        public void ShouldLoadDatatypePropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDatatypeProperty(new RDFResource("ex:dtprop")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDatatypeProperty(new RDFResource("ex:dtprop2")));
        }

        [TestMethod]
        public void ShouldLoadObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop1")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop2")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop3")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop4")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop5")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop6")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:propertyChainAxiom")));
        }

        [TestMethod]
        public void ShouldLoadDeprecatedPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDeprecatedProperty(new RDFResource("ex:objprop4")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDeprecatedProperty(new RDFResource("ex:dtprop")));
        }

        [TestMethod]
        public void ShouldLoadSymmetricObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasSymmetricProperty(new RDFResource("ex:objprop1")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasSymmetricProperty(new RDFResource("ex:objprop5")));
        }

        [TestMethod]
        public void ShouldLoadTransitiveObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasTransitiveProperty(new RDFResource("ex:objprop1")));
        }

        [TestMethod]
        public void ShouldLoadFunctionalPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasFunctionalProperty(new RDFResource("ex:objprop2")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasFunctionalProperty(new RDFResource("ex:dtprop2")));
        }

        [TestMethod]
        public void ShouldLoadInverseFunctionalObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasInverseFunctionalProperty(new RDFResource("ex:objprop2")));
        }

        [TestMethod]
        public void ShouldLoadAsymmetricObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAsymmetricProperty(new RDFResource("ex:objprop3")));
        }

        [TestMethod]
        public void ShouldLoadReflexiveObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasReflexiveProperty(new RDFResource("ex:objprop3")));
        }

        [TestMethod]
        public void ShouldLoadIrreflexiveObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasIrreflexiveProperty(new RDFResource("ex:objprop4")));
        }

        [TestMethod]
        public void ShouldLoadAllDisjointPropertiesDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAllDisjointProperties(new RDFResource("ex:alldisjointproperties")));
        }

        [TestMethod]
        public void ShouldLoadPropertyChainAxiomDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasPropertyChainAxiom(new RDFResource("ex:propertyChainAxiom")));
        }

        [TestMethod]
        public void ShouldLoadSubPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetSubPropertiesOf(new RDFResource("ex:objprop1")).Count == 2);
        }

        [TestMethod]
        public void ShouldLoadSuperPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetSuperPropertiesOf(new RDFResource("ex:objprop2")).Count == 1);
            Assert.IsTrue(Ontology.Model.PropertyModel.GetSuperPropertiesOf(new RDFResource("ex:objprop3")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadEquivalentPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:objprop2")).Count == 1);
            Assert.IsTrue(Ontology.Model.PropertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:objprop3")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadDisjointPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetDisjointPropertiesWith(new RDFResource("ex:objprop1")).Count == 2);
        }

        [TestMethod]
        public void ShouldLoadInversePropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetInversePropertiesOf(new RDFResource("ex:objprop1")).Count == 1);
            Assert.IsTrue(Ontology.Model.PropertyModel.GetInversePropertiesOf(new RDFResource("ex:objprop6")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadPropertyAnnotations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:objprop1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:objprop1"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
        }
        #endregion
    }
}