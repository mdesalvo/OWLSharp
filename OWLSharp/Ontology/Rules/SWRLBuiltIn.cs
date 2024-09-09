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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

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

        [XmlIgnore]
        public bool IsBooleanBuiltIn
            => string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#booleanNot");
        [XmlIgnore]
        public bool IsMathBuiltIn
            => string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#abs")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#add")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#ceiling")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#cos")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#divide")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#floor")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#integerDivide")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#mod")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#multiply")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#pow")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#round")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#roundHalfToEven")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#sin")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#subtract")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#tan")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#unaryMinus")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#unaryPlus");      
        [XmlIgnore]
        public bool IsComparisonFilterBuiltIn
            => string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#equal")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#greaterThan")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#lessThanOrEqual")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#lessThan")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#notEqual");
        [XmlIgnore]
        public bool IsStringFilterBuiltIn
            => string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#contains")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#containsIgnoreCase")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#endsWith")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#lowerCase")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#matches")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#startsWith")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#stringConcat")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#stringLength")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#upperCase");
        #endregion

        #region Ctors/Factory
        internal SWRLBuiltIn() { }

        //Boolean

        public static SWRLBuiltIn BooleanNot(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#booleanNot",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument 
                }
            };
        }

        //Math

        public static SWRLBuiltIn Abs(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#abs",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument 
                }
            };
        }

        public static SWRLBuiltIn Add(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double literal)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#add",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument, 
                    new SWRLLiteralArgument(new RDFTypedLiteral(Convert.ToString(literal, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                }
            };
        }

        public static SWRLBuiltIn Ceiling(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#ceiling",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument 
                }
            };
        }

        public static SWRLBuiltIn Cos(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#cos",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument 
                }
            };
        }

        public static SWRLBuiltIn Divide(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double literal)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            if (literal == 0d)
                throw new OWLException("Cannot create built-in because given \"literal\" parameter is 0!");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#divide",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument, 
                    new SWRLLiteralArgument(new RDFTypedLiteral(Convert.ToString(literal, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                }
            };
        }

        public static SWRLBuiltIn Floor(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#floor",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn IntegerDivide(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double literal)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            if (literal == 0d)
                throw new OWLException("Cannot create built-in because given \"literal\" parameter is 0!");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#integerDivide",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument, 
                    new SWRLLiteralArgument(new RDFTypedLiteral(Convert.ToString(literal, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                }
            };
        }

        public static SWRLBuiltIn Mod(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double literal)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            if (literal == 0d)
                throw new OWLException("Cannot create built-in because given \"literal\" parameter is 0!");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#mod",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument, 
                    new SWRLLiteralArgument(new RDFTypedLiteral(Convert.ToString(literal, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                }
            };
        }

        public static SWRLBuiltIn Multiply(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double literal)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#multiply",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument, 
                    new SWRLLiteralArgument(new RDFTypedLiteral(Convert.ToString(literal, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                }
            };
        }

        public static SWRLBuiltIn Pow(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double literal)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#pow",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument, 
                    new SWRLLiteralArgument(new RDFTypedLiteral(Convert.ToString(literal, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                }
            };
        }

        public static SWRLBuiltIn Round(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#round",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn RoundHalfToEven(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#roundHalfToEven",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn Sin(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#sin",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn Subtract(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double literal)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#subtract",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument, 
                    new SWRLLiteralArgument(new RDFTypedLiteral(Convert.ToString(literal, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                }
            };
        }

        public static SWRLBuiltIn Tan(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#tan",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn UnaryMinus(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn UnaryPlus(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#unaryPlus",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        //Comparison

        public static SWRLBuiltIn Equal(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => EqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn Equal(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => EqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn Equal(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => EqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltIn EqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#equal",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn GreaterThan(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => GreaterThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn GreaterThan(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => GreaterThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn GreaterThan(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => GreaterThanInternal(leftArgument, rightArgument);
        internal static SWRLBuiltIn GreaterThanInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#greaterThan",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn GreaterThanOrEqual(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => GreaterThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn GreaterThanOrEqual(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => GreaterThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn GreaterThanOrEqual(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => GreaterThanOrEqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltIn GreaterThanOrEqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn LessThanOrEqual(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => LessThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn LessThanOrEqual(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => LessThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn LessThanOrEqual(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => LessThanOrEqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltIn LessThanOrEqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn LessThan(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => LessThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn LessThan(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => LessThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn LessThan(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => LessThanInternal(leftArgument, rightArgument);
        internal static SWRLBuiltIn LessThanInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#lessThan",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn NotEqual(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => NotEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn NotEqual(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => NotEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltIn NotEqual(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => NotEqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltIn NotEqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        //String

        public static SWRLBuiltIn Contains(SWRLVariableArgument leftArgument, string containString)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (containString == null)
                throw new OWLException("Cannot create built-in because given \"containString\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#contains",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    new SWRLLiteralArgument(new RDFPlainLiteral(containString))
                }
            };
        }

        public static SWRLBuiltIn ContainsIgnoreCase(SWRLVariableArgument leftArgument, string containString)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (containString == null)
                throw new OWLException("Cannot create built-in because given \"containString\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#containsIgnoreCase",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    new SWRLLiteralArgument(new RDFPlainLiteral(containString))
                }
            };
        }

        public static SWRLBuiltIn EndsWith(SWRLVariableArgument leftArgument, string endString)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (endString == null)
                throw new OWLException("Cannot create built-in because given \"endString\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#endsWith",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    new SWRLLiteralArgument(new RDFPlainLiteral(endString))
                }
            };
        }

        public static SWRLBuiltIn LowerCase(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#lowerCase",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn Matches(SWRLVariableArgument leftArgument, string matchString, RegexOptions matchOptions=RegexOptions.None)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (matchString == null)
                throw new OWLException("Cannot create built-in because given \"matchString\" parameter is null");
            #endregion

            StringBuilder sbMatchOptions = new StringBuilder();
            if (matchOptions.HasFlag(RegexOptions.IgnoreCase))
                sbMatchOptions.Append('i');
            if (matchOptions.HasFlag(RegexOptions.Singleline))
                sbMatchOptions.Append('s');
            if (matchOptions.HasFlag(RegexOptions.Multiline))
                sbMatchOptions.Append('m');
            if (matchOptions.HasFlag(RegexOptions.IgnorePatternWhitespace))
                sbMatchOptions.Append('x');

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#matches",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    new SWRLLiteralArgument(new RDFPlainLiteral(matchString)),
                    new SWRLLiteralArgument(new RDFPlainLiteral(sbMatchOptions.ToString()))
                }
            };
        }

        public static SWRLBuiltIn StartsWith(SWRLVariableArgument leftArgument, string startString)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (startString == null)
                throw new OWLException("Cannot create built-in because given \"startString\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#startsWith",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    new SWRLLiteralArgument(new RDFPlainLiteral(startString))
                }
            };
        }

        public static SWRLBuiltIn StringEqualIgnoreCase(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn StringConcat(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, string concatString)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#stringConcat",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument,
                    new SWRLLiteralArgument(new RDFPlainLiteral(concatString))
                }
            };
        }

        public static SWRLBuiltIn StringLength(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#stringLength",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }

        public static SWRLBuiltIn UpperCase(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (leftArgument == null)
                throw new OWLException("Cannot create built-in because given \"leftArgument\" parameter is null");
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#upperCase",
                Arguments = new List<SWRLArgument>() { 
                    leftArgument, 
                    rightArgument
                }
            };
        }
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
            #region Guards
            if (Arguments?.Count == 0)
                return new DataTable();
            #endregion

            DataTable filteredTable = antecedentResults.Clone();

            //TODO

            return filteredTable;
        }
        
        /*
        internal double? GetMathLiteralValue()
        {
            double? mathLiteralvalue = new double?();

            if (Literal?.GetLiteral() is RDFTypedLiteral literal
                 && literal.HasDecimalDatatype()
                 && double.TryParse(literal.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double litNumVal))
                mathLiteralvalue = litNumVal;

            return mathLiteralvalue;
        }

        internal void OldCode()
        {
            if (IsBooleanBuiltIn || IsMathBuiltIn)
            {
                #region Guards
                //Preliminary checks for builtin's applicability (requires arguments to be known variables)
                string leftArgumentString = Arguments.ToString();
                if (!antecedentResults.Columns.Contains(leftArgumentString))
                    return filteredTable;
                string rightArgumentString = RightArgument.ToString();
                if (!antecedentResults.Columns.Contains(rightArgumentString))
                    return filteredTable;
                #endregion

                //Iterate the rows of the antecedent result table
                double? literalNumericValue = GetMathLiteralValue();
                IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
                while (rowsEnum.MoveNext())
                {
                    try
                    {
                        //Fetch data corresponding to the built-in's arguments
                        DataRow currentRow = (DataRow)rowsEnum.Current;
                        string leftArgumentValue = currentRow[leftArgumentString].ToString();
                        string rightArgumentValue = currentRow[rightArgumentString].ToString();

                        //Transform fetched data to pattern members
                        RDFPatternMember leftArgumentPMember = RDFQueryUtilities.ParseRDFPatternMember(leftArgumentValue);
                        RDFPatternMember rightArgumentPMember = RDFQueryUtilities.ParseRDFPatternMember(rightArgumentValue);

                        #region BooleanBuiltIn
                        //Check compatibility of pattern members with the built-in (requires boolean typed literals)
                        if (leftArgumentPMember is RDFTypedLiteral leftArgumentBoolTypedLiteral
                             && leftArgumentBoolTypedLiteral.HasBooleanDatatype()
                             && rightArgumentPMember is RDFTypedLiteral rightArgumentBoolTypedLiteral
                             && rightArgumentBoolTypedLiteral.HasBooleanDatatype())
                        {
                            //Execute the built-in's math logics
                            bool leftArgumentBoolTypedLiteralValue = bool.Parse(leftArgumentBoolTypedLiteral.Value);
                            bool rightArgumentBoolTypedLiteralValue = bool.Parse(rightArgumentBoolTypedLiteral.Value);
                            bool keepRow = false;
                            switch (IRI)
                            {
                                case "http://www.w3.org/2003/11/swrlb#booleanNot":
                                    keepRow = leftArgumentBoolTypedLiteralValue == !rightArgumentBoolTypedLiteralValue;
                                    break;
                            }

                            //If the row has passed the built-in, keep it in the filtered result table
                            if (keepRow)
                            {
                                DataRow newRow = filteredTable.NewRow();
                                newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                                filteredTable.Rows.Add(newRow);
                            }
                        }
                        #endregion

                        #region MathBuiltIn
                        //Check compatibility of pattern members with the built-in (requires numeric typed literals)
                        else if (leftArgumentPMember is RDFTypedLiteral leftArgumentNumericTypedLiteral
                                  && leftArgumentNumericTypedLiteral.HasDecimalDatatype()
                                  && rightArgumentPMember is RDFTypedLiteral rightArgumentNumericTypedLiteral
                                  && rightArgumentNumericTypedLiteral.HasDecimalDatatype())
                        {
                            if (double.TryParse(leftArgumentNumericTypedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double leftArgumentNumericValue)
                                 && double.TryParse(rightArgumentNumericTypedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double rightArgumentNumericValue))
                            {
                                //Execute the built-in's math logics
                                bool keepRow = false;
                                switch (IRI)
                                {
                                    case "http://www.w3.org/2003/11/swrlb#abs":
                                        keepRow = leftArgumentNumericValue == Math.Abs(rightArgumentNumericValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#add":
                                        if (!literalNumericValue.HasValue)
                                            throw new OWLException($"Cannot evaluate arithmetic SWRLBuiltIn with predicate '{IRI}' because it doesn't have required numeric literal argument");
                                        keepRow = leftArgumentNumericValue == rightArgumentNumericValue + literalNumericValue.Value;
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#ceiling":
                                        keepRow = leftArgumentNumericValue == Math.Ceiling(rightArgumentNumericValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#cos":
                                        keepRow = leftArgumentNumericValue == Math.Cos(rightArgumentNumericValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#divide":
                                        if (!literalNumericValue.HasValue)
                                            throw new OWLException($"Cannot evaluate arithmetic SWRLBuiltIn with predicate '{IRI}' because it doesn't have required numeric literal argument");
                                        keepRow = leftArgumentNumericValue == rightArgumentNumericValue / literalNumericValue.Value;
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#floor":
                                        keepRow = leftArgumentNumericValue == Math.Floor(rightArgumentNumericValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#integerDivide":
                                        if (!literalNumericValue.HasValue)
                                            throw new OWLException($"Cannot evaluate arithmetic SWRLBuiltIn with predicate '{IRI}' because it doesn't have required numeric literal argument");
                                        keepRow = leftArgumentNumericValue == Convert.ToInt32(rightArgumentNumericValue) / Convert.ToInt32(literalNumericValue.Value);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#mod":
                                        if (!literalNumericValue.HasValue)
                                            throw new OWLException($"Cannot evaluate arithmetic SWRLBuiltIn with predicate '{IRI}' because it doesn't have required numeric literal argument");
                                        keepRow = leftArgumentNumericValue == rightArgumentNumericValue % literalNumericValue.Value;
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#multiply":
                                        if (!literalNumericValue.HasValue)
                                            throw new OWLException($"Cannot evaluate arithmetic SWRLBuiltIn with predicate '{IRI}' because it doesn't have required numeric literal argument");
                                        keepRow = leftArgumentNumericValue == rightArgumentNumericValue * literalNumericValue.Value;
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#pow":
                                        if (!literalNumericValue.HasValue)
                                            throw new OWLException($"Cannot evaluate arithmetic SWRLBuiltIn with predicate '{IRI}' because it doesn't have required numeric literal argument");
                                        keepRow = leftArgumentNumericValue == Math.Pow(rightArgumentNumericValue, literalNumericValue.Value);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#round":
                                        keepRow = leftArgumentNumericValue == Math.Round(rightArgumentNumericValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#roundHalfToEven":
                                        keepRow = leftArgumentNumericValue == Math.Round(rightArgumentNumericValue, MidpointRounding.ToEven);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#sin":
                                        keepRow = leftArgumentNumericValue == Math.Sin(rightArgumentNumericValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#subtract":
                                        if (!literalNumericValue.HasValue)
                                            throw new OWLException($"Cannot evaluate arithmetic SWRLBuiltIn with predicate '{IRI}' because it doesn't have required numeric literal argument");
                                        keepRow = leftArgumentNumericValue == rightArgumentNumericValue - literalNumericValue.Value;
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#tan":
                                        keepRow = leftArgumentNumericValue == Math.Tan(rightArgumentNumericValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#unaryMinus":
                                        keepRow = leftArgumentNumericValue == -1 * rightArgumentNumericValue;
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#unaryPlus":
                                        keepRow = leftArgumentNumericValue == rightArgumentNumericValue;
                                        break;
                                }

                                //If the row has passed the built-in, keep it in the filtered result table
                                if (keepRow)
                                {
                                    DataRow newRow = filteredTable.NewRow();
                                    newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                                    filteredTable.Rows.Add(newRow);
                                }
                            }
                        }
                        #endregion
                    }
                    catch {  }
                }

                return filteredTable;
            }

            if (IsComparisonFilterBuiltIn || IsStringFilterBuiltIn)
            {
                RDFFilter GetBuiltInComparisonFilter(RDFQueryEnums.RDFComparisonFlavors comparisonFlavor)
                {
                    if (Arguments is SWRLVariableArgument leftArgVarGreaterThan)
                    {
                        if (RightArgument is SWRLVariableArgument rightArgVarGreaterThan)
                            return new RDFComparisonFilter(
                                comparisonFlavor, leftArgVarGreaterThan.GetVariable(), rightArgVarGreaterThan.GetVariable());
                        else if (RightArgument is SWRLIndividualArgument rightArgIdvGreaterThan)
                            return new RDFComparisonFilter(
                                comparisonFlavor, leftArgVarGreaterThan.GetVariable(), rightArgIdvGreaterThan.GetResource());
                        else if (RightArgument is SWRLLiteralArgument rightArgLitGreaterThan)
                            return new RDFComparisonFilter(
                                comparisonFlavor, leftArgVarGreaterThan.GetVariable(), rightArgLitGreaterThan.GetLiteral());
                        //Unlikely to happen
                        throw new OWLException($"Cannot evaluate comparison filter SWRLBuiltIn '{this}': it should have a variable, or an individual, or a literal as right argument");
                    }
                    //Unlikely to happen
                    throw new OWLException($"Cannot evaluate comparison filter SWRLBuiltIn '{this}': it should have a variable as left argument");
                }

                DataTable filteredTable = antecedentResults.Clone();

                //Create the most proper type of RDFFilter for builtin evaluation
                RDFFilter builtInFilter = default;
                switch (IRI)
                {
                    #region ComparisonFilter
                    case "http://www.w3.org/2003/11/swrlb#equal":
                        builtInFilter = GetBuiltInComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.EqualTo);
                        break;
                    case "http://www.w3.org/2003/11/swrlb#greaterThan":
                        builtInFilter = GetBuiltInComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.GreaterThan);
                        break;
                    case "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual":
                        builtInFilter = GetBuiltInComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.GreaterOrEqualThan); 
                        break;
                    case "http://www.w3.org/2003/11/swrlb#lessThanOrEqual":
                        builtInFilter = GetBuiltInComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.LessOrEqualThan); 
                        break;
                    case "http://www.w3.org/2003/11/swrlb#lessThan":
                        builtInFilter = GetBuiltInComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.LessThan);
                        break;
                    case "http://www.w3.org/2003/11/swrlb#notEqual":
                        builtInFilter = GetBuiltInComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.NotEqualTo); 
                        break;
                    #endregion

                    #region StringFilter
                    case "http://www.w3.org/2003/11/swrlb#contains":
                        if (Arguments is SWRLVariableArgument leftArgVarContains 
                             && RightArgument is SWRLLiteralArgument rightArgLitContains)
                            builtInFilter = new RDFRegexFilter(leftArgVarContains.GetVariable(), new Regex(rightArgLitContains.GetLiteral().Value));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a literal as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#containsIgnoreCase":
                        if (Arguments is SWRLVariableArgument leftArgVarContainsIgnoreCase 
                             && RightArgument is SWRLLiteralArgument rightArgLitContainsIgnoreCase)
                            builtInFilter = new RDFRegexFilter(leftArgVarContainsIgnoreCase.GetVariable(), new Regex(rightArgLitContainsIgnoreCase.GetLiteral().Value, RegexOptions.IgnoreCase));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a literal as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#endsWith":
                        if (Arguments is SWRLVariableArgument leftArgVarEndsWith 
                             && RightArgument is SWRLLiteralArgument rightArgLitEndsWith)
                            builtInFilter = new RDFRegexFilter(leftArgVarEndsWith.GetVariable(), new Regex($"{rightArgLitEndsWith.GetLiteral().Value}$"));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a literal as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#lowerCase":
                        if (Arguments is SWRLVariableArgument leftArgVarLowerCase
                             && RightArgument is SWRLVariableArgument rightArgVarLowerCase)
                            builtInFilter = new RDFExpressionFilter(
                                                new RDFBooleanAndExpression(
                                                    new RDFConstantExpression(RDFTypedLiteral.True),
                                                    new RDFComparisonExpression(
                                                        RDFQueryEnums.RDFComparisonFlavors.EqualTo,
                                                        new RDFVariableExpression(leftArgVarLowerCase.GetVariable()),               
                                                        new RDFLowerCaseExpression(new RDFVariableExpression(rightArgVarLowerCase.GetVariable())))));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a variable as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#matches":
                        if (Arguments is SWRLVariableArgument leftArgVarMatches
                             && RightArgument is SWRLLiteralArgument rightArgLitMatches)
                        {
                            RDFVariable varToSearch = leftArgVarMatches.GetVariable();
                            string textToSearch = rightArgLitMatches.GetLiteral().Value;

                            //This built-in may convey regex options into the Literal tag
                            if (Literal?.GetLiteral() is RDFPlainLiteral stringLiteral
                                 && !string.IsNullOrEmpty(stringLiteral.Value))
                            {
                                RegexOptions regexOptions = RegexOptions.None;
                                if (stringLiteral.Value.IndexOf('i') > -1)
                                    regexOptions |= RegexOptions.IgnoreCase;
                                if (stringLiteral.Value.IndexOf('s') > -1)
                                    regexOptions |= RegexOptions.Singleline;
                                if (stringLiteral.Value.IndexOf('m') > -1)
                                    regexOptions |= RegexOptions.Multiline;
                                if (stringLiteral.Value.IndexOf('x') > -1)
                                    regexOptions |= RegexOptions.IgnorePatternWhitespace;

                                Regex matchesRegex = new Regex(textToSearch, regexOptions);
                                builtInFilter = new RDFRegexFilter(varToSearch, matchesRegex);
                            }
                            else
                            {
                                Regex matchesRegex = new Regex(textToSearch);
                                builtInFilter = new RDFRegexFilter(varToSearch, matchesRegex);
                            }
                        }   
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a literal as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#startsWith":
                        if (Arguments is SWRLVariableArgument leftArgVarStartsWith
                             && RightArgument is SWRLLiteralArgument rightArgLitStartsWith)
                            builtInFilter = new RDFRegexFilter(leftArgVarStartsWith.GetVariable(), new Regex($"^{rightArgLitStartsWith.GetLiteral().Value}"));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a literal as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#stringConcat":
                        if (Arguments is SWRLVariableArgument leftArgVarConcat
                             && RightArgument is SWRLVariableArgument rightArgVarConcat)
                            builtInFilter = new RDFExpressionFilter(
                                                new RDFBooleanAndExpression(
                                                    new RDFConstantExpression(RDFTypedLiteral.True),
                                                    new RDFComparisonExpression(
                                                        RDFQueryEnums.RDFComparisonFlavors.EqualTo,
                                                        new RDFVariableExpression(leftArgVarConcat.GetVariable()),               
                                                        new RDFConcatExpression(
                                                            new RDFVariableExpression(rightArgVarConcat.GetVariable()),
                                                            new RDFConstantExpression(Literal?.GetLiteral() ?? new RDFPlainLiteral(null))))));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a variable as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase":
                        if (Arguments is SWRLVariableArgument leftArgVarStringEqualIgnoreCase
                             && RightArgument is SWRLVariableArgument rightArgVarStringEqualIgnoreCase)
                            builtInFilter = new RDFExpressionFilter(
                                                new RDFBooleanAndExpression(
                                                    new RDFConstantExpression(RDFTypedLiteral.True),
                                                    new RDFComparisonExpression(
                                                        RDFQueryEnums.RDFComparisonFlavors.EqualTo,
                                                        new RDFUpperCaseExpression(new RDFVariableExpression(leftArgVarStringEqualIgnoreCase.GetVariable())),               
                                                        new RDFUpperCaseExpression(new RDFVariableExpression(rightArgVarStringEqualIgnoreCase.GetVariable())))));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a variable as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#stringLength":
                        if (Arguments is SWRLVariableArgument leftArgVarLength
                             && RightArgument is SWRLVariableArgument rightArgVarLength)
                            builtInFilter = new RDFExpressionFilter(
                                                new RDFBooleanAndExpression(
                                                    new RDFConstantExpression(RDFTypedLiteral.True),
                                                    new RDFComparisonExpression(
                                                        RDFQueryEnums.RDFComparisonFlavors.EqualTo,
                                                        new RDFVariableExpression(leftArgVarLength.GetVariable()),               
                                                        new RDFLengthExpression(new RDFVariableExpression(rightArgVarLength.GetVariable())))));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a variable as right argument");
                        break;
                    case "http://www.w3.org/2003/11/swrlb#upperCase":
                        if (Arguments is SWRLVariableArgument leftArgVarUpperCase
                             && RightArgument is SWRLVariableArgument rightArgVarUpperCase)
                            builtInFilter = new RDFExpressionFilter(
                                                new RDFBooleanAndExpression(
                                                    new RDFConstantExpression(RDFTypedLiteral.True),
                                                    new RDFComparisonExpression(
                                                        RDFQueryEnums.RDFComparisonFlavors.EqualTo,
                                                        new RDFVariableExpression(leftArgVarUpperCase.GetVariable()),               
                                                        new RDFUpperCaseExpression(new RDFVariableExpression(rightArgVarUpperCase.GetVariable())))));
                        else
                            throw new OWLException($"Cannot evaluate string filter SWRLBuiltIn '{this}': it should have a variable as left argument and a variable as right argument");
                        break;
                }

                //Iterate the rows of the antecedent result table
                IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
                while (rowsEnum.MoveNext())
                {
                    //Apply the built-in filter on the row
                    bool keepRow = builtInFilter.ApplyFilter((DataRow)rowsEnum.Current, false);

                    //If the row has passed the filter, keep it in the filtered result table
                    if (keepRow)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                        filteredTable.Rows.Add(newRow);
                    }
                }

                return filteredTable;
                #endregion
            }

            throw new OWLException($"Cannot evaluate SWRLBuiltIn with unsupported predicate: {IRI}");
        }
        */
        #endregion
    }
}