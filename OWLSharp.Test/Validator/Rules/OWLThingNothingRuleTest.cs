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
namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLThingNothingRuleTest
    {
        #region Tests

        [TestMethod]
        public void ShouldValidateThing()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(RDFVocabulary.OWL.THING, RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:SuperThing")));

            OWLValidatorReport validatorReport = OWLThingNothingRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateThing_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(RDFVocabulary.OWL.THING, RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:SuperThing")));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.ThingNothing);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNothing()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:SubNothing"), RDFVocabulary.RDFS.SUB_CLASS_OF, RDFVocabulary.OWL.NOTHING));

            OWLValidatorReport validatorReport = OWLThingNothingRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNothing_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:SubNothing"), RDFVocabulary.RDFS.SUB_CLASS_OF, RDFVocabulary.OWL.NOTHING));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.ThingNothing);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNothingIndividual()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.Classes.Add(RDFVocabulary.OWL.NOTHING.PatternMemberID, RDFVocabulary.OWL.NOTHING);
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:NothingIDV"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NOTHING));

            OWLValidatorReport validatorReport = OWLThingNothingRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateNothingIndividual_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.Classes.Add(RDFVocabulary.OWL.NOTHING.PatternMemberID, RDFVocabulary.OWL.NOTHING);
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:NothingIDV"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NOTHING));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.ThingNothing);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}