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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyPropertyModelHelperTest
    {
        #region Declarer
        [TestMethod]
        public void ShouldEnlistAnnotationProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objPrp"));
            propertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtPrp"));
            propertyModel.DeclareAnnotationProperty(new RDFResource("ex:annPrp"));
            List<RDFResource> enlistedProperties = propertyModel.EnlistProperties(OWLEnums.OWLPropertyType.Annotation);

            Assert.IsNotNull(enlistedProperties);
            Assert.IsTrue(enlistedProperties.Count == 1);
            Assert.IsTrue(enlistedProperties.Any(prp => prp.Equals(new RDFResource("ex:annPrp"))));
            Assert.IsTrue(new OWLOntologyPropertyModel().EnlistProperties(OWLEnums.OWLPropertyType.Annotation).Count == 0);
            Assert.IsTrue(default(OWLOntologyPropertyModel).EnlistProperties(OWLEnums.OWLPropertyType.Annotation).Count == 0);
        }

        [TestMethod]
        public void ShouldEnlistDatatypeProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objPrp"));
            propertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtPrp"));
            propertyModel.DeclareAnnotationProperty(new RDFResource("ex:annPrp"));
            List<RDFResource> enlistedProperties = propertyModel.EnlistProperties(OWLEnums.OWLPropertyType.Datatype);

            Assert.IsNotNull(enlistedProperties);
            Assert.IsTrue(enlistedProperties.Count == 1);
            Assert.IsTrue(enlistedProperties.Any(prp => prp.Equals(new RDFResource("ex:dtPrp"))));
            Assert.IsTrue(new OWLOntologyPropertyModel().EnlistProperties(OWLEnums.OWLPropertyType.Datatype).Count == 0);
            Assert.IsTrue(default(OWLOntologyPropertyModel).EnlistProperties(OWLEnums.OWLPropertyType.Datatype).Count == 0);
        }

        [TestMethod]
        public void ShouldEnlistObjectProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objPrp"));
            propertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtPrp"));
            propertyModel.DeclareAnnotationProperty(new RDFResource("ex:annPrp"));
            List<RDFResource> enlistedProperties = propertyModel.EnlistProperties(OWLEnums.OWLPropertyType.Object);

            Assert.IsNotNull(enlistedProperties);
            Assert.IsTrue(enlistedProperties.Count == 1);
            Assert.IsTrue(enlistedProperties.Any(prp => prp.Equals(new RDFResource("ex:objPrp"))));
            Assert.IsTrue(new OWLOntologyPropertyModel().EnlistProperties(OWLEnums.OWLPropertyType.Object).Count == 0);
            Assert.IsTrue(default(OWLOntologyPropertyModel).EnlistProperties(OWLEnums.OWLPropertyType.Object).Count == 0);
        }

        [TestMethod]
        public void ShouldCheckHasAnnotationProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareAnnotationProperty(new RDFResource("ex:annprop"));

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:annprop")));
            Assert.IsTrue(propertyModel.CheckHasAnnotationProperty(new RDFResource("ex:annprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAnnotationProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));

            Assert.IsFalse(propertyModel.CheckHasProperty(new RDFResource("ex:dtprop2")));
            Assert.IsFalse(propertyModel.CheckHasProperty(null));
            Assert.IsFalse(propertyModel.CheckHasAnnotationProperty(new RDFResource("ex:dtprop")));
            Assert.IsFalse(new OWLOntologyPropertyModel().CheckHasProperty(new RDFResource("ex:dtprop")));
        }

        [TestMethod]
        public void ShouldCheckHasDatatypeProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:dtprop")));
            Assert.IsTrue(propertyModel.CheckHasDatatypeProperty(new RDFResource("ex:dtprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotDatatypeProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));

            Assert.IsFalse(propertyModel.CheckHasProperty(new RDFResource("ex:objprop2")));
            Assert.IsFalse(propertyModel.CheckHasProperty(null));
            Assert.IsFalse(propertyModel.CheckHasDatatypeProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(new OWLOntologyPropertyModel().CheckHasProperty(new RDFResource("ex:objprop")));
        }

        [TestMethod]
        public void ShouldCheckHasObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:objprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));

            Assert.IsFalse(propertyModel.CheckHasProperty(new RDFResource("ex:dtprop2")));
            Assert.IsFalse(propertyModel.CheckHasProperty(null));
            Assert.IsFalse(propertyModel.CheckHasObjectProperty(new RDFResource("ex:dtprop")));
            Assert.IsFalse(new OWLOntologyPropertyModel().CheckHasProperty(new RDFResource("ex:dtprop")));
        }

        [TestMethod]
        public void ShouldCheckHasSymmetricObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:symobjprop"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:symobjprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:symobjprop")));
            Assert.IsTrue(propertyModel.CheckHasSymmetricProperty(new RDFResource("ex:symobjprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSymmetricObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Symmetric = false });

            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasSymmetricProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasSymmetricProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasAsymmetricObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:asymobjprop"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:asymobjprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:asymobjprop")));
            Assert.IsTrue(propertyModel.CheckHasAsymmetricProperty(new RDFResource("ex:asymobjprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAsymmetricObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = false });

            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasAsymmetricProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasAsymmetricProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasTransitiveObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:trobjprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:trobjprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:trobjprop")));
            Assert.IsTrue(propertyModel.CheckHasTransitiveProperty(new RDFResource("ex:trobjprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotTransitiveObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = false });

            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasTransitiveProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasTransitiveProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasFunctionalObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:fobjprop"), new OWLOntologyObjectPropertyBehavior() { Functional = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:fobjprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:fobjprop")));
            Assert.IsTrue(propertyModel.CheckHasFunctionalProperty(new RDFResource("ex:fobjprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotFunctionalObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Functional = false });

            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasFunctionalProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasFunctionalProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasInverseFunctionalObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:ifobjprop"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:ifobjprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:ifobjprop")));
            Assert.IsTrue(propertyModel.CheckHasInverseFunctionalProperty(new RDFResource("ex:ifobjprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotInverseFunctionalObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = false });

            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasInverseFunctionalProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasInverseFunctionalProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasReflexiveObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:robjprop"), new OWLOntologyObjectPropertyBehavior() { Reflexive = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:robjprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:robjprop")));
            Assert.IsTrue(propertyModel.CheckHasReflexiveProperty(new RDFResource("ex:robjprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotReflexiveObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Reflexive = false });

            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasReflexiveProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasReflexiveProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasIrreflexiveObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:irobjprop"), new OWLOntologyObjectPropertyBehavior() { Irreflexive = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:irobjprop")));
            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:irobjprop")));
            Assert.IsTrue(propertyModel.CheckHasIrreflexiveProperty(new RDFResource("ex:irobjprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotIrreflexiveObjectProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Irreflexive = false });

            Assert.IsTrue(propertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasIrreflexiveProperty(new RDFResource("ex:objprop")));
            Assert.IsFalse(propertyModel.CheckHasIrreflexiveProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasDeprecatedProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareAnnotationProperty(new RDFResource("ex:annprop"), new OWLOntologyAnnotationPropertyBehavior() { Deprecated = true });

            Assert.IsTrue(propertyModel.CheckHasProperty(new RDFResource("ex:annprop")));
            Assert.IsTrue(propertyModel.CheckHasAnnotationProperty(new RDFResource("ex:annprop")));
            Assert.IsTrue(propertyModel.CheckHasDeprecatedProperty(new RDFResource("ex:annprop")));
        }

        [TestMethod]
        public void ShouldCheckHasNotDeprecatedProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareAnnotationProperty(new RDFResource("ex:annprop"), new OWLOntologyAnnotationPropertyBehavior() { Deprecated = false });

            Assert.IsTrue(propertyModel.CheckHasAnnotationProperty(new RDFResource("ex:annprop")));
            Assert.IsFalse(propertyModel.CheckHasDeprecatedProperty(new RDFResource("ex:annprop")));
            Assert.IsFalse(propertyModel.CheckHasDeprecatedProperty(null));
        }

        [TestMethod]
        public void ShouldCheckHasAnnotationLiteral()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));

            Assert.IsTrue(propertyModel.CheckHasAnnotation(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
        }

        [TestMethod]
        public void ShouldCheckHasAnnotationResource()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"));

            Assert.IsTrue(propertyModel.CheckHasAnnotation(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAnnotationLiteral()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));

            Assert.IsFalse(propertyModel.CheckHasAnnotation(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment", "en")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAnnotationResource()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.AnnotateProperty(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"));

            Assert.IsFalse(propertyModel.CheckHasAnnotation(new RDFResource("ex:propertyA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso2")));
        }

        [TestMethod]
        public void ShouldCheckHasPropertyChainAxiom()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:propertyChainAxiom"), [new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")]);

            Assert.IsTrue(propertyModel.CheckHasPropertyChainAxiom(new RDFResource("ex:propertyChainAxiom")));
            Assert.IsFalse(propertyModel.CheckHasPropertyChainAxiom(null));
        }
        #endregion

        #region Analyzer
        [TestMethod]
        public void ShouldCheckAreSubProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));

            Assert.IsTrue(propertyModel.CheckIsSubPropertyOf(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
            Assert.IsTrue(propertyModel.CheckIsSubPropertyOf(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckIsSubPropertyOf(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyA"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsSubPropertyOf(new RDFResource("ex:propertyD"), new RDFResource("ex:propertyB"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsSubPropertyOf(new RDFResource("ex:propertyD"), new RDFResource("ex:propertyA"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerSubProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));

            Assert.IsTrue(propertyModel.GetSubPropertiesOf(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyB"))));
            Assert.IsTrue(propertyModel.GetSubPropertiesOf(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyC")))); //Inferred
            Assert.IsTrue(propertyModel.GetSubPropertiesOf(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyD")))); //Inferred
            Assert.IsTrue(propertyModel.GetSubPropertiesOf(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyC"))));
            Assert.IsTrue(propertyModel.GetSubPropertiesOf(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyD")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreSuperProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));

            Assert.IsTrue(propertyModel.CheckIsSuperPropertyOf(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckIsSuperPropertyOf(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyC")));
            Assert.IsTrue(propertyModel.CheckIsSuperPropertyOf(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyC"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsSuperPropertyOf(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyD"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsSuperPropertyOf(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyD"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerSuperProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyD"));

            Assert.IsTrue(propertyModel.GetSuperPropertiesOf(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyA"))));
            Assert.IsTrue(propertyModel.GetSuperPropertiesOf(new RDFResource("ex:propertyC")).Any(sp => sp.Equals(new RDFResource("ex:propertyB"))));
            Assert.IsTrue(propertyModel.GetSuperPropertiesOf(new RDFResource("ex:propertyC")).Any(sp => sp.Equals(new RDFResource("ex:propertyA")))); //Inferred
            Assert.IsTrue(propertyModel.GetSuperPropertiesOf(new RDFResource("ex:propertyD")).Any(sp => sp.Equals(new RDFResource("ex:propertyB")))); //Inferred
            Assert.IsTrue(propertyModel.GetSuperPropertiesOf(new RDFResource("ex:propertyD")).Any(sp => sp.Equals(new RDFResource("ex:propertyA")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreEquivalentProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyC"));

            Assert.IsTrue(propertyModel.CheckIsEquivalentPropertyOf(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckIsEquivalentPropertyOf(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyC"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsEquivalentPropertyOf(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"))); //Inferred            
            Assert.IsTrue(propertyModel.CheckIsEquivalentPropertyOf(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyC")));
            Assert.IsTrue(propertyModel.CheckIsEquivalentPropertyOf(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyB"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsEquivalentPropertyOf(new RDFResource("ex:propertyC"), new RDFResource("ex:propertyA"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerEquivalentProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyC"));

            Assert.IsTrue(propertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyB"))));
            Assert.IsTrue(propertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyC")))); //Inferred
            Assert.IsTrue(propertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyA")))); //Inferred
            Assert.IsTrue(propertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyC"))));
            Assert.IsTrue(propertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:propertyC")).Any(sp => sp.Equals(new RDFResource("ex:propertyA")))); //Inferred
            Assert.IsTrue(propertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:propertyC")).Any(sp => sp.Equals(new RDFResource("ex:propertyB")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreDisjointProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyC"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyD"), new RDFResource("ex:propertyC"));

            Assert.IsTrue(propertyModel.CheckIsDisjointPropertyWith(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyC"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsDisjointPropertyWith(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyC")));
            Assert.IsTrue(propertyModel.CheckIsDisjointPropertyWith(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyD"))); //Inferred
            Assert.IsTrue(propertyModel.CheckIsDisjointPropertyWith(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyD"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerDisjointProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyC"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyD"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));
            propertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyC"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyD"), new RDFResource("ex:propertyC"));

            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyC")))); //Inferred
            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyD")))); //Inferred
            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyC"))));
            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyD")))); //Inferred
            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyC")).Any(sp => sp.Equals(new RDFResource("ex:propertyA")))); //Inferred
            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyC")).Any(sp => sp.Equals(new RDFResource("ex:propertyB")))); //Inferred
            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyD")).Any(sp => sp.Equals(new RDFResource("ex:propertyA")))); //Inferred
            Assert.IsTrue(propertyModel.GetDisjointPropertiesWith(new RDFResource("ex:propertyD")).Any(sp => sp.Equals(new RDFResource("ex:propertyB")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreInverseProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareInverseProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsTrue(propertyModel.CheckIsInversePropertyOf(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckIsInversePropertyOf(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerInverseProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareInverseProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsTrue(propertyModel.GetInversePropertiesOf(new RDFResource("ex:propertyA")).Any(sp => sp.Equals(new RDFResource("ex:propertyB"))));
            Assert.IsTrue(propertyModel.GetInversePropertiesOf(new RDFResource("ex:propertyB")).Any(sp => sp.Equals(new RDFResource("ex:propertyA")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreInversePropertiesWithInlineBlankSubProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:DescendantOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:ParentOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:ParentOf"), new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("bnode:inlineOP"), new RDFResource("ex:DescendantOf"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iJunior"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iMajor"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iMajor"), new RDFResource("ex:ParentOf"), new RDFResource("ex:iJunior"));

            Assert.IsTrue(ontology.Model.PropertyModel.CheckIsInversePropertyOf(new RDFResource("ex:ParentOf"), new RDFResource("ex:DescendantOf"))); //Inferred
            //Cannot "reverse" inline blank properties during computation of owl:inverseOf relations:
            //(so we'll be able to assert this only after having materialized the above inference)
            Assert.IsFalse(ontology.Model.PropertyModel.CheckIsInversePropertyOf(new RDFResource("ex:DescendantOf"), new RDFResource("ex:ParentOf")));
        }

        [TestMethod]
        public void ShouldCheckAreInversePropertiesWithInlineBlankEquivalentProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:DescendantOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:ParentOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:ParentOf"), new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("bnode:inlineOP"), new RDFResource("ex:DescendantOf"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iJunior"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iMajor"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iMajor"), new RDFResource("ex:ParentOf"), new RDFResource("ex:iJunior"));

            Assert.IsTrue(ontology.Model.PropertyModel.CheckIsInversePropertyOf(new RDFResource("ex:ParentOf"), new RDFResource("ex:DescendantOf"))); //Inferred
            //Cannot "reverse" inline blank properties during computation of owl:inverseOf relations:
            //(so we'll be able to assert this only after having materialized the above inference)
            Assert.IsFalse(ontology.Model.PropertyModel.CheckIsInversePropertyOf(new RDFResource("ex:DescendantOf"), new RDFResource("ex:ParentOf")));
        }

        [TestMethod]
        public void ShouldAnswerDomainOfProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:class") });

            Assert.IsTrue(propertyModel.GetDomainOf(new RDFResource("ex:objprop")).Any(cls => cls.Equals(new RDFResource("ex:class"))));
            Assert.IsTrue(propertyModel.GetDomainOf(new RDFResource("ex:objprop2")).Count == 0);
            Assert.IsTrue(propertyModel.GetDomainOf(null).Count == 0);
        }

        [TestMethod]
        public void ShouldAnswerRangeOfProperty()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Range = new RDFResource("ex:class") });

            Assert.IsTrue(propertyModel.GetRangeOf(new RDFResource("ex:objprop")).Any(cls => cls.Equals(new RDFResource("ex:class"))));
            Assert.IsTrue(propertyModel.GetRangeOf(new RDFResource("ex:objprop2")).Count == 0);
            Assert.IsTrue(propertyModel.GetRangeOf(null).Count == 0);
        }

        [TestMethod]
        public void ShouldAnswerChainAxiomProperties()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:propertyChainAxiom"), [new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")]);

            Assert.IsTrue(propertyModel.GetChainAxiomPropertiesOf(new RDFResource("ex:propertyChainAxiom")).Any(sp => sp.Equals(new RDFResource("ex:propertyA"))));
            Assert.IsTrue(propertyModel.GetChainAxiomPropertiesOf(new RDFResource("ex:propertyChainAxiom")).Any(sp => sp.Equals(new RDFResource("ex:propertyB"))));
        }
        #endregion

        #region Checker
        [TestMethod]
        public void ShouldCheckIsReservedProperty()
            => Assert.IsTrue(OWLOntologyPropertyModelHelper.CheckReservedProperty(RDFVocabulary.RDFS.DOMAIN));

        [TestMethod]
        public void ShouldCheckSubPropertyCompatibility()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));

            Assert.IsTrue(propertyModel.CheckSubPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckSubPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckSubPropertyIncompatibilityBecauseSubPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckSubPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckSubPropertyIncompatibilityBecauseEquivalentPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckSubPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckSubPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckSubPropertyIncompatibilityBecauseDisjointPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckSubPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckSubPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentPropertyCompatibility()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));

            Assert.IsTrue(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentPropertyIncompatibilityBecauseSubPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentPropertyIncompatibilityBecauseEquivalentPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));

            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentPropertyIncompatibilityBecauseDisjointPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareDisjointProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentPropertyIncompatibilityBecauseAllDisjointPropertyesViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareAllDisjointProperties(new RDFResource("ex:allDisjointProperties"),
                [new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")]);

            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckEquivalentPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointPropertyCompatibility()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));

            Assert.IsTrue(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointPropertyIncompatibilityBecauseSubPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointPropertyIncompatibilityBecauseSuperPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));

            Assert.IsFalse(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointPropertyIncompatibilityBecauseEquivalentPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckDisjointPropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckInversePropertyCompatibility()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));

            Assert.IsTrue(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsTrue(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckInversePropertyIncompatibilityBecauseSubPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckInversePropertyIncompatibilityBecauseSuperPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareSubProperties(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA"));

            Assert.IsFalse(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }

        [TestMethod]
        public void ShouldCheckInversePropertyIncompatibilityBecauseEquivalentPropertyViolation()
        {
            OWLOntologyPropertyModel propertyModel = new OWLOntologyPropertyModel();
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyA"));
            propertyModel.DeclareObjectProperty(new RDFResource("ex:propertyB"));
            propertyModel.DeclareEquivalentProperties(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB"));

            Assert.IsFalse(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyA"), new RDFResource("ex:propertyB")));
            Assert.IsFalse(propertyModel.CheckInversePropertyCompatibility(new RDFResource("ex:propertyB"), new RDFResource("ex:propertyA")));
        }
        #endregion
    }
}