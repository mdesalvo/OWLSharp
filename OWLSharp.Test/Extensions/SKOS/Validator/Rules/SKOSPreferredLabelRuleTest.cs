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

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSPreferredLabelRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidatePreferredLabel()
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
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept1", "en-US"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept1B", "en-US")); //clash on "EN-US" language
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2", "en"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2", "en-US"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2B", "en-US")); //clash on "EN-US" language
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept3"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept3", "it-IT"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:label2"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label2", "en-US"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:label2"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label2B", "en-US")); //SKOS-XL: clash on "EN-US" language
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:concept3"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label3"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:label3"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label3"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:label3"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label3B")); //SKOS-XL: clash on empty language
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:label3"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label3", "en-US"));
            
            OWLValidatorReport validatorReport = SKOSPreferredLabelRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 4);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidatePreferredLabelViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptSchemeA"));
            //Since our SKOS API is protected, we are writing this rule to cover low-level OWL modeling use cases...
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept1", "en-US"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept1"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept1B", "en-US")); //clash on "EN-US" language
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2", "en"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2", "en-US"));
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept2"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept2B", "en-US")); //clash on "EN-US" language
            ontology.Data.AnnotateIndividual(new RDFResource("ex:concept3"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("concept3", "it-IT"));

            OWLValidator validator = new OWLValidator().AddSKOSRule(SKOSEnums.SKOSValidatorRules.PreferredLabel);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}