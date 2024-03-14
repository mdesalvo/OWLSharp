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
    public class TIMEIntervalBeforeRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnBefore()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW2INTV")),
                new TIMEInterval(new RDFResource("ex:WW1INTV")), TIMEEnums.TIMEIntervalRelation.Before); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnAfter()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnContains()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Contains); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnDuring()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.During); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnEquals()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Equals); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnFinishedBy()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.FinishedBy); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnFinishes()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Finishes); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnHasInside()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.HasInside); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnIn()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.In); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnMeets()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Meets); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnMetBy()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.MetBy); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnNotDisjoint()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.NotDisjoint); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnOverlappedBy()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.OverlappedBy); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnOverlaps()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Overlaps); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnStartedBy()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.StartedBy); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeFailingOnStarts()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Starts); //clash on time:intervalBefore

            OWLValidatorReport validatorReport = TIMEIntervalBeforeRule.ExecuteRule(ontology, [new RDFResource("ex:WW2INTV"), new RDFResource("ex:WW1INTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalBeforeViaValidator()
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
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.Before);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:WW1INTV")),
                new TIMEInterval(new RDFResource("ex:WW2INTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:intervalBefore

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalBefore);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}