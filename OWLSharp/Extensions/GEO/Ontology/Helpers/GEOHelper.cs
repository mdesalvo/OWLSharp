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
using System.Data;
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

namespace OWLSharp.Extensions.GEO.Ontology.Helpers
{
    public static class GEOHelper
    {
        // WGS84 uses LON/LAT coordinates
        // LON => X (West/East, -180 ->180)
        // LAT => Y (North/South, -90->90)

        #region Methods
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
                            new SWRLIndividualArgument(featureUri)),
                        SWRLBuiltIn.EndsWith(
                            new SWRLVariableArgument(new RDFVariable("?WKT")),
                            new SWRLLiteralArgument(new RDFPlainLiteral($"^^{RDFVocabulary.GEOSPARQL.WKT_LITERAL}")))
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
                                new SWRLIndividualArgument(featureUri)),
                            SWRLBuiltIn.EndsWith(
                                new SWRLVariableArgument(new RDFVariable("?GML")),
                                new SWRLLiteralArgument(new RDFPlainLiteral($"^^{RDFVocabulary.GEOSPARQL.GML_LITERAL}")))
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
            if (string.Equals(inferenceAxiom?.DataProperty.GetIRI(), "urn:swrl:geosparql:asWKT"))
            {
                try
                {
                    //Parse default geometry from WKT
                    RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                    Geometry wgs84Geometry = new WKTReader().Read(inferenceLiteral.Value);
                    wgs84Geometry.SRID = 4326;

                    //Project default geometry from WGS84 to Lambert Azimuthal
                    Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                    return (wgs84Geometry, lazGeometry);
                }
                catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
            }
            if (string.Equals(inferenceAxiom?.DataProperty.GetIRI(), "urn:swrl:geosparql:asGML"))
            {
                try
                {
                    //Parse default geometry from GML
                    RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                    Geometry wgs84Geometry = new GMLReader().Read(inferenceLiteral.Value);
                    wgs84Geometry.SRID = 4326;

                    //Project default geometry from WGS84 to Lambert Azimuthal
                    Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                    return (wgs84Geometry, lazGeometry);
                }
                catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
            }

            return (null,null);
        }

        /// <summary>
        /// Gets the list of secondary geometries (WGS84,UTM) assigned to the feature
        /// </summary>
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
                            new SWRLIndividualArgument(featureUri)),
                        SWRLBuiltIn.EndsWith(
                            new SWRLVariableArgument(new RDFVariable("?WKT")),
                            new SWRLLiteralArgument(new RDFPlainLiteral($"^^{RDFVocabulary.GEOSPARQL.WKT_LITERAL}")))
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
                                new SWRLIndividualArgument(featureUri)),
                            SWRLBuiltIn.EndsWith(
                                new SWRLVariableArgument(new RDFVariable("?GML")),
                                new SWRLLiteralArgument(new RDFPlainLiteral($"^^{RDFVocabulary.GEOSPARQL.GML_LITERAL}")))
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
            
            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometry
            foreach (OWLInference inference in inferences)
            {
                OWLDataPropertyAssertion inferenceAxiom = (OWLDataPropertyAssertion)inference.Axiom;
                if (string.Equals(inferenceAxiom?.DataProperty.GetIRI(), "urn:swrl:geosparql:asWKT"))
                {
                    try
                    {
                        //Parse default geometry from WKT
                        RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                        Geometry wgs84Geometry = new WKTReader().Read(inferenceLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to Lambert Azimuthal
                        Geometry lazGeometry = RDFGeoConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84Geometry);

                        secondaryGeometries.Add((wgs84Geometry, lazGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
                if (string.Equals(inferenceAxiom?.DataProperty.GetIRI(), "urn:swrl:geosparql:asGML"))
                {
                    try
                    {
                        //Parse default geometry from GML
                        RDFTypedLiteral inferenceLiteral = (RDFTypedLiteral)inferenceAxiom.Literal.GetLiteral();
                        Geometry wgs84Geometry = new GMLReader().Read(inferenceLiteral.Value);
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