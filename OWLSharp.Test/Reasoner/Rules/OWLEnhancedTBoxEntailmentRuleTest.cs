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
    public class OWLEnhancedTBoxEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteEnhancedTBoxEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:DescendantOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:ParentOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:ParentOf"), new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("bnode:inlineOP"), new RDFResource("ex:DescendantOf"));

            OWLReasonerReport reasonerReport = OWLEnhancedTBoxEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteEnhancedTBoxEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:DescendantOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:ParentOf"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:ParentOf"), new RDFResource("bnode:inlineOP"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("bnode:inlineOP"), new RDFResource("ex:DescendantOf"));

            OWLReasoner reasoner = new OWLReasoner().AddRule(OWLEnums.OWLReasonerRules.EnhancedTBoxEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }
        #endregion
    }
}