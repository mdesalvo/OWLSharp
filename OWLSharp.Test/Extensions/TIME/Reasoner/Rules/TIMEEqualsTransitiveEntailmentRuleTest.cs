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
    public class TIMEEqualsTransitiveEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteEqualsTransitiveEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval1"), new TIMEInterval(new RDFResource("ex:Interval1Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval2"), new TIMEInterval(new RDFResource("ex:Interval2Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval3"), new TIMEInterval(new RDFResource("ex:Interval3Name")));
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval1Name")), new TIMEInterval(new RDFResource("ex:Interval2Name")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval2Name")), new TIMEInterval(new RDFResource("ex:Interval3Name")), TIMEEnums.TIMEIntervalRelation.Equals);
            OWLReasonerReport reasonerReport = TIMEEqualsTransitiveEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteEqualsTransitiveEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval1"), new TIMEInterval(new RDFResource("ex:Interval1Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval2"), new TIMEInterval(new RDFResource("ex:Interval2Name")));
            ontology.DeclareTimeInterval(new RDFResource("ex:Interval3"), new TIMEInterval(new RDFResource("ex:Interval3Name")));
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval1Name")), new TIMEInterval(new RDFResource("ex:Interval2Name")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareTimeIntervalRelation(new TIMEInterval(new RDFResource("ex:Interval2Name")), new TIMEInterval(new RDFResource("ex:Interval3Name")), TIMEEnums.TIMEIntervalRelation.Equals);

            OWLReasoner reasoner = new OWLReasoner().AddTIMERule(TIMEEnums.TIMEReasonerRules.TIME_EqualsTransitiveEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}