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
using System.Collections.Generic;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLDifferentFromEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteDifferentFromEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivD"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivB"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivC"), new RDFResource("ex:indivD"));

            OWLReasonerReport reasonerReport = OWLDifferentFromEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 4);
        }

        [TestMethod]
        public void ShouldExecuteDifferentFromEntailmentWithAllDifferent()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivD"));
            ontology.Data.DeclareAllDifferentIndividuals(new RDFResource("exx:allDifferent"),
                new List<RDFResource>() { new RDFResource("ex:indivA"), new RDFResource("ex:indivB") });
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivB"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivC"), new RDFResource("ex:indivD"));

            OWLReasonerReport reasonerReport = OWLDifferentFromEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 6);
        }

        [TestMethod]
        public void ShouldExecuteDifferentFromEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivC"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indivD"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivB"), new RDFResource("ex:indivC"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indivC"), new RDFResource("ex:indivD"));

            OWLReasoner reasoner = new OWLReasoner().AddStandardRule(OWLEnums.OWLReasonerStandardRules.DifferentFromEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 4);
        }
        #endregion
    }
}