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

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLHasValueEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteHasValueResourceEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestriction"), new RDFResource("ex:hvRestrictionProperty"), new RDFResource("ex:hvRestrictionValue"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class"), new RDFResource("ex:hvRestriction"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hvRestrictionProperty"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:hvRestrictionValue"));

            OWLReasonerReport reasonerReport = OWLHasValueEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldExecuteHasValueLiteralEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestriction"), new RDFResource("ex:hvRestrictionProperty"), new RDFPlainLiteral("hvRestrictionValue"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class"), new RDFResource("ex:hvRestriction"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:hvRestrictionProperty"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasonerReport reasonerReport = OWLHasValueEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldExecuteHasValueEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestriction"), new RDFResource("ex:hvRestrictionProperty"), new RDFResource("ex:hvRestrictionValue"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class"), new RDFResource("ex:hvRestriction"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:hvRestrictionProperty"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:hvRestrictionValue"));

            OWLReasoner reasoner = new OWLReasoner().AddStandardRule(OWLEnums.OWLReasonerStandardRules.HasValueEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }
        #endregion
    }
}