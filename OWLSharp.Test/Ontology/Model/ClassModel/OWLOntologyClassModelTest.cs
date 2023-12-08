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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyClassModelTest
    {
        #region Test
        [TestMethod]
        public void ShouldCreateClassModel()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();

            Assert.IsNotNull(classModel);
            Assert.IsNotNull(classModel.Classes);
            Assert.IsTrue(classModel.ClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsNotNull(classModel.TBoxGraph);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 0);
            Assert.IsNotNull(classModel.OBoxGraph);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> classesEnumerator = classModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 0);

            int j = 0;
            IEnumerator<RDFResource> allDisjointClassesEnumerator = classModel.AllDisjointClassesEnumerator;
            while (allDisjointClassesEnumerator.MoveNext())
                j++;
            Assert.IsTrue(j == 0);

            int m = 0;
            IEnumerator<RDFResource> compositeClassesEnumerator = classModel.CompositesEnumerator;
            while (compositeClassesEnumerator.MoveNext())
                m++;
            Assert.IsTrue(m == 0);

            int n = 0;
            IEnumerator<RDFResource> deprecatedClassesEnumerator = classModel.DeprecatedClassesEnumerator;
            while (deprecatedClassesEnumerator.MoveNext())
                n++;
            Assert.IsTrue(n == 0);

            int v = 0;
            IEnumerator<RDFResource> simpleClassesEnumerator = classModel.SimpleClassesEnumerator;
            while (simpleClassesEnumerator.MoveNext())
                v++;
            Assert.IsTrue(v == 0);

            int k = 0;
            IEnumerator<RDFResource> enumerateClassesEnumerator = classModel.EnumeratesEnumerator;
            while (enumerateClassesEnumerator.MoveNext())
                k++;
            Assert.IsTrue(k == 0);

            int r = 0;
            IEnumerator<RDFResource> restrictionClassesEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionClassesEnumerator.MoveNext())
                r++;
            Assert.IsTrue(r == 0);
        }

        [TestMethod]
        public void ShouldDisposeClassModelWithUsing()
        {
            OWLOntologyClassModel model;
            using (model = new OWLOntologyClassModel())
            {
                Assert.IsFalse(model.Disposed);
                Assert.IsNotNull(model.Classes);
                Assert.IsNotNull(model.TBoxGraph);
                Assert.IsNotNull(model.OBoxGraph);
            };
            Assert.IsTrue(model.Disposed);
            Assert.IsNull(model.Classes);
            Assert.IsNull(model.TBoxGraph);
            Assert.IsNull(model.OBoxGraph);
        }

        [TestMethod]
        public void ShouldDeclareClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:class1"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 1);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> classesEnumerator = classModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
            {
                Assert.IsTrue(classesEnumerator.Current.Equals(new RDFResource("ex:class1")));
                i++;
            }                
            Assert.IsTrue(i == 1);

            int v = 0;
            IEnumerator<RDFResource> simpleClassesEnumerator = classModel.SimpleClassesEnumerator;
            while (simpleClassesEnumerator.MoveNext())
            {
                Assert.IsTrue(simpleClassesEnumerator.Current.Equals(new RDFResource("ex:class1")));
                v++;
            }
            Assert.IsTrue(v == 1);
        }

        [TestMethod]
        public void ShouldDeclareDeprecatedClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:class1"), new OWLOntologyClassBehavior() { Deprecated = true });

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 1);
            Assert.IsTrue(classModel.SimpleClassesCount == 1);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_CLASS)));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> classesEnumerator = classModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
            {
                Assert.IsTrue(classesEnumerator.Current.Equals(new RDFResource("ex:class1")));
                i++;
            }
            Assert.IsTrue(i == 1);

            int j = 0;
            IEnumerator<RDFResource> deprecatedClassesEnumerator = classModel.DeprecatedClassesEnumerator;
            while (deprecatedClassesEnumerator.MoveNext())
            {
                Assert.IsTrue(deprecatedClassesEnumerator.Current.Equals(new RDFResource("ex:class1")));
                j++;
            }
            Assert.IsTrue(j == 1);

            int v = 0;
            IEnumerator<RDFResource> simpleClassesEnumerator = classModel.SimpleClassesEnumerator;
            while (simpleClassesEnumerator.MoveNext())
            {
                Assert.IsTrue(simpleClassesEnumerator.Current.Equals(new RDFResource("ex:class1")));
                v++;
            }
            Assert.IsTrue(v == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringClassBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareClass(null));

        [TestMethod]
        public void ShouldDeclareAllValuesFromRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareAllValuesFromRestriction(new RDFResource("ex:avRestr"), new RDFResource("ex:onProp"), new RDFResource("ex:avClass"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:avRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:avRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:avRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:avRestr"), RDFVocabulary.OWL.ALL_VALUES_FROM, new RDFResource("ex:avClass"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:avRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAllValuesFromRestrictionBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareAllValuesFromRestriction(new RDFResource("ex:avRestr"), new RDFResource("ex:onProp"), null));

        [TestMethod]
        public void ShouldDeclareSomeValuesFromRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareSomeValuesFromRestriction(new RDFResource("ex:svRestr"), new RDFResource("ex:onProp"), new RDFResource("ex:svClass"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:svRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:svRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:svRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:svRestr"), RDFVocabulary.OWL.SOME_VALUES_FROM, new RDFResource("ex:svClass"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:svRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSomeValuesFromRestrictionBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareSomeValuesFromRestriction(new RDFResource("ex:svClass"), new RDFResource("ex:onProp"), null));

        [TestMethod]
        public void ShouldDeclareHasSelfTrueRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasSelfRestriction(new RDFResource("ex:hsRestr"), new RDFResource("ex:onProp"), true);

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.OWL.HAS_SELF, RDFTypedLiteral.True)));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:hsRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldDeclareHasSelfFalseRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasSelfRestriction(new RDFResource("ex:hsRestr"), new RDFResource("ex:onProp"), false);

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hsRestr"), RDFVocabulary.OWL.HAS_SELF, RDFTypedLiteral.False)));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:hsRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldDeclareHasValueResourceRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestr"), new RDFResource("ex:onProp"), new RDFResource("ex:val"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.OWL.HAS_VALUE, new RDFResource("ex:val"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:hvRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHasValueRestrictionBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareHasValueRestriction(new RDFResource("ex:hvRestr"), new RDFResource("ex:onProp"), null as RDFResource));

        [TestMethod]
        public void ShouldDeclareHasValueLiteralRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestr"), new RDFResource("ex:onProp"), new RDFPlainLiteral("val"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.OWL.HAS_VALUE, new RDFPlainLiteral("val"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:hvRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHasValueLiteralRestrictionBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareHasValueRestriction(new RDFResource("ex:hvRestr"), new RDFResource("ex:onProp"), null as RDFLiteral));

        [TestMethod]
        public void ShouldDeclareCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1);

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldDeclareMinCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1);

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MIN_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldDeclareMaxCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1);

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MAX_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldDeclareMinMaxCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, 2);

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 5);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MIN_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MAX_CARDINALITY, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMinMaxCardinalityRestrictionBecauseInvalidMax()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareMinMaxCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 2, 1));

        [TestMethod]
        public void ShouldDeclareQualifiedCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, new RDFResource("ex:onClass"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 5);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:onClass"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringQualifiedCardinalityRestrictionBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, null));

        [TestMethod]
        public void ShouldDeclareMinQualifiedCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, new RDFResource("ex:onClass"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 5);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:onClass"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMinQualifiedCardinalityRestrictionBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareMinQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, null));

        [TestMethod]
        public void ShouldDeclareMaxQualifiedCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, new RDFResource("ex:onClass"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 5);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:onClass"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMaxQualifiedCardinalityRestrictionBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareMaxQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, null));

        [TestMethod]
        public void ShouldDeclareMinMaxQualifiedCardinalityRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, 2, new RDFResource("ex:onClass"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 6);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:onClass"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:cRestr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMinMaxQualifiedCardinalityRestrictionBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 1, 2, null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMinMaxQualifiedCardinalityRestrictionBecauseInvalidMax()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:cRestr"), new RDFResource("ex:onProp"), 2, 1, new RDFResource("ex:onClass")));

        [TestMethod]
        public void ShouldDeclareEnumerateClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareEnumerateClass(new RDFResource("ex:enumClass"), new List<RDFResource>() { new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2") });

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 1);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 8);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:enumClass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:enumClass"), RDFVocabulary.OWL.ONE_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv1"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv2"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> enumeratesEnumerator = classModel.EnumeratesEnumerator;
            while (enumeratesEnumerator.MoveNext())
            {
                Assert.IsTrue(enumeratesEnumerator.Current.Equals(new RDFResource("ex:enumClass")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEnumerateClassBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareEnumerateClass(null, new List<RDFResource>() { new RDFResource("ex:indiv1") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEnumerateClassBecauseNullIndividuals()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareEnumerateClass(new RDFResource("ex:enumClass"), null as List<RDFResource>));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEnumerateClassBecauseEmptyIndividuals()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareEnumerateClass(new RDFResource("ex:enumClass"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldDeclareEnumerateLiteralClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareEnumerateClass(new RDFResource("ex:enumlitClass"), new List<RDFLiteral>() { new RDFPlainLiteral("lit1"), new RDFPlainLiteral("lit2") });

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 1);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 8);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:enumlitClass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:enumlitClass"), RDFVocabulary.OWL.ONE_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, null, new RDFPlainLiteral("lit1")].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, null, new RDFPlainLiteral("lit2")].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> enumeratesEnumerator = classModel.EnumeratesEnumerator;
            while (enumeratesEnumerator.MoveNext())
            {
                Assert.IsTrue(enumeratesEnumerator.Current.Equals(new RDFResource("ex:enumlitClass")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEnumerateLiteralClassBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareEnumerateClass(null, new List<RDFLiteral>() { new RDFPlainLiteral("lit1") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEnumerateLiteralClassBecauseNullIndividuals()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareEnumerateClass(new RDFResource("ex:enumlitClass"), null as List<RDFLiteral>));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEnumerateLiteralClassBecauseEmptyIndividuals()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareEnumerateClass(new RDFResource("ex:enumlitClass"), new List<RDFLiteral>()));

        [TestMethod]
        public void ShouldDeclareUnionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:class2") });

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 1);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 8);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:unionClass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:unionClass"), RDFVocabulary.OWL.UNION_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class2"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> compositesEnumerator = classModel.CompositesEnumerator;
            while (compositesEnumerator.MoveNext())
            {
                Assert.IsTrue(compositesEnumerator.Current.Equals(new RDFResource("ex:unionClass")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringUnionClassBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareUnionClass(null, new List<RDFResource>() { new RDFResource("ex:class1") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringUnionClassBecauseNullClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareUnionClass(new RDFResource("ex:unionClass"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringUnionClassBecauseEmptyClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringUnionClassBecauseSelfClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareUnionClass(new RDFResource("ex:class1"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:class2") }));

        [TestMethod]
        public void ShouldDeclareIntersectionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareIntersectionClass(new RDFResource("ex:intersectionClass"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:class2") });

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 1);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 8);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:intersectionClass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:intersectionClass"), RDFVocabulary.OWL.INTERSECTION_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class2"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> compositesEnumerator = classModel.CompositesEnumerator;
            while (compositesEnumerator.MoveNext())
            {
                Assert.IsTrue(compositesEnumerator.Current.Equals(new RDFResource("ex:intersectionClass")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIntersectionClassBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareIntersectionClass(null, new List<RDFResource>() { new RDFResource("ex:class1") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIntersectionClassBecauseNullClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareIntersectionClass(new RDFResource("ex:intersectionClass"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIntersectionClassBecauseEmptyClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareIntersectionClass(new RDFResource("ex:intersectionClass"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIntersectionClassBecauseSelfClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareIntersectionClass(new RDFResource("ex:class1"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:class2") }));

        [TestMethod]
        public void ShouldDeclareComplementClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareComplementClass(new RDFResource("ex:complementClass"), new RDFResource("ex:class"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 1);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:complementClass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:complementClass"), RDFVocabulary.OWL.COMPLEMENT_OF, new RDFResource("ex:class"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> compositesEnumerator = classModel.CompositesEnumerator;
            while (compositesEnumerator.MoveNext())
            {
                Assert.IsTrue(compositesEnumerator.Current.Equals(new RDFResource("ex:complementClass")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringComplementClassBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareComplementClass(null, new RDFResource("ex:class1")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringComplementClassBecauseNullComplementClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareComplementClass(new RDFResource("ex:complementClass"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringComplementClassBecauseSelfClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareComplementClass(new RDFResource("ex:complementClass"), new RDFResource("ex:complementClass")));

        [TestMethod]
        public void ShouldDeclareDisjointUnionClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:class2") });

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 8);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:disjointUnionClass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:disjointUnionClass"), RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class2"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDisjointUnionClassBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareDisjointUnionClass(null, new List<RDFResource>() { new RDFResource("ex:class1") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDisjointUnionClassBecauseNullClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDisjointUnionClassBecauseEmptyClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldDeclareAllDisjointClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:class2") });

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 1);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.SimpleClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 0);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 9);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:allDisjointClasses"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:allDisjointClasses"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:allDisjointClasses"), RDFVocabulary.OWL.MEMBERS, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:class2"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);


            int i = 0;
            IEnumerator<RDFResource> allDisjointClassesEnumerator = classModel.AllDisjointClassesEnumerator;
            while (allDisjointClassesEnumerator.MoveNext())
            {
                Assert.IsTrue(allDisjointClassesEnumerator.Current.Equals(new RDFResource("ex:allDisjointClasses")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAllDisjointClassesBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareAllDisjointClasses(null, new List<RDFResource>() { new RDFResource("ex:class1") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAllDisjointClassesBecauseNullClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAllDisjointClassesBecauseEmptyClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldAnnotateResourceClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso")); //Will be discarded, since duplicate annotations are not allowed

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 1);
            Assert.IsTrue(classModel.OBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"))));
        }

        [TestMethod]
        public void ShouldAnnotateLiteralClass()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label"));
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label")); //Will be discarded, since duplicate annotations are not allowed

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 1);
            Assert.IsTrue(classModel.OBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label"))));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceClassBecauseNullSubject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:class1"))
                        .AnnotateClass(null, RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceClassBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:class1"))
                        .AnnotateClass(new RDFResource("ex:class1"), null, new RDFResource("ex:seealso")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceClassBecauseBlankPredicate()
           => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                       .DeclareClass(new RDFResource("ex:class1"))
                       .AnnotateClass(new RDFResource("ex:class1"), new RDFResource(), new RDFResource("ex:seealso")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceClassBecauseNullObject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:class1"))
                        .AnnotateClass(new RDFResource("ex:class1"), RDFVocabulary.RDFS.SEE_ALSO, null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralClassBecauseNullSubject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:class1"))
                        .AnnotateClass(null, RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralClassBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:class1"))
                        .AnnotateClass(new RDFResource("ex:class1"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralClassBecauseBlankPredicate()
           => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                       .DeclareClass(new RDFResource("ex:class1"))
                       .AnnotateClass(new RDFResource("ex:class1"), new RDFResource(), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralClassBecauseNullObject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:class1"))
                        .AnnotateClass(new RDFResource("ex:class1"), RDFVocabulary.RDFS.LABEL, null as RDFLiteral));

        [TestMethod]
        public void ShouldDeclareSubClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 3);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:classB"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringSubClassesBecauseIncompatibleClasses()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));  //OWL-DL contraddiction

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("SubClass relation between class 'ex:classA' and class 'ex:classB' cannot be declared to the model because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:classA"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSubClassesBecauseNullChildClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareClass(new RDFResource("ex:classB"))
                        .DeclareSubClasses(null, new RDFResource("ex:classB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSubClassesBecauseNullMotherClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareClass(new RDFResource("ex:classB"))
                        .DeclareSubClasses(new RDFResource("ex:classA"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSubClassesBecauseSelfClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classA")));

        [TestMethod]
        public void ShouldDeclareEquivalentClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.OWL.EQUIVALENT_CLASS, new RDFResource("ex:classB"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.OWL.EQUIVALENT_CLASS, new RDFResource("ex:classA"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringEquivalentClassesBecauseIncompatibleClasses()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));  //OWL-DL contraddiction

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("EquivalentClass relation between class 'ex:classA' and class 'ex:classB' cannot be declared to the model because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:classB"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEquivalentClassesBecauseNullLeftClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareClass(new RDFResource("ex:classB"))
                        .DeclareEquivalentClasses(null, new RDFResource("ex:classB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEquivalentClassesBecauseNullRightClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareClass(new RDFResource("ex:classB"))
                        .DeclareEquivalentClasses(new RDFResource("ex:classA"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEquivalentClassesBecauseSelfClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classA")));

        [TestMethod]
        public void ShouldDeclareDisjointClasses()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));

            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 4);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.OWL.DISJOINT_WITH, new RDFResource("ex:classB"))));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.OWL.DISJOINT_WITH, new RDFResource("ex:classA"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringDisjointClassesBecauseIncompatibleClasses()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));  //OWL-DL contraddiction

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("DisjointWith relation between class 'ex:classA' and class 'ex:classB' cannot be declared to the model because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:classA"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:classB"))));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDisjointClassesBecauseNullLeftClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareClass(new RDFResource("ex:classB"))
                        .DeclareDisjointClasses(null, new RDFResource("ex:classB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDisjointClassesBecauseNullRightClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareClass(new RDFResource("ex:classB"))
                        .DeclareDisjointClasses(new RDFResource("ex:classA"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDisjointClassesBecauseSelfClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel()
                        .DeclareClass(new RDFResource("ex:classA"))
                        .DeclareDisjointClasses(new RDFResource("ex:classA"), new RDFResource("ex:classA")));

        [TestMethod]
        public void ShouldDeclareKeys()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { new RDFResource("ex:dtProp") });

            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 5);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:classA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[new RDFResource("ex:classA"), RDFVocabulary.OWL.HAS_KEY, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:dtProp"), null].TriplesCount == 1);
            Assert.IsTrue(classModel.TBoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 1);
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHasKeyBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareHasKey(null, new List<RDFResource>() { new RDFResource("ex:dtProp") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHasKeyBecauseNullClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareHasKey(new RDFResource("ex:classA"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHasKeyBecauseEmptyClasses()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldDeclareRestriction()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareRestriction(new RDFResource("ex:restr"), new RDFResource("ex:onProp"));

            Assert.IsTrue(classModel.ClassesCount == 1);
            Assert.IsTrue(classModel.AllDisjointClassesCount == 0);
            Assert.IsTrue(classModel.CompositesCount == 0);
            Assert.IsTrue(classModel.DeprecatedClassesCount == 0);
            Assert.IsTrue(classModel.EnumeratesCount == 0);
            Assert.IsTrue(classModel.RestrictionsCount == 1);
            Assert.IsTrue(classModel.TBoxGraph.TriplesCount == 3);
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:restr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:restr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION)));
            Assert.IsTrue(classModel.TBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:restr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:onProp"))));
            Assert.IsTrue(classModel.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> restrictionsEnumerator = classModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                Assert.IsTrue(restrictionsEnumerator.Current.Equals(new RDFResource("ex:restr")));
                i++;
            }
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRestrictionBecauseNullRestriction()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareRestriction(null, new RDFResource("ex:onProp")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRestrictionBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyClassModel().DeclareRestriction(new RDFResource("ex:restr"), null));

        [TestMethod]
        public void ShouldExportToGraph()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareClass(new RDFResource("ex:classE"), new OWLOntologyClassBehavior() { Deprecated = true });
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            classModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            classModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            classModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            classModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));
            RDFGraph graph = classModel.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 34);
        }

        [TestMethod]
        public void ShouldExportToGraphWithoutInferences()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareClass(new RDFResource("ex:classE"), new OWLOntologyClassBehavior() { Deprecated = true });
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            classModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            classModel.DeclareAllDisjointClasses(new RDFResource("ex:allDisjointClasses"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            classModel.DeclareDisjointUnionClass(new RDFResource("ex:disjointUnionClass"), new List<RDFResource>() { new RDFResource("ex:classD"), new RDFResource("ex:classE") });
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            classModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));
            RDFGraph graph = classModel.ToRDFGraph(false);

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 32);
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsync()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            classModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            classModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));
            RDFGraph graph = await classModel.ToRDFGraphAsync();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 15);
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsyncWithoutInferences()
        {
            OWLOntologyClassModel classModel = new OWLOntologyClassModel();
            classModel.DeclareClass(new RDFResource("ex:classA"));
            classModel.DeclareClass(new RDFResource("ex:classB"));
            classModel.DeclareClass(new RDFResource("ex:classC"));
            classModel.DeclareClass(new RDFResource("ex:classD"));
            classModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classA"));
            classModel.DeclareEquivalentClasses(new RDFResource("ex:classA"), new RDFResource("ex:classC"));
            classModel.DeclareDisjointClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));
            classModel.DeclareHasKey(new RDFResource("ex:classA"), new List<RDFResource>() { RDFVocabulary.FOAF.ACCOUNT });
            classModel.AnnotateClass(new RDFResource("ex:classA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            classModel.AnnotateClass(new RDFResource("ex:classB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));
            RDFGraph graph = await classModel.ToRDFGraphAsync(false);

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 13);
        }
        #endregion
    }
}