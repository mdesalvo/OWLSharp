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

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
using NetTopologySuite.IO.GML3;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace OWLSharp.Extensions.GEO
{
    /// <summary>
    /// GEOOntologyHelper contains methods for declaring and analyzing relations describing GeoSPARQL geometries
    /// </summary>
    public static class GEOOntologyHelper
    {
        // WGS84 uses LON/LAT coordinates
        // LON => X (West/East, -180->180)
        // LAT => Y (North/South, -90->90)

        #region Declarer
        /// <summary>
        /// Declares the given point feature to the spatial ontology (coordinates of the geometry must be WGS84 Lon/Lat)
        /// </summary>
        public static OWLOntology DeclarePointFeature(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, (double,double) wgs84Point, bool isDefaultGeometry)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare point feature to the spatial ontology because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare point feature to the spatial ontology because given \"geometryUri\" parameter is null");
            if (wgs84Point.Item1 < -180 || wgs84Point.Item1 > 180)
                throw new OWLException("Cannot declare point feature to the spatial ontology because given \"geometry\" parameter has not a valid longitude for WGS84");
            if (wgs84Point.Item2 < -90 || wgs84Point.Item2 > 90)
                throw new OWLException("Cannot declare point feature to the spatial ontology because given \"geometry\" parameter has not a valid latitude for WGS84");
            #endregion

            return DeclarePointFeatureInternal(ontology, featureUri, geometryUri, new Point(wgs84Point.Item1, wgs84Point.Item2) { SRID = 4326 }, isDefaultGeometry);
        }
        internal static OWLOntology DeclarePointFeatureInternal(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, Point geometry, bool isDefaultGeometry)
        {
            //Build sf:Point serializations
            string wgs84PointWKT = GEOSpatialHelper.WKTWriter.Write(geometry);
            string wgs84PointGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84PointGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(featureUri);
            ontology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);
            ontology.Data.DeclareObjectAssertion(featureUri, isDefaultGeometry ? RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY : RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);
            ontology.Data.DeclareIndividual(geometryUri);
            ontology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.SF.POINT);
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84PointWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84PointGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return ontology;
        }

        /// <summary>
        /// Declares the given line feature to the spatial ontology (coordinates of the geometry must be WGS84 Lon/Lat)
        /// </summary>
        public static OWLOntology DeclareLineFeature(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, List<(double,double)> wgs84Line, bool isDefaultGeometry)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare line feature to the spatial ontology because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare line feature to the spatial ontology because given \"geometryUri\" parameter is null");
            if (wgs84Line == null)
                throw new OWLException("Cannot declare line feature to the spatial ontology because given \"wgs84Line\" parameter is null");
            if (wgs84Line.Count < 2)
                throw new OWLException("Cannot declare line feature to the spatial ontology because given \"wgs84Line\" parameter contains less than 2 points");
            if (wgs84Line.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLException("Cannot declare line feature to the spatial ontology because given \"wgs84Line\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84Line.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLException("Cannot declare line feature to the spatial ontology because given \"wgs84Line\" parameter contains a point with invalid latitude for WGS84");
            #endregion

            return DeclareLineFeatureInternal(ontology, featureUri, geometryUri, new LineString(wgs84Line.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray()) { SRID = 4326 }, isDefaultGeometry);
        }
        internal static OWLOntology DeclareLineFeatureInternal(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, LineString geometry, bool isDefaultGeometry)
        {
            //Build sf:LineString serializations
            string wgs84LineStringWKT = GEOSpatialHelper.WKTWriter.Write(geometry);
            string wgs84LineStringGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84LineStringGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(featureUri);
            ontology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);
            ontology.Data.DeclareObjectAssertion(featureUri, isDefaultGeometry ? RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY : RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);
            ontology.Data.DeclareIndividual(geometryUri);
            ontology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84LineStringWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84LineStringGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return ontology;
        }

        /// <summary>
        /// Declares the given area feature to the spatial ontology (coordinates of the geometry must be WGS84 Lon/Lat)
        /// </summary>
        public static OWLOntology DeclareAreaFeature(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, List<(double,double)> wgs84Area, bool isDefaultGeometry)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare area feature to the spatial ontology because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare area feature to the spatial ontology because given \"geometryUri\" parameter is null");
            if (wgs84Area == null)
                throw new OWLException("Cannot declare area feature to the spatial ontology because given \"wgs84Area\" parameter is null");
            if (wgs84Area.Count < 3)
                throw new OWLException("Cannot declare area feature to the spatial ontology because given \"wgs84Area\" parameter contains less than 3 points");
            if (wgs84Area.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLException("Cannot declare area feature to the spatial ontology because given \"wgs84Area\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84Area.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLException("Cannot declare area feature to the spatial ontology because given \"wgs84Area\" parameter contains a point with invalid latitude for WGS84");
            #endregion

            //Automatically close polygon (if needed)
            if (wgs84Area[0].Item1 != wgs84Area[wgs84Area.Count-1].Item1
                 && wgs84Area[0].Item2 != wgs84Area[wgs84Area.Count-1].Item2)
                wgs84Area.Add(wgs84Area[0]);

            return DeclareAreaFeatureInternal(ontology, featureUri, geometryUri, new Polygon(new LinearRing(wgs84Area.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray())) { SRID = 4326 }, isDefaultGeometry);
        }
        internal static OWLOntology DeclareAreaFeatureInternal(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, Polygon geometry, bool isDefaultGeometry)
        {
            //Build sf:Polygon serializations
            string wgs84PolygonWKT = GEOSpatialHelper.WKTWriter.Write(geometry);
            string wgs84PolygonGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84PolygonGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(featureUri);
            ontology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);
            ontology.Data.DeclareObjectAssertion(featureUri, isDefaultGeometry ? RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY : RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);
            ontology.Data.DeclareIndividual(geometryUri);
            ontology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.SF.POLYGON);
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84PolygonWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84PolygonGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return ontology;
        }

        /// <summary>
        /// Declares the given multipoint feature to the spatial ontology (coordinates of the geometries must be WGS84 Lon/Lat)
        /// </summary>
        public static OWLOntology DeclareMultiPointFeature(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, List<(double,double)> wgs84Points, bool isDefaultGeometry)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare multipoint feature to the spatial ontology because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare multipoint feature to the spatial ontology because given \"geometryUri\" parameter is null");
            if (wgs84Points == null)
                throw new OWLException("Cannot declare multipoint feature to the spatial ontology because given \"wgs84Points\" parameter is null");
            if (wgs84Points.Count < 2)
                throw new OWLException("Cannot declare multipoint feature to the spatial ontology because given \"wgs84Points\" parameter contains less than 2 points");
            if (wgs84Points.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLException("Cannot declare multipoint feature to the spatial ontology because given \"wgs84Points\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84Points.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLException("Cannot declare multipoint feature to the spatial ontology because given \"wgs84Points\" parameter contains a point with invalid latitude for WGS84");
            #endregion

            return DeclareMultiPointFeatureInternal(ontology, featureUri, geometryUri, new MultiPoint(wgs84Points.Select(wgs84Point => new Point(wgs84Point.Item1, wgs84Point.Item2) { SRID = 4326 }).ToArray()) { SRID = 4326 }, isDefaultGeometry);
        }
        internal static OWLOntology DeclareMultiPointFeatureInternal(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, MultiPoint geometry, bool isDefaultGeometry)
        {
            //Build sf:MultiPoint serializations
            string wgs84MultiPointWKT = GEOSpatialHelper.WKTWriter.Write(geometry);
            string wgs84MultiPointGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84MultiPointGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(featureUri);
            ontology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);
            ontology.Data.DeclareObjectAssertion(featureUri, isDefaultGeometry ? RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY : RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);
            ontology.Data.DeclareIndividual(geometryUri);
            ontology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.SF.MULTI_POINT);
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84MultiPointWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84MultiPointGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return ontology;
        }

        /// <summary>
        /// Declares the given multiline feature to the spatial ontology (coordinates of the geometries must be WGS84 Lon/Lat)
        /// </summary>
        public static OWLOntology DeclareMultiLineFeature(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, List<List<(double,double)>> wgs84Lines, bool isDefaultGeometry)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"geometryUri\" parameter is null");
            if (wgs84Lines == null)
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"wgs84Lines\" parameter is null");
            if (wgs84Lines.Any(ls => ls == null))
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"wgs84Lines\" parameter contains a null linestring");
            if (wgs84Lines.Count < 2)
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"wgs84Lines\" parameter contains less than 2 linestrings");
            if (wgs84Lines.Any(ls => ls.Count < 2))
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"wgs84Lines\" parameter contains a linestring with less than 2 points");
            if (wgs84Lines.Any(ls => ls.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)))
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"wgs84Lines\" parameter contains a linestring having point with invalid longitude for WGS84");
            if (wgs84Lines.Any(ls => ls.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)))
                throw new OWLException("Cannot declare multiline feature to the spatial ontology because given \"wgs84Lines\" parameter contains a linestring having point with invalid latitude for WGS84");
            #endregion

            //Reconstruct sf:MultiLineString
            List<LineString> wgs84LineStrings = new List<LineString>();
            foreach (List<(double, double)> wgs84LineString in wgs84Lines)
                wgs84LineStrings.Add(new LineString(wgs84LineString.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray()) { SRID = 4326 });

            return DeclareMultiLineFeatureInternal(ontology, featureUri, geometryUri, new MultiLineString(wgs84LineStrings.ToArray()) { SRID = 4326 }, isDefaultGeometry);
        }
        internal static OWLOntology DeclareMultiLineFeatureInternal(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, MultiLineString geometry, bool isDefaultGeometry)
        {
            //Build sf:MultiLineString serializations
            string wgs84MultiLineStringWKT = GEOSpatialHelper.WKTWriter.Write(geometry);
            string wgs84MultiLineStringGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84MultiLineStringGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(featureUri);
            ontology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);
            ontology.Data.DeclareObjectAssertion(featureUri, isDefaultGeometry ? RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY : RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);
            ontology.Data.DeclareIndividual(geometryUri);
            ontology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING);
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84MultiLineStringWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84MultiLineStringGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return ontology;
        }

        /// <summary>
        /// Declares the given multiarea to the spatial ontology (coordinates of the geometries must be WGS84 Lon/Lat)
        /// </summary>
        public static OWLOntology DeclareMultiAreaFeature(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, List<List<(double,double)>> wgs84Areas, bool isDefaultGeometry)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"geometryUri\" parameter is null");
            if (wgs84Areas == null)
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"wgs84Areas\" parameter is null");
            if (wgs84Areas.Any(pl => pl == null))
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"wgs84Areas\" parameter contains a null polygon");
            if (wgs84Areas.Count < 2)
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"wgs84Areas\" parameter contains less than 2 polygons");
            if (wgs84Areas.Any(pl => pl.Count < 3))
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"wgs84Areas\" parameter contains a polygon with less than 3 points");
            if (wgs84Areas.Any(pl => pl.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)))
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"wgs84Areas\" parameter contains a polygon having point with invalid longitude for WGS84");
            if (wgs84Areas.Any(pl => pl.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)))
                throw new OWLException("Cannot declare multiarea feature to the spatial ontology because given \"wgs84Areas\" parameter contains a polygon having point with invalid latitude for WGS84");
            #endregion

            //Reconstruct sf:MultiPolygon
            List<Polygon> wgs84Polygons = new List<Polygon>();
            foreach (List<(double, double)> wgs84Polygon in wgs84Areas)
            {
                //Automatically close polygon (if needed)
                if (wgs84Polygon[0].Item1 != wgs84Polygon[wgs84Polygon.Count - 1].Item1
                     && wgs84Polygon[0].Item2 != wgs84Polygon[wgs84Polygon.Count - 1].Item2)
                    wgs84Polygon.Add(wgs84Polygon[0]);
                wgs84Polygons.Add(new Polygon(new LinearRing(wgs84Polygon.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray())) { SRID = 4326 });
            }

            return DeclareMultiAreaFeatureInternal(ontology, featureUri, geometryUri, new MultiPolygon(wgs84Polygons.ToArray()) { SRID = 4326 }, isDefaultGeometry);
        }
        internal static OWLOntology DeclareMultiAreaFeatureInternal(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, MultiPolygon geometry, bool isDefaultGeometry)
        {
            //Build sf:MultiPolygon serializations
            string wgs84MultiPolygonWKT = GEOSpatialHelper.WKTWriter.Write(geometry);
            string wgs84MultiPolygonGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84MultiPolygonGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(featureUri);
            ontology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);
            ontology.Data.DeclareObjectAssertion(featureUri, isDefaultGeometry ? RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY : RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);
            ontology.Data.DeclareIndividual(geometryUri);
            ontology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON);
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84MultiPolygonWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84MultiPolygonGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return ontology;
        }

        /// <summary>
        /// Declares the given collection feature to the spatial ontology (coordinates of the geometries must be WGS84 Lon/Lat)
        /// </summary>
        public static OWLOntology DeclareCollectionFeature(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, List<(double,double)> wgs84Points, 
            List<List<(double,double)>> wgs84Lines, List<List<(double,double)>> wgs84Areas, bool isDefaultGeometry)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"geometryUri\" parameter is null");
            if (wgs84Points?.Any(pt => pt.Item1 < -180 || pt.Item1 > 180) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Points\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84Points?.Any(pt => pt.Item2 < -90 || pt.Item2 > 90) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Points\" parameter contains a point with invalid latitude for WGS84");
            if (wgs84Lines?.Any(ls => ls == null) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Lines\" parameter contains a null linestring");
            if (wgs84Lines.Any(ls => ls.Count < 2))
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Lines\" parameter contains a linestring with less than 2 points");
            if (wgs84Lines?.Any(ls => ls.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Lines\" parameter contains a linestring having a point with invalid longitude for WGS84");
            if (wgs84Lines?.Any(ls => ls.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Lines\" parameter contains a linestring having a point with invalid latitude for WGS84");
            if (wgs84Areas?.Any(pl => pl == null) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Areas\" parameter contains a null polygon");
            if (wgs84Areas.Any(pl => pl.Count < 3))
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Areas\" parameter contains a polygon with less than 3 points");
            if (wgs84Areas?.Any(pl => pl.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Areas\" parameter contains a polygon having a point with invalid longitude for WGS84");
            if (wgs84Areas?.Any(pl => pl.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)) ?? false)
                throw new OWLException("Cannot declare collection feature to the spatial ontology because given \"wgs84Areas\" parameter contains a polygon having a point with invalid latitude for WGS84");
            #endregion

            //Reconstruct sf:Point(s)
            List<Point> wgs84CollectionPoints = new List<Point>();
            foreach ((double,double) wgs84Point in wgs84Points)
                wgs84CollectionPoints.Add(new Point(wgs84Point.Item1, wgs84Point.Item2) { SRID = 4326 });

            //Reconstruct sf:LineString(s)
            List<LineString> wgs84CollectionLineStrings = new List<LineString>();
            foreach (List<(double,double)> wgs84LineString in wgs84Lines)
                wgs84CollectionLineStrings.Add(new LineString(wgs84LineString.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray()) { SRID = 4326 });

            //Reconstruct sf:Polygon(s)
            List<Polygon> wgs84CollectionPolygons = new List<Polygon>();
            foreach (List<(double,double)> wgs84Polygon in wgs84Areas)
            {
                //Automatically close polygon (if needed)
                if (wgs84Polygon[0].Item1 != wgs84Polygon[wgs84Polygon.Count - 1].Item1
                     && wgs84Polygon[0].Item2 != wgs84Polygon[wgs84Polygon.Count - 1].Item2)
                    wgs84Polygon.Add(wgs84Polygon[0]);
                wgs84CollectionPolygons.Add(new Polygon(new LinearRing(wgs84Polygon.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray())) { SRID = 4326 });
            }

            //Build sf:GeometryCollection instance
            GeometryCollection wgs84GeometryCollection = 
                new GeometryCollection(wgs84CollectionPoints.OfType<Geometry>()
                                        .Union(wgs84CollectionLineStrings.OfType<Geometry>())
                                         .Union(wgs84CollectionPolygons.OfType<Geometry>())
                                          .ToArray()) { SRID = 4326 };
            return DeclareCollectionFeatureInternal(ontology, featureUri, geometryUri, wgs84GeometryCollection, isDefaultGeometry);
        }
        internal static OWLOntology DeclareCollectionFeatureInternal(this OWLOntology ontology, RDFResource featureUri, RDFResource geometryUri, GeometryCollection geometry, bool isDefaultGeometry)
        {
            //Build sf:GeometryCollection serializations
            string wgs84GeometryCollectionWKT = GEOSpatialHelper.WKTWriter.Write(geometry);
            string wgs84GeometryCollectionGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84GeometryCollectionGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(featureUri);
            ontology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);
            ontology.Data.DeclareObjectAssertion(featureUri, isDefaultGeometry ? RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY : RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);
            ontology.Data.DeclareIndividual(geometryUri);
            ontology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84GeometryCollectionWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            ontology.Data.DeclareDatatypeAssertion(geometryUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84GeometryCollectionGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return ontology;
        }
        #endregion

        #region Analyzer
        /// <summary>
        /// Gets the default geometry (WGS84,UTM) assigned to the given feature
        /// </summary>
        internal static (Geometry,Geometry) GetDefaultGeometryOfFeature(this OWLOntology ontology, RDFResource featureUri)
        {
            //Execute SPARQL query to retrieve WKT/GML serialization of the given feature's default geometry
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(featureUri, RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))))
                .AddProjectionVariable(new RDFVariable("?GEOMWKT"))
                .AddProjectionVariable(new RDFVariable("?GEOMGML"))
                .AddModifier(new RDFLimitModifier(1));
            RDFSelectQueryResult selectQueryResult = selectQuery.ApplyToGraph(ontology.Data.ABoxGraph);

            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometry
            if (selectQueryResult.SelectResultsCount > 0)
            {
                //WKT
                if (!selectQueryResult.SelectResults.Rows[0].IsNull("?GEOMWKT"))
                {
                    try
                    {
                        //Parse default geometry from WKT
                        RDFTypedLiteral wktGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectQueryResult.SelectResults.Rows[0]["?GEOMWKT"].ToString());
                        Geometry wgs84Geometry = new WKTReader().Read(wktGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                        return (wgs84Geometry, lazGeometry);
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }

                //GML
                if (!selectQueryResult.SelectResults.Rows[0].IsNull("?GEOMGML"))
                {
                    try
                    {
                        //Parse default geometry from GML
                        RDFTypedLiteral gmlGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectQueryResult.SelectResults.Rows[0]["?GEOMGML"].ToString());
                        Geometry wgs84Geometry = new GMLReader().Read(gmlGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                        return (wgs84Geometry, lazGeometry);
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
            }

            return (null,null);
        }

        /// <summary>
        /// Gets the list of secondary geometries (WGS84,UTM) assigned to the feature
        /// </summary>
        internal static List<(Geometry,Geometry)> GetSecondaryGeometriesOfFeature(this OWLOntology ontology, RDFResource featureUri)
        {
            List<(Geometry,Geometry)> secondaryGeometries = new List<(Geometry,Geometry)>();

            //Execute SPARQL query to retrieve WKT/GML serialization of the given feature's not default geometries
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(featureUri, RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))))
                .AddProjectionVariable(new RDFVariable("?GEOMWKT"))
                .AddProjectionVariable(new RDFVariable("?GEOMGML"));
            RDFSelectQueryResult selectQueryResult = selectQuery.ApplyToGraph(ontology.Data.ABoxGraph);

            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometries
            foreach (DataRow selectResultsRow in selectQueryResult.SelectResults.Rows)
            {
                bool geometryCollected = false;

                //WKT
                if (!selectResultsRow.IsNull("?GEOMWKT"))
                {
                    try
                    {
                        //Parse geometry from WKT
                        RDFTypedLiteral wktGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMWKT"].ToString());
                        Geometry wgs84Geometry = new WKTReader().Read(wktGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                        geometryCollected = true;
                        secondaryGeometries.Add((wgs84Geometry, lazGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }

                //GML
                if (!geometryCollected && !selectResultsRow.IsNull("?GEOMGML"))
                {
                    try
                    {
                        //Parse default geometry from GML
                        RDFTypedLiteral gmlGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMGML"].ToString());
                        Geometry wgs84Geometry = new GMLReader().Read(gmlGeometryLiteral.Value);
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

        /// <summary>
        /// Gets the features having at least one serialized WKT/GML geometry (WGS84,UTM)
        /// </summary>
        internal static List<(RDFResource,Geometry,Geometry)> GetFeaturesWithGeometries(this OWLOntology ontology)
        {
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = new List<(RDFResource,Geometry,Geometry)>();

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?FEATURE"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?FEATURE")))
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                    .UnionWithNext())
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?FEATURE"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?FEATURE")))
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))))
                .AddProjectionVariable(new RDFVariable("?FEATURE"))
                .AddProjectionVariable(new RDFVariable("?GEOMWKT"))
                .AddProjectionVariable(new RDFVariable("?GEOMGML"));
            RDFSelectQueryResult selectQueryResult = selectQuery.ApplyToGraph(ontology.Data.ABoxGraph);

            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometries
            foreach (DataRow selectResultsRow in selectQueryResult.SelectResults.Rows)
            {
                bool geometryCollected = false;

                //WKT
                if (!selectResultsRow.IsNull("?GEOMWKT"))
                {
                    try
                    {
                        //Parse feature URI
                        RDFResource featureUri = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?FEATURE"].ToString());

                        //Parse geometry from WKT
                        RDFTypedLiteral wktGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMWKT"].ToString());
                        Geometry wgs84Geometry = new WKTReader().Read(wktGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                        geometryCollected = true;
                        featuresWithGeometry.Add((featureUri, wgs84Geometry, lazGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }

                //GML
                if (!geometryCollected && !selectResultsRow.IsNull("?GEOMGML"))
                {
                    try
                    {
                        //Parse feature URI
                        RDFResource featureUri = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?FEATURE"].ToString());

                        //Parse default geometry from GML
                        RDFTypedLiteral gmlGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMGML"].ToString());
                        Geometry wgs84Geometry = new GMLReader().Read(gmlGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                        featuresWithGeometry.Add((featureUri, wgs84Geometry, lazGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
            }

            return featuresWithGeometry;
        }
        #endregion
    }
}