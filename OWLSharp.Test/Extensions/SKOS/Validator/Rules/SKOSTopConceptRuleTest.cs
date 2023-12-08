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
    public class SKOSTopConceptRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateTopConcept()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareConcept(new RDFResource("ex:concept"));
            conceptScheme.DeclareConcept(new RDFResource("ex:rootConcept"));
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:concept"), new RDFResource("ex:rootConcept"));
            conceptScheme.DeclareTopConcept(new RDFResource("ex:concept")); //clash on skos:broader taxonomy

            OWLValidatorReport validatorReport = SKOSTopConceptRule.ExecuteRule(conceptScheme);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateTopConceptViaValidator()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareConcept(new RDFResource("ex:concept"));
            conceptScheme.DeclareConcept(new RDFResource("ex:rootConcept"));
            conceptScheme.DeclareBroaderConcepts(new RDFResource("ex:concept"), new RDFResource("ex:rootConcept"));
            conceptScheme.DeclareTopConcept(new RDFResource("ex:concept")); //clash on skos:broader taxonomy

            SKOSValidator validator = new SKOSValidator().AddStandardRule(SKOSEnums.SKOSValidatorStandardRules.TopConcept);
            OWLValidatorReport validatorReport = validator.ApplyToConceptScheme(conceptScheme);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }
        #endregion
    }
}