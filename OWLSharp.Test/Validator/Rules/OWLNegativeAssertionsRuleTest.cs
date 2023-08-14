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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLNegativeAssertionsRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateNegativeObjectAssertionsOnSubjectSynonim()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentina"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentine"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:valentina"), new RDFResource("ex:valentine"));
            ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFResource("ex:valentina"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:mark"), new RDFResource("ex:loves"), new RDFResource("ex:valentina")); //clash on subject synonim

            OWLValidatorReport validatorReport = OWLNegativeAssertionsRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeObjectAssertionsOnPredicateSynonim()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentina"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentine"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:valentina"), new RDFResource("ex:valentine"));
            ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFResource("ex:valentina"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:fallInLoveWith"), new RDFResource("ex:valentina")); //clash on predicate synonim (which is equivalent to ex:loves)
            
            OWLValidatorReport validatorReport = OWLNegativeAssertionsRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeObjectAssertionsOnObjectSynonim()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentina"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentine"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:valentina"), new RDFResource("ex:valentine"));
            ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFResource("ex:valentina"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFResource("ex:valentine")); //clash on object synonim
            
            OWLValidatorReport validatorReport = OWLNegativeAssertionsRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeObjectAssertionsOnSubjectSynonimAndPredicateSynonimAndObjectSynonim()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentina"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentine"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:valentina"), new RDFResource("ex:valentine"));
            ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFResource("ex:valentina"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:mark"), new RDFResource("ex:fallInLoveWith"), new RDFResource("ex:valentine")); //clash on subject synonim and object synonim and predicate synonim (which is equivalent to ex:loves)

            OWLValidatorReport validatorReport = OWLNegativeAssertionsRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeObjectAssertionsDisjointViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentina"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentine"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:valentina"), new RDFResource("ex:valentine"));
            ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFResource("ex:valentina"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:marco"), new RDFResource("ex:fallInLoveWith"), new RDFResource("ex:valentina")); //clash on negated ex:loves

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.NegativeAssertions);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeDatatypeAssertionsOnSubjectSynonim()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentina"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:mark"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentina")); //clash on subject synonim

            OWLValidatorReport validatorReport = OWLNegativeAssertionsRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeDatatypeAssertionsOnPredicateSynonim()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentina"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:marco"), new RDFResource("ex:fallInLoveWith"), new RDFPlainLiteral("valentina")); //clash on predicate synonim (which is equivalent to ex:loves)

            OWLValidatorReport validatorReport = OWLNegativeAssertionsRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeDatatypeAssertionsOnSubjectSynonimAndPredicateSynonim()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentina"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:mark"), new RDFResource("ex:fallInLoveWith"), new RDFPlainLiteral("valentina")); //clash on subject synonim and predicate synonim (which is equivalent to ex:loves)

            OWLValidatorReport validatorReport = OWLNegativeAssertionsRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNegativeDatatypeAssertionsDisjointViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:fallInLoveWith"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:marco"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:marco"), new RDFResource("ex:mark"));
            ontology.Data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:marco"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentina"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:mark"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentina")); //clash on subject synonim

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.NegativeAssertions);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}