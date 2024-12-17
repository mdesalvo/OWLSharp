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
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Extensions.GEO
{
    public static class GEOZHelper
    {
        #region Methods
        public static OWLOntology DeclarePointFeature(this OWLOntology ontology, RDFResource featureUri, 
            RDFResource geometryUri, (double longitude, double latitude) wgs84Point, bool isDefaultGeometry=true)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare point feature because given \"featureUri\" parameter is null");
            if (geometryUri == null)
                throw new OWLException("Cannot declare point feature because given \"geometryUri\" parameter is null");
            if (wgs84Point.longitude < -180 || wgs84Point.longitude > 180)
                throw new OWLException("Cannot declare point feature because given \"geometry\" parameter has not a valid WGS84 longitude");
            if (wgs84Point.latitude < -90 || wgs84Point.latitude > 90)
                throw new OWLException("Cannot declare point feature because given \"geometry\" parameter has not a valid WGS84 latitude");
            #endregion

            string wktLiteral = GEOEngine.WKTWriter.Write(new Point(wgs84Point.longitude, wgs84Point.latitude) { SRID = 4326 });
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.FEATURE));
            ontology.AddEntity(new OWLClass(RDFVocabulary.GEOSPARQL.GEOMETRY));
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