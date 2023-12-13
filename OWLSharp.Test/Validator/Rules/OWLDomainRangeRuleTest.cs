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
    public class OWLDomainRangeRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateDomainRange()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:class1"), Range = new RDFResource("ex:class2") });
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"), new OWLOntologyDatatypePropertyBehavior() { Domain = new RDFResource("ex:class1") });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("lit"));

            OWLValidatorReport validatorReport = OWLDomainRangeRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateDomainRangeViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"), new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("ex:class1"), Range = new RDFResource("ex:class2") });
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"), new OWLOntologyDatatypePropertyBehavior() { Domain = new RDFResource("ex:class1") });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("lit"));

            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.DomainRange);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 3);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}