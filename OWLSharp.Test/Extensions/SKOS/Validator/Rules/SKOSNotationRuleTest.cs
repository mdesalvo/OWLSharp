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
    public class SKOSNotationRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateNotation()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept5"), new RDFResource("ex:conceptSchemeB"));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept1"), new RDFTypedLiteral("CNPT_X4K", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept2"), new RDFTypedLiteral("CNPT_X4K", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING)); //clash on skos:notation under ex:conceptSchemeA
            ontology.DeclareConceptNotation(new RDFResource("ex:concept3"), new RDFTypedLiteral("CNPT_ZZ6", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept5"), new RDFTypedLiteral("CNPT_X4K", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING)); //no clash, since belonging to ex:conceptSchemeB

            OWLValidatorReport validatorReport = SKOSNotationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNotationViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:concept1"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept2"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept3"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept4"), new RDFResource("ex:conceptSchemeA"));
            ontology.DeclareConcept(new RDFResource("ex:concept5"), new RDFResource("ex:conceptSchemeB"));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept1"), new RDFTypedLiteral("CNPT_X4K", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept2"), new RDFTypedLiteral("CNPT_X4K", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING)); //clash on skos:notation under ex:conceptSchemeA
            ontology.DeclareConceptNotation(new RDFResource("ex:concept3"), new RDFTypedLiteral("CNPT_ZZ6", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING));
            ontology.DeclareConceptNotation(new RDFResource("ex:concept5"), new RDFTypedLiteral("CNPT_X4K", RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING)); //no clash, since belonging to ex:conceptSchemeB

            OWLValidator validator = new OWLValidator().AddSKOSRule(SKOSEnums.SKOSValidatorRules.Notation);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}