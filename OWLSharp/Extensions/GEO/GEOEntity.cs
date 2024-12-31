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
using System.IO;
using System.Linq;
using System.Xml;
using NetTopologySuite.Geometries;
using RDFSharp.Model;

namespace OWLSharp.Extensions.GEO
{
    public class GEOEntity : RDFResource
    {
        #region Properties
        internal Geometry WGS84Geometry { get;set; }
        #endregion

        #region Ctors
        internal GEOEntity(RDFResource geoEntityUri)
            : base(geoEntityUri?.ToString()) { }
        #endregion
    
        #region Methods
        public string ToWKT()
            => GEOHelper.WKTWriter.Write(WGS84Geometry);
        public string ToGML()
        {
            using (XmlReader gmlReader = GEOHelper.GMLWriter.Write(WGS84Geometry))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                return gmlDocument.InnerXml;
            }
        }
        #endregion
    }

    public class GEOPoint : GEOEntity
    {
        #region Ctors
        public GEOPoint(RDFResource geoEntityUri, (double longitude, double latitude) wgs84Coordinate) 
            : base(geoEntityUri)
        {
            #region Guards
            if (wgs84Coordinate.longitude < -180 || wgs84Coordinate.longitude > 180)
                throw new OWLException("Cannot declare point entity because given \"wgs84Coordinate\" parameter has not a valid WGS84 longitude");
            if (wgs84Coordinate.latitude < -90 || wgs84Coordinate.latitude > 90)
                throw new OWLException("Cannot declare point entity because given \"wgs84Coordinate\" parameter has not a valid WGS84 latitude");
            #endregion

            WGS84Geometry = new Point(wgs84Coordinate.longitude, wgs84Coordinate.latitude) { SRID = 4326 };
        }
        #endregion
    }

    public class GEOLine : GEOEntity
    {
        #region Ctors
        public GEOLine(RDFResource geoEntityUri, (double longitude, double latitude)[] wgs84Coordinates) 
            : base(geoEntityUri)
        {
            #region Guards
            if (wgs84Coordinates == null)
                throw new OWLException("Cannot declare line entity because given \"wgs84Coordinates\" parameter is null");
            if (wgs84Coordinates.Length < 2)
                throw new OWLException("Cannot declare line entity because given \"wgs84Coordinates\" parameter contains less than 2 points");
            if (wgs84Coordinates.Any(pt => pt.longitude < -180 || pt.longitude > 180))
                throw new OWLException("Cannot declare line entity because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 longitude");
            if (wgs84Coordinates.Any(pt => pt.latitude < -90 || pt.latitude > 90))
                throw new OWLException("Cannot declare line entity because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 latitude");
            #endregion

            WGS84Geometry = new LineString(wgs84Coordinates.Select(wgs84Point => new Coordinate(wgs84Point.longitude, wgs84Point.latitude)).ToArray()) { SRID = 4326 };
        }
        #endregion
    }

    public class GEOArea : GEOEntity
    {
        #region Ctors
        public GEOArea(RDFResource geoEntityUri, (double longitude, double latitude)[] wgs84Coordinates) 
            : base(geoEntityUri)
        {
            #region Guards
            if (wgs84Coordinates == null)
                throw new OWLException("Cannot declare area entity because given \"wgs84Coordinates\" parameter is null");
            if (wgs84Coordinates.Length < 3)
                throw new OWLException("Cannot declare area entity because given \"wgs84Coordinates\" parameter contains less than 2 points");
            if (wgs84Coordinates.Any(pt => pt.longitude < -180 || pt.longitude > 180))
                throw new OWLException("Cannot declare area entity because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 longitude");
            if (wgs84Coordinates.Any(pt => pt.latitude < -90 || pt.latitude > 90))
                throw new OWLException("Cannot declare area entity because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 latitude");
            #endregion

            //Automatically close polygon (if the last coordinate does not match with the first one)
            if (wgs84Coordinates[0].longitude != wgs84Coordinates[wgs84Coordinates.Length - 1].longitude
                 && wgs84Coordinates[0].latitude != wgs84Coordinates[wgs84Coordinates.Length - 1].latitude)
            {
                List<(double, double)> wgs84CoordinatesList = wgs84Coordinates.ToList();
                wgs84CoordinatesList.Add(wgs84Coordinates[0]);
                wgs84Coordinates = wgs84CoordinatesList.ToArray();
            }

            WGS84Geometry = new Polygon(new LinearRing(wgs84Coordinates.Select(wgs84Point => new Coordinate(wgs84Point.longitude, wgs84Point.latitude)).ToArray())) { SRID = 4326 };
        }
        #endregion
    }
}