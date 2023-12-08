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

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLLocalCardinalityRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateLocalCardinality()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on direct transitivity

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateLocalCardinalityOnSuperProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:superobjprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:objprop"), new RDFResource("ex:superobjprop"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on indirect transitivity

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateLocalCardinalityOnInverseProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:inverseobjprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:objprop"), new RDFResource("ex:inverseobjprop"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on indirect transitivity

            OWLValidatorReport validatorReport = OWLLocalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateLocalCardinalityViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:objprop"), 2); //clash on direct transitivity


            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.LocalCardinality);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}