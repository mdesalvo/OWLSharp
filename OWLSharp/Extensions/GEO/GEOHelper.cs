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
using NetTopologySuite.Geometries;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Extensions.GEO
{
    public static class GEOHelper
    {
        #region Methods
        public static OWLOntology DeclarePointFeature(this OWLOntology ontology, RDFResource featureUri, 
            RDFResource geometryUri, (double longitude, double latitude) wgs84Coordinate, bool isDefaultGeometry=true)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare point feature because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare point feature because given \"geometryUri\" parameter is null");
            if (wgs84Coordinate.longitude < -180 || wgs84Coordinate.longitude > 180)
                throw new OWLException("Cannot declare point feature because given \"wgs84Coordinate\" parameter has not a valid WGS84 longitude");
            if (wgs84Coordinate.latitude < -90 || wgs84Coordinate.latitude > 90)
                throw new OWLException("Cannot declare point feature because given \"wgs84Coordinate\" parameter has not a valid WGS84 latitude");
            #endregion

            string wktLiteral = GEOEngine.WKTWriter.Write(new Point(wgs84Coordinate.longitude, wgs84Coordinate.latitude) { SRID = 4326 });
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE));
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY));
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.SF.POINT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY));
            ontology.AddEntity(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT));
            ontology.AddEntity(new OWLNamedIndividual(featureUri));
            ontology.AddEntity(new OWLNamedIndividual(geometryUri));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                new OWLNamedIndividual(featureUri)));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.SF.POINT),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                isDefaultGeometry ? new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)
                                  : new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                new OWLNamedIndividual(geometryUri),
                new OWLLiteral(new RDFTypedLiteral(wktLiteral, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))));
            
            return ontology;
        }

        public static OWLOntology DeclareLineFeature(this OWLOntology ontology, RDFResource featureUri, 
            RDFResource geometryUri, (double longitude, double latitude)[] wgs84Coordinates, bool isDefaultGeometry=true)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare line feature because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare line feature because given \"geometryUri\" parameter is null");
            if (wgs84Coordinates == null)
                throw new OWLException("Cannot declare line feature because given \"wgs84Coordinates\" parameter is null");
            if (wgs84Coordinates.Length < 2)
                throw new OWLException("Cannot declare line feature because given \"wgs84Coordinates\" parameter contains less than 2 points");
            if (wgs84Coordinates.Any(pt => pt.longitude < -180 || pt.longitude > 180))
                throw new OWLException("Cannot declare line feature because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 longitude");
            if (wgs84Coordinates.Any(pt => pt.latitude < -90 || pt.latitude > 90))
                throw new OWLException("Cannot declare line feature because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 latitude");
            #endregion

            string wktLiteral = GEOEngine.WKTWriter.Write(new LineString(wgs84Coordinates.Select(wgs84Point => new Coordinate(wgs84Point.longitude, wgs84Point.latitude)).ToArray()) { SRID = 4326 });
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE));
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY));
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.SF.LINESTRING));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY));
            ontology.AddEntity(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT));
            ontology.AddEntity(new OWLNamedIndividual(featureUri));
            ontology.AddEntity(new OWLNamedIndividual(geometryUri));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                new OWLNamedIndividual(featureUri)));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.SF.LINESTRING),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                isDefaultGeometry ? new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)
                                  : new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                new OWLNamedIndividual(geometryUri),
                new OWLLiteral(new RDFTypedLiteral(wktLiteral, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))));
            
            return ontology;
        }

        public static OWLOntology DeclareAreaFeature(this OWLOntology ontology, RDFResource featureUri, 
            RDFResource geometryUri, (double longitude, double latitude)[] wgs84Coordinates, bool isDefaultGeometry=true)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare area feature because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare area feature because given \"geometryUri\" parameter is null");
            if (wgs84Coordinates == null)
                throw new OWLException("Cannot declare area feature because given \"wgs84Coordinates\" parameter is null");
            if (wgs84Coordinates.Length < 3)
                throw new OWLException("Cannot declare area feature because given \"wgs84Coordinates\" parameter contains less than 2 points");
            if (wgs84Coordinates.Any(pt => pt.longitude < -180 || pt.longitude > 180))
                throw new OWLException("Cannot declare area feature because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 longitude");
            if (wgs84Coordinates.Any(pt => pt.latitude < -90 || pt.latitude > 90))
                throw new OWLException("Cannot declare area feature because given \"wgs84Coordinates\" parameter contains a point with invalid WGS84 latitude");
            
            //Automatically close polygon (if the last coordinate does not match with the first one)
            if (wgs84Coordinates[0].longitude != wgs84Coordinates[wgs84Coordinates.Length-1].longitude
                 && wgs84Coordinates[0].latitude != wgs84Coordinates[wgs84Coordinates.Length-1].latitude)
            {
                List<(double,double)> wgs84CoordinatesList = wgs84Coordinates.ToList();
                wgs84CoordinatesList.Add(wgs84Coordinates[0]);
                wgs84Coordinates = wgs84CoordinatesList.ToArray();
            }
            #endregion

            string wktLiteral = GEOEngine.WKTWriter.Write(new Polygon(new LinearRing(wgs84Coordinates.Select(wgs84Point => new Coordinate(wgs84Point.longitude, wgs84Point.latitude)).ToArray())) { SRID = 4326 });
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE));
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY));
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.SF.POLYGON));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY));
            ontology.AddEntity(new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT));
            ontology.AddEntity(new OWLNamedIndividual(featureUri));
            ontology.AddEntity(new OWLNamedIndividual(geometryUri));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE),
                new OWLNamedIndividual(featureUri)));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.GEOSPARQL.SF.POLYGON),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                isDefaultGeometry ? new OWLObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY)
                                  : new OWLObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(geometryUri)));
            ontology.AddAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.GEOSPARQL.AS_WKT),
                new OWLNamedIndividual(geometryUri),
                new OWLLiteral(new RDFTypedLiteral(wktLiteral, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))));
            
            return ontology;
        }
        #endregion
    }
}