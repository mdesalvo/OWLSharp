/*
   Copyright 2014-2024 Marco De Salvo

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
using System.Collections;
using System.Linq;

namespace OWLSharp.Ontology.Rules
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
        #endregion

        #region Ctors
        internal SWRLBuiltIn()
            => Arguments = new List<SWRLArgument>();

        public static SWRLBuiltIn Abs(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#abs",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:abs builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:abs builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Add(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#add",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:add builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:add builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn BooleanNot(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#booleanNot",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:booleanNot builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:booleanNot builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Ceiling(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#ceiling",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:ceiling builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:ceiling builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Contains(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#contains",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:contains builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:contains builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn ContainsIgnoreCase(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#containsIgnoreCase",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:containsIgnoreCase builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:containsIgnoreCase builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Cos(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#cos",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:cos builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:cos builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Divide(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#divide",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:divide builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:divide builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn EndsWith(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#endsWith",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:endsWith builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:endsWith builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Equal(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#equal",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:equal builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:equal builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Floor(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#floor",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:floor builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:floor builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn GreaterThan(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#greaterThan",
                    Arguments = new List<SWRLArgument>()
                        {
                            leftArg ?? throw new OWLException("Cannot create swrlb:greaterThan builtIn because: left argument is null"),
                            rightArg ?? throw new OWLException("Cannot create swrlb:greaterThan builtIn because: right argument is null")
                        }
                };

        public static SWRLBuiltIn GreaterThanOrEqual(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual",
                    Arguments = new List<SWRLArgument>()
                        {
                            leftArg ?? throw new OWLException("Cannot create swrlb:greaterThanOrEqual builtIn because: left argument is null"),
                            rightArg ?? throw new OWLException("Cannot create swrlb:greaterThanOrEqual builtIn because: right argument is null")
                        }
                };

        public static SWRLBuiltIn IntegerDivide(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#integerDivide",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:integerDivide builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:integerDivide builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn LessThan(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#lessThan",
                    Arguments = new List<SWRLArgument>()
                        {
                            leftArg ?? throw new OWLException("Cannot create swrlb:lessThan builtIn because: left argument is null"),
                            rightArg ?? throw new OWLException("Cannot create swrlb:lessThan builtIn because: right argument is null")
                        }
                };

        public static SWRLBuiltIn LessThanOrEqual(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                    Arguments = new List<SWRLArgument>()
                        {
                            leftArg ?? throw new OWLException("Cannot create swrlb:lessThanOrEqual builtIn because: left argument is null"),
                            rightArg ?? throw new OWLException("Cannot create swrlb:lessThanOrEqual builtIn because: right argument is null")
                        }
                };

        public static SWRLBuiltIn LowerCase(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#lowerCase",
                    Arguments = new List<SWRLArgument>()
                        {
                            leftArg ?? throw new OWLException("Cannot create swrlb:lowerCase builtIn because: left argument is null"),
                            rightArg ?? throw new OWLException("Cannot create swrlb:lowerCase builtIn because: right argument is null")
                        }
                };

        public static SWRLBuiltIn Matches(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#matches",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:matches builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:matches builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Mod(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#mod",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:mod builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:mod builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Multiply(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#multiply",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:multiply builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:multiply builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn NotEqual(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
                    Arguments = new List<SWRLArgument>()
                        {
                            leftArg ?? throw new OWLException("Cannot create swrlb:notEqual builtIn because: left argument is null"),
                            rightArg ?? throw new OWLException("Cannot create swrlb:notEqual builtIn because: right argument is null")
                        }
                };

        public static SWRLBuiltIn Pow(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#pow",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:pow builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:pow builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Round(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#round",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:round builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:round builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn RoundHalfToEven(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#roundHalfToEven",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:roundHalfToEven builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:roundHalfToEven builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Sin(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#sin",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:sin builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:sin builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn StartsWith(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#startsWith",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:startsWith builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:startsWith builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Subtract(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#subtract",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:subtract builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:subtract builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Tan(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#tan",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:tan builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:tan builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn UnaryMinus(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:unaryMinus builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:unaryMinus builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn UnaryPlus(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#unaryPlus",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:unaryPlus builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:unaryPlus builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn UpperCase(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#upperCase",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:upperCase builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:upperCase builtIn because: right argument is null")
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
            sb.Append("swrlb:");
            sb.Append(RDFModelUtilities.GetShortUri(new Uri(IRI)));
            sb.Append("(");

            //Arguments
            for (int i=0; i<Arguments.Count; i++)
            {
                if (i>0)
                    sb.Append(',');

                if (Arguments[i] is SWRLIndividualArgument leftArgumentIndividual)
                    sb.Append($"{RDFModelUtilities.GetShortUri(leftArgumentIndividual.GetResource().URI)}");
                else if (Arguments[i] is SWRLLiteralArgument leftArgumentLiteral)
                    sb.Append($"{RDFQueryPrinter.PrintPatternMember(leftArgumentLiteral.GetLiteral(), RDFNamespaceRegister.Instance.Register)}");
                else if (Arguments[i] is SWRLVariableArgument leftArgumentVariable)
                    sb.Append($"{RDFQueryPrinter.PrintPatternMember(leftArgumentVariable.GetVariable(), RDFNamespaceRegister.Instance.Register)}");
            }

            sb.Append(")");
            return sb.ToString();
        }
        #endregion

        #region Methods
        internal DataTable EvaluateOnAntecedent(DataTable antecedentResults)
        {
            DataTable filteredTable = antecedentResults.Clone();

            //Iterate the rows of the antecedent results table
            bool keepRow;
            IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
            while (rowsEnum.MoveNext())
                try
                {
                    DataRow currentRow = (DataRow)rowsEnum.Current;
                    
                    //Determine the builtIn to be engaged for current row's evaluation
                    switch (IRI)
                    {
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
                        case "http://www.w3.org/2003/11/swrlb#notEqual":
                            keepRow = SWRLNotEqualBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#pow":
                            keepRow = SWRLPowBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
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
                        case "http://www.w3.org/2003/11/swrlb#subtract":
                            keepRow = SWRLSubtractBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#tan":
                            keepRow = SWRLTanBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
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

                        //Unsupported builtIns must generate an explicit exception
                        default:
                            throw new NotImplementedException($"unsupported IRI {IRI}");
                    }

                    //If current row has satisfied the builtIn, keep it in the filtered result table
                    if (keepRow)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                        filteredTable.Rows.Add(newRow);
                    }
                }
                catch (Exception ex) 
                { 
                    /* This exception is for unsupported (or not yet implemented) builtIns */
                    if (ex is NotImplementedException nex)
                        throw new OWLException($"Cannot evaluate SWRL builtIn because: {nex.Message}", nex);
                    /* This exception is for builtIns violating required n-arity of arguments */
                    if (ex is ArgumentException aex)
                        throw new OWLException($"Cannot evaluate SWRL builtIn with IRI {IRI} because: {ex.Message}", aex);

                    /* NO-OP for every other recoverable situations */
                }

            return filteredTable;
        }
        #endregion
    }
}