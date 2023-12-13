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
    public class OWLHasSelfEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteHasSelfEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareHasSelfRestriction(new RDFResource("ex:hsRestriction"), new RDFResource("ex:hsRestrictionProperty"), true);
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class"), new RDFResource("ex:hsRestriction"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hsRestrictionProperty"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasonerReport reasonerReport = OWLHasSelfEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldExecuteHasSelfEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareHasSelfRestriction(new RDFResource("ex:hsRestriction"), new RDFResource("ex:hsRestrictionProperty"), true);
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class"), new RDFResource("ex:hsRestriction"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hsRestrictionProperty"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasoner reasoner = new OWLReasoner().AddRule(OWLEnums.OWLReasonerRules.HasSelfEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }
        #endregion
    }
}