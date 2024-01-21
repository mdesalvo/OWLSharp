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
using System;
using System.Collections.Generic;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEIntervalAfterRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnAfter()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnBefore()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Before); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnContains()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Contains); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnDuring()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.During); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnEquals()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Equals); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnFinishedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.FinishedBy); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnFinishes()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Finishes); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnHasInside()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.HasInside); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnIn()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.In); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnMeets()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Meets); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnMetBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.MetBy); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnNotDisjoint()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.NotDisjoint); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnOverlappedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.OverlappedBy); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnOverlaps()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Overlaps); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnStartedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.StartedBy); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterFailingOnStarts()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Starts); //clash on time:intervalAfter

            OWLValidatorReport validatorReport = TIMEIntervalAfterRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalAfterViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:WW2"),
                new TIMEInterval(new RDFResource("ex:WW2INTV"),
                new TIMEInstant(new RDFResource("ex:WW2INTVBegin"), new DateTime(1939, 09, 01)),
                new TIMEInstant(new RDFResource("ex:WW2INTVEnd"), new DateTime(1945, 09, 02))));
            ontology.DeclareInterval(new RDFResource("ex:WW1"),
                new TIMEInterval(new RDFResource("ex:WW1INTV"),
                new TIMEInstant(new RDFResource("ex:WW1INTVBegin"), new DateTime(1914, 07, 28)),
                new TIMEInstant(new RDFResource("ex:WW1INTVEnd"), new DateTime(1918, 11, 11))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.After);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:intervalAfter

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalAfter);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}