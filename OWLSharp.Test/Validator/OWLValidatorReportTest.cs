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
using System.Collections.Generic;

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLValidatorReportTest
    {
        #region Test
        [TestMethod]
        public void ShouldCreateValidatorReport()
        {
            OWLValidatorReport report = new OWLValidatorReport();

            Assert.IsNotNull(report);
            Assert.IsNotNull(report.Evidences);
            Assert.IsTrue(report.EvidencesCount == 0);

            int i = 0;
            IEnumerator<OWLValidatorEvidence> evidencesEnumerator = report.EvidencesEnumerator;
            while (evidencesEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 0);
        }

        [TestMethod]
        public void ShouldAddValidatorEvidence()
        {
            OWLValidatorReport report = new OWLValidatorReport();
            report.AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Error, "rulename", "message", "suggestion"));
            report.AddEvidence(null); //Will be discarded, since null evidences are not allowed

            Assert.IsTrue(report.EvidencesCount == 1);

            int i = 0;
            IEnumerator<OWLValidatorEvidence> evidencesEnumerator = report.EvidencesEnumerator;
            while (evidencesEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldMergeValidatorEvidences()
        {
            OWLValidatorReport report1 = new OWLValidatorReport();
            report1.AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Error, "rulename", "message", "suggestion"));
            OWLValidatorReport report2 = new OWLValidatorReport();
            report2.MergeEvidences(report1);

            Assert.IsTrue(report2.EvidencesCount == 1);

            int i = 0;
            IEnumerator<OWLValidatorEvidence> evidencesEnumerator = report2.EvidencesEnumerator;
            while (evidencesEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldSelectWarningEvidences()
        {
            OWLValidatorReport report = new OWLValidatorReport();
            report.AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, "rulename", "message", "suggestion"));
            report.AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Error, "rulename", "message", "suggestion"));
            List<OWLValidatorEvidence> warnings = report.SelectWarnings();

            Assert.IsNotNull(warnings);
            Assert.IsTrue(warnings.Count == 1);
        }

        [TestMethod]
        public void ShouldSelectErrorEvidences()
        {
            OWLValidatorReport report = new OWLValidatorReport();
            report.AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Warning, "rulename", "message", "suggestion"));
            report.AddEvidence(new OWLValidatorEvidence(OWLEnums.OWLValidatorEvidenceCategory.Error, "rulename", "message", "suggestion"));
            List<OWLValidatorEvidence> errors = report.SelectErrors();

            Assert.IsNotNull(errors);
            Assert.IsTrue(errors.Count == 1);
        }
        #endregion
    }
}