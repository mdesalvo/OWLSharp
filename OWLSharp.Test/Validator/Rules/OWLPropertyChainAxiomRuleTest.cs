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
using System.Collections.Generic;

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLPropertyChainAxiomRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidatePropertyChainAxiom()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:pchainaxiom"), [new RDFResource("ex:objprop")]);
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:pchainaxiom"), 2); //clash on cardinality restriction
            ontology.Model.ClassModel.DeclareHasSelfRestriction(new RDFResource("ex:selfrestr"), new RDFResource("ex:pchainaxiom"), true); //clash on self restriction

            OWLValidatorReport validatorReport = OWLPropertyChainAxiomRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidatePropertyChainAxiomOnObjectProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:pchainaxiom"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:pchainaxiom"), [new RDFResource("ex:objprop")]); //clash on definition being asymmetric

            OWLValidatorReport validatorReport = OWLPropertyChainAxiomRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidatePropertyChainAxiomViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:pchainaxiom"), [new RDFResource("ex:objprop")]);
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("ex:crestr"), new RDFResource("ex:pchainaxiom"), 2); //clash on cardinality restriction
            ontology.Model.ClassModel.DeclareHasSelfRestriction(new RDFResource("ex:selfrestr"), new RDFResource("ex:pchainaxiom"), true); //clash on self restriction


            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.PropertyChainAxiom);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}