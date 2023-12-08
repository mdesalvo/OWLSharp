/*
   Copyright 2012-2024 Marco De Salvo

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

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLTermDisjointnessRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateTermDisjointness()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidatorReport validatorReport = OWLTermDisjointnessRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateTermDisjointnessViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:entity"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:entity"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:entity"));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.TermDisjointness);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}