/*
   Copyright 2012-2023 Marco De Salvo

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
using System.IO;
using System.Linq;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLIndividualTypeEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExecuteIndividualTypeEntailmentOnSimpleClass()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class3"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class2"), new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class2"), new RDFResource("ex:class3"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class3"));

            OWLReasonerReport reasonerReport = OWLIndividualTypeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteIndividualTypeEntailmentOnRestrictionClass()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestriction"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2"));

            OWLReasonerReport reasonerReport = OWLIndividualTypeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public void ShouldExecuteIndividualTypeEntailmentOnCompositeClass()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:simpleClass"));
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("ex:hvRestriction"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2"));
            ontology.Model.ClassModel.DeclareUnionClass(new RDFResource("ex:unionClass"), new List<RDFResource>() { new RDFResource("ex:simpleClass"), new RDFResource("ex:hvRestriction") });
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objprop"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv2"), new RDFResource("ex:simpleClass"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2"));

            OWLReasonerReport reasonerReport = OWLIndividualTypeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 3); // 2 evidences from union class, 1 evidence from restriction class
        }

        [TestMethod]
        public void ShouldExecuteIndividualTypeEntailmentOnEnumerateClass()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareEnumerateClass(new RDFResource("ex:enumerateClass"), new List<RDFResource>() { new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2") });
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv1"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv2"));

            OWLReasonerReport reasonerReport = OWLIndividualTypeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        [TestMethod]
        public void ShouldExecuteIndividualTypeEntailmentViaReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class2"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class3"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:class2"), new RDFResource("ex:class1"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("ex:class2"), new RDFResource("ex:class3"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class3"));

            OWLReasoner reasoner = new OWLReasoner().AddStandardRule(OWLEnums.OWLReasonerStandardRules.IndividualTypeEntailment);
            OWLReasonerReport reasonerReport = reasoner.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 2);
        }

        //E2E: FEAT#25 (COVERING BUG#16)
        [TestMethod]
        public void ShouldCorrectlyWorkUnderOWAForCardinalitiesByNotEmittingUndesiredTypeAssignments()
        {
            string ontString =
@"
@prefix ibx: <http://my.test/ItemBox#>.
@prefix owl: <http://www.w3.org/2002/07/owl#>.
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.
@prefix xsd: <http://www.w3.org/2001/XMLSchema#>.
@base <https://rdfsharp.codeplex.com/>.

_:75c93d010f994c8381f3124b0198d989 a owl:Class, owl:Restriction; 
                                   owl:onClass ibx:Item; 
                                   owl:onProperty ibx:contains; 
                                   owl:qualifiedCardinality ""0""^^xsd:nonNegativeInteger. 
_:8b3ab2d1013849668697f581defd5754 a owl:Class, owl:Restriction; 
                                   owl:onClass ibx:Item; 
                                   owl:onProperty ibx:contains; 
                                   owl:qualifiedCardinality ""1""^^xsd:nonNegativeInteger. 
<http://my.test/ItemBox> a owl:Ontology. 
ibx:Box a owl:Class; 
        rdfs:subClassOf ibx:Corporeal. 
ibx:contains a owl:ObjectProperty; 
             rdfs:domain ibx:Box; 
             rdfs:range ibx:Item. 
ibx:Corporeal a owl:Class. 
ibx:Item a owl:Class; 
         rdfs:subClassOf ibx:Corporeal. 
ibx:theBox0 a owl:NamedIndividual, ibx:Box, _:75c93d010f994c8381f3124b0198d989. 
ibx:theBox1 a owl:NamedIndividual, ibx:Box, _:8b3ab2d1013849668697f581defd5754. 
ibx:theItem a owl:NamedIndividual, ibx:Item. ";

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(ontString);
                    writer.Flush();
                    stream.Position = 0;

                    using (RDFGraph graph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, stream))
                    {
                        using (OWLOntology ontology = OWLOntology.FromRDFGraph(graph)) 
                        {
                            OWLReasoner reasoner = new OWLReasoner().AddStandardRule(OWLEnums.OWLReasonerStandardRules.IndividualTypeEntailment);
                            OWLReasonerReport report = reasoner.ApplyToOntology(ontology);

                            Assert.IsNotNull(report);
                            Assert.IsTrue(report.EvidencesCount == 3);
                            Assert.IsTrue(report.Any(evd => evd.EvidenceContent.Equals(new RDFTriple(
                                new RDFResource("http://my.test/ItemBox#theBox0"), RDFVocabulary.RDF.TYPE, new RDFResource("http://my.test/ItemBox#Corporeal")))));
                            Assert.IsTrue(report.Any(evd => evd.EvidenceContent.Equals(new RDFTriple(
                                new RDFResource("http://my.test/ItemBox#theBox1"), RDFVocabulary.RDF.TYPE, new RDFResource("http://my.test/ItemBox#Corporeal")))));
                            Assert.IsTrue(report.Any(evd => evd.EvidenceContent.Equals(new RDFTriple(
                                new RDFResource("http://my.test/ItemBox#theItem"), RDFVocabulary.RDF.TYPE, new RDFResource("http://my.test/ItemBox#Corporeal")))));
                        }
                    } 
                } 
            }
        }
        #endregion
    }
}