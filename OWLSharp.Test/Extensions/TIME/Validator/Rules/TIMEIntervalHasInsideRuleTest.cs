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
    public class TIMEIntervalHasInsideRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnBefore()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Before); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnAfter()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnDisjoint()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Disjoint); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnDuring()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.During); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnEquals()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnFinishes()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Finishes); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnIn()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnMeets()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Meets); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnMetBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.MetBy); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnOverlappedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.OverlappedBy); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnOverlaps()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideFailingOnStarts()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Starts); //clash on time:IntervalHasInside

            OWLValidatorReport validatorReport = TIMEIntervalHasInsideRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalHasInsideViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1900, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.HasInside);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalHasInside

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalHasInside);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}