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
using RDFSharp.Query;
using System.Data;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerRuleObjectPropertyAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectPropertyAtomWithVariableAsRightArgument()
        {
            OWLReasonerRuleObjectPropertyAtom ObjectPropertyAtom = new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(ObjectPropertyAtom);
            Assert.IsNotNull(ObjectPropertyAtom.Predicate);
            Assert.IsTrue(ObjectPropertyAtom.Predicate.Equals(new RDFResource("ex:objprop")));
            Assert.IsNotNull(ObjectPropertyAtom.LeftArgument);
            Assert.IsTrue(ObjectPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(ObjectPropertyAtom.RightArgument);
            Assert.IsTrue(ObjectPropertyAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsFalse(ObjectPropertyAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("ex:objprop(?L,?R)", ObjectPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateObjectPropertyAtomWithResourceAsRightArgument()
        {
            OWLReasonerRuleObjectPropertyAtom ObjectPropertyAtom = new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), new RDFResource("ex:val"));

            Assert.IsNotNull(ObjectPropertyAtom);
            Assert.IsNotNull(ObjectPropertyAtom.Predicate);
            Assert.IsTrue(ObjectPropertyAtom.Predicate.Equals(new RDFResource("ex:objprop")));
            Assert.IsNotNull(ObjectPropertyAtom.LeftArgument);
            Assert.IsTrue(ObjectPropertyAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(ObjectPropertyAtom.RightArgument);
            Assert.IsTrue(ObjectPropertyAtom.RightArgument.Equals(new RDFResource("ex:val")));
            Assert.IsFalse(ObjectPropertyAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("ex:objprop(?L,ex:val)", ObjectPropertyAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleObjectPropertyAtom(null, new RDFVariable("?L"), new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyAtomBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnAntecedentWithVariableAsRightArgument()
        {
            OWLReasonerRuleObjectPropertyAtom ObjectPropertyAtom = new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), new RDFVariable("?R"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv"), new RDFResource("ex:objprop"), new RDFResource("ex:val"));
            
            DataTable antecedentTable = ObjectPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnAntecedentWithResourceAsRightArgument()
        {
            OWLReasonerRuleObjectPropertyAtom ObjectPropertyAtom = new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), new RDFResource("ex:val1"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv"), new RDFResource("ex:objprop"), new RDFResource("ex:val1"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv"), new RDFResource("ex:objprop"), new RDFResource("ex:val2"));

            DataTable antecedentTable = ObjectPropertyAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnConsequentWithVariableAsRightArgument()
        {
            OWLReasonerRuleObjectPropertyAtom ObjectPropertyAtom = new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv","ex:val1");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));

            OWLReasonerReport report = ObjectPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldEvaluateObjectPropertyAtomOnConsequentWithResourceAsRightArgument()
        {
            OWLReasonerRuleObjectPropertyAtom ObjectPropertyAtom = new OWLReasonerRuleObjectPropertyAtom(new RDFResource("ex:objprop"), new RDFVariable("?L"), new RDFResource("ex:val1"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));

            OWLReasonerReport report = ObjectPropertyAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 1);
        }
        #endregion
    }
}