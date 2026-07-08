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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLManchesterLexerTest
{
    #region Tests (single tokens)
    [TestMethod]
    public void ShouldTokenizeEmptyDocument()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize(string.Empty);

        Assert.HasCount(1, tokens);
        Assert.AreEqual(OWLManchesterTokenType.EndOfDocument, tokens[0].Type);
    }

    [TestMethod]
    public void ShouldTokenizeNullDocumentAsEmpty()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize(null);

        Assert.HasCount(1, tokens);
        Assert.AreEqual(OWLManchesterTokenType.EndOfDocument, tokens[0].Type);
    }

    [TestMethod]
    public void ShouldTokenizeFullIRI()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("<http://example.org/Pizza>");

        Assert.AreEqual(OWLManchesterTokenType.FullIRI, tokens[0].Type);
        Assert.AreEqual("http://example.org/Pizza", tokens[0].Value);
        Assert.AreEqual(OWLManchesterTokenType.EndOfDocument, tokens[1].Type);
    }

    [TestMethod]
    public void ShouldTokenizePrefixedName()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("pz:Pizza");

        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[0].Type);
        Assert.AreEqual("pz:Pizza", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizePrefixedNameWithHyphenAndDotInLocalPart()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("pz:my-class.v2");

        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[0].Type);
        Assert.AreEqual("pz:my-class.v2", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeEmptyPrefixName()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize(":Pizza");

        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[0].Type);
        Assert.AreEqual(":Pizza", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeBareColonAsSectionKeyword()
    {
        //A colon not followed by a name-start character is not a valid empty-prefix reference:
        //this only occurs as part of a "word:" section keyword scan, never standalone in valid documents,
        //but the lexer must still emit something deterministic rather than looping
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize(": ");

        Assert.AreEqual(OWLManchesterTokenType.SectionKeyword, tokens[0].Type);
        Assert.AreEqual(":", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeNodeID()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("_:anon1");

        Assert.AreEqual(OWLManchesterTokenType.NodeID, tokens[0].Type);
        Assert.AreEqual("anon1", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnEmptyNodeID()
        => Assert.ThrowsExactly<OWLException>(() => OWLManchesterLexer.Tokenize("_: "));

    [TestMethod]
    public void ShouldTokenizeSectionKeyword()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("SubClassOf:");

        Assert.AreEqual(OWLManchesterTokenType.SectionKeyword, tokens[0].Type);
        Assert.AreEqual("SubClassOf:", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeBareName()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("some");

        Assert.AreEqual(OWLManchesterTokenType.Name, tokens[0].Type);
        Assert.AreEqual("some", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeSingleCharacterName()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("o");

        Assert.AreEqual(OWLManchesterTokenType.Name, tokens[0].Type);
        Assert.AreEqual("o", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeQuotedString()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("\"hello world\"");

        Assert.AreEqual(OWLManchesterTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual("hello world", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeQuotedStringWithEscapes()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("\"say \\\"hi\\\" and \\\\bye\\\\\"");

        Assert.AreEqual(OWLManchesterTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual("say \"hi\" and \\bye\\", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeEmptyQuotedString()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("\"\"");

        Assert.AreEqual(OWLManchesterTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual(string.Empty, tokens[0].Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnterminatedQuotedString()
        => Assert.ThrowsExactly<OWLException>(() => OWLManchesterLexer.Tokenize("\"never closed"));

    [TestMethod]
    public void ShouldTokenizePositiveInteger()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("42");

        Assert.AreEqual(OWLManchesterTokenType.IntegerNumber, tokens[0].Type);
        Assert.AreEqual("42", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeNegativeInteger()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("-42");

        Assert.AreEqual(OWLManchesterTokenType.IntegerNumber, tokens[0].Type);
        Assert.AreEqual("-42", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizePositiveSignedInteger()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("+42");

        Assert.AreEqual(OWLManchesterTokenType.IntegerNumber, tokens[0].Type);
        Assert.AreEqual("+42", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeDecimalNumber()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("3.14");

        Assert.AreEqual(OWLManchesterTokenType.DecimalNumber, tokens[0].Type);
        Assert.AreEqual("3.14", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeFloatNumberWithExponent()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("1.5e10");

        Assert.AreEqual(OWLManchesterTokenType.FloatNumber, tokens[0].Type);
        Assert.AreEqual("1.5e10", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeFloatNumberWithNegativeExponent()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("1.5E-10");

        Assert.AreEqual(OWLManchesterTokenType.FloatNumber, tokens[0].Type);
        Assert.AreEqual("1.5E-10", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeFloatNumberWithSuffix()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("2.5f");

        Assert.AreEqual(OWLManchesterTokenType.FloatNumber, tokens[0].Type);
    }

    [TestMethod]
    public void ShouldTokenizeIntegerWithoutConsumingATrailingDotNotFollowedByDigits()
    {
        //"42." is not a legal decimal (no digit after the dot): the dot must not be swallowed into the
        //number token. A standalone '.' is never a valid Manchester token on its own, so the lexer
        //correctly rejects it once it is left over as the next character
        Assert.ThrowsExactly<OWLException>(() => OWLManchesterLexer.Tokenize("42."));

        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("42");
        Assert.AreEqual(OWLManchesterTokenType.IntegerNumber, tokens[0].Type);
        Assert.AreEqual("42", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldTokenizeLanguageTag()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("@en-US");

        Assert.AreEqual(OWLManchesterTokenType.LanguageTag, tokens[0].Type);
        Assert.AreEqual("en-US", tokens[0].Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnEmptyLanguageTag()
        => Assert.ThrowsExactly<OWLException>(() => OWLManchesterLexer.Tokenize("\"x\"@ "));

    [TestMethod]
    public void ShouldTokenizeDoubleCaret()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("^^");

        Assert.AreEqual(OWLManchesterTokenType.DoubleCaret, tokens[0].Type);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnLoneCaret()
        => Assert.ThrowsExactly<OWLException>(() => OWLManchesterLexer.Tokenize("^"));

    [TestMethod]
    public void ShouldTokenizePunctuation()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize(",(){}[]");

        Assert.AreEqual(OWLManchesterTokenType.Comma, tokens[0].Type);
        Assert.AreEqual(OWLManchesterTokenType.OpenParenthesis, tokens[1].Type);
        Assert.AreEqual(OWLManchesterTokenType.CloseParenthesis, tokens[2].Type);
        Assert.AreEqual(OWLManchesterTokenType.OpenBrace, tokens[3].Type);
        Assert.AreEqual(OWLManchesterTokenType.CloseBrace, tokens[4].Type);
        Assert.AreEqual(OWLManchesterTokenType.OpenBracket, tokens[5].Type);
        Assert.AreEqual(OWLManchesterTokenType.CloseBracket, tokens[6].Type);
    }

    [TestMethod]
    public void ShouldTokenizeFacetComparators()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("<= >= < >");

        Assert.AreEqual(OWLManchesterTokenType.LessOrEqual, tokens[0].Type);
        Assert.AreEqual(OWLManchesterTokenType.GreaterOrEqual, tokens[1].Type);
        Assert.AreEqual(OWLManchesterTokenType.LessThan, tokens[2].Type);
        Assert.AreEqual(OWLManchesterTokenType.GreaterThan, tokens[3].Type);
    }

    [TestMethod]
    public void ShouldDisambiguateFullIRIFromLessThan()
    {
        //A '<' immediately closed by '>' is a full IRI; a '<' not closed before whitespace/another
        //delimiter is the "less than" facet comparator
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("<http://x.org/y> < 5");

        Assert.AreEqual(OWLManchesterTokenType.FullIRI, tokens[0].Type);
        Assert.AreEqual(OWLManchesterTokenType.LessThan, tokens[1].Type);
        Assert.AreEqual(OWLManchesterTokenType.IntegerNumber, tokens[2].Type);
    }

    [TestMethod]
    public void ShouldTokenizeCommentUntilEndOfLine()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("Class: pz:A # this is a comment\nClass: pz:B");

        Assert.AreEqual(OWLManchesterTokenType.SectionKeyword, tokens[0].Type);
        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[1].Type);
        Assert.AreEqual("pz:A", tokens[1].Value);
        Assert.AreEqual(OWLManchesterTokenType.SectionKeyword, tokens[2].Type);
        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[3].Type);
        Assert.AreEqual("pz:B", tokens[3].Value);
    }

    [TestMethod]
    public void ShouldSkipWhitespaceAndTrackLinesAndColumns()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("Class:\n    pz:A");

        Assert.AreEqual(1, tokens[0].Line);
        Assert.AreEqual(1, tokens[0].Column);
        Assert.AreEqual(2, tokens[1].Line);
        Assert.AreEqual(5, tokens[1].Column);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnexpectedCharacter()
        => Assert.ThrowsExactly<OWLException>(() => OWLManchesterLexer.Tokenize("$"));
    #endregion

    #region Tests (token sequences)
    [TestMethod]
    public void ShouldTokenizeTypedLiteral()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("\"42\"^^xsd:integer");

        Assert.AreEqual(OWLManchesterTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual(OWLManchesterTokenType.DoubleCaret, tokens[1].Type);
        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[2].Type);
        Assert.AreEqual("xsd:integer", tokens[2].Value);
    }

    [TestMethod]
    public void ShouldTokenizeLanguageTaggedLiteral()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("\"Pizza\"@it");

        Assert.AreEqual(OWLManchesterTokenType.QuotedString, tokens[0].Type);
        Assert.AreEqual(OWLManchesterTokenType.LanguageTag, tokens[1].Type);
        Assert.AreEqual("it", tokens[1].Value);
    }

    [TestMethod]
    public void ShouldTokenizeFullClassFrameHeader()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("Class: pz:Pizza");

        Assert.AreEqual(OWLManchesterTokenType.SectionKeyword, tokens[0].Type);
        Assert.AreEqual("Class:", tokens[0].Value);
        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[1].Type);
        Assert.AreEqual("pz:Pizza", tokens[1].Value);
        Assert.AreEqual(OWLManchesterTokenType.EndOfDocument, tokens[2].Type);
    }

    [TestMethod]
    public void ShouldTokenizeDatatypeRestrictionWithFacets()
    {
        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize("xsd:integer[>= \"0\"^^xsd:integer, < \"18\"^^xsd:integer]");

        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[0].Type);
        Assert.AreEqual(OWLManchesterTokenType.OpenBracket, tokens[1].Type);
        Assert.AreEqual(OWLManchesterTokenType.GreaterOrEqual, tokens[2].Type);
        Assert.AreEqual(OWLManchesterTokenType.QuotedString, tokens[3].Type);
        Assert.AreEqual(OWLManchesterTokenType.DoubleCaret, tokens[4].Type);
        Assert.AreEqual(OWLManchesterTokenType.PrefixedName, tokens[5].Type);
        Assert.AreEqual(OWLManchesterTokenType.Comma, tokens[6].Type);
        Assert.AreEqual(OWLManchesterTokenType.LessThan, tokens[7].Type);
        Assert.AreEqual(OWLManchesterTokenType.CloseBracket, tokens[11].Type);
    }

    [TestMethod]
    public void ShouldProduceEveryTokenTypeAcrossTheGrammar()
    {
        //Guards against future OWLManchesterTokenType values slipping in without lexer/test coverage:
        //this single document is built to exercise every member of the enum at least once
        const string document =
            "Prefix: pz: <http://example.org/pz#>\n"
            + "Ontology: <http://example.org/pz>\n"
            + "Class: pz:Pizza\n"
            + "    SubClassOf: pz:hasTopping some pz:Topping, pz:hasAge >= 5, pz:hasAge <= 10, pz:hasAge > 1, pz:hasAge < 99\n"
            + "    SubClassOf: pz:hasWeight 3.14, pz:hasFactor 1.5e3\n"
            + "    SubClassOf: (pz:A and pz:B)\n"
            + "    HasKey: _:anon1\n"
            + "Individual: pz:Margherita\n"
            + "    Facts: pz:hasCalories \"850\"^^xsd:integer, pz:hasName \"Pizza\"@it, pz:hasNote \"free text\"\n"
            + "    Types: {pz:A, pz:B}[pz:C]\n"
            + "# a trailing comment\n";

        List<OWLManchesterToken> tokens = OWLManchesterLexer.Tokenize(document);
        HashSet<OWLManchesterTokenType> observedTypes = tokens.Select(tok => tok.Type).ToHashSet();

        foreach (OWLManchesterTokenType tokenType in Enum.GetValues<OWLManchesterTokenType>())
            Assert.IsTrue(observedTypes.Contains(tokenType), $"Token type {tokenType} was never produced by the sample document");
    }
    #endregion
}
