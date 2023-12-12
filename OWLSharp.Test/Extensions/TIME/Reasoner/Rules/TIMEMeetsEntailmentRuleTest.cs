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

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEMeetsEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteMeetsEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval1"), new TIMEInterval(new RDFResource("ex:Interval1Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval2"), new TIMEInterval(new RDFResource("ex:Interval2Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval3"), new TIMEInterval(new RDFResource("ex:Interval3Name")));
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval1Name")), new TIMEInterval(new RDFResource("ex:Interval2Name")), TIMEEnums.TIMEIntervalRelation.Meets);
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval2Name")), new TIMEInterval(new RDFResource("ex:Interval3Name")), TIMEEnums.TIMEIntervalRelation.Starts);
            OWLReasonerReport reasonerReport = TIMEMeetsEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldExecuteMeetsEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval1"), new TIMEInterval(new RDFResource("ex:Interval1Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval2"), new TIMEInterval(new RDFResource("ex:Interval2Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval3"), new TIMEInterval(new RDFResource("ex:Interval3Name")));
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval1Name")), new TIMEInterval(new RDFResource("ex:Interval2Name")), TIMEEnums.TIMEIntervalRelation.Meets);
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval2Name")), new TIMEInterval(new RDFResource("ex:Interval3Name")), TIMEEnums.TIMEIntervalRelation.Starts);

            OWLReasoner reasoner = new OWLReasoner().AddExtensionRule(OWLEnums.OWLReasonerExtensionRules.TIME_MeetsEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }
        #endregion
    }
}