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
        public OWLOntology()
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

		public OWLOntology(OWLOntology ontology)
		{
			IRI = ontology?.IRI;
			VersionIRI = ontology?.VersionIRI;
			Prefixes = new List<OWLPrefix>(ontology?.Prefixes ?? 
						new List<OWLPrefix>()
						{
							new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.OWL.PREFIX)),
							new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDFS.PREFIX)),
							new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX)),
							new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XSD.PREFIX)),
							new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX))
						});
			Imports = new List<OWLImport>(ontology?.Imports ?? Enumerable.Empty<OWLImport>());
			Annotations = new List<OWLAnnotation>(ontology?.Annotations ?? Enumerable.Empty<OWLAnnotation>());
			//Axioms
			DeclarationAxioms = new List<OWLDeclaration>(ontology?.DeclarationAxioms ?? Enumerable.Empty<OWLDeclaration>());
			ClassAxioms = new List<OWLClassAxiom>(ontology?.ClassAxioms ?? Enumerable.Empty<OWLClassAxiom>());
			ObjectPropertyAxioms = new List<OWLObjectPropertyAxiom>(ontology?.ObjectPropertyAxioms ?? Enumerable.Empty<OWLObjectPropertyAxiom>());
			DataPropertyAxioms = new List<OWLDataPropertyAxiom>(ontology?.DataPropertyAxioms ?? Enumerable.Empty<OWLDataPropertyAxiom>());
			DatatypeDefinitionAxioms = new List<OWLDatatypeDefinition>(ontology?.DatatypeDefinitionAxioms ?? Enumerable.Empty<OWLDatatypeDefinition>());
			KeyAxioms = new List<OWLHasKey>(ontology?.KeyAxioms ?? Enumerable.Empty<OWLHasKey>());
			AssertionAxioms = new List<OWLAssertionAxiom>(ontology?.AssertionAxioms ?? Enumerable.Empty<OWLAssertionAxiom>());
			AnnotationAxioms = new List<OWLAnnotationAxiom>(ontology?.AnnotationAxioms ?? Enumerable.Empty<OWLAnnotationAxiom>());
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
					case OWLEnums.OWLFormats.OWL2XML:
					default:						
						string ontology = OWLSerializer.SerializeOntology(this);
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
			RDFGraph typeGraph = graph[null, RDFVocabulary.RDF.TYPE, null, null];

            #region Utilities
            //Ontology
			void LoadOntology(out OWLOntology ont)
            {
                string ontIRI = typeGraph[null, null, RDFVocabulary.OWL.ONTOLOGY, null]
                                 .FirstOrDefault()?.Subject.ToString() ?? throw new OWLException("Cannot find an ontology definition in the given graph!");
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
            void LoadDeclarations(OWLOntology ont)
            {
                //Class
                foreach (RDFTriple typeClass in typeGraph[null, null, RDFVocabulary.OWL.CLASS, null]
                                                 .UnionWith(typeGraph[null, null, RDFVocabulary.OWL.DEPRECATED_CLASS, null])
                                                 .UnionWith(typeGraph[null, null, RDFVocabulary.RDFS.CLASS, null])
                                                 .Where(t => !((RDFResource)t.Subject).IsBlank))
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLClass((RDFResource)typeClass.Subject)));

				//Datatype
                foreach (RDFTriple typeDatatype in typeGraph[null, null, RDFVocabulary.RDFS.DATATYPE, null]
													.Where(t => !((RDFResource)t.Subject).IsBlank))
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLDatatype((RDFResource)typeDatatype.Subject)));

				//ObjectProperty
                foreach (RDFTriple typeObjectProperty in typeGraph[null, null, RDFVocabulary.OWL.OBJECT_PROPERTY, null]
														  .Where(t => !((RDFResource)t.Subject).IsBlank))
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLObjectProperty((RDFResource)typeObjectProperty.Subject)));

				//DataProperty
                foreach (RDFTriple typeDataProperty in typeGraph[null, null, RDFVocabulary.OWL.DATATYPE_PROPERTY, null]
														.Where(t => !((RDFResource)t.Subject).IsBlank))
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLDataProperty((RDFResource)typeDataProperty.Subject)));

				//AnnotationProperty
                foreach (RDFTriple typeAnnotationProperty in typeGraph[null, null, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null]
															  .Where(t => !((RDFResource)t.Subject).IsBlank))
                    ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLAnnotationProperty((RDFResource)typeAnnotationProperty.Subject)));

				//NamedIndividual (OWL)
				List<RDFResource> namedIndividuals = new List<RDFResource>();
				foreach (RDFTriple typeNamedIndividual in typeGraph[null, null, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null]
															  .Where(t => !((RDFResource)t.Subject).IsBlank))
					namedIndividuals.Add((RDFResource)typeNamedIndividual.Subject);
				//NamedIndividual (SPARQL)
                RDFSelectQuery namedIdvQuery = new RDFSelectQuery()
					.AddPatternGroup(new RDFPatternGroup()
						.AddPattern(new RDFPattern(new RDFVariable("?CLS"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS).UnionWithNext())
						.AddPattern(new RDFPattern(new RDFVariable("?CLS"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION))
						.AddPattern(new RDFPattern(new RDFVariable("?NIDV"), RDFVocabulary.RDF.TYPE, new RDFVariable("?CLS")))
						.AddFilter(new RDFBooleanNotFilter(new RDFSameTermFilter(new RDFVariable("?NIDV"), new RDFVariable("?CLS"))))
						.AddFilter(new RDFIsUriFilter(new RDFVariable("?NIDV"))))
					.AddProjectionVariable(new RDFVariable("?NIDV"))
					.AddModifier(new RDFDistinctModifier());
				RDFSelectQueryResult namedIdvQueryResult = namedIdvQuery.ApplyToGraph(typeGraph);
				foreach (DataRow nidvRow in namedIdvQueryResult.SelectResults.Rows)
					namedIndividuals.Add(new RDFResource(nidvRow["?NIDV"].ToString()));
				//NamedIndividual (OWL + SPARQL)
				RDFQueryUtilities.RemoveDuplicates(namedIndividuals)
								 .ForEach(nidv => ont.DeclarationAxioms.Add(new OWLDeclaration(new OWLNamedIndividual(nidv))));
            }
            void PrefetchAnnotationAxioms(OWLOntology ont, out RDFGraph annAxiomsGraph)
            {
                RDFConstructQuery query = new RDFConstructQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFVariable("?ANNOTATED_SOURCE")))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_PROPERTY, new RDFVariable("?ANNOTATED_PROPERTY")))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFVariable("?ANNOTATED_TARGET")))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), new RDFVariable("?ANNPROP"), new RDFVariable("?ANNVAL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?ANNPROP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY)))
                    .AddTemplate(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM))
                    .AddTemplate(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFVariable("?ANNOTATED_SOURCE")))
                    .AddTemplate(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_PROPERTY, new RDFVariable("?ANNOTATED_PROPERTY")))
                    .AddTemplate(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFVariable("?ANNOTATED_TARGET")))
                    .AddTemplate(new RDFPattern(new RDFVariable("?AXIOM"), new RDFVariable("?ANNPROP"), new RDFVariable("?ANNVAL")))
                    .AddTemplate(new RDFPattern(new RDFVariable("?ANNPROP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY));
                RDFConstructQueryResult result = query.ApplyToGraph(graph);
                annAxiomsGraph = result.ToRDFGraph();
            }
            void LoadOntologyAnnotations(OWLOntology ont, RDFGraph annAxiomsGraph)
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
					}, new RDFResource(ont.IRI), annAxiomsGraph, out List <OWLAnnotation> ontologyAnnotations);
                ont.Annotations = ontologyAnnotations;
            }
            //Axioms
            void LoadFunctionalObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple funcPropTriple in typeGraph[null, null, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)funcPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
					{
                        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty() { 
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, funcPropTriple, functionalObjectProperty, annAxiomsGraph);

                    	ont.ObjectPropertyAxioms.Add(functionalObjectProperty);
					}
                }
            }
            void LoadInverseFunctionalObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple invfuncPropTriple in typeGraph[null, null, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)invfuncPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, invfuncPropTriple, inverseFunctionalObjectProperty, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(inverseFunctionalObjectProperty);
                    }
                }
            }
			void LoadSymmetricObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple symPropTriple in typeGraph[null, null, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)symPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, symPropTriple, symmetricObjectProperty, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(symmetricObjectProperty);
                    }
                }
            }
			void LoadAsymmetricObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple asymPropTriple in typeGraph[null, null, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)asymPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLAsymmetricObjectProperty asymmetricObjectProperty = new OWLAsymmetricObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, asymPropTriple, asymmetricObjectProperty, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(asymmetricObjectProperty);
                    }
                }
            }
            void LoadReflexiveObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple refPropTriple in typeGraph[null, null, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)refPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, refPropTriple, reflexiveObjectProperty, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(reflexiveObjectProperty);
                    }
                }
            }
            void LoadIrreflexiveObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple irrefPropTriple in typeGraph[null, null, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)irrefPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, irrefPropTriple, irreflexiveObjectProperty, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(irreflexiveObjectProperty);
                    }
                }
            }
			void LoadTransitiveObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple transPropTriple in typeGraph[null, null, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)transPropTriple.Subject, out OWLObjectPropertyExpression opex);
                    if (opex != null)
                    {
                        OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty() {
                            ObjectPropertyExpression = opex };

                        LoadAxiomAnnotations(ont, transPropTriple, transitiveObjectProperty, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(transitiveObjectProperty);
                    }
                }
            }
            void LoadInverseObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
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
                            LoadAxiomAnnotations(ont, new RDFTriple(OPL, RDFVocabulary.OWL.INVERSE_OF, OPR), inverseObjectProperties, annAxiomsGraph);
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
                            LoadAxiomAnnotations(ont, new RDFTriple(IOPL, RDFVocabulary.OWL.INVERSE_OF, OPR), inverseObjectProperties, annAxiomsGraph);
                            break;
                        case "OI":
                            OPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPL"].ToString());
                            IOPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IOPR"].ToString());
                            OPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPR"].ToString());
                            inverseObjectProperties.LeftObjectPropertyExpression = new OWLObjectProperty(OPL);
                            inverseObjectProperties.RightObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(OPR));
                            LoadAxiomAnnotations(ont, new RDFTriple(OPL, RDFVocabulary.OWL.INVERSE_OF, IOPR), inverseObjectProperties, annAxiomsGraph);
                            break;
                        case "II":
                            IOPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IOPL"].ToString());
                            OPL = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPL"].ToString());
                            IOPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IOPR"].ToString());
                            OPR = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?OPR"].ToString());
                            inverseObjectProperties.LeftObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(OPL));
                            inverseObjectProperties.RightObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(OPR));
                            LoadAxiomAnnotations(ont, new RDFTriple(IOPL, RDFVocabulary.OWL.INVERSE_OF, IOPR), inverseObjectProperties, annAxiomsGraph);
                            break;
                    }

                    ont.ObjectPropertyAxioms.Add(inverseObjectProperties);
                }
            }
			void LoadEquivalentObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple equivPropTriple in graph[null, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)equivPropTriple.Subject, out OWLObjectPropertyExpression leftOPE);
                    LoadObjectPropertyExpression(ont, (RDFResource)equivPropTriple.Object, out OWLObjectPropertyExpression rightOPE);

                    if (leftOPE != null && rightOPE != null)
                    {
                        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties() {
                            ObjectPropertyExpressions = new List<OWLObjectPropertyExpression>() { leftOPE, rightOPE } };

                        LoadAxiomAnnotations(ont, equivPropTriple, equivalentObjectProperties, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(equivalentObjectProperties);
                    }
                }
            }
			void LoadDisjointObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
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

						LoadAxiomAnnotations(ont, propDisjointWithTriple, disjointObjectProperties, annAxiomsGraph);

                    	ont.ObjectPropertyAxioms.Add(disjointObjectProperties);
					}
                }
			
				//Load axioms built with owl:AllDisjointProperties
				foreach (RDFTriple allDisjointPropertiesTriple in typeGraph[null, null, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null])
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
							}, (RDFResource)allDisjointPropertiesTriple.Subject, annAxiomsGraph, out List <OWLAnnotation> adjpAnnotations);
							disjointObjectProperties.Annotations = adjpAnnotations;

							ont.ObjectPropertyAxioms.Add(disjointObjectProperties);
						}
					}
			}
			void LoadSubObjectProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
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

                        LoadAxiomAnnotations(ont, propertyChainAxiomTriple, subObjectPropertyOf, annAxiomsGraph);

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

                        LoadAxiomAnnotations(ont, subPropTriple, subObjectPropertyOf, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(subObjectPropertyOf);
                    }
                }
            }
            void LoadObjectPropertyDomain(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple domainTriple in graph[null, RDFVocabulary.RDFS.DOMAIN, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)domainTriple.Subject, out OWLObjectPropertyExpression objEXP);
                    LoadClassExpression(ont, (RDFResource)domainTriple.Object, out OWLClassExpression clsEXP);

                    if (objEXP != null && clsEXP != null)
                    {
                        OWLObjectPropertyDomain objectPropertyDomain = new OWLObjectPropertyDomain() {
                             ObjectPropertyExpression = objEXP, ClassExpression = clsEXP };

                        LoadAxiomAnnotations(ont, domainTriple, objectPropertyDomain, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(objectPropertyDomain);
                    }
                }
            }
            void LoadObjectPropertyRange(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple rangeTriple in graph[null, RDFVocabulary.RDFS.RANGE, null, null])
                {
                    LoadObjectPropertyExpression(ont, (RDFResource)rangeTriple.Subject, out OWLObjectPropertyExpression objEXP);
                    LoadClassExpression(ont, (RDFResource)rangeTriple.Object, out OWLClassExpression clsEXP);

                    if (objEXP != null && clsEXP != null)
                    {
                        OWLObjectPropertyRange objectPropertyRange = new OWLObjectPropertyRange() {
                            ObjectPropertyExpression = objEXP, ClassExpression = clsEXP };

                        LoadAxiomAnnotations(ont, rangeTriple, objectPropertyRange, annAxiomsGraph);

                        ont.ObjectPropertyAxioms.Add(objectPropertyRange);
                    }
                }
            }
            void LoadFunctionalDataProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple funcPropTriple in typeGraph[null, null, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)funcPropTriple.Subject, out OWLDataPropertyExpression dpex);
                    if (dpex is OWLDataProperty dp)
                    {
                        OWLFunctionalDataProperty functionalDataProperty = new OWLFunctionalDataProperty(dp);

                        LoadAxiomAnnotations(ont, funcPropTriple, functionalDataProperty, annAxiomsGraph);

                        ont.DataPropertyAxioms.Add(functionalDataProperty);
                    }
                }
            }
            void LoadEquivalentDataProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple equivPropTriple in graph[null, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)equivPropTriple.Subject, out OWLDataPropertyExpression leftDPex);
                    LoadDataPropertyExpression(ont, (RDFResource)equivPropTriple.Object, out OWLDataPropertyExpression rightDPex);

                    if (leftDPex is OWLDataProperty leftDP && rightDPex is OWLDataProperty rightDP)
                    {
                        OWLEquivalentDataProperties equivalentDataProperties = new OWLEquivalentDataProperties() {
                            DataProperties = new List<OWLDataProperty>() { leftDP, rightDP } };

                        LoadAxiomAnnotations(ont, equivPropTriple, equivalentDataProperties, annAxiomsGraph);

                        ont.DataPropertyAxioms.Add(equivalentDataProperties);
                    }
                }
            }
            void LoadDisjointDataProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
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

                        LoadAxiomAnnotations(ont, propDisjointWithTriple, disjointDataProperties, annAxiomsGraph);

                        ont.DataPropertyAxioms.Add(disjointDataProperties);
                    }
                }

                //Load axioms built with owl:AllDisjointProperties
                foreach (RDFTriple allDisjointPropertiesTriple in typeGraph[null, null, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null])
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
                            }, (RDFResource)allDisjointPropertiesTriple.Subject, annAxiomsGraph, out List <OWLAnnotation> adjpAnnotations);
                            disjointDataProperties.Annotations = adjpAnnotations;

                            ont.DataPropertyAxioms.Add(disjointDataProperties);
                        }
                    }
            }
            void LoadSubDataProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple subPropTriple in graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)subPropTriple.Subject, out OWLDataPropertyExpression leftDPex);
                    LoadDataPropertyExpression(ont, (RDFResource)subPropTriple.Object, out OWLDataPropertyExpression rightDPex);

                    if (leftDPex is OWLDataProperty leftDP && rightDPex is OWLDataProperty rightDP)
                    {
                        OWLSubDataPropertyOf subDataPropertyOf = new OWLSubDataPropertyOf() {
                            SubDataProperty = leftDP, SuperDataProperty = rightDP };

                        LoadAxiomAnnotations(ont, subPropTriple, subDataPropertyOf, annAxiomsGraph);

                        ont.DataPropertyAxioms.Add(subDataPropertyOf);
                    }
                }
            }
            void LoadDataPropertyDomain(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple domainTriple in graph[null, RDFVocabulary.RDFS.DOMAIN, null, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)domainTriple.Subject, out OWLDataPropertyExpression dtEXP);
                    LoadClassExpression(ont, (RDFResource)domainTriple.Object, out OWLClassExpression clsEXP);

                    if (dtEXP is OWLDataProperty dp && clsEXP != null)
                    {
                        OWLDataPropertyDomain dataPropertyDomain = new OWLDataPropertyDomain() {
                             DataProperty = dp, ClassExpression = clsEXP };

                        LoadAxiomAnnotations(ont, domainTriple, dataPropertyDomain, annAxiomsGraph);

                        ont.DataPropertyAxioms.Add(dataPropertyDomain);
                    }
                }
            }
            void LoadDataPropertyRange(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple rangeTriple in graph[null, RDFVocabulary.RDFS.RANGE, null, null])
                {
                    LoadDataPropertyExpression(ont, (RDFResource)rangeTriple.Subject, out OWLDataPropertyExpression dtEXP);
                    LoadDataRangeExpression(ont, (RDFResource)rangeTriple.Object, out OWLDataRangeExpression drEXP);

                    if (dtEXP is OWLDataProperty dp && drEXP != null)
                    {
                        OWLDataPropertyRange dataPropertyRange = new OWLDataPropertyRange() {
                            DataProperty = dp, DataRangeExpression = drEXP };

                        LoadAxiomAnnotations(ont, rangeTriple, dataPropertyRange, annAxiomsGraph);

                        ont.DataPropertyAxioms.Add(dataPropertyRange);
                    }
                }
            }
			void LoadSubClassOf(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple subClassTriple in graph[null, RDFVocabulary.RDFS.SUB_CLASS_OF, null, null])
                {
                    LoadClassExpression(ont, (RDFResource)subClassTriple.Subject, out OWLClassExpression leftCLEX);
                    LoadClassExpression(ont, (RDFResource)subClassTriple.Object, out OWLClassExpression rightCLEX);

                    if (leftCLEX != null && rightCLEX != null)
                    {
                        OWLSubClassOf subClassOf = new OWLSubClassOf()
                        {
                            SubClassExpression = leftCLEX,
                            SuperClassExpression = rightCLEX
                        };

                        LoadAxiomAnnotations(ont, subClassTriple, subClassOf, annAxiomsGraph);

                        ont.ClassAxioms.Add(subClassOf);
                    }
                }
            }
            void LoadEquivalentClasses(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple equivClassTriple in graph[null, RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null])
                {
                    LoadClassExpression(ont, (RDFResource)equivClassTriple.Subject, out OWLClassExpression leftCLex);
                    LoadClassExpression(ont, (RDFResource)equivClassTriple.Object, out OWLClassExpression rightCLex);

                    if (leftCLex != null && rightCLex != null)
                    {
                        OWLEquivalentClasses equivalentClasses = new OWLEquivalentClasses() {
                             ClassExpressions = new List<OWLClassExpression>() { leftCLex, rightCLex } };

                        LoadAxiomAnnotations(ont, equivClassTriple, equivalentClasses, annAxiomsGraph);

                        ont.ClassAxioms.Add(equivalentClasses);
                    }
                }
            }
            void LoadDisjointClasses(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                //Load axioms built with owl:disjointWith
                foreach (RDFTriple disjointWithTriple in graph[null, RDFVocabulary.OWL.DISJOINT_WITH, null, null])
                {
                    LoadClassExpression(ont, (RDFResource)disjointWithTriple.Subject, out OWLClassExpression leftCLE);
                    LoadClassExpression(ont, (RDFResource)disjointWithTriple.Object, out OWLClassExpression rightCLE);

                    if (leftCLE != null && rightCLE != null)
                    {
                        OWLDisjointClasses disjointClasses = new OWLDisjointClasses() {
                            ClassExpressions = new List<OWLClassExpression>() { leftCLE, rightCLE } };

                        LoadAxiomAnnotations(ont, disjointWithTriple, disjointClasses, annAxiomsGraph);

                        ont.ClassAxioms.Add(disjointClasses);
                    }
                }

                //Load axioms built with owl:AllDisjointClasses
                foreach (RDFTriple allDisjointClassesTriple in typeGraph[null, null, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES, null])
                    if (graph[(RDFResource)allDisjointClassesTriple.Subject, RDFVocabulary.OWL.MEMBERS, null, null]
                         .FirstOrDefault()?.Object is RDFResource adjcCollectionRepresentative)
                    {
                        List<OWLClassExpression> adjcMembers = new List<OWLClassExpression>();

                        RDFCollection adjcCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, adjcCollectionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                        foreach (RDFResource adjcMember in adjcCollection.Items.Cast<RDFResource>())
                        {
                            LoadClassExpression(ont, adjcMember, out OWLClassExpression clex);
                            if (clex != null)
                                adjcMembers.Add(clex);
                        }

                        if (adjcMembers.Count >= 2)
                        {
                            OWLDisjointClasses disjointClasses = new OWLDisjointClasses() {
                                ClassExpressions = adjcMembers };

                            LoadIRIAnnotations(ont, new List<RDFResource>() {
                                RDFVocabulary.OWL.DEPRECATED,
                                RDFVocabulary.RDFS.COMMENT,
                                RDFVocabulary.RDFS.LABEL,
                                RDFVocabulary.RDFS.SEE_ALSO,
                                RDFVocabulary.RDFS.IS_DEFINED_BY
                            }, (RDFResource)allDisjointClassesTriple.Subject, annAxiomsGraph, out List <OWLAnnotation> adjcAnnotations);
                            disjointClasses.Annotations = adjcAnnotations;

                            ont.ClassAxioms.Add(disjointClasses);
                        }
                    }
            }
            void LoadDisjointUnion(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple disjointUnionOfTriple in graph[null, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null])
                {
                    LoadClassExpression(ont, (RDFResource)disjointUnionOfTriple.Subject, out OWLClassExpression clsExp);
                    if (!(clsExp is OWLClass classIRI))
                        continue;

                    List<OWLClassExpression> disjointUnionMembers = new List<OWLClassExpression>();
                    RDFCollection disjointUnionMembersCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)disjointUnionOfTriple.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    foreach (RDFResource disjointUnionMember in disjointUnionMembersCollection.Items.Cast<RDFResource>())
                    {
                        LoadClassExpression(ont, disjointUnionMember, out OWLClassExpression clsMemberExp);
                        if (clsMemberExp != null)
                            disjointUnionMembers.Add(clsMemberExp);
                    }

                    if (disjointUnionMembers.Count >= 2)
                    {
                        OWLDisjointUnion disjointUnion = new OWLDisjointUnion() {
                            ClassIRI = classIRI, ClassExpressions = disjointUnionMembers };

                        LoadAxiomAnnotations(ont, disjointUnionOfTriple, disjointUnion, annAxiomsGraph);

                        ont.ClassAxioms.Add(disjointUnion);
                    }
                }
            }
            void LoadHasKey(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple hasKeyTriple in graph[null, RDFVocabulary.OWL.HAS_KEY, null, null])
                {
                    LoadClassExpression(ont, (RDFResource)hasKeyTriple.Subject, out OWLClassExpression clsExp);
                    if (!(clsExp is OWLClass classIRI))
                        continue;

                    List<OWLObjectPropertyExpression> haskeyOPMembers = new List<OWLObjectPropertyExpression>();
                    List<OWLDataProperty> haskeyDPMembers = new List<OWLDataProperty>();
                    RDFCollection haskeyMembersCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)hasKeyTriple.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    foreach (RDFResource hasKeyMember in haskeyMembersCollection.Items.Cast<RDFResource>())
                    {
                        LoadObjectPropertyExpression(ont, hasKeyMember, out OWLObjectPropertyExpression objPropMember);
                        if (objPropMember != null)
                        {
                            haskeyOPMembers.Add(objPropMember);
                            continue;
                        }
                        LoadDataPropertyExpression(ont, hasKeyMember, out OWLDataPropertyExpression dtPropMember);
                        if (dtPropMember is OWLDataProperty dtProp)
                            haskeyDPMembers.Add(dtProp);
                    }

                    OWLHasKey hasKey = new OWLHasKey() {
                         ClassExpression = clsExp, ObjectPropertyExpressions = haskeyOPMembers, DataProperties = haskeyDPMembers };

                    LoadAxiomAnnotations(ont, hasKeyTriple, hasKey, annAxiomsGraph);

                    ont.KeyAxioms.Add(hasKey);
                }
            }
            void LoadDatatypeDefinition(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple datatypeTriple in typeGraph[null, null, RDFVocabulary.RDFS.DATATYPE, null]
                                                      .Where(t => !((RDFResource)t.Subject).IsBlank))                
                {
                    if (!(graph[(RDFResource)datatypeTriple.Subject, RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null]
                           .FirstOrDefault()?.Object is RDFResource equivalentDatatype))
                        continue;

                    LoadDataRangeExpression(ont, equivalentDatatype, out OWLDataRangeExpression drex);
                    if (drex == null)
                        continue;

                    OWLDatatypeDefinition datatypeDefinition = new OWLDatatypeDefinition()
                    {
                        Datatype = new OWLDatatype((RDFResource)datatypeTriple.Subject),
                        DataRangeExpression = drex
                    };

                    LoadAxiomAnnotations(ont, datatypeTriple, datatypeDefinition, annAxiomsGraph);

                    ont.DatatypeDefinitionAxioms.Add(datatypeDefinition);
                }
            }
            void LoadSameIndividual(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple sameAsTriple in graph[null, RDFVocabulary.OWL.SAME_AS, null, null])
                {
                    LoadIndividualExpression(ont, (RDFResource)sameAsTriple.Subject, out OWLIndividualExpression leftIE);
                    LoadIndividualExpression(ont, (RDFResource)sameAsTriple.Object, out OWLIndividualExpression rightIE);

                    if (leftIE != null && rightIE != null)
                    {
                        OWLSameIndividual sameIndividual = new OWLSameIndividual() {
                            IndividualExpressions = new List<OWLIndividualExpression>() { leftIE, rightIE } };

                        LoadAxiomAnnotations(ont, sameAsTriple, sameIndividual, annAxiomsGraph);

                        ont.AssertionAxioms.Add(sameIndividual);
                    }
                }
            }
            void LoadDifferentIndividuals(OWLOntology ont, RDFGraph annAxiomsGraph)
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

                        LoadAxiomAnnotations(ont, differentFromTriple, differentIndividuals, annAxiomsGraph);

                        ont.AssertionAxioms.Add(differentIndividuals);
                    }
                }

                //Load axioms built with owl:AllDifferent
                foreach (RDFTriple allDifferentTriple in typeGraph[null, null, RDFVocabulary.OWL.ALL_DIFFERENT, null])
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
                            }, (RDFResource)allDifferentTriple.Subject, annAxiomsGraph, out List <OWLAnnotation> adjpAnnotations);
                            differentIndividuals.Annotations = adjpAnnotations;

                            ont.AssertionAxioms.Add(differentIndividuals);
                        }
                    }
            }
			void LoadObjectPropertyAssertions(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple objPropTriple in typeGraph[null, null, RDFVocabulary.OWL.OBJECT_PROPERTY, null])
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

							LoadAxiomAnnotations(ont, objPropAsnTriple, objPropAsn, annAxiomsGraph);

							ont.AssertionAxioms.Add(objPropAsn);
						}
					}
                }
            }
            void LoadNegativeObjectPropertyAssertions(OWLOntology ont, RDFGraph annAxiomsGraph)
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
                            }, axiomIRI, annAxiomsGraph, out List <OWLAnnotation> nasnAnnotations);
                        negObjPropAsn.Annotations = nasnAnnotations;

                        ont.AssertionAxioms.Add(negObjPropAsn);
                    }
                }
            }
            void LoadDataPropertyAssertions(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple dtPropTriple in typeGraph[null, null, RDFVocabulary.OWL.DATATYPE_PROPERTY, null])
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

                            LoadAxiomAnnotations(ont, dtPropAsnTriple, dtPropAsn, annAxiomsGraph);

                            ont.AssertionAxioms.Add(dtPropAsn);
                        }
                    }
                }
            }
            void LoadClassAssertions(OWLOntology ont, RDFGraph annAxiomsGraph)
			{
				RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFVariable("?CLS")))
                        .AddPattern(new RDFPattern(new RDFVariable("?CLS"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS).UnionWithNext())
						.AddPattern(new RDFPattern(new RDFVariable("?CLS"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS).UnionWithNext())
						.AddPattern(new RDFPattern(new RDFVariable("?CLS"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_CLASS).UnionWithNext())
						.AddPattern(new RDFPattern(new RDFVariable("?CLS"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?IDV")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?CLS")))
                        .AddFilter(new RDFBooleanNotFilter(new RDFSameTermFilter(new RDFVariable("?IDV"), new RDFVariable("?CLS"))))
						.AddFilter(new RDFBooleanNotFilter(new RDFInFilter(new RDFVariable("?CLS"), new List<RDFPatternMember>() {
							RDFVocabulary.RDF.LIST, RDFVocabulary.RDFS.CLASS, RDFVocabulary.OWL.CLASS, RDFVocabulary.OWL.DEPRECATED_CLASS, RDFVocabulary.OWL.RESTRICTION }))));
                RDFSelectQueryResult result = query.ApplyToGraph(typeGraph);
                foreach (DataRow resultRow in result.SelectResults.Rows)
                {
					RDFResource clsIRI = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?CLS"].ToString());
					RDFResource idvIRI = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(resultRow["?IDV"].ToString());
					LoadClassExpression(ont, clsIRI, out OWLClassExpression clsEx);
                    LoadIndividualExpression(ont, idvIRI, out OWLIndividualExpression idvEx);

                    if (idvEx != null && clsEx != null)
                    {
                        OWLClassAssertion classAssertion = new OWLClassAssertion() {
                            ClassExpression = clsEx, IndividualExpression = idvEx };

                        LoadAxiomAnnotations(ont, new RDFTriple(idvIRI, RDFVocabulary.RDF.TYPE, clsIRI), classAssertion, annAxiomsGraph);

                        ont.AssertionAxioms.Add(classAssertion);
                    }
                }
			}
			void LoadNegativeDataPropertyAssertions(OWLOntology ont, RDFGraph annAxiomsGraph)
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
                        }, axiomIRI, annAxiomsGraph, out List <OWLAnnotation> nasnAnnotations);
                        negDtPropAsn.Annotations = nasnAnnotations;

                        ont.AssertionAxioms.Add(negDtPropAsn);
                    }
                }
            }
            void LoadAnnotationAssertions(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                #region Fetch Declarations
                List<OWLClass> declaredClasses = ont.DeclarationAxioms.Where(dax => dax.Expression is OWLClass)
                                                                      .Select(dax => (OWLClass)dax.Expression)
                                                                      .ToList();
                List<OWLDatatype> declaredDatatypes = ont.DeclarationAxioms.Where(dax => dax.Expression is OWLDatatype)
                                                                           .Select(dax => (OWLDatatype)dax.Expression)
                                                                           .ToList();
                List<OWLObjectProperty> declaredObjectProperties = ont.DeclarationAxioms.Where(dax => dax.Expression is OWLObjectProperty)
                                                                                        .Select(dax => (OWLObjectProperty)dax.Expression)
                                                                                        .ToList();
                List<OWLDataProperty> declaredDataProperties = ont.DeclarationAxioms.Where(dax => dax.Expression is OWLDataProperty)
                                                                                      .Select(dax => (OWLDataProperty)dax.Expression)
                                                                                      .ToList();
                List<OWLAnnotationProperty> declaredAnnotationProperties = ont.DeclarationAxioms.Where(dax => dax.Expression is OWLAnnotationProperty)
                                                                                                .Select(dax => (OWLAnnotationProperty)dax.Expression)
                                                                                                .ToList();
                List<OWLNamedIndividual> declaredIndividuals = ont.DeclarationAxioms.Where(dax => dax.Expression is OWLNamedIndividual)
                                                                                    .Select(dax => (OWLNamedIndividual)dax.Expression)
                                                                                    .ToList();
                #endregion

                declaredAnnotationProperties.ForEach(annProp =>
                {
                    RDFResource annPropIRI = annProp.GetIRI();
                    RDFGraph annPropGraph = graph[null, annPropIRI, null, null];

                    //Class Annotations
                    declaredClasses.ForEach(cls =>
                    {
                        RDFResource clsIRI = cls.GetIRI();
                        foreach (RDFTriple clsAnnPropTriple in annPropGraph[clsIRI, null, null, null])
                        {
                            OWLAnnotationAssertion annAsn;
                            if (clsAnnPropTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                                annAsn = new OWLAnnotationAssertion(annProp, clsIRI, (RDFResource)clsAnnPropTriple.Object);
                            else
                                annAsn = new OWLAnnotationAssertion(annProp, clsIRI, new OWLLiteral((RDFLiteral)clsAnnPropTriple.Object));

                            LoadAxiomAnnotations(ont, clsAnnPropTriple, annAsn, annAxiomsGraph);

                            ont.AnnotationAxioms.Add(annAsn);
                        }
                    });

                    //Datatype Annotations
                    declaredDatatypes.ForEach(dt =>
                    {
                        RDFResource dtIRI = dt.GetIRI();
                        foreach (RDFTriple dtAnnPropTriple in annPropGraph[dtIRI, null, null, null])
                        {
                            OWLAnnotationAssertion annAsn;
                            if (dtAnnPropTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                                annAsn = new OWLAnnotationAssertion(annProp, dtIRI, (RDFResource)dtAnnPropTriple.Object);
                            else
                                annAsn = new OWLAnnotationAssertion(annProp, dtIRI, new OWLLiteral((RDFLiteral)dtAnnPropTriple.Object));

                            LoadAxiomAnnotations(ont, dtAnnPropTriple, annAsn, annAxiomsGraph);

                            ont.AnnotationAxioms.Add(annAsn);
                        }
                    });

                    //ObjectProperty Annotations
                    declaredObjectProperties.ForEach(op =>
                    {
                        RDFResource opIRI = op.GetIRI();
                        foreach (RDFTriple opAnnPropTriple in annPropGraph[opIRI, null, null, null])
                        {
                            OWLAnnotationAssertion annAsn;
                            if (opAnnPropTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                                annAsn = new OWLAnnotationAssertion(annProp, opIRI, (RDFResource)opAnnPropTriple.Object);
                            else
                                annAsn = new OWLAnnotationAssertion(annProp, opIRI, new OWLLiteral((RDFLiteral)opAnnPropTriple.Object));

                            LoadAxiomAnnotations(ont, opAnnPropTriple, annAsn, annAxiomsGraph);

                            ont.AnnotationAxioms.Add(annAsn);
                        }
                    });

                    //DataProperty Annotations
                    declaredDataProperties.ForEach(dp =>
                    {
                        RDFResource dpIRI = dp.GetIRI();
                        foreach (RDFTriple dpAnnPropTriple in annPropGraph[dpIRI, null, null, null])
                        {
                            OWLAnnotationAssertion annAsn;
                            if (dpAnnPropTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                                annAsn = new OWLAnnotationAssertion(annProp, dpIRI, (RDFResource)dpAnnPropTriple.Object);
                            else
                                annAsn = new OWLAnnotationAssertion(annProp, dpIRI, new OWLLiteral((RDFLiteral)dpAnnPropTriple.Object));

                            LoadAxiomAnnotations(ont, dpAnnPropTriple, annAsn, annAxiomsGraph);

                            ont.AnnotationAxioms.Add(annAsn);
                        }
                    });

                    //AnnotationProperty Annotations
                    declaredAnnotationProperties.ForEach(ap =>
                    {
                        RDFResource apIRI = ap.GetIRI();
                        foreach (RDFTriple apAnnPropTriple in annPropGraph[apIRI, null, null, null])
                        {
                            OWLAnnotationAssertion annAsn;
                            if (apAnnPropTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                                annAsn = new OWLAnnotationAssertion(annProp, apIRI, (RDFResource)apAnnPropTriple.Object);
                            else
                                annAsn = new OWLAnnotationAssertion(annProp, apIRI, new OWLLiteral((RDFLiteral)apAnnPropTriple.Object));

                            LoadAxiomAnnotations(ont, apAnnPropTriple, annAsn, annAxiomsGraph);

                            ont.AnnotationAxioms.Add(annAsn);
                        }
                    });

                    //Individual Annotations
                    declaredIndividuals.ForEach(idv =>
                    {
                        RDFResource idvIRI = idv.GetIRI();
                        foreach (RDFTriple idvAnnPropTriple in annPropGraph[idvIRI, null, null, null])
                        {
                            OWLAnnotationAssertion annAsn;
                            if (idvAnnPropTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                                annAsn = new OWLAnnotationAssertion(annProp, idvIRI, (RDFResource)idvAnnPropTriple.Object);
                            else
                                annAsn = new OWLAnnotationAssertion(annProp, idvIRI, new OWLLiteral((RDFLiteral)idvAnnPropTriple.Object));

                            LoadAxiomAnnotations(ont, idvAnnPropTriple, annAsn, annAxiomsGraph);

                            ont.AnnotationAxioms.Add(annAsn);
                        }
                    });
                });
            }
            void LoadSubAnnotationProperties(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple subPropTriple in graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null])
                {
                    LoadAnnotationPropertyExpression(ont, (RDFResource)subPropTriple.Subject, out OWLAnnotationPropertyExpression leftAPex);
                    LoadAnnotationPropertyExpression(ont, (RDFResource)subPropTriple.Object, out OWLAnnotationPropertyExpression rightAPex);

                    if (leftAPex is OWLAnnotationProperty leftAP && rightAPex is OWLAnnotationProperty rightAP)
                    {
                        OWLSubAnnotationPropertyOf subAnnotationPropertyOf = new OWLSubAnnotationPropertyOf() {
                            SubAnnotationProperty = leftAP, SuperAnnotationProperty = rightAP };

                        LoadAxiomAnnotations(ont, subPropTriple, subAnnotationPropertyOf, annAxiomsGraph);

                        ont.AnnotationAxioms.Add(subAnnotationPropertyOf);
                    }
                }
            }
            void LoadAnnotationPropertyDomain(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple domainTriple in graph[null, RDFVocabulary.RDFS.DOMAIN, null, null])
                {
                    LoadAnnotationPropertyExpression(ont, (RDFResource)domainTriple.Subject, out OWLAnnotationPropertyExpression annEXP);

                    if (annEXP is OWLAnnotationProperty annProp && domainTriple.Object is RDFResource domainObject)
                    {
                        OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain() {
                             AnnotationProperty = annProp, IRI = domainObject.ToString() };

                        LoadAxiomAnnotations(ont, domainTriple, annotationPropertyDomain, annAxiomsGraph);

                        ont.AnnotationAxioms.Add(annotationPropertyDomain);
                    }
                }
            }
            void LoadAnnotationPropertyRange(OWLOntology ont, RDFGraph annAxiomsGraph)
            {
                foreach (RDFTriple rangeTriple in graph[null, RDFVocabulary.RDFS.RANGE, null, null])
                {
                    LoadAnnotationPropertyExpression(ont, (RDFResource)rangeTriple.Subject, out OWLAnnotationPropertyExpression annEXP);

                    if (annEXP is OWLAnnotationProperty annProp && rangeTriple.Object is RDFResource rangeObject)
                    {
                        OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange() {
                            AnnotationProperty = annProp, IRI = rangeObject.ToString() };

                        LoadAxiomAnnotations(ont, rangeTriple, annotationPropertyRange, annAxiomsGraph);

                        ont.AnnotationAxioms.Add(annotationPropertyRange);
                    }
                }
            }
			//Annotations
            void LoadAxiomAnnotations(OWLOntology ont, RDFTriple axiomTriple, OWLAxiom axiom, RDFGraph annAxiomsGraph)
            {
                #region Guards
                if (annAxiomsGraph.TriplesCount == 0)
                    return;
                #endregion

                RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_SOURCE, axiomTriple.Subject))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_PROPERTY, axiomTriple.Predicate))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_TARGET, axiomTriple.Object))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), new RDFVariable("?ANNPROP"), new RDFVariable("?ANNVAL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?ANNPROP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY)));
                RDFSelectQueryResult result = query.ApplyToGraph(annAxiomsGraph);
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

                    LoadNestedAnnotation(ont, annotationTriple, annotation, annAxiomsGraph);

                    axiom.Annotations.Add(annotation);
                }
            }
            void LoadIRIAnnotations(OWLOntology ont, List<RDFResource> annotationProperties, RDFResource iri, RDFGraph annAxiomsGraph, out List<OWLAnnotation> annotations)
			{
				annotations = new List<OWLAnnotation>();

				foreach (RDFTriple annPropTriple in typeGraph[null, null, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null]
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

                        LoadNestedAnnotation(ont, annotationTriple, annotation, annAxiomsGraph);

                        annotations.Add(annotation);
                    }
                }
			}
			void LoadNestedAnnotation(OWLOntology ont, RDFTriple annotationTriple, OWLAnnotation annotation, RDFGraph annAxiomsGraph)
            {
                #region Guards
                if (annAxiomsGraph.TriplesCount == 0)
                    return;
                #endregion

                RDFSelectQuery query = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_SOURCE, annotationTriple.Subject))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_PROPERTY, annotationTriple.Predicate))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), RDFVocabulary.OWL.ANNOTATED_TARGET, annotationTriple.Object))
                        .AddPattern(new RDFPattern(new RDFVariable("?AXIOM"), new RDFVariable("?ANNPROP"), new RDFVariable("?ANNVAL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?ANNPROP"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY)))
                    .AddModifier(new RDFLimitModifier(1));
                RDFSelectQueryResult result = query.ApplyToGraph(annAxiomsGraph);
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

                    LoadNestedAnnotation(ont, nestedAnnotationTriple, annotation.Annotation, annAxiomsGraph);
                }
            }
            //Expressions
            void LoadAnnotationPropertyExpression(OWLOntology ont, RDFResource apIRI, out OWLAnnotationPropertyExpression apex)
            {
                apex = null;
                if (typeGraph[apIRI, null, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount > 0)
                    apex = new OWLAnnotationProperty(apIRI);
            }
            void LoadObjectPropertyExpression(OWLOntology ont, RDFResource opIRI, out OWLObjectPropertyExpression opex)
            {
                opex = null;
				if (typeGraph[opIRI, null, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
					opex = new OWLObjectProperty(opIRI);
				else if ((graph[opIRI, RDFVocabulary.OWL.INVERSE_OF, null, null].FirstOrDefault()?.Object) is RDFResource objectProperty)
					opex = new OWLObjectInverseOf(new OWLObjectProperty(objectProperty));
			}
            void LoadDataPropertyExpression(OWLOntology ont, RDFResource dpIRI, out OWLDataPropertyExpression dpex)
            {
                dpex = null;
                if (typeGraph[dpIRI, null, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                    dpex = new OWLDataProperty(dpIRI);
            }
            void LoadIndividualExpression(OWLOntology ont, RDFResource idvIRI, out OWLIndividualExpression idvex)
            {
                idvex = null;
                if (idvIRI.IsBlank)
                    idvex = new OWLAnonymousIndividual(idvIRI.ToString().Substring(6));
                else if (typeGraph[idvIRI, null, null, null].TriplesCount > 0)
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
                    if (graph[facetRestrictionMember, RDFVocabulary.XSD.LENGTH, null, null].FirstOrDefault()?.Object is RDFLiteral fctLengthDT)
                         facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctLengthDT), RDFVocabulary.XSD.LENGTH));
                    else if (graph[facetRestrictionMember, RDFVocabulary.XSD.MIN_LENGTH, null, null].FirstOrDefault()?.Object is RDFLiteral fctMinLengthDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMinLengthDT), RDFVocabulary.XSD.MIN_LENGTH));
                    else if (graph[facetRestrictionMember, RDFVocabulary.XSD.MAX_LENGTH, null, null].FirstOrDefault()?.Object is RDFLiteral fctMaxLengthDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMaxLengthDT), RDFVocabulary.XSD.MAX_LENGTH));
                    else if (graph[facetRestrictionMember, RDFVocabulary.XSD.PATTERN, null, null].FirstOrDefault()?.Object is RDFLiteral fctPatternDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctPatternDT), RDFVocabulary.XSD.PATTERN));
                    else if (graph[facetRestrictionMember, RDFVocabulary.XSD.MAX_INCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMaxInclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMaxInclusiveDT), RDFVocabulary.XSD.MAX_INCLUSIVE));
                    else if (graph[facetRestrictionMember, RDFVocabulary.XSD.MAX_EXCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMaxExclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMaxExclusiveDT), RDFVocabulary.XSD.MAX_INCLUSIVE));
                    else if (graph[facetRestrictionMember, RDFVocabulary.XSD.MIN_INCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMinInclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMinInclusiveDT), RDFVocabulary.XSD.MAX_INCLUSIVE));
                    else if (graph[facetRestrictionMember, RDFVocabulary.XSD.MIN_EXCLUSIVE, null, null].FirstOrDefault()?.Object is RDFLiteral fctMinExclusiveDT)
                        facetRestrictions.Add(new OWLFacetRestriction(new OWLLiteral(fctMinExclusiveDT), RDFVocabulary.XSD.MAX_INCLUSIVE));
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

            //Ontology
			LoadOntology(out OWLOntology ontology);
            LoadImports(ontology);
            LoadDeclarations(ontology);
            PrefetchAnnotationAxioms(ontology, out RDFGraph annotationAxiomsGraph);
            LoadOntologyAnnotations(ontology, annotationAxiomsGraph);			
			//Axioms
            LoadFunctionalObjectProperties(ontology, annotationAxiomsGraph);
            LoadInverseFunctionalObjectProperties(ontology, annotationAxiomsGraph);
			LoadSymmetricObjectProperties(ontology, annotationAxiomsGraph);
            LoadAsymmetricObjectProperties(ontology, annotationAxiomsGraph);
			LoadReflexiveObjectProperties(ontology, annotationAxiomsGraph);
            LoadIrreflexiveObjectProperties(ontology, annotationAxiomsGraph);
			LoadTransitiveObjectProperties(ontology, annotationAxiomsGraph);
			LoadInverseObjectProperties(ontology, annotationAxiomsGraph);
			LoadEquivalentObjectProperties(ontology, annotationAxiomsGraph);
			LoadDisjointObjectProperties(ontology, annotationAxiomsGraph);
			LoadSubObjectProperties(ontology, annotationAxiomsGraph);
            LoadObjectPropertyDomain(ontology, annotationAxiomsGraph);
            LoadObjectPropertyRange(ontology, annotationAxiomsGraph);
            LoadFunctionalDataProperties(ontology, annotationAxiomsGraph);
            LoadEquivalentDataProperties(ontology, annotationAxiomsGraph);
            LoadDisjointDataProperties(ontology, annotationAxiomsGraph);
            LoadSubDataProperties(ontology, annotationAxiomsGraph);
            LoadDataPropertyDomain(ontology, annotationAxiomsGraph);
            LoadDataPropertyRange(ontology, annotationAxiomsGraph);
            LoadSubClassOf(ontology, annotationAxiomsGraph);
            LoadEquivalentClasses(ontology, annotationAxiomsGraph);
            LoadDisjointClasses(ontology, annotationAxiomsGraph);
            LoadDisjointUnion(ontology, annotationAxiomsGraph);
            LoadHasKey(ontology, annotationAxiomsGraph);
            LoadDatatypeDefinition(ontology, annotationAxiomsGraph);
            LoadSameIndividual(ontology, annotationAxiomsGraph);
            LoadDifferentIndividuals(ontology, annotationAxiomsGraph);
			LoadObjectPropertyAssertions(ontology, annotationAxiomsGraph);
            LoadNegativeObjectPropertyAssertions(ontology, annotationAxiomsGraph);
            LoadDataPropertyAssertions(ontology, annotationAxiomsGraph);
            LoadNegativeDataPropertyAssertions(ontology, annotationAxiomsGraph);
            LoadClassAssertions(ontology, annotationAxiomsGraph);
            LoadAnnotationAssertions(ontology, annotationAxiomsGraph);
            LoadSubAnnotationProperties(ontology, annotationAxiomsGraph);
			LoadAnnotationPropertyDomain(ontology, annotationAxiomsGraph);
            LoadAnnotationPropertyRange(ontology, annotationAxiomsGraph);

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
					case OWLEnums.OWLFormats.OWL2XML:
					default:						
						using (StreamReader streamReader = new StreamReader(inputStream, RDFModelUtilities.UTF8_NoBOM))
							return OWLSerializer.DeserializeOntology(streamReader.ReadToEnd());
				}
			}
			catch (Exception ex)
			{
				throw new OWLException($"Cannot read ontology from stream because: {ex.Message}", ex);
			}
        }

        public void Import(Uri importUri, int timeoutMilliseconds=20000)
        {
            #region Guards
            if (importUri == null)
                throw new OWLException("Cannot import ontology because given \"importUri\" parameter is null");
            #endregion

            try
            {
                RDFGraph importedGraph = RDFGraph.FromUri(importUri, timeoutMilliseconds);
                OWLOntology importedOntology = FromRDFGraph(importedGraph);

                Annotations.Add(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.OWL.IMPORTS), new RDFResource(importedOntology.IRI)));
                importedOntology.AnnotationAxioms.ForEach(ax => { ax.IsImport = true; AnnotationAxioms.Add(ax); });
                importedOntology.AssertionAxioms.ForEach(ax => { ax.IsImport = true; AssertionAxioms.Add(ax); });
                importedOntology.ClassAxioms.ForEach(ax => { ax.IsImport = true; ClassAxioms.Add(ax); });
                importedOntology.DataPropertyAxioms.ForEach(ax => { ax.IsImport = true; DataPropertyAxioms.Add(ax); });
                importedOntology.DatatypeDefinitionAxioms.ForEach(ax => { ax.IsImport = true; DatatypeDefinitionAxioms.Add(ax); });
                importedOntology.DeclarationAxioms.ForEach(ax => { ax.IsImport = true; DeclarationAxioms.Add(ax); });
                importedOntology.KeyAxioms.ForEach(ax => { ax.IsImport = true; KeyAxioms.Add(ax); });
                importedOntology.ObjectPropertyAxioms.ForEach(ax => { ax.IsImport = true; ObjectPropertyAxioms.Add(ax); });
                importedOntology.Prefixes.ForEach(pfx => {
                    if (!Prefixes.Any(PFX => string.Equals(PFX.Name, pfx.Name, StringComparison.OrdinalIgnoreCase)))
                        Prefixes.Add(pfx); });
            }
            catch (Exception ex)
            {
                throw new OWLException($"Cannot import ontology because: {ex.Message}", ex);
            }
        }
		#endregion
	}
}