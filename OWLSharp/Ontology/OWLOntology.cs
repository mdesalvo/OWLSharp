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
                                                 .UnionWith(typeGraph[null, null, RDFVocabulary.RDFS.CLASS, null]))
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

			//ObjectPropertyAxioms
			void LoadFunctionalObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple funcPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null])
                {
                    OWLFunctionalObjectProperty functionalObjectProperty;
                    if (graph[(RDFResource)funcPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                    {
                        if (graph[(RDFResource)funcPropTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
                            functionalObjectProperty = new OWLFunctionalObjectProperty(new OWLObjectProperty((RDFResource)funcPropTriple.Subject));
                        else continue; //Discard functional data properties, or functional untyped properties
                    }   
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)funcPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        functionalObjectProperty = new OWLFunctionalObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, funcPropTriple, functionalObjectProperty);

                    ont.ObjectPropertyAxioms.Add(functionalObjectProperty);
                }
            }
            void LoadInverseFunctionalObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple invfuncPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY, null])
                {
                    OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty;
                    if (graph[(RDFResource)invfuncPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                        inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty(new OWLObjectProperty((RDFResource)invfuncPropTriple.Subject));
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)invfuncPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, invfuncPropTriple, inverseFunctionalObjectProperty);

                    ont.ObjectPropertyAxioms.Add(inverseFunctionalObjectProperty);
                }
            }
			void LoadSymmetricObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple symPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null])
                {
                    OWLSymmetricObjectProperty symmetricObjectProperty;
                    if (graph[(RDFResource)symPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                        symmetricObjectProperty = new OWLSymmetricObjectProperty(new OWLObjectProperty((RDFResource)symPropTriple.Subject));
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)symPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        symmetricObjectProperty = new OWLSymmetricObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, symPropTriple, symmetricObjectProperty);

                    ont.ObjectPropertyAxioms.Add(symmetricObjectProperty);
                }
            }
			void LoadAsymmetricObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple asymPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY, null])
                {
                    OWLAsymmetricObjectProperty asymmetricObjectProperty;
                    if (graph[(RDFResource)asymPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                        asymmetricObjectProperty = new OWLAsymmetricObjectProperty(new OWLObjectProperty((RDFResource)asymPropTriple.Subject));
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)asymPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        asymmetricObjectProperty = new OWLAsymmetricObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, asymPropTriple, asymmetricObjectProperty);

                    ont.ObjectPropertyAxioms.Add(asymmetricObjectProperty);
                }
            }
            void LoadReflexiveObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple refPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null])
                {
                    OWLReflexiveObjectProperty reflexiveObjectProperty;
                    if (graph[(RDFResource)refPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                        reflexiveObjectProperty = new OWLReflexiveObjectProperty(new OWLObjectProperty((RDFResource)refPropTriple.Subject));
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)refPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        reflexiveObjectProperty = new OWLReflexiveObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, refPropTriple, reflexiveObjectProperty);

                    ont.ObjectPropertyAxioms.Add(reflexiveObjectProperty);
                }
            }
            void LoadIrreflexiveObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple irrefPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null])
                {
                    OWLIrreflexiveObjectProperty irreflexiveObjectProperty;
                    if (graph[(RDFResource)irrefPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                        irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(new OWLObjectProperty((RDFResource)irrefPropTriple.Subject));
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)irrefPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, irrefPropTriple, irreflexiveObjectProperty);

                    ont.ObjectPropertyAxioms.Add(irreflexiveObjectProperty);
                }
            }
			void LoadTransitiveObjectProperties(OWLOntology ont)
            {
                foreach (RDFTriple transPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null])
                {
                    OWLTransitiveObjectProperty transitiveObjectProperty;
                    if (graph[(RDFResource)transPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                        transitiveObjectProperty = new OWLTransitiveObjectProperty(new OWLObjectProperty((RDFResource)transPropTriple.Subject));
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)transPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        transitiveObjectProperty = new OWLTransitiveObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, transPropTriple, transitiveObjectProperty);

                    ont.ObjectPropertyAxioms.Add(transitiveObjectProperty);
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
                    OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties() {
						ObjectPropertyExpressions = new List<OWLObjectPropertyExpression>()	};

					//Left
                    if (graph[(RDFResource)equivPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
					{
						if (graph[(RDFResource)equivPropTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
							equivalentObjectProperties.ObjectPropertyExpressions.Add(new OWLObjectProperty((RDFResource)equivPropTriple.Subject));
						else continue; //Discard equivalent data properties, or equivalent untyped properties
					}
					else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)equivPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        equivalentObjectProperties.ObjectPropertyExpressions.Add(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

					//Right
					if (graph[(RDFResource)equivPropTriple.Object, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
					{
						if (graph[(RDFResource)equivPropTriple.Object, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
							equivalentObjectProperties.ObjectPropertyExpressions.Add(new OWLObjectProperty((RDFResource)equivPropTriple.Object));
						else continue; //Discard equivalent data properties, or equivalent untyped properties
					}
					else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)equivPropTriple.Object, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        equivalentObjectProperties.ObjectPropertyExpressions.Add(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
                    }

                    LoadAxiomAnnotations(ont, equivPropTriple, equivalentObjectProperties);

                    ont.ObjectPropertyAxioms.Add(equivalentObjectProperties);
                }
            }
			void LoadDisjointObjectProperties(OWLOntology ont)
			{
				//Load axioms built with owl:propertyDisjointWith
				foreach (RDFTriple propDisjointWithTriple in graph[null, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, null, null])
                {
					OWLObjectPropertyExpression leftOPE = null, rightOPE = null;
                    
					//Left
					if (graph[(RDFResource)propDisjointWithTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
                    {
                        if (graph[(RDFResource)propDisjointWithTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
                            leftOPE = new OWLObjectProperty((RDFResource)propDisjointWithTriple.Subject);
                    }   
                    else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)propDisjointWithTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        leftOPE = new OWLObjectInverseOf(new OWLObjectProperty(inverseOf));
                    }

					//Right
					if (graph[(RDFResource)propDisjointWithTriple.Object, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
					{
						if (graph[(RDFResource)propDisjointWithTriple.Object, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
							rightOPE = new OWLObjectProperty((RDFResource)propDisjointWithTriple.Object);
                    }
					else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)propDisjointWithTriple.Object, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        rightOPE = new OWLObjectInverseOf(new OWLObjectProperty(inverseOf));
                    }

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
							if (graph[adjpMember, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
							{
								if (graph[adjpMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
									adjpMembers.Add(new OWLObjectProperty(adjpMember));
                            }
							else
							{
								RDFResource inverseOf = (RDFResource)graph[adjpMember, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
								adjpMembers.Add(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
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
					OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf();

					//Left
					OWLObjectPropertyChain objectPropertyChain = new OWLObjectPropertyChain() {
						ObjectPropertyExpressions = new List<OWLObjectPropertyExpression>()	};
					RDFCollection chainAxiomMembers = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)propertyChainAxiomTriple.Object, RDFModelEnums.RDFTripleFlavors.SPO);
					foreach (RDFResource chainAxiomMember in chainAxiomMembers.Items.Cast<RDFResource>())
					{
						if (graph[chainAxiomMember, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
						{
							if (graph[chainAxiomMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
								objectPropertyChain.ObjectPropertyExpressions.Add(new OWLObjectProperty(chainAxiomMember));
							else continue; //Discard data properties, or untyped properties
						}
						else
						{
							RDFResource inverseOf = (RDFResource)graph[chainAxiomMember, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
							objectPropertyChain.ObjectPropertyExpressions.Add(new OWLObjectInverseOf(new OWLObjectProperty(inverseOf)));
						}
					}
					subObjectPropertyOf.SubObjectPropertyChain = objectPropertyChain;

					//Right
					if (graph[(RDFResource)propertyChainAxiomTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
					{
						if (graph[(RDFResource)propertyChainAxiomTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
							subObjectPropertyOf.SuperObjectPropertyExpression = new OWLObjectProperty((RDFResource)propertyChainAxiomTriple.Subject);
						else continue; //Discard sub data properties, or sub untyped properties
					}
					else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)propertyChainAxiomTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        subObjectPropertyOf.SuperObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(inverseOf));
                    }

					LoadAxiomAnnotations(ont, propertyChainAxiomTriple, subObjectPropertyOf);

                    ont.ObjectPropertyAxioms.Add(subObjectPropertyOf);
				}

				//Load axioms built with rdfs:subPropertyOf
                foreach (RDFTriple subPropTriple in graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null])
                {
                    OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf();

					//Left
					if (graph[(RDFResource)subPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
					{
						if (graph[(RDFResource)subPropTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
							subObjectPropertyOf.SubObjectPropertyExpression = new OWLObjectProperty((RDFResource)subPropTriple.Subject);
						else continue; //Discard sub data properties, or sub untyped properties
					}
					else
					{
						RDFResource inverseOf = (RDFResource)graph[(RDFResource)subPropTriple.Subject, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
						subObjectPropertyOf.SubObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(inverseOf));
					}

					//Right
					if (graph[(RDFResource)subPropTriple.Object, RDFVocabulary.OWL.INVERSE_OF, null, null].TriplesCount == 0)
					{
						if (graph[(RDFResource)subPropTriple.Object, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount > 0)
							subObjectPropertyOf.SuperObjectPropertyExpression = new OWLObjectProperty((RDFResource)subPropTriple.Object);
						else continue; //Discard sub data properties, or sub untyped properties
					}
					else
                    {
                        RDFResource inverseOf = (RDFResource)graph[(RDFResource)subPropTriple.Object, RDFVocabulary.OWL.INVERSE_OF, null, null].First().Object;
                        subObjectPropertyOf.SuperObjectPropertyExpression = new OWLObjectInverseOf(new OWLObjectProperty(inverseOf));
                    }

                    LoadAxiomAnnotations(ont, subPropTriple, subObjectPropertyOf);

                    ont.ObjectPropertyAxioms.Add(subObjectPropertyOf);
                }
            }
            //TODO: ObjectPropertyDomain, ObjectPropertyRange

            //DataPropertyAxioms
            void LoadFunctionalDataProperties(OWLOntology ont)
            {
                foreach (RDFTriple funcPropTriple in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null])
                    if (graph[(RDFResource)funcPropTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                    {
                        OWLFunctionalDataProperty functionalDataProperty = new OWLFunctionalDataProperty(new OWLDataProperty((RDFResource)funcPropTriple.Subject));

                        LoadAxiomAnnotations(ont, funcPropTriple, functionalDataProperty);

                        ont.DataPropertyAxioms.Add(functionalDataProperty);
                    }
            }
            void LoadEquivalentDataProperties(OWLOntology ont)
            {
                foreach (RDFTriple equivPropTriple in graph[null, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null])
                {
                    OWLEquivalentDataProperties equivalentDataProperties = new OWLEquivalentDataProperties() {
                        DataProperties = new List<OWLDataProperty>() };

                    //Left
                    if (graph[(RDFResource)equivPropTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                        equivalentDataProperties.DataProperties.Add(new OWLDataProperty((RDFResource)equivPropTriple.Subject));
                    else continue; //Discard equivalent object properties, or equivalent untyped properties

                    //Right
                    if (graph[(RDFResource)equivPropTriple.Object, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                        equivalentDataProperties.DataProperties.Add(new OWLDataProperty((RDFResource)equivPropTriple.Object));
                    else continue; //Discard equivalent object properties, or equivalent untyped properties

                    LoadAxiomAnnotations(ont, equivPropTriple, equivalentDataProperties);

                    ont.DataPropertyAxioms.Add(equivalentDataProperties);
                }
            }
            void LoadDisjointDataProperties(OWLOntology ont)
            {
                //Load axioms built with owl:propertyDisjointWith
                foreach (RDFTriple propDisjointWithTriple in graph[null, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, null, null])
                {
                    OWLDataProperty leftDP, rightDP;

                    //Left
                    if (graph[(RDFResource)propDisjointWithTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                        leftDP = new OWLDataProperty((RDFResource)propDisjointWithTriple.Subject);
                    else continue; //Discard disjoint data properties, or disjoint untyped properties

                    //Right
                    if (graph[(RDFResource)propDisjointWithTriple.Object, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                        rightDP = new OWLDataProperty((RDFResource)propDisjointWithTriple.Object);
                    else continue; //Discard disjoint data properties, or disjoint untyped properties

                    if (leftDP != null && rightDP != null)
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
                            if (graph[adjpMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                                adjpMembers.Add(new OWLDataProperty(adjpMember));
                            else continue; //Discard disjoint object properties, or disjoint untyped properties

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
                    OWLSubDataPropertyOf subDataPropertyOf = new OWLSubDataPropertyOf();

                    //Left
                    if (graph[(RDFResource)subPropTriple.Subject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                        subDataPropertyOf.SubDataProperty = new OWLDataProperty((RDFResource)subPropTriple.Subject);
                    else continue; //Discard sub object properties, or sub untyped properties

                    //Right
                    if (graph[(RDFResource)subPropTriple.Object, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount > 0)
                        subDataPropertyOf.SuperDataProperty = new OWLDataProperty((RDFResource)subPropTriple.Object);
                    else continue; //Discard sub object properties, or sub untyped properties

                    LoadAxiomAnnotations(ont, subPropTriple, subDataPropertyOf);

                    ont.DataPropertyAxioms.Add(subDataPropertyOf);
                }
            }
            //TODO: DataPropertyDomain, DataPropertyRange

            //ClassAxioms
            //TODO: EquivalentClasses, DisjointClasses, DisjointUnionOf, SubClassOf

            //HasKeyAxiom

            //DatatypeDefinition

            //AssertionAxioms
            //TODO: ClassAssertion, ObjectPropertyAssertion, NegativeObjectPrpertyAssertion, DataPropertyAssertion, NegativeDataPropertyAssertion
            void LoadSameIndividual(OWLOntology ont)
            {
                foreach (RDFTriple sameAsTriple in graph[null, RDFVocabulary.OWL.SAME_AS, null, null])
                {
                    OWLIndividualExpression leftIDVEXP = null, rightIDVEXP = null;

                    //Left
                    RDFResource subjectIdv = (RDFResource)sameAsTriple.Subject;
                    if (subjectIdv.IsBlank)
                        leftIDVEXP = new OWLAnonymousIndividual(subjectIdv.ToString().Substring(6));
                    else if (graph[subjectIdv, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount > 0)
                        leftIDVEXP = new OWLNamedIndividual(subjectIdv);

                    //Right
                    RDFResource objectIdv = (RDFResource)sameAsTriple.Object;
                    if (objectIdv.IsBlank)
                        rightIDVEXP = new OWLAnonymousIndividual(objectIdv.ToString().Substring(6));
                    else if (graph[objectIdv, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount > 0)
                        rightIDVEXP = new OWLNamedIndividual(objectIdv);

                    if (leftIDVEXP != null && rightIDVEXP != null)
                    {
                        OWLSameIndividual sameIndividual = new OWLSameIndividual() {
                            IndividualExpressions = new List<OWLIndividualExpression>() { leftIDVEXP, rightIDVEXP } };

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
                    OWLIndividualExpression leftIDVEXP = null, rightIDVEXP = null;

                    //Left
                    RDFResource subjectIdv = (RDFResource)differentFromTriple.Subject;
                    if (subjectIdv.IsBlank)
                        leftIDVEXP = new OWLAnonymousIndividual(subjectIdv.ToString().Substring(6));
                    else if (graph[subjectIdv, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount > 0)
                        leftIDVEXP = new OWLNamedIndividual(subjectIdv);

                    //Right
                    RDFResource objectIdv = (RDFResource)differentFromTriple.Object;
                    if (objectIdv.IsBlank)
                        rightIDVEXP = new OWLAnonymousIndividual(objectIdv.ToString().Substring(6));
                    else if (graph[objectIdv, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount > 0)
                        rightIDVEXP = new OWLNamedIndividual(objectIdv);

                    if (leftIDVEXP != null && rightIDVEXP != null)
                    {
                        OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals() {
                            IndividualExpressions = new List<OWLIndividualExpression>() { leftIDVEXP, rightIDVEXP } };

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
                            if (adiffMember.IsBlank)
                                adiffMembers.Add(new OWLAnonymousIndividual(adiffMember.ToString().Substring(6)));
                            else if (graph[adiffMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount > 0)
                                adiffMembers.Add(new OWLNamedIndividual(adiffMember));
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

            //AnnotationAxioms
            //TODO: AnnotationAssertion, SubAnnotationPropertyOf, AnnotationPropertyDomain, AnnotationPropertyRange

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

				foreach (OWLDeclaration annPropDeclaration in ont.DeclarationAxioms.Where(dax => dax.Expression is OWLAnnotationProperty daxAnnProp
                                                                                                  && !daxAnnProp.GetIRI().Equals(RDFVocabulary.OWL.VERSION_IRI)))
                {
                    RDFResource annPropIRI = ((OWLAnnotationProperty)annPropDeclaration.Expression).GetIRI();
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
            LoadPrefixes(ontology);
            LoadDeclarations(ontology);
            LoadOntologyAnnotations(ontology);
			
			//ObjectPropertyAxioms
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

            //DataPropertyAxioms
            LoadFunctionalDataProperties(ontology);
            LoadEquivalentDataProperties(ontology);
            LoadDisjointDataProperties(ontology);
            LoadSubDataProperties(ontology);

            //AssertionAxioms
            LoadSameIndividual(ontology);
            LoadDifferentIndividuals(ontology);

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