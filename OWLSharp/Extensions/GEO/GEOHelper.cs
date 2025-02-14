/*
   Copyright 2014-2025 Marco De Salvo

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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Extensions.GEO
{
    public static class GEOHelper
    {
        // WGS84 uses LON/LAT coordinates
        // LON => X (West/East, -180 ->180)
        // LAT => Y (North/South, -90->90)

        internal static readonly WKTReader WKTReader = new WKTReader();
        internal static readonly WKTWriter WKTWriter = new WKTWriter();
        internal static readonly GMLReader GMLReader = new GMLReader();
        internal static readonly GMLWriter GMLWriter = new GMLWriter();

        #region Initializer
        [ExcludeFromCodeCoverage]
        public static async Task InitializeGEOAsync(this OWLOntology ontology, int timeoutMilliseconds=20000, int cacheMilliseconds=3600000)
        {
            await ontology?.ImportAsync(new Uri(RDFVocabulary.GEOSPARQL.DEREFERENCE_URI), timeoutMilliseconds, cacheMilliseconds);
            await ontology?.ImportAsync(new Uri(RDFVocabulary.GEOSPARQL.SF.DEREFERENCE_URI), timeoutMilliseconds, cacheMilliseconds);
            await ontology?.ImportAsync(new Uri(RDFVocabulary.GEOSPARQL.GEOF.DEREFERENCE_URI), timeoutMilliseconds, cacheMilliseconds);
        }
        #endregion

        #region Declarer
        public static OWLOntology DeclarePointFeature(this OWLOntology ontology, RDFResource featureUri, GEOPoint geoPoint, bool isDefaultGeometry = true)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare point feature because given \"featureUri\" parameter is null");
            if (geoPoint == null)
                throw new OWLException("Cannot declare point feature because given \"geoPoint\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLNamedIndividual(featureUri));
            ontology.DeclareEntity(new OWLNamedIndividual(geoPoint));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                new OWLNamedIndividual(featureUri)));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                new OWLNamedIndividual(geoPoint)));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.SF.POINT),
                new OWLNamedIndividual(geoPoint)));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                isDefaultGeometry ? new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)
                                  : new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(geoPoint)));
            ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                new OWLNamedIndividual(geoPoint),
                new OWLLiteral(new RDFTypedLiteral(geoPoint.ToWKT(), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))));
            ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                new OWLNamedIndividual(geoPoint),
                new OWLLiteral(new RDFTypedLiteral(geoPoint.ToGML(), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))));

            return ontology;
        }

        public static OWLOntology DeclareLineFeature(this OWLOntology ontology, RDFResource featureUri, GEOLine geoLine, bool isDefaultGeometry = true)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare line feature because given \"featureUri\" parameter is null");
            if (geoLine == null)
                throw new OWLException("Cannot declare line feature because given \"geoLine\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLNamedIndividual(featureUri));
            ontology.DeclareEntity(new OWLNamedIndividual(geoLine));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                new OWLNamedIndividual(featureUri)));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                new OWLNamedIndividual(geoLine)));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.SF.LINESTRING),
                new OWLNamedIndividual(geoLine)));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                isDefaultGeometry ? new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)
                                  : new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(geoLine)));
            ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                new OWLNamedIndividual(geoLine),
                new OWLLiteral(new RDFTypedLiteral(geoLine.ToWKT(), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))));
            ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                new OWLNamedIndividual(geoLine),
                new OWLLiteral(new RDFTypedLiteral(geoLine.ToGML(), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))));

            return ontology;
        }

        public static OWLOntology DeclareAreaFeature(this OWLOntology ontology, RDFResource featureUri, GEOArea geoArea, bool isDefaultGeometry = true)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare area feature because given \"featureUri\" parameter is null");
            if (geoArea == null)
                throw new OWLException("Cannot declare area feature because given \"geoArea\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLNamedIndividual(featureUri));
            ontology.DeclareEntity(new OWLNamedIndividual(geoArea));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                new OWLNamedIndividual(featureUri)));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                new OWLNamedIndividual(geoArea)));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.SF.POLYGON),
                new OWLNamedIndividual(geoArea)));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                isDefaultGeometry ? new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)
                                  : new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(geoArea)));
            ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                new OWLNamedIndividual(geoArea),
                new OWLLiteral(new RDFTypedLiteral(geoArea.ToWKT(), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))));
            ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                new OWLNamedIndividual(geoArea),
                new OWLLiteral(new RDFTypedLiteral(geoArea.ToGML(), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))));

            return ontology;
        }
        #endregion

        #region Analyzer
        public static async Task<List<GEOEntity>> GetSpatialFeatureAsync(this OWLOntology ontology, RDFResource featureURI)
        {
            #region Guards
            if (featureURI == null)
                throw new OWLException("Cannot get spatial dimension of feature because given \"featureURI\" parameter is null");
            #endregion

            List<GEOEntity> spatialExtentOfFeature = new List<GEOEntity>();
            Dictionary<string,List<(Geometry,Geometry)>> featuresWithGeometry = await GetFeaturesWithGeometriesAsync(ontology);
            if (featuresWithGeometry.TryGetValue(featureURI.ToString(), out List<(Geometry,Geometry)> featureGeometries))
                foreach ((Geometry wgs84Geom,Geometry lazGeom) in featureGeometries)
                {
                    RDFResource geometryUri = new RDFResource((string)wgs84Geom.UserData);
                    switch (wgs84Geom)
                    {
                        case Point wgs84Point:
                            spatialExtentOfFeature.Add(new GEOPoint(geometryUri, (wgs84Point.Coordinate.X,wgs84Point.Coordinate.Y)));
                            break;
                        case LineString wgs84Line:
                            spatialExtentOfFeature.Add(new GEOLine(geometryUri, wgs84Line.Coordinates.Select(c => (c.X,c.Y)).ToArray()));
                            break;
                        case Polygon wgs84Area:
                            spatialExtentOfFeature.Add(new GEOArea(geometryUri, wgs84Area.Coordinates.Select(c => (c.X,c.Y)).ToArray()));
                            break;
                    }
                    //other types of OGC geometries are not supported yet...
                }
            return spatialExtentOfFeature;
        }
        #endregion

        #region Analyzer (Distance)
        public static async Task<double?> GetDistanceBetweenFeaturesAsync(OWLOntology ontology, RDFResource fromFeatureUri, RDFResource toFeatureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get distance between features because given \"ontology\" parameter is null");
            if (fromFeatureUri == null)
                throw new OWLException("Cannot get distance between features because given \"fromFeatureUri\" parameter is null");
            if (toFeatureUri == null)
                throw new OWLException("Cannot get distance between features because given \"toFeatureUri\" parameter is null");
            #endregion

            //Collect secondaryGeometries of "From" feature
            (Geometry, Geometry) defaultGeometryFrom = await ontology.GetDefaultGeometryOfFeatureAsync(fromFeatureUri);
            List<(Geometry, Geometry)> geometriesFrom = await ontology.GetSecondaryGeometriesOfFeatureAsync(fromFeatureUri);
            if (defaultGeometryFrom.Item1 != null && defaultGeometryFrom.Item2 != null)
                geometriesFrom.Insert(0, defaultGeometryFrom);

            //Collect secondaryGeometries of "To" feature
            (Geometry, Geometry) defaultGeometryTo = await ontology.GetDefaultGeometryOfFeatureAsync(toFeatureUri);
            List<(Geometry, Geometry)> geometriesTo = await ontology.GetSecondaryGeometriesOfFeatureAsync(toFeatureUri);
            if (defaultGeometryTo.Item1 != null && defaultGeometryTo.Item2 != null)
                geometriesTo.Insert(0, defaultGeometryTo);

            //Perform spatial analysis between collected secondaryGeometries (calibrate minimum distance)
            double? featuresDistance = double.MaxValue;
            geometriesFrom.ForEach(fromGeom => 
            {
                geometriesTo.ForEach(toGeom => 
                {
                    double tempDistance = fromGeom.Item2.Distance(toGeom.Item2);
                    if (tempDistance < featuresDistance)
                        featuresDistance = tempDistance;
                });
            });

            //Give null in case distance could not be calculated (no available secondaryGeometries from any sides)
            return featuresDistance == double.MaxValue ? null : featuresDistance;
        }

        public static async Task<double?> GetDistanceBetweenFeaturesAsync(OWLOntology ontology, RDFResource fromFeatureUri, RDFTypedLiteral toFeatureLiteral)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get distance between features because given \"ontology\" parameter is null");
            if (fromFeatureUri == null)
                throw new OWLException("Cannot get distance between features because given \"fromFeatureUri\" parameter is null");
            if (toFeatureLiteral == null)
                throw new OWLException("Cannot get distance between features because given \"toFeatureLiteral\" parameter is null");
            if (!toFeatureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get distance between features because given \"toFeatureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Collect secondaryGeometries of "From" feature
            (Geometry, Geometry) defaultGeometryFrom = await ontology.GetDefaultGeometryOfFeatureAsync(fromFeatureUri);
            List<(Geometry, Geometry)> geometriesFrom = await ontology.GetSecondaryGeometriesOfFeatureAsync(fromFeatureUri);
            if (defaultGeometryFrom.Item1 != null && defaultGeometryFrom.Item2 != null)
                geometriesFrom.Insert(0, defaultGeometryFrom);

            //Transform "To" feature into geometry
            bool isWKT = toFeatureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84GeometryTo = isWKT ? WKTReader.Read(toFeatureLiteral.Value) : GMLReader.Read(toFeatureLiteral.Value);
            wgs84GeometryTo.SRID = 4326;
            Geometry lazGeometryTo = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84GeometryTo);

            //Perform spatial analysis between collected secondaryGeometries (calibrate minimum distance)
            double? featuresDistance = double.MaxValue;
            geometriesFrom.ForEach(fromGeom => 
            {
                double tempDistance = fromGeom.Item2.Distance(lazGeometryTo);
                if (tempDistance < featuresDistance)
                    featuresDistance = tempDistance;
            });

            //Give null in case distance could not be calculated (no available secondaryGeometries)
            return featuresDistance == double.MaxValue ? null : featuresDistance;
        }

        public static async Task<double?> GetDistanceBetweenFeaturesAsync(RDFTypedLiteral fromFeatureLiteral, RDFTypedLiteral toFeatureLiteral)
        {
            #region Guards
            if (fromFeatureLiteral == null)
                throw new OWLException("Cannot get distance between features because given \"fromFeatureLiteral\" parameter is null");
            if (!fromFeatureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get distance between features because given \"fromFeatureLiteral\" parameter is not a geographic typed literal");
            if (toFeatureLiteral == null)
                throw new OWLException("Cannot get distance between features because given \"toFeatureLiteral\" parameter is null");
            if (!toFeatureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get distance between features because given \"toFeatureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform "From" feature into geometry
            bool fromIsWKT = fromFeatureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84GeometryFrom = fromIsWKT ? WKTReader.Read(fromFeatureLiteral.Value) : GMLReader.Read(fromFeatureLiteral.Value);
            wgs84GeometryFrom.SRID = 4326;
            Geometry lazGeometryFrom = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84GeometryFrom);

            //Transform "To" feature into geometry
            bool toIsWKT = toFeatureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84GeometryTo = toIsWKT ? WKTReader.Read(toFeatureLiteral.Value) : GMLReader.Read(toFeatureLiteral.Value);
            wgs84GeometryTo.SRID = 4326;
            Geometry lazGeometryTo = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84GeometryTo);

            //Perform spatial analysis between secondaryGeometries
            return await Task.FromResult(lazGeometryFrom.Distance(lazGeometryTo));
        }
        #endregion

        #region Analyzer (Measure)
        public static async Task<double?> GetLengthOfFeatureAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get length of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get length of feature because given \"featureUri\" parameter is null");
            #endregion

            //Collect secondaryGeometries of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            List<(Geometry, Geometry)> geometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
                geometries.Insert(0, defaultGeometry);

            //Perform spatial analysis between collected secondaryGeometries (calibrate maximum length)
            double? featureLength = double.MinValue;
            geometries.ForEach(geom => 
            {
                double tempLength = geom.Item2.Length;
                if (tempLength > featureLength)
                    featureLength = tempLength;
            });

            //Give null in case length could not be calculated (no available secondaryGeometries)
            return featureLength == double.MinValue ? null : featureLength;
        }

        public static async Task<double?> GetLengthOfFeatureAsync(RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (featureLiteral == null)
                throw new OWLException("Cannot get length of feature because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get length of feature because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            return await Task.FromResult(lazGeometry.Length);
        }

        public static async Task<double?> GetAreaOfFeatureAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get area of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get area of feature because given \"featureUri\" parameter is null");
            #endregion

            //Collect secondaryGeometries of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            List<(Geometry, Geometry)> geometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
                geometries.Insert(0, defaultGeometry);

            //Perform spatial analysis between collected secondaryGeometries (calibrate maximum area)
            double? featureArea = double.MinValue;
            geometries.ForEach(geom => 
            {
                double tempArea = geom.Item2.Area;
                if (tempArea > featureArea)
                    featureArea = tempArea;
            });

            //Give null in case area could not be calculated (no available secondaryGeometries)
            return featureArea == double.MinValue ? null : featureArea;
        }

        public static async Task<double?> GetAreaOfFeatureAsync(RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (featureLiteral == null)
                throw new OWLException("Cannot get area of feature because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get area of feature because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            return await Task.FromResult(lazGeometry.Area);
        }
        #endregion

        #region Analyzer (Centroid)
        public static async Task<RDFTypedLiteral> GetCentroidOfFeatureAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get centroid of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get centroid of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                Geometry centroidGeometryAZ = defaultGeometry.Item2.Centroid;
                Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(centroidGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                Geometry centroidGeometryAZ = secondaryGeometries.First().Item2.Centroid;
                Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(centroidGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        public static async Task<RDFTypedLiteral> GetCentroidOfFeatureAsync(RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (featureLiteral == null)
                throw new OWLException("Cannot get centroid of feature because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get centroid of feature because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry centroidGeometryAZ = lazGeometry.Centroid;
            Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
            return await Task.FromResult(new RDFTypedLiteral(WKTWriter.Write(centroidGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
        }
        #endregion

        #region Analyzer (Boundary)
        public static async Task<RDFTypedLiteral> GetBoundaryOfFeatureAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get boundary of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get boundary of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                Geometry boundaryGeometryAZ = defaultGeometry.Item2.Boundary;
                Geometry boundaryGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(boundaryGeometryWGS84).Replace("LINEARRING", "LINESTRING"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                Geometry boundaryGeometryAZ = secondaryGeometries.First().Item2.Boundary;
                Geometry boundaryGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(boundaryGeometryWGS84).Replace("LINEARRING", "LINESTRING"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        public static async Task<RDFTypedLiteral> GetBoundaryOfFeatureAsync(RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (featureLiteral == null)
                throw new OWLException("Cannot get boundary of feature because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get boundary of feature because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry boundaryGeometryAZ = lazGeometry.Boundary;
            Geometry boundaryGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
            return await Task.FromResult(new RDFTypedLiteral(WKTWriter.Write(boundaryGeometryWGS84).Replace("LINEARRING", "LINESTRING"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
        }
        #endregion

        #region Analyzer (Buffer)
        public static async Task<RDFTypedLiteral> GetBufferAroundFeatureAsync(OWLOntology ontology, RDFResource featureUri, double bufferMeters)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get buffer around feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get buffer around feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                Geometry bufferGeometryAZ = defaultGeometry.Item2.Buffer(bufferMeters);
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                Geometry bufferGeometryAZ = secondaryGeometries.First().Item2.Buffer(bufferMeters);
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        public static async Task<RDFTypedLiteral> GetBufferAroundFeatureAsync(RDFTypedLiteral featureLiteral, double bufferMeters)
        {
            #region Guards
            if (featureLiteral == null)
                throw new OWLException("Cannot get buffer around feature because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get buffer around feature because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry bufferGeometryAZ = lazGeometry.Buffer(bufferMeters);
            Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
            return await Task.FromResult(new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
        }
        #endregion

        #region Analyzer (ConvexHull)
        public static async Task<RDFTypedLiteral> GetConvexHullOfFeatureAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get convex hull of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get convex hull of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                Geometry bufferGeometryAZ = defaultGeometry.Item2.ConvexHull();
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                Geometry bufferGeometryAZ = secondaryGeometries.First().Item2.ConvexHull();
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        public static async Task<RDFTypedLiteral> GetConvexHullOfFeatureAsync(RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (featureLiteral == null)
                throw new OWLException("Cannot get convex hull of feature because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get convex hull of feature because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry bufferGeometryAZ = lazGeometry.ConvexHull();
            Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
            return await Task.FromResult(new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
        }
        #endregion

        #region Analyzer (Envelope)
        public static async Task<RDFTypedLiteral> GetEnvelopeOfFeatureAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get envelope of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get envelope of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                Geometry bufferGeometryAZ = defaultGeometry.Item2.Envelope;
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                Geometry bufferGeometryAZ = secondaryGeometries.First().Item2.Envelope;
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        public static async Task<RDFTypedLiteral> GetEnvelopeOfFeatureAsync(RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (featureLiteral == null)
                throw new OWLException("Cannot get envelope of feature because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get envelope of feature because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry bufferGeometryAZ = lazGeometry.Envelope;
            Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
            return await Task.FromResult(new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
        }
        #endregion

        #region Analyzer (NearBy)
        public static async Task<List<RDFResource>> GetFeaturesNearBy(OWLOntology ontology, RDFResource featureUri, double distanceMeters)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features within distance because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features within distance because given \"featureUri\" parameter is null");
            #endregion

            //Get centroid of feature
            RDFTypedLiteral centroidOfFeature = await GetCentroidOfFeatureAsync(ontology, featureUri);
            if (centroidOfFeature == null)
                return null;

            //Create WGS84 geometry from centroid of feature
            bool isWKT = centroidOfFeature.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84CentroidOfFeature = isWKT ? WKTReader.Read(centroidOfFeature.Value) : GMLReader.Read(centroidOfFeature.Value);
            wgs84CentroidOfFeature.SRID = 4326;

            //Create Lambert Azimuthal geometry from centroid of feature
            Geometry lazCentroidOfFeature = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84CentroidOfFeature);

            //Retrieve WKT/GML serialization of features
            Dictionary<string,List<(Geometry,Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those within given radius
            List<RDFResource> featuresWithinDistance = new List<RDFResource>();
            foreach (KeyValuePair<string,List<(Geometry,Geometry)>> featureWithGeometry in featuresWithGeometry)
            {
                //Obviously exclude the given feature itself
                if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                    continue;

                foreach ((Geometry,Geometry) geometryOfFeature in featureWithGeometry.Value)
                    if (geometryOfFeature.Item2.IsWithinDistance(lazCentroidOfFeature, distanceMeters))
                    {
                        featuresWithinDistance.Add(new RDFResource(featureWithGeometry.Key));
                    }
            }

            return RDFQueryUtilities.RemoveDuplicates(featuresWithinDistance);
        }

        public static async Task<List<RDFResource>> GetFeaturesNearBy(OWLOntology ontology, RDFTypedLiteral featureLiteral, double distanceMeters)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features within distance because given \"ontology\" parameter is null");
            if (featureLiteral == null)
                throw new OWLException("Cannot get features within distance because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get features within distance because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Create Lambert Azimuthal geometry from centroid of feature
            Geometry lazCentroidOfFeature = lazGeometry.Centroid;

            //Retrieve WKT/GML serialization of features
            Dictionary<string,List<(Geometry,Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those within given radius
            List<RDFResource> featuresWithinDistance = new List<RDFResource>();
            foreach (KeyValuePair<string,List<(Geometry,Geometry)>> featureWithGeometry in featuresWithGeometry)
            {
                foreach ((Geometry,Geometry) geometryOfFeature in featureWithGeometry.Value)
                    if (geometryOfFeature.Item2.IsWithinDistance(lazCentroidOfFeature, distanceMeters))
                    {
                        featuresWithinDistance.Add(new RDFResource(featureWithGeometry.Key));
                    }
            }

            return RDFQueryUtilities.RemoveDuplicates(featuresWithinDistance);
        }
        #endregion

        #region Analyzer (Direction)
        public static async Task<List<RDFResource>> GetFeaturesDirectionAsync(OWLOntology ontology, RDFResource featureUri, GEOEnums.GeoDirections geoDirection)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features direction because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features direction because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresDirection = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (geometryOfFeature.Item2.Coordinates.Any(c1 => defaultGeometry.Item2.Coordinates.Any(c2 => MatchCoordinates(c1, c2, geoDirection))))
                        {
                            featuresDirection.Add(new RDFResource(featureWithGeometry.Key));
                        }   
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresDirection);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresDirection = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (geometryOfFeature.Item2.Coordinates.Any(c1 => secondaryGeometries.Any(sg => sg.Item2.Coordinates.Any(c2 => MatchCoordinates(c1, c2, geoDirection)))))
                        {
                            featuresDirection.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresDirection);
            }

            return null;
        }

        public static async Task<List<RDFResource>> GetFeaturesDirectionAsync(OWLOntology ontology, RDFTypedLiteral featureLiteral, GEOEnums.GeoDirections geoDirection)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features direction because given \"ontology\" parameter is null");
            if (featureLiteral == null)
                throw new OWLException("Cannot get features direction because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get features direction because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Retrieve WKT/GML serialization of features
            Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

            //Perform spatial analysis between collected geometries
            List<RDFResource> featuresDirection = new List<RDFResource>();
            foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
            {
                foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                    if (geometryOfFeature.Item2.Coordinates.Any(c1 => lazGeometry.Coordinates.Any(c2 => MatchCoordinates(c1, c2, geoDirection))))
                    {
                        featuresDirection.Add(new RDFResource(featureWithGeometry.Key));
                    }
            }

            return RDFQueryUtilities.RemoveDuplicates(featuresDirection);
        }
        #endregion

        #region Analyzer (Interaction)
        public static async Task<List<RDFResource>> GetFeaturesCrossedByAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features interaction because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (defaultGeometry.Item2.Crosses(geometryOfFeature.Item2))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (secondaryGeometries.Any(sg => sg.Item2.Crosses(geometryOfFeature.Item2)))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            return null;
        }

        public static async Task<List<RDFResource>> GetFeaturesCrossedByAsync(OWLOntology ontology, RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureLiteral == null)
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Retrieve WKT/GML serialization of features
            Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

            //Perform spatial analysis between collected geometries
            List<RDFResource> featuresDirectionOf = new List<RDFResource>();
            foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
            {
                foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                    if (lazGeometry.Crosses(geometryOfFeature.Item2))
                    {
                        featuresDirectionOf.Add(new RDFResource(featureWithGeometry.Key));
                    }
            }

            return RDFQueryUtilities.RemoveDuplicates(featuresDirectionOf);
        }

        public static async Task<List<RDFResource>> GetFeaturesTouchedByAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features interaction because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (defaultGeometry.Item2.Touches(geometryOfFeature.Item2))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (secondaryGeometries.Any(sg => sg.Item2.Touches(geometryOfFeature.Item2)))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            return null;
        }

        public static async Task<List<RDFResource>> GetFeaturesTouchedByAsync(OWLOntology ontology, RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureLiteral == null)
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Retrieve WKT/GML serialization of features
            Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

            //Perform spatial analysis between collected geometries
            List<RDFResource> featuresDirectionOf = new List<RDFResource>();
            foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
            {
                foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                    if (lazGeometry.Touches(geometryOfFeature.Item2))
                    {
                        featuresDirectionOf.Add(new RDFResource(featureWithGeometry.Key));
                    }
            }

            return RDFQueryUtilities.RemoveDuplicates(featuresDirectionOf);
        }

        public static async Task<List<RDFResource>> GetFeaturesOverlappedByAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features interaction because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (defaultGeometry.Item2.Overlaps(geometryOfFeature.Item2))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (secondaryGeometries.Any(sg => sg.Item2.Overlaps(geometryOfFeature.Item2)))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            return null;
        }

        public static async Task<List<RDFResource>> GetFeaturesOverlappedByAsync(OWLOntology ontology, RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureLiteral == null)
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Retrieve WKT/GML serialization of features
            Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

            //Perform spatial analysis between collected geometries
            List<RDFResource> featuresDirectionOf = new List<RDFResource>();
            foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
            {
                foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                    if (lazGeometry.Overlaps(geometryOfFeature.Item2))
                    {
                        featuresDirectionOf.Add(new RDFResource(featureWithGeometry.Key));
                    }
            }

            return RDFQueryUtilities.RemoveDuplicates(featuresDirectionOf);
        }

        public static async Task<List<RDFResource>> GetFeaturesWithinAsync(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features interaction because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureUri);
            if (defaultGeometry.Item1 != null && defaultGeometry.Item2 != null)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (defaultGeometry.Item2.Contains(geometryOfFeature.Item2))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                //Retrieve WKT/GML serialization of features
                Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

                //Perform spatial analysis between collected geometries
                List<RDFResource> featuresInteraction = new List<RDFResource>();
                foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
                {
                    //Obviously exclude the given feature itself
                    if (string.Equals(featureWithGeometry.Key, featureUri.ToString()))
                        continue;

                    foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                        if (secondaryGeometries.Any(sg => sg.Item2.Contains(geometryOfFeature.Item2)))
                        {
                            featuresInteraction.Add(new RDFResource(featureWithGeometry.Key));
                        }
                }

                return RDFQueryUtilities.RemoveDuplicates(featuresInteraction);
            }

            return null;
        }

        public static async Task<List<RDFResource>> GetFeaturesWithinAsync(OWLOntology ontology, RDFTypedLiteral featureLiteral)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features interaction because given \"ontology\" parameter is null");
            if (featureLiteral == null)
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is null");
            if (!featureLiteral.HasGeographicDatatype())
                throw new OWLException("Cannot get features interaction because given \"featureLiteral\" parameter is not a geographic typed literal");
            #endregion

            //Transform feature into geometry
            bool isWKT = featureLiteral.Datatype.TargetDatatype == RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT;
            Geometry wgs84Geometry = isWKT ? WKTReader.Read(featureLiteral.Value) : GMLReader.Read(featureLiteral.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Retrieve WKT/GML serialization of features
            Dictionary<string, List<(Geometry, Geometry)>> featuresWithGeometry = await ontology.GetFeaturesWithGeometriesAsync();

            //Perform spatial analysis between collected geometries
            List<RDFResource> featuresDirectionOf = new List<RDFResource>();
            foreach (KeyValuePair<string, List<(Geometry, Geometry)>> featureWithGeometry in featuresWithGeometry)
            {
                foreach ((Geometry, Geometry) geometryOfFeature in featureWithGeometry.Value)
                    if (lazGeometry.Contains(geometryOfFeature.Item2))
                    {
                        featuresDirectionOf.Add(new RDFResource(featureWithGeometry.Key));
                    }
            }

            return RDFQueryUtilities.RemoveDuplicates(featuresDirectionOf);
        }
        #endregion

        #region Utilities
        internal static async Task<Dictionary<string,List<(Geometry wgs84Geom,Geometry lazGeom)>>> GetFeaturesWithGeometriesAsync(this OWLOntology ontology)
        {
            Dictionary<string,List<(Geometry,Geometry)>> featuresWithGeometry = new Dictionary<string,List<(Geometry,Geometry)>>();

            foreach(OWLIndividualExpression featureIdv in OWLAssertionAxiomHelper.GetIndividualsOf(ontology, new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE)))
            {
                RDFResource featureIRI = featureIdv.GetIRI();
                string featureIRIString = featureIRI.ToString();
                if (!featuresWithGeometry.ContainsKey(featureIRIString))
                    featuresWithGeometry.Add(featureIRIString, new List<(Geometry wgs84Geom, Geometry lazGeom)>());

                //Analyze default geometry of feature
                (Geometry wgs84Geom,Geometry lazGeom) defaultGeometry = await ontology.GetDefaultGeometryOfFeatureAsync(featureIRI);
                if (defaultGeometry.wgs84Geom != null && defaultGeometry.lazGeom != null)
                    featuresWithGeometry[featureIRIString].Add(defaultGeometry);

                //Analyze secondary geometries of feature
                List<(Geometry wgs84Geom, Geometry lazGeom)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureIRI);
                if (secondaryGeometries.Count > 0)
                    featuresWithGeometry[featureIRIString].AddRange(secondaryGeometries);
            }

            return featuresWithGeometry;
        }

        internal static async Task<(Geometry wgs84Geom,Geometry lazGeom)> GetDefaultGeometryOfFeatureAsync(this OWLOntology ontology, RDFResource featureUri)
        {
            //Execute SWRL rule to retrieve WKT serialization of the given feature's default geometry
            List<OWLInference> inferences = new List<OWLInference>();
            SWRLRule defaultGeometryAsWKT = new SWRLRule
            {
                Antecedent = new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY), 
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                            new SWRLIndividualArgument(featureUri))
                    }
                },
                Consequent = new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asWKT")),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    }
                }
            };
            inferences.AddRange(await defaultGeometryAsWKT.ApplyToOntologyAsync(ontology));

            //Execute SWRL rule to retrieve GML serialization of the given feature's default geometry
            if (inferences.Count == 0)
            {
                SWRLRule defaultGeometryAsGML = new SWRLRule
                {
                    Antecedent = new SWRLAntecedent
                    {
                        Atoms = new List<SWRLAtom>
                        {
                            new SWRLClassAtom(
                                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                            new SWRLClassAtom(
                                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY), 
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                            new SWRLObjectPropertyAtom(
                                new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                                new SWRLVariableArgument(new RDFVariable("?GML")))
                        },
                        BuiltIns = new List<SWRLBuiltIn>
                        {
                            SWRLBuiltIn.Equal(
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                                new SWRLIndividualArgument(featureUri))
                        }
                    },
                    Consequent = new SWRLConsequent
                    {
                        Atoms = new List<SWRLAtom>
                        {
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asGML")),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                                new SWRLVariableArgument(new RDFVariable("?GML")))
                        }
                    }
                };
                inferences.AddRange(await defaultGeometryAsGML.ApplyToOntologyAsync(ontology));
            }

            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometry
            OWLDataPropertyAssertion inferenceAxiom = (OWLDataPropertyAssertion)inferences.FirstOrDefault()?.Axiom;
            if (string.Equals(inferenceAxiom?.DataProperty.GetIRI().ToString(), "urn:swrl:geosparql:asWKT"))
            {
                try
                {
                    //Parse default geometry from WKT
                    RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                    Geometry wgs84Geometry = WKTReader.Read(inferenceLiteral.Value);
                    wgs84Geometry.SRID = 4326;
                    wgs84Geometry.UserData = inferenceAxiom.IndividualExpression.GetIRI().ToString();

                    //Project default geometry from WGS84 to Lambert Azimuthal
                    Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                    return (wgs84Geometry, lazGeometry);
                }
                catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
            }
            if (string.Equals(inferenceAxiom?.DataProperty.GetIRI().ToString(), "urn:swrl:geosparql:asGML"))
            {
                try
                {
                    //Parse default geometry from GML
                    RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                    Geometry wgs84Geometry = GMLReader.Read(inferenceLiteral.Value);
                    wgs84Geometry.SRID = 4326;
                    wgs84Geometry.UserData = inferenceAxiom.IndividualExpression.GetIRI().ToString();

                    //Project default geometry from WGS84 to Lambert Azimuthal
                    Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);
                    lazGeometry.UserData = inferenceAxiom.IndividualExpression.GetIRI().ToString();

                    return (wgs84Geometry, lazGeometry);
                }
                catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
            }

            return (null,null);
        }

        internal static async Task<List<(Geometry wgs84Geom,Geometry lazGeom)>> GetSecondaryGeometriesOfFeatureAsync(this OWLOntology ontology, RDFResource featureUri)
        {
            List<(Geometry,Geometry)> secondaryGeometries = new List<(Geometry,Geometry)>();

            //Execute SWRL rule to retrieve WKT serialization of the given feature's default geometry
            List<OWLInference> inferences = new List<OWLInference>();
            SWRLRule secondaryGeometriesAsWKT = new SWRLRule
            {
                Antecedent = new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY), 
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                            new SWRLIndividualArgument(featureUri))
                    }
                },
                Consequent = new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asWKT")),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    }
                }
            };
            inferences.AddRange(await secondaryGeometriesAsWKT.ApplyToOntologyAsync(ontology));

            //Execute SWRL rule to retrieve GML serialization of the given feature's default geometry
            if (inferences.Count == 0)
            {
                SWRLRule defaultGeomAsGML = new SWRLRule
                {
                    Antecedent = new SWRLAntecedent
                    {
                        Atoms = new List<SWRLAtom>
                        {
                            new SWRLClassAtom(
                                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                            new SWRLClassAtom(
                                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY), 
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                            new SWRLObjectPropertyAtom(
                                new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                                new SWRLVariableArgument(new RDFVariable("?GML")))
                        },
                        BuiltIns = new List<SWRLBuiltIn>
                        {
                            SWRLBuiltIn.Equal(
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                                new SWRLIndividualArgument(featureUri))
                        }
                    },
                    Consequent = new SWRLConsequent
                    {
                        Atoms = new List<SWRLAtom>
                        {
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asGML")),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                                new SWRLVariableArgument(new RDFVariable("?GML")))
                        }
                    }
                };
                inferences.AddRange(await defaultGeomAsGML.ApplyToOntologyAsync(ontology));
            }
            
            //Parse retrieved WKT/GML serializations into (WGS84,UTM) result secondaryGeometries
            foreach (OWLInference inference in inferences)
            {
                OWLDataPropertyAssertion inferenceAxiom = (OWLDataPropertyAssertion)inference.Axiom;
                if (string.Equals(inferenceAxiom?.DataProperty.GetIRI().ToString(), "urn:swrl:geosparql:asWKT"))
                {
                    try
                    {
                        //Parse default geometry from WKT
                        RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                        Geometry wgs84Geometry = WKTReader.Read(inferenceLiteral.Value);
                        wgs84Geometry.SRID = 4326;
                        wgs84Geometry.UserData = inferenceAxiom.IndividualExpression.GetIRI().ToString();

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);
                        lazGeometry.UserData = inferenceAxiom.IndividualExpression.GetIRI().ToString();

                        secondaryGeometries.Add((wgs84Geometry, lazGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
                if (string.Equals(inferenceAxiom?.DataProperty.GetIRI().ToString(), "urn:swrl:geosparql:asGML"))
                {
                    try
                    {
                        //Parse default geometry from GML
                        RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                        Geometry wgs84Geometry = GMLReader.Read(inferenceLiteral.Value);
                        wgs84Geometry.SRID = 4326;
                        wgs84Geometry.UserData = inferenceAxiom.IndividualExpression.GetIRI().ToString();

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);
                        lazGeometry.UserData = inferenceAxiom.IndividualExpression.GetIRI().ToString();

                        secondaryGeometries.Add((wgs84Geometry, lazGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
            }

            return secondaryGeometries;
        }

        internal static bool MatchCoordinates(Coordinate c1, Coordinate c2, GEOEnums.GeoDirections geoDirection)
        {
            bool answer = false;
            switch (geoDirection)
            {
                case GEOEnums.GeoDirections.North:
                    answer = c1.Y > c2.Y;
                    break;
                case GEOEnums.GeoDirections.East:
                    answer = c1.X > c2.X;
                    break;
                case GEOEnums.GeoDirections.South:
                    answer = c1.Y < c2.Y;
                    break;
                case GEOEnums.GeoDirections.West:
                    answer = c1.X < c2.X;
                    break;
                case GEOEnums.GeoDirections.NorthEast:
                    answer = c1.Y > c2.Y && c1.X > c2.X;
                    break;
                case GEOEnums.GeoDirections.NorthWest:
                    answer = c1.Y > c2.Y && c1.X < c2.X;
                    break;
                case GEOEnums.GeoDirections.SouthEast:
                    answer = c1.Y < c2.Y && c1.X > c2.X;
                    break;
                case GEOEnums.GeoDirections.SouthWest:
                    answer = c1.Y < c2.Y && c1.X < c2.X;
                    break;
            }
            return answer;
        }
        #endregion
    }
}