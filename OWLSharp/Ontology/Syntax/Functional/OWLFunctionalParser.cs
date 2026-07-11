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

using RDFSharp.Model;
using System;
using System.Collections.Generic;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLFunctionalParser materializes an OWLOntology from an OWL2/Functional-Style document
    /// (https://www.w3.org/TR/owl2-syntax/#Functional-Style_Syntax). Unlike OWLManchesterParser,
    /// a single left-to-right recursive-descent pass is enough: the Functional-Style grammar is
    /// context-free with respect to entity kinds (every construct names its own keyword, so "p some C"
    /// never needs a symbol table to disambiguate object/data properties the way Manchester does),
    /// and every prefix declaration is guaranteed by the grammar (ontologyDocument := { prefixDeclaration }
    /// Ontology) to precede any place a prefixed name could reference it.
    /// </summary>
    public sealed class OWLFunctionalParser
    {
        #region Statics
        /// <summary>
        /// Defense against DOS attacks which might be conducted by specially crafted long-nested Functional-Style structures.<br/>
        /// Indicates that, at most, this depth level will be reached during ontology parsing. One more will generate exception.
        /// </summary>
        public static uint MaximumNestingLevel { get; set; } = 25;

        /// <summary>
        /// The Functional-Style keywords introducing a ClassExpression production other than a bare Class IRI
        /// </summary>
        private static readonly HashSet<string> ClassExpressionKeywords = new HashSet<string>
        {
            "ObjectIntersectionOf", "ObjectUnionOf", "ObjectComplementOf", "ObjectOneOf",
            "ObjectSomeValuesFrom", "ObjectAllValuesFrom", "ObjectHasValue", "ObjectHasSelf",
            "ObjectMinCardinality", "ObjectMaxCardinality", "ObjectExactCardinality",
            "DataSomeValuesFrom", "DataAllValuesFrom", "DataHasValue",
            "DataMinCardinality", "DataMaxCardinality", "DataExactCardinality"
        };

        /// <summary>
        /// The Functional-Style keywords introducing a DataRange production other than a bare Datatype IRI
        /// </summary>
        private static readonly HashSet<string> DataRangeKeywords = new HashSet<string>
        {
            "DataIntersectionOf", "DataUnionOf", "DataComplementOf", "DataOneOf", "DatatypeRestriction"
        };
        #endregion

        #region Properties
        /// <summary>
        /// The flat token sequence produced by OWLFunctionalLexer, consumed left-to-right
        /// </summary>
        private readonly List<OWLFunctionalToken> tokens;

        /// <summary>
        /// The prefix-to-namespace map, seeded with the ontology's default prefixes (rdf, rdfs, xsd, owl)
        /// and grown while parsing "Prefix(...)" declarations, which the grammar guarantees all precede
        /// the "Ontology(...)" block and therefore every place a prefixed name could reference them
        /// </summary>
        private readonly Dictionary<string, string> prefixes = new Dictionary<string, string>();

        /// <summary>
        /// The ontology being incrementally populated as parsing proceeds, returned by DeserializeOntology on completion
        /// </summary>
        private readonly OWLOntology ontology = new OWLOntology();

        /// <summary>
        /// The index of the next token to be consumed from the token sequence
        /// </summary>
        private int pos;

        /// <summary>
        /// The current depth of nested-expression recursion, guarded by EnterRecursion/ExitRecursion against
        /// stack exhaustion from adversarially deep-nested OWL2/Functional documents (e.g: long chains of
        /// ObjectIntersectionOf/ObjectComplementOf nesting, or deeply nested Annotation-on-Annotation blocks)
        /// </summary>
        private int nestingLevel;
        #endregion

        #region Ctors
        private OWLFunctionalParser(List<OWLFunctionalToken> tokens)
        {
            this.tokens = tokens;

            //Seed the prefix map with the ontology's default prefixes, so documents that only ever use
            //rdf:/rdfs:/xsd:/owl: without an explicit Prefix(...) declaration for them still resolve
            foreach (OWLPrefix prefix in ontology.Prefixes)
                prefixes[prefix.Name] = prefix.IRI;
        }
        #endregion

        #region Methods

        #region Entry point
        /// <summary>
        /// Deserializes the given OWL2/Functional-Style document to an OWLOntology object
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal static OWLOntology DeserializeOntology(string funDocument)
        {
            OWLFunctionalParser parser = new OWLFunctionalParser(OWLFunctionalLexer.Tokenize(funDocument));
            parser.ParseDocument();
            return parser.ontology;
        }
        #endregion

        #region Recursion guard
        /// <summary>
        /// Enters a nested production (composite class/data range expression, or nested annotation),
        /// failing fast once the maximum allowed recursion depth is exceeded
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private void EnterRecursion(string production)
        {
            if (++nestingLevel > MaximumNestingLevel)
                throw new OWLException($"Cannot parse OWL2/Functional document: exceeded the maximum allowed nesting depth ({MaximumNestingLevel}) while parsing {production}");
        }

        /// <summary>
        /// Leaves a nested production entered via EnterRecursion
        /// </summary>
        private void ExitRecursion()
            => nestingLevel--;
        #endregion

        #region Token utilities
        /// <summary>
        /// The token at the current parse position, not yet consumed
        /// </summary>
        private OWLFunctionalToken Current => tokens[pos];

        /// <summary>
        /// Looks ahead the given number of positions without consuming any token
        /// (clamped to the last token, which is always EndOfDocument, so lookahead never runs off the sequence)
        /// </summary>
        private OWLFunctionalToken Peek(int lookAhead)
            => pos + lookAhead < tokens.Count ? tokens[pos + lookAhead] : tokens[tokens.Count - 1];

        /// <summary>
        /// Consumes and returns the current token, moving the parse position forward
        /// </summary>
        private OWLFunctionalToken Advance()
            => tokens[pos++];

        /// <summary>
        /// Consumes the current token if it matches the given type, otherwise fails with a diagnostic
        /// naming the grammar production being parsed
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private OWLFunctionalToken Expect(OWLFunctionalTokenType tokenType, string production)
            => Current.Type == tokenType
                ? Advance()
                : throw new OWLException($"Cannot parse OWL2/Functional document: expected {tokenType} while parsing {production}, found {Current}");

        /// <summary>
        /// Tells if the current token is the given bare keyword (e.g: "SubClassOf", "Prefix"), without consuming it
        /// </summary>
        private bool CurrentIsName(string name)
            => Current.Type == OWLFunctionalTokenType.Name && string.Equals(Current.Value, name, StringComparison.Ordinal);

        /// <summary>
        /// Tells if the current token can reference an entity by IRI (either a full IRI or a prefixed name),
        /// without consuming it: entities in Functional-Style are always bare IRIs, never bare keywords
        /// </summary>
        private bool CurrentIsIRI()
            => Current.Type == OWLFunctionalTokenType.FullIRI || Current.Type == OWLFunctionalTokenType.PrefixedName;

        /// <summary>
        /// Resolves the given FullIRI/PrefixedName token to its full IRI string
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private string ResolveIRI(OWLFunctionalToken token)
        {
            if (token.Type == OWLFunctionalTokenType.FullIRI)
                return token.Value;

            //PrefixedName carries the whole "prefix:local" form (or ":local" for the default prefix):
            //split on the first colon, since local names themselves may legally contain further colons
            //only in the escaped PN_LOCAL production we don't attempt to recognize (see OWLFunctionalContext)
            int colonIndex = token.Value.IndexOf(':');
            string prefixName = token.Value.Substring(0, colonIndex);
            string localName = token.Value.Substring(colonIndex + 1);
            return prefixes.TryGetValue(prefixName, out string namespaceIRI)
                ? string.Concat(namespaceIRI, localName)
                : throw new OWLException($"Cannot parse OWL2/Functional document: undeclared prefix in {token}");
        }

        /// <summary>
        /// Consumes the current FullIRI/PrefixedName token and resolves it to an RDFResource, for use in
        /// building any of the "bare IRI" entity productions (Class, Datatype, ObjectProperty, DataProperty,
        /// AnnotationProperty, NamedIndividual all reduce to this)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private RDFResource ParseIRI(string production)
            => CurrentIsIRI()
                ? new RDFResource(ResolveIRI(Advance()))
                : throw new OWLException($"Cannot parse OWL2/Functional document: expected IRI while parsing {production}, found {Current}");
        #endregion

        #region Document structure
        /// <summary>
        /// ontologyDocument := { prefixDeclaration } Ontology
        /// </summary>
        private void ParseDocument()
        {
            //Every prefix must be registered before anything that might reference it, and the grammar
            //itself guarantees this ordering (all prefixDeclaration tokens precede the Ontology block)
            while (CurrentIsName("Prefix"))
                ParsePrefixDeclaration();

            ParseOntology();

            //Nothing legitimate follows the closing paren of Ontology(...): catching stray trailing
            //tokens here turns a silently-truncated parse into an immediate, precisely-located error
            Expect(OWLFunctionalTokenType.EndOfDocument, "document");
        }

        /// <summary>
        /// prefixDeclaration := 'Prefix' '(' prefixName '=' fullIRI ')'
        /// </summary>
        private void ParsePrefixDeclaration()
        {
            Advance(); //'Prefix'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "Prefix");

            //prefixName is lexed as a single PrefixNamespace token carrying "name:" (or just ":" for
            //the default/empty prefix) - strip the trailing colon to get the bare prefix name
            OWLFunctionalToken prefixNameToken = Expect(OWLFunctionalTokenType.PrefixNamespace, "Prefix");
            string prefixName = prefixNameToken.Value.TrimEnd(':');

            Expect(OWLFunctionalTokenType.Equals, "Prefix");
            string namespaceIRI = Expect(OWLFunctionalTokenType.FullIRI, "Prefix").Value;
            Expect(OWLFunctionalTokenType.CloseParenthesis, "Prefix");

            //The reserved rdf/rdfs/xsd/owl prefixes are already seeded by the constructor and MUST NOT
            //be redeclared per the grammar's own rules (Table 2): silently keep the built-in mapping
            //rather than raising an error over what is typically just a redundant, harmless restatement
            prefixes[prefixName] = namespaceIRI;
            if (!ontology.Prefixes.Exists(declaredPrefix => string.Equals(declaredPrefix.Name, prefixName, StringComparison.Ordinal)))
                ontology.Prefixes.Add(new OWLPrefix { Name = prefixName, IRI = namespaceIRI });
        }

        /// <summary>
        /// Ontology := 'Ontology' '(' [ ontologyIRI [ versionIRI ] ] directlyImportsDocuments ontologyAnnotations axioms ')'
        /// </summary>
        private void ParseOntology()
        {
            if (!CurrentIsName("Ontology"))
                throw new OWLException($"Cannot parse OWL2/Functional document: expected 'Ontology', found {Current}");
            Advance(); //'Ontology'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "Ontology");

            //Both ontologyIRI and versionIRI are optional, and versionIRI can only ever appear after
            //ontologyIRI - so a second consecutive IRI token, if present, can only be the version
            if (CurrentIsIRI())
            {
                ontology.IRI = ResolveIRI(Advance());
                if (CurrentIsIRI())
                    ontology.VersionIRI = ResolveIRI(Advance());
            }

            while (CurrentIsName("Import"))
            {
                Advance(); //'Import'
                Expect(OWLFunctionalTokenType.OpenParenthesis, "Import");
                ontology.Imports.Add(new OWLImport(ParseIRI("Import")));
                Expect(OWLFunctionalTokenType.CloseParenthesis, "Import");
            }

            while (CurrentIsName("Annotation"))
                ontology.Annotations.Add(ParseAnnotation());

            //Everything remaining until the closing paren of Ontology(...) is an Axiom production
            while (!(Current.Type == OWLFunctionalTokenType.CloseParenthesis))
                ParseAxiom();

            Expect(OWLFunctionalTokenType.CloseParenthesis, "Ontology");
        }
        #endregion

        #region Annotations
        /// <summary>
        /// axiomAnnotations := { Annotation } (identical shape to annotationAnnotations/ontologyAnnotations:
        /// zero or more leading "Annotation( ... )" tokens, consumed here regardless of which production hosts them)
        /// </summary>
        private List<OWLAnnotation> ParseAxiomAnnotations()
        {
            List<OWLAnnotation> annotations = new List<OWLAnnotation>();
            while (CurrentIsName("Annotation"))
                annotations.Add(ParseAnnotation());
            return annotations;
        }

        /// <summary>
        /// Annotation := 'Annotation' '(' annotationAnnotations AnnotationProperty AnnotationValue ')'
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private OWLAnnotation ParseAnnotation()
        {
            EnterRecursion("Annotation");
            try
            {
                Advance(); //'Annotation'
                Expect(OWLFunctionalTokenType.OpenParenthesis, "Annotation");

                //annotationAnnotations can carry more than one nested Annotation(...) token, but the OWL2
                //object model only has a single "Annotation" slot for the annotation-on-annotation relation
                //(mirrors OWLManchesterParser.NestAnnotations): only the first is representable, the rest
                //are silently unrepresentable and are simply not attached (no event bus hook exists here
                //to raise a non-fatal warning the way the Manchester parser does)
                List<OWLAnnotation> nestedAnnotations = ParseAxiomAnnotations();

                OWLAnnotationProperty annotationProperty = new OWLAnnotationProperty(ParseIRI("Annotation"));
                OWLAnnotation annotation = ParseAnnotationValue(annotationProperty);

                if (nestedAnnotations.Count > 0)
                    annotation.Annotation = nestedAnnotations[0];

                Expect(OWLFunctionalTokenType.CloseParenthesis, "Annotation");
                return annotation;
            }
            finally
            {
                ExitRecursion();
            }
        }

        /// <summary>
        /// AnnotationValue := AnonymousIndividual | IRI | Literal
        /// </summary>
        private OWLAnnotation ParseAnnotationValue(OWLAnnotationProperty annotationProperty)
        {
            if (Current.Type == OWLFunctionalTokenType.NodeID)
                return new OWLAnnotation(annotationProperty, ParseAnonymousIndividual());
            if (CurrentIsIRI())
                return new OWLAnnotation(annotationProperty, ParseIRI("AnnotationValue"));
            if (Current.Type == OWLFunctionalTokenType.QuotedString)
                return new OWLAnnotation(annotationProperty, ParseLiteral());

            throw new OWLException($"Cannot parse OWL2/Functional document: expected AnnotationValue, found {Current}");
        }
        #endregion

        #region Entities, individuals, literals
        /// <summary>
        /// AnonymousIndividual := nodeID
        /// </summary>
        private OWLAnonymousIndividual ParseAnonymousIndividual()
            => new OWLAnonymousIndividual(Expect(OWLFunctionalTokenType.NodeID, "AnonymousIndividual").Value);

        /// <summary>
        /// Individual := NamedIndividual | AnonymousIndividual
        /// </summary>
        private OWLIndividualExpression ParseIndividual()
            => Current.Type == OWLFunctionalTokenType.NodeID
                ? (OWLIndividualExpression)ParseAnonymousIndividual()
                : new OWLNamedIndividual(ParseIRI("Individual"));

        /// <summary>
        /// Literal := typedLiteral | stringLiteralNoLanguage | stringLiteralWithLanguage
        /// typedLiteral := lexicalForm '^^' Datatype; stringLiteralWithLanguage := quotedString languageTag
        /// </summary>
        private OWLLiteral ParseLiteral()
        {
            string lexicalForm = Expect(OWLFunctionalTokenType.QuotedString, "Literal").Value;

            if (Current.Type == OWLFunctionalTokenType.DoubleCaret)
            {
                Advance(); //'^^'
                return new OWLLiteral { Value = lexicalForm, DatatypeIRI = ParseIRI("Literal datatype").ToString() };
            }
            if (Current.Type == OWLFunctionalTokenType.LanguageTag)
                return new OWLLiteral { Value = lexicalForm, Language = Advance().Value };

            //Neither '^^Datatype' nor '@lang' follows: a bare quoted string, accepted as the abbreviation
            //for a plain literal with no language (stringLiteralNoLanguage production)
            return new OWLLiteral { Value = lexicalForm };
        }
        #endregion

        #region Property expressions
        /// <summary>
        /// ObjectPropertyExpression := ObjectProperty | InverseObjectProperty
        /// InverseObjectProperty := 'ObjectInverseOf' '(' ObjectProperty ')'
        /// </summary>
        private OWLObjectPropertyExpression ParseObjectPropertyExpression()
        {
            if (CurrentIsName("ObjectInverseOf"))
            {
                Advance(); //'ObjectInverseOf'
                Expect(OWLFunctionalTokenType.OpenParenthesis, "ObjectInverseOf");
                OWLObjectProperty invertedProperty = new OWLObjectProperty(ParseIRI("ObjectInverseOf"));
                Expect(OWLFunctionalTokenType.CloseParenthesis, "ObjectInverseOf");
                return new OWLObjectInverseOf(invertedProperty);
            }
            return new OWLObjectProperty(ParseIRI("ObjectPropertyExpression"));
        }

        /// <summary>
        /// DataPropertyExpression := DataProperty (the grammar's only current form: no composite data
        /// property expression exists yet, unlike the object property case with ObjectInverseOf)
        /// </summary>
        private OWLDataProperty ParseDataPropertyExpression()
            => new OWLDataProperty(ParseIRI("DataPropertyExpression"));
        #endregion

        #region Data ranges
        /// <summary>
        /// DataRange := Datatype | DataIntersectionOf | DataUnionOf | DataComplementOf | DataOneOf | DatatypeRestriction
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private OWLDataRangeExpression ParseDataRange()
        {
            //A bare IRI at this position is always the atomic case (Datatype), since every composite
            //DataRange production starts with one of the reserved keywords below
            if (CurrentIsIRI())
                return new OWLDatatype(ParseIRI("DataRange"));

            if (!CurrentIsAnyName(DataRangeKeywords))
                throw new OWLException($"Cannot parse OWL2/Functional document: expected DataRange, found {Current}");

            EnterRecursion("DataRange");
            try
            {
                string keyword = Advance().Value;
                Expect(OWLFunctionalTokenType.OpenParenthesis, keyword);

                OWLDataRangeExpression result;
                switch (keyword)
                {
                    case "DataIntersectionOf":
                        result = new OWLDataIntersectionOf(ParseDataRangeList(atLeast: 2));
                        break;
                    case "DataUnionOf":
                        result = new OWLDataUnionOf(ParseDataRangeList(atLeast: 2));
                        break;
                    case "DataComplementOf":
                        result = new OWLDataComplementOf(ParseDataRange());
                        break;
                    case "DataOneOf":
                        result = new OWLDataOneOf(ParseLiteralList(atLeast: 1));
                        break;
                    case "DatatypeRestriction":
                        result = ParseDatatypeRestriction();
                        break;
                    default:
                        //Unreachable: keyword was already checked against DataRangeKeywords above
                        throw new OWLException($"Cannot parse OWL2/Functional document: unexpected DataRange keyword '{keyword}'");
                }

                Expect(OWLFunctionalTokenType.CloseParenthesis, keyword);
                return result;
            }
            finally
            {
                ExitRecursion();
            }
        }

        /// <summary>
        /// DatatypeRestriction := 'DatatypeRestriction' '(' Datatype constrainingFacet restrictionValue { constrainingFacet restrictionValue } ')'
        /// (the opening keyword and '(' have already been consumed by the caller, ParseDataRange)
        /// </summary>
        private OWLDatatypeRestriction ParseDatatypeRestriction()
        {
            OWLDatatype restrictedDatatype = new OWLDatatype(ParseIRI("DatatypeRestriction"));

            //At least one (facet, literal) pair is required by the grammar; further pairs simply repeat
            //for as long as the next token is an IRI (a facet) rather than the closing parenthesis
            List<OWLFacetRestriction> facetRestrictions = new List<OWLFacetRestriction>();
            while (CurrentIsIRI())
            {
                RDFResource facetIRI = ParseIRI("DatatypeRestriction facet");
                OWLLiteral restrictionValue = ParseLiteral();
                facetRestrictions.Add(new OWLFacetRestriction(restrictionValue, facetIRI));
            }

            return new OWLDatatypeRestriction(restrictedDatatype, facetRestrictions);
        }

        /// <summary>
        /// Parses a whitespace-separated list of at least "atLeast" DataRange productions,
        /// stopping as soon as the next token cannot start one (the closing parenthesis)
        /// </summary>
        private List<OWLDataRangeExpression> ParseDataRangeList(int atLeast)
        {
            List<OWLDataRangeExpression> dataRanges = new List<OWLDataRangeExpression>();
            while (CurrentIsIRI() || CurrentIsAnyName(DataRangeKeywords))
                dataRanges.Add(ParseDataRange());

            if (dataRanges.Count < atLeast)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected at least {atLeast} DataRange, found {dataRanges.Count}");
            return dataRanges;
        }

        /// <summary>
        /// Parses a whitespace-separated list of at least "atLeast" Literal productions,
        /// stopping as soon as the next token cannot start one (anything but a quoted string)
        /// </summary>
        private List<OWLLiteral> ParseLiteralList(int atLeast)
        {
            List<OWLLiteral> literals = new List<OWLLiteral>();
            while (Current.Type == OWLFunctionalTokenType.QuotedString)
                literals.Add(ParseLiteral());

            if (literals.Count < atLeast)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected at least {atLeast} Literal, found {literals.Count}");
            return literals;
        }
        #endregion

        #region Class expressions
        /// <summary>
        /// ClassExpression := Class | ObjectIntersectionOf | ObjectUnionOf | ObjectComplementOf | ObjectOneOf |
        /// ObjectSomeValuesFrom | ObjectAllValuesFrom | ObjectHasValue | ObjectHasSelf |
        /// ObjectMinCardinality | ObjectMaxCardinality | ObjectExactCardinality |
        /// DataSomeValuesFrom | DataAllValuesFrom | DataHasValue |
        /// DataMinCardinality | DataMaxCardinality | DataExactCardinality
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private OWLClassExpression ParseClassExpression()
        {
            //A bare IRI at this position is always the atomic case (Class), since every composite
            //ClassExpression production starts with one of the reserved keywords below
            if (CurrentIsIRI())
                return new OWLClass(ParseIRI("ClassExpression"));

            if (!CurrentIsAnyName(ClassExpressionKeywords))
                throw new OWLException($"Cannot parse OWL2/Functional document: expected ClassExpression, found {Current}");

            EnterRecursion("ClassExpression");
            try
            {
                string keyword = Advance().Value;
                Expect(OWLFunctionalTokenType.OpenParenthesis, keyword);

                OWLClassExpression result = ParseClassExpressionBody(keyword);

                Expect(OWLFunctionalTokenType.CloseParenthesis, keyword);
                return result;
            }
            finally
            {
                ExitRecursion();
            }
        }

        /// <summary>
        /// Dispatches on the already-consumed composite keyword to build the corresponding ClassExpression,
        /// with the opening '(' already consumed by the caller and the closing ')' left for it to consume
        /// </summary>
        private OWLClassExpression ParseClassExpressionBody(string keyword)
        {
            switch (keyword)
            {
                case "ObjectIntersectionOf":
                    return new OWLObjectIntersectionOf(ParseClassExpressionList(atLeast: 2));
                case "ObjectUnionOf":
                    return new OWLObjectUnionOf(ParseClassExpressionList(atLeast: 2));
                case "ObjectComplementOf":
                    return new OWLObjectComplementOf(ParseClassExpression());
                case "ObjectOneOf":
                    return new OWLObjectOneOf(ParseIndividualList(atLeast: 1));

                case "ObjectSomeValuesFrom":
                {
                    OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();
                    return new OWLObjectSomeValuesFrom(objectPropertyExpression, ParseClassExpression());
                }
                case "ObjectAllValuesFrom":
                {
                    OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();
                    return new OWLObjectAllValuesFrom(objectPropertyExpression, ParseClassExpression());
                }
                case "ObjectHasValue":
                {
                    OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();
                    return new OWLObjectHasValue(objectPropertyExpression, ParseIndividual());
                }
                case "ObjectHasSelf":
                    return new OWLObjectHasSelf(ParseObjectPropertyExpression());

                case "ObjectMinCardinality":
                case "ObjectMaxCardinality":
                case "ObjectExactCardinality":
                    return ParseObjectCardinality(keyword);

                case "DataSomeValuesFrom":
                {
                    //The grammar formally allows "{ DataPropertyExpression }" (more than one property before
                    //the DataRange), a hook for n-ary data ranges that the current spec never actually defines
                    //beyond arity 1: the object model mirrors this by only carrying a single DataProperty
                    OWLDataProperty dataProperty = ParseDataPropertyExpression();
                    return new OWLDataSomeValuesFrom(dataProperty, ParseDataRange());
                }
                case "DataAllValuesFrom":
                {
                    OWLDataProperty dataProperty = ParseDataPropertyExpression();
                    return new OWLDataAllValuesFrom(dataProperty, ParseDataRange());
                }
                case "DataHasValue":
                {
                    OWLDataProperty dataProperty = ParseDataPropertyExpression();
                    return new OWLDataHasValue(dataProperty, ParseLiteral());
                }

                case "DataMinCardinality":
                case "DataMaxCardinality":
                case "DataExactCardinality":
                    return ParseDataCardinality(keyword);

                default:
                    //Unreachable: keyword was already checked against ClassExpressionKeywords by the caller
                    throw new OWLException($"Cannot parse OWL2/Functional document: unexpected ClassExpression keyword '{keyword}'");
            }
        }

        /// <summary>
        /// 'ObjectXxxCardinality' '(' nonNegativeInteger ObjectPropertyExpression [ ClassExpression ] ')'
        /// (the qualifying ClassExpression is optional: if absent, the restriction is implicitly qualified
        /// by owl:Thing - the model represents this by simply leaving ClassExpression null)
        /// </summary>
        private OWLClassExpression ParseObjectCardinality(string keyword)
        {
            uint cardinality = uint.Parse(Expect(OWLFunctionalTokenType.NonNegativeInteger, keyword).Value);
            OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();

            //A qualifying class expression is present unless the very next token is the closing paren
            OWLClassExpression qualifyingClassExpression = Current.Type != OWLFunctionalTokenType.CloseParenthesis
                ? ParseClassExpression()
                : null;

            switch (keyword)
            {
                case "ObjectMinCardinality":
                    return qualifyingClassExpression != null
                        ? new OWLObjectMinCardinality(objectPropertyExpression, cardinality, qualifyingClassExpression)
                        : new OWLObjectMinCardinality(objectPropertyExpression, cardinality);
                case "ObjectMaxCardinality":
                    return qualifyingClassExpression != null
                        ? new OWLObjectMaxCardinality(objectPropertyExpression, cardinality, qualifyingClassExpression)
                        : new OWLObjectMaxCardinality(objectPropertyExpression, cardinality);
                default: // "ObjectExactCardinality"
                    return qualifyingClassExpression != null
                        ? new OWLObjectExactCardinality(objectPropertyExpression, cardinality, qualifyingClassExpression)
                        : new OWLObjectExactCardinality(objectPropertyExpression, cardinality);
            }
        }

        /// <summary>
        /// 'DataXxxCardinality' '(' nonNegativeInteger DataPropertyExpression [ DataRange ] ')'
        /// (the qualifying DataRange is optional: if absent, the restriction is implicitly qualified
        /// by rdfs:Literal - the model represents this by simply leaving DataRangeExpression null)
        /// </summary>
        private OWLClassExpression ParseDataCardinality(string keyword)
        {
            uint cardinality = uint.Parse(Expect(OWLFunctionalTokenType.NonNegativeInteger, keyword).Value);
            OWLDataProperty dataProperty = ParseDataPropertyExpression();

            OWLDataRangeExpression qualifyingDataRange = Current.Type != OWLFunctionalTokenType.CloseParenthesis
                ? ParseDataRange()
                : null;

            switch (keyword)
            {
                case "DataMinCardinality":
                    return qualifyingDataRange != null
                        ? new OWLDataMinCardinality(dataProperty, cardinality, qualifyingDataRange)
                        : new OWLDataMinCardinality(dataProperty, cardinality);
                case "DataMaxCardinality":
                    return qualifyingDataRange != null
                        ? new OWLDataMaxCardinality(dataProperty, cardinality, qualifyingDataRange)
                        : new OWLDataMaxCardinality(dataProperty, cardinality);
                default: // "DataExactCardinality"
                    return qualifyingDataRange != null
                        ? new OWLDataExactCardinality(dataProperty, cardinality, qualifyingDataRange)
                        : new OWLDataExactCardinality(dataProperty, cardinality);
            }
        }

        /// <summary>
        /// Parses a whitespace-separated list of at least "atLeast" ClassExpression productions,
        /// stopping as soon as the next token cannot start one (the closing parenthesis)
        /// </summary>
        private List<OWLClassExpression> ParseClassExpressionList(int atLeast)
        {
            List<OWLClassExpression> classExpressions = new List<OWLClassExpression>();
            while (CurrentIsIRI() || CurrentIsAnyName(ClassExpressionKeywords))
                classExpressions.Add(ParseClassExpression());

            if (classExpressions.Count < atLeast)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected at least {atLeast} ClassExpression, found {classExpressions.Count}");
            return classExpressions;
        }

        /// <summary>
        /// Parses a whitespace-separated list of at least "atLeast" Individual productions,
        /// stopping as soon as the next token cannot start one (anything but an IRI or a nodeID)
        /// </summary>
        private List<OWLIndividualExpression> ParseIndividualList(int atLeast)
        {
            List<OWLIndividualExpression> individuals = new List<OWLIndividualExpression>();
            while (CurrentIsIRI() || Current.Type == OWLFunctionalTokenType.NodeID)
                individuals.Add(ParseIndividual());

            if (individuals.Count < atLeast)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected at least {atLeast} Individual, found {individuals.Count}");
            return individuals;
        }

        /// <summary>
        /// Tells if the current token is a bare keyword found in the given reserved-word set, without consuming it
        /// </summary>
        private bool CurrentIsAnyName(HashSet<string> names)
            => Current.Type == OWLFunctionalTokenType.Name && names.Contains(Current.Value);
        #endregion

        #region Axioms
        /// <summary>
        /// Axiom := Declaration | ClassAxiom | ObjectPropertyAxiom | DataPropertyAxiom |
        /// DatatypeDefinition | HasKey | Assertion | AnnotationAxiom
        /// (dispatches on the leading keyword and appends the parsed axiom to the matching
        /// OWLOntology registry list - mirrors OWLManchesterParser's per-frame dispatch, but flat:
        /// every axiom keyword is legal at this position regardless of what came before it)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private void ParseAxiom()
        {
            if (Current.Type != OWLFunctionalTokenType.Name)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected Axiom, found {Current}");

            string keyword = Current.Value;
            switch (keyword)
            {
                case "Declaration":
                    ontology.DeclarationAxioms.Add(ParseDeclaration());
                    return;

                //ClassAxiom
                case "SubClassOf": ontology.ClassAxioms.Add(ParseSubClassOf()); return;
                case "EquivalentClasses": ontology.ClassAxioms.Add(ParseEquivalentClasses()); return;
                case "DisjointClasses": ontology.ClassAxioms.Add(ParseDisjointClasses()); return;
                case "DisjointUnion": ontology.ClassAxioms.Add(ParseDisjointUnion()); return;

                //ObjectPropertyAxiom
                case "SubObjectPropertyOf": ontology.ObjectPropertyAxioms.Add(ParseSubObjectPropertyOf()); return;
                case "EquivalentObjectProperties": ontology.ObjectPropertyAxioms.Add(ParseEquivalentObjectProperties()); return;
                case "DisjointObjectProperties": ontology.ObjectPropertyAxioms.Add(ParseDisjointObjectProperties()); return;
                case "InverseObjectProperties": ontology.ObjectPropertyAxioms.Add(ParseInverseObjectProperties()); return;
                case "ObjectPropertyDomain": ontology.ObjectPropertyAxioms.Add(ParseObjectPropertyDomain()); return;
                case "ObjectPropertyRange": ontology.ObjectPropertyAxioms.Add(ParseObjectPropertyRange()); return;
                case "FunctionalObjectProperty": ontology.ObjectPropertyAxioms.Add(ParsePropertyCharacteristic(keyword)); return;
                case "InverseFunctionalObjectProperty": ontology.ObjectPropertyAxioms.Add(ParsePropertyCharacteristic(keyword)); return;
                case "ReflexiveObjectProperty": ontology.ObjectPropertyAxioms.Add(ParsePropertyCharacteristic(keyword)); return;
                case "IrreflexiveObjectProperty": ontology.ObjectPropertyAxioms.Add(ParsePropertyCharacteristic(keyword)); return;
                case "SymmetricObjectProperty": ontology.ObjectPropertyAxioms.Add(ParsePropertyCharacteristic(keyword)); return;
                case "AsymmetricObjectProperty": ontology.ObjectPropertyAxioms.Add(ParsePropertyCharacteristic(keyword)); return;
                case "TransitiveObjectProperty": ontology.ObjectPropertyAxioms.Add(ParsePropertyCharacteristic(keyword)); return;

                //DataPropertyAxiom
                case "SubDataPropertyOf": ontology.DataPropertyAxioms.Add(ParseSubDataPropertyOf()); return;
                case "EquivalentDataProperties": ontology.DataPropertyAxioms.Add(ParseEquivalentDataProperties()); return;
                case "DisjointDataProperties": ontology.DataPropertyAxioms.Add(ParseDisjointDataProperties()); return;
                case "DataPropertyDomain": ontology.DataPropertyAxioms.Add(ParseDataPropertyDomain()); return;
                case "DataPropertyRange": ontology.DataPropertyAxioms.Add(ParseDataPropertyRange()); return;
                case "FunctionalDataProperty": ontology.DataPropertyAxioms.Add(ParseFunctionalDataProperty()); return;

                case "DatatypeDefinition": ontology.DatatypeDefinitionAxioms.Add(ParseDatatypeDefinition()); return;
                case "HasKey": ontology.KeyAxioms.Add(ParseHasKey()); return;

                //Assertion
                case "SameIndividual": ontology.AssertionAxioms.Add(ParseSameOrDifferentIndividual(keyword)); return;
                case "DifferentIndividuals": ontology.AssertionAxioms.Add(ParseSameOrDifferentIndividual(keyword)); return;
                case "ClassAssertion": ontology.AssertionAxioms.Add(ParseClassAssertion()); return;
                case "ObjectPropertyAssertion": ontology.AssertionAxioms.Add(ParseObjectPropertyAssertion(negative: false)); return;
                case "NegativeObjectPropertyAssertion": ontology.AssertionAxioms.Add(ParseObjectPropertyAssertion(negative: true)); return;
                case "DataPropertyAssertion": ontology.AssertionAxioms.Add(ParseDataPropertyAssertion(negative: false)); return;
                case "NegativeDataPropertyAssertion": ontology.AssertionAxioms.Add(ParseDataPropertyAssertion(negative: true)); return;

                //AnnotationAxiom
                case "AnnotationAssertion": ontology.AnnotationAxioms.Add(ParseAnnotationAssertion()); return;
                case "SubAnnotationPropertyOf": ontology.AnnotationAxioms.Add(ParseSubAnnotationPropertyOf()); return;
                case "AnnotationPropertyDomain": ontology.AnnotationAxioms.Add(ParseAnnotationPropertyDomainOrRange(keyword)); return;
                case "AnnotationPropertyRange": ontology.AnnotationAxioms.Add(ParseAnnotationPropertyDomainOrRange(keyword)); return;

                default:
                    throw new OWLException($"Cannot parse OWL2/Functional document: unrecognized Axiom keyword '{keyword}'");
            }
        }

        /// <summary>
        /// Applies the parsed axiomAnnotations to the given axiom, annotating it before returning
        /// so it is never observed by callers in a half-annotated state (same discipline as
        /// OWLManchesterParser.AddAxiom, just without a shared registry parameter: each call site
        /// here already knows which OWLOntology list it will append the axiom to)
        /// </summary>
        private static T Annotate<T>(T axiom, List<OWLAnnotation> annotations) where T : OWLAxiom
        {
            annotations.ForEach(axiom.Annotate);
            return axiom;
        }

        /// <summary>
        /// Declaration := 'Declaration' '(' axiomAnnotations Entity ')'
        /// Entity := 'Class'(Class) | 'Datatype'(Datatype) | 'ObjectProperty'(ObjectProperty) |
        /// 'DataProperty'(DataProperty) | 'AnnotationProperty'(AnnotationProperty) | 'NamedIndividual'(NamedIndividual)
        /// (unlike every other "bare IRI" entity reference in the grammar, a declared entity is explicitly
        /// wrapped in its own keyword here, since this is the one production that needs to name the kind
        /// of a freshly-introduced IRI rather than merely referencing an already-typed one)
        /// </summary>
        private OWLDeclaration ParseDeclaration()
        {
            Advance(); //'Declaration'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "Declaration");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();

            if (Current.Type != OWLFunctionalTokenType.Name)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected Entity while parsing Declaration, found {Current}");

            string entityKeyword = Advance().Value;
            Expect(OWLFunctionalTokenType.OpenParenthesis, entityKeyword);
            OWLDeclaration declaration;
            switch (entityKeyword)
            {
                case "Class": declaration = new OWLDeclaration(new OWLClass(ParseIRI(entityKeyword))); break;
                case "Datatype": declaration = new OWLDeclaration(new OWLDatatype(ParseIRI(entityKeyword))); break;
                case "ObjectProperty": declaration = new OWLDeclaration(new OWLObjectProperty(ParseIRI(entityKeyword))); break;
                case "DataProperty": declaration = new OWLDeclaration(new OWLDataProperty(ParseIRI(entityKeyword))); break;
                case "AnnotationProperty": declaration = new OWLDeclaration(new OWLAnnotationProperty(ParseIRI(entityKeyword))); break;
                case "NamedIndividual": declaration = new OWLDeclaration(new OWLNamedIndividual(ParseIRI(entityKeyword))); break;
                default:
                    throw new OWLException($"Cannot parse OWL2/Functional document: unrecognized Entity keyword '{entityKeyword}'");
            }
            Expect(OWLFunctionalTokenType.CloseParenthesis, entityKeyword);

            Expect(OWLFunctionalTokenType.CloseParenthesis, "Declaration");
            return Annotate(declaration, annotations);
        }

        #region Class axioms
        /// <summary>
        /// SubClassOf := 'SubClassOf' '(' axiomAnnotations subClassExpression superClassExpression ')'
        /// </summary>
        private OWLSubClassOf ParseSubClassOf()
        {
            Advance(); //'SubClassOf'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "SubClassOf");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLClassExpression subClassExpression = ParseClassExpression();
            OWLClassExpression superClassExpression = ParseClassExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "SubClassOf");
            return Annotate(new OWLSubClassOf(subClassExpression, superClassExpression), annotations);
        }

        /// <summary>
        /// EquivalentClasses := 'EquivalentClasses' '(' axiomAnnotations ClassExpression ClassExpression { ClassExpression } ')'
        /// </summary>
        private OWLEquivalentClasses ParseEquivalentClasses()
        {
            Advance(); //'EquivalentClasses'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "EquivalentClasses");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            List<OWLClassExpression> classExpressions = ParseClassExpressionList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, "EquivalentClasses");
            return Annotate(new OWLEquivalentClasses(classExpressions), annotations);
        }

        /// <summary>
        /// DisjointClasses := 'DisjointClasses' '(' axiomAnnotations ClassExpression ClassExpression { ClassExpression } ')'
        /// </summary>
        private OWLDisjointClasses ParseDisjointClasses()
        {
            Advance(); //'DisjointClasses'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "DisjointClasses");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            List<OWLClassExpression> classExpressions = ParseClassExpressionList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, "DisjointClasses");
            return Annotate(new OWLDisjointClasses(classExpressions), annotations);
        }

        /// <summary>
        /// DisjointUnion := 'DisjointUnion' '(' axiomAnnotations Class disjointClassExpressions ')'
        /// (the first argument is specifically a named Class, not an arbitrary ClassExpression)
        /// </summary>
        private OWLDisjointUnion ParseDisjointUnion()
        {
            Advance(); //'DisjointUnion'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "DisjointUnion");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLClass unionClass = new OWLClass(ParseIRI("DisjointUnion"));
            List<OWLClassExpression> disjointClassExpressions = ParseClassExpressionList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, "DisjointUnion");
            return Annotate(new OWLDisjointUnion(unionClass, disjointClassExpressions), annotations);
        }
        #endregion

        #region Object property axioms
        /// <summary>
        /// SubObjectPropertyOf := 'SubObjectPropertyOf' '(' axiomAnnotations subObjectPropertyExpression superObjectPropertyExpression ')'
        /// subObjectPropertyExpression := ObjectPropertyExpression | propertyExpressionChain
        /// propertyExpressionChain := 'ObjectPropertyChain' '(' ObjectPropertyExpression ObjectPropertyExpression { ObjectPropertyExpression } ')'
        /// </summary>
        private OWLSubObjectPropertyOf ParseSubObjectPropertyOf()
        {
            Advance(); //'SubObjectPropertyOf'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "SubObjectPropertyOf");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();

            OWLSubObjectPropertyOf axiom;
            if (CurrentIsName("ObjectPropertyChain"))
            {
                Advance(); //'ObjectPropertyChain'
                Expect(OWLFunctionalTokenType.OpenParenthesis, "ObjectPropertyChain");
                List<OWLObjectPropertyExpression> chainMembers = new List<OWLObjectPropertyExpression>();
                //The chain keeps reading property expressions until it hits its own closing paren:
                //a plain IRI/PrefixedName or "ObjectInverseOf(...)" are the only two things that can follow
                while (CurrentIsIRI() || CurrentIsName("ObjectInverseOf"))
                    chainMembers.Add(ParseObjectPropertyExpression());
                Expect(OWLFunctionalTokenType.CloseParenthesis, "ObjectPropertyChain");

                OWLObjectPropertyExpression superObjectPropertyExpression = ParseObjectPropertyExpression();
                axiom = new OWLSubObjectPropertyOf(new OWLObjectPropertyChain(chainMembers), superObjectPropertyExpression);
            }
            else
            {
                OWLObjectPropertyExpression subObjectPropertyExpression = ParseObjectPropertyExpression();
                OWLObjectPropertyExpression superObjectPropertyExpression = ParseObjectPropertyExpression();
                axiom = new OWLSubObjectPropertyOf(subObjectPropertyExpression, superObjectPropertyExpression);
            }

            Expect(OWLFunctionalTokenType.CloseParenthesis, "SubObjectPropertyOf");
            return Annotate(axiom, annotations);
        }

        /// <summary>
        /// EquivalentObjectProperties := 'EquivalentObjectProperties' '(' axiomAnnotations ObjectPropertyExpression ObjectPropertyExpression { ObjectPropertyExpression } ')'
        /// </summary>
        private OWLEquivalentObjectProperties ParseEquivalentObjectProperties()
        {
            Advance(); //'EquivalentObjectProperties'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "EquivalentObjectProperties");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            List<OWLObjectPropertyExpression> objectPropertyExpressions = ParseObjectPropertyExpressionList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, "EquivalentObjectProperties");
            return Annotate(new OWLEquivalentObjectProperties(objectPropertyExpressions), annotations);
        }

        /// <summary>
        /// DisjointObjectProperties := 'DisjointObjectProperties' '(' axiomAnnotations ObjectPropertyExpression ObjectPropertyExpression { ObjectPropertyExpression } ')'
        /// </summary>
        private OWLDisjointObjectProperties ParseDisjointObjectProperties()
        {
            Advance(); //'DisjointObjectProperties'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "DisjointObjectProperties");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            List<OWLObjectPropertyExpression> objectPropertyExpressions = ParseObjectPropertyExpressionList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, "DisjointObjectProperties");
            return Annotate(new OWLDisjointObjectProperties(objectPropertyExpressions), annotations);
        }

        /// <summary>
        /// InverseObjectProperties := 'InverseObjectProperties' '(' axiomAnnotations ObjectPropertyExpression ObjectPropertyExpression ')'
        /// (exactly two members, unlike Equivalent/DisjointObjectProperties which allow further ones)
        /// </summary>
        private OWLInverseObjectProperties ParseInverseObjectProperties()
        {
            Advance(); //'InverseObjectProperties'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "InverseObjectProperties");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLObjectPropertyExpression leftObjectPropertyExpression = ParseObjectPropertyExpression();
            OWLObjectPropertyExpression rightObjectPropertyExpression = ParseObjectPropertyExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "InverseObjectProperties");
            return Annotate(new OWLInverseObjectProperties(leftObjectPropertyExpression, rightObjectPropertyExpression), annotations);
        }

        /// <summary>
        /// ObjectPropertyDomain := 'ObjectPropertyDomain' '(' axiomAnnotations ObjectPropertyExpression ClassExpression ')'
        /// </summary>
        private OWLObjectPropertyDomain ParseObjectPropertyDomain()
        {
            Advance(); //'ObjectPropertyDomain'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "ObjectPropertyDomain");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();
            OWLClassExpression domainClassExpression = ParseClassExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "ObjectPropertyDomain");
            return Annotate(new OWLObjectPropertyDomain(objectPropertyExpression, domainClassExpression), annotations);
        }

        /// <summary>
        /// ObjectPropertyRange := 'ObjectPropertyRange' '(' axiomAnnotations ObjectPropertyExpression ClassExpression ')'
        /// </summary>
        private OWLObjectPropertyRange ParseObjectPropertyRange()
        {
            Advance(); //'ObjectPropertyRange'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "ObjectPropertyRange");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();
            OWLClassExpression rangeClassExpression = ParseClassExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "ObjectPropertyRange");
            return Annotate(new OWLObjectPropertyRange(objectPropertyExpression, rangeClassExpression), annotations);
        }

        /// <summary>
        /// 'Keyword' '(' axiomAnnotations ObjectPropertyExpression ')', shared by the seven single-argument
        /// object property characteristic axioms (Functional/InverseFunctional/Reflexive/Irreflexive/
        /// Symmetric/Asymmetric/TransitiveObjectProperty). Each of these axiom classes exposes two public
        /// constructors (one for a plain OWLObjectProperty, one for OWLObjectInverseOf) instead of a single
        /// constructor over the OWLObjectPropertyExpression base type, so the runtime type of the parsed
        /// property expression has to be dispatched here rather than passed straight through
        /// </summary>
        private OWLObjectPropertyAxiom ParsePropertyCharacteristic(string keyword)
        {
            Advance(); //keyword
            Expect(OWLFunctionalTokenType.OpenParenthesis, keyword);
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, keyword);

            OWLObjectPropertyAxiom axiom;
            switch (keyword)
            {
                case "FunctionalObjectProperty":
                    axiom = objectPropertyExpression is OWLObjectProperty namedFp ? new OWLFunctionalObjectProperty(namedFp) : new OWLFunctionalObjectProperty((OWLObjectInverseOf)objectPropertyExpression);
                    break;
                case "InverseFunctionalObjectProperty":
                    axiom = objectPropertyExpression is OWLObjectProperty namedIfp ? new OWLInverseFunctionalObjectProperty(namedIfp) : new OWLInverseFunctionalObjectProperty((OWLObjectInverseOf)objectPropertyExpression);
                    break;
                case "ReflexiveObjectProperty":
                    axiom = objectPropertyExpression is OWLObjectProperty namedRp ? new OWLReflexiveObjectProperty(namedRp) : new OWLReflexiveObjectProperty((OWLObjectInverseOf)objectPropertyExpression);
                    break;
                case "IrreflexiveObjectProperty":
                    axiom = objectPropertyExpression is OWLObjectProperty namedIrp ? new OWLIrreflexiveObjectProperty(namedIrp) : new OWLIrreflexiveObjectProperty((OWLObjectInverseOf)objectPropertyExpression);
                    break;
                case "SymmetricObjectProperty":
                    axiom = objectPropertyExpression is OWLObjectProperty namedSp ? new OWLSymmetricObjectProperty(namedSp) : new OWLSymmetricObjectProperty((OWLObjectInverseOf)objectPropertyExpression);
                    break;
                case "AsymmetricObjectProperty":
                    axiom = objectPropertyExpression is OWLObjectProperty namedAsp ? new OWLAsymmetricObjectProperty(namedAsp) : new OWLAsymmetricObjectProperty((OWLObjectInverseOf)objectPropertyExpression);
                    break;
                default: // "TransitiveObjectProperty"
                    axiom = objectPropertyExpression is OWLObjectProperty namedTp ? new OWLTransitiveObjectProperty(namedTp) : new OWLTransitiveObjectProperty((OWLObjectInverseOf)objectPropertyExpression);
                    break;
            }
            return Annotate(axiom, annotations);
        }

        /// <summary>
        /// Parses a whitespace-separated list of at least "atLeast" ObjectPropertyExpression productions
        /// </summary>
        private List<OWLObjectPropertyExpression> ParseObjectPropertyExpressionList(int atLeast)
        {
            List<OWLObjectPropertyExpression> objectPropertyExpressions = new List<OWLObjectPropertyExpression>();
            while (CurrentIsIRI() || CurrentIsName("ObjectInverseOf"))
                objectPropertyExpressions.Add(ParseObjectPropertyExpression());

            if (objectPropertyExpressions.Count < atLeast)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected at least {atLeast} ObjectPropertyExpression, found {objectPropertyExpressions.Count}");
            return objectPropertyExpressions;
        }
        #endregion

        #region Data property axioms
        /// <summary>
        /// SubDataPropertyOf := 'SubDataPropertyOf' '(' axiomAnnotations subDataPropertyExpression superDataPropertyExpression ')'
        /// (no property-chain form here, unlike SubObjectPropertyOf: data properties are never composable)
        /// </summary>
        private OWLSubDataPropertyOf ParseSubDataPropertyOf()
        {
            Advance(); //'SubDataPropertyOf'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "SubDataPropertyOf");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLDataProperty subDataProperty = ParseDataPropertyExpression();
            OWLDataProperty superDataProperty = ParseDataPropertyExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "SubDataPropertyOf");
            return Annotate(new OWLSubDataPropertyOf(subDataProperty, superDataProperty), annotations);
        }

        /// <summary>
        /// EquivalentDataProperties := 'EquivalentDataProperties' '(' axiomAnnotations DataPropertyExpression DataPropertyExpression { DataPropertyExpression } ')'
        /// </summary>
        private OWLEquivalentDataProperties ParseEquivalentDataProperties()
        {
            Advance(); //'EquivalentDataProperties'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "EquivalentDataProperties");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            List<OWLDataProperty> dataProperties = ParseDataPropertyList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, "EquivalentDataProperties");
            return Annotate(new OWLEquivalentDataProperties(dataProperties), annotations);
        }

        /// <summary>
        /// DisjointDataProperties := 'DisjointDataProperties' '(' axiomAnnotations DataPropertyExpression DataPropertyExpression { DataPropertyExpression } ')'
        /// </summary>
        private OWLDisjointDataProperties ParseDisjointDataProperties()
        {
            Advance(); //'DisjointDataProperties'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "DisjointDataProperties");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            List<OWLDataProperty> dataProperties = ParseDataPropertyList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, "DisjointDataProperties");
            return Annotate(new OWLDisjointDataProperties(dataProperties), annotations);
        }

        /// <summary>
        /// DataPropertyDomain := 'DataPropertyDomain' '(' axiomAnnotations DataPropertyExpression ClassExpression ')'
        /// </summary>
        private OWLDataPropertyDomain ParseDataPropertyDomain()
        {
            Advance(); //'DataPropertyDomain'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "DataPropertyDomain");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLDataProperty dataProperty = ParseDataPropertyExpression();
            OWLClassExpression domainClassExpression = ParseClassExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "DataPropertyDomain");
            return Annotate(new OWLDataPropertyDomain(dataProperty, domainClassExpression), annotations);
        }

        /// <summary>
        /// DataPropertyRange := 'DataPropertyRange' '(' axiomAnnotations DataPropertyExpression DataRange ')'
        /// </summary>
        private OWLDataPropertyRange ParseDataPropertyRange()
        {
            Advance(); //'DataPropertyRange'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "DataPropertyRange");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLDataProperty dataProperty = ParseDataPropertyExpression();
            OWLDataRangeExpression rangeDataRange = ParseDataRange();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "DataPropertyRange");
            return Annotate(new OWLDataPropertyRange(dataProperty, rangeDataRange), annotations);
        }

        /// <summary>
        /// FunctionalDataProperty := 'FunctionalDataProperty' '(' axiomAnnotations DataPropertyExpression ')'
        /// </summary>
        private OWLFunctionalDataProperty ParseFunctionalDataProperty()
        {
            Advance(); //'FunctionalDataProperty'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "FunctionalDataProperty");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLDataProperty dataProperty = ParseDataPropertyExpression();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "FunctionalDataProperty");
            return Annotate(new OWLFunctionalDataProperty(dataProperty), annotations);
        }

        /// <summary>
        /// Parses a whitespace-separated list of at least "atLeast" DataPropertyExpression productions
        /// </summary>
        private List<OWLDataProperty> ParseDataPropertyList(int atLeast)
        {
            List<OWLDataProperty> dataProperties = new List<OWLDataProperty>();
            while (CurrentIsIRI())
                dataProperties.Add(ParseDataPropertyExpression());

            if (dataProperties.Count < atLeast)
                throw new OWLException($"Cannot parse OWL2/Functional document: expected at least {atLeast} DataPropertyExpression, found {dataProperties.Count}");
            return dataProperties;
        }
        #endregion

        #region Datatype definition and keys
        /// <summary>
        /// DatatypeDefinition := 'DatatypeDefinition' '(' axiomAnnotations Datatype DataRange ')'
        /// </summary>
        private OWLDatatypeDefinition ParseDatatypeDefinition()
        {
            Advance(); //'DatatypeDefinition'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "DatatypeDefinition");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLDatatype definedDatatype = new OWLDatatype(ParseIRI("DatatypeDefinition"));
            OWLDataRangeExpression equivalentDataRange = ParseDataRange();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "DatatypeDefinition");
            return Annotate(new OWLDatatypeDefinition(definedDatatype, equivalentDataRange), annotations);
        }

        /// <summary>
        /// HasKey := 'HasKey' '(' axiomAnnotations ClassExpression '(' { ObjectPropertyExpression } ')' '(' { DataPropertyExpression } ')' ')'
        /// (two independent parenthesized lists, each individually allowed to be empty - but per the DL
        /// global restrictions at least one of the two must end up non-empty for a conformant document;
        /// this parser does not enforce that DL-level constraint, since it also accepts non-DL ontologies)
        /// </summary>
        private OWLHasKey ParseHasKey()
        {
            Advance(); //'HasKey'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "HasKey");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLClassExpression keyedClassExpression = ParseClassExpression();

            Expect(OWLFunctionalTokenType.OpenParenthesis, "HasKey object properties");
            List<OWLObjectPropertyExpression> keyObjectProperties = new List<OWLObjectPropertyExpression>();
            while (CurrentIsIRI() || CurrentIsName("ObjectInverseOf"))
                keyObjectProperties.Add(ParseObjectPropertyExpression());
            Expect(OWLFunctionalTokenType.CloseParenthesis, "HasKey object properties");

            Expect(OWLFunctionalTokenType.OpenParenthesis, "HasKey data properties");
            List<OWLDataProperty> keyDataProperties = new List<OWLDataProperty>();
            while (CurrentIsIRI())
                keyDataProperties.Add(ParseDataPropertyExpression());
            Expect(OWLFunctionalTokenType.CloseParenthesis, "HasKey data properties");

            Expect(OWLFunctionalTokenType.CloseParenthesis, "HasKey");

            //OWLHasKey only exposes constructors seeded with either the object or the data property list
            //(each defaulting the other side to empty): build with whichever side is non-empty, and if
            //both are non-empty, seed with the object side then assign the data side directly afterwards
            OWLHasKey axiom = keyObjectProperties.Count > 0
                ? new OWLHasKey(keyedClassExpression, keyObjectProperties)
                : new OWLHasKey(keyedClassExpression, keyDataProperties);
            if (keyObjectProperties.Count > 0 && keyDataProperties.Count > 0)
                axiom.DataProperties = keyDataProperties;

            return Annotate(axiom, annotations);
        }
        #endregion

        #region Assertions
        /// <summary>
        /// SameIndividual := 'SameIndividual' '(' axiomAnnotations Individual Individual { Individual } ')'
        /// DifferentIndividuals := 'DifferentIndividuals' '(' axiomAnnotations Individual Individual { Individual } ')'
        /// </summary>
        private OWLAssertionAxiom ParseSameOrDifferentIndividual(string keyword)
        {
            Advance(); //keyword
            Expect(OWLFunctionalTokenType.OpenParenthesis, keyword);
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            List<OWLIndividualExpression> individuals = ParseIndividualList(atLeast: 2);
            Expect(OWLFunctionalTokenType.CloseParenthesis, keyword);

            OWLAssertionAxiom axiom = keyword == "SameIndividual"
                ? (OWLAssertionAxiom)new OWLSameIndividual(individuals)
                : new OWLDifferentIndividuals(individuals);
            return Annotate(axiom, annotations);
        }

        /// <summary>
        /// ClassAssertion := 'ClassAssertion' '(' axiomAnnotations ClassExpression Individual ')'
        /// (argument order is class-expression-then-individual, the opposite of how the English reads)
        /// </summary>
        private OWLClassAssertion ParseClassAssertion()
        {
            Advance(); //'ClassAssertion'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "ClassAssertion");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLClassExpression classExpression = ParseClassExpression();
            OWLIndividualExpression individual = ParseIndividual();
            Expect(OWLFunctionalTokenType.CloseParenthesis, "ClassAssertion");

            //OWLClassAssertion only exposes constructors typed on the concrete individual kind
            OWLClassAssertion axiom = individual is OWLNamedIndividual namedIndividual
                ? new OWLClassAssertion(classExpression, namedIndividual)
                : new OWLClassAssertion(classExpression, (OWLAnonymousIndividual)individual);
            return Annotate(axiom, annotations);
        }

        /// <summary>
        /// ObjectPropertyAssertion := 'ObjectPropertyAssertion' '(' axiomAnnotations ObjectPropertyExpression sourceIndividual targetIndividual ')'
        /// NegativeObjectPropertyAssertion := same shape, opposite polarity
        /// </summary>
        private OWLAssertionAxiom ParseObjectPropertyAssertion(bool negative)
        {
            string keyword = negative ? "NegativeObjectPropertyAssertion" : "ObjectPropertyAssertion";
            Advance(); //keyword
            Expect(OWLFunctionalTokenType.OpenParenthesis, keyword);
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLObjectPropertyExpression objectPropertyExpression = ParseObjectPropertyExpression();
            OWLIndividualExpression sourceIndividual = ParseIndividual();
            OWLIndividualExpression targetIndividual = ParseIndividual();
            Expect(OWLFunctionalTokenType.CloseParenthesis, keyword);

            OWLAssertionAxiom axiom = negative
                ? (OWLAssertionAxiom)new OWLNegativeObjectPropertyAssertion(objectPropertyExpression, sourceIndividual, targetIndividual)
                : new OWLObjectPropertyAssertion(objectPropertyExpression, sourceIndividual, targetIndividual);
            return Annotate(axiom, annotations);
        }

        /// <summary>
        /// DataPropertyAssertion := 'DataPropertyAssertion' '(' axiomAnnotations DataPropertyExpression sourceIndividual targetValue ')'
        /// NegativeDataPropertyAssertion := same shape, opposite polarity
        /// </summary>
        private OWLAssertionAxiom ParseDataPropertyAssertion(bool negative)
        {
            string keyword = negative ? "NegativeDataPropertyAssertion" : "DataPropertyAssertion";
            Advance(); //keyword
            Expect(OWLFunctionalTokenType.OpenParenthesis, keyword);
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLDataProperty dataProperty = ParseDataPropertyExpression();
            OWLIndividualExpression sourceIndividual = ParseIndividual();
            OWLLiteral targetValue = ParseLiteral();
            Expect(OWLFunctionalTokenType.CloseParenthesis, keyword);

            //OWLDataPropertyAssertion/OWLNegativeDataPropertyAssertion only expose constructors typed
            //on the concrete individual kind, same as OWLClassAssertion above
            OWLAssertionAxiom axiom;
            if (negative)
                axiom = sourceIndividual is OWLNamedIndividual namedIndividual
                    ? new OWLNegativeDataPropertyAssertion(dataProperty, namedIndividual, targetValue)
                    : new OWLNegativeDataPropertyAssertion(dataProperty, (OWLAnonymousIndividual)sourceIndividual, targetValue);
            else
                axiom = sourceIndividual is OWLNamedIndividual namedIndividual2
                    ? new OWLDataPropertyAssertion(dataProperty, namedIndividual2, targetValue)
                    : new OWLDataPropertyAssertion(dataProperty, (OWLAnonymousIndividual)sourceIndividual, targetValue);
            return Annotate(axiom, annotations);
        }
        #endregion

        #region Annotation axioms
        /// <summary>
        /// AnnotationAssertion := 'AnnotationAssertion' '(' axiomAnnotations AnnotationProperty AnnotationSubject AnnotationValue ')'
        /// AnnotationSubject := IRI | AnonymousIndividual
        /// (the OWLAnnotationAssertion object model only carries an IRI subject - SubjectIRI - with no
        /// anonymous-individual counterpart, so a nodeID subject is rejected here with a clear diagnostic
        /// rather than silently discarded or mis-parsed)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private OWLAnnotationAssertion ParseAnnotationAssertion()
        {
            Advance(); //'AnnotationAssertion'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "AnnotationAssertion");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLAnnotationProperty annotationProperty = new OWLAnnotationProperty(ParseIRI("AnnotationAssertion"));

            if (Current.Type == OWLFunctionalTokenType.NodeID)
                throw new OWLException("Cannot parse OWL2/Functional document: AnnotationAssertion with an anonymous individual subject is not representable by the OWLSharp object model (OWLAnnotationAssertion only carries an IRI subject)");
            RDFResource subjectIRI = ParseIRI("AnnotationAssertion subject");

            OWLAnnotationAssertion axiom = Current.Type == OWLFunctionalTokenType.QuotedString
                ? new OWLAnnotationAssertion(annotationProperty, subjectIRI, ParseLiteral())
                : new OWLAnnotationAssertion(annotationProperty, subjectIRI, ParseIRI("AnnotationAssertion value"));

            Expect(OWLFunctionalTokenType.CloseParenthesis, "AnnotationAssertion");
            return Annotate(axiom, annotations);
        }

        /// <summary>
        /// SubAnnotationPropertyOf := 'SubAnnotationPropertyOf' '(' axiomAnnotations subAnnotationProperty superAnnotationProperty ')'
        /// </summary>
        private OWLSubAnnotationPropertyOf ParseSubAnnotationPropertyOf()
        {
            Advance(); //'SubAnnotationPropertyOf'
            Expect(OWLFunctionalTokenType.OpenParenthesis, "SubAnnotationPropertyOf");
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLAnnotationProperty subAnnotationProperty = new OWLAnnotationProperty(ParseIRI("SubAnnotationPropertyOf"));
            OWLAnnotationProperty superAnnotationProperty = new OWLAnnotationProperty(ParseIRI("SubAnnotationPropertyOf"));
            Expect(OWLFunctionalTokenType.CloseParenthesis, "SubAnnotationPropertyOf");
            return Annotate(new OWLSubAnnotationPropertyOf(subAnnotationProperty, superAnnotationProperty), annotations);
        }

        /// <summary>
        /// AnnotationPropertyDomain := 'AnnotationPropertyDomain' '(' axiomAnnotations AnnotationProperty IRI ')'
        /// AnnotationPropertyRange := 'AnnotationPropertyRange' '(' axiomAnnotations AnnotationProperty IRI ')'
        /// (the second argument is a bare IRI, not a ClassExpression - it names a domain/range class
        /// only by reference, without allowing composite class expressions at this position)
        /// </summary>
        private OWLAnnotationAxiom ParseAnnotationPropertyDomainOrRange(string keyword)
        {
            Advance(); //keyword
            Expect(OWLFunctionalTokenType.OpenParenthesis, keyword);
            List<OWLAnnotation> annotations = ParseAxiomAnnotations();
            OWLAnnotationProperty annotationProperty = new OWLAnnotationProperty(ParseIRI(keyword));
            RDFResource targetIRI = ParseIRI(keyword);
            Expect(OWLFunctionalTokenType.CloseParenthesis, keyword);

            OWLAnnotationAxiom axiom = keyword == "AnnotationPropertyDomain"
                ? (OWLAnnotationAxiom)new OWLAnnotationPropertyDomain(annotationProperty, targetIRI)
                : new OWLAnnotationPropertyRange(annotationProperty, targetIRI);
            return Annotate(axiom, annotations);
        }
        #endregion

        #endregion

        #endregion
    }
}