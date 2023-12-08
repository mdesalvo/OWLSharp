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
using System.Threading.Tasks;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerReportTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReasonerReport()
        {
            OWLReasonerReport report = new OWLReasonerReport();

            Assert.IsNotNull(report);
            Assert.IsNotNull(report.Evidences);
            Assert.IsTrue(report.EvidencesCount == 0);

            int i = 0;
            IEnumerator<OWLReasonerEvidence> evidencesEnumerator = report.EvidencesEnumerator;
            while (evidencesEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 0);
        }

        [TestMethod]
        public void ShouldAddReasonerEvidence()
        {
            OWLReasonerReport report = new OWLReasonerReport();
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, 
                "rulename", new RDFTriple(new RDFResource("ex:subj/"), new RDFResource("ex:pred/"), new RDFResource("ex:obj/"))));
            report.AddEvidence(null); //Will be discarded, since null evidences are not allowed

            Assert.IsTrue(report.EvidencesCount == 1);

            int i = 0;
            IEnumerator<OWLReasonerEvidence> evidencesEnumerator = report.EvidencesEnumerator;
            while (evidencesEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldMergeValidatorEvidences()
        {
            OWLReasonerReport report1 = new OWLReasonerReport();
            report1.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                "rulename", new RDFTriple(new RDFResource("ex:subj/"), new RDFResource("ex:pred/"), new RDFResource("ex:obj/"))));
            OWLReasonerReport report2 = new OWLReasonerReport();
            report2.MergeEvidences(report1);

            Assert.IsTrue(report2.EvidencesCount == 1);

            int i = 0;
            IEnumerator<OWLReasonerEvidence> evidencesEnumerator = report2.EvidencesEnumerator;
            while (evidencesEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 1);
        }

        [TestMethod]
        public void ShouldExportToGraph()
        {
            OWLReasonerReport report = new OWLReasonerReport();
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                "rulename", new RDFTriple(new RDFResource("ex:subj1/"), new RDFResource("ex:pred1/"), new RDFResource("ex:obj1/"))));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel,
                "rulename", new RDFTriple(new RDFResource("ex:subj2/"), new RDFResource("ex:pred2/"), new RDFResource("ex:obj2/"))));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                "rulename", new RDFTriple(new RDFResource("ex:subj3/"), new RDFResource("ex:pred3/"), new RDFResource("ex:obj3/"))));
            RDFGraph graph = report.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsync()
        {
            OWLReasonerReport report = new OWLReasonerReport();
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                "rulename", new RDFTriple(new RDFResource("ex:subj1/"), new RDFResource("ex:pred1/"), new RDFResource("ex:obj1/"))));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel,
                "rulename", new RDFTriple(new RDFResource("ex:subj2/"), new RDFResource("ex:pred2/"), new RDFResource("ex:obj2/"))));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                "rulename", new RDFTriple(new RDFResource("ex:subj3/"), new RDFResource("ex:pred3/"), new RDFResource("ex:obj3/"))));
            RDFGraph graph = await report.ToRDFGraphAsync();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
        }

        [TestMethod]
        public void ShouldJoinEvidences()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");

            OWLReasonerReport report = new OWLReasonerReport();
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                "rulename", new RDFTriple(new RDFResource("ex:indiv1/"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1/"))));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel,
                "rulename", new RDFTriple(new RDFResource("ex:objprop1/"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY)));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                "rulename", new RDFTriple(new RDFResource("ex:class1/"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            report.JoinEvidences(ontology);

            Assert.IsTrue(ontology.Model.ClassModel.TBoxGraph.TriplesCount == 1);
            Assert.IsTrue(ontology.Model.PropertyModel.TBoxGraph.TriplesCount == 1);
            Assert.IsTrue(ontology.Data.ABoxGraph.TriplesCount == 1);
        }

        [TestMethod]
        public async Task ShouldJoinEvidencesAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");

            OWLReasonerReport report = new OWLReasonerReport();
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                "rulename", new RDFTriple(new RDFResource("ex:indiv1/"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1/"))));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel,
                "rulename", new RDFTriple(new RDFResource("ex:objprop1/"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY)));
            report.AddEvidence(new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                "rulename", new RDFTriple(new RDFResource("ex:class1/"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS)));
            await report.JoinEvidencesAsync(ontology);

            Assert.IsTrue(ontology.Model.ClassModel.TBoxGraph.TriplesCount == 1);
            Assert.IsTrue(ontology.Model.PropertyModel.TBoxGraph.TriplesCount == 1);
            Assert.IsTrue(ontology.Data.ABoxGraph.TriplesCount == 1);
        }
        #endregion
    }
}