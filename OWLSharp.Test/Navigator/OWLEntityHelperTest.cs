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
using OWLSharp.Modeler;
using OWLSharp.Modeler.Axioms;
using OWLSharp.Modeler.Expressions;
using OWLSharp.Navigator;
using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Navigator
{
    [TestClass]
    public class OWLEntityHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetDeclaredClasses()
        {
            OWLOntology ont = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON))
                ]
            };
            List<RDFResource> cls = ont.GetDeclaredClasses();

            Assert.IsTrue(cls.Count == 3);
            Assert.IsTrue(cls.Any(c => c.Equals(RDFVocabulary.FOAF.AGENT)));
            Assert.IsTrue(cls.Any(c => c.Equals(RDFVocabulary.FOAF.ORGANIZATION)));
            Assert.IsTrue(cls.Any(c => c.Equals(RDFVocabulary.FOAF.PERSON)));

            Assert.IsTrue((null as OWLOntology).GetDeclaredClasses().Count == 0);
        }

        [TestMethod]
        public void ShouldGetDeclaredDatatypes()
        {
            OWLOntology ont = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.INTEGER)),
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.STRING)),
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.GEOSPARQL.WKT_LITERAL))
                ]
            };
            List<RDFResource> dts = ont.GetDeclaredDatatypes();

            Assert.IsTrue(dts.Count == 3);
            Assert.IsTrue(dts.Any(c => c.Equals(RDFVocabulary.XSD.INTEGER)));
            Assert.IsTrue(dts.Any(c => c.Equals(RDFVocabulary.XSD.STRING)));
            Assert.IsTrue(dts.Any(c => c.Equals(RDFVocabulary.GEOSPARQL.WKT_LITERAL)));

            Assert.IsTrue((null as OWLOntology).GetDeclaredDatatypes().Count == 0);
        }

        [TestMethod]
        public void ShouldGetDeclaredDataProperties()
        {
            OWLOntology ont = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.TITLE)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEO.LAT_LONG))
                ]
            };
            List<RDFResource> dps = ont.GetDeclaredDataProperties();

            Assert.IsTrue(dps.Count == 3);
            Assert.IsTrue(dps.Any(c => c.Equals(RDFVocabulary.FOAF.AGE)));
            Assert.IsTrue(dps.Any(c => c.Equals(RDFVocabulary.FOAF.TITLE)));
            Assert.IsTrue(dps.Any(c => c.Equals(RDFVocabulary.GEO.LAT_LONG)));

            Assert.IsTrue((null as OWLOntology).GetDeclaredDataProperties().Count == 0);
        }

        [TestMethod]
        public void ShouldGetDeclaredObjectProperties()
        {
            OWLOntology ont = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME))
                ]
            };
            List<RDFResource> ops = ont.GetDeclaredObjectProperties();

            Assert.IsTrue(ops.Count == 3);
            Assert.IsTrue(ops.Any(c => c.Equals(RDFVocabulary.FOAF.KNOWS)));
            Assert.IsTrue(ops.Any(c => c.Equals(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)));
            Assert.IsTrue(ops.Any(c => c.Equals(RDFVocabulary.TIME.HAS_TIME)));

            Assert.IsTrue((null as OWLOntology).GetDeclaredObjectProperties().Count == 0);
        }

        [TestMethod]
        public void ShouldGetDeclaredAnnotationProperties()
        {
            OWLOntology ont = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL))
                ]
            };
            List<RDFResource> aps = ont.GetDeclaredAnnotationProperties();

            Assert.IsTrue(aps.Count == 3);
            Assert.IsTrue(aps.Any(c => c.Equals(RDFVocabulary.DC.TITLE)));
            Assert.IsTrue(aps.Any(c => c.Equals(RDFVocabulary.RDFS.COMMENT)));
            Assert.IsTrue(aps.Any(c => c.Equals(RDFVocabulary.RDFS.LABEL)));

            Assert.IsTrue((null as OWLOntology).GetDeclaredAnnotationProperties().Count == 0);
        }

        [TestMethod]
        public void ShouldGetDeclaredNamedIndividuals()
        {
            OWLOntology ont = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV3")))
                ]
            };
            List<RDFResource> ids = ont.GetDeclaredNamedIndividuals();

            Assert.IsTrue(ids.Count == 3);
            Assert.IsTrue(ids.Any(c => c.Equals(new RDFResource("ex:IDV1"))));
            Assert.IsTrue(ids.Any(c => c.Equals(new RDFResource("ex:IDV2"))));
            Assert.IsTrue(ids.Any(c => c.Equals(new RDFResource("ex:IDV3"))));

            Assert.IsTrue((null as OWLOntology).GetDeclaredNamedIndividuals().Count == 0);
        }
        #endregion
    }
}