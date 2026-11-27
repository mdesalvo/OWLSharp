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
using System;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLIntegerDivideBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#integerDivide built-in
    /// </summary>
    internal static class SWRLIntegerDivideBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the quotient of the second and the third argument truncated toward zero
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3)
                throw new ArgumentException("it requires exactly 3 arguments");
            #endregion

            //Only numbers take part in the computation: an unbound or non-numeric operand leaves the result undefined
            if (!SWRLHelper.TryGetNumericOperands(antecedentResultsRow, builtInArguments, out SWRLNumericFamily numericFamily, out decimal[] exactOperands, out double[] approximateOperands))
                return null;

            //A division by zero has no quotient
            if (approximateOperands[1] == 0d)
                return null;

            //The quotient is truncated toward zero, as XQuery idiv does: a float operand makes the division happen in double precision first
            decimal truncatedQuotient = numericFamily == SWRLNumericFamily.Float
                ? (decimal)Math.Truncate(approximateOperands[0] / approximateOperands[1])
                : decimal.Truncate(exactOperands[0] / exactOperands[1]);

            //The truncated quotient is always an integer (beyond the decimal range it overflows and the row is dropped)
            return SWRLHelper.ToNumericLiteral(truncatedQuotient, SWRLNumericFamily.Integer);
        }
        #endregion
    }
}
