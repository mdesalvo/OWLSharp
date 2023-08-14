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
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.GEO.Test
{
    [TestClass]
    public class GEOSpatialHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetDistanceBetweenFeatures()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanDefGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanSecGeom"), (9.19193456, 45.46420722), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            double? milanRomeDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(geoOntology, new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsTrue(milanRomeDistance >= 450000 && milanRomeDistance <= 4800000); //milan-rome should be between 450km and 480km
        }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingFromGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            double? milanRomeDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(geoOntology, new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsNull(milanRomeDistance);
        }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingToGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanDefGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanSecGeom"), (9.19193456, 45.46420722), false);
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:romeFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:romeFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            double? milanRomeDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(geoOntology, new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsNull(milanRomeDistance);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenFeaturesBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(null, new RDFResource("ex:from"), new RDFResource("ex:to")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenFeaturesBecauseNullFrom()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(new OWLOntology("ex:geoOnt"), null, new RDFResource("ex:to")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenFeaturesBecauseNullTo()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(new OWLOntology("ex:geoOnt"), new RDFResource("ex:from"), null as RDFResource));

        [TestMethod]
        public void ShouldGetDistanceBetweenWKTFeatures()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanDefGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanSecGeom"), (9.19193456, 45.46420722), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            double? milanTriesteDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(geoOntology, new RDFResource("ex:milanFeat"), new RDFTypedLiteral("POINT(13.77197043 45.65248059)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)); //Trieste
            double? romeTriesteDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(geoOntology, new RDFResource("ex:romeFeat"), new RDFTypedLiteral("POINT(13.77197043 45.65248059)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)); //Trieste

            Assert.IsTrue(milanTriesteDistance >= 380000 && milanTriesteDistance <= 3900000); //milan-trieste should be between 380km and 390km
            Assert.IsTrue(romeTriesteDistance >= 410000 && romeTriesteDistance <= 4200000); //rome-trieste should be between 410km and 420km
        }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenWKTFeaturesBecauseMissingFromGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            double? milanTriesteDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(geoOntology, new RDFResource("ex:milanFeat"), new RDFTypedLiteral("POINT(13.77197043 45.65248059)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)); //Trieste

            Assert.IsNull(milanTriesteDistance);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenWKTFeaturesBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(null, new RDFResource("ex:milanFeat"), new RDFResource("ex:to")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenWKTFeaturesBecauseNullFrom()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(new OWLOntology("ex:geoOnt"), null, new RDFResource("ex:to")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenWKTFeaturesBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(new OWLOntology("ex:geoOnt"), new RDFResource("ex:milanFeat"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenWKTFeaturesBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(new OWLOntology("ex:geoOnt"), new RDFResource("ex:milanFeat"), new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetDistanceBetweenAllWKTFeatures()
        {
            double milanTriesteDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                new RDFTypedLiteral("POINT(13.77197043 45.65248059)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)); //Milan-Trieste
            double romeTriesteDistance = GEOSpatialHelper.GetDistanceBetweenFeatures(new RDFTypedLiteral("POINT(12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                new RDFTypedLiteral("POINT(13.77197043 45.65248059)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)); //Rome-Trieste

            Assert.IsTrue(Math.Round(milanTriesteDistance, 2) == 381798.39);
            Assert.IsTrue(Math.Round(romeTriesteDistance, 2) == 413508.13);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenAllWKTFeaturesBecauseNullFrom()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(
                new OWLOntology("ex:geoOnt"),
                null, 
                new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenAllWKTFeaturesBecauseNotGeographicLiteralFrom()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL),
                new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenAllWKTFeaturesBecauseNullTo()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(
                new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                null));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenAllWKTFeaturesBecauseNotGeographicLiteralTo()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetDistanceBetweenFeatures(
                new RDFTypedLiteral("POINT(9.188540 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetLengthOfFeature()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom1"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom2"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.62118352, 45.65014585), (10.26423284, 45.59758259) }, true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            double? milanCentreLength = GEOSpatialHelper.GetLengthOfFeature(geoOntology, new RDFResource("ex:milanCentreFeat"));
            double? brebemiLength = GEOSpatialHelper.GetLengthOfFeature(geoOntology, new RDFResource("ex:brebemiFeat"));
            double? milanLength = GEOSpatialHelper.GetLengthOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsTrue(milanCentreLength >= 3000 && milanCentreLength <= 3300); //Perimeter of milan centre is about 3KM lineair
            Assert.IsTrue(brebemiLength >= 95000 && brebemiLength <= 100000); //BreBeMi is about 95-100KM lineair
            Assert.IsTrue(milanLength == 0); //points have no length
        }

        [TestMethod]
        public void ShouldNotGetLengthOfFeatureBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            double? milanLength = GEOSpatialHelper.GetLengthOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanLength);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingLengthOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetLengthOfFeature(new OWLOntology("ex:geoOnt"), null));

        [TestMethod]
        public void ShouldGetLengthOfWKTFeature()
        {
            double milanCentreLength = GEOSpatialHelper.GetLengthOfFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            double brebemiLength = GEOSpatialHelper.GetLengthOfFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            double milanLength = GEOSpatialHelper.GetLengthOfFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsTrue(Math.Round(milanCentreLength, 2) == 3102.63);
            Assert.IsTrue(Math.Round(brebemiLength, 2) == 95357.31);
            Assert.IsTrue(milanLength == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingLengthOfWKTFeatureBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetLengthOfFeature(null, new RDFResource("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingLengthOfWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetLengthOfFeature(null));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingLengthOfWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetLengthOfFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetAreaOfFeature()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            double? brebemiArea = GEOSpatialHelper.GetAreaOfFeature(geoOntology, new RDFResource("ex:brebemiFeat"));
            double? milanArea = GEOSpatialHelper.GetAreaOfFeature(geoOntology, new RDFResource("ex:milanFeat"));
            double? milanCentreArea = GEOSpatialHelper.GetAreaOfFeature(geoOntology, new RDFResource("ex:milanCentreFeat"));

            Assert.IsTrue(milanCentreArea >= 590000 && milanCentreArea <= 600000);
            Assert.IsTrue(brebemiArea == 0); //lines have no area
            Assert.IsTrue(milanArea == 0); //points have no area
        }

        [TestMethod]
        public void ShouldNotGetAreaOfFeatureBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            double? milanArea = GEOSpatialHelper.GetAreaOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanArea);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingAreaOfFeatureBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetAreaOfFeature(null, new OWLOntology("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingAreaOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetAreaOfFeature(new OWLOntology("ex:geoOnt"), null));

        [TestMethod]
        public void ShouldGetAreaOfWKTFeature()
        {
            double milanCentreArea = GEOSpatialHelper.GetAreaOfFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            double brebemiArea = GEOSpatialHelper.GetAreaOfFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            double milanArea = GEOSpatialHelper.GetAreaOfFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsTrue(Math.Round(milanCentreArea, 2) == 593322.27);
            Assert.IsTrue(brebemiArea == 0);
            Assert.IsTrue(milanArea == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingAreaOfWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetAreaOfFeature(null));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingAreaOfWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetAreaOfFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetCentroidOfFeature()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), 
                (9.16778508, 45.46481222), false);
            RDFTypedLiteral milanCentreCentroid = GEOSpatialHelper.GetCentroidOfFeature(geoOntology, new RDFResource("ex:milanCentreFeat"));
            RDFTypedLiteral brebemiCentroid = GEOSpatialHelper.GetCentroidOfFeature(geoOntology, new RDFResource("ex:brebemiFeat"));
            RDFTypedLiteral milanCentroid = GEOSpatialHelper.GetCentroidOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanCentreCentroid);
            Assert.IsTrue(milanCentreCentroid.Equals(new RDFTypedLiteral("POINT (9.18635964 45.46411499)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiCentroid);
            Assert.IsTrue(brebemiCentroid.Equals(new RDFTypedLiteral("POINT (9.66872097 45.59479136)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanCentroid);
            Assert.IsTrue(milanCentroid.Equals(new RDFTypedLiteral("POINT (9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldNotGetCentroidOfFeatureBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            RDFTypedLiteral milanCentroid = GEOSpatialHelper.GetCentroidOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanCentroid);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingCentroidOfFeatureBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetCentroidOfFeature(null, new OWLOntology("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingCentroidOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetCentroidOfFeature(new OWLOntology("ex:geoOnt"), null));

        [TestMethod]
        public void ShouldGetCentroidOfWKTFeature()
        {
            RDFTypedLiteral milanCentreCentroid = GEOSpatialHelper.GetCentroidOfFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiCentroid = GEOSpatialHelper.GetCentroidOfFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral milanCentroid = GEOSpatialHelper.GetCentroidOfFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanCentreCentroid);
            Assert.IsTrue(milanCentreCentroid.Equals(new RDFTypedLiteral("POINT (9.18635964 45.46411499)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiCentroid);
            Assert.IsTrue(brebemiCentroid.Equals(new RDFTypedLiteral("POINT (9.66872097 45.59479136)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanCentroid);
            Assert.IsTrue(milanCentroid.Equals(new RDFTypedLiteral("POINT (9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingCentroidOfWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetCentroidOfFeature(null));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingCentroidOfWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetCentroidOfFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetBoundaryOfFeature()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            RDFTypedLiteral milanCentreBoundary = GEOSpatialHelper.GetBoundaryOfFeature(geoOntology, new RDFResource("ex:milanCentreFeat"));
            RDFTypedLiteral brebemiBoundary = GEOSpatialHelper.GetBoundaryOfFeature(geoOntology, new RDFResource("ex:brebemiFeat"));
            RDFTypedLiteral milanBoundary = GEOSpatialHelper.GetBoundaryOfFeature(geoOntology, new RDFResource("ex:milanFeat"));
            
            Assert.IsNotNull(milanCentreBoundary);
            Assert.IsTrue(milanCentreBoundary.Equals(new RDFTypedLiteral("LINESTRING (9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBoundary);
            Assert.IsTrue(brebemiBoundary.Equals(new RDFTypedLiteral("MULTIPOINT ((9.16778508 45.46481222), (10.21423284 45.54758259))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBoundary);
            Assert.IsTrue(milanBoundary.Equals(new RDFTypedLiteral("GEOMETRYCOLLECTION EMPTY", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldNotGetBoundaryOfFeatureBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            RDFTypedLiteral milanBoundary = GEOSpatialHelper.GetBoundaryOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanBoundary);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBoundaryOfFeatureBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBoundaryOfFeature(null, new OWLOntology("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBoundaryOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBoundaryOfFeature(new OWLOntology("ex:geoOnt"), null));

        [TestMethod]
        public void ShouldGetBoundaryOfWKTFeature()
        {
            RDFTypedLiteral milanCentreBoundary = GEOSpatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiBoundary = GEOSpatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral milanBoundary = GEOSpatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanCentreBoundary);
            Assert.IsTrue(milanCentreBoundary.Equals(new RDFTypedLiteral("LINESTRING (9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBoundary);
            Assert.IsTrue(brebemiBoundary.Equals(new RDFTypedLiteral("MULTIPOINT ((9.16778508 45.46481222), (10.21423284 45.54758259))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBoundary);
            Assert.IsTrue(milanBoundary.Equals(new RDFTypedLiteral("GEOMETRYCOLLECTION EMPTY", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBoundaryOfWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBoundaryOfFeature(null));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBoundaryOfWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetBufferAroundFeature()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            RDFTypedLiteral milanCentreBuffer = GEOSpatialHelper.GetBufferAroundFeature(geoOntology, new RDFResource("ex:milanCentreFeat"), 5000);
            RDFTypedLiteral brebemiBuffer = GEOSpatialHelper.GetBufferAroundFeature(geoOntology, new RDFResource("ex:brebemiFeat"), 5000);
            RDFTypedLiteral milanBuffer = GEOSpatialHelper.GetBufferAroundFeature(geoOntology, new RDFResource("ex:milanFeat"), 5000);

            Assert.IsNotNull(milanCentreBuffer);
            Assert.IsTrue(milanCentreBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.12167581 45.47166215, 9.12272511 45.48045166, 9.12585291 45.48881321, 9.13095057 45.49645448, 9.13784041 45.50310824, 9.14628183 45.50854175, 9.15597971 45.5125649, 9.16659471 45.51503687, 9.17775522 45.51587112, 9.18612951 45.51587156, 9.19854003 45.51486882, 9.21061331 45.51190188, 9.22184051 45.50709563, 9.23174866 45.50065245, 9.23992067 45.49284362, 9.24601291 45.48399789, 9.24976966 45.47448762, 9.25103377 45.46471308, 9.25102652 45.45655599, 9.24996149 45.44776617, 9.24682014 45.43940575, 9.24171298 45.43176688, 9.234819 45.42511643, 9.22637941 45.41968668, 9.2166891 45.41566723, 9.20608644 45.41319837, 9.19494141 45.41236626, 9.18657871 45.4123667, 9.17418592 45.41336954, 9.16212839 45.41633501, 9.15091245 45.42113841, 9.14100927 45.42757784, 9.1328352 45.43538264, 9.12673428 45.44422471, 9.12296373 45.4537323, 9.12168305 45.46350562, 9.12167581 45.47166215))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.57750259 45.7213073, 9.58820757 45.72531047, 9.59986996 45.72748624, 9.61201717 45.72774644, 9.62415683 45.72608045, 10.22695044 45.5934877, 10.23824673 45.59004725, 10.24861827 45.58497436, 10.25766599 45.57846416, 10.26504204 45.57076707, 10.27046316 45.56217912, 10.27372157 45.55303054, 10.27469286 45.54367306, 10.27334062 45.53446636, 10.26971773 45.52576421, 10.26396419 45.51790094, 10.25630159 45.51117854, 10.24702456 45.50585511, 10.2364894 45.50213501, 10.22510042 45.50016099, 10.21329451 45.50000875, 10.20152443 45.50168408, 9.62481345 45.62869051, 9.20220303 45.4237427, 9.19184348 45.41981076, 9.18056095 45.41760779, 9.16878828 45.41721828, 9.15697698 45.41865711, 9.14558001 45.42186899, 9.13503456 45.42673058, 9.12574534 45.43305522, 9.11806912 45.44060008, 9.11230103 45.44907545, 9.10866322 45.45815585, 9.10729626 45.46749248, 9.1082536 45.47672665, 9.11149935 45.48550347, 9.11690961 45.49348557, 9.12427699 45.50036604, 9.1333186 45.50588024, 9.57750259 45.7213073))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.22781127 45.46725521, 9.22811497 45.45791943, 9.22610039 45.44884908, 9.22184584 45.44039267, 9.21551552 45.43287506, 9.2073531 45.42658493, 9.1976723 45.4217638, 9.18684485 45.41859669, 9.17528622 45.41720513, 9.16343974 45.41764244, 9.15175974 45.41989179, 9.14069419 45.42386676, 9.13066764 45.42941474, 9.122065 45.43632272, 9.1152168 45.44432546, 9.11038653 45.45311564, 9.10776044 45.46235567, 9.10744029 45.4716906, 9.10943931 45.48076173, 9.11368159 45.48922043, 9.12000477 45.49674149, 9.12816629 45.5030357, 9.13785255 45.50786094, 9.14869099 45.51103154, 9.16026449 45.51242547, 9.17212743 45.511989, 9.18382298 45.50973888, 9.1949008 45.50576162, 9.20493449 45.50021018, 9.21353803 45.49329812, 9.22038077 45.48529128, 9.22520004 45.4764976, 9.22781127 45.46725521))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldNotGetBufferAroundFeatureBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            RDFTypedLiteral milanBuffer = GEOSpatialHelper.GetBufferAroundFeature(geoOntology, new RDFResource("ex:milanFeat"), 2500);

            Assert.IsNull(milanBuffer);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBufferAroundFeatureBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBufferAroundFeature(null, new OWLOntology("ex:fromUri"), 650));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBufferAroundFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBufferAroundFeature(new OWLOntology("ex:geoOnt"), null, 650));

        [TestMethod]
        public void ShouldGetBufferAroundWKTFeature()
        {
            RDFTypedLiteral milanCentreBuffer = GEOSpatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);
            RDFTypedLiteral brebemiBuffer = GEOSpatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);
            RDFTypedLiteral milanBuffer = GEOSpatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);

            Assert.IsNotNull(milanCentreBuffer);
            Assert.IsTrue(milanCentreBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.12167581 45.47166215, 9.12272511 45.48045166, 9.12585291 45.48881321, 9.13095057 45.49645448, 9.13784041 45.50310824, 9.14628183 45.50854175, 9.15597971 45.5125649, 9.16659471 45.51503687, 9.17775522 45.51587112, 9.18612951 45.51587156, 9.19854003 45.51486882, 9.21061331 45.51190188, 9.22184051 45.50709563, 9.23174866 45.50065245, 9.23992067 45.49284362, 9.24601291 45.48399789, 9.24976966 45.47448762, 9.25103377 45.46471308, 9.25102652 45.45655599, 9.24996149 45.44776617, 9.24682014 45.43940575, 9.24171298 45.43176688, 9.234819 45.42511643, 9.22637941 45.41968668, 9.2166891 45.41566723, 9.20608644 45.41319837, 9.19494141 45.41236626, 9.18657871 45.4123667, 9.17418592 45.41336954, 9.16212839 45.41633501, 9.15091245 45.42113841, 9.14100927 45.42757784, 9.1328352 45.43538264, 9.12673428 45.44422471, 9.12296373 45.4537323, 9.12168305 45.46350562, 9.12167581 45.47166215))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.57750259 45.7213073, 9.58820757 45.72531047, 9.59986996 45.72748624, 9.61201717 45.72774644, 9.62415683 45.72608045, 10.22695044 45.5934877, 10.23824673 45.59004725, 10.24861827 45.58497436, 10.25766599 45.57846416, 10.26504204 45.57076707, 10.27046316 45.56217912, 10.27372157 45.55303054, 10.27469286 45.54367306, 10.27334062 45.53446636, 10.26971773 45.52576421, 10.26396419 45.51790094, 10.25630159 45.51117854, 10.24702456 45.50585511, 10.2364894 45.50213501, 10.22510042 45.50016099, 10.21329451 45.50000875, 10.20152443 45.50168408, 9.62481345 45.62869051, 9.20220303 45.4237427, 9.19184348 45.41981076, 9.18056095 45.41760779, 9.16878828 45.41721828, 9.15697698 45.41865711, 9.14558001 45.42186899, 9.13503456 45.42673058, 9.12574534 45.43305522, 9.11806912 45.44060008, 9.11230103 45.44907545, 9.10866322 45.45815585, 9.10729626 45.46749248, 9.1082536 45.47672665, 9.11149935 45.48550347, 9.11690961 45.49348557, 9.12427699 45.50036604, 9.1333186 45.50588024, 9.57750259 45.7213073))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.22781127 45.46725521, 9.22811497 45.45791943, 9.22610039 45.44884908, 9.22184584 45.44039267, 9.21551552 45.43287506, 9.2073531 45.42658493, 9.1976723 45.4217638, 9.18684485 45.41859669, 9.17528622 45.41720513, 9.16343974 45.41764244, 9.15175974 45.41989179, 9.14069419 45.42386676, 9.13066764 45.42941474, 9.122065 45.43632272, 9.1152168 45.44432546, 9.11038653 45.45311564, 9.10776044 45.46235567, 9.10744029 45.4716906, 9.10943931 45.48076173, 9.11368159 45.48922043, 9.12000477 45.49674149, 9.12816629 45.5030357, 9.13785255 45.50786094, 9.14869099 45.51103154, 9.16026449 45.51242547, 9.17212743 45.511989, 9.18382298 45.50973888, 9.1949008 45.50576162, 9.20493449 45.50021018, 9.21353803 45.49329812, 9.22038077 45.48529128, 9.22520004 45.4764976, 9.22781127 45.46725521))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBufferAroundWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBufferAroundFeature(null, 20000));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBufferAroundWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL), 12000));

        [TestMethod]
        public void ShouldGetConvexHullOfFeature()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            RDFTypedLiteral milanCentreConvexHull = GEOSpatialHelper.GetConvexHullOfFeature(geoOntology, new RDFResource("ex:milanCentreFeat"));
            RDFTypedLiteral brebemiConvexHull = GEOSpatialHelper.GetConvexHullOfFeature(geoOntology, new RDFResource("ex:brebemiFeat"));
            RDFTypedLiteral milanConvexHull = GEOSpatialHelper.GetConvexHullOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanCentreConvexHull);
            Assert.IsTrue(milanCentreConvexHull.Equals(new RDFTypedLiteral("POLYGON ((9.19054385 45.46003666, 9.19054385 45.46819347, 9.18217536 45.46819347, 9.18217536 45.46003666, 9.19054385 45.46003666))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiConvexHull);
            Assert.IsTrue(brebemiConvexHull.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 10.21423284 45.54758259, 9.6118352 45.68014585, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanConvexHull);
            Assert.IsTrue(milanConvexHull.Equals(new RDFTypedLiteral("POINT (9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldNotGetConvexHullOfFeatureBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            RDFTypedLiteral milanConvexHull = GEOSpatialHelper.GetConvexHullOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanConvexHull);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingConvexHullOfFeatureBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetConvexHullOfFeature(null, new OWLOntology("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingConvexHullOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetConvexHullOfFeature(new OWLOntology("ex:geoOnt"), null));

        [TestMethod]
        public void ShouldGetConvexHullOfWKTFeature()
        {
            RDFTypedLiteral milanCentreConvexHull = GEOSpatialHelper.GetConvexHullOfFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiConvexHull = GEOSpatialHelper.GetConvexHullOfFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral milanConvexHull = GEOSpatialHelper.GetConvexHullOfFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanCentreConvexHull);
            Assert.IsTrue(milanCentreConvexHull.Equals(new RDFTypedLiteral("POLYGON ((9.19054385 45.46003666, 9.19054385 45.46819347, 9.18217536 45.46819347, 9.18217536 45.46003666, 9.19054385 45.46003666))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiConvexHull);
            Assert.IsTrue(brebemiConvexHull.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 10.21423284 45.54758259, 9.6118352 45.68014585, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanConvexHull);
            Assert.IsTrue(milanConvexHull.Equals(new RDFTypedLiteral("POINT (9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingConvexHullOfWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetConvexHullOfFeature(null));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingConvexHullOfWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetConvexHullOfFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetEnvelopeOfFeature()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            RDFTypedLiteral milanCentreEnvelope = GEOSpatialHelper.GetEnvelopeOfFeature(geoOntology, new RDFResource("ex:milanCentreFeat"));
            RDFTypedLiteral brebemiEnvelope = GEOSpatialHelper.GetEnvelopeOfFeature(geoOntology, new RDFResource("ex:brebemiFeat"));
            RDFTypedLiteral milanEnvelope = GEOSpatialHelper.GetEnvelopeOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanCentreEnvelope);
            Assert.IsTrue(milanCentreEnvelope.Equals(new RDFTypedLiteral("POLYGON ((9.18222872 45.45969789, 9.18089846 45.46814142, 9.19049051 45.46853225, 9.19181962 45.46008861, 9.18222872 45.45969789))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiEnvelope);
            Assert.IsTrue(brebemiEnvelope.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 9.13666191 45.66109756, 10.19206555 45.70224787, 10.22020906 45.50567292, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanEnvelope);
            Assert.IsTrue(milanEnvelope.Equals(new RDFTypedLiteral("POINT (9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldNotGetEnvelopeOfFeatureBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            RDFTypedLiteral milanEnvelope = GEOSpatialHelper.GetEnvelopeOfFeature(geoOntology, new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanEnvelope);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingEnvelopeOfFeatureBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetEnvelopeOfFeature(null, new OWLOntology("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingEnvelopeOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetEnvelopeOfFeature(new OWLOntology("ex:geoOnt"), null as RDFResource));

        [TestMethod]
        public void ShouldGetEnvelopeOfWKTFeature()
        {
            RDFTypedLiteral milanCentreEnvelope = GEOSpatialHelper.GetEnvelopeOfFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiEnvelope = GEOSpatialHelper.GetEnvelopeOfFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral milanEnvelope = GEOSpatialHelper.GetEnvelopeOfFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanCentreEnvelope);
            Assert.IsTrue(milanCentreEnvelope.Equals(new RDFTypedLiteral("POLYGON ((9.18222872 45.45969789, 9.18089846 45.46814142, 9.19049051 45.46853225, 9.19181962 45.46008861, 9.18222872 45.45969789))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiEnvelope);
            Assert.IsTrue(brebemiEnvelope.Equals(new RDFTypedLiteral("POLYGON ((9.16778508 45.46481222, 9.13666191 45.66109756, 10.19206555 45.70224787, 10.22020906 45.50567292, 9.16778508 45.46481222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanEnvelope);
            Assert.IsTrue(milanEnvelope.Equals(new RDFTypedLiteral("POINT (9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingEnvelopeOfWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetEnvelopeOfFeature(null));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingEnvelopeOfWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetEnvelopeOfFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetFeaturesNearBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNearBy = GEOSpatialHelper.GetFeaturesNearBy(geoOntology, new RDFResource("ex:romeFeat"), 100000); //100km around Rome

            Assert.IsNotNull(featuresNearBy);
            Assert.IsTrue(featuresNearBy.Count == 1);
            Assert.IsTrue(featuresNearBy.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNearBy = GEOSpatialHelper.GetFeaturesNearBy(geoOntology, new RDFResource("ex:milanFeat"), 20000); //20km around Milan

            Assert.IsNotNull(featuresNearBy);
            Assert.IsTrue(featuresNearBy.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearByBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresNearBy = GEOSpatialHelper.GetFeaturesNearBy(geoOntology, new RDFResource("ex:milanFeat"), 20000); //20km around Milan

            Assert.IsNull(featuresNearBy);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearByBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNearBy(null, new OWLOntology("ex:fromUri"), 1000));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearByBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNearBy(new OWLOntology("ex:geoOnt"), null, 1000));

        [TestMethod]
        public void ShouldGetFeaturesNearPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNearPoint = GEOSpatialHelper.GetFeaturesNearPoint(geoOntology, (12.496365, 41.902782), 100000); //100km around Rome (DefGeom)

            Assert.IsNotNull(featuresNearPoint);
            Assert.IsTrue(featuresNearPoint.Count == 2);
            Assert.IsTrue(featuresNearPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresNearPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesNearPointFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresNearPoint = GEOSpatialHelper.GetFeaturesNearPoint(geoOntology, (9.15513558, 45.46777408), 10000); //10Km around Milan De Angeli

            Assert.IsNotNull(featuresNearPoint);
            Assert.IsTrue(featuresNearPoint.Count == 1);
            Assert.IsTrue(featuresNearPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearPoint()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithin100KmFromRome = GEOSpatialHelper.GetFeaturesNearPoint(geoOntology, (11.53860088, 45.54896859), 20000); //20km around Vicenza

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearPointBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin100KmFromRome = GEOSpatialHelper.GetFeaturesNearPoint(geoOntology, (11.53860088, 45.54896859), 20000); //20km around Vicenza

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearPointBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNearPoint(null, (-181, 45), 1000));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNearPoint(new OWLOntology("ex:geoOnt"), (-181, 45), 1000));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNearPoint(new OWLOntology("ex:geoOnt"), (9, 91), 1000));

        [TestMethod]
        public void ShouldGetFeaturesNorthOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNorthOfPoint = GEOSpatialHelper.GetFeaturesNorthOfPoint(geoOntology, (9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 1);
            Assert.IsTrue(featuresNorthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesNorthOfPointFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresNorthOfPoint = GEOSpatialHelper.GetFeaturesNorthOfPoint(geoOntology, (9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 1);
            Assert.IsTrue(featuresNorthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNorthOfPoint = GEOSpatialHelper.GetFeaturesNorthOfPoint(geoOntology, (10.20883090, 45.56077293)); //Brescia

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresNorthOfPoint = GEOSpatialHelper.GetFeaturesNorthOfPoint(geoOntology, (11.53860090, 45.54896859));

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNorthOfPointBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNorthOfPoint(null, (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNorthOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNorthOfPoint(new OWLOntology("ex:geoOnt"), (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNorthOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesNorthOfPoint(new OWLOntology("ex:geoOnt"), (9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesEastOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresEastOfPoint = GEOSpatialHelper.GetFeaturesEastOfPoint(geoOntology, (12.396365, 42.902782));

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 2);
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesEastOfPointFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresEastOfPoint = GEOSpatialHelper.GetFeaturesEastOfPoint(geoOntology, (12.396365, 42.902782));

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 2);
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesEastOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresEastOfPoint = GEOSpatialHelper.GetFeaturesEastOfPoint(geoOntology, (15.80512362, 40.64259592)); //Potenza

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesEastOfPointBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresEastOfPoint = GEOSpatialHelper.GetFeaturesEastOfPoint(geoOntology, (11.53860090, 45.54896859));

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesEastOfPointBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesEastOfPoint(null, (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesEastOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesEastOfPoint(new OWLOntology("ex:geoOnt"), (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesEastOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesEastOfPoint(new OWLOntology("ex:geoOnt"), (9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesWestOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWestOfPoint = GEOSpatialHelper.GetFeaturesWestOfPoint(geoOntology, (12.396365, 42.902782));

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 1);
            Assert.IsTrue(featuresWestOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesWestOfPointFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresWestOfPoint = GEOSpatialHelper.GetFeaturesWestOfPoint(geoOntology, (12.396365, 42.902782));

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 1);
            Assert.IsTrue(featuresWestOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWestOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWestOfPoint = GEOSpatialHelper.GetFeaturesWestOfPoint(geoOntology, (8.20958180, 44.90095240)); //Asti

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWestOfPointBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWestOfPoint = GEOSpatialHelper.GetFeaturesWestOfPoint(geoOntology, (11.53860090, 45.54896859));

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWestOfPointBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWestOfPoint(null, (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWestOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWestOfPoint(new OWLOntology("ex:geoOnt"), (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWestOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWestOfPoint(new OWLOntology("ex:geoOnt"), (9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesSouthOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresSouthOfPoint = GEOSpatialHelper.GetFeaturesSouthOfPoint(geoOntology, (9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 2);
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesSouthOfPointFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresSouthOfPoint = GEOSpatialHelper.GetFeaturesSouthOfPoint(geoOntology, (9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 2);
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesSouthOfPointFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresSouthOfPoint = GEOSpatialHelper.GetFeaturesSouthOfPoint(geoOntology, (15.80512362, 40.64259592)); //Potenza

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesSouthOfPointBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresSouthOfPoint = GEOSpatialHelper.GetFeaturesSouthOfPoint(geoOntology, (11.53860090, 45.54896859));

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesSouthOfPointBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesSouthOfPoint(null, (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesSouthOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesSouthOfPoint(new OWLOntology("ex:geoOnt"), (-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesSouthOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesSouthOfPoint(new OWLOntology("ex:geoOnt"), (9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesInsideBoxFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithinSearchBox = GEOSpatialHelper.GetFeaturesInsideBox(geoOntology, (12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 2);
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesInsideBoxFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresWithinSearchBox = GEOSpatialHelper.GetFeaturesInsideBox(geoOntology, (12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 2);
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesInsideBox()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithinSearchBox = GEOSpatialHelper.GetFeaturesInsideBox(geoOntology, (12.50687563, 41.67714954), (12.67853701, 41.80728360)); //Pomezia-Frascati

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesInsideBoxBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin100KmFromRome = GEOSpatialHelper.GetFeaturesInsideBox(geoOntology, (12.50687563, 41.67714954), (12.67853701, 41.80728360)); //Pomezia-Frascati

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesInsideBox(null, (-181, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidLowerLeftLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesInsideBox(new OWLOntology("ex:geoOnt"),(-181, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidLowerLeftLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesInsideBox(new OWLOntology("ex:geoOnt"), (9, 91), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidUpperRightLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesInsideBox(new OWLOntology("ex:geoOnt"), (32, 45), (181, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidUpperRightLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesInsideBox(new OWLOntology("ex:geoOnt"), (9, 45), (76, 91)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseExceedingLowerLeftLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesInsideBox(new OWLOntology("ex:geoOnt"), (81, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseExceedingLowerLeftLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesInsideBox(new OWLOntology("ex:geoOnt"), (9, 84), (76, 58)));

        [TestMethod]
        public void ShouldGetFeaturesOutsideBoxFromWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresOutsideSearchBox = GEOSpatialHelper.GetFeaturesOutsideBox(geoOntology, (12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresOutsideSearchBox);
            Assert.IsTrue(featuresOutsideSearchBox.Count == 1);
            Assert.IsTrue(featuresOutsideSearchBox.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesOutsideBoxFromGML()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresOutsideSearchBox = GEOSpatialHelper.GetFeaturesOutsideBox(geoOntology, (12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresOutsideSearchBox);
            Assert.IsTrue(featuresOutsideSearchBox.Count == 1);
            Assert.IsTrue(featuresOutsideSearchBox.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOutsideBox()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            List<RDFResource> featuresOutsideSearchBox = GEOSpatialHelper.GetFeaturesOutsideBox(geoOntology, (9.12149722, 45.18770380), (9.82530581, 45.77780892));

            Assert.IsNotNull(featuresOutsideSearchBox);
            Assert.IsTrue(featuresOutsideSearchBox.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOutsideBoxBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresOutside100KmFromRome = GEOSpatialHelper.GetFeaturesOutsideBox(geoOntology, (12.50687563, 41.67714954), (12.67853701, 41.80728360));

            Assert.IsNotNull(featuresOutside100KmFromRome);
            Assert.IsTrue(featuresOutside100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOutsideBox(null, (-181, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidLowerLeftLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOutsideBox(new OWLOntology("ex:geoOnt"),(-181, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidLowerLeftLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOutsideBox(new OWLOntology("ex:geoOnt"),(9, 91), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidUpperRightLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOutsideBox(new OWLOntology("ex:geoOnt"),(32, 45), (181, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidUpperRightLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOutsideBox(new OWLOntology("ex:geoOnt"),(9, 45), (76, 91)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseExceedingLowerLeftLongitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOutsideBox(new OWLOntology("ex:geoOnt"),(81, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseExceedingLowerLeftLatitude()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOutsideBox(new OWLOntology("ex:geoOnt"),(9, 84), (76, 58)));

        [TestMethod]
        public void ShouldGetFeaturesCrossedBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:PoFeat"), new RDFResource("ex:PoGeom"), new List<(double, double)>() {
                (11.001141059265075, 45.06554633935097), (11.058819281921325, 45.036440377586516), (11.127483832702575, 45.05972633195962),
                (11.262066352233825, 45.05002500301712), (11.421368110046325, 44.960695556664774), (11.605389106140075, 44.89068838827955),
                (11.814129340515075, 44.97624111890936), (12.069561469421325, 44.98012685115769) } , true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:MontagnanaCentoFeat"), new RDFResource("ex:MontagnanaCentoGeom"), new List<(double, double)>() {
                (11.492779242858825, 45.22633159406854), (11.514751899108825, 45.0539057320877), (11.448833930358825, 44.86538705476387),
                (11.289532172546325, 44.734811449636325) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:NogaraPortoMaggioreFeat"), new RDFResource("ex:NogaraPortoMaggioreGeom"), new List<(double, double)>() {
                (11.067059028015075, 45.17020515864295), (11.794903266296325, 45.06554633935097), (11.778423774108825, 44.68015498753276),
                (10.710003363952575, 44.97818401794916), (11.067059028015075, 45.17020515864295) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresCrossedBy = GEOSpatialHelper.GetFeaturesCrossedBy(geoOntology, new RDFResource("ex:PoFeat"));

            Assert.IsNotNull(featuresCrossedBy);
            Assert.IsTrue(featuresCrossedBy.Count == 2);
            Assert.IsTrue(featuresCrossedBy.Any(ft => ft.Equals(new RDFResource("ex:MontagnanaCentoFeat"))));
            Assert.IsTrue(featuresCrossedBy.Any(ft => ft.Equals(new RDFResource("ex:NogaraPortoMaggioreFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesCrossedBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:BresciaSuzzaraFeat"), new RDFResource("ex:BresciaSuzzaraGeom"), new List<(double, double)>() {
                (10.177166449890075, 45.53692325390463), (10.144207465515075, 45.33648772403282), (10.388653266296325, 45.201178314133756),
                (10.740215766296325, 44.987897525678754) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:BresciaSuzzaraFeat"), new RDFResource("ex:BresciaSuzzaraGeom"), new List<(double, double)>() {
                (10.177166449890075, 45.53692325390463), (10.210125434265075, 45.42330201702671), (10.144207465515075, 45.33648772403282),
                (10.2499508737182, 45.21472377036376), (10.421612250671325, 45.14502706082907), (10.589153754577575, 45.07233559914505),
                (10.740215766296325, 44.987897525678754) }, false);
            geoOntology.DeclareLineFeature(new RDFResource("ex:MontagnanaCentoFeat"), new RDFResource("ex:MontagnanaCentoGeom"), new List<(double, double)>() {
                (11.492779242858825, 45.22633159406854), (11.514751899108825, 45.0539057320877), (11.448833930358825, 44.86538705476387),
                (11.289532172546325, 44.734811449636325) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresCrossedBy = GEOSpatialHelper.GetFeaturesCrossedBy(geoOntology, new RDFResource("ex:BresciaSuzzaraFeat"));

            Assert.IsNotNull(featuresCrossedBy);
            Assert.IsTrue(featuresCrossedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesCrossedByBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresCrossedBy = GEOSpatialHelper.GetFeaturesCrossedBy(geoOntology, new RDFResource("ex:poFeat"));

            Assert.IsNotNull(featuresCrossedBy);
            Assert.IsTrue(featuresCrossedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesCrossedByBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesCrossedBy(null, new OWLOntology("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesCrossedByBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesCrossedBy(new OWLOntology("ex:geoOnt"), null as RDFResource));

        [TestMethod]
        public void ShouldGetFeaturesCrossedByWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:MontagnanaCentoFeat"), new RDFResource("ex:MontagnanaCentoGeom"), new List<(double, double)>() {
                (11.492779242858825, 45.22633159406854), (11.514751899108825, 45.0539057320877), (11.448833930358825, 44.86538705476387),
                (11.289532172546325, 44.734811449636325) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:NogaraPortoMaggioreFeat"), new RDFResource("ex:NogaraPortoMaggioreGeom"), new List<(double, double)>() {
                (11.067059028015075, 45.17020515864295), (11.794903266296325, 45.06554633935097), (11.778423774108825, 44.68015498753276),
                (10.710003363952575, 44.97818401794916), (11.067059028015075, 45.17020515864295) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresCrossedBy = GEOSpatialHelper.GetFeaturesCrossedBy(geoOntology, new RDFTypedLiteral("LINESTRING(11.001141059265075 45.06554633935097, 11.058819281921325 45.036440377586516, " +
                "11.127483832702575 45.05972633195962, 11.262066352233825 45.05002500301712, 11.421368110046325 44.960695556664774, 11.605389106140075 44.89068838827955, 11.814129340515075 44.97624111890936, " +
                "12.069561469421325 44.98012685115769)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)); //PO river

            Assert.IsNotNull(featuresCrossedBy);
            Assert.IsTrue(featuresCrossedBy.Count == 2);
            Assert.IsTrue(featuresCrossedBy.Any(ft => ft.Equals(new RDFResource("ex:MontagnanaCentoFeat"))));
            Assert.IsTrue(featuresCrossedBy.Any(ft => ft.Equals(new RDFResource("ex:NogaraPortoMaggioreFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesCrossedByWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:MontagnanaCentoFeat"), new RDFResource("ex:MontagnanaCentoGeom"), new List<(double, double)>() {
                (11.492779242858825, 45.22633159406854), (11.514751899108825, 45.0539057320877), (11.448833930358825, 44.86538705476387),
                (11.289532172546325, 44.734811449636325) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresCrossedBy = GEOSpatialHelper.GetFeaturesCrossedBy(geoOntology, new RDFTypedLiteral("LINESTRING(10.177166449890075 45.53692325390463, 10.144207465515075 45.33648772403282," +
                "10.388653266296325 45.201178314133756, 10.740215766296325 44.987897525678754)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)); //brescia-suzzara

            Assert.IsNotNull(featuresCrossedBy);
            Assert.IsTrue(featuresCrossedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesCrossedByWKTBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresCrossedBy = GEOSpatialHelper.GetFeaturesCrossedBy(geoOntology, new RDFTypedLiteral("POINT (9.15 45.15)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresCrossedBy);
            Assert.IsTrue(featuresCrossedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesCrossedByWKTBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesCrossedBy(new OWLOntology("ex:geoOnt"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesCrossedByWKTBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesCrossedBy(new OWLOntology("ex:geoOnt"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesCrossedByWKTBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesCrossedBy(new OWLOntology("ex:geoOnt"), new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetFeaturesTouchedBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"), (10.090599060058592, 45.701863522304734), true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:IseoBiennoFeat"), new RDFResource("ex:IseoBiennoGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:IseoLevrangeFeat"), new RDFResource("ex:IseoLevrangeGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291),
                (10.392609558105468, 45.73283147810295), (10.090599060058592, 45.701863522304734) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresTouchedBy = GEOSpatialHelper.GetFeaturesTouchedBy(geoOntology, new RDFResource("ex:IseoFeat"));

            Assert.IsNotNull(featuresTouchedBy);
            Assert.IsTrue(featuresTouchedBy.Count == 2);
            Assert.IsTrue(featuresTouchedBy.Any(ft => ft.Equals(new RDFResource("ex:IseoBiennoFeat"))));
            Assert.IsTrue(featuresTouchedBy.Any(ft => ft.Equals(new RDFResource("ex:IseoLevrangeFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesTouchedBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:DomodossolaVerbaniaFeat"), new RDFResource("ex:DomodossolaVerbaniaGeom"), new List<(double, double)>() {
                (8.287124633789062, 46.11703764257686), (8.561782836914062, 45.932050196856295) }, false);
            geoOntology.DeclareLineFeature(new RDFResource("ex:IseoBiennoFeat"), new RDFResource("ex:IseoBiennoGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:IseoLevrangeFeat"), new RDFResource("ex:IseoLevrangeGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291),
                (10.392609558105468, 45.73283147810295), (10.090599060058592, 45.701863522304734) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresTouchedBy = GEOSpatialHelper.GetFeaturesTouchedBy(geoOntology, new RDFResource("ex:DomodossolaVerbaniaFeat"));

            Assert.IsNotNull(featuresTouchedBy);
            Assert.IsTrue(featuresTouchedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesTouchedByBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresTouchedBy = GEOSpatialHelper.GetFeaturesTouchedBy(geoOntology, new RDFResource("ex:poFeat"));

            Assert.IsNotNull(featuresTouchedBy);
            Assert.IsTrue(featuresTouchedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesTouchedByBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesTouchedBy(null, new RDFResource("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesTouchedByBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesTouchedBy(new OWLOntology("ex:geoOnt"), null as RDFResource));

        [TestMethod]
        public void ShouldGetFeaturesTouchedByWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:IseoBiennoFeat"), new RDFResource("ex:IseoBiennoGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:IseoLevrangeFeat"), new RDFResource("ex:IseoLevrangeGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291),
                (10.392609558105468, 45.73283147810295), (10.090599060058592, 45.701863522304734) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresTouchedBy = GEOSpatialHelper.GetFeaturesTouchedBy(geoOntology, new RDFTypedLiteral("POINT(10.090599060058592 45.701863522304734)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresTouchedBy);
            Assert.IsTrue(featuresTouchedBy.Count == 2);
            Assert.IsTrue(featuresTouchedBy.Any(ft => ft.Equals(new RDFResource("ex:IseoBiennoFeat"))));
            Assert.IsTrue(featuresTouchedBy.Any(ft => ft.Equals(new RDFResource("ex:IseoLevrangeFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesTouchedByWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:IseoBiennoFeat"), new RDFResource("ex:IseoBiennoGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:IseoLevrangeFeat"), new RDFResource("ex:IseoLevrangeGeom"), new List<(double, double)>() {
                (10.090599060058592, 45.701863522304734), (10.182609558105467, 45.89383147810295), (10.292609558105466, 45.93283147810291),
                (10.392609558105468, 45.73283147810295), (10.090599060058592, 45.701863522304734) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:VeronaVillafrancaFeat"), new RDFResource("ex:VeronaVillafrancaGeom"), new List<(double, double)>() {
                (11.270306098327575, 45.4078781070719), (10.992901313171325, 45.432939821462234), (10.866558539733825, 45.338418378714074),
                (11.270306098327575, 45.4078781070719) }, false);
            List<RDFResource> featuresTouchedBy = GEOSpatialHelper.GetFeaturesTouchedBy(geoOntology, new RDFTypedLiteral("LINESTRING(8.287124633789062 46.11703764257686, " +
                "8.561782836914062 45.932050196856295)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresTouchedBy);
            Assert.IsTrue(featuresTouchedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesTouchedByWKTBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresTouchedBy = GEOSpatialHelper.GetFeaturesTouchedBy(geoOntology, new RDFTypedLiteral("POINT (9.15 45.15)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresTouchedBy);
            Assert.IsTrue(featuresTouchedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesTouchedByWKTBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesTouchedBy(null, new RDFTypedLiteral("POINT(10 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesTouchedByWKTBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesTouchedBy(new OWLOntology("ex:geoOnt"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesTouchedByWKTBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesTouchedBy(new OWLOntology("ex:geoOnt"), new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetFeaturesOverlappedBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BallabioCivateFeat"), new RDFResource("ex:BallabioCivateGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.346078615493791, 45.828624093492635), (9.455255251235979, 45.77932096932273), 
                (9.425042848892229, 45.89413442236222) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:LaorcaVercuragoFeat"), new RDFResource("ex:LaorcaVercuragoGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.386934023208635, 45.87907866204932), (9.421609621353166, 45.81283269722657),
                (9.425042848892229, 45.89413442236222) }, false);
            geoOntology.DeclareLineFeature(new RDFResource("ex:LeccoValseccaFeat"), new RDFResource("ex:LeccoValseccaGeom"), new List<(double, double)>() {
                (9.406846742935198, 45.855650479509684), (9.435685854263323, 45.8271886970881), (9.475854616470354, 45.82694946075535) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"), 
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresOverlappedBy = GEOSpatialHelper.GetFeaturesOverlappedBy(geoOntology, new RDFResource("ex:BallabioCivateFeat"));

            Assert.IsNotNull(featuresOverlappedBy);
            Assert.IsTrue(featuresOverlappedBy.Count == 1);
            Assert.IsTrue(featuresOverlappedBy.Any(ft => ft.Equals(new RDFResource("ex:LaorcaVercuragoFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOverlappedBy()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BallabioCivateFeat"), new RDFResource("ex:BallabioCivateGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.346078615493791, 45.828624093492635), (9.455255251235979, 45.77932096932273),
                (9.425042848892229, 45.89413442236222) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BorzioArtavaggioFeat"), new RDFResource("ex:BorzioArtavaggioGeom"), new List<(double, double)>() {
                (9.525979738540666, 45.93092021340824), (9.461091738052385, 45.94261967012623), (9.375604372329729, 45.92709944804776), 
                (9.426416139907854, 45.92136780653028), (9.525979738540666, 45.93092021340824) }, false);
            geoOntology.DeclareLineFeature(new RDFResource("ex:LeccoValseccaFeat"), new RDFResource("ex:LeccoValseccaGeom"), new List<(double, double)>() {
                (9.406846742935198, 45.855650479509684), (9.435685854263323, 45.8271886970881), (9.475854616470354, 45.82694946075535) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"),
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresOverlappedBy = GEOSpatialHelper.GetFeaturesOverlappedBy(geoOntology, new RDFResource("ex:BallabioCivateFeat"));

            Assert.IsNotNull(featuresOverlappedBy);
            Assert.IsTrue(featuresOverlappedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOverlappedByBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresOverlappedBy = GEOSpatialHelper.GetFeaturesOverlappedBy(geoOntology, new RDFResource("ex:poFeat"));

            Assert.IsNotNull(featuresOverlappedBy);
            Assert.IsTrue(featuresOverlappedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOverlappedByBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOverlappedBy(null, new OWLOntology("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOverlappedByBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOverlappedBy(new OWLOntology("ex:geoOnt"), null as RDFResource));

        [TestMethod]
        public void ShouldGetFeaturesOverlappedByWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:LaorcaVercuragoFeat"), new RDFResource("ex:LaorcaVercuragoGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.386934023208635, 45.87907866204932), (9.421609621353166, 45.81283269722657),
                (9.425042848892229, 45.89413442236222) }, false);
            geoOntology.DeclareLineFeature(new RDFResource("ex:LeccoValseccaFeat"), new RDFResource("ex:LeccoValseccaGeom"), new List<(double, double)>() {
                (9.406846742935198, 45.855650479509684), (9.435685854263323, 45.8271886970881), (9.475854616470354, 45.82694946075535) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"),
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresOverlappedBy = GEOSpatialHelper.GetFeaturesOverlappedBy(geoOntology, new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222," +
                "9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresOverlappedBy);
            Assert.IsTrue(featuresOverlappedBy.Count == 1);
            Assert.IsTrue(featuresOverlappedBy.Any(ft => ft.Equals(new RDFResource("ex:LaorcaVercuragoFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOverlappedByWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BorzioArtavaggioFeat"), new RDFResource("ex:BorzioArtavaggioGeom"), new List<(double, double)>() {
                (9.525979738540666, 45.93092021340824), (9.461091738052385, 45.94261967012623), (9.375604372329729, 45.92709944804776),
                (9.426416139907854, 45.92136780653028), (9.525979738540666, 45.93092021340824) }, false);
            geoOntology.DeclareLineFeature(new RDFResource("ex:LeccoValseccaFeat"), new RDFResource("ex:LeccoValseccaGeom"), new List<(double, double)>() {
                (9.406846742935198, 45.855650479509684), (9.435685854263323, 45.8271886970881), (9.475854616470354, 45.82694946075535) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"),
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresOverlappedBy = GEOSpatialHelper.GetFeaturesOverlappedBy(geoOntology, new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222," +
                "9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273, 9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresOverlappedBy);
            Assert.IsTrue(featuresOverlappedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOverlappedByWKTBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresOverlappedBy = GEOSpatialHelper.GetFeaturesOverlappedBy(geoOntology, new RDFTypedLiteral("POINT (9.15 45.15)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresOverlappedBy);
            Assert.IsTrue(featuresOverlappedBy.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOverlappedByWKTBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOverlappedBy(null, new RDFTypedLiteral("POINT(10 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOverlappedByWKTBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOverlappedBy(new OWLOntology("ex:geoOnt"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOverlappedByWKTBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesOverlappedBy(new OWLOntology("ex:geoOnt"), new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetFeaturesWithin()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BallabioCivateFeat"), new RDFResource("ex:BallabioCivateGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.346078615493791, 45.828624093492635), (9.455255251235979, 45.77932096932273),
                (9.425042848892229, 45.89413442236222) }, true);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BallabioPescateFeat"), new RDFResource("ex:BallabioPescateGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.392083864517229, 45.85254191756793), (9.346078615493791, 45.828624093492635),
                (9.393457155532854, 45.82814563213719), (9.425042848892229, 45.89413442236222) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:FornaciVillaFeat"), new RDFResource("ex:FornaciVillaGeom"), new List<(double, double)>() {
                (9.370156172304162, 45.83948216157425), (9.390755537538537, 45.837807855535225) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"),
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresWithin = GEOSpatialHelper.GetFeaturesWithin(geoOntology, new RDFResource("ex:BallabioCivateFeat"));

            Assert.IsNotNull(featuresWithin);
            Assert.IsTrue(featuresWithin.Count == 2);
            Assert.IsTrue(featuresWithin.Any(ft => ft.Equals(new RDFResource("ex:BallabioPescateFeat"))));
            Assert.IsTrue(featuresWithin.Any(ft => ft.Equals(new RDFResource("ex:FornaciVillaFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWithin()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:LaglioMoltrasioFeat"), new RDFResource("ex:LaglioMoltrasioGeom"), new List<(double, double)>() {
                (9.138069990663537, 45.88108443556158), (9.101334455995568, 45.86196081911207), (9.128113630800256, 45.87343577859146),
                (9.138069990663537, 45.88108443556158) }, false);
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BallabioPescateFeat"), new RDFResource("ex:BallabioPescateGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.392083864517229, 45.85254191756793), (9.346078615493791, 45.828624093492635),
                (9.393457155532854, 45.82814563213719), (9.425042848892229, 45.89413442236222) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:FornaciVillaFeat"), new RDFResource("ex:FornaciVillaGeom"), new List<(double, double)>() {
                (9.370156172304162, 45.83948216157425), (9.390755537538537, 45.837807855535225) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"),
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresWithin = GEOSpatialHelper.GetFeaturesWithin(geoOntology, new RDFResource("ex:LaglioMoltrasioFeat"));

            Assert.IsNotNull(featuresWithin);
            Assert.IsTrue(featuresWithin.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWithinBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin = GEOSpatialHelper.GetFeaturesWithin(geoOntology, new RDFResource("ex:poFeat"));

            Assert.IsNotNull(featuresWithin);
            Assert.IsTrue(featuresWithin.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWithin(null, new RDFResource("ex:fromUri")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWithin(new OWLOntology("ex:geoOnt"), null as RDFResource));

        [TestMethod]
        public void ShouldGetFeaturesWithinWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:BallabioPescateFeat"), new RDFResource("ex:BallabioPescateGeom"), new List<(double, double)>() {
                (9.425042848892229, 45.89413442236222), (9.392083864517229, 45.85254191756793), (9.346078615493791, 45.828624093492635),
                (9.393457155532854, 45.82814563213719), (9.425042848892229, 45.89413442236222) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:FornaciVillaFeat"), new RDFResource("ex:FornaciVillaGeom"), new List<(double, double)>() {
                (9.370156172304162, 45.83948216157425), (9.390755537538537, 45.837807855535225) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"),
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresWithin = GEOSpatialHelper.GetFeaturesWithin(geoOntology, new RDFTypedLiteral("POLYGON((9.425042848892229 45.89413442236222," +
                "9.346078615493791 45.828624093492635, 9.455255251235979 45.77932096932273,9.425042848892229 45.89413442236222))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresWithin);
            Assert.IsTrue(featuresWithin.Count == 2);
            Assert.IsTrue(featuresWithin.Any(ft => ft.Equals(new RDFResource("ex:BallabioPescateFeat"))));
            Assert.IsTrue(featuresWithin.Any(ft => ft.Equals(new RDFResource("ex:FornaciVillaFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWithinWKT()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.DeclareLineFeature(new RDFResource("ex:LeccoValseccaFeat"), new RDFResource("ex:LeccoValseccaGeom"), new List<(double, double)>() {
                (9.406846742935198, 45.855650479509684), (9.435685854263323, 45.8271886970881), (9.475854616470354, 45.82694946075535) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:IseoFeat"), new RDFResource("ex:IseoGeom"),
                (10.090599060058592, 45.701863522304734), true);
            List<RDFResource> featuresWithin = GEOSpatialHelper.GetFeaturesWithin(geoOntology, new RDFTypedLiteral("POLYGON((9.525979738540666 45.93092021340824," +
                "9.461091738052385 45.94261967012623, 9.375604372329729 45.92709944804776,9.426416139907854 45.92136780653028, 9.525979738540666 45.93092021340824))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresWithin);
            Assert.IsTrue(featuresWithin.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWithinWKTBecauseMissingGeometries()
        {
            OWLOntology geoOntology = new OWLOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:poFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:poFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin = GEOSpatialHelper.GetFeaturesWithin(geoOntology, new RDFTypedLiteral("POINT (9.15 45.15)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(featuresWithin);
            Assert.IsTrue(featuresWithin.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinWKTBecauseNullOntology()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWithin(null, new RDFTypedLiteral("POINT(10 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinWKTBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWithin(new OWLOntology("ex:geoOnt"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinWKTBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLException>(() => GEOSpatialHelper.GetFeaturesWithin(new OWLOntology("ex:geoOnt"), new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));
        #endregion
    }
}