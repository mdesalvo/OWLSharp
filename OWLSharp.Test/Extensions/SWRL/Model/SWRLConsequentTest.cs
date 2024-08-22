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
using OWLSharp.Extensions.SWRL.Model.Atoms;
using OWLSharp.Extensions.SWRL.Model;
using OWLSharp.Ontology;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Extensions.SWRL.Model.BuiltIns;
using OWLSharp.Ontology.Axioms;
using System;
using System.Collections.Generic;
using OWLSharp.Reasoner;

namespace OWLSharp.Test.Extensions.SWRL.Model
{
    [TestClass]
    public class SWRLConsequentTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateEmptyConsequent()
        {
            SWRLConsequent consequent = new SWRLConsequent();

            Assert.IsNotNull(consequent);
            Assert.IsNotNull(consequent.Atoms);
            Assert.IsTrue(consequent.Atoms.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateConsequentWithAtoms()
        {
            SWRLConsequent consequent = new SWRLConsequent() {
                Atoms = [
                    new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), new RDFVariable("?C")),
                    new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("ex:objprop")), new RDFVariable("?C"), new RDFVariable("?I"))
                ]
            };

            Assert.IsNotNull(consequent);
            Assert.IsNotNull(consequent.Atoms);
            Assert.IsTrue(consequent.Atoms.Count == 2);
            Assert.IsTrue(string.Equals("ex:class(?C) ^ ex:objprop(?C,?I)", consequent.ToString()));
        }

        [TestMethod]
        public void ShouldEvaluateConsequent()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:class"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dtprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv"))),
                ]
            };

            SWRLConsequent consequent = new SWRLConsequent() {
                Atoms = [
                    new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:dtprop")), new RDFVariable("?C"), RDFTypedLiteral.True)
                ]
            };

            Assert.IsTrue(string.Equals("ex:dtprop(?C,\"true\"^^xsd:boolean)", consequent.ToString()));

            List<OWLInference> report = consequent.Evaluate(antecedentTable, ontology);

            Assert.IsNotNull(report);
            Assert.IsTrue(report.Count == 1);
        }
        #endregion
    }
}