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
    public class TIMEValidatorTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAddTIMERule()
        {
            OWLValidator validator = new OWLValidator();
            validator.AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalAfter);
            validator.AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalAfter); //Will be discarded, since duplicate rules are not allowed

            Assert.IsNotNull(validator);
            Assert.IsTrue(validator.Rules.Count == 3);
            Assert.IsTrue(validator.Rules.ContainsKey("STD"));
            Assert.IsTrue(validator.Rules["STD"] is List<OWLEnums.OWLValidatorRules> stdRules && stdRules.Count == 0);
            Assert.IsTrue(validator.Rules.ContainsKey("CTM"));
            Assert.IsTrue(validator.Rules["CTM"] is List<OWLValidatorRule> ctmRules && ctmRules.Count == 0);
            Assert.IsTrue(validator.Rules.ContainsKey("TIME"));
            Assert.IsTrue(validator.Rules["TIME"] is List<TIMEEnums.TIMEValidatorRules> TIMERules && TIMERules.Count == 1);
            Assert.IsNotNull(validator.Extensions);
            Assert.IsTrue(validator.Extensions.Count == 1);
        }

        [TestMethod]
        public void ShouldValidateWithTIMERule()
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

            OWLValidator validator = new OWLValidator();
            validator.AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalAfter);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateWithTIMERuleAndSubscribedEvents()
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

            string warningMsg = null;
            OWLEvents.OnInfo += (string msg) => { warningMsg += msg; };

            OWLValidator validator = new OWLValidator();
            validator.AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_IntervalAfter);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("found 2 evidences") > -1);
        }
        #endregion
    }
}