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

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSExactMatchEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteExactMatchEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:Concept1"), new RDFResource("ex:ConceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:Concept2"), new RDFResource("ex:ConceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:Concept3"), new RDFResource("ex:ConceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:Concept1"), new RDFResource("ex:Concept2"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:Concept2"), new RDFResource("ex:Concept3"));
            OWLReasonerReport reasonerReport = SKOSExactMatchEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteExactMatchEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeSKOS();
            ontology.DeclareConcept(new RDFResource("ex:Concept1"), new RDFResource("ex:ConceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:Concept2"), new RDFResource("ex:ConceptScheme"));
            ontology.DeclareConcept(new RDFResource("ex:Concept3"), new RDFResource("ex:ConceptScheme"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:Concept1"), new RDFResource("ex:Concept2"));
            ontology.DeclareExactMatchConcepts(new RDFResource("ex:Concept2"), new RDFResource("ex:Concept3"));

            OWLReasoner reasoner = new OWLReasoner().AddSKOSRule(SKOSEnums.SKOSReasonerRules.SKOS_ExactMatchEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}