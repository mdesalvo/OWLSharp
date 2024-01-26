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

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLDifferentIndividualConsistencyRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateDifferentIndividualConsistency()
        {
            OWLOntologyData data1 = new OWLOntologyData();
            data1.DeclareIndividual(new RDFResource("ex:indiv1"));
            data1.DeclareIndividual(new RDFResource("ex:indiv2"));
            data1.DeclareAllDifferentIndividuals(new RDFResource("ex:adi12"), new List<RDFResource>() {
                new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2") });
            OWLOntologyData data2 = new OWLOntologyData();
            data2.DeclareIndividual(new RDFResource("ex:indiv1"));
            data2.DeclareIndividual(new RDFResource("ex:indiv2"));
            data2.DeclareSameIndividuals(new RDFResource("ex:indiv2"), new RDFResource("ex:indiv1"));

            data1.Merge(data2);

            OWLValidatorReport validatorReport = OWLDifferentIndividualConsistencyRule.ExecuteRule(new OWLOntology("ex:org") { Data = data1 });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateDifferentIndividualConsistencyViaValidator()
        {
            OWLOntologyData data1 = new OWLOntologyData();
            data1.DeclareIndividual(new RDFResource("ex:indiv1"));
            data1.DeclareIndividual(new RDFResource("ex:indiv2"));
            data1.DeclareDifferentIndividuals(new RDFResource("ex:indiv2"),new RDFResource("ex:indiv1"));
            OWLOntologyData data2 = new OWLOntologyData();
            data2.DeclareIndividual(new RDFResource("ex:indiv1"));
            data2.DeclareIndividual(new RDFResource("ex:indiv2"));
            data2.DeclareSameIndividuals(new RDFResource("ex:indiv1"),new RDFResource("ex:indiv2"));

            data1.Merge(data2);

            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.DifferentIndividualConsistency);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(new OWLOntology("ex:org") { Data = data1 });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}