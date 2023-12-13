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
    public class SWRLClassAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateClassAtom()
        {
            SWRLClassAtom classAtom = new SWRLClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"));

            Assert.IsNotNull(classAtom);
            Assert.IsNotNull(classAtom.Predicate);
            Assert.IsTrue(classAtom.Predicate.Equals(new RDFResource("ex:class")));
            Assert.IsNotNull(classAtom.LeftArgument);
            Assert.IsTrue(classAtom.LeftArgument.Equals(new RDFVariable("?C")));
            Assert.IsNull(classAtom.RightArgument);
            Assert.IsFalse(classAtom.IsBuiltIn);
            Assert.IsTrue(string.Equals("ex:class(?C)", classAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new SWRLClassAtom(null, new RDFVariable("?C")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLClassAtom(new RDFResource("ex:class"), null));

        [TestMethod]
        public void ShouldEvaluateClassAtomOnAntecedent()
        {
            SWRLClassAtom classAtom = new SWRLClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"));

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class"));
            
            DataTable antecedentTable = classAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 2);
        }

        [TestMethod]
        public void ShouldEvaluateClassAtomOnConsequent()
        {
            SWRLClassAtom classAtom = new SWRLClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv");
            antecedentTable.Rows.Add("ex:indiv2");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasonerReport report = classAtom.EvaluateOnConsequent(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.EvidencesCount == 1);
        }
        #endregion
    }
}