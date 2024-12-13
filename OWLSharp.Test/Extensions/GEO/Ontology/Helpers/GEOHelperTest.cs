using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.GEO.Ontology.Helpers;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System;
using System.Threading.Tasks;

namespace OWLSharp.Test.Extensions.GEO.Ontology.Helpers
{
    [TestClass]
    public class GEOHelperTest
    {
        #region Tests (Distance)
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

        #region Tests (Length/Area)
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
        }
        #endregion
    }
}