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
    public class OWLOntologyClassModelLensTest
    {
        #region Initialize
        private OWLOntologyClassModelLens ModelLens { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            ModelLens = new OWLOntologyClassModelLens(new RDFResource("ex:class1"), new OWLOntology("ex:ont"));
            ModelLens.Ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:annObjProp"));
            ModelLens.Ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:annLitProp"));
            ModelLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp"));
            ModelLens.Ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtProp"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class0S"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class0E"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2E"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class3"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class4"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class5"));
            ModelLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class6"));
            ModelLens.Ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class1"), new RDFResource("ex:class0S"));
            ModelLens.Ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class0S"), new RDFResource("ex:class0E"));
            ModelLens.Ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class2"), new RDFResource("ex:class1"));
            ModelLens.Ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class2"), new RDFResource("ex:class2E"));
            ModelLens.Ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class3"), new RDFResource("ex:class1"));
            ModelLens.Ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class6"), new RDFResource("ex:class3"));
            ModelLens.Ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:class3"), new RDFResource("ex:class4"));
            ModelLens.Ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class4"), new RDFResource("ex:class5"));
            ModelLens.Ontology.Model.ClassModel.DeclareHasKey(new RDFResource("ex:class1"), [new RDFResource("ex:objProp")]);
            ModelLens.Ontology.Model.ClassModel.AnnotateClass(new RDFResource("ex:class1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann"));
            ModelLens.Ontology.Model.ClassModel.AnnotateClass(new RDFResource("ex:class1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann"));
            ModelLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv0"));
            ModelLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ModelLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ModelLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ModelLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv4"));
            ModelLens.Ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv0"), new RDFResource("ex:class0S"));
            ModelLens.Ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ModelLens.Ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class2"));
            ModelLens.Ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv3"), new RDFResource("ex:class3"));
            ModelLens.Ontology.Data.DeclareNegativeIndividualType(ModelLens.Ontology.Model, new RDFResource("ex:indiv4"), new RDFResource("ex:class1"));
        }
        #endregion

        #region Test
        [TestMethod]
        public void ShouldEnlistSubClasses()
        {
            List<RDFResource> subClasses = ModelLens.SubClasses();

            Assert.IsNotNull(subClasses);
            Assert.IsTrue(subClasses.Count == 2);
            Assert.IsTrue(subClasses.Any(cls => cls.Equals(new RDFResource("ex:class2"))));
            Assert.IsTrue(subClasses.Any(cls => cls.Equals(new RDFResource("ex:class2E")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistSubClassesAsync()
        {
            List<RDFResource> subClasses = await ModelLens.SubClassesAsync();

            Assert.IsNotNull(subClasses);
            Assert.IsTrue(subClasses.Count == 2);
            Assert.IsTrue(subClasses.Any(cls => cls.Equals(new RDFResource("ex:class2"))));
            Assert.IsTrue(subClasses.Any(cls => cls.Equals(new RDFResource("ex:class2E")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistSuperClasses()
        {
            List<RDFResource> superClasses = ModelLens.SuperClasses();

            Assert.IsNotNull(superClasses);
            Assert.IsTrue(superClasses.Count == 2);
            Assert.IsTrue(superClasses.Any(cls => cls.Equals(new RDFResource("ex:class0S"))));
            Assert.IsTrue(superClasses.Any(cls => cls.Equals(new RDFResource("ex:class0E")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistSuperClassesAsync()
        {
            List<RDFResource> superClasses = await ModelLens.SuperClassesAsync();

            Assert.IsNotNull(superClasses);
            Assert.IsTrue(superClasses.Count == 2);
            Assert.IsTrue(superClasses.Any(cls => cls.Equals(new RDFResource("ex:class0S"))));
            Assert.IsTrue(superClasses.Any(cls => cls.Equals(new RDFResource("ex:class0E")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistEquivalentClasses()
        {
            List<RDFResource> equivalentClasses = ModelLens.EquivalentClasses();

            Assert.IsNotNull(equivalentClasses);
            Assert.IsTrue(equivalentClasses.Count == 2);
            Assert.IsTrue(equivalentClasses.Any(cls => cls.Equals(new RDFResource("ex:class3"))));
            Assert.IsTrue(equivalentClasses.Any(cls => cls.Equals(new RDFResource("ex:class6")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistEquivalentClassesAsync()
        {
            List<RDFResource> equivalentClasses = await ModelLens.EquivalentClassesAsync();

            Assert.IsNotNull(equivalentClasses);
            Assert.IsTrue(equivalentClasses.Count == 2);
            Assert.IsTrue(equivalentClasses.Any(cls => cls.Equals(new RDFResource("ex:class3"))));
            Assert.IsTrue(equivalentClasses.Any(cls => cls.Equals(new RDFResource("ex:class6")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistDisjointClasses()
        {
            List<RDFResource> disjointClasses = ModelLens.DisjointClasses();

            Assert.IsNotNull(disjointClasses);
            Assert.IsTrue(disjointClasses.Count == 2);
            Assert.IsTrue(disjointClasses.Any(cls => cls.Equals(new RDFResource("ex:class4")))); //Inference
            Assert.IsTrue(disjointClasses.Any(cls => cls.Equals(new RDFResource("ex:class5")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistDisjointClassesAsync()
        {
            List<RDFResource> disjointClasses = await ModelLens.DisjointClassesAsync();

            Assert.IsNotNull(disjointClasses);
            Assert.IsTrue(disjointClasses.Count == 2);
            Assert.IsTrue(disjointClasses.Any(cls => cls.Equals(new RDFResource("ex:class4")))); //Inference
            Assert.IsTrue(disjointClasses.Any(cls => cls.Equals(new RDFResource("ex:class5")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistKeyProperties()
        {
            List<RDFResource> keyProperties = ModelLens.KeyProperties();

            Assert.IsNotNull(keyProperties);
            Assert.IsTrue(keyProperties.Count == 1);
            Assert.IsTrue(keyProperties.Any(cls => cls.Equals(new RDFResource("ex:objProp"))));
        }

        [TestMethod]
        public async Task ShouldEnlistKeyPropertiesAsync()
        {
            List<RDFResource> keyProperties = await ModelLens.KeyPropertiesAsync();

            Assert.IsNotNull(keyProperties);
            Assert.IsTrue(keyProperties.Count == 1);
            Assert.IsTrue(keyProperties.Any(cls => cls.Equals(new RDFResource("ex:objProp"))));
        }

        [TestMethod]
        public void ShouldEnlistIndividuals()
        {
            List<RDFResource> individuals = ModelLens.Individuals();

            Assert.IsNotNull(individuals);
            Assert.IsTrue(individuals.Count == 3);
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv1"))));
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv2")))); //Inference
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv3")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistIndividualsAsync()
        {
            List<RDFResource> individuals = await ModelLens.IndividualsAsync();

            Assert.IsNotNull(individuals);
            Assert.IsTrue(individuals.Count == 3);
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv1"))));
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv2")))); //Inference
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv3")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistNegativeIndividuals()
        {
            List<RDFResource> individuals = ModelLens.NegativeIndividuals();

            Assert.IsNotNull(individuals);
            Assert.IsTrue(individuals.Count == 1);
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv4"))));
        }

        [TestMethod]
        public async Task ShouldEnlistNegativeIndividualsAsync()
        {
            List<RDFResource> individuals = await ModelLens.NegativeIndividualsAsync();

            Assert.IsNotNull(individuals);
            Assert.IsTrue(individuals.Count == 1);
            Assert.IsTrue(individuals.Any(idv => idv.Equals(new RDFResource("ex:indiv4"))));
        }

        [TestMethod]
        public void ShouldEnlistObjectAnnotations()
        {
            List<RDFTriple> objAnnotations = ModelLens.ObjectAnnotations();

            Assert.IsNotNull(objAnnotations);
            Assert.IsTrue(objAnnotations.Count == 1);
            Assert.IsTrue(objAnnotations.Any(objAnn => objAnn.Equals(new RDFTriple(new RDFResource("ex:class1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann")))));
        }

        [TestMethod]
        public async Task ShouldEnlistObjectAnnotationsAsync()
        {
            List<RDFTriple> objAnnotations = await ModelLens.ObjectAnnotationsAsync();

            Assert.IsNotNull(objAnnotations);
            Assert.IsTrue(objAnnotations.Count == 1);
            Assert.IsTrue(objAnnotations.Any(objAnn => objAnn.Equals(new RDFTriple(new RDFResource("ex:class1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann")))));
        }

        [TestMethod]
        public void ShouldEnlistDataAnnotations()
        {
            List<RDFTriple> dtAnnotations = ModelLens.DataAnnotations();

            Assert.IsNotNull(dtAnnotations);
            Assert.IsTrue(dtAnnotations.Count == 1);
            Assert.IsTrue(dtAnnotations.Any(dtAsn => dtAsn.Equals(new RDFTriple(new RDFResource("ex:class1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann")))));
        }

        [TestMethod]
        public async Task ShouldEnlistDataAnnotationsAsync()
        {
            List<RDFTriple> dtAnnotations = await ModelLens.DataAnnotationsAsync();

            Assert.IsNotNull(dtAnnotations);
            Assert.IsTrue(dtAnnotations.Count == 1);
            Assert.IsTrue(dtAnnotations.Any(dtAsn => dtAsn.Equals(new RDFTriple(new RDFResource("ex:class1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann")))));
        }
        #endregion
    }
}