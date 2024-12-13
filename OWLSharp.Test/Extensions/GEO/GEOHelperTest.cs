using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.GEO;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System;
using System.Threading.Tasks;

namespace OWLSharp.Test.Extensions.GEO
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

        #region Tests (Centroid)
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

        #region Tests (Boundary)
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

        #region Tests (Buffer)
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
    }
}