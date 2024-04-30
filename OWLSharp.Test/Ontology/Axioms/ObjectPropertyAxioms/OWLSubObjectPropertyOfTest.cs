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
    public class OWLSubObjectPropertyOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSubObjectPropertyOf()
        {
			OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
				new OWLObjectProperty(new RDFResource("ex:objPropA")),
                new OWLObjectProperty(new RDFResource("ex:objPropB")));

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectProperty subObjProp
                            && string.Equals(subObjProp.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldCreateSubObjectPropertyOfWithInverseOfAsRightExpression()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectProperty(new RDFResource("ex:objPropA")),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))));

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectProperty subObjProp
                            && string.Equals(subObjProp.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectInverseOf superObjInvOf
                            && string.Equals(superObjInvOf.ObjectProperty.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldCreateSubObjectPropertyOfWithInverseOfAsLeftExpression()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA"))),
                new OWLObjectProperty(new RDFResource("ex:objPropB")));

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectInverseOf subObjInvOf
                            && string.Equals(subObjInvOf.ObjectProperty.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldCreateSubObjectPropertyOfWithInverseOf()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA"))),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))));

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectInverseOf subObjInvOf
                            && string.Equals(subObjInvOf.ObjectProperty.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectInverseOf superObjInvOf
                            && string.Equals(superObjInvOf.ObjectProperty.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldCreateSubObjectPropertyOfWithChainAndObjectProperty()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectPropertyChain([
                    new OWLObjectProperty(new RDFResource("ex:hasFather")),
                    new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]),
                new OWLObjectProperty(new RDFResource("ex:hasUncle")));

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions.Count == 2);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[0]
                            is OWLObjectProperty firstChainProp && string.Equals(firstChainProp.IRI, "ex:hasFather"));
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[1]
                            is OWLObjectProperty secondChainProp && string.Equals(secondChainProp.IRI, "ex:hasBrother"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:hasUncle"));
        }

        [TestMethod]
        public void ShouldCreateSubObjectPropertyOfWithChainAndObjectInverseOf()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectPropertyChain([
                    new OWLObjectProperty(new RDFResource("ex:hasFather")),
                    new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasUncle"))));

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions.Count == 2);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[0]
                            is OWLObjectProperty firstChainProp && string.Equals(firstChainProp.IRI, "ex:hasFather"));
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[1]
                            is OWLObjectProperty secondChainProp && string.Equals(secondChainProp.IRI, "ex:hasBrother"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectInverseOf superObjInvOf
                            && string.Equals(superObjInvOf.ObjectProperty.IRI, "ex:hasUncle"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubObjectProperty1()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                null as OWLObjectProperty,
                new OWLObjectProperty(new RDFResource("ex:objPropB"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubObjectProperty2()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                null as OWLObjectProperty,
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB")))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubObjectProperty3()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                null as OWLObjectInverseOf,
                new OWLObjectProperty(new RDFResource("ex:objPropB"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubObjectProperty4()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                null as OWLObjectInverseOf,
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB")))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSuperObjectProperty1()
        => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:objPropB")),
            null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSuperObjectProperty2()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))),
                null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSuperObjectProperty3()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                new OWLObjectProperty(new RDFResource("ex:objPropB")),
                null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSuperObjectProperty4()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))),
                null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubPropertyChain1()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                null as OWLObjectPropertyChain,
                new OWLObjectProperty(new RDFResource("ex:objPropB"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubPropertyChain2()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                null as OWLObjectPropertyChain,
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB")))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubPropertyChain3()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                new OWLObjectPropertyChain([
                    new OWLObjectProperty(new RDFResource("ex:hasFather")),
                    new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]),
                null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubObjectPropertyOfBecauseNullSubPropertyChain4()
            => Assert.ThrowsException<OWLException>(() => new OWLSubObjectPropertyOf(
                new OWLObjectPropertyChain([
                    new OWLObjectProperty(new RDFResource("ex:hasFather")),
                    new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]),
                null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyOf()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectProperty(new RDFResource("ex:objPropA")),
                new OWLObjectProperty(new RDFResource("ex:objPropB")));
            string serializedXML = OWLTestSerializer<OWLSubObjectPropertyOf>.Serialize(subObjectPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubObjectPropertyOf><ObjectProperty IRI=""ex:objPropA"" /><ObjectProperty IRI=""ex:objPropB"" /></SubObjectPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyOfWithSubAsInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA"))),
                new OWLObjectProperty(new RDFResource("ex:objPropB")));
            string serializedXML = OWLTestSerializer<OWLSubObjectPropertyOf>.Serialize(subObjectPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubObjectPropertyOf><ObjectInverseOf><ObjectProperty IRI=""ex:objPropA"" /></ObjectInverseOf><ObjectProperty IRI=""ex:objPropB"" /></SubObjectPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyOfWithSuperAsInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectProperty(new RDFResource("ex:objPropA")),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))));
            string serializedXML = OWLTestSerializer<OWLSubObjectPropertyOf>.Serialize(subObjectPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubObjectPropertyOf><ObjectProperty IRI=""ex:objPropA"" /><ObjectInverseOf><ObjectProperty IRI=""ex:objPropB"" /></ObjectInverseOf></SubObjectPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyOfWithInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA"))),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))));
            string serializedXML = OWLTestSerializer<OWLSubObjectPropertyOf>.Serialize(subObjectPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubObjectPropertyOf><ObjectInverseOf><ObjectProperty IRI=""ex:objPropA"" /></ObjectInverseOf><ObjectInverseOf><ObjectProperty IRI=""ex:objPropB"" /></ObjectInverseOf></SubObjectPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyOfWithChain()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectPropertyChain([
                    new OWLObjectProperty(new RDFResource("ex:hasFather")),
                    new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]),
                new OWLObjectProperty(new RDFResource("ex:hasUncle")));
            string serializedXML = OWLTestSerializer<OWLSubObjectPropertyOf>.Serialize(subObjectPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubObjectPropertyOf><ObjectPropertyChain><ObjectProperty IRI=""ex:hasFather"" /><ObjectProperty IRI=""ex:hasBrother"" /></ObjectPropertyChain><ObjectProperty IRI=""ex:hasUncle"" /></SubObjectPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyOfWithChainAndInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectPropertyChain([
                    new OWLObjectProperty(new RDFResource("ex:hasFather")),
                    new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:isUncleOf"))));
            string serializedXML = OWLTestSerializer<OWLSubObjectPropertyOf>.Serialize(subObjectPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubObjectPropertyOf><ObjectPropertyChain><ObjectProperty IRI=""ex:hasFather"" /><ObjectProperty IRI=""ex:hasBrother"" /></ObjectPropertyChain><ObjectInverseOf><ObjectProperty IRI=""ex:isUncleOf"" /></ObjectInverseOf></SubObjectPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("ex:objPropA")),
                    new OWLObjectProperty(new RDFResource("ex:objPropB"))));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><SubObjectPropertyOf><ObjectProperty IRI=""ex:objPropA"" /><ObjectProperty IRI=""ex:objPropB"" /></SubObjectPropertyOf></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOf()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = OWLTestSerializer<OWLSubObjectPropertyOf>.Deserialize(
@"<SubObjectPropertyOf>
  <ObjectProperty IRI=""ex:objPropA"" />
  <ObjectProperty IRI=""ex:objPropB"" />
</SubObjectPropertyOf>");

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectProperty subObjProp
                            && string.Equals(subObjProp.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOfWithSubAsInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = OWLTestSerializer<OWLSubObjectPropertyOf>.Deserialize(
@"<SubObjectPropertyOf>
  <ObjectInverseOf>
    <ObjectProperty IRI=""ex:objPropA"" />
  </ObjectInverseOf>
  <ObjectProperty IRI=""ex:objPropB"" />
</SubObjectPropertyOf>");

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectInverseOf subObjInvOf
                            && string.Equals(subObjInvOf.ObjectProperty.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOfWithSuperAsInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = OWLTestSerializer<OWLSubObjectPropertyOf>.Deserialize(
@"<SubObjectPropertyOf>
  <ObjectProperty IRI=""ex:objPropA"" />
  <ObjectInverseOf>
    <ObjectProperty IRI=""ex:objPropB"" />
  </ObjectInverseOf>
</SubObjectPropertyOf>");

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectProperty subObjProp
                            && string.Equals(subObjProp.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectInverseOf superObjInvOf
                            && string.Equals(superObjInvOf.ObjectProperty.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOfWithInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = OWLTestSerializer<OWLSubObjectPropertyOf>.Deserialize(
@"<SubObjectPropertyOf>
  <ObjectInverseOf>
    <ObjectProperty IRI=""ex:objPropA"" />
  </ObjectInverseOf>
  <ObjectInverseOf>
    <ObjectProperty IRI=""ex:objPropB"" />
  </ObjectInverseOf>
</SubObjectPropertyOf>");

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyExpression is OWLObjectInverseOf subObjInvOf
                            && string.Equals(subObjInvOf.ObjectProperty.IRI, "ex:objPropA"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectInverseOf superObjInvOf
                            && string.Equals(superObjInvOf.ObjectProperty.IRI, "ex:objPropB"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOfWithChain()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = OWLTestSerializer<OWLSubObjectPropertyOf>.Deserialize(
@"<SubObjectPropertyOf>
  <ObjectPropertyChain>
    <ObjectProperty IRI=""ex:hasFather"" />
    <ObjectProperty IRI=""ex:hasBrother"" />
  </ObjectPropertyChain>
  <ObjectProperty IRI=""ex:hasUncle"" />
</SubObjectPropertyOf>");

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions.Count == 2);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[0]
                            is OWLObjectProperty firstChainProp && string.Equals(firstChainProp.IRI, "ex:hasFather"));
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[1]
                            is OWLObjectProperty secondChainProp && string.Equals(secondChainProp.IRI, "ex:hasBrother"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:hasUncle"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOfWithChainAndInverse()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = OWLTestSerializer<OWLSubObjectPropertyOf>.Deserialize(
@"<SubObjectPropertyOf>
  <ObjectPropertyChain>
    <ObjectProperty IRI=""ex:hasFather"" />
    <ObjectProperty IRI=""ex:hasBrother"" />
  </ObjectPropertyChain>
  <ObjectInverseOf>
    <ObjectProperty IRI=""ex:hasUncle"" />
  </ObjectInverseOf>
</SubObjectPropertyOf>");

            Assert.IsNotNull(subObjectPropertyOf);
            Assert.IsNull(subObjectPropertyOf.SubObjectPropertyExpression);
            Assert.IsNotNull(subObjectPropertyOf.SubObjectPropertyChain);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions.Count == 2);
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[0]
                            is OWLObjectProperty firstChainProp && string.Equals(firstChainProp.IRI, "ex:hasFather"));
            Assert.IsTrue(subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions[1]
                            is OWLObjectProperty secondChainProp && string.Equals(secondChainProp.IRI, "ex:hasBrother"));
            Assert.IsNotNull(subObjectPropertyOf.SuperObjectPropertyExpression);
            Assert.IsTrue(subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectInverseOf superObjInvOf
                            && string.Equals(superObjInvOf.ObjectProperty.IRI, "ex:hasUncle"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SubObjectPropertyOf>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""ex:objPropA"" />
    <ObjectProperty IRI=""ex:objPropB"" />
  </SubObjectPropertyOf>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSubObjectPropertyOf subObjPropOf
                            && subObjPropOf.SubObjectPropertyExpression is OWLObjectProperty subObjProp
                            && string.Equals(subObjProp.IRI, "ex:objPropA")
                            && subObjPropOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:objPropB"));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSubObjectPropertyOf subObjPropOf1
							&& string.Equals(subObjPropOf1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(subObjPropOf1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(subObjPropOf1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldDeserializeSubObjectPropertyOfWithChainViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SubObjectPropertyOf>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectPropertyChain>
      <ObjectProperty IRI=""ex:hasFather"" />
      <ObjectProperty IRI=""ex:hasBrother"" />
    </ObjectPropertyChain>
    <ObjectProperty IRI=""ex:hasUncle"" />
  </SubObjectPropertyOf>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSubObjectPropertyOf subObjPropOf
                            && subObjPropOf.SubObjectPropertyChain.ObjectPropertyExpressions[0] is OWLObjectProperty firstChainProp
                            && string.Equals(firstChainProp.IRI, "ex:hasFather")
                            && subObjPropOf.SubObjectPropertyChain.ObjectPropertyExpressions[1] is OWLObjectProperty secondChainProp
                            && string.Equals(secondChainProp.IRI, "ex:hasBrother")
                            && subObjPropOf.SuperObjectPropertyExpression is OWLObjectProperty superObjProp
                            && string.Equals(superObjProp.IRI, "ex:hasUncle"));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSubObjectPropertyOf subObjPropOf1
                            && string.Equals(subObjPropOf1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(subObjPropOf1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(subObjPropOf1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertSubObjectPropertyOfToGraph()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectProperty(new RDFResource("ex:objPropA")),
                new OWLObjectProperty(new RDFResource("ex:objPropB")));
            RDFGraph graph = subObjectPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[new RDFResource("ex:objPropA"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:objPropB"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertSubObjectPropertyOfWithInverseSubToGraph()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA"))),
                new OWLObjectProperty(new RDFResource("ex:objPropB")));
            RDFGraph graph = subObjectPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 4);
            Assert.IsTrue(graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:objPropB"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:objPropA"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertSubObjectPropertyOfWithInverseSuperToGraph()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectProperty(new RDFResource("ex:objPropA")),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))));
            RDFGraph graph = subObjectPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 4);
            Assert.IsTrue(graph[new RDFResource("ex:objPropA"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:objPropB"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertSubObjectPropertyOfWithBothInverseToGraph()
        {
            OWLSubObjectPropertyOf subObjectPropertyOf = new OWLSubObjectPropertyOf(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA"))),
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB"))));
            RDFGraph graph = subObjectPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 5);
            Assert.IsTrue(graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:objPropA"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:objPropB"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:objPropB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }
        #endregion
    }
}