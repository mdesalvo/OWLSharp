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
    public class OWLReasonerRuleDataPropertyAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataPropertyAtomWithVariableAsRightArgument()
        {
            OWLReasonerRuleDataPropertyAtom DataPropertyAtom = new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(DataPropertyAtom);
            Assert.IsNotNull(DataPropertyAtom.Predicate);
            Assert.IsTrue(DataPropertyAtom.Predicate.Equals(new RDFResource("ex:dtprop")));
            Assert.IsNotNull(DataPropertyAtom.LeftArgument);
            Assert.IsTrue(DataPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(DataPropertyAtom.RightArgument);
            Assert.IsTrue(DataPropertyAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsFalse(DataPropertyAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("ex:dtprop(?L,?R)", DataPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateDataPropertyAtomWithLiteralAsRightArgument()
        {
            OWLReasonerRuleDataPropertyAtom DataPropertyAtom = new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), new RDFPlainLiteral("hello","en-US"));

            Assert.IsNotNull(DataPropertyAtom);
            Assert.IsNotNull(DataPropertyAtom.Predicate);
            Assert.IsTrue(DataPropertyAtom.Predicate.Equals(new RDFResource("ex:dtprop")));
            Assert.IsNotNull(DataPropertyAtom.LeftArgument);
            Assert.IsTrue(DataPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(DataPropertyAtom.RightArgument);
            Assert.IsTrue(DataPropertyAtom.RightArgument.Equals(new RDFPlainLiteral("hello", "en-US")));
            Assert.IsFalse(DataPropertyAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("ex:dtprop(?L,\"hello\"@EN-US)", DataPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleDataPropertyAtom(null, new RDFVariable("?L"), new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyAtomBecauseNullRightArgumentLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), null as RDFLiteral));

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnAntecedentWithVariableAsRightArgument()
        {
            OWLReasonerRuleDataPropertyAtom DataPropertyAtom = new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), new RDFVariable("?R"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("value"));
            
            DataTable antecedentTable = DataPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnAntecedentWithLiteralAsRightArgument()
        {
            OWLReasonerRuleDataPropertyAtom DataPropertyAtom = new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), new RDFPlainLiteral("hello","en-US"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("value"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:indiv"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("hello","en-US"));

            DataTable antecedentTable = DataPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnConsequentWithVariableAsRightArgument()
        {
            OWLReasonerRuleDataPropertyAtom DataPropertyAtom = new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv","value");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));

            OWLReasonerReport report = DataPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDataPropertyAtomOnConsequentWithLiteralAsRightArgument()
        {
            OWLReasonerRuleDataPropertyAtom DataPropertyAtom = new OWLReasonerRuleDataPropertyAtom(new RDFResource("ex:dtprop"), new RDFVariable("?L"), new RDFPlainLiteral("hello","en-US"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));

            OWLReasonerReport report = DataPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 1);
        }
        #endregion
    }
}