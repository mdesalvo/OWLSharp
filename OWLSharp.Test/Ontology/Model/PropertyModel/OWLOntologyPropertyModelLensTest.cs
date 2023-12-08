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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyPropertyModelLensTest
    {
        #region Initialize
        private OWLOntologyPropertyModelLens ModelLens { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            ModelLens = new OWLOntologyPropertyModelLens(new RDFResource("ex:objprop1"), new OWLOntology("ex:ont"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ModelLens.Ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:annObjProp"));
            ModelLens.Ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:annLitProp"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp0E"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp0S"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp1"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp2"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp3"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp4"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp5"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp6"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp7"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp8"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp9"));
            ModelLens.Ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtProp"));
            ModelLens.Ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:objprop1"), new RDFResource("ex:objprop0S"));
            ModelLens.Ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:objprop0S"), new RDFResource("ex:objprop0E"));
            ModelLens.Ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:objprop1"), new RDFResource("ex:objprop6"));
            ModelLens.Ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:objprop6"), new RDFResource("ex:objprop7"));
            ModelLens.Ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:objprop5"), new RDFResource("ex:objprop8"));
            ModelLens.Ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:objprop1"), new RDFResource("ex:objprop9"));
            ModelLens.Ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:objprop2"), new RDFResource("ex:objprop1"));
            ModelLens.Ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:objprop2"), new RDFResource("ex:objprop3"));
            ModelLens.Ontology.Model.PropertyModel.DeclareDisjointProperties(new RDFResource("ex:objprop1"), new RDFResource("ex:objprop4"));
            ModelLens.Ontology.Model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("ex:alldisjprop"), new List<RDFResource>() { new RDFResource("ex:objprop1"), new RDFResource("ex:objprop5") });
            ModelLens.Ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:objprop1"), new List<RDFResource>() { new RDFResource("ex:objprop8"), new RDFResource("ex:objprop9") });
            ModelLens.Ontology.Model.PropertyModel.AnnotateProperty(new RDFResource("ex:objprop1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann"));
            ModelLens.Ontology.Model.PropertyModel.AnnotateProperty(new RDFResource("ex:objprop1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann"));
        }
        #endregion

        #region Test
        [TestMethod]
        public void ShouldEnlistSubProperties()
        {
            List<RDFResource> subProperties = ModelLens.SubProperties();

            Assert.IsNotNull(subProperties);
            Assert.IsTrue(subProperties.Count == 2);
            Assert.IsTrue(subProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop2"))));
            Assert.IsTrue(subProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop3")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistSubPropertiesAsync()
        {
            List<RDFResource> subProperties = await ModelLens.SubPropertiesAsync();

            Assert.IsNotNull(subProperties);
            Assert.IsTrue(subProperties.Count == 2);
            Assert.IsTrue(subProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop2"))));
            Assert.IsTrue(subProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop3")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistSuperProperties()
        {
            List<RDFResource> superProperties = ModelLens.SuperProperties();

            Assert.IsNotNull(superProperties);
            Assert.IsTrue(superProperties.Count == 2);
            Assert.IsTrue(superProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop0S"))));
            Assert.IsTrue(superProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop0E")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistSuperPropertiesAsync()
        {
            List<RDFResource> superProperties = await ModelLens.SuperPropertiesAsync();

            Assert.IsNotNull(superProperties);
            Assert.IsTrue(superProperties.Count == 2);
            Assert.IsTrue(superProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop0S"))));
            Assert.IsTrue(superProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop0E")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistEquivalentProperties()
        {
            List<RDFResource> equivalentProperties = ModelLens.EquivalentProperties();

            Assert.IsNotNull(equivalentProperties);
            Assert.IsTrue(equivalentProperties.Count == 2);
            Assert.IsTrue(equivalentProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop6"))));
            Assert.IsTrue(equivalentProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop7")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistEquivalentPropertiesAsync()
        {
            List<RDFResource> equivalentProperties = await ModelLens.EquivalentPropertiesAsync();

            Assert.IsNotNull(equivalentProperties);
            Assert.IsTrue(equivalentProperties.Count == 2);
            Assert.IsTrue(equivalentProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop6"))));
            Assert.IsTrue(equivalentProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop7")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistDisjointProperties()
        {
            List<RDFResource> disjointProperties = ModelLens.DisjointProperties();

            Assert.IsNotNull(disjointProperties);
            Assert.IsTrue(disjointProperties.Count == 3);
            Assert.IsTrue(disjointProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop5")))); //Inference
            Assert.IsTrue(disjointProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop8")))); //Inference
            Assert.IsTrue(disjointProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop4"))));
        }

        [TestMethod]
        public async Task ShouldEnlistDisjointPropertiesAsync()
        {
            List<RDFResource> disjointProperties = await ModelLens.DisjointPropertiesAsync();

            Assert.IsNotNull(disjointProperties);
            Assert.IsTrue(disjointProperties.Count == 3);
            Assert.IsTrue(disjointProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop5")))); //Inference
            Assert.IsTrue(disjointProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop8")))); //Inference
            Assert.IsTrue(disjointProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop4"))));
        }

        [TestMethod]
        public void ShouldEnlistInverseProperties()
        {
            List<RDFResource> inverseProperties = ModelLens.InverseProperties();

            Assert.IsNotNull(inverseProperties);
            Assert.IsTrue(inverseProperties.Count == 1);
            Assert.IsTrue(inverseProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop9"))));
        }

        [TestMethod]
        public async Task ShouldEnlistInversePropertiesAsync()
        {
            List<RDFResource> inverseProperties = await ModelLens.InversePropertiesAsync();

            Assert.IsNotNull(inverseProperties);
            Assert.IsTrue(inverseProperties.Count == 1);
            Assert.IsTrue(inverseProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop9"))));
        }

        [TestMethod]
        public void ShouldEnlistChainAxiomProperties()
        {
            List<RDFResource> chainAxiomProperties = ModelLens.ChainAxiomProperties();

            Assert.IsNotNull(chainAxiomProperties);
            Assert.IsTrue(chainAxiomProperties.Count == 2);
            Assert.IsTrue(chainAxiomProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop8"))));
            Assert.IsTrue(chainAxiomProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop9"))));
        }

        [TestMethod]
        public async Task ShouldEnlistChainAxiomPropertiesAsync()
        {
            List<RDFResource> chainAxiomProperties = await ModelLens.ChainAxiomPropertiesAsync();

            Assert.IsNotNull(chainAxiomProperties);
            Assert.IsTrue(chainAxiomProperties.Count == 2);
            Assert.IsTrue(chainAxiomProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop8"))));
            Assert.IsTrue(chainAxiomProperties.Any(prop => prop.Equals(new RDFResource("ex:objprop9"))));
        }

        [TestMethod]
        public void ShouldEnlistObjectAnnotations()
        {
            List<RDFTriple> objAnnotations = ModelLens.ObjectAnnotations();

            Assert.IsNotNull(objAnnotations);
            Assert.IsTrue(objAnnotations.Count == 1);
            Assert.IsTrue(objAnnotations.Any(objAnn => objAnn.Equals(new RDFTriple(new RDFResource("ex:objprop1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann")))));
        }

        [TestMethod]
        public async Task ShouldEnlistObjectAnnotationsAsync()
        {
            List<RDFTriple> objAnnotations = await ModelLens.ObjectAnnotationsAsync();

            Assert.IsNotNull(objAnnotations);
            Assert.IsTrue(objAnnotations.Count == 1);
            Assert.IsTrue(objAnnotations.Any(objAnn => objAnn.Equals(new RDFTriple(new RDFResource("ex:objprop1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann")))));
        }

        [TestMethod]
        public void ShouldEnlistDataAnnotations()
        {
            List<RDFTriple> dtAnnotations = ModelLens.DataAnnotations();

            Assert.IsNotNull(dtAnnotations);
            Assert.IsTrue(dtAnnotations.Count == 1);
            Assert.IsTrue(dtAnnotations.Any(dtAsn => dtAsn.Equals(new RDFTriple(new RDFResource("ex:objprop1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann")))));
        }

        [TestMethod]
        public async Task ShouldEnlistDataAnnotationsAsync()
        {
            List<RDFTriple> dtAnnotations = await ModelLens.DataAnnotationsAsync();

            Assert.IsNotNull(dtAnnotations);
            Assert.IsTrue(dtAnnotations.Count == 1);
            Assert.IsTrue(dtAnnotations.Any(dtAsn => dtAsn.Equals(new RDFTriple(new RDFResource("ex:objprop1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann")))));
        }
        #endregion
    }
}