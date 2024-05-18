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

using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("Ontology")]
    public class OWLOntology
    {
        #region Properties
        [XmlAttribute("ontologyIRI", DataType="anyURI")]
        public string IRI { get; set; }

        [XmlAttribute("ontologyVersion", DataType="anyURI")]
        public string VersionIRI { get; set; }

        [XmlElement("Prefix")]
        public List<OWLPrefix> Prefixes { get; internal set; }

        [XmlElement("Import")]
        public List<OWLImport> Imports { get; internal set; }

        [XmlElement("Annotation")]
        public List<OWLAnnotation> Annotations { get; internal set; }

        //Axioms

        [XmlElement("Declaration")]
        public List<OWLDeclaration> DeclarationAxioms { get; internal set; }

        [XmlElement(typeof(OWLSubClassOf), ElementName="SubClassOf")]
        [XmlElement(typeof(OWLEquivalentClasses), ElementName="EquivalentClasses")]
        [XmlElement(typeof(OWLDisjointClasses), ElementName="DisjointClasses")]
        [XmlElement(typeof(OWLDisjointUnion), ElementName="DisjointUnion")]
        public List<OWLClassAxiom> ClassAxioms { get; internal set; }

        [XmlElement(typeof(OWLSubObjectPropertyOf), ElementName="SubObjectPropertyOf")]
        [XmlElement(typeof(OWLEquivalentObjectProperties), ElementName="EquivalentObjectProperties")]
        [XmlElement(typeof(OWLDisjointObjectProperties), ElementName="DisjointObjectProperties")]
        [XmlElement(typeof(OWLInverseObjectProperties), ElementName="InverseObjectProperties")]
        [XmlElement(typeof(OWLObjectPropertyDomain), ElementName="ObjectPropertyDomain")]
        [XmlElement(typeof(OWLObjectPropertyRange), ElementName="ObjectPropertyRange")]
        [XmlElement(typeof(OWLFunctionalObjectProperty), ElementName="FunctionalObjectProperty")]
        [XmlElement(typeof(OWLInverseFunctionalObjectProperty), ElementName="InverseFunctionalObjectProperty")]
        [XmlElement(typeof(OWLReflexiveObjectProperty), ElementName="ReflexiveObjectProperty")]
        [XmlElement(typeof(OWLIrreflexiveObjectProperty), ElementName="IrreflexiveObjectProperty")]
        [XmlElement(typeof(OWLSymmetricObjectProperty), ElementName="SymmetricObjectProperty")]
        [XmlElement(typeof(OWLAsymmetricObjectProperty), ElementName="AsymmetricObjectProperty")]
        [XmlElement(typeof(OWLTransitiveObjectProperty), ElementName="TransitiveObjectProperty")]
        public List<OWLObjectPropertyAxiom> ObjectPropertyAxioms { get; internal set; }

        [XmlElement(typeof(OWLSubDataPropertyOf), ElementName="SubDataPropertyOf")]
        [XmlElement(typeof(OWLEquivalentDataProperties), ElementName="EquivalentDataProperties")]
        [XmlElement(typeof(OWLDisjointDataProperties), ElementName="DisjointDataProperties")]
        [XmlElement(typeof(OWLDataPropertyDomain), ElementName="DataPropertyDomain")]
        [XmlElement(typeof(OWLDataPropertyRange), ElementName="DataPropertyRange")]
        [XmlElement(typeof(OWLFunctionalDataProperty), ElementName="FunctionalDataProperty")]
        public List<OWLDataPropertyAxiom> DataPropertyAxioms { get; internal set; }

        [XmlElement(ElementName="DatatypeDefinition")]
        public List<OWLDatatypeDefinition> DatatypeDefinitionAxioms { get; internal set; }

        [XmlElement(ElementName="HasKey")]
        public List<OWLHasKey> KeyAxioms { get; internal set; }

        [XmlElement(typeof(OWLSameIndividual), ElementName="SameIndividual")]
        [XmlElement(typeof(OWLDifferentIndividuals), ElementName="DifferentIndividuals")]
        [XmlElement(typeof(OWLClassAssertion), ElementName="ClassAssertion")]
        [XmlElement(typeof(OWLObjectPropertyAssertion), ElementName="ObjectPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeObjectPropertyAssertion), ElementName="NegativeObjectPropertyAssertion")]
        [XmlElement(typeof(OWLDataPropertyAssertion), ElementName="DataPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeDataPropertyAssertion), ElementName="NegativeDataPropertyAssertion")]
        public List<OWLAssertionAxiom> AssertionAxioms { get; internal set; }

        [XmlElement(typeof(OWLAnnotationAssertion), ElementName="AnnotationAssertion")]
        [XmlElement(typeof(OWLSubAnnotationPropertyOf), ElementName="SubAnnotationPropertyOf")]
        [XmlElement(typeof(OWLAnnotationPropertyDomain), ElementName="AnnotationPropertyDomain")]
        [XmlElement(typeof(OWLAnnotationPropertyRange), ElementName="AnnotationPropertyRange")]
        public List<OWLAnnotationAxiom> AnnotationAxioms { get; internal set; }
        #endregion

        #region Ctors
        internal OWLOntology()
        {
            Prefixes = new List<OWLPrefix>()
            {
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.OWL.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDFS.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XSD.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX))
            };
            Imports = new List<OWLImport>();
            Annotations = new List<OWLAnnotation>();

            //Axioms
            DeclarationAxioms = new List<OWLDeclaration>();
            ClassAxioms = new List<OWLClassAxiom>();
            ObjectPropertyAxioms = new List<OWLObjectPropertyAxiom>();
            DataPropertyAxioms = new List<OWLDataPropertyAxiom>();
            DatatypeDefinitionAxioms = new List<OWLDatatypeDefinition>();
            KeyAxioms = new List<OWLHasKey>();
            AssertionAxioms = new List<OWLAssertionAxiom>();
            AnnotationAxioms = new List<OWLAnnotationAxiom>();
        }
        public OWLOntology(Uri ontologyIRI, Uri ontologyVersionIRI=null) : this()
        {
            IRI = ontologyIRI?.ToString();
            VersionIRI = ontologyVersionIRI?.ToString();
        }
        #endregion

        #region Methods
		public RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //IRI
            RDFResource ontologyIRI = new RDFResource();
            if (!string.IsNullOrWhiteSpace(IRI))
                ontologyIRI = new RDFResource(IRI);
            graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));

            //VersionIRI
            if (!string.IsNullOrWhiteSpace(VersionIRI))
                graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.OWL.VERSION_IRI, new RDFResource(VersionIRI)));

            //Imports
            foreach (OWLImport import in Imports)
                graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.OWL.IMPORTS, new RDFResource(import.IRI)));

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraphInternal(ontologyIRI));

            //Axioms
            foreach (OWLDeclaration declarationAxiom in DeclarationAxioms)
                graph = graph.UnionWith(declarationAxiom.ToRDFGraph());
            foreach (OWLClassAxiom classAxiom in ClassAxioms)
                graph = graph.UnionWith(classAxiom.ToRDFGraph());
            foreach (OWLObjectPropertyAxiom objectPropertyAxiom in ObjectPropertyAxioms)
                graph = graph.UnionWith(objectPropertyAxiom.ToRDFGraph());
            foreach (OWLDataPropertyAxiom dataPropertyAxiom in DataPropertyAxioms)
                graph = graph.UnionWith(dataPropertyAxiom.ToRDFGraph());
            foreach (OWLDatatypeDefinition datatypeDefinitionAxiom in DatatypeDefinitionAxioms)
                graph = graph.UnionWith(datatypeDefinitionAxiom.ToRDFGraph());
            foreach (OWLHasKey keyAxiom in KeyAxioms)
                graph = graph.UnionWith(keyAxiom.ToRDFGraph());
            foreach (OWLAssertionAxiom assertionAxiom in AssertionAxioms)
                graph = graph.UnionWith(assertionAxiom.ToRDFGraph());
            foreach (OWLAnnotationAxiom annotationAxiom in AnnotationAxioms)
                graph = graph.UnionWith(annotationAxiom.ToRDFGraph());

            if (!ontologyIRI.IsBlank)
                graph.SetContext(ontologyIRI.URI);
            return graph;
        }

		public void ToFile(OWLEnums.OWLFormats owlFormat, string outputFile)
        {
            #region Guards
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new OWLException("Cannot write ontology to file because given \"outputFile\" parameter is null or empty");
            #endregion

            ToStream(owlFormat, new FileStream(outputFile, FileMode.Create));
        }

        public void ToStream(OWLEnums.OWLFormats owlFormat, Stream outputStream)
        {
            #region Guards
            if (outputStream == null)
                throw new OWLException("Cannot write ontology to stream because given \"outputStream\" parameter is null");
            #endregion

			try
			{
				switch (owlFormat)
				{
					case OWLEnums.OWLFormats.Owl2Xml:
					default:						
						string ontology = OWLSerializer.Serialize(this);
						using (StreamWriter streamWriter = new StreamWriter(outputStream, RDFModelUtilities.UTF8_NoBOM))
							streamWriter.Write(ontology);
					break;
				}
			}
			catch(Exception ex)
			{
				throw new OWLException($"Cannot write ontology to stream because: {ex.Message}", ex);
			}
        }

        public static OWLOntology FromRDFGraph(RDFGraph graph)
        {
            #region Utilities
            //Ontology
			void LoadOntology(out OWLOntology ont)
            {
                string ontIRI = graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null]
                                 .First().Subject.ToString();
                ont = new OWLOntology()
                {
                    IRI = ontIRI,
                    VersionIRI = (graph[new RDFResource(ontIRI), RDFVocabulary.OWL.VERSION_IRI, null, null]
                                    .FirstOrDefault()?.Object as RDFResource)?.ToString()
                };
            }
            void LoadImports(OWLOntology ont)
            {
                foreach (RDFTriple imports in graph[new RDFResource(ont.IRI), RDFVocabulary.OWL.IMPORTS, null, null]
                                               .Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
                    ont.Imports.Add(new OWLImport((RDFResource)imports.Object));
            }
            void LoadPrefixes(OWLOntology ont)
            {
                foreach (RDFNamespace graphPrefix in RDFModelUtilities.GetGraphNamespaces(graph))
                    if (!ont.Prefixes.Any(pfx => string.Equals(pfx.IRI, graphPrefix.NamespaceUri.ToString())))
                        ont.Prefixes.Add(new OWLPrefix(graphPrefix));
            }
            void LoadDeclarations(OWLOntology ont)
            {
                RDFGraph typeGraph = graph[null, RDFVocabulary.RDF.TYPE, null, null];

                foreach (RDFTriple typeClass in typeGraph[null, null, RDFVocabulary.OWL.CLASS, null]
                                                 .UnionWith(typeGraph[null, null, RDFVocabulary.OWL.DEPRECATED_CLASS, null])
                                                 .UnionWith(typeGraph[null, null, RDFVocabulary.RDFS.CLASS, null])
                                                 .Where(t => !((RDFResource)t.Subject).IsBlank))
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLClass((RDFResource)typeClass.Subject)));

                foreach (RDFTriple typeDatatype in typeGraph[null, null, RDFVocabulary.RDFS.DATATYPE, null])
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLDatatype((RDFResource)typeDatatype.Subject)));

                foreach (RDFTriple typeObjectProperty in typeGraph[null, null, RDFVocabulary.OWL.OBJECT_PROPERTY, null])
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLObjectProperty((RDFResource)typeObjectProperty.Subject)));

                foreach (RDFTriple typeDataProperty in typeGraph[null, null, RDFVocabulary.OWL.DATATYPE_PROPERTY, null])
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLDataProperty((RDFResource)typeDataProperty.Subject)));

                foreach (RDFTriple typeAnnotationProperty in typeGraph[null, null, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null])
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLAnnotationProperty((RDFResource)typeAnnotationProperty.Subject)));

                foreach (RDFTriple typeNamedIndividual in typeGraph[null, null, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null])
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLNamedIndividual((RDFResource)typeNamedIndividual.Subject)));
            }
            void LoadOntologyAnnotations(OWLOntology ont)
            {
				LoadIRIAnnotations(ont,	new List<RDFResource>() {
						RDFVocabulary.OWL.BACKWARD_COMPATIBLE_WITH,
						RDFVocabulary.OWL.INCOMPATIBLE_WITH,
						RDFVocabulary.OWL.PRIOR_VERSION,
						RDFVocabulary.OWL.VERSION_INFO,
						RDFVocabulary.OWL.DEPRECATED,
						RDFVocabulary.RDFS.COMMENT,
						RDFVocabulary.RDFS.LABEL,
						RDFVocabulary.RDFS.SEE_ALSO,
						RDFVocabulary.RDFS.IS_DEFINED_BY
					}, new RDFResource(ont.IRI), out List<OWLAnnotation> ontologyAnnotations);
                ont.Annotations = ontologyAnnotations;
            }
            //Axioms
            void LoadFunctionalObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple funcPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)funcPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
					{
                        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty() { 
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, funcPropTriple, functionalObjectProperty);

                    	ont.ObjectPropertyAxioms.Add(functionalObjectProperty);
					}
                }
            }
            void LoadInverseFunctionalObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple invfuncPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)invfuncPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, invfuncPropTriple, inverseFunctionalObjectProperty);

                        ont.ObjectPropertyAxioms.Add(inverseFunctionalObjectProperty);
                    }
                }
            }
			void LoadSymmetricObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple symPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)symPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, symPropTriple, symmetricObjectProperty);

                        ont.ObjectPropertyAxioms.Add(symmetricObjectProperty);
                    }
                }
            }
			void LoadAsymmetricObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple asymPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)asymPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLAsymmetricObjectProperty asymmetricObjectProperty = new OWLAsymmetricObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, asymPropTriple, asymmetricObjectProperty);

                        ont.ObjectPropertyAxioms.Add(asymmetricObjectProperty);
                    }
                }
            }
            void LoadReflexiveObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple refPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)refPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, refPropTriple, reflexiveObjectProperty);

                        ont.ObjectPropertyAxioms.Add(reflexiveObjectProperty);
                    }
                }
            }
            void LoadIrreflexiveObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple irrefPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)irrefPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, irrefPropTriple, irreflexiveObjectProperty);

                        ont.ObjectPropertyAxioms.Add(irreflexiveObjectProperty);
                    }
                }
            }
			void LoadTransitiveObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple transPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)transPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, transPropTriple, transitiveObjectProperty);

                        ont.ObjectPropertyAxioms.Add(transitiveObjectProperty);
                    }
                }
            }
            void LoadInverseObjectProperties(OWLOntology ont)
            {
                RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?OPL"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?OPR")))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPL"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPR"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPL")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPR")))
                        .AddBind(new RDFBind(new RDFConstantExpression(new RDFPlainLiteral("OO")), new RDFVariable("?CASE")))
                        .UnionWithNext())
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?IOPL"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?OPL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?IOPL"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?OPR")))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPL"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPR"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddFilter(new RDFIsBlankFilter(new RDFVariable("?IOPL")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPL")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPR")))
                        .AddFilter(new RDFBooleanNotFilter(new RDFSameTermFilter(new RDFVariable("?OPL"), new RDFVariable("?OPR"))))
                        .AddBind(new RDFBind(new RDFConstantExpression(new RDFPlainLiteral("IO")), new RDFVariable("?CASE")))
                        .UnionWithNext())
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?OPL"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?IOPR")))
                        .AddPattern(new RDFPattern(new RDFVariable("?IOPR"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?OPR")))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPL"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPR"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddFilter(new RDFIsBlankFilter(new RDFVariable("?IOPR")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPL")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPR")))
                        .AddFilter(new RDFBooleanNotFilter(new RDFSameTermFilter(new RDFVariable("?OPL"), new RDFVariable("?OPR"))))
                        .AddBind(new RDFBind(new RDFConstantExpression(new RDFPlainLiteral("OI")), new RDFVariable("?CASE")))
                        .UnionWithNext())
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?IOPL"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?OPL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?IOPL"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?IOPR")))
                        .AddPattern(new RDFPattern(new RDFVariable("?IOPR"), RDFVocabulary.OWL.INVERSE_OF, new RDFVariable("?OPR")))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPL"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddPattern(new RDFPattern(new RDFVariable("?OPR"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddFilter(new RDFIsBlankFilter(new RDFVariable("?IOPL")))
                        .AddFilter(new RDFIsBlankFilter(new RDFVariable("?IOPR")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPL")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OPR")))
                        .AddFilter(new RDFBooleanNotFilter(new RDFSameTermFilter(new RDFVariable("?OPL"), new RDFVariable("?OPR"))))
                        .AddBind(new RDFBind(new RDFConstantExpression(new RDFPlainLiteral("II")), new RDFVariable("?CASE"))))
                    .AddProjectionVariable(new RDFVariable("?IOPL"))
                    .AddProjectionVariable(new RDFVariable("?OPL"))
                    .AddProjectionVariable(new RDFVariable("?IOPR"))
                    .AddProjectionVariable(new RDFVariable("?OPR"))
                    .AddProjectionVariable(new RDFVariable("?CASE"))
                    .AddModifier(new RDFOrderByModifier(new RDFVariable("?CASE"), RDFQueryEnums.RDFOrderByFlavors.DESC));
                RDFSelectQueryResult result = query.ApplyToGraph(graph);

                HashSet<long> ioplLookup = new HashSet<long>();
                foreach (DataRow resultRow in result.SelectResults.Rows)
                {
                    OWLInverseObjectProperties inverseObjectProperties = new OWLInverseObjectProperties();

                    RDFResource IOPL/*InverseOfPropertyLeft*/, OPL/*ObjectPropertyLeft*/, IOPR/*InverseOfPropertyRight*/, OPR/*ObjectPropertyRight*/;
                    switch (resultRow["?CASE"].ToString())
                    {
                        case "OO":
                            OPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPL"].ToString());
                            OPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPR"].ToString());
                            inverseObjectProperties.LeftObjectPropertyExpression = new OWLObjectProperty(OPL);
                            inverseObjectProperties.RightObjectPropertyExpression = new OWLObjectProperty(OPR);
                            LoadAxiomAnnotations(ont, new RDFTriple(OPL, RDFVocabulary.OWL.INVERSE_OF, OPR), inverseObjectProperties);
                            break;
                        case "IO":
                            IOPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IOPL"].ToString());
                            #region IOPL Guard
                            if (ioplLookup.Contains(IOPL.PatternMemberID)) continue;
                            ioplLookup.Add(IOPL.PatternMemberID);
                            #endregion
                            OPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPL"].ToString());
                            OPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPR"].ToString());
                            inverseObjectProperties.LeftObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(OPL));
                            inverseObjectProperties.RightObjectPropertyExpression = new OWLObjectProperty(OPR);
                            LoadAxiomAnnotations(ont, new RDFTriple(IOPL, RDFVocabulary.OWL.INVERSE_OF, OPR), inverseObjectProperties);
                            break;
                        case "OI":
                            OPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPL"].ToString());
                            IOPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IOPR"].ToString());
                            OPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPR"].ToString());
                            inverseObjectProperties.LeftObjectPropertyExpression = new OWLObjectProperty(OPL);
                            inverseObjectProperties.RightObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(OPR));
                            LoadAxiomAnnotations(ont, new RDFTriple(OPL, RDFVocabulary.OWL.INVERSE_OF, IOPR), inverseObjectProperties);
                            break;
                        case "II":
                            IOPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IOPL"].ToString());
                            OPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPL"].ToString());
                            IOPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IOPR"].ToString());
                            OPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPR"].ToString());
                            inverseObjectProperties.LeftObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(OPL));
                            inverseObjectProperties.RightObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(OPR));
                            LoadAxiomAnnotations(ont, new RDFTriple(IOPL, RDFVocabulary.OWL.INVERSE_OF, IOPR), inverseObjectProperties);
                            break;
                    }

                    ont.ObjectPropertyAxioms.Add(inverseObjectProperties);
                }
            }
			void LoadEquivalentObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple equivPropTriple in graph[null, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)equivPropTriple.Subject, out OWLObjectPropertyExpression leftOPE);
                    LoadObjectPropertyExpression(ont, (RDFResource)equivPropTriple.Object, out OWLObjectPropertyExpression rightOPE);

                    if (leftOPE != null && rightOPE != null)
                    {
                        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties() {
                            ObjectPropertyExpressions = new List<OWLObjectPropertyExpression>() { leftOPE, rightOPE } };

                        LoadAxiomAnnotations(ont, equivPropTriple, equivalentObjectProperties);

                        ont.ObjectPropertyAxioms.Add(equivalentObjectProperties);
                    }
                }
            }
			void LoadDisjointObjectProperties(OWLOntology ont)
			{
				//Load axioms built with owl:propertyDisjointWith
				foreach (RDFTriple propDisjointWithTriple in graph[null, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)propDisjointWithTriple.Subject, out OWLObjectPropertyExpression leftOPE);
                    LoadObjectPropertyExpression(ont, (RDFResource)propDisjointWithTriple.Object, out OWLObjectPropertyExpression rightOPE);

                    if (leftOPE != null && rightOPE != null)
					{
						OWLDisjointObjectProperties disjointObjectProperties = new OWLDisjointObjectProperties() {
							ObjectPropertyExpressions = new List<OWLObjectPropertyExpression>() { leftOPE, rightOPE } }; 

						LoadAxiomAnnotations(ont, propDisjointWithTriple, disjointObjectProperties);

                    	ont.ObjectPropertyAxioms.Add(disjointObjectProperties);
					}
                }
			
				//Load axioms built with owl:AllDisjointProperties
				foreach (RDFTriple allDisjointPropertiesTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null])
					if (graph[(RDFResource)allDisjointPropertiesTriple.Subject, RDFVocabulary.OWL.MEMBERS, null, null]
						 .FirstOrDefault()?.Object is RDFResource adjpCollectionRepresentative)
					{
						List<OWLObjectPropertyExpression> adjpMembers = new List<OWLObjectPropertyExpression>();

						RDFCollection adjpCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, adjpCollectionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
						foreach (RDFResource adjpMember in adjpCollection.Items.Cast<RDFResource>())
                        {
                            LoadObjectPropertyExpression(ont, adjpMember, out OWLObjectPropertyExpression opex);
                            if (opex != null)
                                adjpMembers.Add(opex);
                        }

						if (adjpMembers.Count >= 2)
						{
							OWLDisjointObjectProperties disjointObjectProperties = new OWLDisjointObjectProperties() {
								ObjectPropertyExpressions = adjpMembers	};

							LoadIRIAnnotations(ont,	new List<RDFResource>() {
								RDFVocabulary.OWL.DEPRECATED,
								RDFVocabulary.RDFS.COMMENT,
								RDFVocabulary.RDFS.LABEL,
								RDFVocabulary.RDFS.SEE_ALSO,
								RDFVocabulary.RDFS.IS_DEFINED_BY
							}, (RDFResource)allDisjointPropertiesTriple.Subject, out List<OWLAnnotation> adjpAnnotations);
							disjointObjectProperties.Annotations = adjpAnnotations;

							ont.ObjectPropertyAxioms.Add(disjointObjectProperties);
						}
					}
			}
			void LoadSubObjectProperties(OWLOntology ont)
            {
				//Load axioms built with owl:propertyChainAxiom
				foreach (RDFTriple propertyChainAxiomTriple in graph[null, RDFVocabulary.OWL.PROPERTY_CHAIN_AXIOM, null, null])
				{
                    OWLObjectPropertyChain objectPropertyChain = new OWLObjectPropertyChain() {
                        ObjectPropertyExpressions = new List<OWLObjectPropertyExpression>() };
                    
                    //Left                    
					RDFCollection chainAxiomMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)propertyChainAxiomTriple.Object, RDFModelEnums.RDFTripleFlavors.SPO);
					foreach (RDFResource chainAxiomMember in chainAxiomMembers.Items.Cast<RDFResource>())
                    {
                        LoadObjectPropertyExpression(ont, chainAxiomMember, out OWLObjectPropertyExpression opex);
                        if (opex != null)
                            objectPropertyChain.ObjectPropertyExpressions.Add(opex);
                    }

                    //Right
                    LoadObjectPropertyExpression(ont, (RDFResource)propertyChainAxiomTriple.Subject, out OWLObjectPropertyExpression rightOPE);

                    if (objectPropertyChain.ObjectPropertyExpressions.Count >= 2 && rightOPE != null)
                    {
                        OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf() {
                            SubObjectPropertyChain = objectPropertyChain, SuperObjectPropertyExpression = rightOPE };

                        LoadAxiomAnnotations(ont, propertyChainAxiomTriple, subObjectPropertyOf);

                        ont.ObjectPropertyAxioms.Add(subObjectPropertyOf);
                    }
				}

				//Load axioms built with rdfs:subPropertyOf
                foreach (RDFTriple subPropTriple in graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)subPropTriple.Subject, out OWLObjectPropertyExpression leftOPE);
                    LoadObjectPropertyExpression(ont, (RDFResource)subPropTriple.Object, out OWLObjectPropertyExpression rightOPE);

                    if (leftOPE != null && rightOPE != null)
                    {
                        OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf() {
                            SubObjectPropertyExpression = leftOPE, SuperObjectPropertyExpression = rightOPE };

                        LoadAxiomAnnotations(ont, subPropTriple, subObjectPropertyOf);

                        ont.ObjectPropertyAxioms.Add(subObjectPropertyOf);
                    }
                }
            }
            void LoadObjectPropertyDomain(OWLOntology ont)
            {
                foreach (RDFTriple domainTriple in graph[null, RDFVocabulary.RDFS.DOMAIN, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)domainTriple.Subject, out OWLObjectPropertyExpression objEXP);
                    LoadClassExpression(ont, (RDFResource)domainTriple.Object, out OWLClassExpression clsEXP);

                    if (objEXP != null && clsEXP != null)
                    {
                        OWLObjectPropertyDomain objectPropertyDomain = new OWLObjectPropertyDomain() {
                             ObjectPropertyExpression = objEXP, ClassExpression = clsEXP };

                        LoadAxiomAnnotations(ont, domainTriple, objectPropertyDomain);

                        ont.ObjectPropertyAxioms.Add(objectPropertyDomain);
                    }
                }
            }
            void LoadObjectPropertyRange(OWLOntology ont)
            {
                foreach (RDFTriple rangeTriple in graph[null, RDFVocabulary.RDFS.RANGE, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)rangeTriple.Subject, out OWLObjectPropertyExpression objEXP);
                    LoadClassExpression(ont, (RDFResource)rangeTriple.Object, out OWLClassExpression clsEXP);

                    if (objEXP != null && clsEXP != null)
                    {
                        OWLObjectPropertyRange objectPropertyRange = new OWLObjectPropertyRange() {
                            ObjectPropertyExpression = objEXP, ClassExpression = clsEXP };

                        LoadAxiomAnnotations(ont, rangeTriple, objectPropertyRange);

                        ont.ObjectPropertyAxioms.Add(objectPropertyRange);
                    }
                }
            }
            void LoadFunctionalDataProperties(OWLOntology ont)
            {
                foreach (RDFTriple funcPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)funcPropTriple.Subject, out OWLDataPropertyExpression dpex);
                    if (dpex is OWLDataProperty dp)
                    {
                        OWLFunctionalDataProperty functionalDataProperty = new OWLFunctionalDataProperty(dp);

                        LoadAxiomAnnotations(ont, funcPropTriple, functionalDataProperty);

                        ont.DataPropertyAxioms.Add(functionalDataProperty);
                    }
                }
            }
            void LoadEquivalentDataProperties(OWLOntology ont)
            {
                foreach (RDFTriple equivPropTriple in graph[null, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)equivPropTriple.Subject, out OWLDataPropertyExpression leftDPex);
                    LoadDataPropertyExpression(ont, (RDFResource)equivPropTriple.Object, out OWLDataPropertyExpression rightDPex);

                    if (leftDPex is OWLDataProperty leftDP && rightDPex is OWLDataProperty rightDP)
                    {
                        OWLEquivalentDataProperties equivalentDataProperties = new OWLEquivalentDataProperties() {
                            DataProperties = new List<OWLDataProperty>() { leftDP, rightDP } };

                        LoadAxiomAnnotations(ont, equivPropTriple, equivalentDataProperties);

                        ont.DataPropertyAxioms.Add(equivalentDataProperties);
                    }
                }
            }
            void LoadDisjointDataProperties(OWLOntology ont)
            {
                //Load axioms built with owl:propertyDisjointWith
                foreach (RDFTriple propDisjointWithTriple in graph[null, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, null, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)propDisjointWithTriple.Subject, out OWLDataPropertyExpression leftDPex);
                    LoadDataPropertyExpression(ont, (RDFResource)propDisjointWithTriple.Object, out OWLDataPropertyExpression rightDPex);

                    if (leftDPex is OWLDataProperty leftDP && rightDPex is OWLDataProperty rightDP)
                    {
                        OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties() {
                            DataProperties = new List<OWLDataProperty>() { leftDP, rightDP } };

                        LoadAxiomAnnotations(ont, propDisjointWithTriple, disjointDataProperties);

                        ont.DataPropertyAxioms.Add(disjointDataProperties);
                    }
                }

                //Load axioms built with owl:AllDisjointProperties
                foreach (RDFTriple allDisjointPropertiesTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null])
                    if (graph[(RDFResource)allDisjointPropertiesTriple.Subject, RDFVocabulary.OWL.MEMBERS, null, null]
                         .FirstOrDefault()?.Object is RDFResource adjpCollectionRepresentative)
                    {
                        List<OWLDataProperty> adjpMembers = new List<OWLDataProperty>();

                        RDFCollection adjpCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, adjpCollectionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                        foreach (RDFResource adjpMember in adjpCollection.Items.Cast<RDFResource>())
                        {
                            LoadDataPropertyExpression(ont, adjpMember, out OWLDataPropertyExpression dpex);
                            if (dpex is OWLDataProperty dp)
                                adjpMembers.Add(dp);
                        }

                        if (adjpMembers.Count >= 2)
                        {
                            OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties() {
                                DataProperties = adjpMembers };

                            LoadIRIAnnotations(ont, new List<RDFResource>() {
                                RDFVocabulary.OWL.DEPRECATED,
                                RDFVocabulary.RDFS.COMMENT,
                                RDFVocabulary.RDFS.LABEL,
                                RDFVocabulary.RDFS.SEE_ALSO,
                                RDFVocabulary.RDFS.IS_DEFINED_BY
                            }, (RDFResource)allDisjointPropertiesTriple.Subject, out List<OWLAnnotation> adjpAnnotations);
                            disjointDataProperties.Annotations = adjpAnnotations;

                            ont.DataPropertyAxioms.Add(disjointDataProperties);
                        }
                    }
            }
            void LoadSubDataProperties(OWLOntology ont)
            {
                foreach (RDFTriple subPropTriple in graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)subPropTriple.Subject, out OWLDataPropertyExpression leftDPex);
                    LoadDataPropertyExpression(ont, (RDFResource)subPropTriple.Object, out OWLDataPropertyExpression rightDPex);

                    if (leftDPex is OWLDataProperty leftDP && rightDPex is OWLDataProperty rightDP)
                    {
                        OWLSubDataPropertyOf subDataPropertyOf = new OWLSubDataPropertyOf() {
                            SubDataProperty = leftDP, SuperDataProperty = rightDP };

                        LoadAxiomAnnotations(ont, subPropTriple, subDataPropertyOf);

                        ont.DataPropertyAxioms.Add(subDataPropertyOf);
                    }
                }
            }
            void LoadSameIndividual(OWLOntology ont)
            {
                foreach (RDFTriple sameAsTriple in graph[null, RDFVocabulary.OWL.SAME_AS, null, null])
                {
                    LoadIndividualExpression(ont, (RDFResource)sameAsTriple.Subject, out OWLIndividualExpression leftIE);
                    LoadIndividualExpression(ont, (RDFResource)sameAsTriple.Object, out OWLIndividualExpression rightIE);

                    if (leftIE != null && rightIE != null)
                    {
                        OWLSameIndividual sameIndividual = new OWLSameIndividual() {
                            IndividualExpressions = new List<OWLIndividualExpression>() { leftIE, rightIE } };

                        LoadAxiomAnnotations(ont, sameAsTriple, sameIndividual);

                        ont.AssertionAxioms.Add(sameIndividual);
                    }
                }
            }
            void LoadDifferentIndividuals(OWLOntology ont)
            {
                //Load axioms built with owl:differentFrom
                foreach (RDFTriple differentFromTriple in graph[null, RDFVocabulary.OWL.DIFFERENT_FROM, null, null])
                {
                    LoadIndividualExpression(ont, (RDFResource)differentFromTriple.Subject, out OWLIndividualExpression leftIE);
                    LoadIndividualExpression(ont, (RDFResource)differentFromTriple.Object, out OWLIndividualExpression rightIE);

                    if (leftIE != null && rightIE != null)
                    {
                        OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals() {
                            IndividualExpressions = new List<OWLIndividualExpression>() { leftIE, rightIE } };

                        LoadAxiomAnnotations(ont, differentFromTriple, differentIndividuals);

                        ont.AssertionAxioms.Add(differentIndividuals);
                    }
                }

                //Load axioms built with owl:AllDifferent
                foreach (RDFTriple allDifferentTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT, null])
                    if (graph[(RDFResource)allDifferentTriple.Subject, RDFVocabulary.OWL.DISTINCT_MEMBERS, null, null]
                         .FirstOrDefault()?.Object is RDFResource adiffCollectionRepresentative)
                    {
                        List<OWLIndividualExpression> adiffMembers = new List<OWLIndividualExpression>();

                        RDFCollection adiffCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, adiffCollectionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                        foreach (RDFResource adiffMember in adiffCollection.Items.Cast<RDFResource>())
                        {
                            LoadIndividualExpression(ont, adiffMember, out OWLIndividualExpression idvex);
                            if (idvex != null)
                                adiffMembers.Add(idvex);
                        }                            

                        if (adiffMembers.Count >= 2)
                        {
                            OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals() {
                                IndividualExpressions = adiffMembers };

                            LoadIRIAnnotations(ont, new List<RDFResource>() {
                                RDFVocabulary.OWL.DEPRECATED,
                                RDFVocabulary.RDFS.COMMENT,
                                RDFVocabulary.RDFS.LABEL,
                                RDFVocabulary.RDFS.SEE_ALSO,
                                RDFVocabulary.RDFS.IS_DEFINED_BY
                            }, (RDFResource)allDifferentTriple.Subject, out List<OWLAnnotation> adjpAnnotations);
                            differentIndividuals.Annotations = adjpAnnotations;

                            ont.AssertionAxioms.Add(differentIndividuals);
                        }
                    }
            }
			void LoadObjectPropertyAssertions(OWLOntology ont)
            {
                foreach (RDFTriple objPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null])
                {
                    OWLObjectProperty objProp = new OWLObjectProperty((RDFResource)objPropTriple.Subject);
                    foreach (RDFTriple objPropAsnTriple in graph[null, (RDFResource)objPropTriple.Subject, null, null])
					{
                        LoadIndividualExpression(ont, (RDFResource)objPropAsnTriple.Subject, out OWLIndividualExpression leftIE);
                        LoadIndividualExpression(ont, (RDFResource)objPropAsnTriple.Object, out OWLIndividualExpression rightIE);

                        if (leftIE != null && rightIE != null)
						{
							OWLObjectPropertyAssertion objPropAsn = new OWLObjectPropertyAssertion() {
								 ObjectPropertyExpression = objProp,
								 SourceIndividualExpression = leftIE,
								 TargetIndividualExpression = rightIE };

							LoadAxiomAnnotations(ont, objPropAsnTriple, objPropAsn);

							ont.AssertionAxioms.Add(objPropAsn);
						}
					}
                }
            }
            void LoadNegativeObjectPropertyAssertions(OWLOntology ont)
            {
                RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION))
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFVariable("?SIDV")))
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFVariable("?OBJP")))
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFVariable("?TIDV")))
                        .AddPattern(new RDFPattern(new RDFVariable("?OBJP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?SIDV")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?OBJP")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?TIDV"))));
                RDFSelectQueryResult result = query.ApplyToGraph(graph);
                foreach (DataRow resultRow in result.SelectResults.Rows)
                {
                    LoadIndividualExpression(ont, (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?SIDV"].ToString()), out OWLIndividualExpression leftIE);
                    LoadIndividualExpression(ont, (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?TIDV"].ToString()), out OWLIndividualExpression rightIE);

                    if (leftIE != null && rightIE != null)
                    {
                        RDFResource axiomIRI = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?NASN"].ToString());
                        OWLObjectProperty objProp = new OWLObjectProperty((RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OBJP"].ToString()));

                        OWLNegativeObjectPropertyAssertion negObjPropAsn = new OWLNegativeObjectPropertyAssertion()
                        {
                            ObjectPropertyExpression = objProp,
                            SourceIndividualExpression = leftIE,
                            TargetIndividualExpression = rightIE
                        };

                        LoadIRIAnnotations(ont, new List<RDFResource>() {
                                RDFVocabulary.OWL.DEPRECATED,
                                RDFVocabulary.RDFS.COMMENT,
                                RDFVocabulary.RDFS.LABEL,
                                RDFVocabulary.RDFS.SEE_ALSO,
                                RDFVocabulary.RDFS.IS_DEFINED_BY
                            }, axiomIRI, out List<OWLAnnotation> nasnAnnotations);
                        negObjPropAsn.Annotations = nasnAnnotations;

                        ont.AssertionAxioms.Add(negObjPropAsn);
                    }
                }
            }
            void LoadDataPropertyAssertions(OWLOntology ont)
            {
                foreach (RDFTriple dtPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null])
                {
                    OWLDataProperty dtProp = new OWLDataProperty((RDFResource)dtPropTriple.Subject);
                    foreach (RDFTriple dtPropAsnTriple in graph[null, (RDFResource)dtPropTriple.Subject, null, null])
                    {
                        LoadIndividualExpression(ont, (RDFResource)dtPropAsnTriple.Subject, out OWLIndividualExpression leftIE);

                        if (leftIE != null)
                        {
                            OWLDataPropertyAssertion dtPropAsn = new OWLDataPropertyAssertion()
                            {
                                DataProperty = dtProp,
                                IndividualExpression = leftIE,
                                Literal = new OWLLiteral((RDFLiteral)dtPropAsnTriple.Object)
                            };

                            LoadAxiomAnnotations(ont, dtPropAsnTriple, dtPropAsn);

                            ont.AssertionAxioms.Add(dtPropAsn);
                        }
                    }
                }
            }
            void LoadNegativeDataPropertyAssertions(OWLOntology ont)
            {
                RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION))
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFVariable("?SIDV")))
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFVariable("?DTP")))
                        .AddPattern(new RDFPattern(new RDFVariable("?NASN"), RDFVocabulary.OWL.TARGET_VALUE, new RDFVariable("?TVAL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?DTP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?SIDV")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?DTP")))
                        .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?TVAL"))));
                RDFSelectQueryResult result = query.ApplyToGraph(graph);
                foreach (DataRow resultRow in result.SelectResults.Rows)
                {
                    LoadIndividualExpression(ont, (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?SIDV"].ToString()), out OWLIndividualExpression leftIE);

                    if (leftIE != null)
                    {
                        RDFResource axiomIRI = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?NASN"].ToString());
                        OWLDataProperty dtProp = new OWLDataProperty((RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?DTP"].ToString()));
                        OWLLiteral litVal = new OWLLiteral((RDFLiteral)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?TVAL"].ToString()));

                        OWLNegativeDataPropertyAssertion negDtPropAsn = new OWLNegativeDataPropertyAssertion()
                        {
                            DataProperty = dtProp,
                            IndividualExpression = leftIE,
                            Literal = litVal
                        };

                        LoadIRIAnnotations(ont, new List<RDFResource>() {
                            RDFVocabulary.OWL.DEPRECATED,
                            RDFVocabulary.RDFS.COMMENT,
                            RDFVocabulary.RDFS.LABEL,
                            RDFVocabulary.RDFS.SEE_ALSO,
                            RDFVocabulary.RDFS.IS_DEFINED_BY
                        }, axiomIRI, out List<OWLAnnotation> nasnAnnotations);
                        negDtPropAsn.Annotations = nasnAnnotations;

                        ont.AssertionAxioms.Add(negDtPropAsn);
                    }
                }
            }
            void LoadSubAnnotationProperties(OWLOntology ont)
            {
                foreach (RDFTriple subPropTriple in graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null])
                {
                    LoadAnnotationPropertyExpression(ont, (RDFResource)subPropTriple.Subject, out OWLAnnotationPropertyExpression leftAPex);
                    LoadAnnotationPropertyExpression(ont, (RDFResource)subPropTriple.Object, out OWLAnnotationPropertyExpression rightAPex);

                    if (leftAPex is OWLAnnotationProperty leftAP && rightAPex is OWLAnnotationProperty rightAP)
                    {
                        OWLSubAnnotationPropertyOf subAnnotationPropertyOf = new OWLSubAnnotationPropertyOf() {
                            SubAnnotationProperty = leftAP, SuperAnnotationProperty = rightAP };

                        LoadAxiomAnnotations(ont, subPropTriple, subAnnotationPropertyOf);

                        ont.AnnotationAxioms.Add(subAnnotationPropertyOf);
                    }
                }
            }
            void LoadAnnotationPropertyDomain(OWLOntology ont)
            {
                foreach (RDFTriple domainTriple in graph[null, RDFVocabulary.RDFS.DOMAIN, null, null])
                {
                    LoadAnnotationPropertyExpression(ont, (RDFResource)domainTriple.Subject, out OWLAnnotationPropertyExpression annEXP);

                    if (annEXP is OWLAnnotationProperty annProp && domainTriple.Object is RDFResource domainObject)
                    {
                        OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain() {
                             AnnotationProperty = annProp, IRI = domainObject.ToString() };

                        LoadAxiomAnnotations(ont, domainTriple, annotationPropertyDomain);

                        ont.AnnotationAxioms.Add(annotationPropertyDomain);
                    }
                }
            }
            void LoadAnnotationPropertyRange(OWLOntology ont)
            {
                foreach (RDFTriple rangeTriple in graph[null, RDFVocabulary.RDFS.RANGE, null, null])
                {
                    LoadAnnotationPropertyExpression(ont, (RDFResource)rangeTriple.Subject, out OWLAnnotationPropertyExpression annEXP);

                    if (annEXP is OWLAnnotationProperty annProp && rangeTriple.Object is RDFResource rangeObject)
                    {
                        OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange() {
                            AnnotationProperty = annProp, IRI = rangeObject.ToString() };

                        LoadAxiomAnnotations(ont, rangeTriple, annotationPropertyRange);

                        ont.AnnotationAxioms.Add(annotationPropertyRange);
                    }
                }
            }
			//Annotations
            void LoadAxiomAnnotations(OWLOntology ont, RDFTriple axiomTriple, OWLAxiom axiom)
            {
                RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_SOURCE, axiomTriple.Subject))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_PROPERTY, axiomTriple.Predicate))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_TARGET, axiomTriple.Object))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), new RDFVariable("?ANNPROP"), new RDFVariable("?ANNVAL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?ANNPROP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY)));
                RDFSelectQueryResult result = query.ApplyToGraph(graph);
                foreach (DataRow resultRow in result.SelectResults.Rows)
                {
                    RDFPatternMember axiomIRI = RDFQueryUtilities.ParseRDFPatternMember(resultRow["?AXIOM"].ToString());
                    RDFPatternMember annProp = RDFQueryUtilities.ParseRDFPatternMember(resultRow["?ANNPROP"].ToString());
                    RDFPatternMember annVal = RDFQueryUtilities.ParseRDFPatternMember(resultRow["?ANNVAL"].ToString());
                    RDFTriple annotationTriple = annVal is RDFResource annValRes
                        ? new RDFTriple((RDFResource)axiomIRI, (RDFResource)annProp, annValRes)
                        : new RDFTriple((RDFResource)axiomIRI, (RDFResource)annProp, (RDFLiteral)annVal);
                    OWLAnnotation annotation = annVal is RDFResource annValRes2
                        ? new OWLAnnotation(new OWLAnnotationProperty((RDFResource)annProp), annValRes2)
                        : new OWLAnnotation(new OWLAnnotationProperty((RDFResource)annProp), new OWLLiteral((RDFLiteral)annVal));

                    LoadNestedAnnotation(ont, annotationTriple, annotation);

                    axiom.Annotations.Add(annotation);
                }
            }
            void LoadIRIAnnotations(OWLOntology ont, List<RDFResource> annotationProperties, RDFResource iri, out List<OWLAnnotation> annotations)
			{
				annotations = new List<OWLAnnotation>();

				foreach (RDFTriple annPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null]
                                                     .Where(ap => !ap.Equals(RDFVocabulary.OWL.VERSION_IRI)))
                {
                    RDFResource annPropIRI = (RDFResource)annPropTriple.Subject;
                    if (!annotationProperties.Any(ap => ap.Equals(annPropIRI)))
                        annotationProperties.Add(annPropIRI);
                }   

                foreach (RDFResource workingAnnotationProperty in annotationProperties)
                {
                    OWLAnnotationProperty annotationProperty = new OWLAnnotationProperty(workingAnnotationProperty);
                    foreach (RDFTriple annotationTriple in graph[iri, workingAnnotationProperty, null, null])
                    {
                        OWLAnnotation annotation = annotationTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO
                            ? new OWLAnnotation(annotationProperty, (RDFResource)annotationTriple.Object)
                            : new OWLAnnotation(annotationProperty, new OWLLiteral((RDFLiteral)annotationTriple.Object));

                        LoadNestedAnnotation(ont, annotationTriple, annotation);

                        annotations.Add(annotation);
                    }
                }
			}
			void LoadNestedAnnotation(OWLOntology ont, RDFTriple annotationTriple, OWLAnnotation annotation)
            {
                RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_SOURCE, annotationTriple.Subject))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_PROPERTY, annotationTriple.Predicate))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_TARGET, annotationTriple.Object))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), new RDFVariable("?ANNPROP"), new RDFVariable("?ANNVAL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?ANNPROP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY)))
                    .AddModifier(new RDFLimitModifier(1));
                RDFSelectQueryResult result = query.ApplyToGraph(graph);
                if (result.SelectResultsCount > 0)
                {
                    DataRow resultRow = result.SelectResults.Rows[0];
                    RDFPatternMember axiom = RDFQueryUtilities.ParseRDFPatternMember(resultRow["?AXIOM"].ToString());
                    RDFPatternMember annProp = RDFQueryUtilities.ParseRDFPatternMember(resultRow["?ANNPROP"].ToString());
                    RDFPatternMember annVal = RDFQueryUtilities.ParseRDFPatternMember(resultRow["?ANNVAL"].ToString());
                    RDFTriple nestedAnnotationTriple = annVal is RDFResource annValRes
                        ? new RDFTriple((RDFResource)axiom, (RDFResource)annProp, annValRes)
                        : new RDFTriple((RDFResource)axiom, (RDFResource)annProp, (RDFLiteral)annVal);
                    OWLAnnotation nestedAnnotation = annVal is RDFResource annValRes2
                        ? new OWLAnnotation(new OWLAnnotationProperty((RDFResource)annProp), annValRes2)
                        : new OWLAnnotation(new OWLAnnotationProperty((RDFResource)annProp), new OWLLiteral((RDFLiteral)annVal));
                    annotation.Annotation = nestedAnnotation;

                    LoadNestedAnnotation(ont, nestedAnnotationTriple, annotation.Annotation);
                }
            }
            //Expressions
            void LoadAnnotationPropertyExpression(OWLOntology ont, RDFResource apIRI, out OWLAnnotationPropertyExpression apex)
            {
                apex = null;
                if (graph[apIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount > 0)
                    apex = new OWLAnnotationProperty(apIRI);
            }
            void LoadObjectPropertyExpression(OWLOntology ont, RDFResource opIRI, out OWLObjectPropertyExpression opex)
            {
                opex = null;
                if (graph[opIRI, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                {
                    if (graph[opIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
                        opex = new OWLObjectProperty(opIRI);
                }
                else
                {
                    RDFResource inverseOf = (RDFResource)graph[opIRI, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                    opex = new OWLObjectInverseOf(new OWLObjectProperty(inverseOf));
                }
            }
            void LoadDataPropertyExpression(OWLOntology ont, RDFResource dpIRI, out OWLDataPropertyExpression dpex)
            {
                dpex = null;
                if (graph[dpIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                    dpex = new OWLDataProperty(dpIRI);
            }
            void LoadIndividualExpression(OWLOntology ont, RDFResource idvIRI, out OWLIndividualExpression idvex)
            {
                idvex = null;
                if (idvIRI.IsBlank)
                    idvex = new OWLAnonymousIndividual(idvIRI.ToString().Substring(6));
                else if (graph[idvIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount > 0)
                    idvex = new OWLNamedIndividual(idvIRI);
            }
            void LoadClassExpression(OWLOntology ont, RDFResource clsIRI, out OWLClassExpression clex)
            {
                clex = null;
				RDFGraph clsGraph = graph[clsIRI, null, null, null];

				#region Restriction
				if (clsGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount > 0)
                {
                    #region AllValuesFrom
                    if (clsGraph[null, RDFVocabulary.OWL.ALL_VALUES_FROM, null, null].FirstOrDefault()?.Object is RDFResource allValuesFrom)
                    {
                        LoadObjectAllValuesFrom(ont, clsIRI, allValuesFrom, out OWLObjectAllValuesFrom objAVF);
                        if (objAVF != null)
                        {
                            clex = objAVF;
                            return;
                        }
                        LoadDataAllValuesFrom(ont, clsIRI, allValuesFrom, out OWLDataAllValuesFrom dtAVF);
                        if (dtAVF != null)
                        {
                            clex = dtAVF;
                            return;
                        }
                    }
                    #endregion

                    #region SomeValuesFrom
                    if (clsGraph[null, RDFVocabulary.OWL.SOME_VALUES_FROM, null, null].FirstOrDefault()?.Object is RDFResource someValuesFrom)
                    {
                        LoadObjectSomeValuesFrom(ont, clsIRI, someValuesFrom, out OWLObjectSomeValuesFrom objSVF);
                        if (objSVF != null)
                        {
                            clex = objSVF;
                            return;
                        }
                        LoadDataSomeValuesFrom(ont, clsIRI, someValuesFrom, out OWLDataSomeValuesFrom dtSVF);
                        if (dtSVF != null)
                        {
                            clex = dtSVF;
                            return;
                        }
                    }
                    #endregion

                    #region HasSelf
                    if (clsGraph[null, RDFVocabulary.OWL.HAS_SELF, null, RDFTypedLiteral.True].TriplesCount > 0)
                    {
                        LoadObjectHasSelf(ont, clsIRI, out OWLObjectHasSelf objHS);
                        if (objHS != null)
                        {
                            clex = objHS;
                            return;
                        }
                    }
                    #endregion

					#region HasValue
					if (clsGraph[null, RDFVocabulary.OWL.HAS_VALUE, null, null].TriplesCount > 0)
					{
						LoadObjectHasValue(ont, clsIRI, out OWLObjectHasValue objHV);
                        if (objHV != null)
                        {
                            clex = objHV;
                            return;
                        }
						LoadDataHasValue(ont, clsIRI, out OWLDataHasValue dtHV);
                        if (dtHV != null)
                        {
                            clex = dtHV;
                            return;
                        }
					}
                    #endregion

                    #region ExactCardinality
                    if (clsGraph[null, RDFVocabulary.OWL.CARDINALITY, null, null].TriplesCount > 0
                         || clsGraph[null, RDFVocabulary.OWL.QUALIFIED_CARDINALITY, null, null].TriplesCount > 0)
                    {
                        LoadObjectExactCardinality(ont, clsIRI, out OWLObjectExactCardinality objEXCR);
                        if (objEXCR != null)
                        {
                            clex = objEXCR;
                            return;
                        }
                        LoadDataExactCardinality(ont, clsIRI, out OWLDataExactCardinality dtEXCR);
                        if (dtEXCR != null)
                        {
                            clex = dtEXCR;
                            return;
                        }
                    }
                    #endregion

                    #region MinCardinality
                    if (clsGraph[null, RDFVocabulary.OWL.MIN_CARDINALITY, null, null].TriplesCount > 0
                         || clsGraph[null, RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, null, null].TriplesCount > 0)
                    {
                        LoadObjectMinCardinality(ont, clsIRI, out OWLObjectMinCardinality objMINCR);
                        if (objMINCR != null)
                        {
                            clex = objMINCR;
                            return;
                        }
                        LoadDataMinCardinality(ont, clsIRI, out OWLDataMinCardinality dtMINCR);
                        if (dtMINCR != null)
                        {
                            clex = dtMINCR;
                            return;
                        }
                    }
                    #endregion

                    #region MaxCardinality
                    if (clsGraph[null, RDFVocabulary.OWL.MAX_CARDINALITY, null, null].TriplesCount > 0
                         || clsGraph[null, RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, null, null].TriplesCount > 0)
                    {
                        LoadObjectMaxCardinality(ont, clsIRI, out OWLObjectMaxCardinality objMAXCR);
                        if (objMAXCR != null)
                        {
                            clex = objMAXCR;
                            return;
                        }
                        LoadDataMaxCardinality(ont, clsIRI, out OWLDataMaxCardinality dtMAXCR);
                        if (dtMAXCR != null)
                        {
                            clex = dtMAXCR;
                            return;
                        }
                    }
                    #endregion
                }
                #endregion

                #region Composite
                if (clsGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount > 0
					 && (clsGraph[null, RDFVocabulary.OWL.UNION_OF, null, null].TriplesCount > 0
					 	  || clsGraph[null, RDFVocabulary.OWL.INTERSECTION_OF, null, null].TriplesCount > 0
					 	  || clsGraph[null, RDFVocabulary.OWL.COMPLEMENT_OF, null, null].TriplesCount > 0))
				{
					#region UnionOf
					LoadObjectUnionOf(ont, clsIRI, out OWLObjectUnionOf objUNOF);
					if (objUNOF != null)
					{
						clex = objUNOF;
						return;
					}
					#endregion

					#region IntersectionOf
					LoadObjectIntersectionOf(ont, clsIRI, out OWLObjectIntersectionOf objINTOF);
					if (objINTOF != null)
					{
						clex = objINTOF;
						return;
					}
					#endregion

					#region ComplementOf
					LoadObjectComplementOf(ont, clsIRI, out OWLObjectComplementOf objCMPOF);
					if (objCMPOF != null)
					{
						clex = objCMPOF;
						return;
					}
					#endregion
				}
				#endregion

				#region Enumerate
				if (clsGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount > 0
					 && clsGraph[null, RDFVocabulary.OWL.ONE_OF, null, null].TriplesCount > 0)
				{
					#region OneOf
					LoadObjectOneOf(ont, clsIRI, out OWLObjectOneOf objONEOF);
					if (objONEOF != null)
					{
						clex = objONEOF;
						return;
					}
					#endregion
				}
				#endregion

				#region Class				
				if (clsGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount > 0)
					clex = new OWLClass(clsIRI);
				#endregion
            }
             void LoadObjectAllValuesFrom(OWLOntology ont, RDFResource clsIRI, RDFResource allValuesFrom, out OWLObjectAllValuesFrom objAVF)
             {
                 objAVF = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadObjectPropertyExpression(ont, onProperty, out OWLObjectPropertyExpression onPropertyOPEX);
                     if (onPropertyOPEX == null)
                         return;
             
                     LoadClassExpression(ont, allValuesFrom, out OWLClassExpression allValuesFromCLEX);
                     if (allValuesFromCLEX != null)
                         objAVF = new OWLObjectAllValuesFrom(onPropertyOPEX, allValuesFromCLEX);
                 }
             }
             void LoadObjectSomeValuesFrom(OWLOntology ont, RDFResource clsIRI, RDFResource someValuesFrom, out OWLObjectSomeValuesFrom objSVF)
             {
                 objSVF = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadObjectPropertyExpression(ont, onProperty, out OWLObjectPropertyExpression onPropertyOPEX);
                     if (onPropertyOPEX == null)
                         return;
             
                     LoadClassExpression(ont, someValuesFrom, out OWLClassExpression someValuesFromCLEX);
                     if (someValuesFromCLEX != null)
                         objSVF = new OWLObjectSomeValuesFrom(onPropertyOPEX, someValuesFromCLEX);
                 }
             }
             void LoadObjectHasSelf(OWLOntology ont, RDFResource clsIRI, out OWLObjectHasSelf objHS)
             {
                 objHS = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadObjectPropertyExpression(ont, onProperty, out OWLObjectPropertyExpression onPropertyOPEX);
                     if (onPropertyOPEX != null)
                         objHS = new OWLObjectHasSelf(onPropertyOPEX);
                 }
             }
             void LoadObjectHasValue(OWLOntology ont, RDFResource clsIRI, out OWLObjectHasValue objHV)
             {
                 objHV = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadObjectPropertyExpression(ont, onProperty, out OWLObjectPropertyExpression onPropertyOPEX);
                     if (onPropertyOPEX == null)
                         return;
             
                     if (graph[clsIRI, RDFVocabulary.OWL.HAS_VALUE, null, null].FirstOrDefault()?.Object is RDFResource hasValue)
                     {
                         LoadIndividualExpression(ont, hasValue, out OWLIndividualExpression hasValueIDVEX);
                         if (hasValueIDVEX != null)
                             objHV = new OWLObjectHasValue(onPropertyOPEX, hasValueIDVEX);
                     }
                 }
             }
             void LoadObjectExactCardinality(OWLOntology ont, RDFResource clsIRI, out OWLObjectExactCardinality objEXCR)
             {
                 objEXCR = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadObjectPropertyExpression(ont, onProperty, out OWLObjectPropertyExpression onPropertyOPEX);
                     if (onPropertyOPEX == null)
                         return;
             
                     //Cardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral exactCardLit
                          && exactCardLit.HasDecimalDatatype()
                          && uint.TryParse(exactCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint exactCardinality))
                     {
                         objEXCR = new OWLObjectExactCardinality(onPropertyOPEX, exactCardinality);
                         return;
                     }
             
                     //QualifiedCardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral exactQCardLit
                               && exactQCardLit.HasDecimalDatatype()
                               && uint.TryParse(exactQCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint exactQCardinality)
                               && graph[clsIRI, RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object is RDFResource onClass)
                     {
                         LoadClassExpression(ont, onClass, out OWLClassExpression onClassEX);
                         if (onClassEX != null)
                             objEXCR = new OWLObjectExactCardinality(onPropertyOPEX, exactQCardinality, onClassEX);
                     }
                 }
             }
             void LoadObjectMinCardinality(OWLOntology ont, RDFResource clsIRI, out OWLObjectMinCardinality objMINCR)
             {
                 objMINCR = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadObjectPropertyExpression(ont, onProperty, out OWLObjectPropertyExpression onPropertyOPEX);
                     if (onPropertyOPEX == null)
                         return;
             
                     //Cardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MIN_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral minCardLit
                          && minCardLit.HasDecimalDatatype()
                          && uint.TryParse(minCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint minCardinality))
                     {
                         objMINCR = new OWLObjectMinCardinality(onPropertyOPEX, minCardinality);
                         return;
                     }
             
                     //QualifiedCardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral minQCardLit
                               && minQCardLit.HasDecimalDatatype()
                               && uint.TryParse(minQCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint minQCardinality)
                               && graph[clsIRI, RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object is RDFResource onClass)
                     {
                         LoadClassExpression(ont, onClass, out OWLClassExpression onClassEX);
                         if (onClassEX != null)
                             objMINCR = new OWLObjectMinCardinality(onPropertyOPEX, minQCardinality, onClassEX);
                     }
                 }
             }
             void LoadObjectMaxCardinality(OWLOntology ont, RDFResource clsIRI, out OWLObjectMaxCardinality objMAXCR)
             {
                 objMAXCR = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadObjectPropertyExpression(ont, onProperty, out OWLObjectPropertyExpression onPropertyOPEX);
                     if (onPropertyOPEX == null)
                         return;
             
                     //Cardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MAX_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral maxCardLit
                          && maxCardLit.HasDecimalDatatype()
                          && uint.TryParse(maxCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint maxCardinality))
                     {
                         objMAXCR = new OWLObjectMaxCardinality(onPropertyOPEX, maxCardinality);
                         return;
                     }
             
                     //QualifiedCardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral maxQCardLit
                               && maxQCardLit.HasDecimalDatatype()
                               && uint.TryParse(maxQCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint maxQCardinality)
                               && graph[clsIRI, RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object is RDFResource onClass)
                     {
                         LoadClassExpression(ont, onClass, out OWLClassExpression onClassEX);
                         if (onClassEX != null)
                             objMAXCR = new OWLObjectMaxCardinality(onPropertyOPEX, maxQCardinality, onClassEX);
                     }
                 }
             }
             void LoadDataAllValuesFrom(OWLOntology ont, RDFResource clsIRI, RDFResource allValuesFrom, out OWLDataAllValuesFrom dtAVF)
             {
                 dtAVF = null;
             
                 LoadDataRangeExpression(ont, allValuesFrom, out OWLDataRangeExpression allValuesFromDREX);
                 if (allValuesFromDREX == null)
                     return;
             
                 foreach (RDFResource onProperty in graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null]
                                                     .Select(t => t.Object)
                                                     .OfType<RDFResource>())
                 {
                     LoadDataPropertyExpression(ont, onProperty, out OWLDataPropertyExpression onPropertyDPEX);
                     if (onPropertyDPEX != null)
                     {
                         if (dtAVF == null)
                             dtAVF = new OWLDataAllValuesFrom() { DataProperties = new List<OWLDataProperty>() };
                         dtAVF.DataProperties.Add((OWLDataProperty)onPropertyDPEX);
                         dtAVF.DataRangeExpression = allValuesFromDREX;
                     }
                 }
             }
             void LoadDataSomeValuesFrom(OWLOntology ont, RDFResource clsIRI, RDFResource someValuesFrom, out OWLDataSomeValuesFrom dtSVF)
             {
                 dtSVF = null;
             
                 LoadDataRangeExpression(ont, someValuesFrom, out OWLDataRangeExpression someValuesFromDREX);
                 if (someValuesFromDREX == null)
                     return;
             
                 foreach (RDFResource onProperty in graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null]
                                                     .Select(t => t.Object)
                                                     .OfType<RDFResource>())
                 {
                     LoadDataPropertyExpression(ont, onProperty, out OWLDataPropertyExpression onPropertyDPEX);
                     if (onPropertyDPEX != null)
                     {
                         if (dtSVF == null)
                             dtSVF = new OWLDataSomeValuesFrom() { DataProperties = new List<OWLDataProperty>() };
                         dtSVF.DataProperties.Add((OWLDataProperty)onPropertyDPEX);
                         dtSVF.DataRangeExpression = someValuesFromDREX;
                     }
                 }
             }
             void LoadDataHasValue(OWLOntology ont, RDFResource clsIRI, out OWLDataHasValue dtHV)
             {
                 dtHV = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadDataPropertyExpression(ont, onProperty, out OWLDataPropertyExpression onPropertyDPEX);
                     if (onPropertyDPEX == null)
                         return;
             
                     if (graph[clsIRI, RDFVocabulary.OWL.HAS_VALUE, null, null].FirstOrDefault()?.Object is RDFLiteral hasValueLIT)
                         dtHV = new OWLDataHasValue((OWLDataProperty)onPropertyDPEX, new OWLLiteral(hasValueLIT));
                 }
             }
             void LoadDataExactCardinality(OWLOntology ont, RDFResource clsIRI, out OWLDataExactCardinality dtEXCR)
             {
                 dtEXCR = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadDataPropertyExpression(ont, onProperty, out OWLDataPropertyExpression onPropertyDPEX);
                     if (onPropertyDPEX == null)
                         return;
             
                     //Cardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral exactCardLit
                          && exactCardLit.HasDecimalDatatype()
                          && uint.TryParse(exactCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint exactCardinality))
                     {
                         dtEXCR = new OWLDataExactCardinality((OWLDataProperty)onPropertyDPEX, exactCardinality);
                         return;
                     }
             
                     //QualifiedCardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral exactQCardLit
                               && exactQCardLit.HasDecimalDatatype()
                               && uint.TryParse(exactQCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint exactQCardinality)
                               && graph[clsIRI, RDFVocabulary.OWL.ON_DATARANGE, null, null].FirstOrDefault()?.Object is RDFResource onDataRange)
                     {
                         LoadDataRangeExpression(ont, onDataRange, out OWLDataRangeExpression onDataRangeEX);
                         if (onDataRangeEX != null)
                             dtEXCR = new OWLDataExactCardinality((OWLDataProperty)onPropertyDPEX, exactQCardinality, onDataRangeEX);
                     }
                 }
             }
             void LoadDataMinCardinality(OWLOntology ont, RDFResource clsIRI, out OWLDataMinCardinality dtMINCR)
             {
                 dtMINCR = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadDataPropertyExpression(ont, onProperty, out OWLDataPropertyExpression onPropertyDPEX);
                     if (onPropertyDPEX == null)
                         return;
             
                     //Cardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MIN_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral minCardLit
                          && minCardLit.HasDecimalDatatype()
                          && uint.TryParse(minCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint minCardinality))
                     {
                         dtMINCR = new OWLDataMinCardinality((OWLDataProperty)onPropertyDPEX, minCardinality);
                         return;
                     }
             
                     //QualifiedCardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral minQCardLit
                               && minQCardLit.HasDecimalDatatype()
                               && uint.TryParse(minQCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint minQCardinality)
                               && graph[clsIRI, RDFVocabulary.OWL.ON_DATARANGE, null, null].FirstOrDefault()?.Object is RDFResource onDataRange)
                     {
                         LoadDataRangeExpression(ont, onDataRange, out OWLDataRangeExpression onDataRangeEX);
                         if (onDataRangeEX != null)
                             dtMINCR = new OWLDataMinCardinality((OWLDataProperty)onPropertyDPEX, minQCardinality, onDataRangeEX);
                     }
                 }
             }
             void LoadDataMaxCardinality(OWLOntology ont, RDFResource clsIRI, out OWLDataMaxCardinality dtMAXCR)
             {
                 dtMAXCR = null;
             
                 RDFGraph onPropertyGraph = graph[clsIRI, RDFVocabulary.OWL.ON_PROPERTY, null, null];
                 if (onPropertyGraph.TriplesCount == 1 && onPropertyGraph.Single().Object is RDFResource onProperty)
                 {
                     LoadDataPropertyExpression(ont, onProperty, out OWLDataPropertyExpression onPropertyDPEX);
                     if (onPropertyDPEX == null)
                         return;
             
                     //Cardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MAX_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral maxCardLit
                          && maxCardLit.HasDecimalDatatype()
                          && uint.TryParse(maxCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint maxCardinality))
                     {
                         dtMAXCR = new OWLDataMaxCardinality((OWLDataProperty)onPropertyDPEX, maxCardinality);
                         return;
                     }
             
                     //QualifiedCardinality
                     if (graph[clsIRI, RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object is RDFTypedLiteral maxQCardLit
                               && maxQCardLit.HasDecimalDatatype()
                               && uint.TryParse(maxQCardLit.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint maxQCardinality)
                               && graph[clsIRI, RDFVocabulary.OWL.ON_DATARANGE, null, null].FirstOrDefault()?.Object is RDFResource onDataRange)
                     {
                         LoadDataRangeExpression(ont, onDataRange, out OWLDataRangeExpression onDataRangeEX);
                         if (onDataRangeEX != null)
                             dtMAXCR = new OWLDataMaxCardinality((OWLDataProperty)onPropertyDPEX, maxQCardinality, onDataRangeEX);
                     }
                 }
             }
             void LoadObjectUnionOf(OWLOntology ont, RDFResource clsIRI, out OWLObjectUnionOf objUNOF)
             {
                 objUNOF = null;
             
                 if (graph[clsIRI, RDFVocabulary.OWL.UNION_OF, null, null].FirstOrDefault()?.Object is RDFResource unionOf)
                 {
                     List<OWLClassExpression> objectUnionOfMembers = new List<OWLClassExpression>();
                     RDFCollection unionOfMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, unionOf, RDFModelEnums.RDFTripleFlavors.SPO);
                     foreach (RDFResource unionOfMember in unionOfMembers.Items.Cast<RDFResource>())
                     {
                         LoadClassExpression(ont, unionOfMember, out OWLClassExpression clsExp);
                         if (clsExp != null)
                             objectUnionOfMembers.Add(clsExp);
                     }
                     objUNOF = new OWLObjectUnionOf(objectUnionOfMembers);
                 }
             }
             void LoadObjectIntersectionOf(OWLOntology ont, RDFResource clsIRI, out OWLObjectIntersectionOf objINTOF)
             {
                 objINTOF = null;
             
                 if (graph[clsIRI, RDFVocabulary.OWL.INTERSECTION_OF, null, null].FirstOrDefault()?.Object is RDFResource intersectionOf)
                 {
                     List<OWLClassExpression> objectIntersectionOfMembers = new List<OWLClassExpression>();
                     RDFCollection intersectionOfMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, intersectionOf, RDFModelEnums.RDFTripleFlavors.SPO);
                     foreach (RDFResource intersectionOfMember in intersectionOfMembers.Items.Cast<RDFResource>())
                     {
                         LoadClassExpression(ont, intersectionOfMember, out OWLClassExpression clsExp);
                         if (clsExp != null)
                             objectIntersectionOfMembers.Add(clsExp);
                     }
                     objINTOF = new OWLObjectIntersectionOf(objectIntersectionOfMembers);
                 }
             }
             void LoadObjectComplementOf(OWLOntology ont, RDFResource clsIRI, out OWLObjectComplementOf objCMPOF)
             {
                 objCMPOF = null;
             
                 if (graph[clsIRI, RDFVocabulary.OWL.COMPLEMENT_OF, null, null].FirstOrDefault()?.Object is RDFResource complementOf)
                 {
                     LoadClassExpression(ont, complementOf, out OWLClassExpression clsExp);
                     if (clsExp != null)
                         objCMPOF = new OWLObjectComplementOf(clsExp);
                 }
             }
             void LoadObjectOneOf(OWLOntology ont, RDFResource clsIRI, out OWLObjectOneOf objONEOF)
             {
                 objONEOF = null;
             
                 if (graph[clsIRI, RDFVocabulary.OWL.ONE_OF, null, null].FirstOrDefault()?.Object is RDFResource oneOf)
                 {
                     List<OWLIndividualExpression> objectOneOfMembers = new List<OWLIndividualExpression>();
                     RDFCollection oneOfMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, oneOf, RDFModelEnums.RDFTripleFlavors.SPO);
                     foreach (RDFResource oneOfMember in oneOfMembers.Items.Cast<RDFResource>())
                     {
                         LoadIndividualExpression(ont, oneOfMember, out OWLIndividualExpression idvExp);
                         if (idvExp != null)
                             objectOneOfMembers.Add(idvExp);
                     }
                     objONEOF = new OWLObjectOneOf(objectOneOfMembers);
                 }
             }
            void LoadDataRangeExpression(OWLOntology ont, RDFResource drIRI, out OWLDataRangeExpression drex)
            {
                drex = null;
                RDFGraph drGraph = graph[drIRI, null, null, null];

                #region Composite
                if (drGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE, null].TriplesCount > 0
                     && (drGraph[null, RDFVocabulary.OWL.UNION_OF, null, null].TriplesCount > 0
                          || drGraph[null, RDFVocabulary.OWL.INTERSECTION_OF, null, null].TriplesCount > 0
                          || drGraph[null, RDFVocabulary.OWL.COMPLEMENT_OF, null, null].TriplesCount > 0))
                {
                    #region UnionOf
                    LoadDataUnionOf(ont, drIRI, out OWLDataUnionOf dtUNOF);
                    if (dtUNOF != null)
                    {
                        drex = dtUNOF;
                        return;
                    }
                    #endregion

                    #region IntersectionOf
                    LoadDataIntersectionOf(ont, drIRI, out OWLDataIntersectionOf dtINTOF);
                    if (dtINTOF != null)
                    {
                        drex = dtINTOF;
                        return;
                    }
                    #endregion

                    #region ComplementOf
                    LoadDataComplementOf(ont, drIRI, out OWLDataComplementOf dtCMPOF);
                    if (dtCMPOF != null)
                    {
                        drex = dtCMPOF;
                        return;
                    }
                    #endregion
                }
                #endregion

                #region Enumerate
                if (drGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE, null].TriplesCount > 0
                     && drGraph[null, RDFVocabulary.OWL.ONE_OF, null, null].TriplesCount > 0)
                {
                    #region OneOf
                    LoadDataOneOf(ont, drIRI, out OWLDataOneOf dtONEOF);
                    if (dtONEOF != null)
                    {
                        drex = dtONEOF;
                        return;
                    }
                    #endregion
                }
                #endregion

                #region DatatypeRestriction/Datatype
                if (drGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount > 0)
                {
                    #region DatatypeRestriction (Faceted Datatype)
                    if (drGraph[null, RDFVocabulary.OWL.WITH_RESTRICTIONS, null, null].FirstOrDefault()?.Object is RDFResource withRestrictions
                         && drGraph[null, RDFVocabulary.OWL.ON_DATATYPE, null, null].FirstOrDefault()?.Object is RDFResource onDatatype)
                    {
                        LoadDatatypeRestriction(ont, drIRI, onDatatype, withRestrictions, out OWLDatatypeRestriction dtRST);
                        if (dtRST != null)
                        {
                            drex = dtRST;
                            return;
                        }
                    }
                    #endregion

                    #region DatatypeRestriction (Alias Datatype)
                    else if (drGraph[null, RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null].FirstOrDefault()?.Object is RDFResource equivalentDatatype)
                    {
                        drex = new OWLDatatypeRestriction(new OWLDatatype(equivalentDatatype), null);
                        return;
                    }
                    #endregion

                    #region Datatype
                    drex = new OWLDatatype(drIRI);
                    #endregion
                }   
                #endregion
            }
             void LoadDataUnionOf(OWLOntology ont, RDFResource dtIRI, out OWLDataUnionOf dtUNOF)
             {
                 dtUNOF = null;
             
                 if (graph[dtIRI, RDFVocabulary.OWL.UNION_OF, null, null].FirstOrDefault()?.Object is RDFResource unionOf)
                 {
                     List<OWLDataRangeExpression> dtUnionOfMembers = new List<OWLDataRangeExpression>();
                     RDFCollection unionOfMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, unionOf, RDFModelEnums.RDFTripleFlavors.SPO);
                     foreach (RDFResource unionOfMember in unionOfMembers.Items.Cast<RDFResource>())
                     {
                         LoadDataRangeExpression(ont, unionOfMember, out OWLDataRangeExpression dtExp);
                         if (dtExp != null)
                             dtUnionOfMembers.Add(dtExp);
                     }
                     dtUNOF = new OWLDataUnionOf(dtUnionOfMembers);
                 }
             }
             void LoadDataIntersectionOf(OWLOntology ont, RDFResource dtIRI, out OWLDataIntersectionOf dtINTOF)
             {
                 dtINTOF = null;
             
                 if (graph[dtIRI, RDFVocabulary.OWL.INTERSECTION_OF, null, null].FirstOrDefault()?.Object is RDFResource intersectionOf)
                 {
                     List<OWLDataRangeExpression> dtIntersectionOfMembers = new List<OWLDataRangeExpression>();
                     RDFCollection intersectionOfMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, intersectionOf, RDFModelEnums.RDFTripleFlavors.SPO);
                     foreach (RDFResource intersectionOfMember in intersectionOfMembers.Items.Cast<RDFResource>())
                     {
                         LoadDataRangeExpression(ont, intersectionOfMember, out OWLDataRangeExpression dtExp);
                         if (dtExp != null)
                             dtIntersectionOfMembers.Add(dtExp);
                     }
                     dtINTOF = new OWLDataIntersectionOf(dtIntersectionOfMembers);
                 }
             }
             void LoadDataComplementOf(OWLOntology ont, RDFResource dtIRI, out OWLDataComplementOf dtCMPOF)
             {
                 dtCMPOF = null;
             
                 if (graph[dtIRI, RDFVocabulary.OWL.COMPLEMENT_OF, null, null].FirstOrDefault()?.Object is RDFResource complementOf)
                 {
                     LoadDataRangeExpression(ont, complementOf, out OWLDataRangeExpression dtExp);
                     if (dtExp != null)
                         dtCMPOF = new OWLDataComplementOf(dtExp);
                 }
             }
             void LoadDataOneOf(OWLOntology ont, RDFResource drIRI, out OWLDataOneOf dtONEOF)
             {
                 dtONEOF = null;
             
                 if (graph[drIRI, RDFVocabulary.OWL.ONE_OF, null, null].FirstOrDefault()?.Object is RDFResource oneOf)
                 {
                     List<OWLLiteral> dataOneOfMembers = new List<OWLLiteral>();
                     RDFCollection oneOfMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, oneOf, RDFModelEnums.RDFTripleFlavors.SPL);
                     foreach (RDFLiteral oneOfMember in oneOfMembers.Items.Cast<RDFLiteral>())
                         dataOneOfMembers.Add(new OWLLiteral(oneOfMember));
                     dtONEOF = new OWLDataOneOf(dataOneOfMembers);
                 }
             }
             void LoadDatatypeRestriction(OWLOntology ont, RDFResource drIRI, RDFResource onDatatype, RDFResource withRestrictions, out OWLDatatypeRestriction dtRST)
             {
                 dtRST = null;
             
                 List<OWLFacetRestriction> facetRestrictions = new List<OWLFacetRestriction>();
                 RDFCollection facetRestrictionMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, withRestrictions, RDFModelEnums.RDFTripleFlavors.SPO);
                 foreach (RDFResource facetRestrictionMember in facetRestrictionMembers.Items.Cast<RDFResource>())
                 {
                    //Statics (TODO: wait for RDFSharp-3.12)
                    if (graph[facetRestrictionMember, OWLFacetRestriction.LENGTH, null, null].FirstOrDefault()?.Object is RDFLiteral fctLengthDT)
                         facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctLengthDT), OWLFacetRestriction.LENGTH));
                    else if (graph[facetRestrictionMember, OWLFacetRestriction.MIN_LENGTH, null, null].FirstOrDefault()?.Object is RDFLiteral fctMinLengthDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMinLengthDT), OWLFacetRestriction.MIN_LENGTH));
                    else if (graph[facetRestrictionMember, OWLFacetRestriction.MAX_LENGTH, null, null].FirstOrDefault()?.Object is RDFLiteral fctMaxLengthDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMaxLengthDT), OWLFacetRestriction.MAX_LENGTH));
                    else if (graph[facetRestrictionMember, OWLFacetRestriction.PATTERN, null, null].FirstOrDefault()?.Object is RDFLiteral fctPatternDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctPatternDT), OWLFacetRestriction.PATTERN));
                    else if (graph[facetRestrictionMember, OWLFacetRestriction.MAX_INCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMaxInclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMaxInclusiveDT), OWLFacetRestriction.MAX_INCLUSIVE));
                    else if (graph[facetRestrictionMember, OWLFacetRestriction.MAX_EXCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMaxExclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMaxExclusiveDT), OWLFacetRestriction.MAX_INCLUSIVE));
                    else if (graph[facetRestrictionMember, OWLFacetRestriction.MIN_INCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMinInclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMinInclusiveDT), OWLFacetRestriction.MAX_INCLUSIVE));
                    else if (graph[facetRestrictionMember, OWLFacetRestriction.MIN_EXCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMinExclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMinExclusiveDT), OWLFacetRestriction.MAX_INCLUSIVE));
                 }

                 dtRST = new OWLDatatypeRestriction(new OWLDatatype(onDatatype), facetRestrictions);
             }
            #endregion

            #region Guards
            if (graph == null)
                throw new OWLException("Cannot read ontology from graph because: given \"graph\" parameter is null");
            if (graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 0)
                throw new OWLException("Cannot read ontology from graph because: no ontology declaration available in RDF data!");
            #endregion

			LoadOntology(out OWLOntology ontology);
            LoadImports(ontology);
            LoadPrefixes(ontology);
            LoadDeclarations(ontology);
            LoadOntologyAnnotations(ontology);			
			LoadFunctionalObjectProperties(ontology);
            LoadInverseFunctionalObjectProperties(ontology);
			LoadSymmetricObjectProperties(ontology);
            LoadAsymmetricObjectProperties(ontology);
			LoadReflexiveObjectProperties(ontology);
            LoadIrreflexiveObjectProperties(ontology);
			LoadTransitiveObjectProperties(ontology);
			LoadInverseObjectProperties(ontology);
			LoadEquivalentObjectProperties(ontology);
			LoadDisjointObjectProperties(ontology);
			LoadSubObjectProperties(ontology);
            LoadObjectPropertyDomain(ontology);
            LoadObjectPropertyRange(ontology);
            LoadFunctionalDataProperties(ontology);
            LoadEquivalentDataProperties(ontology);
            LoadDisjointDataProperties(ontology);
            LoadSubDataProperties(ontology);
            //TODO: DataPropertyDomain, DataPropertyRange
            //TODO: SubClassOf, EquivalentClasses, DisjointClasses, DisjointUnion
            //TODO: HasKey
            //TODO: DatatypeDefinition
            LoadSameIndividual(ontology);
            LoadDifferentIndividuals(ontology);
			LoadObjectPropertyAssertions(ontology);
            LoadNegativeObjectPropertyAssertions(ontology);
            LoadDataPropertyAssertions(ontology);
            LoadNegativeDataPropertyAssertions(ontology);
            //TODO: ClassAssertion
            LoadSubAnnotationProperties(ontology);
			LoadAnnotationPropertyDomain(ontology);
            LoadAnnotationPropertyRange(ontology);
            //TODO: AnnotationAssertion

            return ontology;
        }

        public static OWLOntology FromFile(OWLEnums.OWLFormats owlFormat, string inputFile)
        {
            #region Guards
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new OWLException("Cannot read ontology from file because given \"inputFile\" parameter is null or empty");
			if (!File.Exists(inputFile))
                throw new OWLException("Cannot read ontology from file because given \"inputFile\" parameter (" + inputFile + ") does not indicate an existing file");
            #endregion

            return FromStream(owlFormat, new FileStream(inputFile, FileMode.Open));
        }

        public static OWLOntology FromStream(OWLEnums.OWLFormats owlFormat, Stream inputStream)
        {
            #region Guards
            if (inputStream == null)
                throw new OWLException("Cannot read ontology from stream because given \"inputStream\" parameter is null");
            #endregion

			try
			{
				switch (owlFormat)
				{
					case OWLEnums.OWLFormats.Owl2Xml:
					default:						
						using (StreamReader streamReader = new StreamReader(inputStream, RDFModelUtilities.UTF8_NoBOM))
							return OWLSerializer.Deserialize(streamReader.ReadToEnd());
				}
			}
			catch(Exception ex)
			{
				throw new OWLException($"Cannot read ontology from stream because: {ex.Message}", ex);
			}
        }
        #endregion
    }
}