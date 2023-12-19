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

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSValidatorTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAddSKOSRule()
        {
            OWLValidator validator = new OWLValidator();
            validator.AddSKOSRule(SKOSEnums.SKOSValidatorRules.TopConcept);
            validator.AddSKOSRule(SKOSEnums.SKOSValidatorRules.TopConcept); //Will be discarded, since duplicate rules are not allowed

            Assert.IsNotNull(validator);
            Assert.IsTrue(validator.Rules.Count == 3);
            Assert.IsTrue(validator.Rules.ContainsKey("STD"));
            Assert.IsTrue(validator.Rules["STD"] is List<OWLEnums.OWLValidatorRules> stdRules && stdRules.Count == 0);
            Assert.IsTrue(validator.Rules.ContainsKey("CTM"));
            Assert.IsTrue(validator.Rules["CTM"] is List<OWLValidatorRule> ctmRules && ctmRules.Count == 0);
            Assert.IsTrue(validator.Rules.ContainsKey("SKOS"));
            Assert.IsTrue(validator.Rules["SKOS"] is List<SKOSEnums.SKOSValidatorRules> skosRules && skosRules.Count == 1);
            Assert.IsNotNull(validator.Extensions);
            Assert.IsTrue(validator.Extensions.Count == 1);
        }

        [TestMethod]
        public void ShouldValidateWithSKOSRule()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rootConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:concept"), new RDFResource("ex:rootConcept"));
            ontology.DeclareTopConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme")); //clash on skos:broader taxonomy

            OWLValidator validator = new OWLValidator();
            validator.AddSKOSRule(SKOSEnums.SKOSValidatorRules.TopConcept);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateWithSKOSRuleAndSubscribedEvents()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.DeclareConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:rootConcept"), new RDFResource("ex:conceptScheme"));
            ontology.DeclareBroaderConcepts(new RDFResource("ex:concept"), new RDFResource("ex:rootConcept"));
            ontology.DeclareTopConcept(new RDFResource("ex:concept"), new RDFResource("ex:conceptScheme")); //clash on skos:broader taxonomy

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLValidator validator = new OWLValidator();
            validator.AddSKOSRule(SKOSEnums.SKOSValidatorRules.TopConcept);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 1 evidences") > -1);
        }
        #endregion
    }
}