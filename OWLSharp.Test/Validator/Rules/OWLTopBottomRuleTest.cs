/*
   Copyright 2012-2023 Marco De Salvo

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
    public class OWLTopBottomRuleTest
    {
        #region Tests

        [TestMethod]
        public void ShouldValidateTopObject()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY, RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:SuperObjectProperty")));

            OWLValidatorReport validatorReport = OWLTopBottomRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateTopObject_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY, RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:SuperObjectProperty")));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.TopBottom);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateTopData()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(RDFVocabulary.OWL.TOP_DATA_PROPERTY, RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:SuperDataProperty")));

            OWLValidatorReport validatorReport = OWLTopBottomRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateTopData_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(RDFVocabulary.OWL.TOP_DATA_PROPERTY, RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:SuperDataProperty")));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.TopBottom);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateBottomObject()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:SubBottomObject"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY));

            OWLValidatorReport validatorReport = OWLTopBottomRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateBottomObject_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:SubBottomObject"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.TopBottom);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateBottomData()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:SubBottomData"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY));

            OWLValidatorReport validatorReport = OWLTopBottomRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateBottomData_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:SubBottomData"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.TopBottom);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}