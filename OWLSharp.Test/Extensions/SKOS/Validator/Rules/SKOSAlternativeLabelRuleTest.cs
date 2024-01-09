﻿/*
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

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSAlternativeLabelRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateAlternativeLabel()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareLabel(new RDFResource("ex:label2"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareLabel(new RDFResource("ex:label3"), new RDFResource("ex:conceptSchemeA"));
            //Since our SKOS API is protected, we are writing this rule to cover low-level OWL modeling use cases...
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("concept1", "en-US"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept1", "en-US")); //clash
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept3"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("concept3", "en"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept3"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("concept3", "en")); //clash
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("concept2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:label2"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("concept2")); //clash
            
            OWLValidatorReport validatorReport = SKOSAlternativeLabelRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateAlternativeLabelViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareLabel(new RDFResource("ex:label2"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareLabel(new RDFResource("ex:label3"), new RDFResource("ex:conceptSchemeA"));
            //Since our SKOS API is protected, we are writing this rule to cover low-level OWL modeling use cases...
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("concept1", "en-US"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept1", "en-US")); //clash
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept3"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("concept3", "en"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept3"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("concept3", "en")); //clash
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("concept2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:label2"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("concept2")); //clash

            OWLValidator validator = new OWLValidator().AddSKOSRule(SKOSEnums.SKOSValidatorRules.AlternativeLabel);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}