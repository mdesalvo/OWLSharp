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
    public class OWLReasonerRuleSameAsAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSameAsAtomWithVariableAsRightArgument()
        {
            OWLReasonerRuleSameAsAtom SameAsAtom = new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(SameAsAtom);
            Assert.IsNotNull(SameAsAtom.Predicate);
            Assert.IsTrue(SameAsAtom.Predicate.Equals(RDFVocabulary.OWL.SAME_AS));
            Assert.IsNotNull(SameAsAtom.LeftArgument);
            Assert.IsTrue(SameAsAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(SameAsAtom.RightArgument);
            Assert.IsTrue(SameAsAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsFalse(SameAsAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("sameAs(?L,?R)", SameAsAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateSameAsAtomWithResourceAsRightArgument()
        {
            OWLReasonerRuleSameAsAtom SameAsAtom = new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), new RDFResource("ex:val"));

            Assert.IsNotNull(SameAsAtom);
            Assert.IsNotNull(SameAsAtom.Predicate);
            Assert.IsTrue(SameAsAtom.Predicate.Equals(RDFVocabulary.OWL.SAME_AS));
            Assert.IsNotNull(SameAsAtom.LeftArgument);
            Assert.IsTrue(SameAsAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(SameAsAtom.RightArgument);
            Assert.IsTrue(SameAsAtom.RightArgument.Equals(new RDFResource("ex:val")));
            Assert.IsFalse(SameAsAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("sameAs(?L,ex:val)", SameAsAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameAsAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleSameAsAtom(null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameAsAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameAsAtomBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnAntecedentWithVariableAsRightArgument()
        {
            OWLReasonerRuleSameAsAtom SameAsAtom = new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));
            
            DataTable antecedentTable = SameAsAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnAntecedentWithResourceAsRightArgument()
        {
            OWLReasonerRuleSameAsAtom SameAsAtom = new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), new RDFResource("ex:indiv2"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv3"));

            DataTable antecedentTable = SameAsAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnConsequentWithVariableAsRightArgument()
        {
            OWLReasonerRuleSameAsAtom SameAsAtom = new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv","ex:indiv2");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));

            OWLReasonerReport report = SameAsAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldEvaluateSameAsAtomOnConsequentWithResourceAsRightArgument()
        {
            OWLReasonerRuleSameAsAtom SameAsAtom = new OWLReasonerRuleSameAsAtom(new RDFVariable("?L"), new RDFResource("ex:val1"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));

            OWLReasonerReport report = SameAsAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 2);
        }
        #endregion
    }
}