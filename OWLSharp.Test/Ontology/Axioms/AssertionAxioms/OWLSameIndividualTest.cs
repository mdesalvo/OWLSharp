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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Test;
using RDFSharp.Model;
using System.Linq;

namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLSameIndividualTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSameIndividual()
        {
            OWLSameIndividual SameIndividual = new OWLSameIndividual(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLNamedIndividual(new RDFResource("ex:Carl")) ]);

            Assert.IsNotNull(SameIndividual);
            Assert.IsNotNull(SameIndividual.IndividualExpressions);
			Assert.IsTrue(SameIndividual.IndividualExpressions.Count == 3);
            Assert.IsTrue(SameIndividual.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Alice")));
			Assert.IsTrue(SameIndividual.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Bob")));
			Assert.IsTrue(SameIndividual.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Carl")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameIndividualBecauseNullIndividualExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLSameIndividual(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameIndividualBecauseLessThan2IndividualExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLSameIndividual([new OWLNamedIndividual(new RDFResource("ex:Carl"))]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSameIndividualBecauseFoundNullIndividualExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLSameIndividual([new OWLNamedIndividual(new RDFResource("ex:Carl")), null]));

        [TestMethod]
        public void ShouldSerializeSameIndividual()
        {
            OWLSameIndividual SameIndividual = new OWLSameIndividual(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLNamedIndividual(new RDFResource("ex:Carl")) ]);
            string serializedXML = OWLTestSerializer<OWLSameIndividual>.Serialize(SameIndividual);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SameIndividual><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /><NamedIndividual IRI=""ex:Carl"" /></SameIndividual>"));
        }

		[TestMethod]
        public void ShouldSerializeSameIndividualViaOntology()
        {
			OWLOntology ontology = new OWLOntology();
			ontology.AssertionAxioms.Add(
				new OWLSameIndividual(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLAnonymousIndividual("AnonIdv") ]));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><SameIndividual><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /><AnonymousIndividual nodeID=""AnonIdv"" /></SameIndividual></Ontology>"));
        }

		[TestMethod]
        public void ShouldDeserializeNamedIndividuals()
        {
            OWLSameIndividual SameIndividual = OWLTestSerializer<OWLSameIndividual>.Deserialize(
@"<SameIndividual><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /><NamedIndividual IRI=""ex:Carl"" /></SameIndividual>");
        
			Assert.IsNotNull(SameIndividual);
            Assert.IsNotNull(SameIndividual.IndividualExpressions);
			Assert.IsTrue(SameIndividual.IndividualExpressions.Count == 3);
            Assert.IsTrue(SameIndividual.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Alice")));
			Assert.IsTrue(SameIndividual.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Bob")));
			Assert.IsTrue(SameIndividual.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Carl")));
		}

		[TestMethod]
        public void ShouldDeserializeNamedIndividualsViaOntology()
        {
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Ontology>
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SameIndividual>
    <Annotation>
		<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
		<Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <NamedIndividual IRI=""ex:Alice"" />
    <NamedIndividual IRI=""ex:Bob"" />
    <AnonymousIndividual nodeID=""AnonIdv"" />
  </SameIndividual>
</Ontology>");

			Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AssertionAxioms.Count == 1);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLSameIndividual diffAsn
                            && diffAsn.IndividualExpressions.Count == 3
							&& diffAsn.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv 
								&& string.Equals(nidv.IRI, "ex:Alice"))
							&& diffAsn.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv 
								&& string.Equals(nidv.IRI, "ex:Bob"))
							&& diffAsn.IndividualExpressions.Any(iex => iex is OWLAnonymousIndividual anonidv 
								&& string.Equals(anonidv.NodeID, "AnonIdv")));
			Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLSameIndividual diffAsn1
							&& string.Equals(diffAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(diffAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(diffAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }
        #endregion
    }
}