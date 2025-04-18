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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLLiteralTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreatePlainLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFPlainLiteral("hello"));

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, "hello"));
        Assert.IsNull(lit.Language);
        Assert.IsNull(lit.DatatypeIRI);
    }

    [TestMethod]
    public void ShouldCreateEmptyPlainLiteral()
    {
        OWLLiteral lit = new OWLLiteral(null);

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, string.Empty));
        Assert.IsNull(lit.Language);
        Assert.IsNull(lit.DatatypeIRI);
    }

    [TestMethod]
    public void ShouldCreateLanguageLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFPlainLiteral("hello", "en-US--rtl"));

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, "hello"));
        Assert.IsTrue(string.Equals(lit.Language, "EN-US--RTL"));
        Assert.IsNull(lit.DatatypeIRI);
    }

    [TestMethod]
    public void ShouldCreateTypedLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING));

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, "hello"));
        Assert.IsNull(lit.Language);
        Assert.IsTrue(string.Equals(lit.DatatypeIRI, RDFVocabulary.XSD.STRING.ToString()));
    }

    [TestMethod]
    public void ShouldSerializePlainLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFPlainLiteral("hello"));
        string serializedXML = OWLSerializer.SerializeObject(lit);

        Assert.IsTrue(string.Equals(serializedXML, "<Literal>hello</Literal>"));
        Assert.IsTrue(string.Equals(lit.ToSWRLString(), "\"hello\""));
    }

    [TestMethod]
    public void ShouldSerializeEmptyPlainLiteral()
    {
        OWLLiteral lit = new OWLLiteral(null);
        string serializedXML = OWLSerializer.SerializeObject(lit);

        Assert.IsTrue(string.Equals(serializedXML, "<Literal></Literal>"));
        Assert.IsTrue(string.Equals(lit.ToSWRLString(), "\"\""));
    }

    [TestMethod]
    public void ShouldSerializeLanguageLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFPlainLiteral("hello","en-US--RTL"));
        string serializedXML = OWLSerializer.SerializeObject(lit);

        Assert.IsTrue(string.Equals(serializedXML, """<Literal xml:lang="EN-US--RTL">hello</Literal>"""));
        Assert.IsTrue(string.Equals(lit.ToSWRLString(), "\"hello\"@EN-US--RTL"));
    }

    [TestMethod]
    public void ShouldSerializeTypedLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING));
        string serializedXML = OWLSerializer.SerializeObject(lit);

        Assert.IsTrue(string.Equals(serializedXML, """<Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#string">hello</Literal>"""));
        Assert.IsTrue(string.Equals(lit.ToSWRLString(), "\"hello\"^^xsd:string"));
    }

    [TestMethod]
    public void ShouldDeserializePlainLiteral()
    {
        OWLLiteral lit = OWLSerializer.DeserializeObject<OWLLiteral>("<Literal>hello</Literal>");

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, "hello"));
        Assert.IsNull(lit.Language);
        Assert.IsNull(lit.DatatypeIRI);
    }

    [TestMethod]
    public void ShouldDeserializeEmptyLiteral()
    {
        OWLLiteral lit = OWLSerializer.DeserializeObject<OWLLiteral>("<Literal></Literal>");

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, string.Empty));
        Assert.IsNull(lit.Language);
        Assert.IsNull(lit.DatatypeIRI);
    }

    [TestMethod]
    public void ShouldDeserializeLanguageLiteral()
    {
        OWLLiteral lit = OWLSerializer.DeserializeObject<OWLLiteral>("""<Literal xml:lang="en-US--rtl">hello</Literal>""");

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, "hello"));
        Assert.IsTrue(string.Equals(lit.Language, "en-US--rtl"));
        Assert.IsNull(lit.DatatypeIRI);
    }

    [TestMethod]
    public void ShouldDeserializeTypedLiteral()
    {
        OWLLiteral lit = OWLSerializer.DeserializeObject<OWLLiteral>("""<Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#string">hello</Literal>""");

        Assert.IsNotNull(lit);
        Assert.IsTrue(string.Equals(lit.Value, "hello"));
        Assert.IsNull(lit.Language);
        Assert.IsTrue(string.Equals(lit.DatatypeIRI, RDFVocabulary.XSD.STRING.ToString()));
    }

    [TestMethod]
    public void ShouldConvertLiteralToGraph()
    {
        OWLLiteral lit = new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING));
        RDFGraph graph = lit.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(0, graph.TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertLiteralToResource()
    {
        OWLLiteral lit = new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING));
        RDFResource representative = lit.GetIRI();

        Assert.IsNotNull(representative);
        Assert.IsTrue(representative.IsBlank);
    }

    [TestMethod]
    public void ShouldConvertLiteralToPlainLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFPlainLiteral("hello"));
        RDFLiteral rdfLit = lit.GetLiteral();

        Assert.IsNotNull(rdfLit);
        Assert.IsTrue(rdfLit is RDFPlainLiteral rdfLitPL
                      && string.Equals(rdfLitPL.ToString(), "hello"));
    }

    [TestMethod]
    public void ShouldConvertLiteralToPlainLiteralWithLanguage()
    {
        OWLLiteral lit = new OWLLiteral(new RDFPlainLiteral("hello", "en-US--ltr"));
        RDFLiteral rdfLit = lit.GetLiteral();

        Assert.IsNotNull(rdfLit);
        Assert.IsTrue(rdfLit is RDFPlainLiteral rdfLitPL
                      && string.Equals(rdfLitPL.ToString(), "hello@EN-US--LTR"));
    }

    [TestMethod]
    public void ShouldConvertLiteralToTypedLiteral()
    {
        OWLLiteral lit = new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING));
        RDFLiteral rdfLit = lit.GetLiteral();

        Assert.IsNotNull(rdfLit);
        Assert.IsTrue(rdfLit is RDFTypedLiteral rdfLitTL
                      && string.Equals(rdfLitTL.ToString(), "hello^^http://www.w3.org/2001/XMLSchema#string"));
    }
    #endregion
}