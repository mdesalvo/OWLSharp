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
    [XmlRoot("BuiltInAtom")]
    public class SWRLBuiltIn
    {
        #region Properties
        [XmlAttribute(DataType="anyURI")]
        public string IRI { get; set; }

        [XmlElement(typeof(SWRLIndividualArgument), ElementName="NamedIndividual")]
        [XmlElement(typeof(SWRLLiteralArgument), ElementName="Literal")]
        [XmlElement(typeof(SWRLVariableArgument), ElementName="Variable")]
        public List<SWRLArgument> Arguments { get; set; }

        internal delegate bool BuiltinEvaluator(DataRow antecedentResultsRow);
        [XmlIgnore]
        internal BuiltinEvaluator EvaluatorFunction { get; set;}

        [XmlIgnore]
        internal bool IsCustom => !string.IsNullOrEmpty(IRI)
                                    && !IRI.StartsWith(RDFVocabulary.SWRL.SWRLB.BASE_URI, StringComparison.Ordinal)
                                    && !IRI.StartsWith("https://github.com/mdesalvo/OWLSharp#", StringComparison.Ordinal);
        [XmlIgnore]
        internal bool IsExtension => !string.IsNullOrEmpty(IRI)
                                       && IRI.StartsWith("https://github.com/mdesalvo/OWLSharp#", StringComparison.Ordinal);
        #endregion

        #region Ctors
        internal SWRLBuiltIn()
            => Arguments = new List<SWRLArgument>();

        //Custom BuiltIns

        public SWRLBuiltIn(Func<DataRow,bool> evaluator, RDFResource iri, params SWRLArgument[] arguments)
        {
            EvaluatorFunction = evaluator != null ? new BuiltinEvaluator(evaluator)
                                                  : throw new SWRLException("Cannot create custom SWRL builtIn because: evaluator is null");
            IRI = iri?.ToString() ?? throw new SWRLException("Cannot create custom SWRL builtIn because: iri is null");
            Arguments = arguments?.ToList() ?? Enumerable.Empty<SWRLArgument>().ToList();
        }

        //Official BuiltIns

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

        public static SWRLBuiltIn Add(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#add",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:add builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:add builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn Date(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#date",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:date builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:date builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn DateTime(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#dateTime",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:dateTime builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:dateTime builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn DayTimeDuration(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#dayTimeDuration",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:dayTimeDuration builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:dayTimeDuration builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Divide(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#divide",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:divide builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:divide builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn IntegerDivide(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#integerDivide",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:integerDivide builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:integerDivide builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn Matches(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#matches",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:matches builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:matches builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Mod(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#mod",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:mod builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:mod builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Multiply(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#multiply",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:multiply builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:multiply builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn Pow(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#pow",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:pow builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:pow builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Replace(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#replace",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:replace builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:replace builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn StringConcat(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#stringConcat",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:stringConcat builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:stringConcat builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn Substring(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#substring",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:substring builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:substring builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn SubstringAfter(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            => new SWRLBuiltIn
            {
                IRI = "http://www.w3.org/2003/11/swrlb#substringAfter",
                Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:substringAfter builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:substringAfter builtIn because: right arguments are null")).ToList()
            };

        public static SWRLBuiltIn SubstringBefore(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#substringBefore",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:substringBefore builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:substringBefore builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Subtract(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#subtract",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:subtract builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:subtract builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn Time(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#time",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:time builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:time builtIn because: right arguments are null")).ToList()
                };

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

        public static SWRLBuiltIn YearMonthDuration(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn
            {
                    IRI = "http://www.w3.org/2003/11/swrlb#yearMonthDuration",
                    Arguments = new List<SWRLArgument> { leftArg ?? throw new SWRLException("Cannot create swrlb:yearMonthDuration builtIn because: left argument is null") }.Concat(rightArgs?.ToList() ?? throw new SWRLException("Cannot create swrlb:yearMonthDuration builtIn because: right arguments are null")).ToList()
                };

        //Extension BuiltIns

        public static SWRLBuiltIn EXTLangMatches(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn
            {
                    IRI = "https://github.com/mdesalvo/OWLSharp#langMatches",
                    Arguments = new List<SWRLArgument>
                    {
                        leftArg ?? throw new SWRLException("Cannot create owlsharp:langMatches builtIn because: left argument is null"),
                        rightArg ?? throw new SWRLException("Cannot create owlsharp:langMatches builtIn because: right argument is null")
                    }
                };
        #endregion

        #region Interfaces
        public override string ToString()
        {
            #region Guards
            if (Arguments?.Count == 0)
                return string.Empty;
            #endregion

            StringBuilder sb = new StringBuilder();

            //Predicate
            if (IsCustom)
                sb.Append(IRI);
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
        internal DataTable EvaluateOnAntecedent(DataTable antecedentResults)
        {
            DataTable filteredTable = antecedentResults.Clone();

            //Iterate the rows of the antecedent results table
            foreach (DataRow currentRow in antecedentResults.Rows)
                try
                {
                    bool keepRow;
                    switch (IRI)
                    {
                        //Official BuiltIns => handle them directly
                        case "http://www.w3.org/2003/11/swrlb#abs":
                            keepRow = SWRLAbsBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#add":
                            keepRow = SWRLAddBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#booleanNot":
                            keepRow = SWRLBooleanNotBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#ceiling":
                            keepRow = SWRLCeilingBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#contains":
                            keepRow = SWRLContainsBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#containsIgnoreCase":
                            keepRow = SWRLContainsIgnoreCaseBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#cos":
                            keepRow = SWRLCosBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#date":
                            keepRow = SWRLDateBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#dateTime":
                            keepRow = SWRLDateTimeBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#dayTimeDuration":
                            keepRow = SWRLDayTimeDurationBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#divide":
                            keepRow = SWRLDivideBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#endsWith":
                            keepRow = SWRLEndsWithBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#equal":
                            keepRow = SWRLEqualBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#floor":
                            keepRow = SWRLFloorBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#greaterThan":
                            keepRow = SWRLGreaterThanBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual":
                            keepRow = SWRLGreaterThanOrEqualBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#integerDivide":
                            keepRow = SWRLIntegerDivideBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#lessThan":
                            keepRow = SWRLLessThanBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#lessThanOrEqual":
                            keepRow = SWRLLessThanOrEqualBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#lowerCase":
                            keepRow = SWRLLowerCaseBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#matches":
                            keepRow = SWRLMatchesBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#mod":
                            keepRow = SWRLModBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#multiply":
                            keepRow = SWRLMultiplyBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#normalizeSpace":
                            keepRow = SWRLNormalizeSpaceBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#notEqual":
                            keepRow = SWRLNotEqualBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#pow":
                            keepRow = SWRLPowBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#replace":
                            keepRow = SWRLReplaceBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#round":
                            keepRow = SWRLRoundBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#roundHalfToEven":
                            keepRow = SWRLRoundHalfToEvenBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#sin":
                            keepRow = SWRLSinBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#startsWith":
                            keepRow = SWRLStartsWithBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#stringConcat":
                            keepRow = SWRLStringConcatBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase":
                            keepRow = SWRLStringEqualIgnoreCaseBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#stringLength":
                            keepRow = SWRLStringLengthBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#substring":
                            keepRow = SWRLSubstringBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#substringAfter":
                            keepRow = SWRLSubstringAfterBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#substringBefore":
                            keepRow = SWRLSubstringBeforeBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#subtract":
                            keepRow = SWRLSubtractBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#tan":
                            keepRow = SWRLTanBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#time":
                            keepRow = SWRLTimeBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#unaryMinus":
                            keepRow = SWRLUnaryMinusBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#unaryPlus":
                            keepRow = SWRLUnaryPlusBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#upperCase":
                            keepRow = SWRLUpperCaseBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#yearMonthDuration":
                            keepRow = SWRLYearMonthDurationBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;

                        //Extension BuiltIns => handle them directly
                        case "https://github.com/mdesalvo/OWLSharp#langMatches":
                            keepRow = SWRLEXTLangMatchesBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;

                        //Custom BuiltIns => handle them via register
                        default:
                            SWRLBuiltIn customBuiltIn = SWRLBuiltInRegister.GetBuiltIn(IRI)
                                                          ?? throw new NotImplementedException($"unsupported IRI {IRI}");
                            //Since the custom builtIn may have been registered with different arguments,
                            //we must temporary swap them with ones coming from the executing builtIn
                            List<SWRLArgument> registeredArguments = customBuiltIn.Arguments;
                            customBuiltIn.Arguments = Arguments;
                            keepRow = customBuiltIn.EvaluatorFunction(currentRow);
                            customBuiltIn.Arguments = registeredArguments;
                            break;
                    }

                    //If current row has satisfied the builtIn, keep it in the filtered result table
                    if (keepRow)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow.ItemArray = currentRow.ItemArray;
                        filteredTable.Rows.Add(newRow);
                    }
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        /* This exception is for unsupported (or not yet implemented) builtIns */
                        case NotImplementedException nex:
                            throw new SWRLException($"Cannot evaluate SWRL builtIn because: {nex.Message}", nex);
                        /* This exception is for builtIns violating required n-arity of arguments */
                        case ArgumentException aex:
                            throw new SWRLException($"Cannot evaluate SWRL builtIn with IRI {IRI} because: {aex.Message}", aex);
                    }
                    /* NO-OP for every other recoverable situations */
                }

            return filteredTable;
        }

        public RDFGraph ToRDFGraph(RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource builtinBN = new RDFResource();
            atomsList.AddItem(builtinBN);

            graph.AddTriple(new RDFTriple(builtinBN, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM));
            graph.AddTriple(new RDFTriple(builtinBN, RDFVocabulary.SWRL.BUILTIN_PROP, new RDFResource(IRI)));

            RDFCollection builtinArguments = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource, true);
            foreach (SWRLArgument argument in Arguments)
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
            graph = graph.UnionWith(SWRLRule.ReifySWRLCollection(builtinArguments, false));
            graph.AddTriple(new RDFTriple(builtinBN, RDFVocabulary.SWRL.ARGUMENTS, builtinArguments.ReificationSubject));

            return graph;
        }
        #endregion
    }
}