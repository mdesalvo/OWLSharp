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

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLFunctionalContextTest
{
    #region Tests (ctor)
    [TestMethod]
    public void ShouldCreateContext()
    {
        OWLFunctionalContext context = new OWLFunctionalContext([]);

        Assert.IsNotNull(context);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingContextBecauseNullPrefixes()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLFunctionalContext(null));
    #endregion

    #region Tests (Abbreviate)
    [TestMethod]
    public void ShouldAbbreviateIRIWithMatchingPrefix()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("pz:Pizza", context.Abbreviate(new RDFResource("http://example.org/pz#Pizza")));
    }

    [TestMethod]
    public void ShouldAbbreviateWithLongestMatchingPrefix()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
        [
            new OWLPrefix(new RDFNamespace("a", "http://example.org/")),
            new OWLPrefix(new RDFNamespace("b", "http://example.org/pz#"))
        ]);

        Assert.AreEqual("b:Pizza", context.Abbreviate(new RDFResource("http://example.org/pz#Pizza")));
    }

    [TestMethod]
    public void ShouldFallBackToFullIRIWhenNoPrefixMatches()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("<http://other.org/Pizza>", context.Abbreviate(new RDFResource("http://other.org/Pizza")));
    }

    [TestMethod]
    public void ShouldFallBackToFullIRIWhenLocalNameIsNotLegal()
    {
        //The remainder after the prefix match must be a legal PN_LOCAL: a slash is not, nor is a space
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("<http://example.org/pz#a/b>", context.Abbreviate(new RDFResource("http://example.org/pz#a/b")));
        Assert.AreEqual("<http://example.org/pz#a b>", context.Abbreviate(new RDFResource("http://example.org/pz#a b")));
    }

    [TestMethod]
    public void ShouldFallBackToFullIRIWhenLocalNameIsEmpty()
    {
        //A prefix matching the whole IRI (empty local name) is not abbreviable
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);

        Assert.AreEqual("<http://example.org/pz#>", context.Abbreviate(new RDFResource("http://example.org/pz#")));
    }

    [TestMethod]
    public void ShouldAbbreviateWithDefaultPrefix()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix { Name = string.Empty, IRI = "http://example.org/pz#" } ]);

        Assert.AreEqual(":Pizza", context.Abbreviate(new RDFResource("http://example.org/pz#Pizza")));
    }
    #endregion

    #region Tests (RenderAnnotation)
    [TestMethod]
    public void ShouldRenderAnnotationWithIRIValue()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)),
              new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);
        OWLAnnotation annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO), new RDFResource("http://example.org/pz#Pizza"));

        Assert.AreEqual("Annotation( rdfs:seeAlso pz:Pizza )", context.RenderAnnotation(annotation));
    }

    [TestMethod]
    public void ShouldRenderAnnotationWithLiteralValue()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        OWLAnnotation annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("why")));

        Assert.AreEqual("Annotation( rdfs:comment \"why\" )", context.RenderAnnotation(annotation));
    }

    [TestMethod]
    public void ShouldRenderAnnotationWithAbbreviatedIRIValue()
    {
        //OWLAnnotation has a constructor overload taking an XmlQualifiedName (ValueAbbreviatedIRI), distinct
        //from the RDFResource overload (ValueIRI) already covered above: the two fields are mutually
        //exclusive alternatives for the same "IRI value" case, kept separate by the underlying object model
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)),
              new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")) ]);
        OWLAnnotation annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO), new System.Xml.XmlQualifiedName("Pizza", "http://example.org/pz#"));

        Assert.AreEqual("Annotation( rdfs:seeAlso pz:Pizza )", context.RenderAnnotation(annotation));
    }

    [TestMethod]
    public void ShouldRenderAnnotationWithAnonymousIndividualValue()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        OWLAnnotation annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO), new OWLAnonymousIndividual("anon1"));

        Assert.AreEqual("Annotation( rdfs:seeAlso _:anon1 )", context.RenderAnnotation(annotation));
    }

    [TestMethod]
    public void ShouldRenderNestedAnnotationBeforePropertyAndValue()
    {
        //As documented on OWLFunctionalContext.RenderAnnotation, a nested Annotation.Annotation renders
        //as a further "Annotation( ... )" token injected BEFORE the outer property/value pair
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        OWLAnnotation nested = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("outer")))
        {
            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new OWLLiteral(new RDFPlainLiteral("meta")))
        };

        string rendered = context.RenderAnnotation(nested);

        Assert.AreEqual("Annotation( Annotation( rdfs:label \"meta\" ) rdfs:comment \"outer\" )", rendered);
    }
    #endregion

    #region Tests (RenderAxiomAnnotations)
    [TestMethod]
    public void ShouldRenderAxiomAnnotationsAsEmptyStringWhenNone()
    {
        OWLFunctionalContext context = new OWLFunctionalContext([]);

        Assert.AreEqual(string.Empty, context.RenderAxiomAnnotations(null));
        Assert.AreEqual(string.Empty, context.RenderAxiomAnnotations([]));
    }

    [TestMethod]
    public void ShouldRenderAxiomAnnotationsWithTrailingSpace()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        List<OWLAnnotation> annotations =
        [
            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("why")))
        ];

        Assert.AreEqual("Annotation( rdfs:comment \"why\" ) ", context.RenderAxiomAnnotations(annotations));
    }

    [TestMethod]
    public void ShouldRenderMultipleAxiomAnnotationsConcatenated()
    {
        OWLFunctionalContext context = new OWLFunctionalContext(
            [ new OWLPrefix(new RDFNamespace("rdfs", RDFVocabulary.RDFS.BASE_URI)) ]);
        List<OWLAnnotation> annotations =
        [
            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("why"))),
            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new OWLLiteral(new RDFPlainLiteral("label")))
        ];

        Assert.AreEqual(
            "Annotation( rdfs:comment \"why\" ) Annotation( rdfs:label \"label\" ) ",
            context.RenderAxiomAnnotations(annotations));
    }
    #endregion
}
