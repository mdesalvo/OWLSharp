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

using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Test;
using RDFSharp.Model;


namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLDeclarationTest
    {
        #region Tests
        [TestMethod]
		public void ShouldCreateClassDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT));

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLClass cls && string.Equals(cls.IRI, "http://xmlns.com/foaf/0.1/Agent"));
		}

		[TestMethod]
		public void ShouldSerializeClassDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT));
			string serializedXML = OWLTestSerializer<OWLDeclaration>.Serialize(declaration);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Declaration><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /></Declaration>"));
		}

		[TestMethod]
		public void ShouldDeserializeClassDeclaration()
		{
			OWLDeclaration declaration = OWLTestSerializer<OWLDeclaration>.Deserialize(
@"<Declaration><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /></Declaration>");

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLClass cls && string.Equals(cls.IRI, "http://xmlns.com/foaf/0.1/Agent"));
		}

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingClassDeclarationBecauseNullClass()
			=> Assert.ThrowsException<OWLException>(() => new OWLDeclaration(null as OWLClass));

		[TestMethod]
		public void ShouldCreateDatatypeDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.INT));

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLDatatype dt && string.Equals(dt.IRI, "http://www.w3.org/2001/XMLSchema#int"));
		}

		[TestMethod]
		public void ShouldSerializeDatatypeDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.INT));
			string serializedXML = OWLTestSerializer<OWLDeclaration>.Serialize(declaration);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Declaration><Datatype IRI=""http://www.w3.org/2001/XMLSchema#int"" /></Declaration>"));
		}

		[TestMethod]
		public void ShouldDeserializeDatatypeDeclaration()
		{
			OWLDeclaration declaration = OWLTestSerializer<OWLDeclaration>.Deserialize(
@"<Declaration><Datatype IRI=""http://www.w3.org/2001/XMLSchema#int"" /></Declaration>");

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLDatatype dt && string.Equals(dt.IRI, "http://www.w3.org/2001/XMLSchema#int"));
		}

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingDatatypeDeclarationBecauseNullDatatype()
			=> Assert.ThrowsException<OWLException>(() => new OWLDeclaration(null as OWLDatatype));

		[TestMethod]
		public void ShouldCreateObjectPropertyDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLObjectProperty objProp && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
		}

		[TestMethod]
		public void ShouldSerializeObjectPropertyDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
			string serializedXML = OWLTestSerializer<OWLDeclaration>.Serialize(declaration);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Declaration><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></Declaration>"));
		}

		[TestMethod]
		public void ShouldDeserializeObjectPropertyDeclaration()
		{
			OWLDeclaration declaration = OWLTestSerializer<OWLDeclaration>.Deserialize(
@"<Declaration><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></Declaration>");

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLObjectProperty objProp && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
		}

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingObjectPropertyDeclarationBecauseNullObjectProperty()
			=> Assert.ThrowsException<OWLException>(() => new OWLDeclaration(null as OWLObjectProperty));

		[TestMethod]
		public void ShouldCreateDataPropertyDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE));

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLDataProperty dtProp && string.Equals(dtProp.IRI, "http://xmlns.com/foaf/0.1/age"));
		}

		[TestMethod]
		public void ShouldSerializeDataPropertyDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE));
			string serializedXML = OWLTestSerializer<OWLDeclaration>.Serialize(declaration);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Declaration><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /></Declaration>"));
		}

		[TestMethod]
		public void ShouldDeserializeDataPropertyDeclaration()
		{
			OWLDeclaration declaration = OWLTestSerializer<OWLDeclaration>.Deserialize(
@"<Declaration><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /></Declaration>");

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLDataProperty dtProp && string.Equals(dtProp.IRI, "http://xmlns.com/foaf/0.1/age"));
		}

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingDataPropertyDeclarationBecauseNullDataProperty()
			=> Assert.ThrowsException<OWLException>(() => new OWLDeclaration(null as OWLDataProperty));

		[TestMethod]
		public void ShouldCreateAnnotationPropertyDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR));

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLAnnotationProperty annProp && string.Equals(annProp.IRI, "http://purl.org/dc/elements/1.1/creator"));
		}

		[TestMethod]
		public void ShouldSerializeAnnotationPropertyDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR));
			string serializedXML = OWLTestSerializer<OWLDeclaration>.Serialize(declaration);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Declaration><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/creator"" /></Declaration>"));
		}

		[TestMethod]
		public void ShouldDeserializeAnnotationPropertyDeclaration()
		{
			OWLDeclaration declaration = OWLTestSerializer<OWLDeclaration>.Deserialize(
@"<Declaration><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/creator"" /></Declaration>");

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLAnnotationProperty annProp && string.Equals(annProp.IRI, "http://purl.org/dc/elements/1.1/creator"));
		}

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingAnnotationPropertyDeclarationBecauseNullAnnotationProperty()
			=> Assert.ThrowsException<OWLException>(() => new OWLDeclaration(null as OWLAnnotationProperty));

		[TestMethod]
		public void ShouldCreateNamedIndividualDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")));

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLNamedIndividual nIdv && string.Equals(nIdv.IRI, "ex:Mark"));
		}

		[TestMethod]
		public void ShouldSerializeNamedIndividualDeclaration()
		{
			OWLDeclaration declaration = new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")));
			string serializedXML = OWLTestSerializer<OWLDeclaration>.Serialize(declaration);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Declaration><NamedIndividual IRI=""ex:Mark"" /></Declaration>"));
		}

		[TestMethod]
		public void ShouldDeserializeNamedIndividualDeclaration()
		{
			OWLDeclaration declaration = OWLTestSerializer<OWLDeclaration>.Deserialize(
@"<Declaration><NamedIndividual IRI=""ex:Mark"" /></Declaration>");

			Assert.IsNotNull(declaration);
			Assert.IsTrue(declaration.Expression is OWLNamedIndividual nIdv && string.Equals(nIdv.IRI, "ex:Mark"));
		}

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingNamedIndividualDeclarationBecauseNullNamedIndividual()
			=> Assert.ThrowsException<OWLException>(() => new OWLDeclaration(null as OWLNamedIndividual));

		[TestMethod]
		public void ShouldConvertDeclarationToGraph()
		{
            OWLDeclaration declaration = new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT));
			RDFGraph graph = declaration.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }
		#endregion
	}
}