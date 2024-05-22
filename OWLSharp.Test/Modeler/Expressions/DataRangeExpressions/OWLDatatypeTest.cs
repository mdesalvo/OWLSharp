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

using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Modeler.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Modeler.Expressions
{
    [TestClass]
    public class OWLDatatypeTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIDatatype()
        {
            OWLDatatype dt = new OWLDatatype(RDFVocabulary.XSD.STRING);

            Assert.IsNotNull(dt);
            Assert.IsTrue(string.Equals(dt.IRI, RDFVocabulary.XSD.STRING.ToString()));
            Assert.IsNull(dt.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatype(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatype(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameDatatype()
        {
            OWLDatatype dt = new OWLDatatype(new XmlQualifiedName("string", RDFVocabulary.XSD.BASE_URI));

            Assert.IsNotNull(dt);
            Assert.IsNull(dt.IRI);
            Assert.IsTrue(string.Equals(dt.AbbreviatedIRI, new XmlQualifiedName("string", RDFVocabulary.XSD.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatype(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIDatatype()
        {
            OWLDatatype dt = new OWLDatatype(RDFVocabulary.XSD.STRING);
            string serializedXML = OWLTestSerializer<OWLDatatype>.Serialize(dt);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIDatatype()
        {
            OWLDatatype dt = OWLTestSerializer<OWLDatatype>.Deserialize(
@"<Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />");

            Assert.IsNotNull(dt);
            Assert.IsTrue(string.Equals(dt.IRI, RDFVocabulary.XSD.STRING.ToString()));
            Assert.IsNull(dt.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameDatatype()
        {
            OWLDatatype dt = new OWLDatatype(new XmlQualifiedName("string", RDFVocabulary.XSD.BASE_URI));
            string serializedXML = OWLTestSerializer<OWLDatatype>.Serialize(dt);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Datatype xmlns:q1=""http://www.w3.org/2001/XMLSchema#"" abbreviatedIRI=""q1:string"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameDatatype()
        {
            OWLDatatype dt = OWLTestSerializer<OWLDatatype>.Deserialize(
@"<Datatype xmlns:q1=""http://www.w3.org/2001/XMLSchema#"" abbreviatedIRI=""q1:string"" />");

            Assert.IsNotNull(dt);
            Assert.IsNull(dt.IRI);
            Assert.IsTrue(string.Equals(dt.AbbreviatedIRI, new XmlQualifiedName("string", RDFVocabulary.XSD.BASE_URI)));
        }

		[TestMethod]
        public void ShouldConvertIRIDatatypeToGraph()
        {
            OWLDatatype dt = new OWLDatatype(RDFVocabulary.XSD.STRING);
			RDFGraph graph = dt.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameDatatypeToGraph()
        {
            OWLDatatype dt = new OWLDatatype(new XmlQualifiedName("string", RDFVocabulary.XSD.BASE_URI));
			RDFGraph graph = dt.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertIRIDatatypeToResource()
        {
            OWLDatatype dt = new OWLDatatype(RDFVocabulary.XSD.STRING);
			RDFResource representative = dt.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.XSD.STRING));
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameDatatypeToResource()
        {
            OWLDatatype dt = new OWLDatatype(new XmlQualifiedName("string", RDFVocabulary.XSD.BASE_URI));
			RDFResource representative = dt.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.XSD.STRING));
        }
        #endregion
    }
}