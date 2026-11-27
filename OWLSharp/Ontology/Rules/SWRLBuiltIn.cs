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
using RDFSharp.Query;
using System;
using System.Data;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLBuiltIn is a special type of atom that invokes a built-in predicate to perform operations like mathematical calculations,
    /// string manipulations, comparisons, or other computational functions on data values.
    /// Built-in atoms are identified by IRIs from predefined libraries (like swrlb:greaterThan, swrlb:add, or swrlb:contains)
    /// and allow rules to include procedural computations beyond the declarative expressiveness of standard OWL,
    /// enabling complex data processing and conditional logic within rule antecedents.
    /// </summary>
    [XmlRoot("BuiltInAtom")]
    public class SWRLBuiltIn
    {
        #region Properties
        /// <summary>
        /// The IRI of the built-in (e.g: http://www.w3.org/2001/XMLSchema#date)
        /// </summary>
        [XmlAttribute(DataType="anyURI")]
        public string IRI { get; set; }

        /// <summary>
        /// The set of positional arguments on which the predicate of the built-in is applied
        /// </summary>
        [XmlElement(typeof(SWRLIndividualArgument), ElementName="NamedIndividual")]
        [XmlElement(typeof(SWRLLiteralArgument), ElementName="Literal")]
        [XmlElement(typeof(SWRLVariableArgument), ElementName="Variable")]
        public List<SWRLArgument> Arguments { get; set; }

        /// <summary>
        /// Signature of the function which every SWRL built-in must respect to be eligible for evaluation
        /// </summary>
        internal delegate bool BuiltinEvaluator(DataRow antecedentResultsRow);
        /// <summary>
        /// Function which is executed by the SWRL engine for evaluating the built-in
        /// </summary>
        [XmlIgnore]
        internal BuiltinEvaluator EvaluatorFunction { get; set;}

        /// <summary>
        /// Indicates that this built-in is custom and provides its own semantic
        /// </summary>
        [XmlIgnore]
        internal bool IsCustom => !string.IsNullOrEmpty(IRI)
                                    && !IRI.StartsWith(RDFVocabulary.SWRL.SWRLB.BASE_URI, StringComparison.Ordinal)
                                    && !IRI.StartsWith("https://github.com/mdesalvo/OWLSharp#", StringComparison.Ordinal);

        /// <summary>
        /// Indicates that this built-in is an extension internally developed
        /// </summary>
        [XmlIgnore]
        internal bool IsExtension => !string.IsNullOrEmpty(IRI)
                                       && IRI.StartsWith("https://github.com/mdesalvo/OWLSharp#", StringComparison.Ordinal);
        #endregion

        #region Ctors
        internal SWRLBuiltIn()
            => Arguments = new List<SWRLArgument>();

        /// <summary>
        /// Builds a built-in with the given predicate IRI, applying the given evaluator function on the given set of positional arguments
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public SWRLBuiltIn(Func<DataRow,bool> evaluator, RDFResource iri, params SWRLArgument[] arguments)
        {
            EvaluatorFunction = evaluator != null ? new BuiltinEvaluator(evaluator)
                                                  : throw new SWRLException($"Cannot create custom SWRL builtIn because: '{nameof(evaluator)}' is null");
            IRI = iri?.ToString() ?? throw new SWRLException($"Cannot create custom SWRL builtIn because: '{nameof(iri)}' is null");
            Arguments = arguments?.ToList() ?? Enumerable.Empty<SWRLArgument>().ToList();
        }
        #endregion

        #region Factory
        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#abs built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Abs(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#abs",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:abs builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:abs builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#add built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Add(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#add",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:add builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:add builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#booleanNot built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn BooleanNot(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#booleanNot",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:booleanNot builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:booleanNot builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#ceiling built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Ceiling(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#ceiling",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:ceiling builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:ceiling builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#contains built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Contains(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#contains",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:contains builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:contains builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#containsIgnoreCase built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn ContainsIgnoreCase(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#containsIgnoreCase",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:containsIgnoreCase builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:containsIgnoreCase builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#cos built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Cos(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#cos",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:cos builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:cos builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#date built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Date(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#date",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:date builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:date builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#dateTime built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn DateTime(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#dateTime",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:dateTime builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:dateTime builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#dayTimeDuration built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn DayTimeDuration(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#dayTimeDuration",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:dayTimeDuration builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:dayTimeDuration builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#divide built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Divide(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#divide",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:divide builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:divide builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#endsWith built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn EndsWith(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#endsWith",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:endsWith builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:endsWith builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#equal built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Equal(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#equal",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:equal builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:equal builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#floor built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Floor(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#floor",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:floor builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:floor builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#greaterThan built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn GreaterThan(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#greaterThan",
                    Arguments = new List<SWRLArgument>
                    {
                            leftArg ?? throw new SWRLException("Cannot create swrlb:greaterThan builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create swrlb:greaterThan builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#greaterThanOrEqual built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn GreaterThanOrEqual(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual",
                    Arguments = new List<SWRLArgument>
                    {
                            leftArg ?? throw new SWRLException("Cannot create swrlb:greaterThanOrEqual builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create swrlb:greaterThanOrEqual builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#integerDivide built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn IntegerDivide(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#integerDivide",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:integerDivide builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:integerDivide builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds an extension https://github.com/mdesalvo/OWLSharp#langMatches built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn LangMatches(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "https://github.com/mdesalvo/OWLSharp#langMatches",
                    Arguments = new List<SWRLArgument>
                        {
                            leftArg ?? throw new SWRLException("Cannot create owlsharp:langMatches builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create owlsharp:langMatches builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#lessThan built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn LessThan(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#lessThan",
                    Arguments = new List<SWRLArgument>
                    {
                            leftArg ?? throw new SWRLException("Cannot create swrlb:lessThan builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create swrlb:lessThan builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#lessThanOrEqual built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn LessThanOrEqual(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                    Arguments = new List<SWRLArgument>
                    {
                            leftArg ?? throw new SWRLException("Cannot create swrlb:lessThanOrEqual builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create swrlb:lessThanOrEqual builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#lowercase built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn LowerCase(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#lowerCase",
                    Arguments = new List<SWRLArgument>
                    {
                            leftArg ?? throw new SWRLException("Cannot create swrlb:lowerCase builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create swrlb:lowerCase builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#matches built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Matches(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#matches",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:matches builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:matches builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#mod built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Mod(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#mod",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:mod builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:mod builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#multiply built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Multiply(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#multiply",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:multiply builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:multiply builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#normalizeSpace built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn NormalizeSpace(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#normalizeSpace",
                    Arguments = new List<SWRLArgument>
                    {
                            leftArg ?? throw new SWRLException("Cannot create swrlb:normalizeSpace builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create swrlb:normalizeSpace builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#notEqual built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn NotEqual(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
                    Arguments = new List<SWRLArgument>
                    {
                            leftArg ?? throw new SWRLException("Cannot create swrlb:notEqual builtIn because: left argument is null"),
                            rightArg ?? throw new SWRLException("Cannot create swrlb:notEqual builtIn because: right argument is null")
                        }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#pow built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Pow(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#pow",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:pow builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:pow builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#replace built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Replace(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#replace",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:replace builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:replace builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#round built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Round(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#round",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:round builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:round builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#roundHalfToEven built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn RoundHalfToEven(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#roundHalfToEven",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:roundHalfToEven builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:roundHalfToEven builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#sin built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Sin(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#sin",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:sin builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:sin builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#startsWith built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn StartsWith(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#startsWith",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:startsWith builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:startsWith builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#stringConcat built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn StringConcat(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#stringConcat",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:stringConcat builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:stringConcat builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn StringEqualIgnoreCase(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:stringEqualIgnoreCase builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:stringEqualIgnoreCase builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#stringLength built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn StringLength(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#stringLength",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:stringLength builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:stringLength builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#substring built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Substring(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#substring",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:substring builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:substring builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#substringAfter built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn SubstringAfter(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#substringAfter",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:substringAfter builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:substringAfter builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#substringBefore built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn SubstringBefore(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#substringBefore",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:substringBefore builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:substringBefore builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#subtract built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Subtract(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#subtract",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:subtract builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:subtract builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#tan built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Tan(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#tan",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:tan builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:tan builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#time built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn Time(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#time",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:time builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:time builtIn because: right arguments are null")).ToList()
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#unaryMinus built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn UnaryMinus(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:unaryMinus builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:unaryMinus builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#unaryPlus built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn UnaryPlus(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#unaryPlus",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:unaryPlus builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:unaryPlus builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#upperCase built-in
        /// </summary>
        /// <remarks>When the first argument is a variable bound by no atom, the built-in binds it to the computed value.</remarks>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn UpperCase(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#upperCase",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create swrlb:upperCase builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create swrlb:upperCase builtIn because: right argument is null")
                    }
                };

        /// <summary>
        /// Builds a standard http://www.w3.org/2003/11/swrlb#yearMonthDuration built-in
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public static SWRLBuiltIn YearMonthDuration(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#yearMonthDuration",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:yearMonthDuration builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:yearMonthDuration builtIn because: right arguments are null")).ToList()
                };
        #endregion

        #region Interfaces
        /// <summary>
        /// Gets the string representation of the built-in
        /// </summary>
        public override string ToString()
        {
            #region Guards
            if (Arguments?.Count == 0)
                return string.Empty;
            #endregion

            StringBuilder sb = new StringBuilder();

            //Predicate
            if (IsCustom)
            {
                sb.Append(IRI);
            }
            else
            {
                sb.Append(IsExtension ? "owlsharp:" : "swrlb:");
                sb.Append(new Uri(IRI).GetShortUri());
            }
            sb.Append('(');

            //Arguments
            for (int i=0; i<Arguments.Count; i++)
            {
                if (i>0)
                    sb.Append(',');

                switch (Arguments[i])
                {
                    case SWRLIndividualArgument leftArgumentIndividual:
                        sb.Append($"{leftArgumentIndividual.GetResource().URI.GetShortUri()}");
                        break;
                    case SWRLLiteralArgument leftArgumentLiteral:
                        sb.Append($"{RDFQueryPrinter.PrintPatternMember(leftArgumentLiteral.GetLiteral(), RDFNamespaceRegister.Instance.Register)}");
                        break;
                    case SWRLVariableArgument leftArgumentVariable:
                        sb.Append($"{RDFQueryPrinter.PrintPatternMember(leftArgumentVariable.GetVariable(), RDFNamespaceRegister.Instance.Register)}");
                        break;
                }
            }

            sb.Append(')');
            return sb.ToString();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        internal RDFTable EvaluateOnAntecedent(RDFTable antecedentResults)
        {
            RDFTable filteredTable = antecedentResults.Clone();

            //Binding built-ins compute their first argument from the others while predicate built-ins only test the row: the IRI selects the evaluator once for all rows
            Func<RDFTableRow, List<SWRLArgument>, RDFLiteral> literalComputer = null;
            Func<RDFTableRow, List<SWRLArgument>, bool> rowEvaluator = null;
            //Custom built-ins (and unregistered IRIs) match no case: they are resolved row by row against the register
            switch (IRI)
            {
                case "http://www.w3.org/2003/11/swrlb#abs":
                    literalComputer = SWRLAbsBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#add":
                    literalComputer = SWRLAddBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#booleanNot":
                    literalComputer = SWRLBooleanNotBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#ceiling":
                    literalComputer = SWRLCeilingBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#cos":
                    literalComputer = SWRLCosBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#divide":
                    literalComputer = SWRLDivideBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#floor":
                    literalComputer = SWRLFloorBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#integerDivide":
                    literalComputer = SWRLIntegerDivideBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#lowerCase":
                    literalComputer = SWRLLowerCaseBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#mod":
                    literalComputer = SWRLModBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#multiply":
                    literalComputer = SWRLMultiplyBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#normalizeSpace":
                    literalComputer = SWRLNormalizeSpaceBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#pow":
                    literalComputer = SWRLPowBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#replace":
                    literalComputer = SWRLReplaceBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#round":
                    literalComputer = SWRLRoundBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#roundHalfToEven":
                    literalComputer = SWRLRoundHalfToEvenBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#sin":
                    literalComputer = SWRLSinBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#stringConcat":
                    literalComputer = SWRLStringConcatBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#stringLength":
                    literalComputer = SWRLStringLengthBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#substring":
                    literalComputer = SWRLSubstringBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#substringAfter":
                    literalComputer = SWRLSubstringAfterBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#substringBefore":
                    literalComputer = SWRLSubstringBeforeBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#subtract":
                    literalComputer = SWRLSubtractBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#tan":
                    literalComputer = SWRLTanBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#unaryMinus":
                    literalComputer = SWRLUnaryMinusBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#unaryPlus":
                    literalComputer = SWRLUnaryPlusBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#upperCase":
                    literalComputer = SWRLUpperCaseBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#contains":
                    rowEvaluator = SWRLContainsBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#containsIgnoreCase":
                    rowEvaluator = SWRLContainsIgnoreCaseBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#date":
                    rowEvaluator = SWRLDateBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#dateTime":
                    rowEvaluator = SWRLDateTimeBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#dayTimeDuration":
                    rowEvaluator = SWRLDayTimeDurationBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#endsWith":
                    rowEvaluator = SWRLEndsWithBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#equal":
                    rowEvaluator = SWRLEqualBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#greaterThan":
                    rowEvaluator = SWRLGreaterThanBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual":
                    rowEvaluator = SWRLGreaterThanOrEqualBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#lessThan":
                    rowEvaluator = SWRLLessThanBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#lessThanOrEqual":
                    rowEvaluator = SWRLLessThanOrEqualBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#matches":
                    rowEvaluator = SWRLMatchesBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#notEqual":
                    rowEvaluator = SWRLNotEqualBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#startsWith":
                    rowEvaluator = SWRLStartsWithBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase":
                    rowEvaluator = SWRLStringEqualIgnoreCaseBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#time":
                    rowEvaluator = SWRLTimeBuiltIn.EvaluateOnAntecedent;
                    break;
                case "http://www.w3.org/2003/11/swrlb#yearMonthDuration":
                    rowEvaluator = SWRLYearMonthDurationBuiltIn.EvaluateOnAntecedent;
                    break;
                case "https://github.com/mdesalvo/OWLSharp#langMatches":
                    rowEvaluator = SWRLLangMatchesBuiltIn.EvaluateOnAntecedent;
                    break;
            }

            //A first variable bound by no previous atom receives the computed value in a new column
            string bindingColumnName = null;
            if (literalComputer != null
                 && Arguments?.Count > 0
                 && Arguments[0] is SWRLVariableArgument firstArgumentVariable
                 && !antecedentResults.HasColumn(firstArgumentVariable.GetVariable().ToString()))
            {
                bindingColumnName = firstArgumentVariable.GetVariable().ToString();
                filteredTable.AddColumn(bindingColumnName);
            }

            //Custom built-ins expose a public DataRow-based evaluator function: this DataTable is materialized
            //lazily (at most once), only if a custom built-in is actually encountered while scanning the rows.
            DataTable lazyDataTableForCustomBuiltIns = null;

            int rowIndex = -1;
            //Iterate the rows of the antecedent results table
            foreach (RDFTableRow currentRow in antecedentResults.Rows)
            {
                rowIndex++;
                try
                {
                    bool keepRow;
                    RDFLiteral computedLiteral = null;
                    if (literalComputer != null)
                    {
                        //An undefined computation never satisfies the built-in
                        computedLiteral = literalComputer(currentRow, Arguments);
                        keepRow = computedLiteral != null;
                        //Predicate mode also requires the first argument to hold the computed value, compared by value as SPARQL does: an unbound one never does
                        if (keepRow && bindingColumnName == null)
                        {
                            RDFPatternMember firstPatternMember = SWRLHelper.GetArgumentValue(currentRow, Arguments[0]);
                            keepRow = firstPatternMember != null && RDFQueryUtilities.CompareRDFPatternMembers(firstPatternMember, computedLiteral) == 0;
                        }
                    }
                    else if (rowEvaluator != null)
                    {
                        keepRow = rowEvaluator(currentRow, Arguments);
                    }
                    else
                    {
                        SWRLBuiltIn customBuiltIn = SWRLBuiltInRegister.GetBuiltIn(IRI)
                                                      ?? throw new NotImplementedException($"unregistered IRI {IRI}");
                        //Assign the built-in arguments before evaluation
                        customBuiltIn.Arguments = Arguments;
                        //Custom built-ins are public API and evaluate against a DataRow: materialize the
                        //antecedent results into a DataTable (once) so the corresponding row can be handed to it
                        if (lazyDataTableForCustomBuiltIns == null)
                            lazyDataTableForCustomBuiltIns = antecedentResults.ToDataTable();
                        //Evaluate the built-in on the current row
                        keepRow = customBuiltIn.EvaluatorFunction(lazyDataTableForCustomBuiltIns.Rows[rowIndex]);
                        //Cleanup the built-in arguments after evaluation
                        customBuiltIn.Arguments = null;
                    }

                    //If current row has satisfied the builtIn, keep it in the filtered result table
                    if (keepRow)
                    {
                        string[] cells = new string[filteredTable.ColumnsCount];
                        for (int c = 0; c < antecedentResults.ColumnsCount; c++)
                            cells[c] = currentRow[c];
                        //In binding mode the computed value fills the new column, which is the last one
                        if (bindingColumnName != null)
                            cells[cells.Length - 1] = computedLiteral.ToString();
                        filteredTable.AddRow(cells);
                    }
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        /* This exception is for unsupported (or not yet implemented) builtIns */
                        case NotImplementedException nex:
                            throw new SWRLException($"Cannot evaluate SWRL builtIn with IRI '{IRI}' because: {nex.Message}", nex);
                        /* This exception is for builtIns violating required n-arity of arguments */
                        case ArgumentException aex:
                            throw new SWRLException($"Cannot evaluate SWRL builtIn with IRI '{IRI}' because: {aex.Message}", aex);
                    }
                    /* NO-OP for every other recoverable situations */
                }
            }

            return filteredTable;
        }

        /// <summary>
        /// Exports this SWRL built-in to an equivalent RDFGraph object
        /// </summary>
        internal RDFGraph ToRDFGraph(RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource builtinBN = new RDFResource();
            atomsList.AddItem(builtinBN);

            graph.AddTriple(new RDFTriple(builtinBN, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM));
            graph.AddTriple(new RDFTriple(builtinBN, RDFVocabulary.SWRL.BUILTIN_PROP, new RDFResource(IRI)));

            RDFCollection builtinArguments = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource, true);
            foreach (SWRLArgument argument in Arguments)
            {
                switch (argument)
                {
                    case SWRLVariableArgument argVar:
                    {
                        RDFResource argVarIRI = new RDFResource(argVar.IRI);
                        builtinArguments.AddItemInternal(argVarIRI);
                        graph.AddTriple(new RDFTriple(argVarIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE));
                        break;
                    }
                    case SWRLIndividualArgument argIdv:
                        builtinArguments.AddItemInternal(argIdv.GetResource());
                        break;
                    case SWRLLiteralArgument argLit:
                        builtinArguments.AddItemInternal(argLit.GetLiteral());
                        break;
                }
            }
            graph = graph.UnionWith(SWRLRule.ReifySWRLCollection(builtinArguments, false));
            graph.AddTriple(new RDFTriple(builtinBN, RDFVocabulary.SWRL.ARGUMENTS, builtinArguments.ReificationSubject));

            return graph;
        }
        #endregion
    }
}