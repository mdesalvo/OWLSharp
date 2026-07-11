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
    /// OWLManchesterLexer turns an OWL2/Manchester document into the flat sequence of tokens
    /// consumed by OWLManchesterParser (https://www.w3.org/TR/owl2-manchester-syntax/)
    /// </summary>
    internal static class OWLManchesterLexer
    {
        #region Methods
        /// <summary>
        /// Tokenizes the given OWL2/Manchester document (the resulting sequence always ends with an EndOfDocument token)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal static List<OWLManchesterToken> Tokenize(string manDocument)
        {
            List<OWLManchesterToken> tokens = new List<OWLManchesterToken>();
            string doc = manDocument ?? string.Empty;
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
            void EmitToken(OWLManchesterTokenType type, string value, int tokLine, int tokCol)
                => tokens.Add(new OWLManchesterToken { Type = type, Value = value, Line = tokLine, Column = tokCol });

            //Tells if the given character can start a bare word, prefix name or local name
            bool IsNameStartChar(char c)
                => char.IsLetterOrDigit(c) || c == '_';

            //Tells if the given character can continue a bare word, prefix name or local name
            //(wider than IsNameStartChar: local names may contain '-' and '.' after the first character)
            bool IsNameChar(char c)
                => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.';
            #endregion

            while (pos < doc.Length)
            {
                char c = doc[pos];
                int tokLine = line, tokCol = col;

                //Whitespaces (skipped)
                if (char.IsWhiteSpace(c))
                {
                    Move();
                    continue;
                }

                //Comments (skipped until end of line)
                if (c == '#')
                {
                    while (pos < doc.Length && doc[pos] != '\n')
                        Move();
                    continue;
                }

                switch (c)
                {
                    //Punctuation
                    case ',': EmitToken(OWLManchesterTokenType.Comma, ",", tokLine, tokCol); Move(); continue;
                    case '(': EmitToken(OWLManchesterTokenType.OpenParenthesis, "(", tokLine, tokCol); Move(); continue;
                    case ')': EmitToken(OWLManchesterTokenType.CloseParenthesis, ")", tokLine, tokCol); Move(); continue;
                    case '{': EmitToken(OWLManchesterTokenType.OpenBrace, "{", tokLine, tokCol); Move(); continue;
                    case '}': EmitToken(OWLManchesterTokenType.CloseBrace, "}", tokLine, tokCol); Move(); continue;
                    case '[': EmitToken(OWLManchesterTokenType.OpenBracket, "[", tokLine, tokCol); Move(); continue;
                    case ']': EmitToken(OWLManchesterTokenType.CloseBracket, "]", tokLine, tokCol); Move(); continue;

                    //Full IRI or facet comparator "<="/"<"
                    case '<':
                    {
                        int scan = pos + 1;
                        while (scan < doc.Length && doc[scan] != '>' && doc[scan] != '<' && doc[scan] != '"' && !char.IsWhiteSpace(doc[scan]))
                            scan++;
                        if (scan < doc.Length && doc[scan] == '>')
                        {
                            EmitToken(OWLManchesterTokenType.FullIRI, doc.Substring(pos + 1, scan - pos - 1), tokLine, tokCol);
                            Move(scan - pos + 1);
                        }
                        else if (pos + 1 < doc.Length && doc[pos + 1] == '=')
                        {
                            EmitToken(OWLManchesterTokenType.LessOrEqual, "<=", tokLine, tokCol);
                            Move(2);
                        }
                        else
                        {
                            EmitToken(OWLManchesterTokenType.LessThan, "<", tokLine, tokCol);
                            Move();
                        }
                        continue;
                    }

                    //Facet comparator ">="/">"
                    case '>':
                        if (pos + 1 < doc.Length && doc[pos + 1] == '=')
                        {
                            EmitToken(OWLManchesterTokenType.GreaterOrEqual, ">=", tokLine, tokCol);
                            Move(2);
                        }
                        else
                        {
                            EmitToken(OWLManchesterTokenType.GreaterThan, ">", tokLine, tokCol);
                            Move();
                        }
                        continue;

                    //Quoted string (with \" and \\ escapes)
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
                            throw new OWLException($"Cannot tokenize OWL2/Manchester document: unterminated string literal at line {tokLine}, column {tokCol}");
                        EmitToken(OWLManchesterTokenType.QuotedString, sb.ToString(), tokLine, tokCol);
                        continue;
                    }

                    //Datatype suffix separator
                    case '^':
                        if (pos + 1 < doc.Length && doc[pos + 1] == '^')
                        {
                            EmitToken(OWLManchesterTokenType.DoubleCaret, "^^", tokLine, tokCol);
                            Move(2);
                            continue;
                        }
                        throw new OWLException($"Cannot tokenize OWL2/Manchester document: unexpected character '^' at line {tokLine}, column {tokCol}");

                    //Language tag
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
                            throw new OWLException($"Cannot tokenize OWL2/Manchester document: empty language tag at line {tokLine}, column {tokCol}");
                        EmitToken(OWLManchesterTokenType.LanguageTag, sb.ToString(), tokLine, tokCol);
                        continue;
                    }

                    //Prefixed name with empty prefix (":Pizza"), or empty-prefix declaration keyword (":")
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
                            EmitToken(OWLManchesterTokenType.PrefixedName, sb.ToString(), tokLine, tokCol);
                        }
                        else
                            EmitToken(OWLManchesterTokenType.SectionKeyword, ":", tokLine, tokCol);
                        continue;
                    }
                }

                //Anonymous individual ("_:idv1")
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
                        throw new OWLException($"Cannot tokenize OWL2/Manchester document: empty anonymous individual at line {tokLine}, column {tokCol}");
                    EmitToken(OWLManchesterTokenType.NodeID, sb.ToString(), tokLine, tokCol);
                    continue;
                }

                //Numeric literal (integer, decimal, float)
                if (char.IsDigit(c) || ((c == '+' || c == '-') && pos + 1 < doc.Length && char.IsDigit(doc[pos + 1])))
                {
                    StringBuilder sb = new StringBuilder();
                    bool hasDot = false, hasExponent = false, hasFloatSuffix = false;
                    if (c == '+' || c == '-')
                    {
                        sb.Append(c);
                        Move();
                    }
                    while (pos < doc.Length && char.IsDigit(doc[pos]))
                    {
                        sb.Append(doc[pos]);
                        Move();
                    }
                    if (pos + 1 < doc.Length && doc[pos] == '.' && char.IsDigit(doc[pos + 1]))
                    {
                        hasDot = true;
                        sb.Append('.');
                        Move();
                        while (pos < doc.Length && char.IsDigit(doc[pos]))
                        {
                            sb.Append(doc[pos]);
                            Move();
                        }
                    }
                    if (pos < doc.Length && (doc[pos] == 'e' || doc[pos] == 'E'))
                    {
                        hasExponent = true;
                        sb.Append(doc[pos]);
                        Move();
                        if (pos < doc.Length && (doc[pos] == '+' || doc[pos] == '-'))
                        {
                            sb.Append(doc[pos]);
                            Move();
                        }
                        while (pos < doc.Length && char.IsDigit(doc[pos]))
                        {
                            sb.Append(doc[pos]);
                            Move();
                        }
                    }
                    if (pos < doc.Length && (doc[pos] == 'f' || doc[pos] == 'F'))
                    {
                        hasFloatSuffix = true;
                        Move();
                    }
                    EmitToken(hasExponent || hasFloatSuffix ? OWLManchesterTokenType.FloatNumber
                                                            : hasDot ? OWLManchesterTokenType.DecimalNumber
                                                                     : OWLManchesterTokenType.IntegerNumber, sb.ToString(), tokLine, tokCol);
                    continue;
                }

                //Word: bare name, section keyword ("word:") or prefixed name ("word:local")
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
                            EmitToken(OWLManchesterTokenType.PrefixedName, sb.ToString(), tokLine, tokCol);
                        }
                        else
                            EmitToken(OWLManchesterTokenType.SectionKeyword, $"{sb}:", tokLine, tokCol);
                    }
                    else
                        EmitToken(OWLManchesterTokenType.Name, sb.ToString(), tokLine, tokCol);
                    continue;
                }

                throw new OWLException($"Cannot tokenize OWL2/Manchester document: unexpected character '{c}' at line {tokLine}, column {tokCol}");
            }

            EmitToken(OWLManchesterTokenType.EndOfDocument, string.Empty, line, col);
            return tokens;
        }
        #endregion
    }

    #region ManchesterToken
    /// <summary>
    /// OWLManchesterTokenType enumerates the kinds of token emitted by the Manchester lexer
    /// </summary>
    internal enum OWLManchesterTokenType
    {
        /* IRI enclosed in angular brackets (value carries the IRI without brackets) */
        FullIRI = 1,
        /* Prefixed name, e.g: "pz:Pizza" (value carries the whole "prefix:local" form) */
        PrefixedName = 2,
        /* Anonymous individual, e.g: "_:idv1" (value carries the local part without "_:") */
        NodeID = 3,
        /* Word ending with ':' followed by a non-name character, e.g: "Class:", "SubClassOf:" */
        SectionKeyword = 4,
        /* Bare word, e.g: "some", "and", "Functional", "o" (interpreted contextually by the parser) */
        Name = 5,
        /* Quoted string (value carries the unescaped content without quotes) */
        QuotedString = 6,
        IntegerNumber = 7,
        DecimalNumber = 8,
        FloatNumber = 9,
        /* Language tag, e.g: "@it" (value carries the tag without '@') */
        LanguageTag = 10,
        /* Datatype suffix separator "^^" */
        DoubleCaret = 11,
        Comma = 12,
        OpenParenthesis = 13,
        CloseParenthesis = 14,
        OpenBrace = 15,
        CloseBrace = 16,
        OpenBracket = 17,
        CloseBracket = 18,
        /* Facet comparators */
        LessOrEqual = 19,
        GreaterOrEqual = 20,
        LessThan = 21,
        GreaterThan = 22,
        EndOfDocument = 23
    }

    /// <summary>
    /// OWLManchesterToken is a lexical unit of an OWL2/Manchester document, carrying its position for error reporting
    /// </summary>
    internal sealed class OWLManchesterToken
    {
        #region Properties
        /// <summary>
        /// Kind of this token
        /// </summary>
        internal OWLManchesterTokenType Type { get; set; }

        /// <summary>
        /// Textual payload of this token (meaning depends on <see cref="Type"/>: see OWLManchesterTokenType members)
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