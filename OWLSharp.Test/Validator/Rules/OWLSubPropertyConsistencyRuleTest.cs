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
using System.Collections.Generic;

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLSubPropertyConsistencyRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateSubPropertyConsistencyFailingOnSubProperty()
        {
            OWLOntologyPropertyModel propertymodel1 = new OWLOntologyPropertyModel();
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel1.DeclareSubProperties(new RDFResource("ex:objprop2"),new RDFResource("ex:objprop1"));
            OWLOntologyPropertyModel propertymodel2 = new OWLOntologyPropertyModel();
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel2.DeclareSubProperties(new RDFResource("ex:objprop1"),new RDFResource("ex:objprop2"));

            propertymodel1.Merge(propertymodel2);

            OWLValidatorReport validatorReport = OWLSubPropertyConsistencyRule.ExecuteRule(new OWLOntology("ex:org") { 
                Model = new OWLOntologyModel() { PropertyModel = propertymodel1 } });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateSubPropertyConsistencyFailingOnEquivalentProperty()
        {
            OWLOntologyPropertyModel propertymodel1 = new OWLOntologyPropertyModel();
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel1.DeclareSubProperties(new RDFResource("ex:objprop2"),new RDFResource("ex:objprop1"));
            OWLOntologyPropertyModel propertymodel2 = new OWLOntologyPropertyModel();
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel2.DeclareEquivalentProperties(new RDFResource("ex:objprop1"),new RDFResource("ex:objprop2"));

            propertymodel1.Merge(propertymodel2);

            OWLValidatorReport validatorReport = OWLSubPropertyConsistencyRule.ExecuteRule(new OWLOntology("ex:org") { 
                Model = new OWLOntologyModel() { PropertyModel = propertymodel1 } });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 4);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateSubPropertyConsistencyFailingOnDisjointProperty()
        {
            OWLOntologyPropertyModel propertymodel1 = new OWLOntologyPropertyModel();
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel1.DeclareSubProperties(new RDFResource("ex:objprop2"),new RDFResource("ex:objprop1"));
            OWLOntologyPropertyModel propertymodel2 = new OWLOntologyPropertyModel();
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel2.DeclareAllDisjointProperties(new RDFResource("ex:adj12"), new List<RDFResource>() { 
                new RDFResource("ex:objprop1"),new RDFResource("ex:objprop2") });

            propertymodel1.Merge(propertymodel2);

            OWLValidatorReport validatorReport = OWLSubPropertyConsistencyRule.ExecuteRule(new OWLOntology("ex:org") { 
                Model = new OWLOntologyModel() { PropertyModel = propertymodel1 } });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateSubPropertyConsistencyViaValidator()
        {
            OWLOntologyPropertyModel propertymodel1 = new OWLOntologyPropertyModel();
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel1.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel1.DeclareSubProperties(new RDFResource("ex:objprop2"),new RDFResource("ex:objprop1"));
            OWLOntologyPropertyModel propertymodel2 = new OWLOntologyPropertyModel();
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop1"));
            propertymodel2.DeclareObjectProperty(new RDFResource("ex:objprop2"));
            propertymodel2.DeclareSubProperties(new RDFResource("ex:objprop1"),new RDFResource("ex:objprop2"));

            propertymodel1.Merge(propertymodel2);

            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.SubPropertyConsistency);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(new OWLOntology("ex:org") { 
                Model = new OWLOntologyModel() { PropertyModel = propertymodel1 } });

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}