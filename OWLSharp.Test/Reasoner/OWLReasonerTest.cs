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
using OWLSharp.Extensions.SWRL;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReasoner()
        {
            OWLReasoner reasoner = new OWLReasoner();

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.Rules);
            Assert.IsTrue(reasoner.Rules.Count == 1);
            Assert.IsTrue(reasoner.Rules.ContainsKey("STD"));
            Assert.IsTrue(reasoner.Rules["STD"] is List<OWLEnums.OWLReasonerRules> stdRules && stdRules.Count == 0);
            Assert.IsNotNull(reasoner.Extensions);
            Assert.IsTrue(reasoner.Extensions.Count == 0);
        }

        [TestMethod]
        public void ShouldAddRule()
        {
            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddRule(OWLEnums.OWLReasonerRules.SubClassTransitivity)
                    .AddRule(OWLEnums.OWLReasonerRules.SubClassTransitivity); //Will be discarded, since duplicate standard rules are not allowed

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.Rules);
            Assert.IsTrue(reasoner.Rules.Count == 1);
            Assert.IsTrue(reasoner.Rules.ContainsKey("STD"));
            Assert.IsTrue(reasoner.Rules["STD"] is List<OWLEnums.OWLReasonerRules> stdRules && stdRules.Count == 1);
            Assert.IsNotNull(reasoner.Extensions);
            Assert.IsTrue(reasoner.Extensions.Count == 0);
        }

        [TestMethod]
        public void ShouldReasonWithRule()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddRule(OWLEnums.OWLReasonerRules.SubClassTransitivity);

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldReasonWithRuleAndSubscribedEvents()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddRule(OWLEnums.OWLReasonerRules.SubClassTransitivity);

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 1 evidences") > -1);
        }

        [TestMethod]
        public void ShouldReasonWithMixedRules()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:classA"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddRule(OWLEnums.OWLReasonerRules.SubClassTransitivity);
            reasoner.AddSWRLRule(new SWRLRule("testRule", "this is test rule",
                new SWRLAntecedent().AddAtom(new SWRLClassAtom(new RDFResource("ex:classA"), new RDFVariable("?C"))),
                new SWRLConsequent().AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL))));
            reasoner.AddSWRLRule(null); //Will be discarded, since null is not allowed

            Assert.IsNotNull(reasoner.Rules);
            Assert.IsTrue(reasoner.Rules.Count == 2);
            Assert.IsTrue(reasoner.Rules.ContainsKey("STD"));
            Assert.IsTrue(reasoner.Rules["STD"] is List<OWLEnums.OWLReasonerRules> stdRules && stdRules.Count == 1);
            Assert.IsTrue(reasoner.Rules.ContainsKey("SWRL"));
            Assert.IsTrue(reasoner.Rules["SWRL"] is List<SWRLRule> swrlRules && swrlRules.Count == 1);
            Assert.IsNotNull(reasoner.Extensions);
            Assert.IsTrue(reasoner.Extensions.Count == 1);
            Assert.IsTrue(reasoner.Extensions.ContainsKey("SWRL"));

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public async Task ShouldReasonWithMixedRulesAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:classA"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddRule(OWLEnums.OWLReasonerRules.SubClassTransitivity);
            reasoner.AddSWRLRule(new SWRLRule("testRule", "this is test rule",
                new SWRLAntecedent().AddAtom(new SWRLClassAtom(new RDFResource("ex:classA"), new RDFVariable("?C"))),
                new SWRLConsequent().AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL))));

            OWLReasonerReport reasonerReport = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}