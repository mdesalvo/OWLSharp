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
    public class SKOSRelatedRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateRelated()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.DeclareConcept(new RDFResource("ex:conceptA"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:conceptB"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:conceptC"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:conceptD"), new RDFResource("ex:conceptScheme"));
            //Since our SKOS API is protected, we are writing this rule to cover low-level OWL modeling use cases...
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:conceptB"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:conceptB")); //clash on hierarchical relations
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:conceptC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:conceptC")); //clash on mapping relations
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptB"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:conceptD"));

            OWLValidatorReport validatorReport = SKOSRelatedRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateRelatedViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.DeclareConcept(new RDFResource("ex:conceptA"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:conceptB"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:conceptC"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:conceptD"), new RDFResource("ex:conceptScheme"));
            //Since our SKOS API is protected, we are writing this rule to cover low-level OWL modeling use cases...
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:conceptB"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.NARROWER, new RDFResource("ex:conceptB")); //clash on hierarchical relations
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.RELATED_MATCH, new RDFResource("ex:conceptC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptA"), RDFVocabulary.SKOS.CLOSE_MATCH, new RDFResource("ex:conceptC")); //clash on mapping relations
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:conceptB"), RDFVocabulary.SKOS.RELATED, new RDFResource("ex:conceptD"));

            OWLValidator validator = new OWLValidator().AddSKOSRule(SKOSEnums.SKOSValidatorRules.Related);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}