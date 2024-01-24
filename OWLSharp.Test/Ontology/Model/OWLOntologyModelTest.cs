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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyModelTest
    {
        #region Test
        [TestMethod]
        public void ShouldCreateModel()
        {
            OWLOntologyModel model = new OWLOntologyModel();

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.ClassModel);
            Assert.IsNotNull(model.PropertyModel);
        }

        [TestMethod]
        public void ShouldCreateModelFromClassAndProperty()
        {
            OWLOntologyModel model = new OWLOntologyModel(new OWLOntologyClassModel(), new OWLOntologyPropertyModel());

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.ClassModel);
            Assert.IsNotNull(model.PropertyModel);
        }

        [TestMethod]
        public void ShouldDisposeModelWithUsing()
        {
            OWLOntologyModel model;
            using (model = new OWLOntologyModel())
            {
                Assert.IsFalse(model.Disposed);
                Assert.IsNotNull(model.ClassModel);
                Assert.IsNotNull(model.PropertyModel);
            };
            Assert.IsTrue(model.Disposed);
            Assert.IsNull(model.ClassModel);
            Assert.IsNull(model.PropertyModel);
        }

        [TestMethod]
        public void ShouldMergeModel()
        {
            OWLOntologyModel model = new OWLOntologyModel();
            model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:propertyC"), new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.RDFS.RESOURCE, Range = RDFVocabulary.RDFS.RESOURCE });
            model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:propertyD"));

            OWLOntologyModel model2 = new OWLOntologyModel();
            model2.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            model2.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            model2.ClassModel.DeclareSubClasses(new RDFResource("ex:classC"), new RDFResource("ex:classA"));
            model2.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            model2.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyE"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            model2.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyE"), new RDFResource("ex:propertyA"));

            model.Merge(model2);
            model.Merge(null); //Acts like a no-op

            Assert.IsTrue(model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:classC")));
            Assert.IsTrue(model.ClassModel.CheckIsSubClassOf(new RDFResource("ex:classC"), new RDFResource("ex:classA")));
            Assert.IsTrue(model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:propertyE")));
            Assert.IsTrue(model.PropertyModel.CheckIsEquivalentPropertyOf(new RDFResource("ex:propertyE"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldExportToGraph()
        {
            OWLOntologyModel model = new OWLOntologyModel();

            //ClassModel definition from tests
            model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classD"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classE"), new OWLOntologyClassBehavior() { Deprecated = true });
            model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            model.ClassModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            model.ClassModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            model.ClassModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));

            //PropertyModel definition from tests
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"), new OWLOntologyObjectPropertyBehavior() { Reflexive = true });
            model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:propertyE"), new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.RDFS.RESOURCE, Range = RDFVocabulary.RDFS.RESOURCE });
            model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:propertyF"));
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyG"), new OWLOntologyObjectPropertyBehavior() { Deprecated = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyH"), new OWLOntologyObjectPropertyBehavior() { Irreflexive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyI"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyJ"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            model.PropertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyC"));
            model.PropertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));
            model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:propertyChainAxiom"), new List<RDFResource>() { new RDFResource("ex:propertyA") });
            model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("ex:allDisjointProperties"), new List<RDFResource>() { new RDFResource("ex:propertyH"), new RDFResource("ex:propertyI") });
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("comment"));
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("title"));

            RDFGraph graph = model.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 74);
        }

        [TestMethod]
        public void ShouldExportToGraphWithoutInferences()
        {
            OWLOntologyModel model = new OWLOntologyModel();

            //ClassModel definition from tests
            model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classD"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classE"), new OWLOntologyClassBehavior() { Deprecated = true });
            model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            model.ClassModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            model.ClassModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            model.ClassModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));

            //PropertyModel definition from tests
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"), new OWLOntologyObjectPropertyBehavior() { Reflexive = true });
            model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:propertyE"), new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.RDFS.RESOURCE, Range = RDFVocabulary.RDFS.RESOURCE });
            model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:propertyF"));
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyG"), new OWLOntologyObjectPropertyBehavior() { Deprecated = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyH"), new OWLOntologyObjectPropertyBehavior() { Irreflexive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyI"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyJ"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            model.PropertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyC"));
            model.PropertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));
            model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:propertyChainAxiom"), new List<RDFResource>() { new RDFResource("ex:propertyA") });
            model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("ex:allDisjointProperties"), new List<RDFResource>() { new RDFResource("ex:propertyH"), new RDFResource("ex:propertyI") });
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("comment"));
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("title"));

            RDFGraph graph = model.ToRDFGraph(false);

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 70);
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsync()
        {
            OWLOntologyModel model = new OWLOntologyModel();

            //ClassModel definition from tests
            model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classD"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classE"), new OWLOntologyClassBehavior() { Deprecated = true });
            model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            model.ClassModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            model.ClassModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            model.ClassModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));

            //PropertyModel definition from tests
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"), new OWLOntologyObjectPropertyBehavior() { Reflexive = true });
            model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:propertyE"), new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.RDFS.RESOURCE, Range = RDFVocabulary.RDFS.RESOURCE });
            model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:propertyF"));
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyG"), new OWLOntologyObjectPropertyBehavior() { Deprecated = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyH"), new OWLOntologyObjectPropertyBehavior() { Irreflexive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyI"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyJ"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            model.PropertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyC"));
            model.PropertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));
            model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:propertyChainAxiom"), new List<RDFResource>() { new RDFResource("ex:propertyA") });
            model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("ex:allDisjointProperties"), new List<RDFResource>() { new RDFResource("ex:propertyH"), new RDFResource("ex:propertyI") });
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("comment"));
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("title"));

            RDFGraph graph = await model.ToRDFGraphAsync();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 74);
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsyncWithoutInferences()
        {
            OWLOntologyModel model = new OWLOntologyModel();

            //ClassModel definition from tests
            model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classD"));
            model.ClassModel.DeclareClass(new RDFResource("ex:classE"), new OWLOntologyClassBehavior() { Deprecated = true });
            model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            model.ClassModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            model.ClassModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            model.ClassModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            model.ClassModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));

            //PropertyModel definition from tests
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"), new OWLOntologyObjectPropertyBehavior() { Reflexive = true });
            model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:propertyE"), new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.RDFS.RESOURCE, Range = RDFVocabulary.RDFS.RESOURCE });
            model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:propertyF"));
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyG"), new OWLOntologyObjectPropertyBehavior() { Deprecated = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyH"), new OWLOntologyObjectPropertyBehavior() { Irreflexive = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyI"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:propertyJ"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            model.PropertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyC"));
            model.PropertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));
            model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:propertyChainAxiom"), new List<RDFResource>() { new RDFResource("ex:propertyA") });
            model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("ex:allDisjointProperties"), new List<RDFResource>() { new RDFResource("ex:propertyH"), new RDFResource("ex:propertyI") });
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("comment"));
            model.PropertyModel.AnnotateProperty(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyF"), new RDFPlainLiteral("title"));

            RDFGraph graph = await model.ToRDFGraphAsync(false);

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 70);
        }
        #endregion
    }
}