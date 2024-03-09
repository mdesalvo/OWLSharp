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
    public class OWLLocalCardinalityRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateLocalCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on direct transitivity

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateLocalCardinalityOnSuperProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:superobjprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:objprop"), new RDFResource("ex:superobjprop"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on indirect transitivity

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateLocalCardinalityOnInverseProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:inverseobjprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:objprop"), new RDFResource("ex:inverseobjprop"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on indirect transitivity

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateLocalCardinalityViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on direct transitivity


            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.LocalCardinality);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        //BUGFIX #26

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnExactCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:EmptyBox"), new RDFResource("ex:contains"), 0);
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:EmptyBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnExactDataCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:EmptyBox"), new RDFResource("ex:contains"), 0);
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:EmptyBox"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFPlainLiteral("an item"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnExactQualifiedCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareQualifiedCardinalityRestriction(new RDFResource("ex:EmptyBox"), new RDFResource("ex:contains"), 0, new RDFResource("ex:Item"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:EmptyBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1")); //no clash (because item1 is not an item)

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnMaxCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:Max1ItemBox"), new RDFResource("ex:contains"), 1);
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:item0"), new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeNotClashingOnMaxCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:Max1ItemBox"), new RDFResource("ex:contains"), 1);
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1")); //no clash because we dont know if they are owl:differentFrom

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 0);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeNotClashingOnMax2Cardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:Max2ItemBox"), new RDFResource("ex:contains"), 2);
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:item0"), new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item2"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Max2ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item2")); //no clash because we dont know if it is owl:differentFrom against ex:item0 or ex:item1

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 0);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnMaxDataCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMaxCardinalityRestriction(new RDFResource("ex:Max1ItemBox"), new RDFResource("ex:contains"), 1);
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Max1ItemBox"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFPlainLiteral("item 1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFPlainLiteral("item 2"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnMaxQualifiedCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("ex:Max1ItemBox"), new RDFResource("ex:contains"), 1, new RDFResource("ex:Item"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:item0"), new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeNotClashingOnMaxQualifiedCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("ex:Max1ItemBox"), new RDFResource("ex:contains"), 1, new RDFResource("ex:Item"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1")); //no clash because we dont know if they are owl:differentFrom

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 0);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnMinMaxCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:Min1Max1ItemBox"), new RDFResource("ex:contains"), 1, 1);
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:item0"), new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Min1Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeNotClashingOnMinMaxCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:Min1Max1ItemBox"), new RDFResource("ex:contains"), 1, 1);
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Min1Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1")); //no clash because we dont know if they are different individuals

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 0);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnMinMaxDataCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMinMaxCardinalityRestriction(new RDFResource("ex:Min1Max1ItemBox"), new RDFResource("ex:contains"), 1, 1);
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Min1Max1ItemBox"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFPlainLiteral("item 1"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFPlainLiteral("item 2"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnMinMaxQualifiedCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:Min1Max1ItemBox"), new RDFResource("ex:contains"), 1, 1, new RDFResource("ex:Item"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:item0"), new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Min1Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        
        [TestMethod]
        public void ShouldValidateClassTypeNotClashingOnMinMaxQualifiedCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("ex:Min1Max1ItemBox"), new RDFResource("ex:contains"), 1, 1, new RDFResource("ex:Item"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Min1Max1ItemBox"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item0"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFResource("ex:item1")); //no clash because we dont know if they are owl:differentFrom

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 0);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeNeverProceedingOnMinCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:Item"));
            ontology.Model.ClassModel.DeclareMinCardinalityRestriction(new RDFResource("ex:Min3ItemBox"), new RDFResource("ex:contains"), 3); //min are not validated
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:contains"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item0"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item0"), new RDFResource("ex:Item"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:item1"), new RDFResource("ex:Item"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:item0"), new RDFResource("ex:item1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:iBox"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:iBox"), new RDFResource("ex:Min3ItemBox"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:iBox"), new RDFResource("ex:contains"), new RDFPlainLiteral("an item"));

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 0);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}