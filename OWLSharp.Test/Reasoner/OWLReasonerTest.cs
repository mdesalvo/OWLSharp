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
            Assert.IsNotNull(reasoner.StandardRules);
            Assert.IsTrue(reasoner.StandardRules.Count == 0);
            Assert.IsNotNull(reasoner.CustomRules);
            Assert.IsTrue(reasoner.CustomRules.Count == 0);
        }

        [TestMethod]
        public void ShouldAddStandarReasonerRule()
        {
            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddStandardRule(OWLEnums.OWLReasonerStandardRules.SubClassTransitivity);
            reasoner.AddStandardRule(OWLEnums.OWLReasonerStandardRules.SubClassTransitivity); //Will be discarded, since duplicate standard rules are not allowed

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.StandardRules);
            Assert.IsTrue(reasoner.StandardRules.Count == 1);
            Assert.IsNotNull(reasoner.CustomRules);
            Assert.IsTrue(reasoner.CustomRules.Count == 0);
        }

        [TestMethod]
        public void ShouldAddCustomReasonerRule()
        {
            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddCustomRule(new SWRLRule("testRule", "description",
                new SWRLAntecedent(), new SWRLConsequent()));

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.StandardRules);
            Assert.IsTrue(reasoner.StandardRules.Count == 0);
            Assert.IsNotNull(reasoner.CustomRules);
            Assert.IsTrue(reasoner.CustomRules.Count == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingCustomReasonerRuleBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new OWLReasoner().AddCustomRule(null));

        [TestMethod]
        public void ShouldReasonWithStandardRule()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddStandardRule(OWLEnums.OWLReasonerStandardRules.SubClassTransitivity);

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldReasonWithStandardRuleAndSubscribedEvents()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddStandardRule(OWLEnums.OWLReasonerStandardRules.SubClassTransitivity);

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 1 evidences") > -1);
        }

        [TestMethod]
        public void ShouldReasonWithCustomRule()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddCustomRule(new SWRLRule("testRule", "this is test rule",
                new SWRLAntecedent().AddAtom(new SWRLClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"))),
                new SWRLConsequent().AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL))));

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldReasonWithCustomRuleAndSubscribedEvents()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddCustomRule(new SWRLRule("testRule", "this is test rule",
                new SWRLAntecedent().AddAtom(new SWRLClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"))),
                new SWRLConsequent().AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL))));

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 1 evidences") > -1);
        }

        [TestMethod]
        public void ShouldReasonWithStandardAndCustomRules()
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
            reasoner.AddStandardRule(OWLEnums.OWLReasonerStandardRules.SubClassTransitivity);
            reasoner.AddCustomRule(new SWRLRule("testRule", "this is test rule",
                new SWRLAntecedent().AddAtom(new SWRLClassAtom(new RDFResource("ex:classA"), new RDFVariable("?C"))),
                new SWRLConsequent().AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL))));

            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public async Task ShouldReasonWithStandardAndCustomRulesAsync()
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
            reasoner.AddStandardRule(OWLEnums.OWLReasonerStandardRules.SubClassTransitivity);
            reasoner.AddCustomRule(new SWRLRule("testRule", "this is test rule",
                new SWRLAntecedent().AddAtom(new SWRLClassAtom(new RDFResource("ex:classA"), new RDFVariable("?C"))),
                new SWRLConsequent().AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL))));

            OWLReasonerReport reasonerReport = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}