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

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLPropertyEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteObjectPropertyEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fallenInLoveWith"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:loves"), new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallenInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentine"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:mark"), new RDFResource("ex:loves"), new RDFResource("ex:valentine"));

            OWLReasonerReport reasonerReport = OWLPropertyEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteObjectPropertyEntailmentWithInlineBlankSubPropertiesAndNotCrashBecauseBlankPredicate()
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

            OWLReasonerReport reasonerReport = OWLPropertyEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 0);
        }

        [TestMethod]
        public void ShouldExecuteObjectPropertyEntailmentWithInlineBlankEquivalentPropertiesAndNotCrashBecauseBlankPredicate()
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

            OWLReasonerReport reasonerReport = OWLPropertyEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 0);
        }

        [TestMethod]
        public void ShouldExecuteObjectPropertyEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fallenInLoveWith"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:loves"), new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallenInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:valentine"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:mark"), new RDFResource("ex:loves"), new RDFResource("ex:valentine"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2"));

            OWLReasoner reasoner = new OWLReasoner().AddRule(OWLEnums.OWLReasonerRules.PropertyEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteDatatypePropertyEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:fallenInLoveWith"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:loves"), new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallenInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:mark"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentine", "en-US"));

            OWLReasonerReport reasonerReport = OWLPropertyEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteDatatypePropertyEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:loves"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:fallenInLoveWith"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:loves"), new RDFResource("ex:hasSentimentFor"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:loves"), new RDFResource("ex:fallenInLoveWith"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:mark"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:mark"), new RDFResource("ex:loves"), new RDFPlainLiteral("valentine", "en-US"));

            OWLReasoner reasoner = new OWLReasoner().AddRule(OWLEnums.OWLReasonerRules.PropertyEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}