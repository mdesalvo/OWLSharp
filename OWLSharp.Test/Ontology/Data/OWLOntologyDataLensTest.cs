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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyDataLensTest
    {
        #region Initialize
        private OWLOntologyDataLens DataLens { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DataLens = new OWLOntologyDataLens(new RDFResource("ex:indiv1"), new OWLOntology("ex:ont"));
            DataLens.Ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:annObjProp"));
            DataLens.Ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:annLitProp"));
            DataLens.Ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp"));
            DataLens.Ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtProp"));
            DataLens.Ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            DataLens.Ontology.Model.ClassModel.DeclareEnumerateClass(new RDFResource("ex:enumClass"), new List<RDFResource>() { new RDFResource("ex:indiv1"), new RDFResource("ex:indiv5") });
            DataLens.Ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestr"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv2"));
            DataLens.Ontology.Model.ClassModel.DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:enumClass") });
            DataLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            DataLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            DataLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            DataLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv4"));
            DataLens.Ontology.Data.DeclareIndividual(new RDFResource("ex:indiv5"));
            DataLens.Ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            DataLens.Ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));
            DataLens.Ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv3"));
            DataLens.Ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indiv2"), new RDFResource("ex:indiv4"));
            DataLens.Ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indiv3"), new RDFResource("ex:indiv5"));
            DataLens.Ontology.Data.AnnotateIndividual(new RDFResource("ex:indiv1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann"));
            DataLens.Ontology.Data.AnnotateIndividual(new RDFResource("ex:indiv1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann"));
            DataLens.Ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv2"));
            DataLens.Ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv1"));
            DataLens.Ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("lit"));
            DataLens.Ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv3"));
            DataLens.Ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv1"));
            DataLens.Ontology.Data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("neglit"));
        }
        #endregion

        #region Test
        [TestMethod]
        public void ShouldEnlistSameIndividuals()
        {
            List<RDFResource> sameIndividuals = DataLens.SameIndividuals();

            Assert.IsNotNull(sameIndividuals);
            Assert.IsTrue(sameIndividuals.Count == 2);
            Assert.IsTrue(sameIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv2"))));
            Assert.IsTrue(sameIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv4")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistSameIndividualsAsync()
        {
            List<RDFResource> sameIndividuals = await DataLens.SameIndividualsAsync();

            Assert.IsNotNull(sameIndividuals);
            Assert.IsTrue(sameIndividuals.Count == 2);
            Assert.IsTrue(sameIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv2"))));
            Assert.IsTrue(sameIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv4")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistDifferentIndividuals()
        {
            List<RDFResource> differentIndividuals = DataLens.DifferentIndividuals();

            Assert.IsNotNull(differentIndividuals);
            Assert.IsTrue(differentIndividuals.Count == 2);
            Assert.IsTrue(differentIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv3"))));
            Assert.IsTrue(differentIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv5")))); //Inference
        }

        [TestMethod]
        public async Task ShouldEnlistDifferentIndividualsAsync()
        {
            List<RDFResource> differentIndividuals = await DataLens.DifferentIndividualsAsync();

            Assert.IsNotNull(differentIndividuals);
            Assert.IsTrue(differentIndividuals.Count == 2);
            Assert.IsTrue(differentIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv3"))));
            Assert.IsTrue(differentIndividuals.Any(idv => idv.Equals(new RDFResource("ex:indiv5")))); //Inference
        }

        [TestMethod]
        public void ShouldEnlistClassTypes()
        {
            List<RDFResource> classTypes = DataLens.ClassTypes();

            Assert.IsNotNull(classTypes);
            Assert.IsTrue(classTypes.Count == 4);
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:class1"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:enumClass"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:hvRestr"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:unionClass"))));
        }

        [TestMethod]
        public void ShouldEnlistClassTypesWithSmartDiscovery()
        {
            List<RDFResource> classTypes = DataLens.ClassTypes(requireDeepDiscovery:false);

            Assert.IsNotNull(classTypes);
            Assert.IsTrue(classTypes.Count == 2);
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:class1"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
        }

        [TestMethod]
        public async Task ShouldEnlistClassTypesAsync()
        {
            List<RDFResource> classTypes = await DataLens.ClassTypesAsync();

            Assert.IsNotNull(classTypes);
            Assert.IsTrue(classTypes.Count == 4);
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:class1"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:enumClass"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:hvRestr"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:unionClass"))));
        }

        [TestMethod]
        public async Task ShouldEnlistClassTypesWithSmartDiscoveryAsync()
        {
            List<RDFResource> classTypes = await DataLens.ClassTypesAsync(requireDeepDiscovery:false);

            Assert.IsNotNull(classTypes);
            Assert.IsTrue(classTypes.Count == 2);
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(new RDFResource("ex:class1"))));
            Assert.IsTrue(classTypes.Any(cls => cls.Equals(RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
        }

        [TestMethod]
        public void ShouldEnlistObjectAssertions()
        {
            List<RDFTriple> objAssertions = DataLens.ObjectAssertions();

            Assert.IsNotNull(objAssertions);
            Assert.IsTrue(objAssertions.Count == 2);
            Assert.IsTrue(objAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv2")))));
            Assert.IsTrue(objAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv2"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv1")))));
        }

        [TestMethod]
        public async Task ShouldEnlistObjectAssertionsAsync()
        {
            List<RDFTriple> objAssertions = await DataLens.ObjectAssertionsAsync();

            Assert.IsNotNull(objAssertions);
            Assert.IsTrue(objAssertions.Count == 2);
            Assert.IsTrue(objAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv2")))));
            Assert.IsTrue(objAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv2"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv1")))));
        }

        [TestMethod]
        public void ShouldEnlistDataAssertions()
        {
            List<RDFTriple> dtAssertions = DataLens.DataAssertions();

            Assert.IsNotNull(dtAssertions);
            Assert.IsTrue(dtAssertions.Count == 1);
            Assert.IsTrue(dtAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("lit")))));
        }

        [TestMethod]
        public async Task ShouldEnlistDataAssertionsAsync()
        {
            List<RDFTriple> dtAssertions = await DataLens.DataAssertionsAsync();

            Assert.IsNotNull(dtAssertions);
            Assert.IsTrue(dtAssertions.Count == 1);
            Assert.IsTrue(dtAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("lit")))));
        }

        [TestMethod]
        public void ShouldEnlistNegativeObjectAssertions()
        {
            List<RDFTriple> negObjAssertions = DataLens.NegativeObjectAssertions();

            Assert.IsNotNull(negObjAssertions);
            Assert.IsTrue(negObjAssertions.Count == 2);
            Assert.IsTrue(negObjAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv3")))));
            Assert.IsTrue(negObjAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv3"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv1")))));
        }

        [TestMethod]
        public async Task ShouldEnlistNegativeObjectAssertionsAsync()
        {
            List<RDFTriple> negObjAssertions = await DataLens.NegativeObjectAssertionsAsync();

            Assert.IsNotNull(negObjAssertions);
            Assert.IsTrue(negObjAssertions.Count == 2);
            Assert.IsTrue(negObjAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv3")))));
            Assert.IsTrue(negObjAssertions.Any(objAsn => objAsn.Equals(new RDFTriple(new RDFResource("ex:indiv3"), new RDFResource("ex:objProp"), new RDFResource("ex:indiv1")))));
        }

        [TestMethod]
        public void ShouldEnlistNegativeDataAssertions()
        {
            List<RDFTriple> negDtAssertions = DataLens.NegativeDataAssertions();

            Assert.IsNotNull(negDtAssertions);
            Assert.IsTrue(negDtAssertions.Count == 1);
            Assert.IsTrue(negDtAssertions.Any(negDtAsn => negDtAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("neglit")))));
        }

        [TestMethod]
        public async Task ShouldEnlistNegativeDataAssertionsAsync()
        {
            List<RDFTriple> negDtAssertions = await DataLens.NegativeDataAssertionsAsync();

            Assert.IsNotNull(negDtAssertions);
            Assert.IsTrue(negDtAssertions.Count == 1);
            Assert.IsTrue(negDtAssertions.Any(negDtAsn => negDtAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("neglit")))));
        }

        [TestMethod]
        public void ShouldEnlistObjectAnnotations()
        {
            List<RDFTriple> objAnnotations = DataLens.ObjectAnnotations();

            Assert.IsNotNull(objAnnotations);
            Assert.IsTrue(objAnnotations.Count == 1);
            Assert.IsTrue(objAnnotations.Any(objAnn => objAnn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann")))));
        }

        [TestMethod]
        public async Task ShouldEnlistObjectAnnotationsAsync()
        {
            List<RDFTriple> objAnnotations = await DataLens.ObjectAnnotationsAsync();

            Assert.IsNotNull(objAnnotations);
            Assert.IsTrue(objAnnotations.Count == 1);
            Assert.IsTrue(objAnnotations.Any(objAnn => objAnn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:annObjProp"), new RDFResource("ex:ann")))));
        }

        [TestMethod]
        public void ShouldEnlistDataAnnotations()
        {
            List<RDFTriple> dtAnnotations = DataLens.DataAnnotations();

            Assert.IsNotNull(dtAnnotations);
            Assert.IsTrue(dtAnnotations.Count == 1);
            Assert.IsTrue(dtAnnotations.Any(dtAsn => dtAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann")))));
        }

        [TestMethod]
        public async Task ShouldEnlistDataAnnotationsAsync()
        {
            List<RDFTriple> dtAnnotations = await DataLens.DataAnnotationsAsync();

            Assert.IsNotNull(dtAnnotations);
            Assert.IsTrue(dtAnnotations.Count == 1);
            Assert.IsTrue(dtAnnotations.Any(dtAsn => dtAsn.Equals(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:annLitProp"), new RDFPlainLiteral("ann")))));
        }
        #endregion
    }
}