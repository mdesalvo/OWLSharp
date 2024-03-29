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

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLInverseInverseFunctionalEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteInverseFunctionalEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:invfuncobjprop1"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:invfuncobjprop2"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:functdtprop"), new OWLOntologyDatatypePropertyBehavior() { Functional = true });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivD"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivE"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indivA"),new RDFResource("ex:indivE"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indivB"),new RDFResource("ex:indivE"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:invfuncobjprop1"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivB"), new RDFResource("ex:invfuncobjprop1"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivE"), new RDFResource("ex:invfuncobjprop1"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:invfuncobjprop2"), new RDFResource("ex:indivE"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivB"), new RDFResource("ex:invfuncobjprop2"), new RDFResource("ex:indivD"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:functdtprop"), new RDFPlainLiteral("value"));

            OWLReasonerReport reasonerReport = OWLInverseFunctionalEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteInverseFunctionalEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:invfuncobjprop1"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:invfuncobjprop2"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:functdtprop"), new OWLOntologyDatatypePropertyBehavior() { Functional = true });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivD"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivE"));
            ontology.Data.DeclareAllDifferentIndividuals(new RDFResource("ex:alldiffAE"), [
                new RDFResource("ex:indivA"),new RDFResource("ex:indivE")]);
            ontology.Data.DeclareAllDifferentIndividuals(new RDFResource("ex:alldiffBE"), [
                new RDFResource("ex:indivB"),new RDFResource("ex:indivE")]);
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:invfuncobjprop1"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivB"), new RDFResource("ex:invfuncobjprop1"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivE"), new RDFResource("ex:invfuncobjprop1"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:invfuncobjprop2"), new RDFResource("ex:indivE"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivB"), new RDFResource("ex:invfuncobjprop2"), new RDFResource("ex:indivD"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:functdtprop"), new RDFPlainLiteral("value"));

            OWLReasoner reasoner = new OWLReasoner().AddRule(OWLEnums.OWLReasonerRules.InverseFunctionalEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}