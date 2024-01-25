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
using System.Collections.Generic;

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLDisjointClassConsistencyRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateDisjointClassConsistencyFailingOnSubClassLeft()
        {
            OWLOntologyClassModel classmodel1 = new OWLOntologyClassModel();
            classmodel1.DeclareClass(new RDFResource("ex:class1"));
            classmodel1.DeclareClass(new RDFResource("ex:class2"));
            classmodel1.DeclareAllDisjointClasses(new RDFResource("ex:adj"), new List<RDFResource>() { new RDFResource("ex:class1"), new RDFResource("ex:class2") });
            OWLOntologyClassModel classmodel2 = new OWLOntologyClassModel();
            classmodel2.DeclareClass(new RDFResource("ex:class1"));
            classmodel2.DeclareClass(new RDFResource("ex:class2"));
            classmodel2.DeclareSubClasses(new RDFResource("ex:class1"),new RDFResource("ex:class2"));

            classmodel1.Merge(classmodel2);

            OWLValidatorReport validatorReport = OWLDisjointClassConsistencyRule.ExecuteRule(new OWLOntology("ex:org") { 
                Model = new OWLOntologyModel() { ClassModel = classmodel1 } });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateDisjointClassConsistencyFailingOnSubClassRight()
        {
            OWLOntologyClassModel classmodel1 = new OWLOntologyClassModel();
            classmodel1.DeclareClass(new RDFResource("ex:class1"));
            classmodel1.DeclareClass(new RDFResource("ex:class2"));
            classmodel1.DeclareDisjointClasses(new RDFResource("ex:class1"), new RDFResource("ex:class2"));
            OWLOntologyClassModel classmodel2 = new OWLOntologyClassModel();
            classmodel2.DeclareClass(new RDFResource("ex:class1"));
            classmodel2.DeclareClass(new RDFResource("ex:class2"));
            classmodel2.DeclareSubClasses(new RDFResource("ex:class2"), new RDFResource("ex:class1"));

            classmodel1.Merge(classmodel2);

            OWLValidatorReport validatorReport = OWLDisjointClassConsistencyRule.ExecuteRule(new OWLOntology("ex:org")
            {
                Model = new OWLOntologyModel() { ClassModel = classmodel1 }
            });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateDisjointClassConsistencyFailingOnEquivalentClass()
        {
            OWLOntologyClassModel classmodel1 = new OWLOntologyClassModel();
            classmodel1.DeclareClass(new RDFResource("ex:class1"));
            classmodel1.DeclareClass(new RDFResource("ex:class2"));
            classmodel1.DeclareDisjointClasses(new RDFResource("ex:class1"),new RDFResource("ex:class2"));
            OWLOntologyClassModel classmodel2 = new OWLOntologyClassModel();
            classmodel2.DeclareClass(new RDFResource("ex:class1"));
            classmodel2.DeclareClass(new RDFResource("ex:class2"));
            classmodel2.DeclareEquivalentClasses(new RDFResource("ex:class1"),new RDFResource("ex:class2"));

            classmodel1.Merge(classmodel2);

            OWLValidatorReport validatorReport = OWLDisjointClassConsistencyRule.ExecuteRule(new OWLOntology("ex:org") { 
                Model = new OWLOntologyModel() { ClassModel = classmodel1 } });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateDisjointClassConsistencyViaValidator()
        {
            OWLOntologyClassModel classmodel1 = new OWLOntologyClassModel();
            classmodel1.DeclareClass(new RDFResource("ex:class1"));
            classmodel1.DeclareClass(new RDFResource("ex:class2"));
            classmodel1.DeclareDisjointClasses(new RDFResource("ex:class2"),new RDFResource("ex:class1"));
            OWLOntologyClassModel classmodel2 = new OWLOntologyClassModel();
            classmodel2.DeclareClass(new RDFResource("ex:class1"));
            classmodel2.DeclareClass(new RDFResource("ex:class2"));
            classmodel2.DeclareSubClasses(new RDFResource("ex:class1"),new RDFResource("ex:class2"));

            classmodel1.Merge(classmodel2);

            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.DisjointClassConsistency);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(new OWLOntology("ex:org") { 
                Model = new OWLOntologyModel() { ClassModel = classmodel1 } });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}