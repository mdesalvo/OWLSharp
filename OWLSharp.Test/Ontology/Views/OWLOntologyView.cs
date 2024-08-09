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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Views;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Views
{
	[TestClass]
	public class OWLOntologyViewTest
	{
		[TestMethod]
		public async Task ShouldCountAnnotationsAndImportsAndPrefixesAsync()
		{
			OWLOntology ont = new OWLOntology() 
			{
				Imports = [
					new OWLImport(new RDFResource("ex:org"))
				],
				Annotations = [
					new OWLAnnotation(
						new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
						new OWLLiteral(new RDFPlainLiteral("comment")))
				]
			};
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.AnnotationsCountAsync() == 1);
			Assert.IsTrue(await ontView.ImportsCountAsync() == 1);
			Assert.IsTrue(await ontView.PrefixesCountAsync() == 5);
		}
	}
}