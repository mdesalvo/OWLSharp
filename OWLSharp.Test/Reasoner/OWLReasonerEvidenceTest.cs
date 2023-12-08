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

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerEvidenceTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReasonerEvidence()
        {
            OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                "testRule", new RDFTriple(new RDFResource("ex:subj/"), new RDFResource("ex:pred/"), new RDFResource("ex:obj/")));

            Assert.IsNotNull(evidence);
            Assert.IsTrue(evidence.EvidenceCategory == OWLEnums.OWLReasonerEvidenceCategory.ClassModel);
            Assert.IsTrue(string.Equals(evidence.EvidenceRule, "testRule"));
            Assert.IsTrue(evidence.EvidenceContent.Equals(new RDFTriple(new RDFResource("ex:subj/"), new RDFResource("ex:pred/"), new RDFResource("ex:obj/"))));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingReasonerEvidenceBecauseNullOrEmptyRule()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel, 
                    null, new RDFTriple(new RDFResource("ex:subj/"), new RDFResource("ex:pred/"), new RDFResource("ex:obj/"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingReasonerEvidenceBecauseNullContent()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel, "testRule", null));
        #endregion
    }
}