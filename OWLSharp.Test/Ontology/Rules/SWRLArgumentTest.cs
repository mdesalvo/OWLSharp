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
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class SWRLArgumentTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateSWRLIndividualArgument()
    {
        SWRLIndividualArgument arg = new SWRLIndividualArgument(new RDFResource("ex:Mark"));

        Assert.IsNotNull(arg);
        Assert.IsTrue(string.Equals("ex:Mark", arg.IRI));
        Assert.IsTrue(string.Equals("ex:Mark", arg.ToString()));
        Assert.IsTrue(arg.GetResource().Equals(new RDFResource("ex:Mark")));
        Assert.IsTrue(string.Equals("<NamedIndividual IRI=\"ex:Mark\" />", OWLSerializer.SerializeObject(arg)));
        Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLIndividualArgument(null));
        Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLIndividualArgument(new RDFResource()));
        Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLIndividualArgument(new RDFResource("bnode:jd8d7f")));
    }

    [TestMethod]
    public void ShouldCreateSWRLLiteralArgument()
    {
        SWRLLiteralArgument argPL = new SWRLLiteralArgument(new RDFPlainLiteral("hello"));

        Assert.IsNotNull(argPL);
        Assert.IsTrue(string.Equals("hello", argPL.Value));
        Assert.IsNull(argPL.Language);
        Assert.IsNull(argPL.DatatypeIRI);
        Assert.IsTrue(string.Equals("hello", argPL.ToString()));
        Assert.IsTrue(argPL.GetLiteral().Equals(new RDFPlainLiteral("hello")));
        Assert.IsTrue(string.Equals("<Literal>hello</Literal>", OWLSerializer.SerializeObject(argPL)));

        SWRLLiteralArgument argPLL = new SWRLLiteralArgument(new RDFPlainLiteral("hello", "en"));

        Assert.IsNotNull(argPLL);
        Assert.IsTrue(string.Equals("hello", argPLL.Value));
        Assert.IsTrue(string.Equals("EN", argPLL.Language));
        Assert.IsNull(argPLL.DatatypeIRI);
        Assert.IsTrue(string.Equals("hello@EN", argPLL.ToString()));
        Assert.IsTrue(argPLL.GetLiteral().Equals(new RDFPlainLiteral("hello", "en")));
        Assert.IsTrue(string.Equals("<Literal xml:lang=\"EN\">hello</Literal>", OWLSerializer.SerializeObject(argPLL)));

        SWRLLiteralArgument argPLT = new SWRLLiteralArgument(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING));

        Assert.IsNotNull(argPLT);
        Assert.IsTrue(string.Equals("hello", argPLT.Value));
        Assert.IsNull(argPLT.Language);
        Assert.IsTrue(string.Equals("http://www.w3.org/2001/XMLSchema#string", argPLT.DatatypeIRI));
        Assert.IsTrue(string.Equals("hello^^http://www.w3.org/2001/XMLSchema#string", argPLT.ToString()));
        Assert.IsTrue(argPLT.GetLiteral().Equals(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        Assert.IsTrue(string.Equals("<Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#string\">hello</Literal>", OWLSerializer.SerializeObject(argPLT)));
    }

    [TestMethod]
    public void ShouldCreateSWRLVariableArgument()
    {
        SWRLVariableArgument arg = new SWRLVariableArgument(new RDFVariable("?V"));

        Assert.IsNotNull(arg);
        Assert.IsTrue(string.Equals("urn:swrl:var#V", arg.IRI));
        Assert.IsTrue(string.Equals("?V", arg.ToString()));
        Assert.IsTrue(arg.GetVariable().Equals(new RDFVariable("?V")));
        Assert.IsTrue(string.Equals("<Variable IRI=\"urn:swrl:var#V\" />", OWLSerializer.SerializeObject(arg)));
        Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLVariableArgument(null));
    }
    #endregion
}