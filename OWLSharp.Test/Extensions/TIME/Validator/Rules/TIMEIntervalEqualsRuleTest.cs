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
using RDFSharp.Model;
using System;
using System.Collections.Generic;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEIntervalEqualsRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnBefore()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Before); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnAfter()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnContains()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Contains); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnDisjoint()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Disjoint); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnDuring()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.During); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnFinishedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.FinishedBy); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnFinishes()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnHasInside()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnIn()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnMeets()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Meets); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnMetBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.MetBy); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnOverlappedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.OverlappedBy); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnOverlaps()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnStartedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.StartedBy); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsFailingOnStarts()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Starts); //clash on time:IntervalEquals

            OWLValidatorReport validatorReport = TIMEIntervalEqualsRule.ExecuteRule(ontology, [new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV")]);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalEqualsViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalEquals

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalEquals);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}