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
    public class OWLClassKeyRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateClassKey()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareHasKey(new RDFResource("ex:class1"), [new RDFResource("ex:dtprop1"), new RDFResource("ex:dtprop2")]);
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2")); //this class has no keys, it will not be considered
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class3")); //this class has no members, it will not be considered
            ontology.Model.ClassModel.DeclareHasKey(new RDFResource("ex:class3"), [new RDFResource("ex:dtprop1")]);
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3")); //this individual, although colliding on key with ex:individual1 and ex:individual2, will not clash on owl:differentFrom
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv3"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv4")); //this individual has a partial key: it cannot clash with full keys
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv4"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv4"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv5")); //this individual has not a key: it will not be considered
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv5"), new RDFResource("ex:class1"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv6")); //this individual will not collide on key
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv6"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv6"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv6"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val1"));

            OWLValidatorReport validatorReport = OWLClassKeyRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassKeyPartialKeyValues()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareHasKey(new RDFResource("ex:class1"), [new RDFResource("ex:dtprop1"), new RDFResource("ex:dtprop2")]);
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));

            OWLValidatorReport validatorReport = OWLClassKeyRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassKeyMultipleKeyValues()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareHasKey(new RDFResource("ex:class1"), [new RDFResource("ex:dtprop1"), new RDFResource("ex:dtprop2")]);
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3")); //this individual, although colliding on key, will not clash on owl:differentFrom
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv3"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv4")); //this individual has a partial key: it will not be considered
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv4"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv4"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv5")); //this individual has not a key: it will not be considered
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv5"), new RDFResource("ex:class1"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv6")); //this individual will not collide on key
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv6"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv6"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv6"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val1"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv7")); //this individual is not of constrained class ex:class1, although colliding on key with ex:indiv1
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv7"), new RDFResource("ex:class2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv7"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv7"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv7"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));

            OWLValidatorReport validatorReport = OWLClassKeyRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassKeyViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareHasKey(new RDFResource("ex:class1"), [new RDFResource("ex:dtprop1"), new RDFResource("ex:dtprop2")]);
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3")); //this individual, although colliding on key, will not clash on owl:differentFrom
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv3"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val2"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv4")); //this individual has a partial key: it will not be considered
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv4"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv4"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv5")); //this individual has not a key: it will not be considered
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv5"), new RDFResource("ex:class1"));

            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv6")); //this individual will not collide on key
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv6"), new RDFResource("ex:class1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv6"), new RDFResource("ex:dtprop1"), new RDFPlainLiteral("val1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv6"), new RDFResource("ex:dtprop2"), new RDFPlainLiteral("val1"));

            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.ClassKey);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}