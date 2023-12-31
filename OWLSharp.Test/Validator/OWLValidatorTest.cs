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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLValidatorTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateValidator()
        {
            OWLValidator validator = new OWLValidator();

            Assert.IsNotNull(validator);
            Assert.IsNotNull(validator.Rules);
            Assert.IsTrue(validator.Rules.Count == 2);
            Assert.IsTrue(validator.Rules.ContainsKey("STD"));
            Assert.IsTrue(validator.Rules["STD"] is List<OWLEnums.OWLValidatorRules> stdRules && stdRules.Count == 0);
            Assert.IsTrue(validator.Rules.ContainsKey("CTM"));
            Assert.IsTrue(validator.Rules["CTM"] is List<OWLValidatorRule> ctmRules && ctmRules.Count == 0);
            Assert.IsNotNull(validator.Extensions);
            Assert.IsTrue(validator.Extensions.Count == 0);
        }

        [TestMethod]
        public void ShouldAddStandardRule()
        {
            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness); //Will be discarded, since duplicate standard rules are not allowed

            Assert.IsNotNull(validator);
            Assert.IsNotNull(validator.Rules);
            Assert.IsTrue(validator.Rules.Count == 2);
            Assert.IsTrue(validator.Rules.ContainsKey("STD"));
            Assert.IsTrue(validator.Rules["STD"] is List<OWLEnums.OWLValidatorRules> stdRules && stdRules.Count == 1);
            Assert.IsTrue(validator.Rules.ContainsKey("CTM"));
            Assert.IsTrue(validator.Rules["CTM"] is List<OWLValidatorRule> ctmRules && ctmRules.Count == 0);
            Assert.IsNotNull(validator.Extensions);
            Assert.IsTrue(validator.Extensions.Count == 0);
        }

        [TestMethod]
        public void ShouldAddCustomRule()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            Assert.IsNotNull(validator);
            Assert.IsNotNull(validator.Rules);
            Assert.IsTrue(validator.Rules.Count == 2);
            Assert.IsTrue(validator.Rules.ContainsKey("STD"));
            Assert.IsTrue(validator.Rules["STD"] is List<OWLEnums.OWLValidatorRules> stdRules && stdRules.Count == 0);
            Assert.IsTrue(validator.Rules.ContainsKey("CTM"));
            Assert.IsTrue(validator.Rules["CTM"] is List<OWLValidatorRule> ctmRules && ctmRules.Count == 1);
            Assert.IsNotNull(validator.Extensions);
            Assert.IsTrue(validator.Extensions.Count == 0);
        }

        [TestMethod]
        public void ShouldAddMixedRules()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            Assert.IsNotNull(validator);
            Assert.IsNotNull(validator.Rules);
            Assert.IsTrue(validator.Rules.Count == 2);
            Assert.IsTrue(validator.Rules.ContainsKey("STD"));
            Assert.IsTrue(validator.Rules["STD"] is List<OWLEnums.OWLValidatorRules> stdRules && stdRules.Count == 1);
            Assert.IsTrue(validator.Rules.ContainsKey("CTM"));
            Assert.IsTrue(validator.Rules["CTM"] is List<OWLValidatorRule> ctmRules && ctmRules.Count == 1);
            Assert.IsNotNull(validator.Extensions);
            Assert.IsTrue(validator.Extensions.Count == 0);
        }

        [TestMethod]
        public void ShouldValidateWithStandardRule()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);

            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateWithStandardRuleAndSubscribedEvents()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 3 evidences") > -1);
        }

        [TestMethod]
        public void ShouldValidateWithCustomRule()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLOntology ontology = new OWLOntology("ex:ont");

            OWLValidator validator = new OWLValidator();
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateWithStandardAndCustomRules()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateWithStandardAndCustomRulesAndSubscribedEvents()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 4 evidences") > -1);
        }

        [TestMethod]
        public async Task ShouldValidateWithStandardRuleAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);

            OWLValidatorReport validatorReport = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public async Task ShouldValidateWithStandardRuleAndSubscribedEventsAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLValidatorReport validatorReport = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 3 evidences") > -1);
        }

        [TestMethod]
        public async Task ShouldValidateWithCustomRuleAsync()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLOntology ontology = new OWLOntology("ex:ont");

            OWLValidator validator = new OWLValidator();
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            OWLValidatorReport validatorReport = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public async Task ShouldValidateWithStandardAndCustomRulesAsync()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            OWLValidatorReport validatorReport = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public async Task ShouldValidateWithStandardAndCustomRulesAndSubscribedEventsAsync()
        {
            OWLValidatorReport CustomValidatorRule(OWLOntology ontology)
                => new OWLValidatorReport().AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, nameof(CustomValidatorRule), "test message", "test suggestion"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator();
            validator.AddRule(OWLEnums.OWLValidatorRules.TermDisjointness);
            validator.AddRule(new OWLValidatorRule("testRule", "this is test rule", CustomValidatorRule));

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLValidatorReport validatorReport = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 4 evidences") > -1);
        }
        #endregion
    }
}