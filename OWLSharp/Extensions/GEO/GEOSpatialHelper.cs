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

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.GEO
{
    /// <summary>
    /// GEOSpatialHelper represents an helper for spatial analysis on features
    /// </summary>
    public static class GEOSpatialHelper
    {
        #region Properties
        /// <summary>
        /// Reader for WKT spatial representation
        /// </summary>
        internal static WKTReader WKTReader = new WKTReader();

        /// <summary>
        /// Writer for WKT spatial representation
        /// </summary>
        internal static WKTWriter WKTWriter = new WKTWriter();

        /// <summary>
        /// Reader for GML spatial representation
        /// </summary>
        internal static GMLReader GMLReader = new GMLReader();

        /// <summary>
        /// Writer for GML spatial representation
        /// </summary>
        internal static GMLWriter GMLWriter = new GMLWriter();
        #endregion

        #region Methods

        #region Distance
        /// <summary>
        /// Gets the distance, expressed in meters, between the given features
        /// </summary>
        public static double? GetDistanceBetweenFeatures(OWLOntology ontology, RDFResource fromFeatureUri, RDFResource toFeatureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get distance between features because given \"ontology\" parameter is null");
            if (fromFeatureUri == null)
                throw new OWLException("Cannot get distance between features because given \"fromFeatureUri\" parameter is null");
            if (toFeatureUri == null)
                throw new OWLException("Cannot get distance between features because given \"toFeatureUri\" parameter is null");
            #endregion

            //Collect geometries of "From" feature
            (Geometry,Geometry) defaultGeometryOfFromFeature = ontology.GetDefaultGeometryOfFeature(fromFeatureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFromFeature = ontology.GetSecondaryGeometriesOfFeature(fromFeatureUri);
            if (defaultGeometryOfFromFeature.Item1 != null && defaultGeometryOfFromFeature.Item2 != null)
                secondaryGeometriesOfFromFeature.Add(defaultGeometryOfFromFeature);

            //Collect geometries of "To" feature
            (Geometry,Geometry) defaultGeometryOfToFeature = ontology.GetDefaultGeometryOfFeature(toFeatureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfToFeature = ontology.GetSecondaryGeometriesOfFeature(toFeatureUri);
            if (defaultGeometryOfToFeature.Item1 != null && defaultGeometryOfToFeature.Item2 != null)
                secondaryGeometriesOfToFeature.Add(defaultGeometryOfToFeature);

            //Perform spatial analysis between collected geometries (calibrate minimum distance)
            double? featuresDistance = double.MaxValue;
            secondaryGeometriesOfFromFeature.ForEach(fromGeom => {
                secondaryGeometriesOfToFeature.ForEach(toGeom => {
                    double tempDistance = fromGeom.Item2.Distance(toGeom.Item2);
                    if (tempDistance < featuresDistance)
                        featuresDistance = tempDistance;
                });
            });

            //Give null in case distance could not be calculated (no available geometries from any sides)
            return featuresDistance == double.MaxValue ? null : featuresDistance;
        }

        /// <summary>
        /// Gets the distance, expressed in meters, between the given feature and the given WKT feature
        /// </summary>
        public static double? GetDistanceBetweenFeatures(OWLOntology ontology, RDFResource fromFeatureUri, RDFTypedLiteral toFeatureWKT)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get distance between features because given \"ontology\" parameter is null");
            if (fromFeatureUri == null)
                throw new OWLException("Cannot get distance between features because given \"fromFeatureUri\" parameter is null");
            if (toFeatureWKT == null)
                throw new OWLException("Cannot get distance between features because given \"toFeatureWKT\" parameter is null");
            if (!toFeatureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get distance between features because given \"toFeatureWKT\" parameter is not a WKT literal");
            #endregion

            //Collect geometries of "From" feature
            (Geometry, Geometry) defaultGeometryOfFromFeature = ontology.GetDefaultGeometryOfFeature(fromFeatureUri);
            List<(Geometry, Geometry)> secondaryGeometriesOfFromFeature = ontology.GetSecondaryGeometriesOfFeature(fromFeatureUri);
            if (defaultGeometryOfFromFeature.Item1 != null && defaultGeometryOfFromFeature.Item2 != null)
                secondaryGeometriesOfFromFeature.Add(defaultGeometryOfFromFeature);

            //Transform "To" feature into geometry
            Geometry wgs84GeometryTo = WKTReader.Read(toFeatureWKT.Value);
            wgs84GeometryTo.SRID = 4326;
            Geometry lazGeometryTo = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84GeometryTo);

            //Perform spatial analysis between collected geometries (calibrate minimum distance)
            double? featuresDistance = double.MaxValue;
            secondaryGeometriesOfFromFeature.ForEach(fromGeom => {
                double tempDistance = fromGeom.Item2.Distance(lazGeometryTo);
                if (tempDistance < featuresDistance)
                    featuresDistance = tempDistance;
            });

            //Give null in case distance could not be calculated (no available geometries)
            return featuresDistance == double.MaxValue ? null : featuresDistance;
        }

        /// <summary>
        /// Gets the distance, expressed in meters, between the given WKT features
        /// </summary>
        public static double GetDistanceBetweenFeatures(RDFTypedLiteral fromFeatureWKT, RDFTypedLiteral toFeatureWKT)
        {
            #region Guards
            if (fromFeatureWKT == null)
                throw new OWLException("Cannot get distance between features because given \"fromFeatureWKT\" parameter is null");
            if (!fromFeatureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get distance between features because given \"fromFeatureWKT\" parameter is not a WKT literal");
            if (toFeatureWKT == null)
                throw new OWLException("Cannot get distance between features because given \"toFeatureWKT\" parameter is null");
            if (!toFeatureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get distance between features because given \"toFeatureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform "From" feature into geometry
            Geometry wgs84GeometryFrom = WKTReader.Read(fromFeatureWKT.Value);
            wgs84GeometryFrom.SRID = 4326;
            Geometry lazGeometryFrom = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84GeometryFrom);

            //Transform "To" feature into geometry
            Geometry wgs84GeometryTo = WKTReader.Read(toFeatureWKT.Value);
            wgs84GeometryTo.SRID = 4326;
            Geometry lazGeometryTo = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84GeometryTo);

            //Perform spatial analysis between geometries
            return lazGeometryFrom.Distance(lazGeometryTo);
        }
        #endregion

        #region Measure
        /// <summary>
        /// Gets the length, expressed in meters, of the given feature (the perimeter in case of area)
        /// </summary>
        public static double? GetLengthOfFeature(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get length of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get length of feature because given \"featureUri\" parameter is null");
            #endregion

            //Collect geometries of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Insert(0, defaultGeometryOfFeature);

            //Perform spatial analysis between collected geometries (calibrate maximum length)
            double? featureLength = double.MinValue;
            secondaryGeometriesOfFeature.ForEach(geom => {
                double tempLength = geom.Item2.Length;
                if (tempLength > featureLength)
                    featureLength = tempLength;
            });

            //Give null in case length could not be calculated (no available geometries)
            return featureLength == double.MinValue ? null : featureLength;
        }

        /// <summary>
        /// Gets the length, expressed in meters, of the given WKT feature (the perimeter in case of area)
        /// </summary>
        public static double GetLengthOfFeature(RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (featureWKT == null)
                throw new OWLException("Cannot get length of feature because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get length of feature because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            return lazGeometry.Length;
        }

        /// <summary>
        /// Gets the area, expressed in square meters, of the given feature
        /// </summary>
        public static double? GetAreaOfFeature(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get area of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get area of feature because given \"featureUri\" parameter is null");
            #endregion

            //Collect geometries of feature
            (Geometry, Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry, Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Insert(0, defaultGeometryOfFeature);

            //Perform spatial analysis between collected geometries (calibrate maximum area)
            double? featureArea = double.MinValue;
            secondaryGeometriesOfFeature.ForEach(geom => {
                double tempArea = geom.Item2.Area;
                if (tempArea > featureArea)
                    featureArea = tempArea;
            });

            //Give null in case area could not be calculated (no available geometries)
            return featureArea == double.MinValue ? null : featureArea;
        }

        /// <summary>
        /// Gets the area, expressed in square meters, of the given WKT feature
        /// </summary>
        public static double GetAreaOfFeature(RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (featureWKT == null)
                throw new OWLException("Cannot get area of feature because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get area of feature because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            return lazGeometry.Area;
        }
        #endregion

        #region Centroid
        /// <summary>
        /// Calculates the centroid of the given feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetCentroidOfFeature(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get centroid of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get centroid of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
            {
                Geometry centroidGeometryAZ = defaultGeometryOfFeature.Item2.Centroid;
                Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
                string wktCentroidGeometryWGS84 = WKTWriter.Write(centroidGeometryWGS84);
                return new RDFTypedLiteral(wktCentroidGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature: if any, just work on the first available
            List<(Geometry, Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (secondaryGeometriesOfFeature.Any())
            {
                Geometry centroidGeometryAZ = secondaryGeometriesOfFeature.First().Item2.Centroid;
                Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
                string wktCentroidGeometryWGS84 = WKTWriter.Write(centroidGeometryWGS84);
                return new RDFTypedLiteral(wktCentroidGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        /// <summary>
        /// Calculates the centroid of the given WKT feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetCentroidOfFeature(RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (featureWKT == null)
                throw new OWLException("Cannot get centroid of feature because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get centroid of feature because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry centroidGeometryAZ = lazGeometry.Centroid;
            Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
            string wktCentroidGeometryWGS84 = WKTWriter.Write(centroidGeometryWGS84);
            return new RDFTypedLiteral(wktCentroidGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
        }
        #endregion

        #region Boundary
        /// <summary>
        /// Calculates the boundaries of the given feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetBoundaryOfFeature(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get boundary of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get boundary of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
            {
                Geometry boundaryGeometryAZ = defaultGeometryOfFeature.Item2.Boundary;
                Geometry boundaryGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
                string wktBoundaryGeometryWGS84 = WKTWriter.Write(boundaryGeometryWGS84)
                                                    .Replace("LINEARRING", "LINESTRING");
                return new RDFTypedLiteral(wktBoundaryGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature: if any, just work on the first available
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (secondaryGeometriesOfFeature.Any())
            {
                Geometry boundaryGeometryAZ = secondaryGeometriesOfFeature.First().Item2.Boundary;
                Geometry boundaryGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
                string wktBoundaryGeometryWGS84 = WKTWriter.Write(boundaryGeometryWGS84)
                                                    .Replace("LINEARRING", "LINESTRING");
                return new RDFTypedLiteral(wktBoundaryGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        /// <summary>
        /// Calculates the boundaries of the given WKT feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetBoundaryOfFeature(RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (featureWKT == null)
                throw new OWLException("Cannot get boundary of feature because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get boundary of feature because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry boundaryGeometryAZ = lazGeometry.Boundary;
            Geometry boundaryGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
            string wktBoundaryGeometryWGS84 = WKTWriter.Write(boundaryGeometryWGS84)
                                                .Replace("LINEARRING", "LINESTRING");
            return new RDFTypedLiteral(wktBoundaryGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
        }
        #endregion

        #region Buffer
        /// <summary>
        /// Calculates a buffer of the given meters on the given feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetBufferAroundFeature(OWLOntology ontology, RDFResource featureUri, double bufferMeters)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get buffer around feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get buffer around feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
            {
                Geometry bufferGeometryAZ = defaultGeometryOfFeature.Item2.Buffer(bufferMeters);
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                string wktBufferGeometryWGS84 = WKTWriter.Write(bufferGeometryWGS84);
                return new RDFTypedLiteral(wktBufferGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature: if any, just work on the first available
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (secondaryGeometriesOfFeature.Any())
            {
                Geometry bufferGeometryAZ = secondaryGeometriesOfFeature.First().Item2.Buffer(bufferMeters);
                Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
                string wktBufferGeometryWGS84 = WKTWriter.Write(bufferGeometryWGS84);
                return new RDFTypedLiteral(wktBufferGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        /// <summary>
        /// Calculates a buffer of the given meters on the given WKT feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetBufferAroundFeature(RDFTypedLiteral featureWKT, double bufferMeters)
        {
            #region Guards
            if (featureWKT == null)
                throw new OWLException("Cannot get buffer around feature because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get buffer around feature because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry bufferGeometryAZ = lazGeometry.Buffer(bufferMeters);
            Geometry bufferGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryAZ);
            string wktBufferGeometryWGS84 = WKTWriter.Write(bufferGeometryWGS84);
            return new RDFTypedLiteral(wktBufferGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
        }
        #endregion

        #region ConvexHull
        /// <summary>
        /// Calculates the ConvexHull (smallest convex polygon containing) of the given feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetConvexHullOfFeature(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get convex hull of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get convex hull of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
            {
                Geometry convexHullGeometryAZ = defaultGeometryOfFeature.Item2.ConvexHull();
                Geometry convexHullGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(convexHullGeometryAZ);
                string wktConvexHullGeometryWGS84 = WKTWriter.Write(convexHullGeometryWGS84);
                return new RDFTypedLiteral(wktConvexHullGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature: if any, just work on the first available
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (secondaryGeometriesOfFeature.Any())
            {
                Geometry convexHullGeometryAZ = secondaryGeometriesOfFeature.First().Item2.ConvexHull();
                Geometry convexHullGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(convexHullGeometryAZ);
                string wktConvexHullGeometryWGS84 = WKTWriter.Write(convexHullGeometryWGS84);
                return new RDFTypedLiteral(wktConvexHullGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        /// <summary>
        /// Calculates the ConvexHull (smallest convex polygon containing) of the given WKT feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetConvexHullOfFeature(RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (featureWKT == null)
                throw new OWLException("Cannot get convex hull of feature because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get convex hull of feature because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry convexHullGeometryAZ = lazGeometry.ConvexHull();
            Geometry convexHullGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(convexHullGeometryAZ);
            string wktConvexHullGeometryWGS84 = WKTWriter.Write(convexHullGeometryWGS84);
            return new RDFTypedLiteral(wktConvexHullGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
        }
        #endregion

        #region Envelope
        /// <summary>
        /// Calculates the envelope (bounding-box) of the given feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetEnvelopeOfFeature(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get envelope of feature because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get envelope of feature because given \"featureUri\" parameter is null");
            #endregion

            //Analyze default geometry of feature
            (Geometry, Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
            {
                Geometry envelopeGeometryAZ = defaultGeometryOfFeature.Item2.Envelope;
                Geometry envelopeGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(envelopeGeometryAZ);
                string wktEnvelopeGeometryWGS84 = WKTWriter.Write(envelopeGeometryWGS84);
                return new RDFTypedLiteral(wktEnvelopeGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature: if any, just work on the first available
            List<(Geometry, Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (secondaryGeometriesOfFeature.Any())
            {
                Geometry envelopeGeometryAZ = secondaryGeometriesOfFeature.First().Item2.Envelope;
                Geometry envelopeGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(envelopeGeometryAZ);
                string wktEnvelopeGeometryWGS84 = WKTWriter.Write(envelopeGeometryWGS84);
                return new RDFTypedLiteral(wktEnvelopeGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        /// <summary>
        /// Calculates the envelope (bounding-box) of the given WKT feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public static RDFTypedLiteral GetEnvelopeOfFeature(RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (featureWKT == null)
                throw new OWLException("Cannot get envelope of feature because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get envelope of feature because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Analyze geometry
            Geometry envelopeGeometryAZ = lazGeometry.Envelope;
            Geometry envelopeGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(envelopeGeometryAZ);
            string wktEnvelopeGeometryWGS84 = WKTWriter.Write(envelopeGeometryWGS84);
            return new RDFTypedLiteral(wktEnvelopeGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
        }
        #endregion

        #region Proximity
        /// <summary>
        /// Gets the features near the given feature within a radius of given meters 
        /// </summary>
        public static List<RDFResource> GetFeaturesNearBy(OWLOntology ontology, RDFResource featureUri, double radiusMeters)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features nearby because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features nearby because given \"featureUri\" parameter is null");
            #endregion

            //Get centroid of feature
            RDFTypedLiteral centroidOfFeature = GetCentroidOfFeature(ontology, featureUri);
            if (centroidOfFeature == null)
                return null;

            //Create WGS84 geometry from centroid of feature
            Geometry wgs84CentroidOfFeature = WKTReader.Read(centroidOfFeature.Value);
            wgs84CentroidOfFeature.SRID = 4326;

            //Create Lambert Azimuthal geometry from centroid of feature
            Geometry lazCentroidOfFeature = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84CentroidOfFeature);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries()
                .Where(ft => !ft.Item1.Equals(featureUri)).ToList();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those within given radius
            List<RDFResource> featuresNearBy = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.IsWithinDistance(lazCentroidOfFeature, radiusMeters))
                    featuresNearBy.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNearBy);
        }

        /// <summary>
        /// Gets the features near the given WGS84 Lon/Lat point within a radius of given meters 
        /// </summary>
        public static List<RDFResource> GetFeaturesNearPoint(OWLOntology ontology, (double,double) wgs84LonLat, double radiusMeters)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features near point because given \"ontology\" parameter is null");
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");
            #endregion

            //Create WGS84 geometry from given center of search
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Create Lambert Azimuthal geometry from given center of search
            Geometry lazSearchPoint = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84SearchPoint);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those within given radius
            List<RDFResource> featuresNearPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.IsWithinDistance(lazSearchPoint, radiusMeters))
                    featuresNearPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNearPoint);
        }
        #endregion

        #region Direction
        /// <summary>
        /// Gets the features located north of the given WGS84 Lon/Lat point
        /// </summary>
        public static List<RDFResource> GetFeaturesNorthOfPoint(OWLOntology ontology, (double,double) wgs84LonLat)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features north of point because given \"ontology\" parameter is null");
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLException("Cannot get features north of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLException("Cannot get features north of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");
            #endregion

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having latitudes higher than given point
            List<RDFResource> featuresNorthOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.Y > wgs84SearchPoint.Coordinate.Y))
                    featuresNorthOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNorthOfPoint);
        }

        /// <summary>
        /// Gets the features located east of the given WGS84 Lon/Lat point
        /// </summary>
        public static List<RDFResource> GetFeaturesEastOfPoint(OWLOntology ontology, (double,double) wgs84LonLat)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features east of point because given \"ontology\" parameter is null");
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLException("Cannot get features east of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLException("Cannot get features east of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");
            #endregion

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having longitudes greater than given point
            List<RDFResource> featuresEastOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.X > wgs84SearchPoint.Coordinate.X))
                    featuresEastOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresEastOfPoint);
        }

        /// <summary>
        /// Gets the features located west of the given WGS84 Lon/Lat point
        /// </summary>
        public static List<RDFResource> GetFeaturesWestOfPoint(OWLOntology ontology, (double,double) wgs84LonLat)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features west of point because given \"ontology\" parameter is null");
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLException("Cannot get features west of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLException("Cannot get features west of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");
            #endregion

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having longitudes lower than given point
            List<RDFResource> featuresWestOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.X < wgs84SearchPoint.Coordinate.X))
                    featuresWestOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresWestOfPoint);
        }

        /// <summary>
        /// Gets the features located south of the given WGS84 Lon/Lat point
        /// </summary>
        public static List<RDFResource> GetFeaturesSouthOfPoint(OWLOntology ontology, (double,double) wgs84LonLat)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features south of point because given \"ontology\" parameter is null");
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLException("Cannot get features south of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLException("Cannot get features south of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");
            #endregion

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having latitudes lower than given point
            List<RDFResource> featuresSouthOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.Y < wgs84SearchPoint.Coordinate.Y))
                    featuresSouthOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresSouthOfPoint);
        }
        #endregion

        #region Box
        /// <summary>
        /// Gets the features inside the given box represented by WGS84 Lon/Lat (lower-left, upper-right) corner points
        /// </summary>
        public static List<RDFResource> GetFeaturesInsideBox(OWLOntology ontology, (double,double) wgs84LonLat_LowerLeft, (double,double) wgs84LonLat_UpperRight)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features inside box because given \"ontology\" parameter is null");
            if (wgs84LonLat_LowerLeft.Item1 < -180 || wgs84LonLat_LowerLeft.Item1 > 180)
                throw new OWLException("Cannot get features inside box because given \"wgs84LonMin\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item2 < -90 || wgs84LonLat_LowerLeft.Item2 > 90)
                throw new OWLException("Cannot get features inside box because given \"wgs84LatMin\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_UpperRight.Item1 < -180 || wgs84LonLat_UpperRight.Item1 > 180)
                throw new OWLException("Cannot get features inside box because given \"wgs84LonMax\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_UpperRight.Item2 < -90 || wgs84LonLat_UpperRight.Item2 > 90)
                throw new OWLException("Cannot get features inside box because given \"wgs84LatMax\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item1 >= wgs84LonLat_UpperRight.Item1)
                throw new OWLException("Cannot get features inside box because given \"wgs84LonMin\" parameter must be lower than given \"wgs84LonMax\" parameter");
            if (wgs84LonLat_LowerLeft.Item2 >= wgs84LonLat_UpperRight.Item2)
                throw new OWLException("Cannot get features inside box because given \"wgs84LatMin\" parameter must be lower than given \"wgs84LatMax\" parameter");
            #endregion

            //Create WGS84 geometry from given box corners
            Geometry wgs84SearchBox = new Polygon(new LinearRing(new Coordinate[] {
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2)
            })) { SRID = 4326 };

            //Create Lambert Azimuthal geometry from given box corners
            Geometry lazSearchBox = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84SearchBox);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those inside given box
            List<RDFResource> featuresInsideBox = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (lazSearchBox.Contains(featureWithGeometry.Item3))
                    featuresInsideBox.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresInsideBox);
        }

        /// <summary>
        /// Gets the features outside the given box represented by WGS84 Lon/Lat (lower-left, upper-right) corner points
        /// </summary>
        public static List<RDFResource> GetFeaturesOutsideBox(OWLOntology ontology, (double, double) wgs84LonLat_LowerLeft, (double, double) wgs84LonLat_UpperRight)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features outside box because given \"ontology\" parameter is null");
            if (wgs84LonLat_LowerLeft.Item1 < -180 || wgs84LonLat_LowerLeft.Item1 > 180)
                throw new OWLException("Cannot get features outside box because given \"wgs84LonMin\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item2 < -90 || wgs84LonLat_LowerLeft.Item2 > 90)
                throw new OWLException("Cannot get features outside box because given \"wgs84LatMin\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_UpperRight.Item1 < -180 || wgs84LonLat_UpperRight.Item1 > 180)
                throw new OWLException("Cannot get features outside box because given \"wgs84LonMax\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_UpperRight.Item2 < -90 || wgs84LonLat_UpperRight.Item2 > 90)
                throw new OWLException("Cannot get features outside box because given \"wgs84LatMax\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item1 >= wgs84LonLat_UpperRight.Item1)
                throw new OWLException("Cannot get features outside box because given \"wgs84LonMin\" parameter must be lower than given \"wgs84LonMax\" parameter");
            if (wgs84LonLat_LowerLeft.Item2 >= wgs84LonLat_UpperRight.Item2)
                throw new OWLException("Cannot get features outside box because given \"wgs84LatMin\" parameter must be lower than given \"wgs84LatMax\" parameter");
            #endregion

            //Create WGS84 geometry from given box corners
            Geometry wgs84SearchBox = new Polygon(new LinearRing(new Coordinate[] {
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2)
            })) { SRID = 4326 };

            //Create Lambert Azimuthal geometry from given box corners
            Geometry lazSearchBox = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84SearchBox);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those outside given box
            List<RDFResource> featuresOutsideBox = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (!lazSearchBox.Contains(featureWithGeometry.Item3))
                    featuresOutsideBox.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresOutsideBox);
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Gets the features crossed by the given feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesCrossedBy(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features crossedby because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features crossedby because given \"featureUri\" parameter is null");
            #endregion

            //Collect geometries of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Insert(0, defaultGeometryOfFeature);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries()
                .Where(ft => !ft.Item1.Equals(featureUri)).ToList();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those crossed by given one
            List<RDFResource> featuresCrossedBy = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                secondaryGeometriesOfFeature.ForEach(ftGeom => {
                    if (ftGeom.Item2.Crosses(featureWithGeometry.Item3))
                        featuresCrossedBy.Add(featureWithGeometry.Item1);
                });
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresCrossedBy);
        }

        /// <summary>
        /// Gets the features crossed by the given WKT feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesCrossedBy(OWLOntology ontology, RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features crossedby because given \"ontology\" parameter is null");
            if (featureWKT == null)
                throw new OWLException("Cannot get features crossedby because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get features crossedby because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between retrieved geometries:
            //iterate geometries and collect those crossed by given one
            List<RDFResource> featuresCrossedBy = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (lazGeometry.Crosses(featureWithGeometry.Item3))
                    featuresCrossedBy.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresCrossedBy);
        }

        /// <summary>
        /// Gets the features touched by the given feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesTouchedBy(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features touchedby because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features touchedby because given \"featureUri\" parameter is null");
            #endregion

            //Collect geometries of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Insert(0, defaultGeometryOfFeature);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries()
                .Where(ft => !ft.Item1.Equals(featureUri)).ToList();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those touched by given one
            List<RDFResource> featuresTouchedBy = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                secondaryGeometriesOfFeature.ForEach(ftGeom => {
                    if (ftGeom.Item2.Touches(featureWithGeometry.Item3))
                        featuresTouchedBy.Add(featureWithGeometry.Item1);
                });
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresTouchedBy);
        }

        /// <summary>
        /// Gets the features touched by the given WKT feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesTouchedBy(OWLOntology ontology, RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features touchedby because given \"ontology\" parameter is null");
            if (featureWKT == null)
                throw new OWLException("Cannot get features touchedby because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get features touchedby because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between retrieved geometries:
            //iterate geometries and collect those touched by given one
            List<RDFResource> featuresTouchedBy = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (lazGeometry.Touches(featureWithGeometry.Item3))
                    featuresTouchedBy.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresTouchedBy);
        }

        /// <summary>
        /// Gets the features overlapped by the given feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesOverlappedBy(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features overlappedby because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features overlappedby because given \"featureUri\" parameter is null");
            #endregion

            //Collect geometries of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Insert(0, defaultGeometryOfFeature);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries()
                .Where(ft => !ft.Item1.Equals(featureUri)).ToList();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those overlapped by given one
            List<RDFResource> featuresOverlappedBy = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                secondaryGeometriesOfFeature.ForEach(ftGeom => {
                    if (ftGeom.Item2.Overlaps(featureWithGeometry.Item3))
                        featuresOverlappedBy.Add(featureWithGeometry.Item1);
                });
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresOverlappedBy);
        }

        /// <summary>
        /// Gets the features overlapped by the given WKT feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesOverlappedBy(OWLOntology ontology, RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features overlappedby because given \"ontology\" parameter is null");
            if (featureWKT == null)
                throw new OWLException("Cannot get features overlappedby because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get features overlappedby because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between retrieved geometries:
            //iterate geometries and collect those overlapped by given one
            List<RDFResource> featuresOverlappedBy = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (lazGeometry.Overlaps(featureWithGeometry.Item3))
                    featuresOverlappedBy.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresOverlappedBy);
        }

        /// <summary>
        /// Gets the features within the given feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesWithin(OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features within because given \"ontology\" parameter is null");
            if (featureUri == null)
                throw new OWLException("Cannot get features within because given \"featureUri\" parameter is null");
            #endregion

            //Collect geometries of feature
            (Geometry,Geometry) defaultGeometryOfFeature = ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Insert(0, defaultGeometryOfFeature);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries()
                .Where(ft => !ft.Item1.Equals(featureUri)).ToList();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those within the given one
            List<RDFResource> featuresWithin = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                secondaryGeometriesOfFeature.ForEach(ftGeom => {
                    if (featureWithGeometry.Item3.Within(ftGeom.Item2))
                        featuresWithin.Add(featureWithGeometry.Item1);
                });
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresWithin);
        }

        /// <summary>
        /// Gets the features within the given WKT feature 
        /// </summary>
        public static List<RDFResource> GetFeaturesWithin(OWLOntology ontology, RDFTypedLiteral featureWKT)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot get features within because given \"ontology\" parameter is null");
            if (featureWKT == null)
                throw new OWLException("Cannot get features within because given \"featureWKT\" parameter is null");
            if (!featureWKT.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                throw new OWLException("Cannot get features within because given \"featureWKT\" parameter is not a WKT literal");
            #endregion

            //Transform feature into geometry
            Geometry wgs84Geometry = WKTReader.Read(featureWKT.Value);
            wgs84Geometry.SRID = 4326;
            Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between retrieved geometries:
            //iterate geometries and collect those within the given one
            List<RDFResource> featuresWithin = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.Within(lazGeometry))
                    featuresWithin.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresWithin);
        }
        #endregion

        #endregion
    }
}