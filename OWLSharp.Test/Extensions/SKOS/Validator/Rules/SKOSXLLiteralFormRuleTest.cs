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
    public class SKOSXLLiteralFormRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateLiteralForm()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label2"), new RDFResource("ex:conceptScheme")); //clash on absence of skosxl:literalForm
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label1"), new RDFPlainLiteral("label", "en-US"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label1"), new RDFPlainLiteral("etichetta", "it-IT")); //clash on cardinality restrictions on skosxl:literalForm

            OWLValidatorReport validatorReport = SKOSXLLiteralFormRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateLiteralFormViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label1"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareLabel(new RDFResource("ex:label2"), new RDFResource("ex:conceptScheme")); //clash on absence of skosxl:literalForm
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label1"), new RDFPlainLiteral("label", "en-US"));
            ontology.DeclareConceptPreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label1"), new RDFPlainLiteral("etichetta", "it-IT")); //clash on cardinality restrictions on skosxl:literalForm

            OWLValidator validator = new OWLValidator().AddSKOSRule(SKOSEnums.SKOSValidatorRules.LiteralForm);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}