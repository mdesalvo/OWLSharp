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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Test;
using RDFSharp.Model;


namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLHasKeyTest
    {
        #region Tests
        [TestMethod]
		public void ShouldCreateHasKey()
		{
			OWLHasKey hasKey = new OWLHasKey(
				new OWLClass(RDFVocabulary.FOAF.AGENT),
				[new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
				[new OWLDataProperty(RDFVocabulary.FOAF.AGE)]);

			Assert.IsNotNull(hasKey);
			Assert.IsTrue(hasKey.ClassExpression is OWLClass cls 
							&& string.Equals(cls.IRI, "http://xmlns.com/foaf/0.1/Agent"));
            Assert.IsTrue(hasKey.ObjectPropertyExpressions.Single() is OWLObjectProperty objProp 
							&& string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsTrue(hasKey.DataProperties.Single() is OWLDataProperty dtProp
                            && string.Equals(dtProp.IRI, "http://xmlns.com/foaf/0.1/age"));
        }

		[TestMethod]
		public void ShouldSerializeHasKey()
		{
            OWLHasKey hasKey = new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.FOCUS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]);
            string serializedXML = OWLTestSerializer<OWLHasKey>.Serialize(hasKey);

            Assert.IsTrue(string.Equals(serializedXML,
@"<HasKey><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/focus"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /></HasKey>"));
		}

		[TestMethod]
		public void ShouldDeserializeHasKey()
		{
			OWLHasKey hasKey = OWLTestSerializer<OWLHasKey>.Deserialize(
@"<HasKey><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/focus"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /></HasKey>");

            Assert.IsNotNull(hasKey);
            Assert.IsTrue(hasKey.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, "http://xmlns.com/foaf/0.1/Agent"));
            Assert.IsTrue(hasKey.ObjectPropertyExpressions[0] is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsTrue(hasKey.ObjectPropertyExpressions[1] is OWLObjectProperty objProp1
                            && string.Equals(objProp1.IRI, "http://xmlns.com/foaf/0.1/focus"));
            Assert.IsTrue(hasKey.DataProperties.Single() is OWLDataProperty dtProp
                            && string.Equals(dtProp.IRI, "http://xmlns.com/foaf/0.1/age"));
        }

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingHasKeyBecauseNullClassExpression()
			=> Assert.ThrowsException<OWLException>(() => new OWLHasKey(
                null,
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingHasKeyBecauseFoundNullObjectPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [null],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingHasKeyBecauseFoundNullDataPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [null]));

        [TestMethod]
        public void ShouldConvertHasKeyToGraph()
        {
            OWLHasKey hasKey = new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]);
            RDFGraph graph = hasKey.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 10);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.HAS_KEY, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertHasKeyWithAnnotationToGraph()
        {
            OWLHasKey hasKey = new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)])
				{
					Annotations = [
						new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
					]
				};
            RDFGraph graph = hasKey.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 16);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.HAS_KEY, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
			//Annotations
			Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.HAS_KEY, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertHasKeyWithSubAnnotationToGraph()
        {
            OWLHasKey hasKey = new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)])
				{
					Annotations = [
						new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
						{
							Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("title", RDFModelEnums.RDFDatatypes.XSD_STRING)))
						}
					]
				};
            RDFGraph graph = hasKey.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 22);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.HAS_KEY, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
			//Annotations+SubAnnotations
			Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 2);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.HAS_KEY, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.DC.TITLE, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:title"), null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, null].TriplesCount == 2);
			Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.DC.DCTERMS.TITLE, null, new RDFTypedLiteral("title", RDFModelEnums.RDFDatatypes.XSD_STRING)].TriplesCount == 1);
        }
        #endregion
    }
}