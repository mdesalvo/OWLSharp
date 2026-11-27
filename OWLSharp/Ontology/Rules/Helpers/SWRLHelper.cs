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
using System.Globalization;
using System.Linq;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLNumericFamily tells how the operands of a numeric built-in are computed and which datatype its result gets
    /// </summary>
    internal enum SWRLNumericFamily
    {
        /// <summary>
        /// Every operand is an integer: the result is computed exactly and written as xsd:integer
        /// </summary>
        Integer = 1,
        /// <summary>
        /// Some operand is a decimal and none is a float or a double: the result is computed exactly and written as xsd:decimal
        /// </summary>
        Decimal = 2,
        /// <summary>
        /// Some operand is a float or a double: the result is computed in double precision and written as xsd:double
        /// </summary>
        Float = 3
    }

    /// <summary>
    /// SWRLHelper simplifies SWRL modeling with a set of facilities
    /// </summary>
    public static class SWRLHelper
    {
        #region Methods
        /// <summary>
        /// Checks if the given ontology has the given SWRL rule in its R-BOX
        /// </summary>
        public static bool CheckHasRule(this OWLOntology ontology, SWRLRule rule)
            => ontology != null && rule != null && ontology.Rules.Any(rl => string.Equals(rl.GetXML(), rule.GetXML()));

        /// <summary>
        /// Declares the given SWRL rule to the R-BOX of the given ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static void DeclareRule(this OWLOntology ontology, SWRLRule rule)
        {
            #region Guards
            if (rule == null)
                throw new OWLException($"Cannot declare SWRL rule because given '{nameof(rule)}' parameter is null");
            #endregion

            if (!CheckHasRule(ontology, rule))
                ontology?.Rules.Add(rule);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Gets the value the given argument holds in the given row, or null when it is a variable that no atom has bound
        /// </summary>
        internal static RDFPatternMember GetArgumentValue(RDFTableRow antecedentResultsRow, SWRLArgument builtInArgument)
        {
            switch (builtInArgument)
            {
                //A variable takes its value from the row: a missing column or an empty cell means no atom has bound it
                case SWRLVariableArgument argumentVariable:
                    string variableName = argumentVariable.GetVariable().ToString();
                    return antecedentResultsRow.HasColumn(variableName) && antecedentResultsRow[variableName] != null
                        ? RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[variableName])
                        : null;
                //A literal is its own value in every row
                case SWRLLiteralArgument argumentLiteral:
                    return argumentLiteral.GetLiteral();
                //An individual is its own value in every row
                case SWRLIndividualArgument argumentIndividual:
                    return argumentIndividual.GetResource();
                //No other kind of argument can hold a value
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the text of a value given to a string built-in, or null when the value is not a string
        /// </summary>
        internal static string GetStringValue(RDFPatternMember argumentValue)
        {
            //The string built-ins read resources by their IRI and literals by their lexical form, the language tag excluded
            if (argumentValue is RDFResource || argumentValue is RDFPlainLiteral
                 || (argumentValue is RDFTypedLiteral typedValue && typedValue.HasStringDatatype()))
                return argumentValue is RDFLiteral literalValue ? literalValue.Value : argumentValue.ToString();

            //Numbers, dates and the other typed values are not strings
            return null;
        }

        /// <summary>
        /// Reads the numeric operands of a built-in, which are all its arguments but the first one, together with their family
        /// </summary>
        /// <remarks>The exact operands are null in the float family, where only the double precision operands take part in the computation.</remarks>
        internal static bool TryGetNumericOperands(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments,
            out SWRLNumericFamily numericFamily, out decimal[] exactOperands, out double[] approximateOperands)
        {
            //Operands are integers until a decimal or a float operand promotes the whole computation
            numericFamily = SWRLNumericFamily.Integer;
            exactOperands = new decimal[builtInArguments.Count - 1];
            approximateOperands = new double[builtInArguments.Count - 1];

            //The first argument is the result of the built-in, so the operands start from the second one
            for (int i = 1; i < builtInArguments.Count; i++)
            {
                //An unbound operand or a value that is not a number cannot take part in a numeric computation
                if (!(GetArgumentValue(antecedentResultsRow, builtInArguments[i]) is RDFTypedLiteral operandLiteral) || !operandLiteral.HasDecimalDatatype())
                    return false;

                //A float or a double operand moves the computation to double precision, a decimal one makes an integer computation exact in decimals
                RDFModelEnums.RDFDatatypes operandDatatype = operandLiteral.Datatype.TargetDatatype;
                bool isFloatOperand = operandDatatype == RDFModelEnums.RDFDatatypes.XSD_FLOAT || operandDatatype == RDFModelEnums.RDFDatatypes.XSD_DOUBLE;
                if (isFloatOperand)
                    numericFamily = SWRLNumericFamily.Float;
                else if (operandDatatype == RDFModelEnums.RDFDatatypes.XSD_DECIMAL && numericFamily == SWRLNumericFamily.Integer)
                    numericFamily = SWRLNumericFamily.Decimal;

                //Every operand is also read in double precision, since a later float operand can still promote the computation
                if (!double.TryParse(operandLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out approximateOperands[i - 1]))
                    return false;

                //A float operand needs no exact value: an integer or decimal one that does not fit a decimal leaves the computation undefined
                if (!decimal.TryParse(operandLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out exactOperands[i - 1]) && !isFloatOperand)
                    return false;
            }

            //In the float family only the double precision operands are meaningful
            if (numericFamily == SWRLNumericFamily.Float)
                exactOperands = null;
            return true;
        }

        /// <summary>
        /// Writes an exact numeric result as a literal of the given family
        /// </summary>
        internal static RDFTypedLiteral ToNumericLiteral(decimal exactResult, SWRLNumericFamily numericFamily)
            => numericFamily == SWRLNumericFamily.Integer
                //An integer result has no fractional part to write
                ? new RDFTypedLiteral(exactResult.ToString("0", CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_INTEGER)
                //A decimal result keeps one fractional digit at least and drops the trailing zeros, so that equal values get the same lexical form
                : new RDFTypedLiteral(exactResult.ToString("0.0############################", CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL);

        /// <summary>
        /// Writes a double precision result as a double literal, or gives null when the result is not a number
        /// </summary>
        internal static RDFTypedLiteral ToNumericLiteral(double approximateResult)
            //NaN and the infinities leave the built-in without a value to compare or to bind
            => double.IsNaN(approximateResult) || double.IsInfinity(approximateResult)
                ? null
                : new RDFTypedLiteral(approximateResult.ToString("R", CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE);
        #endregion
    }
}
