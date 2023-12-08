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

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLInverseOfRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateInverseOfAndNotFail()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:man"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:woman"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:male"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:man"), new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:woman"), new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:male"), new RDFResource("ex:man"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasWife"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:man"), Range = new RDFResource("ex:woman") });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasHusband"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:woman"), Range = new RDFResource("ex:male") });
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:hasWife"), new RDFResource("ex:hasHusband"));

            OWLValidatorReport validatorReport = OWLInverseOfRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 0);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateInverseOfByFailingOnDomainVSRange()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:man"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:woman"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:father"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:father"), new RDFResource("ex:man"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:man"), new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:woman"), new RDFResource("ex:human"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasWife"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:man"), Range = new RDFResource("ex:woman") });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasHusband"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:woman"), Range = new RDFResource("ex:father") }); //clash on hierarchy against ex:man
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:hasWife"), new RDFResource("ex:hasHusband"));

            OWLValidatorReport validatorReport = OWLInverseOfRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateInverseOfByFailingOnRangeVSDomain()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:man"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:woman"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:father"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:father"), new RDFResource("ex:man"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:man"), new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:woman"), new RDFResource("ex:human"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasWife"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:father"), Range = new RDFResource("ex:woman") }); //clash on hierarchy against ex:man
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasHusband"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:woman"), Range = new RDFResource("ex:man") });
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:hasWife"), new RDFResource("ex:hasHusband"));

            OWLValidatorReport validatorReport = OWLInverseOfRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateInverseOfViaValidatorByFailingOnDomainVSRange()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:man"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:woman"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:father"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:father"), new RDFResource("ex:man"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:man"), new RDFResource("ex:human"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:woman"), new RDFResource("ex:human"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasWife"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:man"), Range = new RDFResource("ex:woman") });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasHusband"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:woman"), Range = new RDFResource("ex:father") }); //clash on hierarchy against ex:man
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:hasWife"), new RDFResource("ex:hasHusband"));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.InverseOf);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}