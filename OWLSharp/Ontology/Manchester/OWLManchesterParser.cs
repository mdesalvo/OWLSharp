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
using System.Linq;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLManchesterParser materializes an OWLOntology from an OWL2/Manchester document with a two-pass strategy:
    /// pass 1 scans the frame headers to build the symbol table of declared entity kinds (needed because the
    /// Manchester grammar is type-dependent, e.g "p some C" parses differently for object and data properties);
    /// pass 2 executes the recursive-descent parse of the full grammar, one method per grammar production
    /// (https://www.w3.org/TR/owl2-manchester-syntax/)
    /// </summary>
    public sealed class OWLManchesterParser
    {
        #region Statics
        /// <summary>
        /// Defense against DOS attacks which might be conducted by specially crafted long-nested Manchester structures.<br/>
        /// Indicates that, at most, this depth level will be reached during ontology parsing. One more will generate exception.
        /// </summary>
        public static uint MaximumNestingLevel { get; set; } = 25;

        /// <summary>
        /// The Manchester frame keywords, mapped to the kind of entity their headers declare
        /// </summary>
        private static readonly Dictionary<string, OWLManchesterFrameKind> FrameKeywords =
            new Dictionary<string, OWLManchesterFrameKind>
            {
                { "Class:", OWLManchesterFrameKind.Class },
                { "ObjectProperty:", OWLManchesterFrameKind.ObjectProperty },
                { "DataProperty:", OWLManchesterFrameKind.DataProperty },
                { "AnnotationProperty:", OWLManchesterFrameKind.AnnotationProperty },
                { "Datatype:", OWLManchesterFrameKind.Datatype },
                { "Individual:", OWLManchesterFrameKind.Individual }
            };

        /// <summary>
        /// The Manchester keywords introducing an object/data property restriction after a property expression
        /// </summary>
        private static readonly HashSet<string> RestrictionKeywords =
            new HashSet<string> { "some", "only", "value", "min", "max", "exactly", "Self" };

        /// <summary>
        /// The Manchester reserved keywords, which cannot be used as simple names (bare entity names resolved
        /// against the default prefix, e.g "Person" for ":Person")
        /// </summary>
        private static readonly HashSet<string> ReservedNames =
            new HashSet<string>
            {
                "some", "only", "value", "min", "max", "exactly", "Self",
                "and", "or", "not", "that", "inverse", "o",
                "integer", "decimal", "float", "string", "true", "false",
                "Functional", "InverseFunctional", "Reflexive", "Irreflexive", "Symmetric", "Asymmetric", "Transitive"
            };

        /// <summary>
        /// The Manchester characteristics keywords, mapped to their object property axiom factories
        /// </summary>
        private static readonly Dictionary<string, Func<OWLObjectProperty, OWLObjectPropertyAxiom>> ObjectPropertyCharacteristics =
            new Dictionary<string, Func<OWLObjectProperty, OWLObjectPropertyAxiom>>
            {
                { "Functional", objProp => new OWLFunctionalObjectProperty(objProp) },
                { "InverseFunctional", objProp => new OWLInverseFunctionalObjectProperty(objProp) },
                { "Reflexive", objProp => new OWLReflexiveObjectProperty(objProp) },
                { "Irreflexive", objProp => new OWLIrreflexiveObjectProperty(objProp) },
                { "Symmetric", objProp => new OWLSymmetricObjectProperty(objProp) },
                { "Asymmetric", objProp => new OWLAsymmetricObjectProperty(objProp) },
                { "Transitive", objProp => new OWLTransitiveObjectProperty(objProp) }
            };

        /// <summary>
        /// Reverse mapping between Manchester facet keywords/symbols and OWL2 facet IRIs
        /// </summary>
        private static readonly Dictionary<string, string> SymbolFacets =
            OWLManchesterContext.FacetSymbols.ToDictionary(fct => fct.Value, fct => fct.Key);

        /// <summary>
        /// The Manchester built-in datatype shortcut names, mapped to their XSD IRIs
        /// </summary>
        private static readonly Dictionary<string, string> BuiltInDatatypes =
            new Dictionary<string, string>
            {
                { "integer", RDFVocabulary.XSD.INTEGER.ToString() },
                { "decimal", RDFVocabulary.XSD.DECIMAL.ToString() },
                { "float", RDFVocabulary.XSD.FLOAT.ToString() },
                { "string", RDFVocabulary.XSD.STRING.ToString() }
            };
        #endregion

        #region Properties
        /// <summary>
        /// The flat token sequence produced by OWLManchesterLexer, consumed left-to-right by pass 2
        /// </summary>
        private readonly List<OWLManchesterToken> tokens;

        /// <summary>
        /// The prefix-to-IRI map, seeded with the ontology's default prefixes and grown while parsing
        /// "Prefix:" declarations (also pre-populated by BuildSymbolTable, since frame headers may reference
        /// prefixes declared later in the token stream than where they are first looked up)
        /// </summary>
        private readonly Dictionary<string, string> prefixes = new Dictionary<string, string>();

        /// <summary>
        /// The symbol table built by pass 1: maps a declared entity's full IRI to the kind of frame that
        /// declared it, so that pass 2 can resolve the type-dependent parts of the grammar (e.g: whether a
        /// bare property IRI followed by "some" denotes an object or a data property restriction)
        /// </summary>
        private readonly Dictionary<string, OWLManchesterFrameKind> symbols = new Dictionary<string, OWLManchesterFrameKind>();

        /// <summary>
        /// Tracks which (frameKind, entityIRI) pairs already own a declaration axiom, so that an entity
        /// mentioned by multiple frames (or by punning between compatible kinds) is declared only once
        /// </summary>
        private readonly HashSet<string> declaredEntities = new HashSet<string>();

        /// <summary>
        /// The ontology being incrementally populated by pass 2, returned by DeserializeOntology on completion
        /// </summary>
        private readonly OWLOntology ontology = new OWLOntology();

        /// <summary>
        /// The index of the next token to be consumed from the token sequence
        /// </summary>
        private int pos;

        /// <summary>
        /// The current depth of nested-expression recursion, guarded by EnterRecursion/ExitRecursion against
        /// stack exhaustion from adversarially deep-nested OWL2/Manchester documents
        /// </summary>
        private int nestingLevel;
        #endregion

        #region Ctors
        private OWLManchesterParser(List<OWLManchesterToken> tokens)
        {
            this.tokens = tokens;

            //Seed the prefix map with the ontology's default prefixes (rdf, rdfs, xsd, owl, ...)
            foreach (OWLPrefix prefix in ontology.Prefixes)
                prefixes[prefix.Name] = prefix.IRI;
        }
        #endregion

        #region Methods

        #region Entry point
        /// <summary>
        /// Deserializes the given OWL2/Manchester document to an OWLOntology object
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal static OWLOntology DeserializeOntology(string manDocument)
        {
            //Tokenize the raw document once; parser instance keeps the resulting flat token sequence for both passes
            OWLManchesterParser parser = new OWLManchesterParser(OWLManchesterLexer.Tokenize(manDocument));

            //Pass 1: pre-scan frame headers so pass 2 already knows each entity's kind, even for forward references
            parser.BuildSymbolTable();

            //Pass 2: recursive-descent parse of the full grammar, populating parser.ontology as it goes
            parser.ParseDocument();

            return parser.ontology;
        }
        #endregion

        #region Recursion guard
        /// <summary>
        /// Enters a nested-expression production (class/data range primary, or nested annotations block), failing
        /// fast once the maximum allowed recursion depth is exceeded: protects against stack exhaustion
        /// from adversarially deep-nested documents (e.g: long "not"/parenthesized chains, or "p some p some ..."
        /// property restriction chains), which would otherwise recurse as deep as the input allows
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private void EnterRecursion(string production)
        {
            if (++nestingLevel > MaximumNestingLevel)
                throw new OWLException($"Cannot parse OWL2/Manchester document: exceeded the maximum allowed nesting depth ({MaximumNestingLevel}) while parsing {production}");
        }

        /// <summary>
        /// Leaves a nested-expression production entered via EnterRecursion
        /// </summary>
        private void ExitRecursion()
            => nestingLevel--;
        #endregion

        #region Token utilities
        /// <summary>
        /// The token at the current parse position, not yet consumed
        /// </summary>
        private OWLManchesterToken Current => tokens[pos];

        /// <summary>
        /// Looks ahead the given number of positions without consuming any token
        /// (clamped to the last token, which is always EndOfDocument, so lookahead never runs off the sequence)
        /// </summary>
        private OWLManchesterToken Peek(int lookAhead)
            => pos + lookAhead < tokens.Count ? tokens[pos + lookAhead] : tokens[tokens.Count - 1];

        /// <summary>
        /// Consumes and returns the current token, moving the parse position forward
        /// </summary>
        private OWLManchesterToken Advance()
            => tokens[pos++];

        /// <summary>
        /// Consumes the current token if it matches the given type, otherwise fails with a diagnostic
        /// naming the grammar production being parsed
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private OWLManchesterToken Expect(OWLManchesterTokenType tokenType, string production)
            => Current.Type == tokenType
                ? Advance()
                : throw new OWLException($"Cannot parse OWL2/Manchester document: expected {tokenType} while parsing {production}, found {Current}");

        /// <summary>
        /// Tells if the current token is the given section keyword (e.g: "SubClassOf:"), without consuming it
        /// </summary>
        private bool CurrentIsSection(string sectionKeyword)
            => Current.Type == OWLManchesterTokenType.SectionKeyword && string.Equals(Current.Value, sectionKeyword, StringComparison.Ordinal);

        /// <summary>
        /// Tells if the current token is the given bare keyword (e.g: "some", "and"), without consuming it
        /// </summary>
        private bool CurrentIsName(string name)
            => Current.Type == OWLManchesterTokenType.Name && string.Equals(Current.Value, name, StringComparison.Ordinal);

        /// <summary>
        /// Tells if the current token can reference an entity (see IsEntityToken), without consuming it
        /// </summary>
        private bool CurrentIsEntityIRI()
            => IsEntityToken(Current);

        /// <summary>
        /// Tells if the given token can reference an entity: a full IRI, a prefixed name,
        /// or a simple name (bare non-reserved word resolved against the default prefix)
        /// </summary>
        private static bool IsEntityToken(OWLManchesterToken token)
            => token.Type == OWLManchesterTokenType.FullIRI
                || token.Type == OWLManchesterTokenType.PrefixedName
                || (token.Type == OWLManchesterTokenType.Name && !ReservedNames.Contains(token.Value));

        /// <summary>
        /// Consumes the current token if it is a comma, reporting whether it did (used to drive the
        /// "(',' item)*" tail of every comma-separated grammar list)
        /// </summary>
        private bool TryConsumeComma()
        {
            if (Current.Type == OWLManchesterTokenType.Comma)
            {
                Advance();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resolves the given FullIRI/PrefixedName token to its full IRI string
        /// </summary>
        private string ResolveIRI(OWLManchesterToken token)
        {
            if (token.Type == OWLManchesterTokenType.FullIRI)
                return token.Value;

            //Simple names resolve against the default prefix
            int colonIdx = token.Type == OWLManchesterTokenType.Name ? -1 : token.Value.IndexOf(':');
            string prefixName = colonIdx == -1 ? string.Empty : token.Value.Substring(0, colonIdx);
            string localName = token.Value.Substring(colonIdx + 1);
            return prefixes.TryGetValue(prefixName, out string prefixIRI)
                ? string.Concat(prefixIRI, localName)
                : throw new OWLException($"Cannot parse OWL2/Manchester document: undeclared prefix in {token}");
        }

        /// <summary>
        /// Consumes the current FullIRI/PrefixedName token and resolves it to its full IRI string
        /// </summary>
        private string ParseEntityIRI(string production)
            => CurrentIsEntityIRI()
                ? ResolveIRI(Advance())
                : throw new OWLException($"Cannot parse OWL2/Manchester document: expected IRI while parsing {production}, found {Current}");
        #endregion

        #region Pass 1 (symbol table)
        /// <summary>
        /// Scans the token sequence for prefix declarations and frame headers, building the symbol table
        /// of declared entity kinds which drives the type-dependent grammar decisions of pass 2
        /// </summary>
        private void BuildSymbolTable()
        {
            #region Utilities
            bool IsPropertyKind(OWLManchesterFrameKind kind)
                => kind == OWLManchesterFrameKind.ObjectProperty
                    || kind == OWLManchesterFrameKind.DataProperty
                    || kind == OWLManchesterFrameKind.AnnotationProperty;
            #endregion

            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i].Type != OWLManchesterTokenType.SectionKeyword)
                    continue;

                //Prefix declarations must be registered on the fly, since frame headers may use them
                if (string.Equals(tokens[i].Value, "Prefix:", StringComparison.Ordinal)
                     && tokens[i + 1].Type == OWLManchesterTokenType.SectionKeyword
                     && i + 2 < tokens.Count
                     && tokens[i + 2].Type == OWLManchesterTokenType.FullIRI)
                {
                    prefixes[tokens[i + 1].Value.TrimEnd(':')] = tokens[i + 2].Value;
                    i += 2;
                }

                //Frame headers declare the kind of their owning entity
                else if (FrameKeywords.TryGetValue(tokens[i].Value, out OWLManchesterFrameKind frameKind)
                          && IsEntityToken(tokens[i + 1]))
                {
                    string entityIRI = ResolveIRI(tokens[i + 1]);

                    //In case of punning, property kinds win over the others, since only property kinds
                    //drive grammar decisions (and OWL2 forbids punning between property kinds themselves)
                    if (!symbols.TryGetValue(entityIRI, out OWLManchesterFrameKind existingKind)
                         || (IsPropertyKind(frameKind) && !IsPropertyKind(existingKind)))
                    {
                        symbols[entityIRI] = frameKind;
                    }

                    i++;
                }
            }
        }
        #endregion

        #region Pass 2 (document)
        /// <summary>
        /// document := prefixDeclaration* 'Ontology:' [iri [versionIri]] (import | ontologyAnnotations)* (frame | misc)*
        /// </summary>
        private void ParseDocument()
        {
            //Consume every leading "Prefix:" declaration first, since anything after may reference one of them
            while (CurrentIsSection("Prefix:"))
                ParsePrefixDeclaration();

            //The whole "Ontology:" header is optional, and so is each of its own IRI/versionIRI arguments
            if (CurrentIsSection("Ontology:"))
            {
                Advance(); //Ontology:
                if (CurrentIsEntityIRI())
                {
                    ontology.IRI = ResolveIRI(Advance());
                    //A version IRI can only follow an ontology IRI, never appear on its own
                    if (CurrentIsEntityIRI())
                        ontology.VersionIRI = ResolveIRI(Advance());
                }
            }

            //Imports and ontology-level annotations can be freely interleaved in any order, so loop until neither matches
            while (true)
            {
                if (CurrentIsSection("Import:"))
                {
                    Advance(); //Import:
                    ontology.Imports.Add(new OWLImport(new RDFResource(ParseEntityIRI("Import"))));
                }
                else if (CurrentIsSection("Annotations:"))
                {
                    Advance(); //Annotations:
                    do
                    {
                        //A leading nested "Annotations:" block, if present, becomes this annotation's own meta-annotation
                        List<OWLAnnotation> nestedAnnotations = TryParseAnnotationBlock();
                        ontology.Annotations.Add(NestAnnotations(ParseAnnotation(), nestedAnnotations));
                    } while (TryConsumeComma());
                }
                else
                    break; //Neither an import nor an annotations block: the document header is over
            }

            //From here on, every remaining token starts either a frame (an entity and its axioms) or a
            //frame-less misc section, repeated until the token stream is fully consumed
            while (Current.Type != OWLManchesterTokenType.EndOfDocument)
            {
                //Every frame/misc section starts with one of the section keywords handled below
                if (Current.Type != OWLManchesterTokenType.SectionKeyword)
                    throw new OWLException($"Cannot parse OWL2/Manchester document: expected frame or misc section, found {Current}");

                //Each keyword dispatches to its own dedicated parsing method, which advances past the keyword itself
                switch (Current.Value)
                {
                    case "Class:": ParseClassFrame(); break;
                    case "ObjectProperty:": ParseObjectPropertyFrame(); break;
                    case "DataProperty:": ParseDataPropertyFrame(); break;
                    case "AnnotationProperty:": ParseAnnotationPropertyFrame(); break;
                    case "Datatype:": ParseDatatypeFrame(); break;
                    case "Individual:": ParseIndividualFrame(); break;
                    case "EquivalentClasses:": ParseMiscClassesSection(equivalent: true); break;
                    case "DisjointClasses:": ParseMiscClassesSection(equivalent: false); break;
                    case "EquivalentProperties:": ParseMiscPropertiesSection(equivalent: true); break;
                    case "DisjointProperties:": ParseMiscPropertiesSection(equivalent: false); break;
                    case "SameIndividual:": ParseMiscIndividualsSection(same: true); break;
                    case "DifferentIndividuals:": ParseMiscIndividualsSection(same: false); break;
                    //Any other section keyword is unknown at document scope (frame bodies handle their own set)
                    default:
                        throw new OWLException($"Cannot parse OWL2/Manchester document: unexpected section keyword {Current}");
                }
            }
        }

        /// <summary>
        /// prefixDeclaration := 'Prefix:' prefixName ':' fullIRI
        /// </summary>
        private void ParsePrefixDeclaration()
        {
            Advance(); //Prefix:
            //The prefix name is lexed as a "name:" section keyword (e.g: "pz:"), not as a bare Name token
            OWLManchesterToken nameToken = Expect(OWLManchesterTokenType.SectionKeyword, "Prefix declaration");
            string prefixName = nameToken.Value.TrimEnd(':');
            string prefixIRI = Expect(OWLManchesterTokenType.FullIRI, "Prefix declaration").Value;

            //Registered in the resolution map right away, since IRIs later in the document may already need it
            prefixes[prefixName] = prefixIRI;
            //The empty/default prefix (":") only serves local IRI resolution and is never added to the ontology's
            //own prefix list; a prefix redeclared later in the document is likewise skipped, to avoid a duplicate entry
            if (prefixName.Length > 0 && !ontology.Prefixes.Any(pfx => string.Equals(pfx.Name, prefixName, StringComparison.Ordinal)))
                ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace(prefixName, prefixIRI)));
        }
        #endregion

        #region Frames
        /// <summary>
        /// classFrame := 'Class:' classIRI (annotationsSection | 'SubClassOf:' | 'EquivalentTo:' | 'DisjointWith:' | 'DisjointUnionOf:' | 'HasKey:')*
        /// </summary>
        private void ParseClassFrame()
        {
            Advance(); //Class:
            RDFResource clsIRI = new RDFResource(ParseEntityIRI("Class frame"));
            OWLClass cls = new OWLClass(clsIRI);
            //Frames declare their owning entity; deduplicated internally, so punning across compatible frames is safe
            AddDeclaration(OWLManchesterFrameKind.Class, clsIRI);

            //Loop until a section keyword doesn't belong to this frame's grammar (falls through to "default: return")
            while (Current.Type == OWLManchesterTokenType.SectionKeyword)
            {
                switch (Current.Value)
                {
                    case "Annotations:":
                        ParseEntityAnnotationsSection(clsIRI);
                        break;

                    //Each of the 3 sections below allows a comma-separated list of items, one axiom per item,
                    //each with its own independent (possibly empty) set of annotations
                    case "SubClassOf:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLSubClassOf(cls, ParseDescription()), annotations, ontology.ClassAxioms));
                        break;

                    case "EquivalentTo:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLEquivalentClasses(new List<OWLClassExpression> { cls, ParseDescription() }), annotations, ontology.ClassAxioms));
                        break;

                    case "DisjointWith:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLDisjointClasses(new List<OWLClassExpression> { cls, ParseDescription() }), annotations, ontology.ClassAxioms));
                        break;

                    case "DisjointUnionOf:":
                    {
                        //Unlike the sections above, the whole comma-separated list here produces a SINGLE axiom,
                        //so its annotations are read once upfront rather than once per list item via ParseAnnotatedList
                        Advance();
                        List<OWLAnnotation> annotations = TryParseAnnotationBlock();
                        List<OWLClassExpression> disjointClasses = new List<OWLClassExpression>();
                        do
                        {
                            disjointClasses.Add(ParseDescription());
                        } while (TryConsumeComma());
                        AddAxiom(new OWLDisjointUnion(cls, disjointClasses), annotations, ontology.ClassAxioms);
                        break;
                    }

                    case "HasKey:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                        {
                            List<OWLObjectPropertyExpression> keyObjectProperties = new List<OWLObjectPropertyExpression>();
                            List<OWLDataProperty> keyDataProperties = new List<OWLDataProperty>();
                            //HasKey's member list has no comma separators between properties: each property is
                            //recognized by starting a new IRI/"inverse" token, so the loop keeps consuming until neither matches
                            while (CurrentIsEntityIRI() || CurrentIsName("inverse"))
                            {
                                if (CurrentIsName("inverse"))
                                    keyObjectProperties.Add(ParseObjectPropertyExpression());
                                else
                                {
                                    string keyPropertyIRI = ParseEntityIRI("HasKey");
                                    symbols.TryGetValue(keyPropertyIRI, out OWLManchesterFrameKind keyPropertyKind);
                                    //Only a symbol table hit for DataProperty routes to the data list; anything else
                                    //(including undeclared properties) defaults to object property
                                    if (keyPropertyKind == OWLManchesterFrameKind.DataProperty)
                                        keyDataProperties.Add(new OWLDataProperty(new RDFResource(keyPropertyIRI)));
                                    else
                                        keyObjectProperties.Add(new OWLObjectProperty(new RDFResource(keyPropertyIRI)));
                                }
                            }
                            AddAxiom(new OWLHasKey(cls, keyObjectProperties) { DataProperties = keyDataProperties }, annotations, ontology.KeyAxioms);
                        });
                        break;

                    //An unrecognized section keyword means this frame is over: return without consuming it, so
                    //ParseDocument's own loop can dispatch it as the next frame or misc section
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// objectPropertyFrame := 'ObjectProperty:' objectPropertyIRI (annotationsSection | 'Domain:' | 'Range:' | 'Characteristics:'
        ///                         | 'SubPropertyOf:' | 'EquivalentTo:' | 'DisjointWith:' | 'InverseOf:' | 'SubPropertyChain:')*
        /// </summary>
        private void ParseObjectPropertyFrame()
        {
            Advance(); //ObjectProperty:
            RDFResource objPropIRI = new RDFResource(ParseEntityIRI("ObjectProperty frame"));
            OWLObjectProperty objProp = new OWLObjectProperty(objPropIRI);
            AddDeclaration(OWLManchesterFrameKind.ObjectProperty, objPropIRI);

            //Loop until a section keyword doesn't belong to this frame's grammar (falls through to "default: return")
            while (Current.Type == OWLManchesterTokenType.SectionKeyword)
            {
                switch (Current.Value)
                {
                    case "Annotations:":
                        ParseEntityAnnotationsSection(objPropIRI);
                        break;

                    case "Domain:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLObjectPropertyDomain(objProp, ParseDescription()), annotations, ontology.ObjectPropertyAxioms));
                        break;

                    case "Range:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLObjectPropertyRange(objProp, ParseDescription()), annotations, ontology.ObjectPropertyAxioms));
                        break;

                    case "Characteristics:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                        {
                            //Only the fixed set of OWL2 object property characteristics is legal here (Functional,
                            //InverseFunctional, Reflexive, ...); anything else is rejected against the lookup table
                            OWLManchesterToken characteristicToken = Expect(OWLManchesterTokenType.Name, "Characteristics");
                            if (!ObjectPropertyCharacteristics.TryGetValue(characteristicToken.Value, out Func<OWLObjectProperty, OWLObjectPropertyAxiom> axiomFactory))
                                throw new OWLException($"Cannot parse OWL2/Manchester document: unknown object property characteristic {characteristicToken}");
                            AddAxiom(axiomFactory(objProp), annotations, ontology.ObjectPropertyAxioms);
                        });
                        break;

                    case "SubPropertyOf:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLSubObjectPropertyOf(objProp, ParseObjectPropertyExpression()), annotations, ontology.ObjectPropertyAxioms));
                        break;

                    case "EquivalentTo:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLEquivalentObjectProperties(new List<OWLObjectPropertyExpression> { objProp, ParseObjectPropertyExpression() }), annotations, ontology.ObjectPropertyAxioms));
                        break;

                    case "DisjointWith:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLDisjointObjectProperties(new List<OWLObjectPropertyExpression> { objProp, ParseObjectPropertyExpression() }), annotations, ontology.ObjectPropertyAxioms));
                        break;

                    case "InverseOf:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLInverseObjectProperties(objProp, ParseObjectPropertyExpression()), annotations, ontology.ObjectPropertyAxioms));
                        break;

                    case "SubPropertyChain:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                        {
                            //A chain is one or more property expressions composed with the infix "o" operator
                            //(role composition, e.g: "hasParent o hasParent" for "hasGrandparent"); flat loop, not
                            //recursion, since the chain's length doesn't nest expressions inside one another
                            List<OWLObjectPropertyExpression> chainProperties = new List<OWLObjectPropertyExpression> { ParseObjectPropertyExpression() };
                            while (CurrentIsName("o"))
                            {
                                Advance();
                                chainProperties.Add(ParseObjectPropertyExpression());
                            }
                            AddAxiom(new OWLSubObjectPropertyOf(new OWLObjectPropertyChain(chainProperties), objProp), annotations, ontology.ObjectPropertyAxioms);
                        });
                        break;

                    //An unrecognized section keyword means this frame is over: return without consuming it, so
                    //ParseDocument's own loop can dispatch it as the next frame or misc section
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// dataPropertyFrame := 'DataProperty:' dataPropertyIRI (annotationsSection | 'Domain:' | 'Range:' | 'Characteristics:'
        ///                       | 'SubPropertyOf:' | 'EquivalentTo:' | 'DisjointWith:')*
        /// </summary>
        private void ParseDataPropertyFrame()
        {
            Advance(); //DataProperty:
            RDFResource dtPropIRI = new RDFResource(ParseEntityIRI("DataProperty frame"));
            OWLDataProperty dtProp = new OWLDataProperty(dtPropIRI);
            AddDeclaration(OWLManchesterFrameKind.DataProperty, dtPropIRI);

            //Loop until a section keyword doesn't belong to this frame's grammar (falls through to "default: return")
            while (Current.Type == OWLManchesterTokenType.SectionKeyword)
            {
                switch (Current.Value)
                {
                    case "Annotations:":
                        ParseEntityAnnotationsSection(dtPropIRI);
                        break;

                    //Unlike its object property counterpart, the range here is a data range (a datatype, not a class)
                    case "Domain:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLDataPropertyDomain(dtProp, ParseDescription()), annotations, ontology.DataPropertyAxioms));
                        break;

                    case "Range:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLDataPropertyRange(dtProp, ParseDataRange()), annotations, ontology.DataPropertyAxioms));
                        break;

                    case "Characteristics:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                        {
                            //Data properties support only "Functional" (they can't be Reflexive/Symmetric/... like
                            //object properties can), so this checks a single literal instead of a lookup table
                            OWLManchesterToken characteristicToken = Expect(OWLManchesterTokenType.Name, "Characteristics");
                            if (!string.Equals(characteristicToken.Value, "Functional", StringComparison.Ordinal))
                                throw new OWLException($"Cannot parse OWL2/Manchester document: unknown data property characteristic {characteristicToken}");
                            AddAxiom(new OWLFunctionalDataProperty(dtProp), annotations, ontology.DataPropertyAxioms);
                        });
                        break;

                    //The 3 sections below take a bare data property IRI, unlike their object property counterparts,
                    //since data properties have no "inverse" variant and their expressions are never composite
                    case "SubPropertyOf:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLSubDataPropertyOf(dtProp, new OWLDataProperty(new RDFResource(ParseEntityIRI("SubPropertyOf")))), annotations, ontology.DataPropertyAxioms));
                        break;

                    case "EquivalentTo:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLEquivalentDataProperties(new List<OWLDataProperty> { dtProp, new OWLDataProperty(new RDFResource(ParseEntityIRI("EquivalentTo"))) }), annotations, ontology.DataPropertyAxioms));
                        break;

                    case "DisjointWith:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLDisjointDataProperties(new List<OWLDataProperty> { dtProp, new OWLDataProperty(new RDFResource(ParseEntityIRI("DisjointWith"))) }), annotations, ontology.DataPropertyAxioms));
                        break;

                    //An unrecognized section keyword means this frame is over: return without consuming it, so
                    //ParseDocument's own loop can dispatch it as the next frame or misc section
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// annotationPropertyFrame := 'AnnotationProperty:' annotationPropertyIRI (annotationsSection | 'Domain:' | 'Range:' | 'SubPropertyOf:')*
        /// </summary>
        private void ParseAnnotationPropertyFrame()
        {
            Advance(); //AnnotationProperty:
            RDFResource annPropIRI = new RDFResource(ParseEntityIRI("AnnotationProperty frame"));
            OWLAnnotationProperty annProp = new OWLAnnotationProperty(annPropIRI);
            AddDeclaration(OWLManchesterFrameKind.AnnotationProperty, annPropIRI);

            //Loop until a section keyword doesn't belong to this frame's grammar (falls through to "default: return")
            while (Current.Type == OWLManchesterTokenType.SectionKeyword)
            {
                switch (Current.Value)
                {
                    case "Annotations:":
                        ParseEntityAnnotationsSection(annPropIRI);
                        break;

                    //Domain/Range here take a bare IRI rather than a class/property expression: annotation
                    //properties are metadata-only and have no expressive grammar of their own to resolve into
                    case "Domain:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLAnnotationPropertyDomain(annProp, new RDFResource(ParseEntityIRI("Domain"))), annotations, ontology.AnnotationAxioms));
                        break;

                    case "Range:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLAnnotationPropertyRange(annProp, new RDFResource(ParseEntityIRI("Range"))), annotations, ontology.AnnotationAxioms));
                        break;

                    case "SubPropertyOf:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLSubAnnotationPropertyOf(annProp, new OWLAnnotationProperty(new RDFResource(ParseEntityIRI("SubPropertyOf")))), annotations, ontology.AnnotationAxioms));
                        break;

                    //An unrecognized section keyword means this frame is over: return without consuming it, so
                    //ParseDocument's own loop can dispatch it as the next frame or misc section
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// datatypeFrame := 'Datatype:' datatypeIRI (annotationsSection | 'EquivalentTo:')*
        /// </summary>
        private void ParseDatatypeFrame()
        {
            Advance(); //Datatype:
            RDFResource dtIRI = new RDFResource(ParseEntityIRI("Datatype frame"));
            OWLDatatype dt = new OWLDatatype(dtIRI);
            AddDeclaration(OWLManchesterFrameKind.Datatype, dtIRI);

            //A custom datatype supports only annotations plus a single "EquivalentTo:" (its definition as a data range)
            while (Current.Type == OWLManchesterTokenType.SectionKeyword)
            {
                switch (Current.Value)
                {
                    case "Annotations:":
                        ParseEntityAnnotationsSection(dtIRI);
                        break;

                    case "EquivalentTo:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLDatatypeDefinition(dt, ParseDataRange()), annotations, ontology.DatatypeDefinitionAxioms));
                        break;

                    //An unrecognized section keyword means this frame is over: return without consuming it, so
                    //ParseDocument's own loop can dispatch it as the next frame or misc section
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// individualFrame := 'Individual:' individual (annotationsSection | 'Types:' | 'Facts:' | 'SameAs:' | 'DifferentFrom:')*
        /// </summary>
        private void ParseIndividualFrame()
        {
            Advance(); //Individual:
            //The frame's own subject can be a named or an anonymous individual; only named ones get a declaration
            //axiom, since anonymous individuals aren't formally "declared" entities in the OWL2 sense
            OWLIndividualExpression idvExpr = ParseIndividual();
            if (idvExpr is OWLNamedIndividual namedIdv)
                AddDeclaration(OWLManchesterFrameKind.Individual, namedIdv.GetIRI());

            //Loop until a section keyword doesn't belong to this frame's grammar (falls through to "default: return")
            while (Current.Type == OWLManchesterTokenType.SectionKeyword)
            {
                switch (Current.Value)
                {
                    case "Annotations:":
                        ParseEntityAnnotationsSection(idvExpr.GetIRI());
                        break;

                    case "Types:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                        {
                            OWLClassExpression clsExpr = ParseDescription();
                            //OWLClassAssertion is overloaded on the individual's concrete type (named vs
                            //anonymous), so the frame's own individual is downcast accordingly here
                            OWLClassAssertion clsAsn = idvExpr is OWLNamedIndividual named
                                ? new OWLClassAssertion(clsExpr, named)
                                : new OWLClassAssertion(clsExpr, (OWLAnonymousIndividual)idvExpr);
                            AddAxiom(clsAsn, annotations, ontology.AssertionAxioms);
                        });
                        break;

                    //Each fact in the list is self-contained (own property + own target), so the per-item handler
                    //just forwards to ParseFact rather than building the axiom inline like the other cases here
                    case "Facts:":
                        Advance();
                        ParseAnnotatedList(annotations => ParseFact(idvExpr, annotations));
                        break;

                    case "SameAs:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLSameIndividual(new List<OWLIndividualExpression> { idvExpr, ParseIndividual() }), annotations, ontology.AssertionAxioms));
                        break;

                    case "DifferentFrom:":
                        Advance();
                        ParseAnnotatedList(annotations =>
                            AddAxiom(new OWLDifferentIndividuals(new List<OWLIndividualExpression> { idvExpr, ParseIndividual() }), annotations, ontology.AssertionAxioms));
                        break;

                    //An unrecognized section keyword means this frame is over: return without consuming it, so
                    //ParseDocument's own loop can dispatch it as the next frame or misc section
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// fact := ['not'] (objectPropertyIRI individual | dataPropertyIRI literal)
        /// </summary>
        private void ParseFact(OWLIndividualExpression idvExpr, List<OWLAnnotation> annotations)
        {
            //A leading "not" turns the assertion into its negative counterpart further below
            bool negative = false;
            if (CurrentIsName("not"))
            {
                negative = true;
                Advance();
            }

            string propertyIRI = ParseEntityIRI("Facts");
            //The property kind, looked up in the symbol table, decides whether what follows is an individual
            //(object property fact) or a literal (data property fact): the grammar itself is ambiguous here
            symbols.TryGetValue(propertyIRI, out OWLManchesterFrameKind propertyKind);
            switch (propertyKind)
            {
                case OWLManchesterFrameKind.ObjectProperty:
                {
                    OWLObjectProperty objProp = new OWLObjectProperty(new RDFResource(propertyIRI));
                    OWLIndividualExpression targetIdvExpr = ParseIndividual();
                    AddAxiom(negative
                        ? (OWLAssertionAxiom)new OWLNegativeObjectPropertyAssertion(objProp, idvExpr, targetIdvExpr)
                        : new OWLObjectPropertyAssertion(objProp, idvExpr, targetIdvExpr), annotations, ontology.AssertionAxioms);
                    break;
                }

                case OWLManchesterFrameKind.DataProperty:
                {
                    OWLDataProperty dtProp = new OWLDataProperty(new RDFResource(propertyIRI));
                    OWLLiteral targetLiteral = ParseLiteral();
                    //Assertion/NegativeAssertion axioms are typed on the subject individual's own class
                    //(OWLNamedIndividual vs OWLAnonymousIndividual), hence the two-way branch on both axes
                    OWLAssertionAxiom dtAsn = idvExpr is OWLNamedIndividual named
                        ? negative ? (OWLAssertionAxiom)new OWLNegativeDataPropertyAssertion(dtProp, named, targetLiteral)
                                   : new OWLDataPropertyAssertion(dtProp, named, targetLiteral)
                        : negative ? (OWLAssertionAxiom)new OWLNegativeDataPropertyAssertion(dtProp, (OWLAnonymousIndividual)idvExpr, targetLiteral)
                                   : new OWLDataPropertyAssertion(dtProp, (OWLAnonymousIndividual)idvExpr, targetLiteral);
                    AddAxiom(dtAsn, annotations, ontology.AssertionAxioms);
                    break;
                }

                //A fact's property must be declared, and declared as exactly one of the two kinds handled above
                default:
                    throw new OWLException($"Cannot parse OWL2/Manchester document: property <{propertyIRI}> used in Facts section is not declared as object or data property");
            }
        }
        #endregion

        #region Misc sections
        /// <summary>
        /// misc := ('EquivalentClasses:' | 'DisjointClasses:') annotations? description2List
        /// </summary>
        private void ParseMiscClassesSection(bool equivalent)
        {
            Advance(); //EquivalentClasses: or DisjointClasses:
            //Unlike a Class frame's own "EquivalentTo:"/"DisjointWith:", this whole comma-separated member list
            //produces a SINGLE axiom, so its annotations are read once upfront rather than once per member
            List<OWLAnnotation> annotations = TryParseAnnotationBlock();
            List<OWLClassExpression> clsExprs = new List<OWLClassExpression>();
            do
            {
                clsExprs.Add(ParseDescription());
            } while (TryConsumeComma());

            //The "equivalent" flag picked in ParseDocument's own switch selects which of the two axiom types to build
            AddAxiom(equivalent
                ? (OWLClassAxiom)new OWLEquivalentClasses(clsExprs)
                : new OWLDisjointClasses(clsExprs), annotations, ontology.ClassAxioms);
        }

        /// <summary>
        /// misc := ('EquivalentProperties:' | 'DisjointProperties:') annotations? property2List
        /// (the kind of the member properties, looked up in the symbol table, selects the object/data variant of the axiom)
        /// </summary>
        private void ParseMiscPropertiesSection(bool equivalent)
        {
            //Token kept (rather than discarded like elsewhere) so it can name the section in the error message below
            OWLManchesterToken sectionToken = Advance();
            List<OWLAnnotation> annotations = TryParseAnnotationBlock();
            List<(bool IsInverse, string IRI)> members = new List<(bool, string)>();
            do
            {
                //Each member may optionally be an inverse object property reference ("inverse hasTopping")
                bool isInverse = CurrentIsName("inverse");
                if (isInverse)
                    Advance();
                members.Add((isInverse, ParseEntityIRI("EquivalentProperties/DisjointProperties")));
            } while (TryConsumeComma());

            //The section is untyped in the grammar (unlike EquivalentProperties/DisjointProperties frames, which
            //are nested under a typed property frame), so the variant is inferred from any member's symbol table
            //kind: an "inverse" member forces the object variant outright, since data properties have no inverse
            bool isObjectVariant = members.Any(member => member.IsInverse
                || (symbols.TryGetValue(member.IRI, out OWLManchesterFrameKind memberKind) && memberKind == OWLManchesterFrameKind.ObjectProperty));
            bool isDataVariant = members.Any(member => symbols.TryGetValue(member.IRI, out OWLManchesterFrameKind memberKind) && memberKind == OWLManchesterFrameKind.DataProperty);
            //Fails both when no member resolves to a known kind and when members mix incompatible kinds
            //(both booleans false or both true)
            if (isObjectVariant == isDataVariant)
                throw new OWLException($"Cannot parse OWL2/Manchester document: cannot determine whether members of section {sectionToken} are object or data properties");

            //Rebuild each member into its resolved expression type, now that the variant is settled
            if (isObjectVariant)
            {
                List<OWLObjectPropertyExpression> objPropExprs = members.Select(member => member.IsInverse
                    ? (OWLObjectPropertyExpression)new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource(member.IRI)))
                    : new OWLObjectProperty(new RDFResource(member.IRI))).ToList();
                AddAxiom(equivalent
                    ? (OWLObjectPropertyAxiom)new OWLEquivalentObjectProperties(objPropExprs)
                    : new OWLDisjointObjectProperties(objPropExprs), annotations, ontology.ObjectPropertyAxioms);
            }
            else
            {
                List<OWLDataProperty> dtProps = members.Select(member => new OWLDataProperty(new RDFResource(member.IRI))).ToList();
                AddAxiom(equivalent
                    ? (OWLDataPropertyAxiom)new OWLEquivalentDataProperties(dtProps)
                    : new OWLDisjointDataProperties(dtProps), annotations, ontology.DataPropertyAxioms);
            }
        }

        /// <summary>
        /// misc := ('SameIndividual:' | 'DifferentIndividuals:') annotations? individual2List
        /// </summary>
        private void ParseMiscIndividualsSection(bool same)
        {
            Advance(); //SameIndividual: or DifferentIndividuals:
            //As with the misc classes/properties sections above, the whole member list shares one set of
            //annotations for the single resulting axiom
            List<OWLAnnotation> annotations = TryParseAnnotationBlock();
            List<OWLIndividualExpression> idvExprs = new List<OWLIndividualExpression>();
            do
            {
                idvExprs.Add(ParseIndividual());
            } while (TryConsumeComma());

            AddAxiom(same
                ? (OWLAssertionAxiom)new OWLSameIndividual(idvExprs)
                : new OWLDifferentIndividuals(idvExprs), annotations, ontology.AssertionAxioms);
        }
        #endregion

        #region Annotations
        /// <summary>
        /// annotationsSection := 'Annotations:' annotationAnnotatedList (each item becomes an OWLAnnotationAssertion on the frame's entity)
        /// </summary>
        private void ParseEntityAnnotationsSection(RDFResource subjectIRI)
        {
            Advance(); //Annotations:
            ParseAnnotatedList(annotations =>
            {
                //The property + value pair that this specific list item actually asserts
                OWLAnnotation annotation = ParseAnnotation();
                //OWLAnnotationAssertion has no single constructor covering all 3 possible value shapes, so the
                //literal case is picked first, falling back to whichever of anonymous individual / IRI is set
                OWLAnnotationAssertion annAsn = annotation.ValueLiteral != null
                    ? new OWLAnnotationAssertion(annotation.AnnotationProperty, subjectIRI, annotation.ValueLiteral)
                    : new OWLAnnotationAssertion(annotation.AnnotationProperty, subjectIRI,
                        annotation.ValueAnonymousIndividual?.GetIRI() ?? new RDFResource(annotation.ValueIRI));
                //"annotations" here are meta-annotations on the resulting axiom itself (from the list item's own
                //leading "Annotations:" block, if any), distinct from the "annotation" value parsed just above
                AddAxiom(annAsn, annotations, ontology.AnnotationAxioms);
            });
        }

        /// <summary>
        /// annotations := 'Annotations:' annotationAnnotatedList (returns the empty list if no block is present at the current position)
        /// </summary>
        private List<OWLAnnotation> TryParseAnnotationBlock()
        {
            List<OWLAnnotation> annotations = new List<OWLAnnotation>();
            //No "Annotations:" here means there's simply nothing to parse at this position: not an error
            if (!CurrentIsSection("Annotations:"))
                return annotations;

            //Only reached when a block is actually present, so a chain of nested blocks counts towards the cap
            EnterRecursion("nested annotations block");
            try
            {
                Advance(); //Annotations:
                while (true)
                {
                    //A further nested "Annotations:" block, if present, becomes THIS annotation's own meta-annotation
                    List<OWLAnnotation> nestedAnnotations = TryParseAnnotationBlock();
                    annotations.Add(NestAnnotations(ParseAnnotation(), nestedAnnotations));

                    //The comma is consumed only if what follows still parses as an annotation: this resolves the
                    //grammar ambiguity between "next annotation of this block" and "next item of the enclosing list"
                    if (Current.Type == OWLManchesterTokenType.Comma && IsAnnotationStart(Peek(1), Peek(2)))
                        Advance();
                    else
                        break; //Either no comma, or a comma belonging to the enclosing list: leave it for the caller
                }
                return annotations;
            }
            finally
            {
                //Guaranteed to run even if a nested call above threw, keeping the depth counter accurate
                ExitRecursion();
            }
        }

        /// <summary>
        /// annotation := annotationPropertyIRI annotationValue
        /// </summary>
        private OWLAnnotation ParseAnnotation()
        {
            //The property is always a bare IRI; punning between kinds doesn't apply here, so no symbol table lookup
            OWLAnnotationProperty annProp = new OWLAnnotationProperty(new RDFResource(ParseEntityIRI("annotation")));
            //annotationValue := iri | nodeID | literal: the token type alone tells which of the three
            //OWLAnnotation.Value* overloads applies, so no further lookahead is needed here
            switch (Current.Type)
            {
                case OWLManchesterTokenType.FullIRI:
                case OWLManchesterTokenType.PrefixedName:
                    return new OWLAnnotation(annProp, new RDFResource(ResolveIRI(Advance())));
                case OWLManchesterTokenType.NodeID:
                    return new OWLAnnotation(annProp, new OWLAnonymousIndividual(Advance().Value));
                //Anything else (quoted string, number, boolean name, ...) must be a literal; ParseLiteral itself
                //throws if even that doesn't match
                default:
                    return new OWLAnnotation(annProp, ParseLiteral());
            }
        }

        /// <summary>
        /// Attaches the eventual nested annotations to the given annotation
        /// (the object model supports a single nested annotation, so eventual extra ones are dropped with a warning)
        /// </summary>
        private static OWLAnnotation NestAnnotations(OWLAnnotation annotation, List<OWLAnnotation> nestedAnnotations)
        {
            //No nested block at all: nothing to attach, return the annotation exactly as parsed
            if (nestedAnnotations.Count > 0)
            {
                //Only the first nested annotation is representable, so it's the one that gets attached
                annotation.Annotation = nestedAnnotations[0];
                //Anything past the first is silently unrepresentable, hence surfaced only as a non-fatal warning
                if (nestedAnnotations.Count > 1)
                    OWLEvents.RaiseWarning($"Annotation of type {annotation.AnnotationProperty.GetIRI()} has {nestedAnnotations.Count} nested annotations, but only the first one is representable in the OWL2 object model: the others have been skipped");
            }
            return annotation;
        }

        /// <summary>
        /// Tells if the given token pair begins an annotation (used for solving the block/list comma ambiguity):
        /// an annotation starts with a nested "Annotations:" block, or with an IRI not declared as a
        /// non-annotation entity which is followed by an annotation value
        /// </summary>
        private bool IsAnnotationStart(OWLManchesterToken first, OWLManchesterToken second)
        {
            //A nested meta-annotation block unambiguously starts a further annotation
            if (first.Type == OWLManchesterTokenType.SectionKeyword && string.Equals(first.Value, "Annotations:", StringComparison.Ordinal))
                return true;

            if (IsEntityToken(first))
            {
                //Unresolvable tokens cannot start an annotation (this is a speculative lookahead, so it must not throw)
                if (first.Type != OWLManchesterTokenType.FullIRI
                     && !prefixes.ContainsKey(first.Type == OWLManchesterTokenType.Name ? string.Empty : first.Value.Substring(0, first.Value.IndexOf(':'))))
                    return false;

                //An IRI already known to declare something other than an annotation property can't be one here
                if (symbols.TryGetValue(ResolveIRI(first), out OWLManchesterFrameKind firstKind) && firstKind != OWLManchesterFrameKind.AnnotationProperty)
                    return false;

                //Whatever follows the (candidate) annotation property must itself look like a legal annotation value
                switch (second.Type)
                {
                    case OWLManchesterTokenType.QuotedString:
                    case OWLManchesterTokenType.IntegerNumber:
                    case OWLManchesterTokenType.DecimalNumber:
                    case OWLManchesterTokenType.FloatNumber:
                    case OWLManchesterTokenType.FullIRI:
                    case OWLManchesterTokenType.PrefixedName:
                    case OWLManchesterTokenType.NodeID:
                        return true;
                    //A bare Name value is only legal as the boolean literals "true"/"false", or as a further entity IRI
                    case OWLManchesterTokenType.Name:
                        return string.Equals(second.Value, "true", StringComparison.Ordinal)
                            || string.Equals(second.Value, "false", StringComparison.Ordinal)
                            || IsEntityToken(second);
                }
            }
            return false;
        }
        #endregion

        #region Expressions
        /// <summary>
        /// description := conjunction ('or' conjunction)*
        /// </summary>
        private OWLClassExpression ParseDescription()
        {
            //Guarded because "(" and "not" reach back here through ParsePrimary, letting a crafted document
            //nest descriptions arbitrarily deep; see EnterRecursion's own remarks
            EnterRecursion("class description");
            try
            {
                OWLClassExpression clsExpr = ParseConjunction();
                //No "or" at all: this is just a single conjunction, not a union, so return it as-is
                if (CurrentIsName("or"))
                {
                    List<OWLClassExpression> unionMembers = new List<OWLClassExpression> { clsExpr };
                    //A flat loop, not recursion: an arbitrarily long "A or B or C or ..." chain costs no extra depth
                    while (CurrentIsName("or"))
                    {
                        Advance();
                        unionMembers.Add(ParseConjunction());
                    }
                    return new OWLObjectUnionOf(unionMembers);
                }
                return clsExpr;
            }
            finally
            {
                ExitRecursion();
            }
        }

        /// <summary>
        /// conjunction := primary (('and' | 'that') primary)*
        /// </summary>
        private OWLClassExpression ParseConjunction()
        {
            //Not itself guarded: the only unbounded recursion risk in this grammar level is inside ParsePrimary,
            //which already enters/exits the guard on its own; the "and"/"that" loop below is flat, not recursive
            OWLClassExpression clsExpr = ParsePrimary();
            if (CurrentIsName("and") || CurrentIsName("that"))
            {
                //"and" and "that" are synonyms in Manchester syntax ("Pizza and hasTopping value Mozzarella"
                //reads the same as "Pizza that hasTopping value Mozzarella"), so both drive the same loop
                List<OWLClassExpression> intersectionMembers = new List<OWLClassExpression> { clsExpr };
                while (CurrentIsName("and") || CurrentIsName("that"))
                {
                    Advance();
                    intersectionMembers.Add(ParsePrimary());
                }
                return new OWLObjectIntersectionOf(intersectionMembers);
            }
            return clsExpr;
        }

        /// <summary>
        /// primary := ['not'] (restriction | atomic)
        /// </summary>
        private OWLClassExpression ParsePrimary()
        {
            //The most critical guard of the class-expression grammar: "not"/"(" and property restriction chains
            //(further below) all funnel unbounded nesting back through this very method
            EnterRecursion("class expression");
            try
            {
                //A "not" prefix recurses directly on itself, letting negations stack ("not not not C"), rather
                //than looping, since each level wraps the next in its own OWLObjectComplementOf
                if (CurrentIsName("not"))
                {
                    Advance();
                    return new OWLObjectComplementOf(ParsePrimary());
                }

                switch (Current.Type)
                {
                    //'(' description ')'
                    case OWLManchesterTokenType.OpenParenthesis:
                    {
                        Advance();
                        OWLClassExpression clsExpr = ParseDescription();
                        Expect(OWLManchesterTokenType.CloseParenthesis, "parenthesized description");
                        return clsExpr;
                    }

                    //'{' individualList '}'
                    case OWLManchesterTokenType.OpenBrace:
                    {
                        Advance();
                        List<OWLIndividualExpression> idvExprs = new List<OWLIndividualExpression>();
                        do
                        {
                            idvExprs.Add(ParseIndividual());
                        } while (TryConsumeComma());
                        Expect(OWLManchesterTokenType.CloseBrace, "individual enumeration");
                        return new OWLObjectOneOf(idvExprs);
                    }

                    //'inverse' objectPropertyIRI restriction: the property expression consumes the "inverse"
                    //keyword itself, so what remains here is always a plain object property restriction
                    case OWLManchesterTokenType.Name when CurrentIsName("inverse"):
                        return ParseObjectRestriction(ParseObjectPropertyExpression());

                    //classIRI, or property restriction (disambiguated via lookahead + symbol table)
                    case OWLManchesterTokenType.FullIRI:
                    case OWLManchesterTokenType.PrefixedName:
                    case OWLManchesterTokenType.Name when IsEntityToken(Current):
                    {
                        //A restriction keyword right after this token (without consuming it yet) is what tells a
                        //bare class reference apart from a property restriction; otherwise this token IS the class
                        if (Peek(1).Type == OWLManchesterTokenType.Name && RestrictionKeywords.Contains(Peek(1).Value))
                        {
                            string propertyIRI = ResolveIRI(Current);
                            //The symbol table (built ahead of time in pass 1) is what makes this decision possible,
                            //even when the owning property frame appears later in the document than this reference
                            symbols.TryGetValue(propertyIRI, out OWLManchesterFrameKind propertyKind);
                            switch (propertyKind)
                            {
                                case OWLManchesterFrameKind.ObjectProperty:
                                    Advance(); //consume the property IRI: the restriction keyword is parsed by ParseObjectRestriction itself
                                    return ParseObjectRestriction(new OWLObjectProperty(new RDFResource(propertyIRI)));
                                case OWLManchesterFrameKind.DataProperty:
                                    Advance(); //consume the property IRI: the restriction keyword is parsed by ParseDataRestriction itself
                                    return ParseDataRestriction(new OWLDataProperty(new RDFResource(propertyIRI)));
                                //Undeclared, or declared as something else entirely (e.g: a class): a restriction can't be built on it
                                default:
                                    throw new OWLException($"Cannot parse OWL2/Manchester document: restriction on <{propertyIRI}> requires it to be declared as object or data property ({Current})");
                            }
                        }
                        //No restriction keyword follows: this token is simply a class reference
                        return new OWLClass(new RDFResource(ResolveIRI(Advance())));
                    }

                    default:
                        throw new OWLException($"Cannot parse OWL2/Manchester document: expected class expression, found {Current}");
                }
            }
            finally
            {
                ExitRecursion();
            }
        }

        /// <summary>
        /// objectRestriction := objectPropertyExpression ('some'|'only') primary | 'value' individual | 'Self'
        ///                       | ('min'|'max'|'exactly') nonNegativeInteger [primary]
        /// </summary>
        private OWLClassExpression ParseObjectRestriction(OWLObjectPropertyExpression objPropExpr)
        {
            //The restriction keyword itself was only peeked at by the caller, never consumed, so it's read here
            OWLManchesterToken keywordToken = Expect(OWLManchesterTokenType.Name, "object restriction");
            switch (keywordToken.Value)
            {
                //"some"/"only" recurse into ParsePrimary for their filler: this is exactly what lets restriction
                //chains ("p some p some p some C") nest arbitrarily deep, hence the shared recursion guard
                case "some": return new OWLObjectSomeValuesFrom(objPropExpr, ParsePrimary());
                case "only": return new OWLObjectAllValuesFrom(objPropExpr, ParsePrimary());
                case "value": return new OWLObjectHasValue(objPropExpr, ParseIndividual());
                //"Self" takes no filler at all
                case "Self": return new OWLObjectHasSelf(objPropExpr);
                case "min":
                case "max":
                case "exactly":
                {
                    uint cardinality = ParseCardinality();
                    //The qualifying primary is optional in the grammar: IsPrimaryStart() peeks at the current
                    //token to tell a qualified restriction ("min 2 hasTopping Pizza") from an unqualified one
                    //("min 2 hasTopping") without unconditionally consuming a token
                    OWLClassExpression qualifier = IsPrimaryStart() ? ParsePrimary() : null;
                    //3 axiom types share this cardinality-plus-optional-qualifier shape, so the choice is
                    //made last, once cardinality and qualifier are already in hand
                    switch (keywordToken.Value)
                    {
                        case "min": return qualifier == null ? new OWLObjectMinCardinality(objPropExpr, cardinality) : new OWLObjectMinCardinality(objPropExpr, cardinality, qualifier);
                        case "max": return qualifier == null ? new OWLObjectMaxCardinality(objPropExpr, cardinality) : new OWLObjectMaxCardinality(objPropExpr, cardinality, qualifier);
                        default: return qualifier == null ? new OWLObjectExactCardinality(objPropExpr, cardinality) : new OWLObjectExactCardinality(objPropExpr, cardinality, qualifier);
                    }
                }
                //Any other Name token is not one of the restriction keywords this production accepts
                default:
                    throw new OWLException($"Cannot parse OWL2/Manchester document: unknown object restriction keyword {keywordToken}");
            }
        }

        /// <summary>
        /// dataRestriction := dataPropertyIRI ('some'|'only') dataPrimary | 'value' literal
        ///                     | ('min'|'max'|'exactly') nonNegativeInteger [dataPrimary]
        /// </summary>
        private OWLClassExpression ParseDataRestriction(OWLDataProperty dtProp)
        {
            //Mirrors ParseObjectRestriction one-to-one, but resolving to data ranges instead of class expressions
            //(no "Self" here: reflexivity on oneself has no meaning for a data property)
            OWLManchesterToken keywordToken = Expect(OWLManchesterTokenType.Name, "data restriction");
            switch (keywordToken.Value)
            {
                case "some": return new OWLDataSomeValuesFrom(dtProp, ParseDataPrimary());
                case "only": return new OWLDataAllValuesFrom(dtProp, ParseDataPrimary());
                case "value": return new OWLDataHasValue(dtProp, ParseLiteral());
                case "min":
                case "max":
                case "exactly":
                {
                    uint cardinality = ParseCardinality();
                    //Same optional-qualifier pattern as the object restriction counterpart above
                    OWLDataRangeExpression qualifier = IsPrimaryStart() ? ParseDataPrimary() : null;
                    switch (keywordToken.Value)
                    {
                        case "min": return qualifier == null ? new OWLDataMinCardinality(dtProp, cardinality)   : new OWLDataMinCardinality(dtProp, cardinality, qualifier);
                        case "max": return qualifier == null ? new OWLDataMaxCardinality(dtProp, cardinality)   : new OWLDataMaxCardinality(dtProp, cardinality, qualifier);
                        default:    return qualifier == null ? new OWLDataExactCardinality(dtProp, cardinality) : new OWLDataExactCardinality(dtProp, cardinality, qualifier);
                    }
                }
                default:
                    throw new OWLException($"Cannot parse OWL2/Manchester document: unknown data restriction keyword {keywordToken}");
            }
        }

        /// <summary>
        /// Tells if the current token can begin a (class or data range) primary: used to detect
        /// the optional qualifier of cardinality restrictions
        /// </summary>
        private bool IsPrimaryStart()
        {
            switch (Current.Type)
            {
                //Any of these tokens can only be the start of a genuine qualifier, never of whatever follows
                //the restriction in the enclosing grammar (e.g: a comma, a closing bracket, a further keyword)
                case OWLManchesterTokenType.FullIRI:
                case OWLManchesterTokenType.PrefixedName:
                case OWLManchesterTokenType.OpenParenthesis:
                case OWLManchesterTokenType.OpenBrace:
                    return true;
                //A bare Name only starts a qualifier when it's the negation keyword; other Names (e.g: a
                //further restriction keyword) belong to something else entirely, not to this cardinality's qualifier
                case OWLManchesterTokenType.Name:
                    return CurrentIsName("not");
                default:
                    return false;
            }
        }

        /// <summary>
        /// nonNegativeInteger := digits (consumed as the count of a min/max/exactly cardinality restriction)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private uint ParseCardinality()
        {
            //Cardinalities are non-negative by construction (the grammar has no "-" here), so uint is a safe fit
            OWLManchesterToken cardinalityToken = Expect(OWLManchesterTokenType.IntegerNumber, "cardinality restriction");
            return uint.TryParse(cardinalityToken.Value, out uint cardinality)
                ? cardinality
                //Only reachable if the lexer ever emitted a numeral wider than uint.MaxValue can hold
                : throw new OWLException($"Cannot parse OWL2/Manchester document: invalid cardinality {cardinalityToken}");
        }

        /// <summary>
        /// dataRange := dataConjunction ('or' dataConjunction)*
        /// </summary>
        private OWLDataRangeExpression ParseDataRange()
        {
            //The data-range mirror of ParseDescription: "(" and "p some ..." restriction chains funnel back here
            EnterRecursion("data range");
            try
            {
                OWLDataRangeExpression drExpr = ParseDataConjunction();
                if (CurrentIsName("or"))
                {
                    List<OWLDataRangeExpression> unionMembers = new List<OWLDataRangeExpression> { drExpr };
                    //A flat loop, not recursion: an arbitrarily long "A or B or C or ..." chain costs no extra depth
                    while (CurrentIsName("or"))
                    {
                        Advance();
                        unionMembers.Add(ParseDataConjunction());
                    }
                    return new OWLDataUnionOf(unionMembers);
                }
                return drExpr;
            }
            finally
            {
                ExitRecursion();
            }
        }

        /// <summary>
        /// dataConjunction := dataPrimary ('and' dataPrimary)*
        /// </summary>
        private OWLDataRangeExpression ParseDataConjunction()
        {
            //Not itself guarded, for the same reason as ParseConjunction: the only unbounded-depth risk here is
            //inside ParseDataPrimary, which enters/exits the guard on its own; this "and" loop is flat
            OWLDataRangeExpression drExpr = ParseDataPrimary();
            if (CurrentIsName("and"))
            {
                List<OWLDataRangeExpression> intersectionMembers = new List<OWLDataRangeExpression> { drExpr };
                while (CurrentIsName("and"))
                {
                    Advance();
                    intersectionMembers.Add(ParseDataPrimary());
                }
                return new OWLDataIntersectionOf(intersectionMembers);
            }
            return drExpr;
        }

        /// <summary>
        /// dataPrimary := ['not'] dataAtomic
        /// </summary>
        private OWLDataRangeExpression ParseDataPrimary()
        {
            //The data-range mirror of ParsePrimary: guarded because both "not" (below) and "(" (in ParseDataAtomic)
            //funnel unbounded nesting back through this very method
            EnterRecursion("data range expression");
            try
            {
                //Recurses on itself so negations can stack ("not not xsd:integer"), each level wrapping the next
                if (CurrentIsName("not"))
                {
                    Advance();
                    return new OWLDataComplementOf(ParseDataPrimary());
                }
                return ParseDataAtomic();
            }
            finally
            {
                ExitRecursion();
            }
        }

        /// <summary>
        /// dataAtomic := datatype ['[' facetRestrictionList ']'] | '{' literalList '}' | '(' dataRange ')'
        /// </summary>
        private OWLDataRangeExpression ParseDataAtomic()
        {
            //Not itself guarded: it never recurses into ParseDataRange/ParseDataPrimary except through the
            //explicit "(" case below, which is a bounded, single step delegated back up to the guarded caller
            switch (Current.Type)
            {
                //'(' dataRange ')'
                case OWLManchesterTokenType.OpenParenthesis:
                {
                    Advance();
                    OWLDataRangeExpression drExpr = ParseDataRange();
                    Expect(OWLManchesterTokenType.CloseParenthesis, "parenthesized data range");
                    return drExpr;
                }

                //'{' literalList '}'
                case OWLManchesterTokenType.OpenBrace:
                {
                    Advance();
                    List<OWLLiteral> literals = new List<OWLLiteral>();
                    do
                    {
                        literals.Add(ParseLiteral());
                    } while (TryConsumeComma());
                    Expect(OWLManchesterTokenType.CloseBrace, "literal enumeration");
                    return new OWLDataOneOf(literals);
                }

                //Built-in datatype shortcut names ('integer', 'decimal', 'float', 'string'): tried before the
                //generic IRI case below, since these are lexed as plain Name tokens, not as prefixed/full IRIs
                case OWLManchesterTokenType.Name when BuiltInDatatypes.TryGetValue(Current.Value, out string builtInDatatypeIRI):
                    Advance();
                    return ParseEventualDatatypeRestriction(new OWLDatatype(new RDFResource(builtInDatatypeIRI)));

                //datatypeIRI ['[' facetRestrictionList ']']
                case OWLManchesterTokenType.FullIRI:
                case OWLManchesterTokenType.PrefixedName:
                case OWLManchesterTokenType.Name when IsEntityToken(Current):
                    return ParseEventualDatatypeRestriction(new OWLDatatype(new RDFResource(ResolveIRI(Advance()))));

                default:
                    throw new OWLException($"Cannot parse OWL2/Manchester document: expected data range, found {Current}");
            }
        }

        /// <summary>
        /// facetRestrictionList := facet literal (',' facet literal)*
        /// </summary>
        private OWLDataRangeExpression ParseEventualDatatypeRestriction(OWLDatatype dt)
        {
            //No '[' at all: the datatype has no facet restrictions, return it plain
            if (Current.Type != OWLManchesterTokenType.OpenBracket)
                return dt;

            Advance();
            List<OWLFacetRestriction> facetRestrictions = new List<OWLFacetRestriction>();
            do
            {
                string facetIRI;
                switch (Current.Type)
                {
                    //Facet keywords ("length", "pattern", ...) and comparators ("<=", ">", ...): the reverse
                    //lookup table maps the Manchester-specific spelling back to the underlying OWL2 facet IRI
                    case OWLManchesterTokenType.Name:
                    case OWLManchesterTokenType.LessOrEqual:
                    case OWLManchesterTokenType.GreaterOrEqual:
                    case OWLManchesterTokenType.LessThan:
                    case OWLManchesterTokenType.GreaterThan:
                        OWLManchesterToken facetToken = Advance();
                        facetIRI = SymbolFacets.TryGetValue(facetToken.Value, out string knownFacetIRI)
                            ? knownFacetIRI
                            : throw new OWLException($"Cannot parse OWL2/Manchester document: unknown facet {facetToken}");
                        break;

                    //Explicit facet IRIs (fallback emitted by the serializer for facets without a Manchester symbol)
                    case OWLManchesterTokenType.FullIRI:
                    case OWLManchesterTokenType.PrefixedName:
                        facetIRI = ResolveIRI(Advance());
                        break;

                    default:
                        throw new OWLException($"Cannot parse OWL2/Manchester document: expected facet, found {Current}");
                }
                //Every facet is paired with exactly one literal value (e.g: "length \"3\"^^xsd:integer")
                facetRestrictions.Add(new OWLFacetRestriction(ParseLiteral(), new RDFResource(facetIRI)));
            } while (TryConsumeComma());
            Expect(OWLManchesterTokenType.CloseBracket, "datatype restriction");

            return new OWLDatatypeRestriction(dt, facetRestrictions);
        }

        /// <summary>
        /// objectPropertyExpression := ['inverse'] objectPropertyIRI
        /// </summary>
        private OWLObjectPropertyExpression ParseObjectPropertyExpression()
        {
            //"inverse" wraps the property, rather than being resolved against the symbol table: an inverse
            //object property reference is always valid regardless of how the underlying property was declared
            if (CurrentIsName("inverse"))
            {
                Advance();
                return new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource(ParseEntityIRI("inverse object property"))));
            }
            return new OWLObjectProperty(new RDFResource(ParseEntityIRI("object property")));
        }

        /// <summary>
        /// individual := individualIRI | nodeID
        /// </summary>
        private OWLIndividualExpression ParseIndividual()
            //A NodeID token ("_:foo") is always anonymous; anything else must resolve as a named individual's IRI
            => Current.Type == OWLManchesterTokenType.NodeID
                ? (OWLIndividualExpression)new OWLAnonymousIndividual(Advance().Value)
                : new OWLNamedIndividual(new RDFResource(ParseEntityIRI("individual")));

        /// <summary>
        /// literal := quotedString [('^^' datatypeIRI) | languageTag] | integerLiteral | decimalLiteral | floatingPointLiteral | booleanLiteral
        /// </summary>
        private OWLLiteral ParseLiteral()
        {
            switch (Current.Type)
            {
                case OWLManchesterTokenType.QuotedString:
                {
                    string value = Advance().Value;
                    //A quoted string can carry at most one of a datatype suffix or a language tag, never both
                    if (Current.Type == OWLManchesterTokenType.DoubleCaret)
                    {
                        Advance();
                        return new OWLLiteral { Value = value, DatatypeIRI = ParseEntityIRI("typed literal") };
                    }
                    if (Current.Type == OWLManchesterTokenType.LanguageTag)
                        return new OWLLiteral { Value = value, Language = Advance().Value };
                    //Neither suffix present: a plain, untyped/unlocalized string literal
                    return new OWLLiteral { Value = value };
                }

                //Numeric literals are typed by the lexer's own classification of the numeral's shape
                //(no dot/exponent => integer, dot only => decimal, exponent or "f" suffix => float)
                case OWLManchesterTokenType.IntegerNumber:
                    return new OWLLiteral { Value = Advance().Value, DatatypeIRI = RDFVocabulary.XSD.INTEGER.ToString() };
                case OWLManchesterTokenType.DecimalNumber:
                    return new OWLLiteral { Value = Advance().Value, DatatypeIRI = RDFVocabulary.XSD.DECIMAL.ToString() };
                case OWLManchesterTokenType.FloatNumber:
                    return new OWLLiteral { Value = Advance().Value, DatatypeIRI = RDFVocabulary.XSD.FLOAT.ToString() };

                //"true"/"false" are lexed as plain Name tokens, not as a dedicated boolean token type
                case OWLManchesterTokenType.Name when CurrentIsName("true") || CurrentIsName("false"):
                    return new OWLLiteral { Value = Advance().Value, DatatypeIRI = RDFVocabulary.XSD.BOOLEAN.ToString() };

                default:
                    throw new OWLException($"Cannot parse OWL2/Manchester document: expected literal, found {Current}");
            }
        }
        #endregion

        #region Building utilities
        /// <summary>
        /// annotatedList := [annotations] item (',' [annotations] item)*
        /// (the given handler parses each item from the current position, receiving its eventual axiom annotations)
        /// </summary>
        private void ParseAnnotatedList(Action<List<OWLAnnotation>> itemHandler)
        {
            do
            {
                //Each item's own leading "Annotations:" block (if any) is read first, then handed to the caller's
                //handler alongside the item itself, so the handler can attach them to the axiom it builds
                itemHandler(TryParseAnnotationBlock());
            } while (TryConsumeComma());
        }

        /// <summary>
        /// Annotates the given axiom and adds it to the given registry of the ontology
        /// </summary>
        private static void AddAxiom<T>(T axiom, List<OWLAnnotation> annotations, List<T> axiomRegistry) where T : OWLAxiom
        {
            //Annotated before being added, so the axiom is never visible in the registry in a half-annotated state
            annotations.ForEach(axiom.Annotate);
            axiomRegistry.Add(axiom);
        }

        /// <summary>
        /// Adds the declaration axiom corresponding to the given frame header (frames declare their owning entity)
        /// </summary>
        private void AddDeclaration(OWLManchesterFrameKind frameKind, RDFResource entityIRI)
        {
            //Keyed on both kind and IRI, so an entity punned across compatible frames still gets only one
            //declaration per kind, while a genuinely repeated frame header for the same entity is a no-op here
            if (!declaredEntities.Add($"{frameKind}#{entityIRI}"))
                return;

            //Each frame kind maps 1:1 onto the OWL2 entity wrapper its OWLDeclaration expects
            switch (frameKind)
            {
                case OWLManchesterFrameKind.Class: ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLClass(entityIRI))); break;
                case OWLManchesterFrameKind.ObjectProperty: ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLObjectProperty(entityIRI))); break;
                case OWLManchesterFrameKind.DataProperty: ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLDataProperty(entityIRI))); break;
                case OWLManchesterFrameKind.AnnotationProperty: ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLAnnotationProperty(entityIRI))); break;
                case OWLManchesterFrameKind.Datatype: ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLDatatype(entityIRI))); break;
                case OWLManchesterFrameKind.Individual: ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLNamedIndividual(entityIRI))); break;
            }
        }
        #endregion

        #endregion
    }
}