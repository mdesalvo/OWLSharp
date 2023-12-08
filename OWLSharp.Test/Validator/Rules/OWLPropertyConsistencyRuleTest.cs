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
    public class OWLPropertyConsistencyRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidatePropertyConsistency()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:prop"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:prop"));
            ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:prop"));

            OWLValidatorReport validatorReport = OWLPropertyConsistencyRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidatePropertyConsistencyClashingOnHierarchy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop3"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop4"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop2"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop3"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop4"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop5"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop6"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:objprop1"), new RDFResource("ex:dtprop1"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:objprop2"), new RDFResource("ex:dtprop2"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:dtprop3"), new RDFResource("ex:objprop3"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:objprop4"), new RDFResource("ex:dtprop4"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:dtprop5"), new RDFResource("ex:dtprop6")); //will generate 2 evidences

            OWLValidatorReport validatorReport = OWLPropertyConsistencyRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 7);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 7);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidatePropertyConsistencyViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:prop"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:prop"));
            ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("ex:prop"));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.PropertyConsistency);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}