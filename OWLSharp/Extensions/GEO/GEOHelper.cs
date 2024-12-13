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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Rules;
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

        #region Properties
        internal static WKTReader WKTReader = new WKTReader();
        internal static WKTWriter WKTWriter = new WKTWriter();
        internal static GMLReader GMLReader = new GMLReader();
        internal static GMLWriter GMLWriter = new GMLWriter();
        #endregion

        #region Methods (Distance)
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

        #region Methods (Length/Area)
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

        #region Methods (Centroid)
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

        #region Methods (Boundary)
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
                Geometry centroidGeometryAZ = defaultGeometry.Item2.Boundary;
                Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(centroidGeometryWGS84).Replace("LINEARRING", "LINESTRING"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature
            List<(Geometry, Geometry)> secondaryGeometries = await ontology.GetSecondaryGeometriesOfFeatureAsync(featureUri);
            if (secondaryGeometries.Count > 0)
            {
                Geometry centroidGeometryAZ = secondaryGeometries.First().Item2.Boundary;
                Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
                return new RDFTypedLiteral(WKTWriter.Write(centroidGeometryWGS84).Replace("LINEARRING", "LINESTRING"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
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
            Geometry centroidGeometryAZ = lazGeometry.Boundary;
            Geometry centroidGeometryWGS84 = RDFGeoConverter.GetWGS84GeometryFromLambertAzimuthal(centroidGeometryAZ);
            return await Task.FromResult(new RDFTypedLiteral(WKTWriter.Write(centroidGeometryWGS84).Replace("LINEARRING", "LINESTRING"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
        }
        #endregion

        #region Utilities
        internal static async Task<(Geometry,Geometry)> GetDefaultGeometryOfFeatureAsync(this OWLOntology ontology, RDFResource featureUri)
        {
            //Execute SWRL rule to retrieve WKT serialization of the given feature's default geometry
            List<OWLInference> inferences = new List<OWLInference>();
            SWRLRule defaultGeometryAsWKT = new SWRLRule()
            {
                Antecedent = new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                            new SWRLIndividualArgument(featureUri))
                    }
                },
                Consequent = new SWRLConsequent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asWKT")),
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    }
                }
            };
            inferences.AddRange(await defaultGeometryAsWKT.ApplyToOntologyAsync(ontology));

            //Execute SWRL rule to retrieve GML serialization of the given feature's default geometry
            if (inferences.Count == 0)
            {
                SWRLRule defaultGeometryAsGML = new SWRLRule()
                {
                    Antecedent = new SWRLAntecedent()
                    {
                        Atoms = new List<SWRLAtom>()
                        {
                            new SWRLClassAtom(
                                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                            new SWRLObjectPropertyAtom(
                                new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                                new SWRLVariableArgument(new RDFVariable("?GML")))
                        },
                        BuiltIns = new List<SWRLBuiltIn>()
                        {
                            SWRLBuiltIn.Equal(
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                                new SWRLIndividualArgument(featureUri))
                        }
                    },
                    Consequent = new SWRLConsequent()
                    {
                        Atoms = new List<SWRLAtom>()
                        {
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asGML")),
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")),
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

                    //Project default geometry from WGS84 to Lambert Azimuthal
                    Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                    return (wgs84Geometry, lazGeometry);
                }
                catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
            }

            return (null,null);
        }

        internal static async Task<List<(Geometry,Geometry)>> GetSecondaryGeometriesOfFeatureAsync(this OWLOntology ontology, RDFResource featureUri)
        {
            List<(Geometry,Geometry)> secondaryGeometries = new List<(Geometry,Geometry)>();

            //Execute SWRL rule to retrieve WKT serialization of the given feature's default geometry
            List<OWLInference> inferences = new List<OWLInference>();
            SWRLRule secondaryGeometriesAsWKT = new SWRLRule()
            {
                Antecedent = new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY), 
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                            new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                            new SWRLIndividualArgument(featureUri))
                    }
                },
                Consequent = new SWRLConsequent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asWKT")),
                            new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                            new SWRLVariableArgument(new RDFVariable("?WKT")))
                    }
                }
            };
            inferences.AddRange(await secondaryGeometriesAsWKT.ApplyToOntologyAsync(ontology));

            //Execute SWRL rule to retrieve GML serialization of the given feature's default geometry
            if (inferences.Count == 0)
            {
                SWRLRule defaultGeomAsGML = new SWRLRule()
                {
                    Antecedent = new SWRLAntecedent()
                    {
                        Atoms = new List<SWRLAtom>()
                        {
                            new SWRLClassAtom(
                                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE"))),
                            new SWRLObjectPropertyAtom(
                                new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY), 
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY"))),
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_GML),
                                new SWRLVariableArgument(new RDFVariable("?GEOMETRY")),
                                new SWRLVariableArgument(new RDFVariable("?GML")))
                        },
                        BuiltIns = new List<SWRLBuiltIn>()
                        {
                            SWRLBuiltIn.Equal(
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")), 
                                new SWRLIndividualArgument(featureUri))
                        }
                    },
                    Consequent = new SWRLConsequent()
                    {
                        Atoms = new List<SWRLAtom>()
                        {
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:swrl:geosparql:asGML")),
                                new SWRLVariableArgument(new RDFVariable("?FEATURE")),
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

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

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

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                        secondaryGeometries.Add((wgs84Geometry, lazGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
            }

            return secondaryGeometries;
        }
        #endregion
    }
}