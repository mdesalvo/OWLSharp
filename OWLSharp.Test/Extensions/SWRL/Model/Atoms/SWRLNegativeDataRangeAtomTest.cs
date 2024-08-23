﻿/*
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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Data;

namespace OWLSharp.Test.Extensions.SWRL.Model.Atoms
{
    [TestClass]
    public class SWRLNegativeDataRangeAtomTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateNegativeDataRangeAtom()
        {
            SWRLNegativeDataRangeAtom datarangeAtom = new SWRLNegativeDataRangeAtom(
                new OWLDatatype(new RDFResource("ex:dtt")), 
                new RDFVariable("?D"));

            Assert.IsNotNull(datarangeAtom);
            Assert.IsNotNull(datarangeAtom.Predicate);
            Assert.IsTrue(datarangeAtom.Predicate.GetIRI().Equals(new RDFResource("ex:dtt")));
            Assert.IsNotNull(datarangeAtom.LeftArgument);
            Assert.IsTrue(datarangeAtom.LeftArgument.Equals(new RDFVariable("?D")));
            Assert.IsNull(datarangeAtom.RightArgument);
            Assert.IsTrue(string.Equals("not(ex:dtt(?D))", datarangeAtom.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeDataRangeAtomBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new SWRLDataRangeAtom(null, new RDFVariable("?C")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNegativeDataRangeAtomBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLDataRangeAtom(new OWLDatatype(new RDFResource("ex:dtt")), null));

        [TestMethod]
        public void ShouldEvaluateNegativeDataRangeAtomOnAntecedent()
        {
            SWRLNegativeDataRangeAtom dataRangeAtom = new SWRLNegativeDataRangeAtom(
                new OWLDatatype(RDFVocabulary.XSD.STRING), 
                new RDFVariable("?D"));

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dtprop"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv")))
                ],
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dtprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")),
                        new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dtprop")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")),
                        new OWLLiteral(new RDFTypedLiteral("44", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                ]
            };
            
            DataTable antecedentTable = dataRangeAtom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentTable);
            Assert.IsTrue(antecedentTable.Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldEvaluateNegativeDataRangeAtomOnConsequent()
        {
            SWRLNegativeDataRangeAtom dataRangeAtom = new SWRLNegativeDataRangeAtom(
                new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                new RDFVariable("?D"));

            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?D");
            antecedentTable.Rows.Add("44^^http://www.w3.org/2001/XMLSchema#integer");

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));

            List<OWLInference> inferences = dataRangeAtom.EvaluateOnConsequent(antecedentTable, ontology); //NegativeDataRangeAtom is NO-OP in consequents

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 0);
        }
        #endregion
    }
}