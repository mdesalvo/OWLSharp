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
    public class TIMEIntervalOverlapsRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnBefore()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Before); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnAfter()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnContains()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Contains); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnDisjoint()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Disjoint); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnDuring()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.During); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnFinishedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.FinishedBy); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnFinishes()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnHasInside()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnIn()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnMeets()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Meets); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnMetBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.MetBy); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnOverlappedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.OverlappedBy); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnStartedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.StartedBy); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsFailingOnStarts()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Starts); //clash on time:intervalOverlaps

            OWLValidatorReport validatorReport = TIMEIntervalOverlapsRule.ExecuteRule(ontology, new List<RDFResource>() { new RDFResource("ex:ACenturyINTV"), new RDFResource("ex:BCenturyINTV") });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalOverlapsViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 06, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1998, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:intervalOverlaps

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalOverlaps);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}