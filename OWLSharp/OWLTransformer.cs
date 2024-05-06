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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using System.Linq;
using OWLSharp.Ontology.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace OWLSharp
{	
	internal static class OWLTransformer
	{
		internal static RDFGraph Transform(OWLOntology ontology)
		{
			RDFGraph graph = new RDFGraph();

			//IRI
            RDFResource ontologyIRI = new RDFResource();
			if (!string.IsNullOrWhiteSpace(ontology.IRI))
				ontologyIRI = new RDFResource(ontology.IRI);
			graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));

			//VersionIRI
			if (!string.IsNullOrWhiteSpace(ontology.VersionIRI))
				graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.OWL.VERSION_IRI, new RDFResource(ontology.VersionIRI)));

			//Imports
			foreach (OWLImport import in ontology.Imports)
				graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.OWL.IMPORTS, new RDFResource(import.IRI)));

			//Annotations
			foreach (OWLAnnotation annotation in ontology.Annotations)
				graph = graph.UnionWith(annotation.ToRDFGraphInternal(ontologyIRI));

			//Axioms
			foreach (OWLDeclaration declarationAxiom in ontology.DeclarationAxioms)
				graph = graph.UnionWith(declarationAxiom.ToRDFGraph());
			foreach (OWLClassAxiom classAxiom in ontology.ClassAxioms)
				graph = graph.UnionWith(classAxiom.ToRDFGraph());
			foreach (OWLObjectPropertyAxiom objectPropertyAxiom in ontology.ObjectPropertyAxioms)
				graph = graph.UnionWith(objectPropertyAxiom.ToRDFGraph());
			foreach (OWLDataPropertyAxiom dataPropertyAxiom in ontology.DataPropertyAxioms)
				graph = graph.UnionWith(dataPropertyAxiom.ToRDFGraph());
			foreach (OWLDatatypeDefinition datatypeDefinitionAxiom in ontology.DatatypeDefinitionAxioms)
				graph = graph.UnionWith(datatypeDefinitionAxiom.ToRDFGraph());
			foreach (OWLHasKey keyAxiom in ontology.KeyAxioms)
				graph = graph.UnionWith(keyAxiom.ToRDFGraph());
			foreach (OWLAssertionAxiom assertionAxiom in ontology.AssertionAxioms)
				graph = graph.UnionWith(assertionAxiom.ToRDFGraph());
			foreach (OWLAnnotationAxiom annotationAxiom in ontology.AnnotationAxioms)
				graph = graph.UnionWith(annotationAxiom.ToRDFGraph());

			if (!ontologyIRI.IsBlank)
				graph.SetContext(ontologyIRI.URI);
            return graph;
		}

		internal static OWLOntology Transform(RDFGraph graph)
		{
			if (!TryLoadOntologyHeader(graph, out OWLOntology ontology))
				throw new OWLException("Cannot get ontology from graph because: no ontology declaration available in RDF data!");
			LoadImports(graph, ontology);
			LoadDeclarations(graph, ontology);
			LoadOntologyAnnotations(graph, ontology);

			return ontology;
		}

		#region Privates
		private static bool TryLoadOntologyHeader(RDFGraph graph, out OWLOntology ontology)
		{
			RDFGraph typeOntology = graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null];
			if (typeOntology.TriplesCount == 0)
			{
				ontology = null;
				return false;
			}

			string ontologyIRI = typeOntology.First().Subject.ToString();
			ontology = new OWLOntology()
			{
				IRI = ontologyIRI,
				VersionIRI = (graph[new RDFResource(ontologyIRI), RDFVocabulary.OWL.VERSION_IRI, null, null]
								.FirstOrDefault()?.Object as RDFResource)?.ToString()
			};
			return true;
		}

		private static void LoadImports(RDFGraph graph, OWLOntology ontology)
		{
            foreach (RDFTriple imports in graph[new RDFResource(ontology.IRI), RDFVocabulary.OWL.IMPORTS, null, null]
										   .Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
                ontology.Imports.Add(new OWLImport((RDFResource)imports.Object));
        }

        private static void LoadDeclarations(RDFGraph graph, OWLOntology ontology)
		{
			RDFGraph typeGraph = graph[null, RDFVocabulary.RDF.TYPE, null, null];

			foreach (RDFTriple typeClass in typeGraph[null, null, RDFVocabulary.OWL.CLASS, null]
											 .UnionWith(typeGraph[null, null, RDFVocabulary.RDFS.CLASS, null]))
				ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLClass((RDFResource)typeClass.Subject)));

            foreach (RDFTriple typeDatatype in typeGraph[null, null, RDFVocabulary.RDFS.DATATYPE, null])
                ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLDatatype((RDFResource)typeDatatype.Subject)));

            foreach (RDFTriple typeObjectProperty in typeGraph[null, null, RDFVocabulary.OWL.OBJECT_PROPERTY, null])
                ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLObjectProperty((RDFResource)typeObjectProperty.Subject)));

            foreach (RDFTriple typeDataProperty in typeGraph[null, null, RDFVocabulary.OWL.DATATYPE_PROPERTY, null])
                ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLDataProperty((RDFResource)typeDataProperty.Subject)));

            foreach (RDFTriple typeAnnotationProperty in typeGraph[null, null, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null])
                ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLAnnotationProperty((RDFResource)typeAnnotationProperty.Subject)));

            foreach (RDFTriple typeNamedIndividual in typeGraph[null, null, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null])
                ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLNamedIndividual((RDFResource)typeNamedIndividual.Subject)));
        }

        private static void LoadOntologyAnnotations(RDFGraph graph, OWLOntology ontology)
        {
			#region Facilities
			void LoadAnnotations(RDFResource workingAnnotationProperty, RDFGraph workingGraph)
			{
                OWLAnnotationProperty annotationProperty = new OWLAnnotationProperty(workingAnnotationProperty);
                foreach (RDFTriple annotation in workingGraph[null, workingAnnotationProperty, null, null])
                    if (annotation.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                        ontology.Annotations.Add(new OWLAnnotation(annotationProperty, (RDFResource)annotation.Object));
                    else
                        ontology.Annotations.Add(new OWLAnnotation(annotationProperty, new OWLLiteral((RDFLiteral)annotation.Object)));
            }
			#endregion

			RDFGraph ontologyGraph = graph[new RDFResource(ontology.IRI), null, null, null];

			//Load standard ontology annotations
            LoadAnnotations(RDFVocabulary.OWL.BACKWARD_COMPATIBLE_WITH, ontologyGraph);
            LoadAnnotations(RDFVocabulary.OWL.INCOMPATIBLE_WITH, ontologyGraph);
            LoadAnnotations(RDFVocabulary.OWL.PRIOR_VERSION, ontologyGraph);
            LoadAnnotations(RDFVocabulary.OWL.VERSION_INFO, ontologyGraph);
            LoadAnnotations(RDFVocabulary.OWL.DEPRECATED, ontologyGraph);
            LoadAnnotations(RDFVocabulary.RDFS.COMMENT, ontologyGraph);
            LoadAnnotations(RDFVocabulary.RDFS.LABEL, ontologyGraph);
            LoadAnnotations(RDFVocabulary.RDFS.SEE_ALSO, ontologyGraph);
            LoadAnnotations(RDFVocabulary.RDFS.IS_DEFINED_BY, ontologyGraph);

			//Load custom ontology annotation
			foreach (OWLDeclaration annPropDeclaration in ontology.DeclarationAxioms.Where(dax => dax.Expression is OWLAnnotationProperty annProp))
                LoadAnnotations(((OWLAnnotationProperty)annPropDeclaration.Expression).GetIRI(), ontologyGraph);
        }
        #endregion
    }
}