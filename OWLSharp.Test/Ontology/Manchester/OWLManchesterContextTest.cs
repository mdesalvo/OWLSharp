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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Manchester;

[TestClass]
public class OWLManchesterContextTest
{
    #region Tests (ctor)
    [TestMethod]
    public void ShouldCreateContext()
    {
        OWLManchesterContext context = new OWLManchesterContext(new List<OWLPrefix>());

        Assert.IsNotNull(context);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingContextBecauseNullPrefixes()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLManchesterContext(null));
    #endregion

    #region Tests (Abbreviate)
    [TestMethod]
    public void ShouldAbbreviateIRIWithMatchingPrefix()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("pz:Pizza", context.Abbreviate(new RDFResource("http://example.org/pz#Pizza")));
    }

    [TestMethod]
    public void ShouldAbbreviateWithLongestMatchingPrefix()
    {
        OWLManchesterContext context = new OWLManchesterContext(
        [
            new OWLPrefix(new RDFNamespace("a", "http://example.org/")),
            new OWLPrefix(new RDFNamespace("b", "http://example.org/pz#"))
        ]);

        Assert.AreEqual("b:Pizza", context.Abbreviate(new RDFResource("http://example.org/pz#Pizza")));
    }

    [TestMethod]
    public void ShouldFallBackToFullIRIWhenNoPrefixMatches()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("<http://other.org/Pizza>", context.Abbreviate(new RDFResource("http://other.org/Pizza")));
    }

    [TestMethod]
    public void ShouldFallBackToFullIRIWhenLocalNameIsNotLegal()
    {
        //The remainder after the prefix match must be a legal Manchester simple name: a slash is not
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("<http://example.org/pz#a/b>", context.Abbreviate(new RDFResource("http://example.org/pz#a/b")));
    }

    [TestMethod]
    public void ShouldFallBackToFullIRIWhenLocalNameIsEmpty()
    {
        //A prefix matching the whole IRI (empty local name) is not abbreviable
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("<http://example.org/pz#>", context.Abbreviate(new RDFResource("http://example.org/pz#")));
    }

    [TestMethod]
    public void ShouldAbbreviateLocalNameWithHyphenAndDot()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("pz:my-class.v2", context.Abbreviate(new RDFResource("http://example.org/pz#my-class.v2")));
    }
    #endregion

    #region Tests (Nest)
    [TestMethod]
    public void ShouldNestSelfDelimitingExpressionsWithoutParentheses()
    {
        OWLManchesterContext context = new OWLManchesterContext(
        [
            new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")),
            new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XSD.PREFIX))
        ]);

        Assert.AreEqual("pz:Pizza", context.Nest(new OWLClass(new RDFResource("http://example.org/pz#Pizza"))));
        Assert.AreEqual("pz:hasTopping", context.Nest(new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"))));
        Assert.AreEqual("pz:hasCalories", context.Nest(new OWLDataProperty(new RDFResource("http://example.org/pz#hasCalories"))));
        Assert.AreEqual("xsd:integer", context.Nest(new OWLDatatype(RDFVocabulary.XSD.INTEGER)));
    }

    [TestMethod]
    public void ShouldNestObjectOneOfWithoutExtraParentheses()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);
        OWLObjectOneOf oneOf = new OWLObjectOneOf([ new OWLNamedIndividual(new RDFResource("http://example.org/pz#A")) ]);

        Assert.AreEqual("{pz:A}", context.Nest(oneOf));
    }

    [TestMethod]
    public void ShouldWrapCompositeExpressionInParentheses()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);
        OWLObjectUnionOf union = new OWLObjectUnionOf(
        [
            new OWLClass(new RDFResource("http://example.org/pz#A")),
            new OWLClass(new RDFResource("http://example.org/pz#B"))
        ]);

        Assert.AreEqual("(pz:A or pz:B)", context.Nest(union));
    }
    #endregion

    #region Tests (annotation rendering)
    [TestMethod]
    public void ShouldRenderAxiomAnnotationsAsEmptyStringWhenNone()
    {
        OWLManchesterContext context = new OWLManchesterContext(new List<OWLPrefix>());

        Assert.AreEqual(string.Empty, context.RenderAxiomAnnotations(null));
        Assert.AreEqual(string.Empty, context.RenderAxiomAnnotations([]));
    }

    [TestMethod]
    public void ShouldRenderAxiomAnnotationsWithTrailingSpace()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        List<OWLAnnotation> annotations =
        [
            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("why")))
        ];

        Assert.AreEqual("Annotations: rdfs:comment \"why\" ", context.RenderAxiomAnnotations(annotations));
    }

    [TestMethod]
    public void ShouldRenderNestedAnnotationRecursively()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        OWLAnnotation nested = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("outer")))
        {
            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new OWLLiteral(new RDFPlainLiteral("meta")))
        };

        string rendered = context.RenderAnnotation(nested);

        Assert.AreEqual("Annotations: rdfs:label \"meta\" rdfs:comment \"outer\"", rendered);
    }

    [TestMethod]
    public void ShouldRenderAnnotationWithIRIValue()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)),
              new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);
        OWLAnnotation annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO), new RDFResource("http://example.org/pz#Pizza"));

        Assert.AreEqual("rdfs:seeAlso pz:Pizza", context.RenderAnnotation(annotation));
    }

    [TestMethod]
    public void ShouldRenderAnnotationWithAnonymousIndividualValue()
    {
        OWLManchesterContext context = new OWLManchesterContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        OWLAnnotation annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO), new OWLAnonymousIndividual("anon1"));

        Assert.AreEqual("rdfs:seeAlso _:anon1", context.RenderAnnotation(annotation));
    }
    #endregion

    #region Tests (facet symbols)
    [TestMethod]
    public void ShouldMapEveryKnownFacetToItsSymbol()
    {
        Assert.AreEqual("length", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.LENGTH.ToString()]);
        Assert.AreEqual("minLength", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.MIN_LENGTH.ToString()]);
        Assert.AreEqual("maxLength", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.MAX_LENGTH.ToString()]);
        Assert.AreEqual("pattern", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.PATTERN.ToString()]);
        Assert.AreEqual("langRange", OWLManchesterContext.FacetSymbols[RDFVocabulary.RDF.LANG_RANGE.ToString()]);
        Assert.AreEqual(">=", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.MIN_INCLUSIVE.ToString()]);
        Assert.AreEqual(">", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.MIN_EXCLUSIVE.ToString()]);
        Assert.AreEqual("<=", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.MAX_INCLUSIVE.ToString()]);
        Assert.AreEqual("<", OWLManchesterContext.FacetSymbols[RDFVocabulary.XSD.MAX_EXCLUSIVE.ToString()]);
    }
    #endregion
}
