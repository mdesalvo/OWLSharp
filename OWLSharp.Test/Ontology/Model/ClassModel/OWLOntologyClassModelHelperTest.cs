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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyClassModelHelperTest
    {
        #region Declarer
        [TestMethod]
        public void ShouldEnlistClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:clsA"));
            classModel.DeclareRestriction(new RDFResource("ex:rstB"), new RDFResource("ex:objPrp"));
            List<RDFResource> enlistedClasses = classModel.EnlistClasses();

            Assert.IsNotNull(enlistedClasses);
            Assert.IsTrue(enlistedClasses.Count == 2);
            Assert.IsTrue(enlistedClasses.Any(cls => cls.Equals(new RDFResource("ex:clsA"))));
            Assert.IsTrue(enlistedClasses.Any(cls => cls.Equals(new RDFResource("ex:rstB"))));
            Assert.IsTrue(new OWLOntologyClassModel().EnlistClasses().Count == 0);
            Assert.IsTrue(default(OWLOntologyClassModel).EnlistClasses().Count == 0);
        }

        [TestMethod]
        public void ShouldCheckHasClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));

            Assert.IsTrue(classModel.CheckHasClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasNotClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));

            Assert.IsFalse(classModel.CheckHasClass(new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckHasClass(null));
            Assert.IsFalse(new OWLOntologyClassModel().CheckHasClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasDeprecatedClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"), new OWLOntologyClassBehavior() { Deprecated = true });

            Assert.IsTrue(classModel.CheckHasDeprecatedClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasNotDeprecatedClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"), new OWLOntologyClassBehavior() { Deprecated = false });
            classModel.DeclareClass(new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckHasDeprecatedClass(new RDFResource("ex:classA")));
            Assert.IsFalse(classModel.CheckHasDeprecatedClass(new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckHasDeprecatedClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"));

            Assert.IsTrue(classModel.CheckHasRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"));

            Assert.IsFalse(classModel.CheckHasRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasEnumerateClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareEnumerateClass(new RDFResource("ex:enumClass"), new List<RDFResource>() { new RDFResource("ex:onprop") });

            Assert.IsTrue(classModel.CheckHasEnumerateClass(new RDFResource("ex:enumClass")));
        }

        [TestMethod]
        public void ShouldCheckHasNotEnumerateClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareEnumerateClass(new RDFResource("ex:enumClass"), new List<RDFResource>() { new RDFResource("ex:onprop") });

            Assert.IsFalse(classModel.CheckHasEnumerateClass(new RDFResource("ex:enumClass2")));
            Assert.IsFalse(classModel.CheckHasEnumerateClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasEnumerateLiteralClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareEnumerateClass(new RDFResource("ex:enumlitClass"), new List<RDFLiteral>() { new RDFPlainLiteral("hello") });

            Assert.IsTrue(classModel.CheckHasEnumerateClass(new RDFResource("ex:enumlitClass")));
        }

        [TestMethod]
        public void ShouldCheckHasNotEnumerateLiteralClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareEnumerateClass(new RDFResource("ex:enumlitClass"), new List<RDFLiteral>() { new RDFPlainLiteral("hello") });

            Assert.IsFalse(classModel.CheckHasEnumerateClass(new RDFResource("ex:enumlitClass2")));
            Assert.IsFalse(classModel.CheckHasEnumerateClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasCompositeClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>() { new RDFResource("ex:classA") });

            Assert.IsTrue(classModel.CheckHasCompositeClass(new RDFResource("ex:unionClass")));
        }

        [TestMethod]
        public void ShouldCheckHasNotCompositeClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>() { new RDFResource("ex:classA") });

            Assert.IsFalse(classModel.CheckHasCompositeClass(new RDFResource("ex:unionClass2")));
            Assert.IsFalse(classModel.CheckHasCompositeClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasCompositeUnionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>() { new RDFResource("ex:classA") });

            Assert.IsTrue(classModel.CheckHasCompositeUnionClass(new RDFResource("ex:unionClass")));
        }

        [TestMethod]
        public void ShouldCheckHasNotCompositeUnionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>() { new RDFResource("ex:classA") });

            Assert.IsFalse(classModel.CheckHasCompositeUnionClass(new RDFResource("ex:unionClass2")));
            Assert.IsFalse(classModel.CheckHasCompositeUnionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasCompositeIntersectionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareIntersectionClass(new RDFResource("ex:intersectionClass"), new List<RDFResource>() { new RDFResource("ex:classA") });

            Assert.IsTrue(classModel.CheckHasCompositeIntersectionClass(new RDFResource("ex:intersectionClass")));
        }

        [TestMethod]
        public void ShouldCheckHasNotCompositeIntersectionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareIntersectionClass(new RDFResource("ex:intersectionClass"), new List<RDFResource>() { new RDFResource("ex:classA") });

            Assert.IsFalse(classModel.CheckHasCompositeIntersectionClass(new RDFResource("ex:intersectionClass2")));
            Assert.IsFalse(classModel.CheckHasCompositeIntersectionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasCompositeComplementClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareComplementClass(new RDFResource("ex:complementClass"), new RDFResource("ex:classA"));

            Assert.IsTrue(classModel.CheckHasCompositeComplementClass(new RDFResource("ex:complementClass")));
        }

        [TestMethod]
        public void ShouldCheckHasNotCompositeComplementClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareComplementClass(new RDFResource("ex:complementClass"), new RDFResource("ex:classA"));

            Assert.IsFalse(classModel.CheckHasCompositeComplementClass(new RDFResource("ex:complementClass2")));
            Assert.IsFalse(classModel.CheckHasCompositeComplementClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasAllValuesFromRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareAllValuesFromRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), new RDFResource("ex:onclass"));

            Assert.IsTrue(classModel.CheckHasAllValuesFromRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAllValuesFromRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareAllValuesFromRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasAllValuesFromRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasAllValuesFromRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasSomeValuesFromRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareSomeValuesFromRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), new RDFResource("ex:onclass"));

            Assert.IsTrue(classModel.CheckHasSomeValuesFromRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSomeValuesFromRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareSomeValuesFromRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasSomeValuesFromRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasSomeValuesFromRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasValueRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasValueRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), new RDFResource("ex:indiv1"));

            Assert.IsTrue(classModel.CheckHasValueRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotValueRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasValueRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), new RDFResource("ex:indiv1"));

            Assert.IsFalse(classModel.CheckHasValueRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasValueRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasSelfRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasSelfRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), true);

            Assert.IsTrue(classModel.CheckHasSelfRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSelfRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasSelfRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), true);

            Assert.IsFalse(classModel.CheckHasSelfRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasSelfRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsTrue(classModel.CheckHasCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsFalse(classModel.CheckHasCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasMinCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsTrue(classModel.CheckHasMinCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsFalse(classModel.CheckHasMinCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasMinCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinCardinalityRestrictionClassBecauseAlsoMax()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2);

            Assert.IsFalse(classModel.CheckHasMinCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasMaxCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsTrue(classModel.CheckHasMaxCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMaxCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsFalse(classModel.CheckHasMaxCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasMaxCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasNotMaxCardinalityRestrictionClassBecauseAlsoMin()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2);

            Assert.IsFalse(classModel.CheckHasMaxCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasMinMaxCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2);

            Assert.IsTrue(classModel.CheckHasMinMaxCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinMaxCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2);

            Assert.IsFalse(classModel.CheckHasMinMaxCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasMinMaxCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinMaxCardinalityRestrictionClassBecauseNotMax()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsFalse(classModel.CheckHasMinMaxCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinMaxCardinalityRestrictionClassBecauseNotMin()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1);

            Assert.IsFalse(classModel.CheckHasMinMaxCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsTrue(classModel.CheckHasQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasQualifiedCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasMinQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsTrue(classModel.CheckHasMinQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasMinQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasMinQualifiedCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinQualifiedCardinalityRestrictionClassBecauseAlsoMax()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasMinQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasMaxQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsTrue(classModel.CheckHasMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMaxQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasMaxQualifiedCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasNotMaxQualifiedCardinalityRestrictionClassBecauseAlsoMin()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasMinMaxQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2, new RDFResource("ex:onclass"));

            Assert.IsTrue(classModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinMaxQualifiedCardinalityRestrictionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, 2, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr2")));
            Assert.IsFalse(classModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinMaxQualifiedCardinalityRestrictionClassBecauseNotMax()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasNotMinMaxQualifiedCardinalityRestrictionClassBecauseNotMin()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onprop"), 1, new RDFResource("ex:onclass"));

            Assert.IsFalse(classModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:restr")));
        }

        [TestMethod]
        public void ShouldCheckHasDisjointUnionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classA"), new RDFResource("ex:classB") });

            Assert.IsTrue(classModel.CheckHasDisjointUnionClass(new RDFResource("ex:disjointUnionClass")));
        }

        [TestMethod]
        public void ShouldCheckHasNotDisjointUnionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classA"), new RDFResource("ex:classB") });

            Assert.IsFalse(classModel.CheckHasDisjointUnionClass(new RDFResource("ex:disjointUnionClass2")));
            Assert.IsFalse(classModel.CheckHasDisjointUnionClass(null));
        }

        [TestMethod]
        public void ShouldCheckHasAllDisjointClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classA"), new RDFResource("ex:classB") });

            Assert.IsTrue(classModel.CheckHasAllDisjointClasses(new RDFResource("ex:allDisjointClasses")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAllDisjointClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classA"), new RDFResource("ex:classB") });

            Assert.IsFalse(classModel.CheckHasAllDisjointClasses(new RDFResource("ex:allDisjointClasses2")));
            Assert.IsFalse(classModel.CheckHasAllDisjointClasses(null));
        }

        [TestMethod]
        public void ShouldCheckHasSimpleClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));

            Assert.IsTrue(classModel.CheckHasSimpleClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSimpleClassBecauseRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareRestriction(new RDFResource("ex:classA"), new RDFResource("ex:onprop"));

            Assert.IsFalse(classModel.CheckHasSimpleClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSimpleClassBecauseEnumerate()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareEnumerateClass(new RDFResource("ex:classA"), new List<RDFResource>() { new RDFResource("ex:indiv1") });

            Assert.IsFalse(classModel.CheckHasSimpleClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSimpleClassBecauseComposite()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareIntersectionClass(new RDFResource("ex:classA"), new List<RDFResource>() { new RDFResource("ex:class1") });

            Assert.IsFalse(classModel.CheckHasSimpleClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSimpleClassBecauseDisjointUnion()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareDisjointUnionClass(new RDFResource("ex:classA"), new List<RDFResource>() { new RDFResource("ex:class1") });

            Assert.IsFalse(classModel.CheckHasSimpleClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasNotSimpleClassBecauseAllDisjointClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareAllDisjointClasses(new RDFResource("ex:classA"), new List<RDFResource>() { new RDFResource("ex:class1") });

            Assert.IsFalse(classModel.CheckHasSimpleClass(new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckHasAnnotationLiteral()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));

            Assert.IsTrue(classModel.CheckHasAnnotation(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
        }

        [TestMethod]
        public void ShouldCheckHasAnnotationResource()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"));

            Assert.IsTrue(classModel.CheckHasAnnotation(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAnnotationLiteral()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));

            Assert.IsFalse(classModel.CheckHasAnnotation(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment", "en")));
        }

        [TestMethod]
        public void ShouldCheckHasNotAnnotationResource()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"));

            Assert.IsFalse(classModel.CheckHasAnnotation(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso2")));
        }
        #endregion

        #region Analyzer
        [TestMethod]
        public void ShouldCheckAreSubClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareSubClasses(new RDFResource("ex:classC"), new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));

            Assert.IsTrue(classModel.CheckIsSubClassOf(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
            Assert.IsTrue(classModel.CheckIsSubClassOf(new RDFResource("ex:classC"), new RDFResource("ex:classB")));
            Assert.IsTrue(classModel.CheckIsSubClassOf(new RDFResource("ex:classC"), new RDFResource("ex:classA"))); //Inferred
            Assert.IsTrue(classModel.CheckIsSubClassOf(new RDFResource("ex:classD"), new RDFResource("ex:classB"))); //Inferred
            Assert.IsTrue(classModel.CheckIsSubClassOf(new RDFResource("ex:classD"), new RDFResource("ex:classA"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerSubClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareSubClasses(new RDFResource("ex:classC"), new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));

            Assert.IsTrue(classModel.GetSubClassesOf(new RDFResource("ex:classA")).Any(sc => sc.Equals(new RDFResource("ex:classB"))));
            Assert.IsTrue(classModel.GetSubClassesOf(new RDFResource("ex:classA")).Any(sc => sc.Equals(new RDFResource("ex:classC")))); //Inferred
            Assert.IsTrue(classModel.GetSubClassesOf(new RDFResource("ex:classA")).Any(sc => sc.Equals(new RDFResource("ex:classD")))); //Inferred
            Assert.IsTrue(classModel.GetSubClassesOf(new RDFResource("ex:classB")).Any(sc => sc.Equals(new RDFResource("ex:classC"))));
            Assert.IsTrue(classModel.GetSubClassesOf(new RDFResource("ex:classB")).Any(sc => sc.Equals(new RDFResource("ex:classD")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreSuperClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareSubClasses(new RDFResource("ex:classC"), new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));

            Assert.IsTrue(classModel.CheckIsSuperClassOf(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsTrue(classModel.CheckIsSuperClassOf(new RDFResource("ex:classB"), new RDFResource("ex:classC")));
            Assert.IsTrue(classModel.CheckIsSuperClassOf(new RDFResource("ex:classA"), new RDFResource("ex:classC"))); //Inferred
            Assert.IsTrue(classModel.CheckIsSuperClassOf(new RDFResource("ex:classB"), new RDFResource("ex:classD"))); //Inferred
            Assert.IsTrue(classModel.CheckIsSuperClassOf(new RDFResource("ex:classA"), new RDFResource("ex:classD"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerSuperClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareSubClasses(new RDFResource("ex:classC"), new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));

            Assert.IsTrue(classModel.GetSuperClassesOf(new RDFResource("ex:classB")).Any(sc => sc.Equals(new RDFResource("ex:classA"))));
            Assert.IsTrue(classModel.GetSuperClassesOf(new RDFResource("ex:classC")).Any(sc => sc.Equals(new RDFResource("ex:classB"))));
            Assert.IsTrue(classModel.GetSuperClassesOf(new RDFResource("ex:classC")).Any(sc => sc.Equals(new RDFResource("ex:classA")))); //Inferred
            Assert.IsTrue(classModel.GetSuperClassesOf(new RDFResource("ex:classD")).Any(sc => sc.Equals(new RDFResource("ex:classB")))); //Inferred
            Assert.IsTrue(classModel.GetSuperClassesOf(new RDFResource("ex:classD")).Any(sc => sc.Equals(new RDFResource("ex:classA")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreEquivalentClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));

            Assert.IsTrue(classModel.CheckIsEquivalentClassOf(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsTrue(classModel.CheckIsEquivalentClassOf(new RDFResource("ex:classA"), new RDFResource("ex:classC"))); //Inferred
            Assert.IsTrue(classModel.CheckIsEquivalentClassOf(new RDFResource("ex:classB"), new RDFResource("ex:classA"))); //Inferred            
            Assert.IsTrue(classModel.CheckIsEquivalentClassOf(new RDFResource("ex:classB"), new RDFResource("ex:classC")));
            Assert.IsTrue(classModel.CheckIsEquivalentClassOf(new RDFResource("ex:classC"), new RDFResource("ex:classB"))); //Inferred
            Assert.IsTrue(classModel.CheckIsEquivalentClassOf(new RDFResource("ex:classC"), new RDFResource("ex:classA"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerEquivalentClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));

            Assert.IsTrue(classModel.GetEquivalentClassesOf(new RDFResource("ex:classA")).Any(sc => sc.Equals(new RDFResource("ex:classB"))));
            Assert.IsTrue(classModel.GetEquivalentClassesOf(new RDFResource("ex:classA")).Any(sc => sc.Equals(new RDFResource("ex:classC")))); //Inferred
            Assert.IsTrue(classModel.GetEquivalentClassesOf(new RDFResource("ex:classB")).Any(sc => sc.Equals(new RDFResource("ex:classA")))); //Inferred
            Assert.IsTrue(classModel.GetEquivalentClassesOf(new RDFResource("ex:classB")).Any(sc => sc.Equals(new RDFResource("ex:classC"))));
            Assert.IsTrue(classModel.GetEquivalentClassesOf(new RDFResource("ex:classC")).Any(sc => sc.Equals(new RDFResource("ex:classA")))); //Inferred
            Assert.IsTrue(classModel.GetEquivalentClassesOf(new RDFResource("ex:classC")).Any(sc => sc.Equals(new RDFResource("ex:classB")))); //Inferred
        }

        [TestMethod]
        public void ShouldCheckAreDisjointClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));
            classModel.DeclareSubClasses(new RDFResource("ex:classD"), new RDFResource("ex:classC"));

            Assert.IsTrue(classModel.CheckIsDisjointClassWith(new RDFResource("ex:classA"), new RDFResource("ex:classC"))); //Inferred
            Assert.IsTrue(classModel.CheckIsDisjointClassWith(new RDFResource("ex:classB"), new RDFResource("ex:classC")));
            Assert.IsTrue(classModel.CheckIsDisjointClassWith(new RDFResource("ex:classB"), new RDFResource("ex:classD"))); //Inferred
            Assert.IsTrue(classModel.CheckIsDisjointClassWith(new RDFResource("ex:classA"), new RDFResource("ex:classD"))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerDisjointClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));
            classModel.DeclareSubClasses(new RDFResource("ex:classD"), new RDFResource("ex:classC"));

            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classA")).Any(sc => sc.Equals(new RDFResource("ex:classC")))); //Inferred
            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classA")).Any(sc => sc.Equals(new RDFResource("ex:classD")))); //Inferred
            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classB")).Any(sc => sc.Equals(new RDFResource("ex:classC"))));
            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classB")).Any(sc => sc.Equals(new RDFResource("ex:classD")))); //Inferred
            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classC")).Any(sc => sc.Equals(new RDFResource("ex:classA")))); //Inferred
            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classC")).Any(sc => sc.Equals(new RDFResource("ex:classB")))); //Inferred
            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classD")).Any(sc => sc.Equals(new RDFResource("ex:classA")))); //Inferred
            Assert.IsTrue(classModel.GetDisjointClassesWith(new RDFResource("ex:classD")).Any(sc => sc.Equals(new RDFResource("ex:classB")))); //Inferred
        }

        [TestMethod]
        public void ShouldAnswerKeyProperties()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { new RDFResource("ex:objprop1"), new RDFResource("ex:objprop2") });

            Assert.IsTrue(classModel.GetKeyPropertiesOf(new RDFResource("ex:classA")).Any(p => p.Equals(new RDFResource("ex:objprop1"))));
            Assert.IsTrue(classModel.GetKeyPropertiesOf(new RDFResource("ex:classA")).Any(p => p.Equals(new RDFResource("ex:objprop2"))));
            Assert.IsTrue(classModel.GetKeyPropertiesOf(new RDFResource("ex:classB")).Count == 0);
        }
        #endregion

        #region Checker
        [TestMethod]
        public void ShouldCheckIsReservedClass()
            => Assert.IsTrue(OWLOntologyClassModelHelper.CheckReservedClass(RDFVocabulary.RDFS.CLASS));

        [TestMethod]
        public void ShouldCheckSubClassCompatibility()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));

            Assert.IsTrue(classModel.CheckSubClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsTrue(classModel.CheckSubClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckSubClassIncompatibilityBecauseSubClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckSubClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckSubClassIncompatibilityBecauseEquivalentClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckSubClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckSubClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckSubClassIncompatibilityBecauseDisjointClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckSubClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckSubClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentClassCompatibility()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));

            Assert.IsTrue(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsTrue(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentClassIncompatibilityBecauseSubClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentClassIncompatibilityBecauseEquivalentClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));

            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentClassIncompatibilityBecauseDisjointClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckEquivalentClassIncompatibilityBecauseAllDisjointClassesViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"),
                new List<RDFResource>() { new RDFResource("ex:classA"), new RDFResource("ex:classB") });

            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckEquivalentClassCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointWithCompatibility()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));

            Assert.IsTrue(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsTrue(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointWithIncompatibilityBecauseSubClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointWithIncompatibilityBecauseSuperClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));

            Assert.IsFalse(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }

        [TestMethod]
        public void ShouldCheckDisjointWithIncompatibilityBecauseEquivalentClassViolation()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsFalse(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classA"), new RDFResource("ex:classB")));
            Assert.IsFalse(classModel.CheckDisjointWithCompatibility(new RDFResource("ex:classB"), new RDFResource("ex:classA")));
        }
        #endregion
    }
}