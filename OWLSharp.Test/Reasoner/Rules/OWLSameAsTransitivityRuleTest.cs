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
    public class OWLSameAsTransitivityRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteSameAsTransitivity()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivD"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivB"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivC"), new RDFResource("ex:indivD"));

            OWLReasonerReport reasonerReport = OWLSameAsTransitivityRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 6);
        }

        [TestMethod]
        public void ShouldExecuteSameAsTransitivityViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivD"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivB"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivC"), new RDFResource("ex:indivD"));

            OWLReasoner reasoner = new OWLReasoner().AddRule(OWLEnums.OWLReasonerRules.SameAsTransitivity);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 6);
        }
        #endregion
    }
}