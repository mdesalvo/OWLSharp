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
using NetTopologySuite.Geometries;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Extensions.GEO.Test
{
    [TestClass]
    public class GEOOntologyHelperTest
    {
        #region Tests (Declarer)
        [TestMethod]
        public void ShouldDeclarePointFeature()
        {
            OWLOntology geoOnt = new OWLOntology("ex:geoOnt") { Model = new OWLOntologyModel() {
                ClassModel = GEOOntologyLoader.BuildGEOClassModel(), PropertyModel = GEOOntologyLoader.BuildGEOPropertyModel() } };
            geoOnt.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (9.188540, 45.464664), true);
            geoOnt.DeclarePointFeature(new RDFResource("ex:RomeFT"), new RDFResource("ex:RomeGM"), (12.496365, 41.902782), false);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 21);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 37);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 4);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanFT"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:MilanGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGM"), RDFVocabulary.GEOSPARQL.SF.POINT));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>9.18854 45.464664</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:RomeFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:RomeFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:RomeFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:RomeFT"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFResource("ex:RomeGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:RomeGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:RomeGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:RomeGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:RomeGM"), RDFVocabulary.GEOSPARQL.SF.POINT));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:RomeGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POINT (12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:RomeGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPointFeatureBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclarePointFeature(null, new RDFResource("ex:MilanGM"), (0, 0), false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPointFeatureBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclarePointFeature(new RDFResource("ex:MilanFT"), null, (0, 0), false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPointFeatureBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (0, 91), false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPointFeatureBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (181, 0), false));

        //sf:LineString
        [TestMethod]
        public void ShouldDeclareLineFeature()
        {
            OWLOntology geoOnt = new OWLOntology("ex:geoOnt") { Model = new OWLOntologyModel() {
                    ClassModel = GEOOntologyLoader.BuildGEOClassModel(), PropertyModel = GEOOntologyLoader.BuildGEOPropertyModel() } };
            geoOnt.DeclareLineFeature(new RDFResource("ex:MilanRomeFT"), new RDFResource("ex:MilanRomeGM"), [
                (9.188540, 45.464664), (12.496365, 41.902782) ], true);
            geoOnt.DeclareLineFeature(new RDFResource("ex:MilanGenoaFT"), new RDFResource("ex:MilanGenoaGM"), [
                (9.188540, 45.464664), (8.9096308, 44.40855119) ], false);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 21);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 37);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 4);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanRomeFT"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:MilanRomeGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.SF.LINESTRING));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("LINESTRING (9.18854 45.464664, 12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:LineString xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanGenoaFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGenoaFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGenoaFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanGenoaFT"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFResource("ex:MilanGenoaGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanGenoaGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGenoaGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGenoaGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGenoaGM"), RDFVocabulary.GEOSPARQL.SF.LINESTRING));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanGenoaGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("LINESTRING (9.18854 45.464664, 8.9096308 44.40855119)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanGenoaGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:LineString xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:posList>9.18854 45.464664 8.9096308 44.40855119</gml:posList></gml:LineString>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineFeatureBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareLineFeature(null, new RDFResource("ex:MilanRomeGM"), [(0, 0), (1, 1)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineFeatureBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareLineFeature(new RDFResource("ex:MilanRomeFT"), null, [(0, 0), (1, 1)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineFeatureBecauseNullPoints()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareLineFeature(new RDFResource("ex:MilanRomeFT"), new RDFResource("ex:MilanRomeGM"), null, false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineFeatureBecauseLessThan2Points()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareLineFeature(new RDFResource("ex:MilanRomeFT"), new RDFResource("ex:MilanRomeGM"), [(181, 0)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineFeatureBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareLineFeature(new RDFResource("ex:MilanRomeFT"), new RDFResource("ex:MilanRomeGM"), [(0, 91), (1, 1)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineFeatureBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareLineFeature(new RDFResource("ex:MilanRomeFT"), new RDFResource("ex:MilanRomeGM"), [(181, 0), (1, 1)], false));

        //sf:Polygon
        [TestMethod]
        public void ShouldDeclareAreaFeature()
        {
            OWLOntology geoOnt = new OWLOntology("ex:geoOnt") { Model = new OWLOntologyModel() {
                 ClassModel = GEOOntologyLoader.BuildGEOClassModel(), PropertyModel = GEOOntologyLoader.BuildGEOPropertyModel() } };
            geoOnt.DeclareAreaFeature(new RDFResource("ex:MilanRomeNaplesFT"), new RDFResource("ex:MilanRomeNaplesGM"), [
                (9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746) ], true); //This will be closed automatically with 4th point being the 1st
            geoOnt.DeclareAreaFeature(new RDFResource("ex:MilanRomeNaplesBoulogneFT"), new RDFResource("ex:MilanRomeNaplesBoulogneGM"), [
                (9.18854, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746), (11.32378846, 44.54794686), (9.18854, 45.464664) ], false);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 21);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 37);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 4);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeNaplesFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanRomeNaplesFT"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:MilanRomeNaplesGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeNaplesGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesGM"), RDFVocabulary.GEOSPARQL.SF.POLYGON));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaplesGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaplesGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Polygon xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeNaplesBoulogneFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesBoulogneFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesBoulogneFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanRomeNaplesBoulogneFT"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFResource("ex:MilanRomeNaplesBoulogneGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeNaplesBoulogneGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesBoulogneGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesBoulogneGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaplesBoulogneGM"), RDFVocabulary.GEOSPARQL.SF.POLYGON));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaplesBoulogneGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 11.32378846 44.54794686, 9.18854 45.464664))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaplesBoulogneGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Polygon xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 11.32378846 44.54794686 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAreaFeatureBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareAreaFeature(null, new RDFResource("ex:geom"), [(0, 0), (1, 1), (2, 2)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAreaFeatureBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareAreaFeature(new RDFResource("ex:feat"), null, [(0, 0), (1, 1), (2, 2)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAreaFeatureBecauseNullPoints()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareAreaFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), null, false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAreaFeatureBecauseLessThan3Points()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareAreaFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), [(45, 0), (1, 1)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAreaFeatureBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareAreaFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), [(0, 91), (1, 1), (2, 2)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAreaFeatureBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareAreaFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), [(181, 0), (1, 1), (2, 2)], false));

        //sf:MultiPoint
        [TestMethod]
        public void ShouldDeclareMultiPointFeature()
        {
            OWLOntology geoOnt = new OWLOntology("ex:geoOnt") { Model = new OWLOntologyModel() {
                    ClassModel = GEOOntologyLoader.BuildGEOClassModel(), PropertyModel = GEOOntologyLoader.BuildGEOPropertyModel() } };
            geoOnt.DeclareMultiPointFeature(new RDFResource("ex:MilanRomeFT"), new RDFResource("ex:MilanRomeGM"), [
                (9.188540, 45.464664), (12.496365, 41.902782) ], true);
            geoOnt.DeclareMultiPointFeature(new RDFResource("ex:MilanNaplesFT"), new RDFResource("ex:MilanNaplesGM"), [
                (9.188540, 45.464664), (14.2681244, 40.8517746) ], false);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 21);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 37);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 4);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanRomeFT"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:MilanRomeGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.SF.MULTI_POINT));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOINT ((9.18854 45.464664), (12.496365 41.902782))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiPoint xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pointMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:pointMember><gml:pointMember><gml:Point><gml:pos>12.496365 41.902782</gml:pos></gml:Point></gml:pointMember></gml:MultiPoint>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanNaplesFT")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanNaplesFT"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanNaplesFT"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanNaplesFT"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFResource("ex:MilanNaplesGM")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanNaplesGM")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanNaplesGM"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanNaplesGM"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanNaplesGM"), RDFVocabulary.GEOSPARQL.SF.MULTI_POINT));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanNaplesGM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOINT ((9.18854 45.464664), (14.2681244 40.8517746))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanNaplesGM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiPoint xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pointMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:pointMember><gml:pointMember><gml:Point><gml:pos>14.2681244 40.8517746</gml:pos></gml:Point></gml:pointMember></gml:MultiPoint>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointFeatureBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiPointFeature(null, new RDFResource("ex:geom"), [(0, 0), (1, 1)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointFeatureBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiPointFeature(new RDFResource("ex:feat"), null, [(0, 0), (1, 1)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointFeatureBecauseNullPoints()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiPointFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), null, false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointFeatureBecauseLessThan2Points()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiPointFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), [(181, 0)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointFeatureBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiPointFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), [(0, 91), (1, 1)], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointFeatureBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiPointFeature(new RDFResource("ex:feat"), new RDFResource("ex:geom"), [(181, 0), (1, 1)], false));

        //sf:MultiLineString
        [TestMethod]
        public void ShouldDeclareMultiLineFeature()
        {
            OWLOntology geoOnt = new OWLOntology("ex:geoOnt") { Model = new OWLOntologyModel() {
                    ClassModel = GEOOntologyLoader.BuildGEOClassModel(), PropertyModel = GEOOntologyLoader.BuildGEOPropertyModel() } };
            geoOnt.DeclareMultiLineFeature(new RDFResource("ex:FT1"), new RDFResource("ex:GM1"), [
                [(9.188540, 45.464664), (12.496365, 41.902782)],
                [(12.496365, 41.902782), (14.2681244, 40.8517746)] ], true);
            geoOnt.DeclareMultiLineFeature(new RDFResource("ex:FT2"), new RDFResource("ex:GM2"), [
                [(9.188540, 45.464664), (12.496365, 41.902782)],
                [(12.496365, 41.902782), (14.2681244, 40.8517746)] ], false);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 21);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 37);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 4);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:FT1")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:GM1")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:GM1")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTILINESTRING ((9.18854 45.464664, 12.496365 41.902782), (12.496365 41.902782, 14.2681244 40.8517746))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiCurve xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:curveMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:curveMember><gml:curveMember><gml:LineString><gml:posList>12.496365 41.902782 14.2681244 40.8517746</gml:posList></gml:LineString></gml:curveMember></gml:MultiCurve>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:FT2")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFResource("ex:GM2")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:GM2")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTILINESTRING ((9.18854 45.464664, 12.496365 41.902782), (12.496365 41.902782, 14.2681244 40.8517746))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiCurve xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:curveMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:curveMember><gml:curveMember><gml:LineString><gml:posList>12.496365 41.902782 14.2681244 40.8517746</gml:posList></gml:LineString></gml:curveMember></gml:MultiCurve>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(null, new RDFResource("ex:GM"), [
                [(0, 0), (1, 1)], [(1, 1), (2, 2)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(new RDFResource("ex:FT"), null, [
                [(0, 0), (1, 1)], [(1, 1), (2, 2)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseNullLineStrings()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), null, false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseLessThan2LineStrings()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(0, 0), (1, 1)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseHavingNullLineString()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                null, [(1, 1), (2, 2)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseHavingLineStringWithLessThan2Points()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(0, 0)], [(1, 1), (2, 2)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(0, 91), (1, 1)], [(1, 1), (2, 2)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineFeatureBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiLineFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(181, 0), (1, 1)], [(1, 1), (2, 2)] ], false));

        //sf:MultiPolygon
        [TestMethod]
        public void ShouldDeclareMultiAreaFeature()
        {
            OWLOntology geoOnt = new OWLOntology("ex:geoOnt") { Model = new OWLOntologyModel() {
                    ClassModel = GEOOntologyLoader.BuildGEOClassModel(), PropertyModel = GEOOntologyLoader.BuildGEOPropertyModel() } };
            geoOnt.DeclareMultiAreaFeature(new RDFResource("ex:FT1"), new RDFResource("ex:GM1"), [
                [(9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746)], //These polygons will be automatically closed
                [(12.496365, 41.902782), (14.2681244, 40.8517746), (9.188540, 45.464664)] ], true);
            geoOnt.DeclareMultiAreaFeature(new RDFResource("ex:FT2"), new RDFResource("ex:GM2"), [
                [(9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746)], //These polygons will be automatically closed
                [(12.496365, 41.902782), (14.2681244, 40.8517746), (9.188540, 45.464664)] ], false);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 21);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 37);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 4);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:FT1")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:GM1")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:GM1")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOLYGON (((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiSurface xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember></gml:MultiSurface>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:FT2")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFResource("ex:GM2")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:GM2")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOLYGON (((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiSurface xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember></gml:MultiSurface>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(null, new RDFResource("ex:GM"), [
                [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(new RDFResource("ex:FT"), null, [
                [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseNullPolygons()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), null, false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseLessThan2Polygons()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(0, 0), (1, 1), (2, 2)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseHavingNullPolygon()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                null, [(1, 1), (2, 2), (3, 3)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseHavingPolygonWithLessThan3Points()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(0, 0), (1, 1)], [(1, 1), (2, 2), (3, 3)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(0, 91), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)] ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiAreaFeatureBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareMultiAreaFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"), [
                [(181, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (2, 2)] ], false));

        //sf:GeometryCollection
        [TestMethod]
        public void ShouldDeclareCollectionFeature()
        {
            OWLOntology geoOnt = new OWLOntology("ex:geoOnt") { Model = new OWLOntologyModel() {
                    ClassModel = GEOOntologyLoader.BuildGEOClassModel(), PropertyModel = GEOOntologyLoader.BuildGEOPropertyModel() } };
            geoOnt.DeclareCollectionFeature(new RDFResource("ex:FT1"), new RDFResource("ex:GM1"),
                new List<(double, double)>() {
                    { (9.188540, 45.464664) },
                    { (12.496365, 41.902782) },
                    { (14.2681244, 40.8517746) }
                },
                [
                    [(9.188540, 45.464664), (12.496365, 41.902782)],
                    [(14.2681244, 40.8517746), (9.188540, 45.464664)]
                ],
                [
                    [(9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746)], //These polygons will be automatically closed
					[(12.496365, 41.902782), (14.2681244, 40.8517746), (9.188540, 45.464664)]
                ], true);
            geoOnt.DeclareCollectionFeature(new RDFResource("ex:FT2"), new RDFResource("ex:GM2"),
                new List<(double, double)>() {
                    { (9.188540, 45.464664) },
                    { (12.496365, 41.902782) },
                    { (14.2681244, 40.8517746) }
                },
                [
                    [(9.188540, 45.464664), (12.496365, 41.902782)],
                    [(14.2681244, 40.8517746), (9.188540, 45.464664)]
                ],
                [
                    [(9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746)], //These polygons will be automatically closed
					[(12.496365, 41.902782), (14.2681244, 40.8517746), (9.188540, 45.464664)]
                ], false);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 21);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 37);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 4);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:FT1")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:FT1"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:GM1")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:GM1")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("GEOMETRYCOLLECTION (POINT (9.18854 45.464664), POINT (12.496365 41.902782), POINT (14.2681244 40.8517746), LINESTRING (9.18854 45.464664, 12.496365 41.902782), LINESTRING (14.2681244 40.8517746, 9.18854 45.464664), POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), POLYGON ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM1"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiGeometry xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:geometryMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>12.496365 41.902782</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>14.2681244 40.8517746</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember></gml:MultiGeometry>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:FT2")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:FT2"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFResource("ex:GM2")));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:GM2")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("GEOMETRYCOLLECTION (POINT (9.18854 45.464664), POINT (12.496365 41.902782), POINT (14.2681244 40.8517746), LINESTRING (9.18854 45.464664, 12.496365 41.902782), LINESTRING (14.2681244 40.8517746, 9.18854 45.464664), POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), POLYGON ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GM2"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiGeometry xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:geometryMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>12.496365 41.902782</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>14.2681244 40.8517746</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember></gml:MultiGeometry>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(null, new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), null,
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringCollectionFeatureBecauseHavingPointWithInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
               new List<(double, double)>() {
                    { (0, 91) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPointWithInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (181, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingNullLineString()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    null, [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingLineStringWithLessThan2Points()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingLineStringWithInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 91), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingLineStringWithInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(181, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingNullPolygon()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    null, [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPolygonWithLessThan3Points()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPolygonWithInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(0, 91), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPolygonWithInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:geoOnt").DeclareCollectionFeature(new RDFResource("ex:FT"), new RDFResource("ex:GM"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                [
                    [(0, 0), (1, 1)], [(1, 1), (2, 2)]
                ],
                [
                    [(181, 0), (1, 1), (2, 2)], [(1, 1), (2, 2), (3, 3)]
                ], false));
        #endregion

        #region Tests (Analyzer)
        [TestMethod]
        public void ShouldGetDefaultGeometryFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            (Geometry, Geometry) milanDefaultGeometry = geoOntology.GetDefaultGeometryOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsTrue(milanDefaultGeometry.Item1.SRID == 4326  && milanDefaultGeometry.Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanDefaultGeometry.Item2.SRID == 42106 && milanDefaultGeometry.Item2.EqualsTopologically(new Point(-899167.1609069, 4413535.43188373)));
        }

        [TestMethod]
        public void ShouldGetDefaultGeometryFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            (Geometry, Geometry) milanDefaultGeometry = geoOntology.GetDefaultGeometryOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsTrue(milanDefaultGeometry.Item1.SRID == 4326 && milanDefaultGeometry.Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanDefaultGeometry.Item2.SRID == 42106 && milanDefaultGeometry.Item2.EqualsTopologically(new Point(-899167.1609069, 4413535.43188373)));
        }

        [TestMethod]
        public void ShouldNotGetDefaultGeometry()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), false);
            (Geometry, Geometry) milanDefaultGeometry = geoOntology.GetDefaultGeometryOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanDefaultGeometry.Item1);
            Assert.IsNull(milanDefaultGeometry.Item2);
        }

        [TestMethod]
        public void ShouldGetSecondaryGeometriesFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomA"), (9.188540, 45.464664), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomB"), (9.588540, 45.864664), false);
            List<(Geometry, Geometry)> milanSecondaryGeometries = geoOntology.GetSecondaryGeometriesOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanSecondaryGeometries);
            Assert.IsTrue(milanSecondaryGeometries.Count == 2);
            Assert.IsTrue(milanSecondaryGeometries[0].Item1.SRID == 4326  && milanSecondaryGeometries[0].Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanSecondaryGeometries[0].Item2.SRID == 42106 && milanSecondaryGeometries[0].Item2.EqualsTopologically(new Point(-899167.1609069, 4413535.43188373)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item1.SRID == 4326  && milanSecondaryGeometries[1].Item1.EqualsTopologically(new Point(9.588540, 45.864664)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item2.SRID == 42106 && milanSecondaryGeometries[1].Item2.EqualsTopologically(new Point(-861009.18576767, 4453576.61617377)));
        }

        [TestMethod]
        public void ShouldGetSecondaryGeometriesFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomA"), (9.188540, 45.464664), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomB"), (9.588540, 45.864664), false);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<(Geometry, Geometry)> milanSecondaryGeometries = geoOntology.GetSecondaryGeometriesOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanSecondaryGeometries);
            Assert.IsTrue(milanSecondaryGeometries.Count == 2);
            Assert.IsTrue(milanSecondaryGeometries[0].Item1.SRID == 4326 && milanSecondaryGeometries[0].Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanSecondaryGeometries[0].Item2.SRID == 42106 && milanSecondaryGeometries[0].Item2.EqualsTopologically(new Point(-899167.1609069, 4413535.43188373)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item1.SRID == 4326 && milanSecondaryGeometries[1].Item1.EqualsTopologically(new Point(9.588540, 45.864664)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item2.SRID == 42106 && milanSecondaryGeometries[1].Item2.EqualsTopologically(new Point(-861009.18576767, 4453576.61617377)));
        }

        [TestMethod]
        public void ShouldNotGetSecondaryGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomA"), (9.188540, 45.464664), true);
            List<(Geometry, Geometry)> milanSecondaryGeometries = geoOntology.GetSecondaryGeometriesOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanSecondaryGeometries);
            Assert.IsTrue(milanSecondaryGeometries.Count == 0);
        }
        #endregion
    }
}