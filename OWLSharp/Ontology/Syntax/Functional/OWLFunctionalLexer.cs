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
using System.Text;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLFunctionalLexer turns an OWL2/Functional-Style document into the flat sequence of tokens
    /// consumed by OWLFunctionalParser (https://www.w3.org/TR/owl2-syntax/#Functional-Style_Syntax).
    /// Compared to OWLManchesterLexer, this lexer is noticeably simpler: the Functional-Style grammar
    /// has no "section keyword" concept (Manchester's "Class:", "SubClassOf:", ...) because every
    /// construct - entity, expression, axiom, or document element - is a self-delimiting
    /// "Keyword( arg1 arg2 ... )" token; keywords are therefore just bare words, resolved contextually
    /// by the parser exactly like any other identifier, and punctuation is limited to parentheses plus
    /// the single '=' separator used by the prefixDeclaration production.
    /// </summary>
    internal static class OWLFunctionalLexer
    {
        #region Methods
        /// <summary>
        /// Tokenizes the given OWL2/Functional-Style document (the resulting sequence always ends with an EndOfDocument token)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal static List<OWLFunctionalToken> Tokenize(string funDocument)
        {
            List<OWLFunctionalToken> tokens = new List<OWLFunctionalToken>();
            string doc = funDocument ?? string.Empty;
            int pos = 0, line = 1, col = 1;

            #region Utilities
            //Advances the cursor by the given number of characters, tracking line/column for error reporting
            void Move(int chars=1)
            {
                for (int i = 0; i < chars; i++)
                {
                    if (doc[pos] == '\n')
                    {
                        line++;
                        col = 1;
                    }
                    else
                    {
                        col++;
                    }
                    pos++;
                }
            }

            //Appends a token to the result sequence, anchored at the given (pre-Move) position
            void EmitToken(OWLFunctionalTokenType type, string value, int tokLine, int tokCol)
                => tokens.Add(new OWLFunctionalToken { Type = type, Value = value, Line = tokLine, Column = tokCol });

            //Tells if the given character can start a bare word, prefix name or local name
            bool IsNameStartChar(char c)
                => char.IsLetterOrDigit(c) || c == '_';

            //Tells if the given character can continue a bare word, prefix name or local name
            //(wider than IsNameStartChar: local names may contain '-' and '.' after the first character,
            //mirroring the conservative PN_LOCAL subset already assumed by OWLFunctionalContext.LocalNameRegex)
            bool IsNameChar(char c)
                => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.';
            #endregion

            while (pos < doc.Length)
            {
                char c = doc[pos];
                int tokLine = line, tokCol = col;

                //Whitespaces (skipped): the Functional-Style grammar is entirely delimiter-driven
                //(parentheses and mandatory whitespace between arguments), never indentation-sensitive
                if (char.IsWhiteSpace(c))
                {
                    Move();
                    continue;
                }

                //Comments (skipped until end of line): not part of the normative grammar, but accepted
                //as a pragmatic convenience for hand-authored documents, mirroring OWLManchesterLexer
                if (c == '#')
                {
                    while (pos < doc.Length && doc[pos] != '\n')
                        Move();
                    continue;
                }

                switch (c)
                {
                    //Punctuation: the Functional-Style grammar only ever nests via parentheses
                    case '(': EmitToken(OWLFunctionalTokenType.OpenParenthesis, "(", tokLine, tokCol); Move(); continue;
                    case ')': EmitToken(OWLFunctionalTokenType.CloseParenthesis, ")", tokLine, tokCol); Move(); continue;

                    //Separator between prefixName and fullIRI in the prefixDeclaration production
                    //(the only place '=' appears in the whole grammar: 'Prefix' '(' prefixName '=' fullIRI ')')
                    case '=': EmitToken(OWLFunctionalTokenType.Equals, "=", tokLine, tokCol); Move(); continue;

                    //Full IRI, enclosed in angle brackets (fullIRI production)
                    case '<':
                    {
                        int scan = pos + 1;
                        while (scan < doc.Length && doc[scan] != '>' && doc[scan] != '<' && doc[scan] != '"' && !char.IsWhiteSpace(doc[scan]))
                            scan++;
                        if (scan >= doc.Length || doc[scan] != '>')
                            throw new OWLException($"Cannot tokenize OWL2/Functional document: unterminated full IRI at line {tokLine}, column {tokCol}");

                        EmitToken(OWLFunctionalTokenType.FullIRI, doc.Substring(pos + 1, scan - pos - 1), tokLine, tokCol);
                        Move(scan - pos + 1);
                        continue;
                    }

                    //Quoted string (lexicalForm/quotedString production, with \" and \\ escapes)
                    case '"':
                    {
                        StringBuilder sb = new StringBuilder();
                        Move();
                        bool closed = false;
                        while (pos < doc.Length)
                        {
                            if (doc[pos] == '\\' && pos + 1 < doc.Length && (doc[pos + 1] == '"' || doc[pos + 1] == '\\'))
                            {
                                sb.Append(doc[pos + 1]);
                                Move(2);
                            }
                            else if (doc[pos] == '"')
                            {
                                Move();
                                closed = true;
                                break;
                            }
                            else
                            {
                                sb.Append(doc[pos]);
                                Move();
                            }
                        }
                        if (!closed)
                            throw new OWLException($"Cannot tokenize OWL2/Functional document: unterminated string literal at line {tokLine}, column {tokCol}");
                        EmitToken(OWLFunctionalTokenType.QuotedString, sb.ToString(), tokLine, tokCol);
                        continue;
                    }

                    //Datatype suffix separator (typedLiteral production: lexicalForm '^^' Datatype)
                    case '^':
                        if (pos + 1 < doc.Length && doc[pos + 1] == '^')
                        {
                            EmitToken(OWLFunctionalTokenType.DoubleCaret, "^^", tokLine, tokCol);
                            Move(2);
                            continue;
                        }
                        throw new OWLException($"Cannot tokenize OWL2/Functional document: unexpected character '^' at line {tokLine}, column {tokCol}");

                    //Language tag (languageTag production: '@' followed by a BCP 47 langtag; we accept
                    //the pragmatic letters/digits/hyphen subset actually produced by real language tags)
                    case '@':
                    {
                        Move();
                        StringBuilder sb = new StringBuilder();
                        while (pos < doc.Length && (char.IsLetterOrDigit(doc[pos]) || doc[pos] == '-'))
                        {
                            sb.Append(doc[pos]);
                            Move();
                        }
                        if (sb.Length == 0)
                            throw new OWLException($"Cannot tokenize OWL2/Functional document: empty language tag at line {tokLine}, column {tokCol}");
                        EmitToken(OWLFunctionalTokenType.LanguageTag, sb.ToString(), tokLine, tokCol);
                        continue;
                    }

                    //Prefixed name with empty/default prefix (":Person"), or bare namespace prefix
                    //used only inside a prefixDeclaration ("Prefix(:=<...>)" - the ':' alone, before '=')
                    case ':':
                    {
                        Move();
                        if (pos < doc.Length && IsNameStartChar(doc[pos]))
                        {
                            StringBuilder sb = new StringBuilder(":");
                            while (pos < doc.Length && IsNameChar(doc[pos]))
                            {
                                sb.Append(doc[pos]);
                                Move();
                            }
                            EmitToken(OWLFunctionalTokenType.PrefixedName, sb.ToString(), tokLine, tokCol);
                        }
                        else
                            EmitToken(OWLFunctionalTokenType.PrefixNamespace, ":", tokLine, tokCol);
                        continue;
                    }
                }

                //Anonymous individual (AnonymousIndividual := nodeID production, e.g: "_:idv1")
                if (c == '_' && pos + 1 < doc.Length && doc[pos + 1] == ':')
                {
                    Move(2);
                    StringBuilder sb = new StringBuilder();
                    while (pos < doc.Length && IsNameChar(doc[pos]))
                    {
                        sb.Append(doc[pos]);
                        Move();
                    }
                    if (sb.Length == 0)
                        throw new OWLException($"Cannot tokenize OWL2/Functional document: empty anonymous individual at line {tokLine}, column {tokCol}");
                    EmitToken(OWLFunctionalTokenType.NodeID, sb.ToString(), tokLine, tokCol);
                    continue;
                }

                //Non-negative integer (nonNegativeInteger production: used exclusively as the cardinality
                //argument of Object/DataMin/Max/ExactCardinality - every other numeric value in the grammar
                //is a quoted, datatype-tagged Literal, e.g: "5"^^xsd:integer, never a bare digit sequence)
                if (char.IsDigit(c))
                {
                    StringBuilder sb = new StringBuilder();
                    while (pos < doc.Length && char.IsDigit(doc[pos]))
                    {
                        sb.Append(doc[pos]);
                        Move();
                    }
                    EmitToken(OWLFunctionalTokenType.NonNegativeInteger, sb.ToString(), tokLine, tokCol);
                    continue;
                }

                //Word: either a bare keyword/name (e.g: "SubClassOf", "Prefix", "Class") or, if immediately
                //followed by ':' and a further name-start character, a prefixed name (abbreviatedIRI, e.g: "a:Person")
                if (IsNameStartChar(c))
                {
                    StringBuilder sb = new StringBuilder();
                    while (pos < doc.Length && IsNameChar(doc[pos]))
                    {
                        sb.Append(doc[pos]);
                        Move();
                    }
                    if (pos < doc.Length && doc[pos] == ':')
                    {
                        Move();
                        if (pos < doc.Length && IsNameStartChar(doc[pos]))
                        {
                            sb.Append(':');
                            while (pos < doc.Length && IsNameChar(doc[pos]))
                            {
                                sb.Append(doc[pos]);
                                Move();
                            }
                            EmitToken(OWLFunctionalTokenType.PrefixedName, sb.ToString(), tokLine, tokCol);
                        }
                        else
                        {
                            //A named prefix immediately followed by ':' with nothing name-like after it:
                            //only legal as the prefixName argument of a prefixDeclaration ("Prefix(owl:=<...>)")
                            EmitToken(OWLFunctionalTokenType.PrefixNamespace, $"{sb}:", tokLine, tokCol);
                        }
                    }
                    else
                        EmitToken(OWLFunctionalTokenType.Name, sb.ToString(), tokLine, tokCol);
                    continue;
                }

                throw new OWLException($"Cannot tokenize OWL2/Functional document: unexpected character '{c}' at line {tokLine}, column {tokCol}");
            }

            EmitToken(OWLFunctionalTokenType.EndOfDocument, string.Empty, line, col);
            return tokens;
        }
        #endregion
    }

    #region FunctionalToken
    /// <summary>
    /// OWLFunctionalTokenType enumerates the kinds of token emitted by the Functional-Style lexer
    /// </summary>
    internal enum OWLFunctionalTokenType
    {
        /* IRI enclosed in angular brackets (value carries the IRI without brackets) */
        FullIRI = 1,
        /* Prefixed name, e.g: "a:Person" or ":Person" (value carries the whole "prefix:local" form) */
        PrefixedName = 2,
        /* Bare namespace prefix used only as the first argument of a prefixDeclaration, e.g: "owl:" or ":" */
        PrefixNamespace = 3,
        /* Anonymous individual, e.g: "_:idv1" (value carries the local part without "_:") */
        NodeID = 4,
        /* Bare word, e.g: "SubClassOf", "Prefix", "Ontology", "ObjectPropertyChain" (interpreted contextually by the parser) */
        Name = 5,
        /* Quoted string (value carries the unescaped content without quotes) */
        QuotedString = 6,
        /* Cardinality argument of a cardinality restriction (nonNegativeInteger production) */
        NonNegativeInteger = 7,
        /* Language tag, e.g: "@it" (value carries the tag without '@') */
        LanguageTag = 8,
        /* Datatype suffix separator "^^" */
        DoubleCaret = 9,
        OpenParenthesis = 10,
        CloseParenthesis = 11,
        /* Separator between prefixName and fullIRI in a prefixDeclaration */
        Equals = 12,
        EndOfDocument = 13
    }

    /// <summary>
    /// OWLFunctionalToken is a lexical unit of an OWL2/Functional-Style document, carrying its position for error reporting
    /// </summary>
    internal sealed class OWLFunctionalToken
    {
        #region Properties
        /// <summary>
        /// Kind of this token
        /// </summary>
        internal OWLFunctionalTokenType Type { get; set; }

        /// <summary>
        /// Textual payload of this token (meaning depends on <see cref="Type"/>: see OWLFunctionalTokenType members)
        /// </summary>
        internal string Value { get; set; }

        /// <summary>
        /// 1-based line where this token starts in the source document
        /// </summary>
        internal int Line { get; set; }

        /// <summary>
        /// 1-based column where this token starts in the source document
        /// </summary>
        internal int Column { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Builds a diagnostic representation of this token, used in parser error messages
        /// </summary>
        public override string ToString()
            => $"'{Value}' ({Type}) at line {Line}, column {Column}";
        #endregion
    }
    #endregion
}