﻿/*
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
using OWLSharp.Ontology.Rules.Arguments;
using OWLSharp.Ontology.Rules.Atoms;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology.Rules
{
    public static class SWRLBuiltInFactory
    {
        #region Factory

        //Math

        public static SWRLBuiltInAtom Abs(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom() 
            { 
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#abs") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        public static SWRLBuiltInAtom Add(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double addValue)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#add") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                MathValue = addValue
            };
        }

        public static SWRLBuiltInAtom Ceiling(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#ceiling") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        public static SWRLBuiltInAtom Cos(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#cos") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        public static SWRLBuiltInAtom Divide(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double divideValue)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            if (divideValue == 0d)
                throw new OWLException("Cannot create built-in because given \"divideValue\" parameter is zero");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#divide") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                MathValue = divideValue
            };
        }

        public static SWRLBuiltInAtom Floor(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#floor") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        public static SWRLBuiltInAtom Multiply(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double multiplyValue)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#multiply") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                MathValue = multiplyValue
            };
        }

        public static SWRLBuiltInAtom Pow(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double powValue)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#pow") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                MathValue = powValue
            };
        }

        public static SWRLBuiltInAtom Round(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#round") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        public static SWRLBuiltInAtom RoundHalfToEven(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#roundHalfToEven") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        public static SWRLBuiltInAtom Sin(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#sin") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        public static SWRLBuiltInAtom Subtract(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument, double subtractValue)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#subtract") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                MathValue = subtractValue
            };
        }

        public static SWRLBuiltInAtom Tan(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#tan") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument
            };
        }

        //Comparison

        public static SWRLBuiltInAtom Equal(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => EqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom Equal(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => EqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom Equal(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => EqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltInAtom EqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#equal") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                FilterValue = new RDFComparisonFilter(
                    RDFQueryEnums.RDFComparisonFlavors.EqualTo,
                    RDFQueryUtilities.ParseRDFPatternMember(leftArgument.ToString()),
                    RDFQueryUtilities.ParseRDFPatternMember(rightArgument.ToString()))
            };
        }

        public static SWRLBuiltInAtom GreaterThan(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => GreaterThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom GreaterThan(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => GreaterThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom GreaterThan(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => GreaterThanInternal(leftArgument, rightArgument);
        internal static SWRLBuiltInAtom GreaterThanInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#greaterThan") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                FilterValue = new RDFComparisonFilter(
                    RDFQueryEnums.RDFComparisonFlavors.GreaterThan,
                    RDFQueryUtilities.ParseRDFPatternMember(leftArgument.ToString()),
                    RDFQueryUtilities.ParseRDFPatternMember(rightArgument.ToString()))
            };
        }

        public static SWRLBuiltInAtom GreaterThanOrEqual(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => GreaterThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom GreaterThanOrEqual(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => GreaterThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom GreaterThanOrEqual(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => GreaterThanOrEqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltInAtom GreaterThanOrEqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#greaterThanOrEqual") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                FilterValue = new RDFComparisonFilter(
                    RDFQueryEnums.RDFComparisonFlavors.GreaterOrEqualThan,
                    RDFQueryUtilities.ParseRDFPatternMember(leftArgument.ToString()),
                    RDFQueryUtilities.ParseRDFPatternMember(rightArgument.ToString()))
            };
        }

        public static SWRLBuiltInAtom LessThan(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => LessThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom LessThan(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => LessThanInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom LessThan(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => LessThanInternal(leftArgument, rightArgument);
        internal static SWRLBuiltInAtom LessThanInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#lessThan") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                FilterValue = new RDFComparisonFilter(
                    RDFQueryEnums.RDFComparisonFlavors.LessThan,
                    RDFQueryUtilities.ParseRDFPatternMember(leftArgument.ToString()),
                    RDFQueryUtilities.ParseRDFPatternMember(rightArgument.ToString()))
            };
        }

        public static SWRLBuiltInAtom LessThanOrEqual(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => LessThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom LessThanOrEqual(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => LessThanOrEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom LessThanOrEqual(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => LessThanOrEqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltInAtom LessThanOrEqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#lessThanOrEqual") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                FilterValue = new RDFComparisonFilter(
                    RDFQueryEnums.RDFComparisonFlavors.LessOrEqualThan,
                    RDFQueryUtilities.ParseRDFPatternMember(leftArgument.ToString()),
                    RDFQueryUtilities.ParseRDFPatternMember(rightArgument.ToString()))
            };
        }

        public static SWRLBuiltInAtom NotEqual(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            => NotEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom NotEqual(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
           => NotEqualInternal(leftArgument, rightArgument);
        public static SWRLBuiltInAtom NotEqual(SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
           => NotEqualInternal(leftArgument, rightArgument);
        internal static SWRLBuiltInAtom NotEqualInternal(SWRLArgument leftArgument, SWRLArgument rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            return new SWRLBuiltInAtom()
            {
                Predicate = new OWLExpression() { ExpressionIRI = new RDFResource("http://www.w3.org/2003/11/swrlb#notEqual") },
                LeftArgument = leftArgument,
                RightArgument = rightArgument,
                FilterValue = new RDFComparisonFilter(
                    RDFQueryEnums.RDFComparisonFlavors.NotEqualTo,
                    RDFQueryUtilities.ParseRDFPatternMember(leftArgument.ToString()),
                    RDFQueryUtilities.ParseRDFPatternMember(rightArgument.ToString()))
            };
        }

        #endregion
    }
}