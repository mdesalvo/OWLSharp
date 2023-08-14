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
    public class OWLDisjointClassEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteDisjointClassEntailment()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classD"));
            ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));

            OWLReasonerReport reasonerReport = OWLDisjointClassEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 4);
        }

        [TestMethod]
        public void ShouldExecuteDisjointClassEntailmentWithAllDisjointClasses()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classD"));
            ontology.Model.ClassModel.DeclareAllDisjointClasses(new RDFResource("exx:allDisjointClasses"),
                new List<RDFResource>() { new RDFResource("ex:classA"), new RDFResource("ex:classB") });
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));

            OWLReasonerReport reasonerReport = OWLDisjointClassEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 6);
        }

        [TestMethod]
        public void ShouldExecuteDisjointClassEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:classD"));
            ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:classA"), new RDFResource("ex:classB"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classB"), new RDFResource("ex:classC"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:classC"), new RDFResource("ex:classD"));

            OWLReasoner reasoner = new OWLReasoner().AddStandardRule(OWLEnums.OWLReasonerStandardRules.DisjointClassEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 4);
        }
        #endregion
    }
}