/*
   Copyright 2014-2026 Marco De Salvo

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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLFunctionalLexerTest
{
    #region Tests (single tokens)
    [TestMethod]
    public void ShouldTokenizeEmptyDocument()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize(string.Empty);

        Assert.HasCount(1, tokens);
        Assert.AreEqual(OWLFunctionalTokenType.EndOfDocument, tokens[0].Type);
    }

    [TestMethod]
    public void ShouldTokenizeNullDocumentAsEmpty()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize(null);

        Assert.HasCount(1, tokens);
        Assert.AreEqual(OWLFunctionalTokenType.EndOfDocument, tokens[0].Type);
    }

    [TestMethod]
    public void ShouldTokenizeOpenAndCloseParenthesis()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("()");

        Assert.AreEqual(OWLFunctionalTokenType.OpenParenthesis, tokens[0].Type);
        Assert.AreEqual(OWLFunctionalTokenType.CloseParenthesis, tokens[1].Type);
    }

    [TestMethod]
    public void ShouldTokenizeEquals()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("=");

        Assert.AreEqual(OWLFunctionalTokenType.Equals, tokens[0].Type);
        Assert.AreEqual("=", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeFullIRI()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("<http://example.org/Pizza>");

        Assert.AreEqual(OWLFunctionalTokenType.FullIRI, tokens[0].Type);
        Assert.AreEqual("http://example.org/Pizza", tokens[0].Value);
        Assert.AreEqual(OWLFunctionalTokenType.EndOfDocument, tokens[1].Type);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnterminatedFullIRI()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalLexer.Tokenize("<http://example.org/Pizza"));

    [TestMethod]
    public void ShouldTokenizePrefixedName()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("pz:Pizza");

        Assert.AreEqual(OWLFunctionalTokenType.PrefixedName, tokens[0].Type);
        Assert.AreEqual("pz:Pizza", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeDefaultPrefixedName()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize(":Pizza");

        Assert.AreEqual(OWLFunctionalTokenType.PrefixedName, tokens[0].Type);
        Assert.AreEqual(":Pizza", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizePrefixNamespace()
    {
        //A named prefix immediately followed by ':' with nothing name-like after it is only legal
        //as the prefixName argument of a prefixDeclaration, e.g: "Prefix(owl:=<...>)"
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("Prefix(owl:=<http://www.w3.org/2002/07/owl#>)");

        Assert.AreEqual(OWLFunctionalTokenType.Name, tokens[0].Type);
        Assert.AreEqual("Prefix", tokens[0].Value);
        Assert.AreEqual(OWLFunctionalTokenType.OpenParenthesis, tokens[1].Type);
        Assert.AreEqual(OWLFunctionalTokenType.PrefixNamespace, tokens[2].Type);
        Assert.AreEqual("owl:", tokens[2].Value);
        Assert.AreEqual(OWLFunctionalTokenType.Equals, tokens[3].Type);
        Assert.AreEqual(OWLFunctionalTokenType.FullIRI, tokens[4].Type);
        Assert.AreEqual(OWLFunctionalTokenType.CloseParenthesis, tokens[5].Type);
    }

    [TestMethod]
    public void ShouldTokenizeDefaultPrefixNamespace()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("Prefix(:=<http://example.org/pz#>)");

        Assert.AreEqual(OWLFunctionalTokenType.PrefixNamespace, tokens[2].Type);
        Assert.AreEqual(":", tokens[2].Value);
    }

    [TestMethod]
    public void ShouldTokenizeNodeID()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("_:anon1");

        Assert.AreEqual(OWLFunctionalTokenType.NodeID, tokens[0].Type);
        Assert.AreEqual("anon1", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnEmptyNodeID()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalLexer.Tokenize("_: "));

    [TestMethod]
    public void ShouldTokenizeQuotedString()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("\"hello world\"");

        Assert.AreEqual(OWLFunctionalTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual("hello world", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeQuotedStringWithEscapes()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("\"say \\\"hi\\\" and \\\\bye\\\\\"");

        Assert.AreEqual(OWLFunctionalTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual("say \"hi\" and \\bye\\", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeEmptyQuotedString()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("\"\"");

        Assert.AreEqual(OWLFunctionalTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual(string.Empty, tokens[0].Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnterminatedQuotedString()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalLexer.Tokenize("\"never closed"));

    [TestMethod]
    public void ShouldTokenizeNonNegativeInteger()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("42");

        Assert.AreEqual(OWLFunctionalTokenType.NonNegativeInteger, tokens[0].Type);
        Assert.AreEqual("42", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeLanguageTag()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("@it");

        Assert.AreEqual(OWLFunctionalTokenType.LanguageTag, tokens[0].Type);
        Assert.AreEqual("it", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeLanguageTagWithRegionSubtag()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("@en-US");

        Assert.AreEqual(OWLFunctionalTokenType.LanguageTag, tokens[0].Type);
        Assert.AreEqual("en-US", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnEmptyLanguageTag()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalLexer.Tokenize("\"x\"@ "));

    [TestMethod]
    public void ShouldTokenizeDoubleCaret()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("^^");

        Assert.AreEqual(OWLFunctionalTokenType.DoubleCaret, tokens[0].Type);
        Assert.AreEqual("^^", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnLoneCaret()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalLexer.Tokenize("^"));

    [TestMethod]
    public void ShouldTokenizeName()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("SubClassOf");

        Assert.AreEqual(OWLFunctionalTokenType.Name, tokens[0].Type);
        Assert.AreEqual("SubClassOf", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldSkipWhitespaceAndComments()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("  # this is a comment\nPrefix");

        Assert.HasCount(2, tokens);
        Assert.AreEqual(OWLFunctionalTokenType.Name, tokens[0].Type);
        Assert.AreEqual("Prefix", tokens[0].Value);
        Assert.AreEqual(OWLFunctionalTokenType.EndOfDocument, tokens[1].Type);
    }

    [TestMethod]
    public void ShouldTokenizeCommentUntilEndOfLine()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("Class(:A) # this is a comment\nClass(:B)");

        Assert.AreEqual(OWLFunctionalTokenType.Name, tokens[0].Type);
        Assert.AreEqual(OWLFunctionalTokenType.OpenParenthesis, tokens[1].Type);
        Assert.AreEqual(OWLFunctionalTokenType.PrefixedName, tokens[2].Type);
        Assert.AreEqual(":A", tokens[2].Value);
        Assert.AreEqual(OWLFunctionalTokenType.CloseParenthesis, tokens[3].Type);
        Assert.AreEqual(OWLFunctionalTokenType.Name, tokens[4].Type);
        Assert.AreEqual(OWLFunctionalTokenType.OpenParenthesis, tokens[5].Type);
        Assert.AreEqual(OWLFunctionalTokenType.PrefixedName, tokens[6].Type);
        Assert.AreEqual(":B", tokens[6].Value);
    }

    [TestMethod]
    public void ShouldSkipWhitespaceAndTrackLinesAndColumns()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("Ontology(\n    Class(:A))");

        Assert.AreEqual(1, tokens[0].Line);
        Assert.AreEqual(1, tokens[0].Column);
        Assert.AreEqual(1, tokens[1].Line);
        Assert.AreEqual(9, tokens[1].Column);
        Assert.AreEqual(2, tokens[2].Line);
        Assert.AreEqual(5, tokens[2].Column);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnexpectedCharacter()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalLexer.Tokenize("%"));
    #endregion

    #region Tests (token sequences)
    [TestMethod]
    public void ShouldTokenizeTypedLiteral()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("\"42\"^^xsd:integer");

        Assert.AreEqual(OWLFunctionalTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual(OWLFunctionalTokenType.DoubleCaret, tokens[1].Type);
        Assert.AreEqual(OWLFunctionalTokenType.PrefixedName, tokens[2].Type);
        Assert.AreEqual("xsd:integer", tokens[2].Value);
    }

    [TestMethod]
    public void ShouldTokenizeLanguageTaggedLiteral()
    {
        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize("\"Pizza\"@it");

        Assert.AreEqual(OWLFunctionalTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual(OWLFunctionalTokenType.LanguageTag, tokens[1].Type);
        Assert.AreEqual("it", tokens[1].Value);
    }

    [TestMethod]
    public void ShouldTokenizeRealisticMultiLineDocument()
    {
        const string document =
            "Prefix(owl:=<http://www.w3.org/2002/07/owl#>)\n"
            + "Prefix(:=<http://example.org/pizza#>)\n"
            + "Ontology(<http://example.org/pizza>\n"
            + "    Declaration(Class(:Pizza))\n"
            + "    SubClassOf(:Margherita :Pizza)\n"
            + ")\n";

        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize(document);
        List<OWLFunctionalTokenType> types = tokens.Select(tok => tok.Type).ToList();

        CollectionAssert.AreEqual(new List<OWLFunctionalTokenType>
        {
            //Prefix(owl:=<http://www.w3.org/2002/07/owl#>)
            OWLFunctionalTokenType.Name, OWLFunctionalTokenType.OpenParenthesis, OWLFunctionalTokenType.PrefixNamespace,
            OWLFunctionalTokenType.Equals, OWLFunctionalTokenType.FullIRI, OWLFunctionalTokenType.CloseParenthesis,
            //Prefix(:=<http://example.org/pizza#>)
            OWLFunctionalTokenType.Name, OWLFunctionalTokenType.OpenParenthesis, OWLFunctionalTokenType.PrefixNamespace,
            OWLFunctionalTokenType.Equals, OWLFunctionalTokenType.FullIRI, OWLFunctionalTokenType.CloseParenthesis,
            //Ontology(<http://example.org/pizza>
            OWLFunctionalTokenType.Name, OWLFunctionalTokenType.OpenParenthesis, OWLFunctionalTokenType.FullIRI,
            //Declaration(Class(:Pizza))
            OWLFunctionalTokenType.Name, OWLFunctionalTokenType.OpenParenthesis, OWLFunctionalTokenType.Name,
            OWLFunctionalTokenType.OpenParenthesis, OWLFunctionalTokenType.PrefixedName, OWLFunctionalTokenType.CloseParenthesis,
            OWLFunctionalTokenType.CloseParenthesis,
            //SubClassOf(:Margherita :Pizza)
            OWLFunctionalTokenType.Name, OWLFunctionalTokenType.OpenParenthesis, OWLFunctionalTokenType.PrefixedName,
            OWLFunctionalTokenType.PrefixedName, OWLFunctionalTokenType.CloseParenthesis,
            //)
            OWLFunctionalTokenType.CloseParenthesis,
            OWLFunctionalTokenType.EndOfDocument
        }, types);

        Assert.AreEqual("owl:", tokens[2].Value);
        Assert.AreEqual("http://www.w3.org/2002/07/owl#", tokens[4].Value);
        Assert.AreEqual(":", tokens[8].Value);
        Assert.AreEqual("http://example.org/pizza#", tokens[10].Value);
        Assert.AreEqual("http://example.org/pizza", tokens[14].Value);
        Assert.AreEqual(":Pizza", tokens[19].Value);
        Assert.AreEqual(":Margherita", tokens[24].Value);
        Assert.AreEqual(":Pizza", tokens[25].Value);
    }

    [TestMethod]
    public void ShouldProduceEveryTokenTypeAcrossTheGrammar()
    {
        //Guards against future OWLFunctionalTokenType values slipping in without lexer/test coverage:
        //this single document is built to exercise every member of the enum at least once
        const string document =
            "Prefix(owl:=<http://www.w3.org/2002/07/owl#>)\n"
            + "Ontology(<http://example.org/pz>\n"
            + "    Declaration(Class(:Pizza))\n"
            + "    AnnotationAssertion(rdfs:label :Pizza \"Pizza\"@it)\n"
            + "    DataPropertyAssertion(:hasCalories :Margherita \"850\"^^xsd:integer)\n"
            + "    ObjectMinCardinality(1 :hasTopping :Topping)\n"
            + "    SameIndividual(_:anon1 :Margherita)\n"
            + ")\n"
            + "# a trailing comment\n";

        List<OWLFunctionalToken> tokens = OWLFunctionalLexer.Tokenize(document);
        HashSet<OWLFunctionalTokenType> observedTypes = tokens.Select(tok => tok.Type).ToHashSet();

        foreach (OWLFunctionalTokenType tokenType in System.Enum.GetValues<OWLFunctionalTokenType>())
            Assert.IsTrue(observedTypes.Contains(tokenType), $"Token type {tokenType} was never produced by the sample document");
    }
    #endregion
}
