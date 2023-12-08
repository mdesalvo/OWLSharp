/*
   Copyright 2012-2024 Marco De Salvo

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
using RDFSharp.Query;
using System.Data;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerRuleConsequentTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateConsequent()
        {
            OWLReasonerRuleConsequent consequent = new OWLReasonerRuleConsequent();

            Assert.IsNotNull(consequent);
            Assert.IsNotNull(consequent.Atoms);
            Assert.IsTrue(consequent.Atoms.Count == 0);
        }

        [TestMethod]
        public void ShouldAddAtoms()
        {
            OWLReasonerRuleConsequent consequent = new OWLReasonerRuleConsequent()
                .AddAtom(new OWLReasonerRuleClassAtom(new RDFResource("ex:class"), new RDFVariable("?C")))
                .AddAtom(new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?C"), new RDFVariable("?I")));

            Assert.IsNotNull(consequent);
            Assert.IsNotNull(consequent.Atoms);
            Assert.IsTrue(consequent.Atoms.Count == 2);
            Assert.IsTrue(string.Equals("ex:class(?C) ^ ex:objprop(?C,?I)", consequent.ToString()));
        }

        [TestMethod]
        public void ShouldEvaluate()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasonerRuleConsequent consequent = new OWLReasonerRuleConsequent()
                .AddAtom(new OWLReasonerRuleObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL));

            Assert.IsTrue(string.Equals("type(?C,Individual)", consequent.ToString()));

            OWLReasonerReport report = consequent.Evaluate(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 1);
        }
        #endregion
    }
}