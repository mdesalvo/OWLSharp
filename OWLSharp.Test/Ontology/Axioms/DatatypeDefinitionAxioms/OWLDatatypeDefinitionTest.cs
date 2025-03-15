/*
   Copyright 2014-2025 Marco De Salvo

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
using OWLSharp.Ontology;
using RDFSharp.Model;


namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLDatatypeDefinitionTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateDatatypeDefinition()
    {
        OWLDatatypeDefinition length6to10DT = new OWLDatatypeDefinition(
            new OWLDatatype(new RDFResource("ex:length6to10")),
            new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                    new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]));

        Assert.IsNotNull(length6to10DT);
        Assert.IsNotNull(length6to10DT.Datatype);
        Assert.IsTrue(string.Equals(length6to10DT.Datatype.IRI, "ex:length6to10"));
        Assert.IsNotNull(length6to10DT.DataRangeExpression);
        Assert.IsTrue(length6to10DT.DataRangeExpression is OWLDatatypeRestriction dtRestr
                      && string.Equals(dtRestr.Datatype.IRI, RDFVocabulary.XSD.STRING.ToString())
                      && dtRestr.FacetRestrictions.Count == 2
                      && dtRestr.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, RDFVocabulary.XSD.MIN_LENGTH.ToString())
                                                             && string.Equals(fr.Literal.Value, "6")
                                                             && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString()))
                      && dtRestr.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, RDFVocabulary.XSD.MAX_LENGTH.ToString())
                                                             && string.Equals(fr.Literal.Value, "10")
                                                             && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString())));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDatatypeDefinitionBecauseNullDatatype()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDatatypeDefinition(
            null,
            new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                    new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)])));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDatatypeDefinitionBecauseNullDataRangeExpression()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDatatypeDefinition(
            new OWLDatatype(new RDFResource("ex:length6to10")), null));

    [TestMethod]
    public void ShouldSerializeDatatypeDefinition()
    {
        OWLDatatypeDefinition length6to10DT = new OWLDatatypeDefinition(
            new OWLDatatype(new RDFResource("ex:length6to10")),
            new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                    new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]));

        string serializedXML = OWLSerializer.SerializeObject(length6to10DT);

        Assert.IsTrue(string.Equals(serializedXML,
            """<DatatypeDefinition><Datatype IRI="ex:length6to10" /><DatatypeRestriction><Datatype IRI="http://www.w3.org/2001/XMLSchema#string" /><FacetRestriction facet="http://www.w3.org/2001/XMLSchema#minLength"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">6</Literal></FacetRestriction><FacetRestriction facet="http://www.w3.org/2001/XMLSchema#maxLength"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">10</Literal></FacetRestriction></DatatypeRestriction></DatatypeDefinition>"""));
    }

    [TestMethod]
    public void ShouldSerializeDatatypeDefinitionViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.DatatypeDefinitionAxioms.Add(
            new OWLDatatypeDefinition(
                new OWLDatatype(new RDFResource("ex:length6to10")),
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)])));

        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><DatatypeDefinition><Datatype IRI="ex:length6to10" /><DatatypeRestriction><Datatype IRI="http://www.w3.org/2001/XMLSchema#string" /><FacetRestriction facet="http://www.w3.org/2001/XMLSchema#minLength"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">6</Literal></FacetRestriction><FacetRestriction facet="http://www.w3.org/2001/XMLSchema#maxLength"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">10</Literal></FacetRestriction></DatatypeRestriction></DatatypeDefinition></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeDatatypeDefinition()
    {
        OWLDatatypeDefinition length6to10DT = OWLSerializer.DeserializeObject<OWLDatatypeDefinition>(
            """
            <DatatypeDefinition>
              <Datatype IRI="ex:length6to10" />
              <DatatypeRestriction>
                <Datatype IRI="http://www.w3.org/2001/XMLSchema#string" />
                <FacetRestriction facet="http://www.w3.org/2001/XMLSchema#minLength">
                  <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">6</Literal>
                </FacetRestriction>
                <FacetRestriction facet="http://www.w3.org/2001/XMLSchema#maxLength">
                  <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">10</Literal>
                </FacetRestriction>
              </DatatypeRestriction>
            </DatatypeDefinition>
            """);

        Assert.IsNotNull(length6to10DT);
        Assert.IsNotNull(length6to10DT.Datatype);
        Assert.IsTrue(string.Equals(length6to10DT.Datatype.IRI, "ex:length6to10"));
        Assert.IsNotNull(length6to10DT.DataRangeExpression);
        Assert.IsTrue(length6to10DT.DataRangeExpression is OWLDatatypeRestriction dtRestr
                      && string.Equals(dtRestr.Datatype.IRI, RDFVocabulary.XSD.STRING.ToString())
                      && dtRestr.FacetRestrictions.Count == 2
                      && dtRestr.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, RDFVocabulary.XSD.MIN_LENGTH.ToString())
                                                             && string.Equals(fr.Literal.Value, "6")
                                                             && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString()))
                      && dtRestr.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, RDFVocabulary.XSD.MAX_LENGTH.ToString())
                                                             && string.Equals(fr.Literal.Value, "10")
                                                             && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString())));
    }

    [TestMethod]
    public void ShouldDeserializeDatatypeDefinitionViaOntology()
    {
        OWLOntology ontology = OWLSerializer.DeserializeOntology(
            """
            <Ontology>
              <Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" />
              <Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" />
              <Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" />
              <Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" />
              <Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" />
              <DatatypeDefinition>
                <Annotation>
                  <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                  <Literal xml:lang="EN">Steve</Literal>
                </Annotation>
                <Datatype IRI="ex:length6to10" />
                <DatatypeRestriction>
                  <Datatype IRI="http://www.w3.org/2001/XMLSchema#string" />
                  <FacetRestriction facet="http://www.w3.org/2001/XMLSchema#minLength">
                    <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">6</Literal>
                  </FacetRestriction>
                  <FacetRestriction facet="http://www.w3.org/2001/XMLSchema#maxLength">
                    <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#int">10</Literal>
                  </FacetRestriction>
                </DatatypeRestriction>
              </DatatypeDefinition>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.DatatypeDefinitionAxioms.Count);
        Assert.IsTrue(ontology.DatatypeDefinitionAxioms.Single() is { } dtDef
                      && string.Equals(dtDef.Datatype.IRI, "ex:length6to10")
                      && dtDef.DataRangeExpression is OWLDatatypeRestriction dtRestr
                      && string.Equals(dtRestr.Datatype.IRI, RDFVocabulary.XSD.STRING.ToString())
                      && dtRestr.FacetRestrictions.Count == 2
                      && dtRestr.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, RDFVocabulary.XSD.MIN_LENGTH.ToString())
                                                             && string.Equals(fr.Literal.Value, "6")
                                                             && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString()))
                      && dtRestr.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, RDFVocabulary.XSD.MAX_LENGTH.ToString())
                                                             && string.Equals(fr.Literal.Value, "10")
                                                             && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString())));
        Assert.IsTrue(ontology.DatatypeDefinitionAxioms.Single() is { } dtDef1
                      && string.Equals(dtDef1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                      && string.Equals(dtDef1.Annotations.Single().ValueLiteral.Value, "Steve")
                      && string.Equals(dtDef1.Annotations.Single().ValueLiteral.Language, "EN"));

    }

    [TestMethod]
    public void ShouldConvertDatatypeDefinitionToGraph()
    {
        OWLDatatypeDefinition length6to10DT = new OWLDatatypeDefinition(
            new OWLDatatype(new RDFResource("ex:length6to10")),
            new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                    new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]));
        RDFGraph graph = length6to10DT.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(14, graph.TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_DATATYPE, RDFVocabulary.XSD.STRING, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.WITH_RESTRICTIONS, null, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.FIRST, null, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.XSD.MIN_LENGTH, null, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.XSD.MAX_LENGTH, null, new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertDatatypeDefinitionWithAnnotationToGraph()
    {
        OWLDatatypeDefinition length6to10DT = new OWLDatatypeDefinition(
            new OWLDatatype(new RDFResource("ex:length6to10")),
            new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                    new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = length6to10DT.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(20, graph.TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_DATATYPE, RDFVocabulary.XSD.STRING, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.WITH_RESTRICTIONS, null, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.FIRST, null, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.XSD.MIN_LENGTH, null, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.XSD.MAX_LENGTH, null, new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:length6to10"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}