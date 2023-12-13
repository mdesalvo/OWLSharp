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

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLClassTypeRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldValidateClassType()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:class1"), new RDFResource("ex:class2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class2")); //clash with class1 because owl:disjointWith
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));

            OWLValidatorReport validatorReport = OWLClassTypeRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod]
        public void ShouldValidateClassTypeViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:class1"), new RDFResource("ex:class2"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class2")); //clash with class1 because owl:disjointWith
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));

            OWLValidator validator = new OWLValidator().AddRule(OWLEnums.OWLValidatorRules.ClassType);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 2);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }

        [TestMethod, Timeout(5000)] // checking 30k of individuals should not take longer than 5 seconds
        public void ShouldCompleteQuickly()
        {
            RDFResource class1 = new RDFResource("ex:class1");
            RDFResource class2 = new RDFResource("ex:class2");

            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(class1);
            ontology.Model.ClassModel.DeclareClass(class2);
            ontology.Model.ClassModel.DeclareDisjointClasses(class1, class2);            
            for (int i = 0; i < 30000; i++)
            {
                RDFResource individual = new RDFResource($"ex:indiv{i}");
                ontology.Data.Individuals.Add(individual.PatternMemberID, individual);
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(individual, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL));
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(individual, RDFVocabulary.RDF.TYPE, class1));
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(individual, RDFVocabulary.RDF.TYPE, class2));
            }

            OWLValidatorReport validatorReport = OWLClassTypeRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 60000);
        }

        [TestMethod]
        public void ShouldValidateClassTypeClashingOnComplement()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareComplementClass(new RDFResource("ex:cclass1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:class1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv1"), new RDFResource("ex:cclass1")); //clash with class1 because owl:complementOf
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:class1"));

            OWLValidatorReport validatorReport = OWLClassTypeRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 1);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 0);
        }
        #endregion
    }
}