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
    public class TIMEIntervalInRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateIntervalInFailingOnBefore()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Before); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnAfter()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnDisjoint()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Disjoint); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnEquals()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Equals); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnFinishedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.FinishedBy); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnMeets()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Meets); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnMetBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.MetBy); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnOverlappedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.OverlappedBy); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnOverlaps()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.Overlaps); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInFailingOnStartedBy()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.StartedBy); //clash on time:IntervalIn

            OWLValidatorReport validatorReport = TIMEIntervalInRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateIntervalInViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInterval(new RDFResource("ex:ACentury"),
                new TIMEInterval(new RDFResource("ex:ACenturyINTV"),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVBegin"), new DateTime(1992, 01, 01)),
                new TIMEInstant(new RDFResource("ex:ACenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareInterval(new RDFResource("ex:BCentury"),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV"),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVBegin"), new DateTime(1990, 01, 01)),
                new TIMEInstant(new RDFResource("ex:BCenturyINTVEnd"), new DateTime(1999, 12, 31))));
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.In);
            ontology.DeclareIntervalRelation(new TIMEInterval(new RDFResource("ex:ACenturyINTV")),
                new TIMEInterval(new RDFResource("ex:BCenturyINTV")), TIMEEnums.TIMEIntervalRelation.After); //clash on time:IntervalIn

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalIn);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}