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
using OWLSharp.Extensions.GEO;
using OWLSharp.Ontology;
using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Test.Extensions.GEO
{
    [TestClass]
    public class GEOHelperTest
    {
        #region Tests (Declarer)
        [TestMethod]
        public void ShouldDeclareGEOPointFeatureWithDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            GEOPoint geom = new GEOPoint(new RDFResource("ex:MilanGM"), (9.188540, 45.464664));
            ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), geom);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 2);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 2);

            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(null, geom));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), null));
        }

        [TestMethod]
        public void ShouldDeclareGEOPointFeatureWithNotDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            GEOPoint geom = new GEOPoint(new RDFResource("ex:MilanGM"), (9.188540, 45.464664));
            ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), geom, false);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 2);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 2);
        }

        [TestMethod]
        public void ShouldDeclareGEOLineFeatureWithDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            GEOLine geom = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664), (9.198540, 45.474664)]);
            ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), geom);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 2);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 2);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(null, geom));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), null));
        }

        [TestMethod]
        public void ShouldDeclareGEOLineFeatureWithNotDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            GEOLine geom = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664), (9.198540, 45.474664)]);
            ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), geom, false);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 2);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 2);
        }

        [TestMethod]
        public void ShouldDeclareGEOAreaFeatureWithDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            GEOArea geom = new GEOArea(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664), (9.198540, 45.474664), (9.208540, 45.484664)]); //will be automatically closed
            ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), geom);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 2);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 2);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(null, geom));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), null));
        }

        [TestMethod]
        public void ShouldDeclareGEOAreaFeatureWithNotDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            GEOArea geom = new GEOArea(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664), (9.198540, 45.474664), (9.208540, 45.484664)]); //will be automatically closed
            ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), geom, false);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 2);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 2);
        }
        #endregion

        #region Tests (Engine:Distance)
        [TestMethod]
        public async Task ShouldGetDistanceBetweenFeaturesAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral(@"<gml:Point xmlns:gml=""http://www.opengis.net/gml/3.2""><gml:pos>9.19193456 45.46420722</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral(@"<gml:Point xmlns:gml=""http://www.opengis.net/gml/3.2""><gml:pos>12.49221871 41.89033014</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                ]
            };
            double? milanRomeDistance = await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology, 
                new RDFResource("ex:milanFT"), new RDFResource("ex:romeFT"));

            Assert.IsTrue(milanRomeDistance >= 450000 && milanRomeDistance <= 4800000); //milan-rome should be between 450km and 480km
            
            //Unexisting features
            Assert.IsNull(await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                new RDFResource("ex:milanFT2"), new RDFResource("ex:romeFT")));
            Assert.IsNull(await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                new RDFResource("ex:milanFT"), new RDFResource("ex:romeFT2")));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async() => await GEOHelper.GetDistanceBetweenFeaturesAsync(null,
                new RDFResource("ex:milanFT"), new RDFResource("ex:romeFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                null as RDFResource, new RDFResource("ex:romeFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                new RDFResource("ex:milanFT"), null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetDistanceBetweenFeatureAndLiteralAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral(@"<gml:Point xmlns:gml=""http://www.opengis.net/gml/3.2""><gml:pos>9.19193456 45.46420722</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                ]
            };
            double? milanRomeDistance = await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                new RDFResource("ex:milanFT"), new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsTrue(milanRomeDistance >= 450000 && milanRomeDistance <= 4800000); //milan-rome should be between 450km and 480km

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                new RDFResource("ex:milanFT2"), new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(null,
                new RDFResource("ex:milanFT"), new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                null as RDFResource, new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                new RDFResource("ex:milanFT"), null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(geoOntology,
                new RDFResource("ex:milanFT"), new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public async Task ShouldGetDistanceBetweenLiteralsAsync()
        {
            double? milanRomeDistance = await GEOHelper.GetDistanceBetweenFeaturesAsync(
                new RDFTypedLiteral(@"<gml:Point xmlns:gml=""http://www.opengis.net/gml/3.2""><gml:pos>9.19193456 45.46420722</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML),
                new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsTrue(milanRomeDistance >= 450000 && milanRomeDistance <= 4800000); //milan-rome should be between 450km and 480km

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(
                null as RDFTypedLiteral, new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING),
                new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(
                new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetDistanceBetweenFeaturesAsync(
                new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }
        #endregion

        #region Tests (Engine:Measure)
        [TestMethod]
        public async Task ShouldGetLengthOfFeatureAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT")),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            double? milanLength = await GEOHelper.GetLengthOfFeatureAsync(geoOntology, new RDFResource("ex:milanFT"));
            double? brebemiLength = await GEOHelper.GetLengthOfFeatureAsync(geoOntology, new RDFResource("ex:brebemiFT"));

            Assert.IsTrue(milanLength >= 3000 && milanLength <= 3300); //Perimeter of milan is about 3KM
            Assert.IsTrue(brebemiLength >= 95000 && brebemiLength <= 100000); //BreBeMi is about 95-100KM

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetLengthOfFeatureAsync(geoOntology,
                new RDFResource("ex:milanFT2")));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetLengthOfFeatureAsync(null,
                new RDFResource("ex:milanFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetLengthOfFeatureAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetLengthOfLiteralAsync()
        {
            double? milanLength = await GEOHelper.GetLengthOfFeatureAsync(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            double ? brebemiLength = await GEOHelper.GetLengthOfFeatureAsync(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsTrue(milanLength >= 3000 && milanLength <= 3300); //Perimeter of milan is about 3KM
            Assert.IsTrue(brebemiLength >= 95000 && brebemiLength <= 100000); //BreBeMi is about 95-100KM

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetLengthOfFeatureAsync(
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetLengthOfFeatureAsync(
               new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public async Task ShouldGetAreaOfFeatureAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT")),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            double? milanArea = await GEOHelper.GetAreaOfFeatureAsync(geoOntology, new RDFResource("ex:milanFT"));
            double? brebemiArea = await GEOHelper.GetAreaOfFeatureAsync(geoOntology, new RDFResource("ex:brebemiFT"));

            Assert.IsTrue(milanArea >= 590000 && milanArea <= 600000);
            Assert.IsTrue(brebemiArea == 0);  //lines have no area

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetAreaOfFeatureAsync(geoOntology,
                new RDFResource("ex:milanFT2")));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetAreaOfFeatureAsync(null,
                new RDFResource("ex:milanFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetAreaOfFeatureAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetAreaOfLiteralAsync()
        {
            double? milanArea = await GEOHelper.GetAreaOfFeatureAsync(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            double? brebemiArea = await GEOHelper.GetAreaOfFeatureAsync(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsTrue(milanArea >= 590000 && milanArea <= 600000);
            Assert.IsTrue(brebemiArea == 0);  //lines have no area

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetAreaOfFeatureAsync(
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetAreaOfFeatureAsync(
               new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }
        #endregion

        #region Tests (Engine:Centroid)
        [TestMethod]
        public async Task ShouldGetCentroidOfFeatureAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT")),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            RDFTypedLiteral milanCentroid = await GEOHelper.GetCentroidOfFeatureAsync(geoOntology, new RDFResource("ex:milanFT"));
            RDFTypedLiteral brebemiCentroid = await GEOHelper.GetCentroidOfFeatureAsync(geoOntology, new RDFResource("ex:brebemiFT"));

            Assert.IsNotNull(milanCentroid);
            Assert.IsTrue(milanCentroid.Equals(new RDFTypedLiteral("POINT (9.18635964 45.46411499)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiCentroid);
            Assert.IsTrue(brebemiCentroid.Equals(new RDFTypedLiteral("POINT (9.66872097 45.59479136)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetCentroidOfFeatureAsync(geoOntology,
                new RDFResource("ex:milanFT2")));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetCentroidOfFeatureAsync(null,
                new RDFResource("ex:milanFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetCentroidOfFeatureAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetCentroidOfLiteralAsync()
        {
            RDFTypedLiteral milanCentroid = await GEOHelper.GetCentroidOfFeatureAsync(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiCentroid = await GEOHelper.GetCentroidOfFeatureAsync(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanCentroid);
            Assert.IsTrue(milanCentroid.Equals(new RDFTypedLiteral("POINT (9.18635964 45.46411499)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiCentroid);
            Assert.IsTrue(brebemiCentroid.Equals(new RDFTypedLiteral("POINT (9.66872097 45.59479136)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetCentroidOfFeatureAsync(
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetCentroidOfFeatureAsync(
               new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }
        #endregion

        #region Tests (Engine:Boundary)
        [TestMethod]
        public async Task ShouldGetBoundaryOfFeatureAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT")),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            RDFTypedLiteral milanBoundary = await GEOHelper.GetBoundaryOfFeatureAsync(geoOntology, new RDFResource("ex:milanFT"));
            RDFTypedLiteral brebemiBoundary = await GEOHelper.GetBoundaryOfFeatureAsync(geoOntology, new RDFResource("ex:brebemiFT"));

            Assert.IsNotNull(milanBoundary);
            Assert.IsTrue(milanBoundary.Equals(new RDFTypedLiteral("LINESTRING (9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBoundary);
            Assert.IsTrue(brebemiBoundary.Equals(new RDFTypedLiteral("MULTIPOINT ((9.16778508 45.46481222), (10.21423284 45.54758259))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetBoundaryOfFeatureAsync(geoOntology,
                new RDFResource("ex:milanFT2")));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBoundaryOfFeatureAsync(null,
                new RDFResource("ex:milanFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBoundaryOfFeatureAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetBoundaryOfLiteralAsync()
        {
            RDFTypedLiteral milanBoundary = await GEOHelper.GetBoundaryOfFeatureAsync(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiBoundary = await GEOHelper.GetBoundaryOfFeatureAsync(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanBoundary);
            Assert.IsTrue(milanBoundary.Equals(new RDFTypedLiteral("LINESTRING (9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBoundary);
            Assert.IsTrue(brebemiBoundary.Equals(new RDFTypedLiteral("MULTIPOINT ((9.16778508 45.46481222), (10.21423284 45.54758259))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBoundaryOfFeatureAsync(
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBoundaryOfFeatureAsync(
               new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }
        #endregion

        #region Tests (Engine:Buffer)
        [TestMethod]
        public async Task ShouldGetBufferAroundFeatureAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT")),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            RDFTypedLiteral milanBuffer = await GEOHelper.GetBufferAroundFeatureAsync(geoOntology, new RDFResource("ex:milanFT"), 5000);
            RDFTypedLiteral brebemiBuffer = await GEOHelper.GetBufferAroundFeatureAsync(geoOntology, new RDFResource("ex:brebemiFT"), 5000);

            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.12167581 45.47166215, 9.12272511 45.48045166, 9.12585291 45.48881321, 9.13095057 45.49645448, 9.13784041 45.50310824, 9.14628183 45.50854175, 9.15597971 45.5125649, 9.16659471 45.51503687, 9.17775522 45.51587112, 9.18612951 45.51587156, 9.19854003 45.51486882, 9.21061331 45.51190188, 9.22184051 45.50709563, 9.23174866 45.50065245, 9.23992067 45.49284362, 9.24601291 45.48399789, 9.24976966 45.47448762, 9.25103377 45.46471308, 9.25102652 45.45655599, 9.24996149 45.44776617, 9.24682014 45.43940575, 9.24171298 45.43176688, 9.234819 45.42511643, 9.22637941 45.41968668, 9.2166891 45.41566723, 9.20608644 45.41319837, 9.19494141 45.41236626, 9.18657871 45.4123667, 9.17418592 45.41336954, 9.16212839 45.41633501, 9.15091245 45.42113841, 9.14100927 45.42757784, 9.1328352 45.43538264, 9.12673428 45.44422471, 9.12296373 45.4537323, 9.12168305 45.46350562, 9.12167581 45.47166215))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.57750259 45.7213073, 9.58820757 45.72531047, 9.59986996 45.72748624, 9.61201717 45.72774644, 9.62415683 45.72608045, 10.22695044 45.5934877, 10.23824673 45.59004725, 10.24861827 45.58497436, 10.25766599 45.57846416, 10.26504204 45.57076707, 10.27046316 45.56217912, 10.27372157 45.55303054, 10.27469286 45.54367306, 10.27334062 45.53446636, 10.26971773 45.52576421, 10.26396419 45.51790094, 10.25630159 45.51117854, 10.24702456 45.50585511, 10.2364894 45.50213501, 10.22510042 45.50016099, 10.21329451 45.50000875, 10.20152443 45.50168408, 9.62481345 45.62869051, 9.20220303 45.4237427, 9.19184348 45.41981076, 9.18056095 45.41760779, 9.16878828 45.41721828, 9.15697698 45.41865711, 9.14558001 45.42186899, 9.13503456 45.42673058, 9.12574534 45.43305522, 9.11806912 45.44060008, 9.11230103 45.44907545, 9.10866322 45.45815585, 9.10729626 45.46749248, 9.1082536 45.47672665, 9.11149935 45.48550347, 9.11690961 45.49348557, 9.12427699 45.50036604, 9.1333186 45.50588024, 9.57750259 45.7213073))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetBufferAroundFeatureAsync(geoOntology,
                new RDFResource("ex:milanFT2"), 5000));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBufferAroundFeatureAsync(null,
                new RDFResource("ex:milanFT"), 5000));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBufferAroundFeatureAsync(geoOntology,
                null as RDFResource, 5000));
        }

        [TestMethod]
        public async Task ShouldGetBufferAroundLiteralAsync()
        {
            RDFTypedLiteral milanBuffer = await GEOHelper.GetBufferAroundFeatureAsync(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);
            RDFTypedLiteral brebemiBuffer = await GEOHelper.GetBufferAroundFeatureAsync(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);

            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.12167581 45.47166215, 9.12272511 45.48045166, 9.12585291 45.48881321, 9.13095057 45.49645448, 9.13784041 45.50310824, 9.14628183 45.50854175, 9.15597971 45.5125649, 9.16659471 45.51503687, 9.17775522 45.51587112, 9.18612951 45.51587156, 9.19854003 45.51486882, 9.21061331 45.51190188, 9.22184051 45.50709563, 9.23174866 45.50065245, 9.23992067 45.49284362, 9.24601291 45.48399789, 9.24976966 45.47448762, 9.25103377 45.46471308, 9.25102652 45.45655599, 9.24996149 45.44776617, 9.24682014 45.43940575, 9.24171298 45.43176688, 9.234819 45.42511643, 9.22637941 45.41968668, 9.2166891 45.41566723, 9.20608644 45.41319837, 9.19494141 45.41236626, 9.18657871 45.4123667, 9.17418592 45.41336954, 9.16212839 45.41633501, 9.15091245 45.42113841, 9.14100927 45.42757784, 9.1328352 45.43538264, 9.12673428 45.44422471, 9.12296373 45.4537323, 9.12168305 45.46350562, 9.12167581 45.47166215))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.57750259 45.7213073, 9.58820757 45.72531047, 9.59986996 45.72748624, 9.61201717 45.72774644, 9.62415683 45.72608045, 10.22695044 45.5934877, 10.23824673 45.59004725, 10.24861827 45.58497436, 10.25766599 45.57846416, 10.26504204 45.57076707, 10.27046316 45.56217912, 10.27372157 45.55303054, 10.27469286 45.54367306, 10.27334062 45.53446636, 10.26971773 45.52576421, 10.26396419 45.51790094, 10.25630159 45.51117854, 10.24702456 45.50585511, 10.2364894 45.50213501, 10.22510042 45.50016099, 10.21329451 45.50000875, 10.20152443 45.50168408, 9.62481345 45.62869051, 9.20220303 45.4237427, 9.19184348 45.41981076, 9.18056095 45.41760779, 9.16878828 45.41721828, 9.15697698 45.41865711, 9.14558001 45.42186899, 9.13503456 45.42673058, 9.12574534 45.43305522, 9.11806912 45.44060008, 9.11230103 45.44907545, 9.10866322 45.45815585, 9.10729626 45.46749248, 9.1082536 45.47672665, 9.11149935 45.48550347, 9.11690961 45.49348557, 9.12427699 45.50036604, 9.1333186 45.50588024, 9.57750259 45.7213073))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBufferAroundFeatureAsync(
                null as RDFTypedLiteral, 5000));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetBufferAroundFeatureAsync(
               new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING), 5000));
        }
        #endregion

        #region Tests (Engine:ConvexHull)
        [TestMethod]
        public async Task ShouldGetConvexHullOfFeatureAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT")),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            RDFTypedLiteral milanBuffer = await GEOHelper.GetConvexHullOfFeatureAsync(geoOntology, new RDFResource("ex:milanFT"));
            RDFTypedLiteral brebemiBuffer = await GEOHelper.GetConvexHullOfFeatureAsync(geoOntology, new RDFResource("ex:brebemiFT"));

            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.19054385 45.46003666, 9.19054385 45.46819347, 9.18217536 45.46819347, 9.18217536 45.46003666, 9.19054385 45.46003666))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 10.21423284 45.54758259, 9.6118352 45.68014585, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetConvexHullOfFeatureAsync(geoOntology,
                new RDFResource("ex:milanFT2")));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetConvexHullOfFeatureAsync(null,
                new RDFResource("ex:milanFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetConvexHullOfFeatureAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetConvexHullOfLiteralAsync()
        {
            RDFTypedLiteral milanBuffer = await GEOHelper.GetConvexHullOfFeatureAsync(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiBuffer = await GEOHelper.GetConvexHullOfFeatureAsync(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.19054385 45.46003666, 9.19054385 45.46819347, 9.18217536 45.46819347, 9.18217536 45.46003666, 9.19054385 45.46003666))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 10.21423284 45.54758259, 9.6118352 45.68014585, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetConvexHullOfFeatureAsync(
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetConvexHullOfFeatureAsync(
               new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }
        #endregion

        #region Tests (Engine:Envelope)
        [TestMethod]
        public async Task ShouldGetEnvelopeOfFeatureAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiFT")),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:brebemiGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            RDFTypedLiteral milanBuffer = await GEOHelper.GetEnvelopeOfFeatureAsync(geoOntology, new RDFResource("ex:milanFT"));
            RDFTypedLiteral brebemiBuffer = await GEOHelper.GetEnvelopeOfFeatureAsync(geoOntology, new RDFResource("ex:brebemiFT"));

            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.18222872 45.45969789, 9.18089846 45.46814142, 9.19049051 45.46853225, 9.19181962 45.46008861, 9.18222872 45.45969789))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 9.13666191 45.66109756, 10.19206555 45.70224787, 10.22020906 45.50567292, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetEnvelopeOfFeatureAsync(geoOntology,
                new RDFResource("ex:milanFT2")));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetEnvelopeOfFeatureAsync(null,
                new RDFResource("ex:milanFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetEnvelopeOfFeatureAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetEnvelopeOfLiteralAsync()
        {
            RDFTypedLiteral milanBuffer = await GEOHelper.GetEnvelopeOfFeatureAsync(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiBuffer = await GEOHelper.GetEnvelopeOfFeatureAsync(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.18222872 45.45969789, 9.18089846 45.46814142, 9.19049051 45.46853225, 9.19181962 45.46008861, 9.18222872 45.45969789))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 9.13666191 45.66109756, 10.19206555 45.70224787, 10.22020906 45.50567292, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetEnvelopeOfFeatureAsync(
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetEnvelopeOfFeatureAsync(
               new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }
        #endregion
    
        #region Tests (Engine:NearBy)
        [TestMethod]
        public async Task ShouldGetFeaturesNearByAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanProximityFeatures = await GEOHelper.GetFeaturesNearBy(geoOntology, new RDFResource("ex:milanFT"), 460000);
            List<RDFResource> romeProximityFeatures = await GEOHelper.GetFeaturesNearBy(geoOntology, new RDFResource("ex:romeFT"), 100000);

            Assert.IsNotNull(milanProximityFeatures);
            Assert.IsTrue(milanProximityFeatures.Single().URI.Equals(new Uri("ex:romeFT")));
            Assert.IsNotNull(romeProximityFeatures);
            Assert.IsTrue(romeProximityFeatures.Single().URI.Equals(new Uri("ex:tivoliFT")));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesNearBy(geoOntology,
                new RDFResource("ex:milanFT2"), 20000));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesNearBy(null,
                new RDFResource("ex:milanFT"), 20000));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesNearBy(geoOntology,
                null as RDFResource, 20000));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesNearByLiteralAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM")))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> proximityFeatures = await GEOHelper.GetFeaturesNearBy(geoOntology, new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 100000);
            
            Assert.IsNotNull(proximityFeatures);
            Assert.IsTrue(proximityFeatures.Single().URI.Equals(new Uri("ex:romeFT")));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesNearBy(geoOntology,
                new RDFResource("ex:milanFT2"), 20000));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesNearBy(null,
                new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 20000));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesNearBy(geoOntology,
                null as RDFTypedLiteral, 20000));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesNearBy(geoOntology,
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING), 20000));
        }
        #endregion

        #region Tests (Engine:Direction)
        [TestMethod]
        public async Task ShouldGetFeaturesNorthDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionNorthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.North);
            List<RDFResource> romeDirectionNorthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.North);
            List<RDFResource> tivoliDirectionNorthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.North);

            Assert.IsNotNull(milanDirectionNorthFeatures);
            Assert.IsTrue(milanDirectionNorthFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionNorthFeatures);
            Assert.IsTrue(romeDirectionNorthFeatures.Count == 2);
            Assert.IsNotNull(tivoliDirectionNorthFeatures);
            Assert.IsTrue(tivoliDirectionNorthFeatures.Count == 1);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.North));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.North));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.North));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesNorthEastDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionNorthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.NorthEast);
            List<RDFResource> romeDirectionNorthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.NorthEast);
            List<RDFResource> tivoliDirectionNorthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.NorthEast);

            Assert.IsNotNull(milanDirectionNorthEastFeatures);
            Assert.IsTrue(milanDirectionNorthEastFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionNorthEastFeatures);
            Assert.IsTrue(romeDirectionNorthEastFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionNorthEastFeatures);
            Assert.IsTrue(tivoliDirectionNorthEastFeatures.Count == 0);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.NorthEast));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.NorthEast));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.NorthEast));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesNorthWestDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionNorthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.NorthWest);
            List<RDFResource> romeDirectionNorthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.NorthWest);
            List<RDFResource> tivoliDirectionNorthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.NorthWest);

            Assert.IsNotNull(milanDirectionNorthWestFeatures);
            Assert.IsTrue(milanDirectionNorthWestFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionNorthWestFeatures);
            Assert.IsTrue(romeDirectionNorthWestFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionNorthWestFeatures);
            Assert.IsTrue(tivoliDirectionNorthWestFeatures.Count == 1);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.NorthWest));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.NorthWest));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.NorthWest));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesEastDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.East);
            List<RDFResource> romeDirectionEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.East);
            List<RDFResource> tivoliDirectionEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.East);

            Assert.IsNotNull(milanDirectionEastFeatures);
            Assert.IsTrue(milanDirectionEastFeatures.Count == 2);
            Assert.IsNotNull(romeDirectionEastFeatures);
            Assert.IsTrue(romeDirectionEastFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionEastFeatures);
            Assert.IsTrue(tivoliDirectionEastFeatures.Count == 0);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.East));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.East));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.East));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesSouthEastDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionSouthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.SouthEast);
            List<RDFResource> romeDirectionSouthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.SouthEast);
            List<RDFResource> tivoliDirectionSouthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.SouthEast);

            Assert.IsNotNull(milanDirectionSouthEastFeatures);
            Assert.IsTrue(milanDirectionSouthEastFeatures.Count == 2);
            Assert.IsNotNull(romeDirectionSouthEastFeatures);
            Assert.IsTrue(romeDirectionSouthEastFeatures.Count == 0);
            Assert.IsNotNull(tivoliDirectionSouthEastFeatures);
            Assert.IsTrue(tivoliDirectionSouthEastFeatures.Count == 0);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.SouthEast));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.SouthEast));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.East));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesWestDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.West);
            List<RDFResource> romeDirectionWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.West);
            List<RDFResource> tivoliDirectionWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.West);

            Assert.IsNotNull(milanDirectionWestFeatures);
            Assert.IsTrue(milanDirectionWestFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionWestFeatures);
            Assert.IsTrue(romeDirectionWestFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionWestFeatures);
            Assert.IsTrue(tivoliDirectionWestFeatures.Count == 2);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.West));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.West));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.West));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesSouthWestDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionSouthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.SouthWest);
            List<RDFResource> romeDirectionSouthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.SouthWest);
            List<RDFResource> tivoliDirectionSouthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.SouthWest);

            Assert.IsNotNull(milanDirectionSouthWestFeatures);
            Assert.IsTrue(milanDirectionSouthWestFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionSouthWestFeatures);
            Assert.IsTrue(romeDirectionSouthWestFeatures.Count == 0);
            Assert.IsNotNull(tivoliDirectionSouthWestFeatures);
            Assert.IsTrue(tivoliDirectionSouthWestFeatures.Count == 1);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.SouthWest));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.SouthWest));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.SouthWest));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesSouthDirectionOfAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> milanDirectionSouthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.South);
            List<RDFResource> romeDirectionSouthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:romeFT"), GEOEnums.GeoDirections.South);
            List<RDFResource> tivoliDirectionSouthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, new RDFResource("ex:tivoliFT"), GEOEnums.GeoDirections.South);

            Assert.IsNotNull(milanDirectionSouthFeatures);
            Assert.IsTrue(milanDirectionSouthFeatures.Count == 2);
            Assert.IsNotNull(romeDirectionSouthFeatures);
            Assert.IsTrue(romeDirectionSouthFeatures.Count == 0);
            Assert.IsNotNull(tivoliDirectionSouthFeatures);
            Assert.IsTrue(tivoliDirectionSouthFeatures.Count == 1);

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                new RDFResource("ex:milanFT2"), GEOEnums.GeoDirections.South));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                new RDFResource("ex:milanFT"), GEOEnums.GeoDirections.South));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFResource, GEOEnums.GeoDirections.South));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesNorthDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                 AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionNorthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.North);
            List<RDFResource> romeDirectionNorthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.North);
            List<RDFResource> tivoliDirectionNorthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.North);

            Assert.IsNotNull(milanDirectionNorthFeatures);
            Assert.IsTrue(milanDirectionNorthFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionNorthFeatures);
            Assert.IsTrue(romeDirectionNorthFeatures.Count == 2);
            Assert.IsNotNull(tivoliDirectionNorthFeatures);
            Assert.IsTrue(tivoliDirectionNorthFeatures.Count == 1);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.North));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.North));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesNorthEastDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionNorthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.NorthEast);
            List<RDFResource> romeDirectionNorthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.NorthEast);
            List<RDFResource> tivoliDirectionNorthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.NorthEast);

            Assert.IsNotNull(milanDirectionNorthEastFeatures);
            Assert.IsTrue(milanDirectionNorthEastFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionNorthEastFeatures);
            Assert.IsTrue(romeDirectionNorthEastFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionNorthEastFeatures);
            Assert.IsTrue(tivoliDirectionNorthEastFeatures.Count == 0);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.NorthEast));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.NorthEast));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesNorthWestDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionNorthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.NorthWest);
            List<RDFResource> romeDirectionNorthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.NorthWest);
            List<RDFResource> tivoliDirectionNorthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.NorthWest);

            Assert.IsNotNull(milanDirectionNorthWestFeatures);
            Assert.IsTrue(milanDirectionNorthWestFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionNorthWestFeatures);
            Assert.IsTrue(romeDirectionNorthWestFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionNorthWestFeatures);
            Assert.IsTrue(tivoliDirectionNorthWestFeatures.Count == 1);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.NorthWest));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.NorthWest));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesEastDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.East);
            List<RDFResource> romeDirectionEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.East);
            List<RDFResource> tivoliDirectionEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.East);

            Assert.IsNotNull(milanDirectionEastFeatures);
            Assert.IsTrue(milanDirectionEastFeatures.Count == 2);
            Assert.IsNotNull(romeDirectionEastFeatures);
            Assert.IsTrue(romeDirectionEastFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionEastFeatures);
            Assert.IsTrue(tivoliDirectionEastFeatures.Count == 0);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.East));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.East));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesSouthEastDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionSouthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.SouthEast);
            List<RDFResource> romeDirectionSouthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.SouthEast);
            List<RDFResource> tivoliDirectionSouthEastFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.SouthEast);

            Assert.IsNotNull(milanDirectionSouthEastFeatures);
            Assert.IsTrue(milanDirectionSouthEastFeatures.Count == 2);
            Assert.IsNotNull(romeDirectionSouthEastFeatures);
            Assert.IsTrue(romeDirectionSouthEastFeatures.Count == 0);
            Assert.IsNotNull(tivoliDirectionSouthEastFeatures);
            Assert.IsTrue(tivoliDirectionSouthEastFeatures.Count == 0);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.SouthEast));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.SouthEast));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesWestDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.West);
            List<RDFResource> romeDirectionWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.West);
            List<RDFResource> tivoliDirectionWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.West);

            Assert.IsNotNull(milanDirectionWestFeatures);
            Assert.IsTrue(milanDirectionWestFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionWestFeatures);
            Assert.IsTrue(romeDirectionWestFeatures.Count == 1);
            Assert.IsNotNull(tivoliDirectionWestFeatures);
            Assert.IsTrue(tivoliDirectionWestFeatures.Count == 2);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.West));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.West));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesSouthWestDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionSouthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.SouthWest);
            List<RDFResource> romeDirectionSouthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.SouthWest);
            List<RDFResource> tivoliDirectionSouthWestFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.SouthWest);

            Assert.IsNotNull(milanDirectionSouthWestFeatures);
            Assert.IsTrue(milanDirectionSouthWestFeatures.Count == 0);
            Assert.IsNotNull(romeDirectionSouthWestFeatures);
            Assert.IsTrue(romeDirectionSouthWestFeatures.Count == 0);
            Assert.IsNotNull(tivoliDirectionSouthWestFeatures);
            Assert.IsTrue(tivoliDirectionSouthWestFeatures.Count == 1);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.SouthWest));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.SouthWest));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesSouthDirectionOfLiteralAsync()
        {
            RDFTypedLiteral milanTL = new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral romeTL = new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            RDFTypedLiteral tivoliTL = new RDFTypedLiteral("POINT(12.79938661 41.96217718)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:milanFT")),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:romeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliFT")),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:milanGM")),
                        new OWLLiteral(milanTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:romeGM")),
                        new OWLLiteral(romeTL)),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:tivoliGM")),
                        new OWLLiteral(tivoliTL)),
                ]
            };
            List<RDFResource> milanDirectionSouthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, milanTL, GEOEnums.GeoDirections.South);
            List<RDFResource> romeDirectionSouthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, romeTL, GEOEnums.GeoDirections.South);
            List<RDFResource> tivoliDirectionSouthFeatures = await GEOHelper.GetFeaturesDirectionAsync(geoOntology, tivoliTL, GEOEnums.GeoDirections.South);

            Assert.IsNotNull(milanDirectionSouthFeatures);
            Assert.IsTrue(milanDirectionSouthFeatures.Count == 2);
            Assert.IsNotNull(romeDirectionSouthFeatures);
            Assert.IsTrue(romeDirectionSouthFeatures.Count == 0);
            Assert.IsNotNull(tivoliDirectionSouthFeatures);
            Assert.IsTrue(tivoliDirectionSouthFeatures.Count == 1);

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(null,
                milanTL, GEOEnums.GeoDirections.South));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesDirectionAsync(geoOntology,
                null as RDFTypedLiteral, GEOEnums.GeoDirections.South));
        }
        #endregion

        #region Tests (Engine:Interaction)
        [TestMethod]
        public async Task ShouldGetFeaturesCrossedByAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:PoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:PoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:PoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:PoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:PoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:PoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreFT")),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:PoGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(11.001141059265075 45.06554633935097, 11.058819281921325 45.036440377586516, 11.127483832702575 45.05972633195962, 11.262066352233825 45.05002500301712, 11.421368110046325 44.960695556664774, 11.605389106140075 44.89068838827955, 11.814129340515075 44.97624111890936, 12.069561469421325 44.98012685115769)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(11.492779242858825 45.22633159406854, 11.514751899108825 45.0539057320877, 11.448833930358825 44.86538705476387, 11.289532172546325 44.734811449636325)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((11.067059028015075 45.17020515864295, 11.794903266296325 45.06554633935097, 11.778423774108825 44.68015498753276, 10.710003363952575 44.97818401794916, 11.067059028015075 45.17020515864295))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((11.270306098327575 45.4078781070719, 10.992901313171325 45.432939821462234, 10.866558539733825 45.338418378714074, 11.270306098327575 45.4078781070719))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> crossedByPoRiver = await GEOHelper.GetFeaturesCrossedByAsync(geoOntology, new RDFResource("ex:PoFT"));
            
            Assert.IsNotNull(crossedByPoRiver);
            Assert.IsTrue(crossedByPoRiver.Count == 2);
            Assert.IsTrue(crossedByPoRiver.Any(ft => ft.Equals(new RDFResource("ex:MontagnanaCentoFT"))));
            Assert.IsTrue(crossedByPoRiver.Any(ft => ft.Equals(new RDFResource("ex:NogaraPortoMaggioreFT"))));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesCrossedByAsync(geoOntology,
                new RDFResource("ex:PoFT2")));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesCrossedByAsync(null,
                new RDFResource("ex:PoFT")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesCrossedByAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesCrossedByLiteralAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreFT")),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:MontagnanaCentoGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(11.492779242858825 45.22633159406854, 11.514751899108825 45.0539057320877, 11.448833930358825 44.86538705476387, 11.289532172546325 44.734811449636325)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:NogaraPortoMaggioreGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((11.067059028015075 45.17020515864295, 11.794903266296325 45.06554633935097, 11.778423774108825 44.68015498753276, 10.710003363952575 44.97818401794916, 11.067059028015075 45.17020515864295))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((11.270306098327575 45.4078781070719, 10.992901313171325 45.432939821462234, 10.866558539733825 45.338418378714074, 11.270306098327575 45.4078781070719))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> crossedByPoRiver = await GEOHelper.GetFeaturesCrossedByAsync(geoOntology, new RDFTypedLiteral("LINESTRING(11.001141059265075 45.06554633935097, 11.058819281921325 45.036440377586516, 11.127483832702575 45.05972633195962, 11.262066352233825 45.05002500301712, 11.421368110046325 44.960695556664774, 11.605389106140075 44.89068838827955, 11.814129340515075 44.97624111890936, 12.069561469421325 44.98012685115769)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(crossedByPoRiver);
            Assert.IsTrue(crossedByPoRiver.Count == 2);
            Assert.IsTrue(crossedByPoRiver.Any(ft => ft.Equals(new RDFResource("ex:MontagnanaCentoFT"))));
            Assert.IsTrue(crossedByPoRiver.Any(ft => ft.Equals(new RDFResource("ex:NogaraPortoMaggioreFT"))));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesCrossedByAsync(null,
                new RDFTypedLiteral("LINESTRING(11.001141059265075 45.06554633935097, 11.058819281921325 45.036440377586516, 11.127483832702575 45.05972633195962, 11.262066352233825 45.05002500301712, 11.421368110046325 44.960695556664774, 11.605389106140075 44.89068838827955, 11.814129340515075 44.97624111890936, 12.069561469421325 44.98012685115769)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesCrossedByAsync(geoOntology,
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesCrossedByAsync(geoOntology,
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesTouchedByAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoBiennoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(10.090599060058592 45.701863522304734, 10.182609558105467 45.89383147810295, 10.292609558105466 45.93283147810291)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((10.090599060058592 45.701863522304734, 10.182609558105467 45.89383147810295, 10.292609558105466 45.93283147810291, 10.392609558105468 45.73283147810295, 10.090599060058592 45.701863522304734))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((11.270306098327575 45.4078781070719, 10.992901313171325 45.432939821462234, 10.866558539733825 45.338418378714074, 11.270306098327575 45.4078781070719))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> touchedByIseoFT = await GEOHelper.GetFeaturesTouchedByAsync(geoOntology, new RDFResource("ex:IseoFT"));
            
            Assert.IsNotNull(touchedByIseoFT);
            Assert.IsTrue(touchedByIseoFT.Count == 2);
            Assert.IsTrue(touchedByIseoFT.Any(ft => ft.Equals(new RDFResource("ex:IseoBiennoFT"))));
            Assert.IsTrue(touchedByIseoFT.Any(ft => ft.Equals(new RDFResource("ex:IseoLevrangeFT"))));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesTouchedByAsync(geoOntology,
                new RDFResource("ex:IseoFT2")));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesTouchedByAsync(null,
                new RDFResource("ex:IseoFT2")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesTouchedByAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesTouchedByLiteralAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoBiennoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoBiennoGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(10.090599060058592 45.701863522304734, 10.182609558105467 45.89383147810295, 10.292609558105466 45.93283147810291)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoLevrangeGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((10.090599060058592 45.701863522304734, 10.182609558105467 45.89383147810295, 10.292609558105466 45.93283147810291, 10.392609558105468 45.73283147810295, 10.090599060058592 45.701863522304734))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:VeronaVillafrancaGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((11.270306098327575 45.4078781070719, 10.992901313171325 45.432939821462234, 10.866558539733825 45.338418378714074, 11.270306098327575 45.4078781070719))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> touchedByIseoFT = await GEOHelper.GetFeaturesTouchedByAsync(geoOntology, new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            
            Assert.IsNotNull(touchedByIseoFT);
            Assert.IsTrue(touchedByIseoFT.Count == 2);
            Assert.IsTrue(touchedByIseoFT.Any(ft => ft.Equals(new RDFResource("ex:IseoBiennoFT"))));
            Assert.IsTrue(touchedByIseoFT.Any(ft => ft.Equals(new RDFResource("ex:IseoLevrangeFT"))));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesTouchedByAsync(null,
                new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesTouchedByAsync(geoOntology,
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesTouchedByAsync(geoOntology,
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesOverlappedByAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioCivateFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateFT")),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.386934023208635 45.87907866204932, 9.421609621353166 45.81283269722657, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.406846742935198 45.855650479509684, 9.435685854263323 45.8271886970881, 9.475854616470354 45.82694946075535)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> overlappedByBallabioCivateFT = await GEOHelper.GetFeaturesOverlappedByAsync(geoOntology, new RDFResource("ex:BallabioCivateFT"));
            
            Assert.IsNotNull(overlappedByBallabioCivateFT);
            Assert.IsTrue(overlappedByBallabioCivateFT.Count == 1);
            Assert.IsTrue(overlappedByBallabioCivateFT.Any(ft => ft.Equals(new RDFResource("ex:LaorcaVercuragoFT"))));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesOverlappedByAsync(geoOntology,
                new RDFResource("ex:BallabioCivateFT2")));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesOverlappedByAsync(null,
                new RDFResource("ex:BallabioCivateFT2")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesOverlappedByAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesOverlappedByLiteralAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:LaorcaVercuragoGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.386934023208635 45.87907866204932, 9.421609621353166 45.81283269722657, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:LeccoValseccaGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.406846742935198 45.855650479509684, 9.435685854263323 45.8271886970881, 9.475854616470354 45.82694946075535)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> overlappedByBallabioCivateFT = await GEOHelper.GetFeaturesOverlappedByAsync(geoOntology, new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            
            Assert.IsNotNull(overlappedByBallabioCivateFT);
            Assert.IsTrue(overlappedByBallabioCivateFT.Count == 1);
            Assert.IsTrue(overlappedByBallabioCivateFT.Any(ft => ft.Equals(new RDFResource("ex:LaorcaVercuragoFT"))));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesOverlappedByAsync(null,
                new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesOverlappedByAsync(geoOntology,
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesOverlappedByAsync(geoOntology,
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesWithinAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioCivateFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioPescateFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:FornaciVillaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateFT")),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateFT")),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioCivateGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.392083864517229 45.85254191756793, 9.346078615493791 45.828624093492635, 9.393457155532854 45.82814563213719, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.370156172304162 45.83948216157425, 9.390755537538537 45.837807855535225)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> overlappedByBallabioCivateFT = await GEOHelper.GetFeaturesWithinAsync(geoOntology, new RDFResource("ex:BallabioCivateFT"));
            
            Assert.IsNotNull(overlappedByBallabioCivateFT);
            Assert.IsTrue(overlappedByBallabioCivateFT.Count == 2);
            Assert.IsTrue(overlappedByBallabioCivateFT.Any(ft => ft.Equals(new RDFResource("ex:BallabioPescateFT"))));
            Assert.IsTrue(overlappedByBallabioCivateFT.Any(ft => ft.Equals(new RDFResource("ex:FornaciVillaFT"))));

            //Unexisting features
            Assert.IsNull(await GEOHelper.GetFeaturesWithinAsync(geoOntology,
                new RDFResource("ex:BallabioCivateFT2")));
            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesWithinAsync(null,
                new RDFResource("ex:BallabioCivateFT2")));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesWithinAsync(geoOntology,
                null as RDFResource));
        }

        [TestMethod]
        public async Task ShouldGetFeaturesWithinLiteralAsync()
        {
            OWLOntology geoOntology = new OWLOntology(new Uri("ex:geoOnt"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioPescateFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:FornaciVillaFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateFT")),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaFT")),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY),
                        new OWLNamedIndividual(new RDFResource("ex:IseoFT")),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:BallabioPescateGM")),
                        new OWLLiteral(new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.392083864517229 45.85254191756793, 9.346078615493791 45.828624093492635, 9.393457155532854 45.82814563213719, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:FornaciVillaGM")),
                        new OWLLiteral(new RDFTypedLiteral("LINESTRING(9.370156172304162 45.83948216157425, 9.390755537538537 45.837807855535225)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                        new OWLNamedIndividual(new RDFResource("ex:IseoGM")),
                        new OWLLiteral(new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))),
                ]
            };
            List<RDFResource> overlappedByBallabioCivateFT = await GEOHelper.GetFeaturesWithinAsync(geoOntology, new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            
            Assert.IsNotNull(overlappedByBallabioCivateFT);
            Assert.IsTrue(overlappedByBallabioCivateFT.Count == 2);
            Assert.IsTrue(overlappedByBallabioCivateFT.Any(ft => ft.Equals(new RDFResource("ex:BallabioPescateFT"))));
            Assert.IsTrue(overlappedByBallabioCivateFT.Any(ft => ft.Equals(new RDFResource("ex:FornaciVillaFT"))));

            //Input guards
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesWithinAsync(null,
                new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222, 9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesWithinAsync(geoOntology,
                null as RDFTypedLiteral));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await GEOHelper.GetFeaturesWithinAsync(geoOntology,
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }
        #endregion
    }
}