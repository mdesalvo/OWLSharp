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

namespace OWLSharp.Extensions.SWRL.Test
{
    [TestClass]
    public class SWRLDifferentFromAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDifferentFromAtomWithVariableAsRightArgument()
        {
            SWRLDifferentFromAtom DifferentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(DifferentFromAtom);
            Assert.IsNotNull(DifferentFromAtom.Predicate);
            Assert.IsTrue(DifferentFromAtom.Predicate.Equals(RDFVocabulary.OWL.DIFFERENT_FROM));
            Assert.IsNotNull(DifferentFromAtom.LeftArgument);
            Assert.IsTrue(DifferentFromAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(DifferentFromAtom.RightArgument);
            Assert.IsTrue(DifferentFromAtom.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsFalse(DifferentFromAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("differentFrom(?L,?R)", DifferentFromAtom.ToString()));
        }

        [TestMethod]
        public void ShouldCreateDifferentFromAtomWithResourceAsRightArgument()
        {
            SWRLDifferentFromAtom DifferentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFResource("ex:val"));

            Assert.IsNotNull(DifferentFromAtom);
            Assert.IsNotNull(DifferentFromAtom.Predicate);
            Assert.IsTrue(DifferentFromAtom.Predicate.Equals(RDFVocabulary.OWL.DIFFERENT_FROM));
            Assert.IsNotNull(DifferentFromAtom.LeftArgument);
            Assert.IsTrue(DifferentFromAtom.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(DifferentFromAtom.RightArgument);
            Assert.IsTrue(DifferentFromAtom.RightArgument.Equals(new RDFResource("ex:val")));
            Assert.IsFalse(DifferentFromAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("differentFrom(?L,ex:val)", DifferentFromAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDifferentFromAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLDifferentFromAtom(null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDifferentFromAtomBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new SWRLDifferentFromAtom(new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDifferentFromAtomBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new SWRLDifferentFromAtom(new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnAntecedentWithVariableAsRightArgument()
        {
            SWRLDifferentFromAtom DifferentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));
            
            DataTable antecedentTable = DifferentFromAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnAntecedentWithResourceAsRightArgument()
        {
            SWRLDifferentFromAtom DifferentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFResource("ex:indiv2"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv3"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2"));
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv3"));

            DataTable antecedentTable = DifferentFromAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnConsequentWithVariableAsRightArgument()
        {
            SWRLDifferentFromAtom DifferentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFVariable("?R"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Columns.Add("?R");
            antecedentTable.Rows.Add("ex:indiv","ex:indiv2");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));

            OWLReasonerReport report = DifferentFromAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldEvaluateDifferentFromAtomOnConsequentWithResourceAsRightArgument()
        {
            SWRLDifferentFromAtom DifferentFromAtom = new SWRLDifferentFromAtom(new RDFVariable("?L"), new RDFResource("ex:val1"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?L");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));

            OWLReasonerReport report = DifferentFromAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 2);
        }
        #endregion
    }
}