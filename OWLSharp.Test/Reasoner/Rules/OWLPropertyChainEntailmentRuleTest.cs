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
    public class OWLPropertyChainEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecutePropertyChainEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:hasUncle"), 
                new List<RDFResource>() { new RDFResource("ex:hasFather"), new RDFResource("ex:hasBrother") });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasFather"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasBrother"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:child"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:father"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:grandFather"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:uncle"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:grandUncle"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:child"), new RDFResource("ex:hasFather"), new RDFResource("ex:father"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:father"), new RDFResource("ex:hasBrother"), new RDFResource("ex:uncle"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:father"), new RDFResource("ex:hasFather"), new RDFResource("ex:grandFather"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:grandFather"), new RDFResource("ex:hasBrother"), new RDFResource("ex:grandUncle"));

            OWLReasonerReport reasonerReport = OWLPropertyChainEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecutePropertyChainEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:hasUncle"),
                new List<RDFResource>() { new RDFResource("ex:hasFather"), new RDFResource("ex:hasBrother") });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasFather"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hasBrother"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:child"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:father"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:grandFather"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:uncle"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:grandUncle"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:child"), new RDFResource("ex:hasFather"), new RDFResource("ex:father"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:father"), new RDFResource("ex:hasBrother"), new RDFResource("ex:uncle"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:father"), new RDFResource("ex:hasFather"), new RDFResource("ex:grandFather"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:grandFather"), new RDFResource("ex:hasBrother"), new RDFResource("ex:grandUncle"));

            OWLReasoner reasoner = new OWLReasoner().AddStandardRule(OWLEnums.OWLReasonerStandardRules.PropertyChainEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}