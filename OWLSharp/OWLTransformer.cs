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
			OWLOntology ontology = new OWLOntology();

			//TODO

			return ontology;
		}
	}
}