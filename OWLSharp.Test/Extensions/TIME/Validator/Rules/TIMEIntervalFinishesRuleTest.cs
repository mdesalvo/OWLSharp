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

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEIntervalFinishesRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnBefore()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Before); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnAfter()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnContains()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Contains); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnDisjoint()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Disjoint); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnDuring()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.During); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnFinishedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.FinishedBy); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnHasInside()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnMeets()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Meets); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnMetBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.MetBy); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnOverlappedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.OverlappedBy); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnOverlaps()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnStartedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.StartedBy); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesFailingOnStarts()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Starts); //clash on time:IntervalFinishes

            OWLValidatorReport validatorReport = TIMEIntervalFinishesRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalFinishesViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1997, 08, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1997, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1997, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalFinishes

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalFinishes);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}