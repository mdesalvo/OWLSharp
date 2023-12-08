/*
   Copyright 2012-2024 Marco De Salvo

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
    public class OWLTermDeclarationRuleTest
    {
        #region Tests

        // CLASSMODEL

        [TestMethod]
        public void ShouldValidateTermDeclaration_SubClassOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class1"), new RDFResource("ex:class2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_SubClassOf_ViaValidator()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class1"), new RDFResource("ex:class2"));

            OWLValidator validator = new OWLValidator().AddStandardRule(OWLEnums.OWLValidatorStandardRules.TermDeclaration);
            OWLValidatorReport validatorReport = validator.ApplyToOntology(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_EquivalentClass()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class1"), new RDFResource("ex:class2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4); //Consider also the automatic inferences
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 4);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_DisjointWith()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("ex:class1"), new RDFResource("ex:class2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4); //Consider also the automatic inferences
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 4);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_OneOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:enumerateClass"), RDFVocabulary.OWL.ONE_OF, new RDFResource("bnode:representative")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:individual1")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_OneOfLiteral()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:enumeratelitClass"), RDFVocabulary.OWL.ONE_OF, new RDFResource("bnode:representative")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.FIRST, new RDFPlainLiteral("hello")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_UnionOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:unionClass"), RDFVocabulary.OWL.UNION_OF, new RDFResource("bnode:representative")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_IntersectionOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:intersectionClass"), RDFVocabulary.OWL.INTERSECTION_OF, new RDFResource("bnode:representative")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_ComplementOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.OWL.COMPLEMENT_OF, new RDFResource("ex:class2")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_HasKey()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:class"), RDFVocabulary.OWL.HAS_KEY, new RDFResource("bnode:representative")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop1")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_DisjointUnionOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:disjointUnionClass"), RDFVocabulary.OWL.DISJOINT_UNION_OF, new RDFResource("bnode:representative")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_AllDisjointClasses()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:allDisjointClasses"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:allDisjointClasses"), RDFVocabulary.OWL.MEMBERS, new RDFResource("bnode:representative")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("bnode:representative"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_AllValuesFrom()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.OWL.ALL_VALUES_FROM, new RDFResource("ex:class2")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_SomeValuesFrom()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.OWL.SOME_VALUES_FROM, new RDFResource("ex:class2")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_HasValue()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:class"), RDFVocabulary.OWL.HAS_VALUE, new RDFResource("ex:individual")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_HasSelf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:class"), RDFVocabulary.OWL.HAS_SELF, RDFTypedLiteral.True));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_OnClass()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:qRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:class")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_OnProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:restr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:prop")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        // PROPERTYMODEL

        [TestMethod]
        public void ShouldValidateTermDeclaration_SubPropertyOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("ex:property1"), new RDFResource("ex:property2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_EquivalentProperty()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("ex:property1"), new RDFResource("ex:property2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4); //Consider also the automatic inferences
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 4);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_PropertyDisjointWith()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDisjointProperties(new RDFResource("ex:property1"), new RDFResource("ex:property2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4); //Consider also the automatic inferences
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 4);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_InverseOf()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("ex:property1"), new RDFResource("ex:property2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4); //Consider also the automatic inferences
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 4);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_PropertyChainAxiom()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("ex:propertyChainAxiom"), new List<RDFResource>() { new RDFResource("ex:objprop1") });

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1); //ChainAxiom is automatically declared as owl:ObjectProperty
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_AllDisjointProperties()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("ex:allDisjointProperties"), new List<RDFResource>() { new RDFResource("ex:class1") });

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_Domain()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:property"), RDFVocabulary.RDFS.DOMAIN, new RDFResource("ex:class")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_Range()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.TBoxGraph.AddTriple(new RDFTriple(new RDFResource("ex:property"), RDFVocabulary.RDFS.RANGE, new RDFResource("ex:class")));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        // DATA

        [TestMethod]
        public void ShouldValidateTermDeclaration_Type()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareIndividualType(new RDFResource("ex:individual"), new RDFResource("ex:class"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_SameAs()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:individual1"), new RDFResource("ex:individual2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4); //Consider also the automatic inferences
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 4);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_DifferentFrom()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareDifferentIndividuals(new RDFResource("ex:individual1"), new RDFResource("ex:individual2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 4); //Consider also the automatic inferences
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 4);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_AllDifferent()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareAllDifferentIndividuals(new RDFResource("ex:allDifferent"), new List<RDFResource>() { new RDFResource("ex:individual1") });

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_ObjectAssertion()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:individual1"), new RDFResource("ex:objprop"), new RDFResource("ex:individual2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_DatatypeAssertion()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("ex:dtprop"));
            ontology.Data.DeclareDatatypeAssertion(new RDFResource("ex:individual1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("value"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 1);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 1);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_NegativeObjectAssertion()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareNegativeObjectAssertion(new RDFResource("ex:individual1"), new RDFResource("ex:objprop"), new RDFResource("ex:individual2"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 3);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 3);
        }

        [TestMethod]
        public void ShouldValidateTermDeclaration_NegativeDatatypeAssertion()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:individual1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("value"));

            OWLValidatorReport validatorReport = OWLTermDeclarationRule.ExecuteRule(ontology);

            Assert.IsNotNull(validatorReport);
            Assert.IsTrue(validatorReport.EvidencesCount == 2);
            Assert.IsTrue(validatorReport.SelectErrors().Count == 0);
            Assert.IsTrue(validatorReport.SelectWarnings().Count == 2);
        }
        #endregion
    }
}