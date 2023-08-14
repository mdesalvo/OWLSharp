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
    public class OWLGlobalCardinalityRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateGlobalCardinalityOnFunctionalProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fobjprop"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:fobjprop"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:fobjprop"), new RDFResource("ex:indiv3")); //clash on fp occurrency counter
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:fobjprop"), new RDFResource("ex:indiv3"));

            OWLValidatorReport validatorReport = OWLGlobalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateGlobalCardinalityOnFunctionalTransitiveProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:ftobjprop"), new OWLOntologyObjectPropertyBehavior() { Functional = true, Transitive = true });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:ftobjprop"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:ftobjprop"), new RDFResource("ex:indiv3"));

            OWLValidatorReport validatorReport = OWLGlobalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateGlobalCardinalityOnSuperPropertiesOfFunctionalProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fobjprop"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:trobjprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:fobjprop"), new RDFResource("ex:trobjprop"));

            OWLValidatorReport validatorReport = OWLGlobalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateGlobalCardinalityOnInverseFunctionalProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:ifobjprop"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:ifobjprop"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv3"), new RDFResource("ex:ifobjprop"), new RDFResource("ex:indiv2")); //clash on ifp occurrency counter
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:ifobjprop"), new RDFResource("ex:indiv3"));

            OWLValidatorReport validatorReport = OWLGlobalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateGlobalCardinalityOnInverseFunctionalTransitiveProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:iftobjprop"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true, Transitive = true });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:iftobjprop"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:iftobjprop"), new RDFResource("ex:indiv3"));

            OWLValidatorReport validatorReport = OWLGlobalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateGlobalCardinalityOnSuperPropertiesOfInverseFunctionalProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:ifobjprop"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:trobjprop"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:ifobjprop"), new RDFResource("ex:trobjprop"));

            OWLValidatorReport validatorReport = OWLGlobalCardinalityRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateGlobalCardinalityViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:fobjprop"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:fobjprop"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:fobjprop"), new RDFResource("ex:indiv3")); //clash on fp occurrency counter
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv2"), new RDFResource("ex:fobjprop"), new RDFResource("ex:indiv3"));


            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.GlobalCardinality);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}