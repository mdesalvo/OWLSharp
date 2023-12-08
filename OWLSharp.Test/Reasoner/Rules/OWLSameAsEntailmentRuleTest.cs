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

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLSameAsEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteSameAsEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:objprop"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivC"), new RDFResource("ex:objprop"), new RDFResource("ex:indivA"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("value"));

            OWLReasonerReport reasonerReport = OWLSameAsEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 3);
        }

        [TestMethod]
        public void ShouldExecuteSameAsEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:objprop"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indivC"), new RDFResource("ex:objprop"), new RDFResource("ex:indivA"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("value"));

            OWLReasoner reasoner = new OWLReasoner().AddStandardRule(OWLEnums.OWLReasonerStandardRules.SameAsEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 3);
        }
        #endregion
    }
}