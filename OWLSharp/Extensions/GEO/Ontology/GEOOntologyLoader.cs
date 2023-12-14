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

using RDFSharp.Model;

namespace OWLSharp.Extensions.GEO
{
    /// <summary>
    /// GEOOntologyLoader is responsible for loading GeoSPARQL ontologies from remote sources or alternative representations
    /// </summary>
    public static class GEOOntologyLoader
    {
        #region Methods
        /// <summary>
        /// Prepares the given ontology for GeoSPARQL support, making it suitable for geospatial modeling and analysis
        /// </summary>
        public static void InitializeGEO(this OWLOntology ontology)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot initialize geospatial ontology because given \"ontology\" parameter is null");
            #endregion

            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEOSPARQL.BASE_URI));
            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEOSPARQL.GEOF.BASE_URI));
            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEOSPARQL.SF.BASE_URI));
            BuildGEOClassModel(ontology.Model.ClassModel);
            BuildGEOPropertyModel(ontology.Model.PropertyModel);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Builds a reference spatial class model
        /// </summary>
        internal static OWLOntologyClassModel BuildGEOClassModel(OWLOntologyClassModel existingClassModel=null)
        {
            OWLOntologyClassModel geoClassModel = new OWLOntologyClassModel();

            #region GEO T-BOX

            //W3C GEO
            geoClassModel.DeclareClass(RDFVocabulary.GEO.SPATIAL_THING);
            geoClassModel.DeclareClass(RDFVocabulary.GEO.POINT);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEO.POINT, RDFVocabulary.GEO.SPATIAL_THING);

            //GeoSPARQL
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.FEATURE);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.GEOMETRY, RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.FEATURE, RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT);
            geoClassModel.DeclareDisjointClasses(RDFVocabulary.GEOSPARQL.GEOMETRY, RDFVocabulary.GEOSPARQL.FEATURE);

            //Simple Features (Geometry)
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.POINT);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.CURVE);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.SURFACE);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.POLYGON);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.TRIANGLE);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.LINEAR_RING);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.LINE);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_POINT);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.POLYHEDRAL_SURFACE);
            geoClassModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.TIN);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POINT, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.CURVE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.SURFACE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYGON, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYGON, RDFVocabulary.GEOSPARQL.SF.SURFACE);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TRIANGLE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TRIANGLE, RDFVocabulary.GEOSPARQL.SF.POLYGON);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINESTRING, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINESTRING, RDFVocabulary.GEOSPARQL.SF.CURVE);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINEAR_RING, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINEAR_RING, RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINE, RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POINT, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POINT, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON, RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING, RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYHEDRAL_SURFACE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYHEDRAL_SURFACE, RDFVocabulary.GEOSPARQL.SF.SURFACE);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TIN, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoClassModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TIN, RDFVocabulary.GEOSPARQL.SF.SURFACE);

            #endregion

            #region IMPORT
            if (existingClassModel != null)
            {
                foreach (RDFTriple geoTriple in geoClassModel.TBoxGraph)
                    existingClassModel.TBoxGraph.AddTriple(geoTriple.SetImport());
                foreach (RDFResource geoClass in geoClassModel.Classes.Values)    
                {
                    if (!existingClassModel.Classes.ContainsKey(geoClass.PatternMemberID))
                        existingClassModel.Classes.Add(geoClass.PatternMemberID, geoClass);    
                }
            }
            #endregion

            return existingClassModel ?? geoClassModel;
        }

        /// <summary>
        /// Builds a reference spatial property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildGEOPropertyModel(OWLOntologyPropertyModel existingPropertyModel=null)
        {
            OWLOntologyPropertyModel geoPropertyModel = new OWLOntologyPropertyModel();

            #region GEO T-BOX

            //W3C GEO
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEO.LAT, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEO.SPATIAL_THING, Range = RDFVocabulary.XSD.DOUBLE });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEO.LONG, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEO.SPATIAL_THING, Range = RDFVocabulary.XSD.DOUBLE });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEO.ALT, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEO.SPATIAL_THING, Range = RDFVocabulary.XSD.DOUBLE });

            //GeoSPARQL
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.FEATURE, Range = RDFVocabulary.GEOSPARQL.GEOMETRY });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.FEATURE, Range = RDFVocabulary.GEOSPARQL.GEOMETRY });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_CONTAINS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_CROSSES, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_DISJOINT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_EQUALS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_INTERSECTS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_OVERLAPS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_TOUCHES, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_WITHIN, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_CONTAINS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_COVERS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_COVERED_BY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_DISJOINT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_EQUALS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_INSIDE, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_MEET, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_OVERLAP, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8DC, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8EC, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8EQ, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8NTPP, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8NTPPI, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8PO, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8TPP, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8TPPI, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.DIMENSION, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.INTEGER });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.COORDINATE_DIMENSION, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.INTEGER });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.SPATIAL_DIMENSION, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.INTEGER });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.HAS_SERIALIZATION, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.RDFS.LITERAL });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.AS_WKT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.GEOSPARQL.WKT_LITERAL });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.AS_GML, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.GEOSPARQL.GML_LITERAL });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.IS_EMPTY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.BOOLEAN });
            geoPropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.IS_SIMPLE, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.BOOLEAN });
            geoPropertyModel.DeclareSubProperties(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, RDFVocabulary.GEOSPARQL.HAS_GEOMETRY);
            geoPropertyModel.DeclareSubProperties(RDFVocabulary.GEOSPARQL.AS_WKT, RDFVocabulary.GEOSPARQL.HAS_SERIALIZATION);
            geoPropertyModel.DeclareSubProperties(RDFVocabulary.GEOSPARQL.AS_GML, RDFVocabulary.GEOSPARQL.HAS_SERIALIZATION);
            geoPropertyModel.DeclareInverseProperties(RDFVocabulary.GEOSPARQL.EH_COVERS, RDFVocabulary.GEOSPARQL.EH_COVERED_BY);

            #endregion

            #region IMPORT
            if (existingPropertyModel != null)
            {
                foreach (RDFTriple geoTriple in geoPropertyModel.TBoxGraph)
                    existingPropertyModel.TBoxGraph.AddTriple(geoTriple.SetImport());
                foreach (RDFResource geoProperty in geoPropertyModel.Properties.Values)    
                {
                    if (!existingPropertyModel.Properties.ContainsKey(geoProperty.PatternMemberID))
                        existingPropertyModel.Properties.Add(geoProperty.PatternMemberID, geoProperty);    
                }
            }            
            #endregion

            return existingPropertyModel ?? geoPropertyModel;
        }
        #endregion
    }
}