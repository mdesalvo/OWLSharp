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
    public class TIMEInstantAfterRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateInstantAfterFailingOnAfter()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInstant(new RDFResource("ex:WW2Begin"),
                new TIMEInstant(new RDFResource("ex:WW2INSTBegin"), new DateTime(1939, 09, 01)));
            ontology.DeclareInstant(new RDFResource("ex:WW2End"),
                new TIMEInstant(new RDFResource("ex:WW2INSTEnd"), new DateTime(1945, 09, 02)));
            ontology.DeclareInstantRelation(new TIMEInstant(new RDFResource("ex:WW2INSTEnd")),
                new TIMEInstant(new RDFResource("ex:WW2INSTBegin")), TIMEEnums.TIMEInstantRelation.After);
            ontology.DeclareInstantRelation(new TIMEInstant(new RDFResource("ex:WW2INSTBegin")),
                new TIMEInstant(new RDFResource("ex:WW2INSTEnd")), TIMEEnums.TIMEInstantRelation.After); //clash on time:after

            OWLValidatorReport validatorReport = TIMEInstantAfterRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateInstantAfterFailingOnBefore()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInstant(new RDFResource("ex:WW2Begin"),
                new TIMEInstant(new RDFResource("ex:WW2INSTBegin"), new DateTime(1939, 09, 01)));
            ontology.DeclareInstant(new RDFResource("ex:WW2End"),
                new TIMEInstant(new RDFResource("ex:WW2INSTEnd"), new DateTime(1945, 09, 02)));
            ontology.DeclareInstantRelation(new TIMEInstant(new RDFResource("ex:WW2INSTEnd")),
                new TIMEInstant(new RDFResource("ex:WW2INSTBegin")), TIMEEnums.TIMEInstantRelation.After);
            ontology.DeclareInstantRelation(new TIMEInstant(new RDFResource("ex:WW2INSTEnd")),
                new TIMEInstant(new RDFResource("ex:WW2INSTBegin")), TIMEEnums.TIMEInstantRelation.Before); //clash on time:after

            OWLValidatorReport validatorReport = TIMEInstantAfterRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateInstantAfterViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.InitializeTIME();
            ontology.DeclareInstant(new RDFResource("ex:WW2Begin"),
                new TIMEInstant(new RDFResource("ex:WW2INSTBegin"), new DateTime(1939, 09, 01)));
            ontology.DeclareInstant(new RDFResource("ex:WW2End"),
                new TIMEInstant(new RDFResource("ex:WW2INSTEnd"), new DateTime(1945, 09, 02)));
            ontology.DeclareInstantRelation(new TIMEInstant(new RDFResource("ex:WW2INSTEnd")),
                new TIMEInstant(new RDFResource("ex:WW2INSTBegin")), TIMEEnums.TIMEInstantRelation.After);
            ontology.DeclareInstantRelation(new TIMEInstant(new RDFResource("ex:WW2INSTBegin")),
                new TIMEInstant(new RDFResource("ex:WW2INSTEnd")), TIMEEnums.TIMEInstantRelation.After); //clash on time:after

            OWLValidator validator = new OWLValidator().AddTIMERule(TIMEEnums.TIMEValidatorRules.TIME_InstantAfter);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}