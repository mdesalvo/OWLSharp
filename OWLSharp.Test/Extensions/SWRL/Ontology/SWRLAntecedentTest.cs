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
    public class SWRLAntecedentTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateAntecedent()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent();

            Assert.IsNotNull(antecedent);
            Assert.IsNotNull(antecedent.Atoms);
            Assert.IsTrue(antecedent.Atoms.Count == 0);
        }

        [TestMethod]
        public void ShouldAddAtom()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent()
                .AddAtom(new SWRLClassAtom(new RDFResource("ex:class"), new RDFVariable("?C")));

            Assert.IsNotNull(antecedent);
            Assert.IsNotNull(antecedent.Atoms);
            Assert.IsTrue(antecedent.Atoms.Count == 1);
            Assert.IsTrue(string.Equals("ex:class(?C)", antecedent.ToString()));
        }

        [TestMethod]
        public void ShouldAddBuiltIn()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent()
                .AddBuiltIn(new SWRLContainsBuiltIn(new RDFVariable("?C"), "ind"));

            Assert.IsNotNull(antecedent);
            Assert.IsNotNull(antecedent.Atoms);
            Assert.IsTrue(antecedent.Atoms.Count == 1);
            Assert.IsTrue(string.Equals("swrlb:contains(?C,\"ind\")", antecedent.ToString()));
        }

        [TestMethod]
        public void ShouldEvaluate()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class"));

            SWRLAntecedent antecedent = new SWRLAntecedent()
                .AddAtom(new SWRLClassAtom(new RDFResource("ex:class"), new RDFVariable("?C")))
                .AddBuiltIn(new SWRLContainsBuiltIn(new RDFVariable("?C"), "iv1"));

            Assert.IsTrue(string.Equals("ex:class(?C) ^ swrlb:contains(?C,\"iv1\")", antecedent.ToString()));

            DataTable results = antecedent.Evaluate(ontology);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Rows.Count == 1);
        }
        #endregion
    }
}